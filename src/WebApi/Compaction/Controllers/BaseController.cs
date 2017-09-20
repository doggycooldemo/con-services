﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VSS.Common.Exceptions;
using VSS.Common.ResultsHandling;
using VSS.ConfigurationStore;
using VSS.MasterData.Models.Handlers;
using VSS.MasterData.Models.Models;
using VSS.MasterData.Proxies;
using VSS.MasterData.Proxies.Interfaces;
using VSS.Productivity3D.Common.Filters.Authentication.Models;
using VSS.Productivity3D.Common.Interfaces;
using VSS.Productivity3D.Common.Models;
using VSS.VisionLink.Interfaces.Events.MasterData.Models;
using VSS.Productivity3D.WebApi.Models.Extensions;
using VSS.Productivity3D.WebApiModels.Notification.Helpers;

namespace VSS.Productivity3D.WebApi.Compaction.Controllers
{
  public abstract class BaseController : Controller
  {
    /// <summary>
    /// Logger for logging
    /// </summary>
    private readonly ILogger log;

    private readonly IServiceExceptionHandler serviceExceptionHandler;

    /// <summary>
    /// Where to get environment variables, connection string etc. from
    /// </summary>
    protected IConfigurationStore configStore;

    /// <summary>
    /// For getting list of imported files for a project
    /// </summary>
    protected readonly IFileListProxy fileListProxy;

    /// <summary>
    /// For getting project settings for a project
    /// </summary>
    protected readonly IProjectSettingsProxy projectSettingsProxy;

    /// <summary>
    /// For getting list of persistent filters for a project
    /// </summary>
    protected readonly IFilterServiceProxy filterServiceProxy;

    /// <summary>
    /// For getting compaction settings for a project
    /// </summary>
    protected readonly ICompactionSettingsManager settingsManager;

    /// <summary>
    /// Gets the custom headers for the request.
    /// </summary>
    /// <value>
    /// The custom headers.
    /// </value>
    protected IDictionary<string, string> customHeaders => Request.Headers.GetCustomHeaders();

    /// <summary>
    /// Gets the customer uid form the current context
    /// </summary>
    /// <value>
    /// The customer uid.
    /// </value>
    protected Guid customerUid => GetCustomerUid();


    protected BaseController(ILogger log, IServiceExceptionHandler serviceExceptionHandler, IConfigurationStore configStore, IFileListProxy fileListProxy,
      IProjectSettingsProxy projectSettingsProxy, IFilterServiceProxy filterServiceProxy, ICompactionSettingsManager settingsManager)
    {
      this.log = log;
      this.serviceExceptionHandler = serviceExceptionHandler;
      this.configStore = configStore;
      this.fileListProxy = fileListProxy;
      this.projectSettingsProxy = projectSettingsProxy;
      this.filterServiceProxy = filterServiceProxy;
      this.settingsManager = settingsManager;
    }

