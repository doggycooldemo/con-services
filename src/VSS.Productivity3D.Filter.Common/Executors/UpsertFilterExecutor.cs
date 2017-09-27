﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VSS.Common.ResultsHandling;
using VSS.ConfigurationStore;
using VSS.KafkaConsumer.Kafka;
using VSS.MasterData.Models.Handlers;
using VSS.MasterData.Repositories;
using VSS.MasterData.Proxies.Interfaces;
using VSS.Productivity3D.Filter.Common.Models;
using VSS.Productivity3D.Filter.Common.ResultHandling;
using VSS.Productivity3D.Filter.Common.Utilities;
using VSS.VisionLink.Interfaces.Events.MasterData.Models;
using VSS.MasterData.Models.Models;

namespace VSS.Productivity3D.Filter.Common.Executors
{
  public class UpsertFilterExecutor : RequestExecutorContainer
  {
    /// <summary>
    /// This constructor allows us to mock raptorClient
    /// </summary>
    public UpsertFilterExecutor(IConfigurationStore configStore, ILoggerFactory logger,
      IServiceExceptionHandler serviceExceptionHandler,
      IProjectListProxy projectListProxy, IRaptorProxy raptorProxy,
      IFilterRepository filterRepo, IKafka producer, string kafkaTopicName)
      : base(configStore, logger, serviceExceptionHandler,
        projectListProxy, raptorProxy,
        filterRepo, producer, kafkaTopicName)
    {
    }

    /// <summary>
    /// Default constructor for RequestExecutorContainer.Build
    /// </summary>
    public UpsertFilterExecutor()
    {
    }

    protected override ContractExecutionResult ProcessEx<T>(T item)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Processes the UpsertFilter request
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <returns>a FiltersResult if successful</returns>     
    protected override async Task<ContractExecutionResult> ProcessAsyncEx<T>(T item)
    {
      ContractExecutionResult result = null;
      var filterRequest = item as FilterRequestFull;
      if (filterRequest == null)
      {
        serviceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 38);
      }

      if (string.IsNullOrEmpty(filterRequest.Name))
        result = await ProcessTransient(filterRequest).ConfigureAwait(false);
      else
        result = await ProcessPersistent(filterRequest).ConfigureAwait(false);

      return result;
    }


    private async Task<FilterDescriptorSingleResult> ProcessTransient(FilterRequestFull filterRequest)
    {
      // if filterUid supplied, then exception as cannot update a transient filter
      //   else create new one Note that can have duplicate transient name (i.e. "") per cust/prj/user
      if (!string.IsNullOrEmpty(filterRequest.FilterUid))
      {
        serviceExceptionHandler.ThrowServiceException(HttpStatusCode.BadRequest, 16);
      }

      var createdCount = 0;
      try
      {
        filterRequest.FilterUid = Guid.NewGuid().ToString();
        var filterEvent = AutoMapperUtility.Automapper.Map<CreateFilterEvent>(filterRequest);
        filterEvent.ActionUTC = DateTime.UtcNow;
        createdCount = await filterRepo.StoreEvent(filterEvent).ConfigureAwait(false);
      }
      catch (Exception e)
      {
        serviceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 20, e.Message);
      }

