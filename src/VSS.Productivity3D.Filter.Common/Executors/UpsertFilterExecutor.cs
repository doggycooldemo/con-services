﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VSS.ConfigurationStore;
using VSS.KafkaConsumer.Kafka;
using VSS.MasterData.Repositories;
using VSS.MasterData.Proxies.Interfaces;
using VSS.Productivity3D.Filter.Common.Models;
using VSS.Productivity3D.Filter.Common.ResultHandling;
using VSS.Productivity3D.Filter.Common.Utilities;
using VSS.VisionLink.Interfaces.Events.MasterData.Models;
using VSS.Productivity3D.Filter.Common.Internal;

namespace VSS.Productivity3D.Filter.Common.Executors
{
  public class UpsertFilterExecutor : RequestExecutorContainer
  {
    /// <summary>
    /// This constructor allows us to mock raptorClient
    /// </summary>
    public UpsertFilterExecutor(IConfigurationStore configStore, ILoggerFactory logger,
      IServiceExceptionHandler serviceExceptionHandler, IProjectListProxy projectListProxy,
      IFilterRepository filterRepo, IKafka producer, string kafkaTopicName) : base(configStore, logger,
      serviceExceptionHandler, projectListProxy, filterRepo, producer, kafkaTopicName)
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
      try
      {
        var filterRequest = item as FilterRequestFull;
        if (filterRequest != null)
        {
          // todo getFiltersForProject(Cust,User,Project
          // todo make ID primary and do exists - for performance
          var projectFilters =
            (await filterRepo.GetFiltersForProject(filterRequest.projectUid).ConfigureAwait(false))
            .Where(f => f.CustomerUid == filterRequest.customerUid && f.UserUid == filterRequest.userUid).ToList();
          log.LogDebug(
            $"UpsertFilter retrieved filter count for projectUID {filterRequest.projectUid} of {projectFilters?.Count()}");

          if (string.IsNullOrEmpty(filterRequest.name))
            result = await ProcessTransient(filterRequest, projectFilters).ConfigureAwait(false);
          else
            result = await ProcessPersistant(filterRequest, projectFilters).ConfigureAwait(false);

          return result;
        }
      }
      catch (Exception e)
      {
        serviceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 15, e.Message);
      }
      return result;
    }


    protected override void ProcessErrorCodes()
    {
    }

    private async Task<FilterDescriptorSingleResult> ProcessTransient(FilterRequestFull filterRequest,
      IList<MasterData.Repositories.DBModels.Filter> projectFilters)
    {

      // if filterUid supplied, and it exists for customer/user/project, and name is empty, then update it. 
      //   else if one exists for the UserUid, and name is empty, then update it.
      MasterData.Repositories.DBModels.Filter filter = null;
      if (!string.IsNullOrEmpty(filterRequest.filterUid))
      {
        filter = projectFilters?.SingleOrDefault(
          f => string.Equals(f.FilterUid, filterRequest.filterUid, StringComparison.OrdinalIgnoreCase) 
               && string.IsNullOrEmpty(f.Name));
        if (filter == null)
          serviceExceptionHandler.ThrowServiceException(HttpStatusCode.BadRequest, 16);
      }

      if (filter == null)
        filter = projectFilters?.SingleOrDefault(f => string.IsNullOrEmpty(f.Name));

      if (filter != null) // going to update it
      {
        try
        {
          var filterEvent = AutoMapperUtility.Automapper.Map<UpdateFilterEvent>(filterRequest);
          filterEvent.FilterUID = Guid.Parse(filter.FilterUid);
          filterEvent.ActionUTC = DateTime.UtcNow;
          var updatedCount = await filterRepo.StoreEvent(filterEvent).ConfigureAwait(false);
          if (updatedCount == 0)
          {
            // error trying to update a transient filter
            serviceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 17);
          }
        }
        catch (Exception e)
        {
          // exception trying to update a transient filter
          serviceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 18, e.Message);
        }
      }
      else // (filter == null)
      {
        try
        {
          var filterEvent = AutoMapperUtility.Automapper.Map<CreateFilterEvent>(filterRequest);
          filterEvent.FilterUID = Guid.NewGuid();
          filterEvent.ActionUTC = DateTime.UtcNow;
          var createdCount = await filterRepo.StoreEvent(filterEvent).ConfigureAwait(false);
          if (createdCount == 0)
          {
            // error trying to create a transient filter
            serviceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 19);
          }
        }
        catch (Exception e)
        {
          // exception trying to create a transient filter
          serviceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 20, e.Message);
        }
      }

      var retrievedFilter = (await filterRepo.GetFiltersForProject(filterRequest.projectUid).ConfigureAwait(false)).SingleOrDefault(f => string.IsNullOrEmpty(f.Name));
      return new FilterDescriptorSingleResult(AutoMapperUtility.Automapper.Map<FilterDescriptor>(retrievedFilter));
    }

    private async Task<FilterDescriptorSingleResult> ProcessPersistant(FilterRequestFull filterRequest,
      IList<MasterData.Repositories.DBModels.Filter> projectFilters)
    {
      // if filterUid supplied, and it exists for customer/user/project, and name is NOT empty, then delete it.
      // if old name exists, then delete it.
      // now create new filter
      // write to kafka (possible delete and the create)
      MasterData.Repositories.DBModels.Filter filter = null;
      if (!string.IsNullOrEmpty(filterRequest.filterUid))
      {
        filter = projectFilters?.SingleOrDefault(
          f => string.Equals(f.FilterUid, filterRequest.filterUid, StringComparison.OrdinalIgnoreCase) 
               && !string.IsNullOrEmpty(f.Name));
        if (filter == null)
          serviceExceptionHandler.ThrowServiceException(HttpStatusCode.BadRequest, 21);
      }

      if (filter == null)
        filter = projectFilters?.SingleOrDefault(f => f.Name == filterRequest.name);

      DeleteFilterEvent deleteFilterEvent = null;
      if (filter != null) // going to delete it
      {
        try
        {
          deleteFilterEvent = AutoMapperUtility.Automapper.Map<DeleteFilterEvent>(filterRequest);
          deleteFilterEvent.FilterUID = Guid.Parse(filter.FilterUid);
          deleteFilterEvent.ActionUTC = DateTime.UtcNow;
          var deletedCount = await filterRepo.StoreEvent(deleteFilterEvent).ConfigureAwait(false);
          if (deletedCount == 0)
          {
            // error trying to delete a persistant filter
            serviceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 22);
          }
        }
        catch (Exception e)
        {
          // exception trying to delete a persistant filter
          serviceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 23, e.Message);
        }
      }
      
      // Create new filter
      CreateFilterEvent createFilterEvent = null;
      try
      {
        createFilterEvent = AutoMapperUtility.Automapper.Map<CreateFilterEvent>(filterRequest);
        createFilterEvent.FilterUID = Guid.NewGuid();
        createFilterEvent.ActionUTC = DateTime.UtcNow;
        var createdCount = await filterRepo.StoreEvent(createFilterEvent).ConfigureAwait(false);
        if (createdCount == 0)
        {
          // error trying to create a persistant filter
          serviceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 24);
        }
      }
      catch (Exception e)
      {
        // exception trying to create a persistant filter
        serviceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 25, e.Message);
      }

      var retrievedFilter = (await filterRepo.GetFiltersForProject(filterRequest.projectUid).ConfigureAwait(false)).SingleOrDefault(f => f.Name == filterRequest.name);
      if (retrievedFilter != null)
        WriteToKafka(deleteFilterEvent, createFilterEvent);

      return new FilterDescriptorSingleResult(AutoMapperUtility.Automapper.Map<FilterDescriptor>(retrievedFilter));
    }

    private void WriteToKafka(DeleteFilterEvent deleteFilterEvent, CreateFilterEvent createFilterEvent)
    {
      try
      {
        if (deleteFilterEvent != null)
        {
          var messagePayloadDeleteEvent =
            JsonConvert.SerializeObject(new { DeleteFilterEvent = deleteFilterEvent });
          producer.Send(kafkaTopicName,
            new List<KeyValuePair<string, string>>
            {
              new KeyValuePair<string, string>(deleteFilterEvent.FilterUID.ToString(), messagePayloadDeleteEvent)
            });
        }

        var messagePayloadCreateEvent =
          JsonConvert.SerializeObject(new { CreateFilterEvent = createFilterEvent });
        producer.Send(kafkaTopicName,
          new List<KeyValuePair<string, string>>
          {
            new KeyValuePair<string, string>(createFilterEvent.FilterUID.ToString(), messagePayloadCreateEvent)
          });
      }
      catch (Exception e)
      {
        serviceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 26, e.Message);
      }
    }

  }
}