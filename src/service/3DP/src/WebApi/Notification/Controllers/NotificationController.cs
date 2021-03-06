﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VSS.Common.Abstractions.Cache.Interfaces;
using VSS.Common.Abstractions.Configuration;
using VSS.Common.Exceptions;
using VSS.MasterData.Models.Models;
using VSS.MasterData.Models.ResultHandling.Abstractions;
using VSS.MasterData.Proxies;
using VSS.MasterData.Proxies.Interfaces;
using VSS.Productivity3D.Common.Filters.Authentication;
using VSS.Productivity3D.Common.Filters.Authentication.Models;
using VSS.Productivity3D.Filter.Abstractions.Interfaces;
using VSS.Productivity3D.Productivity3D.Models.Notification.ResultHandling;
using VSS.Productivity3D.Project.Abstractions.Interfaces;
using VSS.Productivity3D.Project.Abstractions.Models;
#if RAPTOR
using VSS.Productivity3D.WebApi.Models.Notification.Executors;
#endif
using VSS.Visionlink.Interfaces.Events.MasterData.Models;

namespace VSS.Productivity3D.WebApi.Notification.Controllers
{
  /// <summary>
  /// 
  /// </summary>
  [Obsolete("Use Push Service instead, as it supports horizontal scaling")]
  [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
  public class NotificationController : Controller
  {
#if RAPTOR
    /// <summary>
    /// Raptor client for use by executor
    /// </summary>
    private readonly IASNodeClient raptorClient;
#endif
    /// <summary>
    /// LoggerFactory for logging
    /// </summary>
    private readonly ILogger log;

    /// <summary>
    /// LoggerFactory factory for use by executor
    /// </summary>
    private readonly ILoggerFactory logger;

    /// <summary>
    /// Where to get environment variables, connection string etc. from
    /// </summary>
    private readonly IConfigurationStore configStore;

    /// <summary>
    /// For getting imported files for a project
    /// </summary>
    private readonly IFileImportProxy fileImportProxy;

    /// <summary>
    /// For getting filter by uid. Used here so FilterService can clear an item from cache.
    /// </summary>
    private readonly IFilterServiceProxy filterServiceProxy;

    /// <summary>
    /// User preferences interface
    /// </summary>
    private readonly IPreferenceProxy userPreferences;

    /// <summary>
    /// Project list proxy
    /// </summary>
    private readonly IProjectProxy projectProxy;

    private readonly IDataCache dataCache;

    /// <summary>
    /// Gets the users email address from the context.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Incorrect email address value.</exception>
    private string GetUserEmailAddress()
    {
      if (User is RaptorPrincipal principal)
      {
        return principal.UserEmail;
      }

      throw new ArgumentException("Incorrect user email address in request context principal.");
    }

    /// <summary>
    /// Gets the userEmailAddress from the current context
    /// </summary>
    /// <value>
    /// The userEmailAddress.
    /// </value>
    protected string userEmailAddress => GetUserEmailAddress();

    /// <summary>
    /// Constructor with injection
    /// </summary>
    public NotificationController(
#if RAPTOR
      IASNodeClient raptorClient,
#endif
      ILoggerFactory logger, IConfigurationStore configStore,
      IPreferenceProxy prefProxy, IFileImportProxy fileImportProxy,
      IFilterServiceProxy filterServiceProxy, IProjectProxy projectProxy, IDataCache dataCache)
    {
#if RAPTOR
      this.raptorClient = raptorClient;
#endif
      this.logger = logger;
      log = logger.CreateLogger<NotificationController>();
      this.configStore = configStore;
      this.fileImportProxy = fileImportProxy;
      this.filterServiceProxy = filterServiceProxy;
      this.dataCache = dataCache;
      userPreferences = prefProxy;
      this.projectProxy = projectProxy;
    }

    /// <summary>
    /// Notifies Raptor that a file has been added to a project
    /// </summary>
    /// <param name="projectUid">Project UID</param>
    /// <param name="fileUid">File UID</param>
    /// <param name="fileDescriptor">File descriptor in JSON format. Currently this is TCC filespaceId, path and filename</param>
    /// <param name="fileType">Type of the file</param>
    /// <param name="fileId">A unique file identifier</param>
    /// <param name="dxfUnitsType">A DXF file units type</param>
    [ProjectVerifier]
    [Route("api/v2/notification/addfile")]
    [HttpGet]
    public async Task<AddFileResult> GetAddFile(
      [FromQuery] Guid projectUid,
      [FromQuery] ImportedFileType fileType,
      [FromQuery] Guid fileUid,
      [FromQuery] string fileDescriptor,
      [FromQuery] long fileId,
      [FromQuery] DxfUnitsType dxfUnitsType)
    {
      log.LogDebug($"{nameof(GetAddFile)}: " + Request.QueryString);
      var customHeaders = Request.Headers.GetCustomHeaders();

      //Do we need to validate fileUid ?
      await ClearFilesCaches(projectUid, customHeaders);
      dataCache.RemoveByTag(projectUid.ToString());
      log.LogInformation($"{nameof(GetAddFile)} returned: " + Response.StatusCode);
      return new AddFileResult(ContractExecutionStatesEnum.ExecutedSuccessfully, "Add file notification successful");
    }

    /// <summary>
    /// Notifies Raptor that a file has been deleted from a project
    /// </summary> 
    [ProjectVerifier]
    [Route("api/v2/notification/deletefile")]
    [HttpGet]
    public async Task<ContractExecutionResult> GetDeleteFile(
      [FromQuery] Guid projectUid,
      [FromQuery] ImportedFileType fileType,
      [FromQuery] Guid fileUid,
      [FromQuery] string fileDescriptor,
      [FromQuery] long fileId,
      [FromQuery] long? legacyFileId
    )
    {
      log.LogDebug($"{nameof(GetDeleteFile)}: " + Request.QueryString);

      //Cannot delete a design or alignment file that is used in a filter
      //TODO: When scheduled reports are implemented, extend this check to them as well.
      if (fileType == ImportedFileType.DesignSurface || fileType == ImportedFileType.Alignment || fileType == ImportedFileType.ReferenceSurface)
      {
        var filters = await GetFilters(projectUid, Request.Headers.GetCustomHeaders());
        if (filters != null)
        {
          var fileUidStr = fileUid.ToString();
          if (filters.Any(f => f.DesignUid == fileUidStr || f.AlignmentUid == fileUidStr))
          {
            throw new ServiceException(HttpStatusCode.BadRequest,
              new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
                "Cannot delete a design surface, reference surface or alignment file used in a filter"));
          }
        }
      }
      return new ContractExecutionResult(ContractExecutionStatesEnum.ExecutedSuccessfully, "Delete file notification successful");
    }


    /// <summary>
    /// Notifies Raptor that files have been activated or deactivated
    /// </summary>
    [ProjectVerifier]
    [Route("api/v2/notification/updatefiles")]
    [HttpGet]
    public async Task<ContractExecutionResult> GetUpdateFiles(
      [FromQuery] Guid projectUid,
      [FromQuery] Guid[] fileUids)
    {
      log.LogDebug($"{nameof(GetUpdateFiles)}: " + Request.QueryString);
      if (projectUid == Guid.Empty)
      {
        throw new ServiceException(HttpStatusCode.BadRequest,
          new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
            "Missing projectUid parameter"));
      }
      if (fileUids.Length == 0)
      {
        throw new ServiceException(HttpStatusCode.BadRequest,
          new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
            "Missing fileUids parameter"));
      }
      var customHeaders = Request.Headers.GetCustomHeaders();
      await ClearFilesCaches(projectUid, customHeaders);
      dataCache.RemoveByTag(projectUid.ToString());
      log.LogInformation($"{nameof(GetUpdateFiles)} returned: " + Response.StatusCode);
      return new ContractExecutionResult(ContractExecutionStatesEnum.ExecutedSuccessfully, "Update files notification successful");
    }

    /// <summary>
    /// Dumps cache in the ResponseCache and Masterdata cache for a project
    /// </summary>
    /// <param name="projectUid"></param>
    /// <returns></returns>
    [ProjectVerifier]
    [Route("api/v2/notification/invalidatecache")]
    [HttpGet]
    public async Task<ContractExecutionResult> InvalidateCache([FromQuery] Guid projectUid)
    {
      var customHeaders = Request.Headers.GetCustomHeaders();
      if (!customHeaders.ContainsKey("X-VisionLink-ClearCache"))
        customHeaders.Add("X-VisionLink-ClearCache", "true");
      await projectProxy.GetProjectForCustomer(((RaptorPrincipal)User).CustomerUid, projectUid.ToString(),
        customHeaders);
      dataCache.RemoveByTag(projectUid.ToString());
      return new ContractExecutionResult();
    }

    /// <summary>
    /// Notifies Raptor that a file has been CRUD to a project via CGen
    ///      This is called by the SurveyedSurface sync during Lift and Shift/Beta period.
    ///      When a file is added via CGen flexGateway, it will tell raptor.
    ///        However the 3dp UI needs to know about the change, so needs to refresh its caches.
    /// </summary>
    [ProjectVerifier]
    [Route("api/v2/notification/importedfilechange")]
    [HttpGet]
    public async Task<ContractExecutionResult> GetNotifyImportedFileChange(
      [FromQuery] Guid projectUid,
      [FromQuery] Guid fileUid)
    {
      log.LogDebug($"{nameof(GetNotifyImportedFileChange)}: " + Request.QueryString);
      var customHeaders = Request.Headers.GetCustomHeaders();
      await ClearFilesCaches(projectUid, customHeaders);
      dataCache.RemoveByTag(projectUid.ToString());
      log.LogInformation($"{nameof(GetNotifyImportedFileChange)} returned");
      return new ContractExecutionResult();
    }

    /// <summary>
    /// Notifies Raptor that a filterUid has been updated/deleted so clear it from the queue
    /// </summary>
    [Route("api/v2/notification/filterchange")]
    [HttpGet]
    public ContractExecutionResult GetNotifyFilterChange(
      [FromQuery] Guid filterUid, [FromQuery] Guid projectUid)
    {
      log.LogDebug($"{nameof(GetNotifyFilterChange)}: " + Request.QueryString);
      filterServiceProxy.ClearCacheItem(filterUid.ToString());
      filterServiceProxy.ClearCacheListItem(projectUid.ToString());
      dataCache.RemoveByTag(projectUid.ToString());
      dataCache.RemoveByTag(filterUid.ToString());
      log.LogInformation($"{nameof(GetNotifyFilterChange)} returned");
      return new ContractExecutionResult();
    }

    /// <summary>
    /// Clears the imported files cache in the proxy so that linework tile requests are refreshed appropriately
    /// </summary>
    private async Task<List<FileData>> ClearFilesCaches(Guid projectUid, IHeaderDictionary customHeaders)
    {
      log.LogInformation("Clearing imported files cache for project {0}", projectUid);
      //Clear file list cache and reload
      if (!customHeaders.ContainsKey("X-VisionLink-ClearCache"))
        customHeaders.Add("X-VisionLink-ClearCache", "true");

      dataCache.RemoveByTag(projectUid.ToString());

      var fileList = await fileImportProxy.GetFiles(projectUid.ToString(), GetUserId(), customHeaders);
      log.LogInformation("After clearing cache {0} total imported files, {1} activated, for project {2}", fileList.Count, fileList.Count(f => f.IsActivated), projectUid);

      return fileList;
    }

    /// <summary>
    /// Deserializes the file descriptor
    /// </summary>
    private FileDescriptor GetFileDescriptor(string fileDescriptor)
    {
      FileDescriptor fileDes;
      try
      {
        fileDes = JsonConvert.DeserializeObject<FileDescriptor>(fileDescriptor);
      }
      catch (Exception ex)
      {
        throw new ServiceException(HttpStatusCode.BadRequest,
          new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
            ex.Message));
      }
      return fileDes;
    }

    /// <summary>
    /// Get the list of filters for the project
    /// </summary>
    private async Task<List<Filter.Abstractions.Models.Filter>> GetFilters(Guid projectUid, IHeaderDictionary customHeaders)
    {
      var filterDescriptors = await filterServiceProxy.GetFilters(projectUid.ToString(), customHeaders);
      if (filterDescriptors == null || filterDescriptors.Count == 0)
        return null;

      return filterDescriptors.Select(f => JsonConvert.DeserializeObject<Filter.Abstractions.Models.Filter>(f.FilterJson)).ToList();
    }

    /// <summary>
    /// Gets the User uid/applicationID from the context.
    /// </summary>
    private string GetUserId()
    {
      if (User is RaptorPrincipal principal && (principal.Identity is GenericIdentity identity))
        return identity.Name;

      throw new ArgumentException("Incorrect UserId in request context principal.");
    }
  }
}