      if (createdCount == 0)
        serviceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 19);

      var retrievedFilter = (await filterRepo
          .GetFiltersForProjectUser(filterRequest.CustomerUid, filterRequest.ProjectUid, filterRequest.UserId, true)
          .ConfigureAwait(false))
        .OrderByDescending(f => f.LastActionedUtc)
        .FirstOrDefault(f => string.IsNullOrEmpty(f.Name));

      if (retrievedFilter == null)
        serviceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 19);

      return new FilterDescriptorSingleResult(AutoMapperUtility.Automapper.Map<FilterDescriptor>(retrievedFilter));
    }

    private async Task<FilterDescriptorSingleResult> ProcessPersistent(FilterRequestFull filterRequest)
    {
      // if filterUid supplied, and it exists for customer/user/project, then update it
      // if name exists, then exception
      // else create new filter
      // write to kafka (update or create)
      IList<MasterData.Repositories.DBModels.Filter> existingPersistentFilters =
        new List<MasterData.Repositories.DBModels.Filter>();
      try
      {
        existingPersistentFilters =
        (await filterRepo
          .GetFiltersForProjectUser(filterRequest.CustomerUid, filterRequest.ProjectUid, filterRequest.UserId)
          .ConfigureAwait(false)).ToList();
        log.LogDebug(
          $"ProcessPersistent retrieved filter count for projectUID {filterRequest.ProjectUid} of {existingPersistentFilters?.Count()}");
      }
      catch (Exception e)
      {
        serviceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 15, e.Message);
      }

      MasterData.Repositories.DBModels.Filter existingFilter = null;
      if (!string.IsNullOrEmpty(filterRequest.FilterUid))
      {
        existingFilter = existingPersistentFilters.SingleOrDefault(
          f => string.Equals(f.FilterUid, filterRequest.FilterUid, StringComparison.OrdinalIgnoreCase));
        if (existingFilter == null)
          serviceExceptionHandler.ThrowServiceException(HttpStatusCode.BadRequest, 21);

        // don't allow update to Name to a Name which already exists (for a different filterUid)
        var filterOfSameName = existingPersistentFilters
          .FirstOrDefault(f => string.Equals(f.Name, filterRequest.Name, StringComparison.OrdinalIgnoreCase) 
               && !string.Equals(f.FilterUid, filterRequest.FilterUid, StringComparison.OrdinalIgnoreCase));
        if (filterOfSameName != null)
        {
          serviceExceptionHandler.ThrowServiceException(HttpStatusCode.BadRequest, 39);
        }

        UpdateFilterEvent updateFilterEvent = null;
        try
        {
          updateFilterEvent = AutoMapperUtility.Automapper.Map<UpdateFilterEvent>(filterRequest);
          // only Name can be updated, NOT filterJson. Do this here as well as in AutoMapper, just to be sure!
          updateFilterEvent.FilterJson = existingFilter.FilterJson;
          updateFilterEvent.ActionUTC = DateTime.UtcNow;
          var updatedCount = await filterRepo.StoreEvent(updateFilterEvent).ConfigureAwait(false);
          if (updatedCount == 0)
            serviceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 17);
        }
        catch (Exception e)
        {
          serviceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 18, e.Message);
        }

        if (updateFilterEvent != null)
        {
          try
          {
            var payload = JsonConvert.SerializeObject(new {UpdateFilterEvent = updateFilterEvent});
            producer.Send(kafkaTopicName,
              new List<KeyValuePair<string, string>>
              {
                new KeyValuePair<string, string>(updateFilterEvent.FilterUID.ToString(), payload)
              });
          }
          catch (Exception e)
          {
            serviceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 26, e.Message);
          }
        }

      }
      else // create
      {
        var filterOfSameName = existingPersistentFilters
          .FirstOrDefault(f => (string.Equals(f.Name, filterRequest.Name, StringComparison.OrdinalIgnoreCase)));
        if (filterOfSameName != null)
        {
          serviceExceptionHandler.ThrowServiceException(HttpStatusCode.BadRequest, 39);
        }

        CreateFilterEvent createFilterEvent = null;
        try
        {
          filterRequest.FilterUid = Guid.NewGuid().ToString();
          createFilterEvent = AutoMapperUtility.Automapper.Map<CreateFilterEvent>(filterRequest);
          createFilterEvent.ActionUTC = DateTime.UtcNow;
          var createdCount = await filterRepo.StoreEvent(createFilterEvent).ConfigureAwait(false);
          if (createdCount == 0)
            serviceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 24);
        }
        catch (Exception e)
        {
          serviceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 25, e.Message);
        }

        if (createFilterEvent != null)
        {
          try
          {
            var payload = JsonConvert.SerializeObject(new {CreateFilterEvent = createFilterEvent});
            producer.Send(kafkaTopicName,
              new List<KeyValuePair<string, string>>
              {
                new KeyValuePair<string, string>(createFilterEvent.FilterUID.ToString(), payload)
              });
          }
          catch (Exception e)
          {
            serviceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 26, e.Message);
          }
        }
      }

      var retrievedFilter = (await filterRepo
          .GetFiltersForProjectUser(filterRequest.CustomerUid, filterRequest.ProjectUid, filterRequest.UserId)
          .ConfigureAwait(false))
          .FirstOrDefault(f => string.Equals(f.Name, filterRequest.Name, StringComparison.OrdinalIgnoreCase));

      if (retrievedFilter == null)
        serviceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 24);

      return new FilterDescriptorSingleResult(AutoMapperUtility.Automapper.Map<FilterDescriptor>(retrievedFilter));
    }

    protected override void ProcessErrorCodes()
    {
    }
  }
}