    /// <summary>
    /// With the service exception try execute.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="action">The action.</param>
    /// <returns></returns>
    protected TResult WithServiceExceptionTryExecute<TResult>(Func<TResult> action) where TResult : ContractExecutionResult
    {
      TResult result = default(TResult);
      try
      {
        result = action.Invoke();
        log.LogTrace($"Executed {action.Method.Name} with result {JsonConvert.SerializeObject(result)}");

      }
      catch (ServiceException se)
      {
        throw new ServiceException(HttpStatusCode.NoContent,
          new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError, se.Message));
      }
      catch (Exception ex)
      {
        serviceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError,
          ContractExecutionStatesEnum.InternalProcessingError - 2000, ex.Message);
      }
      finally
      {
        log.LogInformation($"Executed {action.Method.Name} with the result {result?.Code}");
      }
      return result;
    }

    /// <summary>
    /// Gets the customer uid form the context.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Incorrect customer uid value.</exception>
    private Guid GetCustomerUid()
    {
      if (User is RaptorPrincipal principal)
        return Guid.Parse(principal.CustomerUid);
      throw new ArgumentException("Incorrect request context principal.");
    }

    /// <summary>
    /// Gets the project identifier.
    /// </summary>
    /// <param name="projectUid">The project uid.</param>
    /// <returns>The project's Id for the Uid provided</returns>
    /// <exception cref="ArgumentException">Incorrect request context principal.</exception>
    protected long GetProjectId(Guid projectUid)
    {
      if (User is RaptorPrincipal principal)
      {
        return principal.GetProjectId(projectUid);
      }

      throw new ArgumentException("Incorrect request context principal.");
    }

    /// <summary>
    /// Gets the ids of the surveyed surfaces to exclude from Raptor calculations. 
    /// This is the deactivated ones.
    /// </summary>
    /// <param name="projectUid">The UID of the project containing the surveyed surfaces</param>
    /// <returns>The list of file ids for the surveyed surfaces to be excluded</returns>
    protected async Task<List<long>> GetExcludedSurveyedSurfaceIds(Guid projectUid)
    {
      var fileList = await fileListProxy.GetFiles(projectUid.ToString(), customHeaders);
      if (fileList == null || fileList.Count == 0)
      {
        return null;
      }

      var results = fileList
        .Where(f => f.ImportedFileType == ImportedFileType.SurveyedSurface && !f.IsActivated)
        .Select(f => f.LegacyFileId).ToList();

      return results;
    }

    protected async Task<DesignDescriptor> GetDesignDescriptor(Guid projectUid, Guid? fileUid, bool forProfile=false)
    {
      if (fileUid.HasValue)
      {
        var fileList = await fileListProxy.GetFiles(projectUid.ToString(), customHeaders);
        if (fileList == null || fileList.Count == 0)
        {
          throw new ServiceException(HttpStatusCode.BadRequest,
            new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
              "Project has no appropriate design files."));
        }

        var file = fileList.SingleOrDefault(
          f => f.ImportedFileUid == fileUid.ToString() &&
               f.IsActivated &&
               (!forProfile || f.IsProfileSupportedFileType()));

        if (file == null)
        {
          throw new ServiceException(HttpStatusCode.BadRequest,
            new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
              "Unable to access design file."));
        }

        var tccFileName = file.Name;
        if (file.ImportedFileType == ImportedFileType.SurveyedSurface)
        {
          //Note: ':' is an invalid character for filenames in Windows so get rid of them
          tccFileName = Path.GetFileNameWithoutExtension(tccFileName) + 
            "_" + file.SurveyedUtc.Value.ToIso8601DateTimeString().Replace(":", string.Empty) + 
            Path.GetExtension(tccFileName);
        }
        string fileSpaceId = FileDescriptor.GetFileSpaceId(configStore, log);
        FileDescriptor fileDescriptor = FileDescriptor.CreateFileDescriptor(fileSpaceId, file.Path, tccFileName);

        return DesignDescriptor.CreateDesignDescriptor(file.LegacyFileId, fileDescriptor, 0.0);
      }
      return null;
    }

    /// <summary>
    /// Gets the project settings for the project.
    /// </summary>
    /// <param name="projectUid">The UID of the project containing the surveyed surfaces</param>
    /// <returns>The project settings</returns>
    protected async Task<CompactionProjectSettings> GetProjectSettings(Guid projectUid)
    {
      CompactionProjectSettings ps;
      var jsonSettings = await projectSettingsProxy.GetProjectSettings(projectUid.ToString(), customHeaders);
      if (!string.IsNullOrEmpty(jsonSettings))
      {
        try
        {
          ps = JsonConvert.DeserializeObject<CompactionProjectSettings>(jsonSettings);
          ps.Validate();
        }
        catch (Exception ex)
        {
          log.LogInformation(
            $"Project Settings deserialization or validation failure for projectUid {projectUid}. Error is {ex.Message}");
          ps = CompactionProjectSettings.DefaultSettings;
        }
      }
      else
      {
        log.LogDebug($"No Project Settings for projectUid {projectUid}. Using defaults.");
        ps = CompactionProjectSettings.DefaultSettings;
      }
      return ps;
    }


    /// <summary>
    /// Gets the list of contributing machines from the query parameters
    /// </summary>
    /// <param name="assetId">The asset ID</param>
    /// <param name="machineName">The machine name</param>
    /// <param name="isJohnDoe">The john doe flag</param>
    /// <returns>List of machines</returns>
    protected List<MachineDetails> GetMachines(long? assetId, string machineName, bool? isJohnDoe)
    {
      MachineDetails machine = null;
      if (assetId.HasValue || !string.IsNullOrEmpty(machineName) || isJohnDoe.HasValue)
      {
        if (!assetId.HasValue || string.IsNullOrEmpty(machineName) || !isJohnDoe.HasValue)
        {
          throw new ServiceException(HttpStatusCode.BadRequest,
            new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
              "If using a machine, asset ID machine name and john doe flag must be provided"));
        }
        machine = MachineDetails.CreateMachineDetails(assetId.Value, machineName, isJohnDoe.Value);
      }
      return machine == null ? null : new List<MachineDetails> { machine };
    }

    /// <summary>
    /// Creates an instance of the Filter class and populate it with data.
    /// </summary>
    /// <param name="projectUid">Project Uid</param>
    /// <param name="startUtc">Start date and time in UTC</param>
    /// <param name="endUtc">End date and time in UTC</param>
    /// <param name="vibeStateOn">Only filter cell passes recorded when the vibratory drum was 'on'.  
    /// If set to null, returns all cell passes. If true, returns only cell passes with the cell pass parameter and the drum was on.  
    /// If false, returns only cell passes with the cell pass parameter and the drum was off.</param>
    /// <param name="elevationType">Controls the cell pass from which to determine data based on its elevation.</param>
    /// <param name="layerNumber"> The number of the 3D spatial layer (determined through bench elevation and layer thickness or the tag file)
    ///  to be used as the layer type filter. Layer 3 is then the third layer from the
    /// datum elevation where each layer has a thickness defined by the layerThickness member.</param>
    /// <param name="onMachineDesignId">A machine reported design. Cell passes recorded when a machine did not have this design loaded at the time is not considered.
    /// May be null/empty, which indicates no restriction.</param>
    /// <param name="assetID">A machine is identified by its asset ID, machine name and john doe flag, indicating if the machine is known in VL.
    /// All three parameters must be specified to specify a machine. 
    /// Cell passes are only considered if the machine that recorded them is this machine. May be null/empty, which indicates no restriction.</param>
    /// <param name="machineName">See assetID</param>
    /// <param name="isJohnDoe">See assetID</param>
    /// <returns>An instance of the Filter class.</returns>
    protected async Task<Common.Models.Filter> GetCompactionFilter(Guid projectUid, Guid? filterUid, DateTime? startUtc, DateTime? endUtc, bool? vibeStateOn, ElevationType? elevationType,
      int? layerNumber, long? onMachineDesignId, long? assetID, string machineName, bool? isJohnDoe)
    {
      var excludedIds = await GetExcludedSurveyedSurfaceIds(projectUid);

      var startTimeUTC = startUtc;
      var endTimeUTC = endUtc;
      var onMachineDesignID = onMachineDesignId;
      var vibrationStateOn = vibeStateOn;
      var elevationTypeEnum = elevationType;
      var layerNo = layerNumber;
      var machines = GetMachines(assetID, machineName, isJohnDoe);

      DesignDescriptor designDescriptor = null;

      if (filterUid.HasValue)
      {
        var filterData = await GetFilter(projectUid, filterUid.Value);

        if (filterData != null)
        {
          startTimeUTC = filterData.startUTC;
          endTimeUTC = filterData.endUTC;
          onMachineDesignID = filterData.onMachineDesignID;
          vibrationStateOn = filterData.vibeStateOn;
          elevationTypeEnum = filterData.elevationType;
          layerNo = filterData.layerNumber;
          machines = filterData.contributingMachines;

          Guid designUidGuid;
          if (filterData.designUid != null && Guid.TryParse(filterData.designUid, out designUidGuid))
            designDescriptor = await GetDesignDescriptor(projectUid, designUidGuid);
        }
      }

      return settingsManager.CompactionFilter(startTimeUTC, endTimeUTC, onMachineDesignID, vibrationStateOn, elevationTypeEnum, layerNo, machines, excludedIds, designDescriptor);
    }

    private async Task<MasterData.Models.Models.Filter> GetFilter(Guid projectUid, Guid filterUid)
    {
      var filterDescriptor = await filterServiceProxy.GetFilter(projectUid.ToString(), filterUid.ToString(), customHeaders);

      if (filterDescriptor == null)
        return null;

      return JsonConvert.DeserializeObject<MasterData.Models.Models.Filter>(filterDescriptor.FilterJson);
    }

  }
}