﻿using System;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VSS.Common.Abstractions.Configuration;
using VSS.MasterData.Models.Models;
using VSS.MasterData.Project.WebAPI.Common.Executors;
using VSS.MasterData.Project.WebAPI.Common.Helpers;
using VSS.MasterData.Project.WebAPI.Common.Internal;
using VSS.MasterData.Project.WebAPI.Common.Models;
using VSS.MasterData.Project.WebAPI.Common.Utilities;
using VSS.MasterData.Proxies;
using VSS.Productivity.Push.Models.Notifications.Changes;
using VSS.Productivity3D.Push.Abstractions.Notifications;
using VSS.Productivity3D.Scheduler.Abstractions;
using VSS.Productivity3D.Scheduler.Models;
using VSS.Visionlink.Interfaces.Core.Events.MasterData.Models;
using VSS.VisionLink.Interfaces.Events.MasterData.Models;

namespace VSS.MasterData.Project.WebAPI.Controllers
{
  /// <summary>
  /// Project controller v4
  /// </summary>
  public class ProjectV4Controller : ProjectBaseController
  {
    /// <summary>
    /// Gets or sets the httpContextAccessor.
    /// </summary>
    protected readonly IHttpContextAccessor HttpContextAccessor;

    private readonly INotificationHubClient notificationHubClient;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public ProjectV4Controller(IConfigurationStore configStore, IHttpContextAccessor httpContextAccessor, INotificationHubClient notificationHubClient)
      : base(configStore)
    {
      this.HttpContextAccessor = httpContextAccessor;
      this.notificationHubClient = notificationHubClient;
    }

    #region projects

    /// <summary>
    /// Gets a list of projects for a customer. The list includes projects of all project types,
    ///    and both active and archived projects.
    /// </summary>
    /// <param name="includeLandfill">Obsolete</param>
    /// <returns>A list of projects</returns>
    [Route("api/v4/project")]
    [HttpGet]
    public async Task<ProjectV4DescriptorsListResult> GetProjectsV4([FromQuery] bool? includeLandfill)
    {
      // Note: includeLandfill is obsolete, but not worth the grief up creating a new endpoint.

      return await GetAllProjectsV4();
    }

    /// <summary>
    /// Gets a list of projects for a customer. The list includes projects of all project types
    ///        and both active and archived projects.
    /// </summary>
    /// <returns>A list of projects</returns>
    [Route("api/v4/project/all")]
    [HttpGet]
    public async Task<ProjectV4DescriptorsListResult> GetAllProjectsV4()
    {
      Logger.LogInformation("GetAllProjectsV4");

      var projects = (await GetProjectList().ConfigureAwait(false)).ToImmutableList();

      return new ProjectV4DescriptorsListResult
      {
        ProjectDescriptors = projects.Select(project =>
            AutoMapperUtility.Automapper.Map<ProjectV4Descriptor>(project))
          .ToImmutableList()
      };
    }

    /// <summary>
    /// Gets a project for a customer. 
    /// </summary>
    /// <returns>A project data</returns>
    [Route("api/v4/project/{projectUid}")]
    [HttpGet]
    public async Task<ProjectV4DescriptorsSingleResult> GetProjectV4(string projectUid)
    {
      Logger.LogInformation("GetProjectV4");
     
      var project = await ProjectRequestHelper.GetProject(projectUid.ToString(), customerUid, Logger, ServiceExceptionHandler, ProjectRepo).ConfigureAwait(false);
      return new ProjectV4DescriptorsSingleResult(AutoMapperUtility.Automapper.Map<ProjectV4Descriptor>(project));
    }

    // POST: api/project
    /// <summary>
    /// Create Project
    ///    as of v4 this creates a project AND the association to Customer
    /// </summary>
    /// <param name="projectRequest">CreateProjectRequest model</param>
    /// <remarks>Create new project</remarks>
    /// <response code="200">Ok</response>
    /// <response code="400">Bad request</response>
    [Route("internal/v4/project")]
    [Route("api/v4/project")]
    [HttpPost]
    public async Task<ProjectV4DescriptorsSingleResult> CreateProjectV4([FromBody] CreateProjectRequest projectRequest)
    {
      if (projectRequest == null)
      {
        ServiceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 39);
      }

      Logger.LogInformation("CreateProjectV4. projectRequest: {0}", JsonConvert.SerializeObject(projectRequest));

      if (string.IsNullOrEmpty(projectRequest.CustomerUID)) projectRequest.CustomerUID = customerUid;
      
      // todoMaverick, no, we need the trn from WM
      if (projectRequest.ProjectUID == null) projectRequest.ProjectUID = Guid.NewGuid().ToString();

      var createProjectEvent = AutoMapperUtility.Automapper.Map<CreateProjectEvent>(projectRequest);
      createProjectEvent.ReceivedUTC = createProjectEvent.ActionUTC = DateTime.UtcNow;
      ProjectDataValidator.Validate(createProjectEvent, ProjectRepo, ServiceExceptionHandler);
      if (createProjectEvent.CustomerUID != customerUid)
      {
        ServiceExceptionHandler.ThrowServiceException(HttpStatusCode.BadRequest, 18);
      }
      await ProjectDataValidator.ValidateProjectName(customerUid, createProjectEvent.ProjectName, createProjectEvent.ProjectUID.ToString(), Logger, ServiceExceptionHandler, ProjectRepo);

      await WithServiceExceptionTryExecuteAsync(() =>
        RequestExecutorContainerFactory
          .Build<CreateProjectExecutor>(LoggerFactory, ConfigStore, ServiceExceptionHandler,
            customerUid, userId, null, customHeaders,
            productivity3dV1ProxyCoord: Productivity3dV1ProxyCoord,
            projectRepo: ProjectRepo, fileRepo: FileRepo,
            dataOceanClient: DataOceanClient, authn: Authorization)
          .ProcessAsync(createProjectEvent)
      );

      var result = new ProjectV4DescriptorsSingleResult(
        AutoMapperUtility.Automapper.Map<ProjectV4Descriptor>(await ProjectRequestHelper.GetProject(createProjectEvent.ProjectUID.ToString(), customerUid, Logger, ServiceExceptionHandler, ProjectRepo)
          .ConfigureAwait(false)));

      await notificationHubClient.Notify(new CustomerChangedNotification(projectRequest.CustomerUID));

      Logger.LogResult(this.ToString(), JsonConvert.SerializeObject(projectRequest), result);
      return result;
    }

    /// <summary>
    /// Create a scheduler job to create a project using internal urls 
    /// </summary>
    /// <param name="projectRequest">The project request model to be used</param>
    /// <param name="scheduler">The scheduler used to queue the job</param>
    /// <returns>Scheduler Job Result, containing the Job ID To poll via the Scheduler</returns>
    [Route("api/v4/project/background")]
    [HttpPost]
    public async Task<ScheduleJobResult> RequestCreateProjectBackgroundJob([FromBody] CreateProjectRequest projectRequest, [FromServices] ISchedulerProxy scheduler)
    {
      if (projectRequest == null)
      {
        ServiceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 39);
      }
      
      var baseUrl = Request.Host.ToUriComponent(); 
      var callbackUrl = $"http://{baseUrl}/internal/v4/project";  
      Logger.LogInformation($"nameof(RequestCreateProjectBackgroundJob): baseUrl {callbackUrl}");

      var request = new ScheduleJobRequest
      {
        Filename = projectRequest.ProjectName + Guid.NewGuid(), // Make sure the filename is unique, it's not important what it's called as the scheduled job keeps a reference
        Method = "POST",
        Url = callbackUrl,
        Headers =
        {
          ["Content-Type"] = Request.Headers["Content-Type"]
        }
      };
      request.SetBinaryPayload(Request.Body);

      var customHeaders = Request.Headers.GetCustomHeaders();

      return await scheduler.ScheduleBackgroundJob(request, customHeaders);
    }

    // PUT: api/v4/project
    /// <summary>
    /// Update Project
    /// </summary>
    /// <param name="projectRequest">UpdateProjectRequest model</param>
    /// <remarks>Updates existing project</remarks>
    /// <response code="200">Ok</response>
    /// <response code="400">Bad request</response>
    [Route("internal/v4/project")]
    [Route("api/v4/project")]
    [HttpPut]
    public async Task<ProjectV4DescriptorsSingleResult> UpdateProjectV4([FromBody] UpdateProjectRequest projectRequest)
    {
      if (projectRequest == null)
      {
        ServiceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 40);
      }

      Logger.LogInformation("UpdateProjectV4. projectRequest: {0}", JsonConvert.SerializeObject(projectRequest));
      var project = AutoMapperUtility.Automapper.Map<UpdateProjectEvent>(projectRequest);
      project.ReceivedUTC = project.ActionUTC = DateTime.UtcNow;

      // validation includes check that project must exist - otherwise there will be a null legacyID.
      ProjectDataValidator.Validate(project, ProjectRepo, ServiceExceptionHandler);
      await ProjectDataValidator.ValidateProjectName(customerUid, projectRequest.ProjectName, projectRequest.ProjectUid.ToString(), Logger, ServiceExceptionHandler, ProjectRepo);

      await WithServiceExceptionTryExecuteAsync(() =>
        RequestExecutorContainerFactory
          .Build<UpdateProjectExecutor>(LoggerFactory, ConfigStore, ServiceExceptionHandler,
            customerUid, userId, null, customHeaders,
            productivity3dV1ProxyCoord: Productivity3dV1ProxyCoord,
            projectRepo: ProjectRepo, fileRepo: FileRepo, httpContextAccessor: HttpContextAccessor,
            dataOceanClient: DataOceanClient, authn: Authorization)
          .ProcessAsync(project)
      );

      //invalidate cache in TRex/Raptor
      Logger.LogInformation("UpdateProjectV4. Invalidating 3D PM cache");
      await notificationHubClient.Notify(new ProjectChangedNotification(project.ProjectUID));

      Logger.LogInformation("UpdateProjectV4. Completed successfully");
      return new ProjectV4DescriptorsSingleResult(
        AutoMapperUtility.Automapper.Map<ProjectV4Descriptor>(await ProjectRequestHelper.GetProject(project.ProjectUID.ToString(), customerUid, Logger, ServiceExceptionHandler, ProjectRepo)
          .ConfigureAwait(false)));
    }

    /// <summary>
    /// Create a scheduler job to update an existing project in the background
    /// </summary>
    /// <param name="projectRequest">The project request model to be used in the update</param>
    /// <param name="scheduler">The scheduler used to queue the job</param>
    /// <returns>Scheduler Job Result, containing the Job ID To poll via the Scheduler</returns>
    [Route("api/v4/project/background")]
    [HttpPut]
    public async Task<ScheduleJobResult> RequestUpdateProjectBackgroundJob([FromBody] UpdateProjectRequest projectRequest, [FromServices] ISchedulerProxy scheduler)
    {
      if (projectRequest == null)
      {
        ServiceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 39);
      }

      // do a quick validation to make sure the project acctually exists (this will also be run in the background task, but a quick response to the UI will be better if the project can't be updated)
      var project = AutoMapperUtility.Automapper.Map<UpdateProjectEvent>(projectRequest);
      project.ReceivedUTC = project.ActionUTC = DateTime.UtcNow;
      // validation includes check that project must exist - otherwise there will be a null legacyID.
      ProjectDataValidator.Validate(project, ProjectRepo, ServiceExceptionHandler);
      await ProjectDataValidator.ValidateProjectName(customerUid, projectRequest.ProjectName, projectRequest.ProjectUid.ToString(), Logger, ServiceExceptionHandler, ProjectRepo);

      var baseUrl = Request.Host.ToUriComponent();
      var callbackUrl = $"http://{baseUrl}/internal/v4/project";
      Logger.LogInformation($"nameof(RequestUpdateProjectBackgroundJob): baseUrl {callbackUrl}");

      var request = new ScheduleJobRequest
      {
        Filename = projectRequest.ProjectName + Guid.NewGuid(), // Make sure the filename is unique, it's not important what it's called as the scheduled job keeps a reference
        Method = "PUT",
        Url = callbackUrl,
        Headers =
        {
          ["Content-Type"] = Request.Headers["Content-Type"]
        }
      };
      request.SetBinaryPayload(Request.Body);

      var customHeaders = Request.Headers.GetCustomHeaders();

      return await scheduler.ScheduleBackgroundJob(request, customHeaders);
    }

    // DELETE: api/Project/
    /// <summary>
    /// Delete Project
    /// </summary>
    /// <param name="projectUid">projectUid to delete</param>
    /// <remarks>Deletes existing project</remarks>
    /// <response code="200">Ok</response>
    /// <response code="400">Bad request</response>
    [Route("api/v4/project/{projectUid}")]
    [HttpDelete]
    public async Task<ProjectV4DescriptorsSingleResult> DeleteProjectV4([FromRoute] string projectUid)
    {
      LogCustomerDetails("DeleteProjectV4", projectUid);
      var project = new DeleteProjectEvent
      {
        ProjectUID = projectUid,
        DeletePermanently = false,
        ActionUTC = DateTime.UtcNow,
        ReceivedUTC = DateTime.UtcNow
      };
      ProjectDataValidator.Validate(project, ProjectRepo, ServiceExceptionHandler);

      var messagePayload = JsonConvert.SerializeObject(new { DeleteProjectEvent = project });
      var isDeleted = await ProjectRepo.StoreEvent(project).ConfigureAwait(false);
      if (isDeleted == 0)
        ServiceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 66);

      if (!string.IsNullOrEmpty(customerUid))
        await notificationHubClient.Notify(new CustomerChangedNotification(customerUid));

      Logger.LogInformation("DeleteProjectV4. Completed successfully");
      return new ProjectV4DescriptorsSingleResult(
        AutoMapperUtility.Automapper.Map<ProjectV4Descriptor>(await ProjectRequestHelper.GetProject(project.ProjectUID.ToString(), customerUid, Logger, ServiceExceptionHandler, ProjectRepo)
          .ConfigureAwait(false)));

    }

    #endregion projects
    
  }
}
