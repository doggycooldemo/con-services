﻿using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VSS.Common.Abstractions.Clients.CWS.Models;
using VSS.MasterData.Models.ResultHandling.Abstractions;
using VSS.MasterData.Project.WebAPI.Common.Helpers;
using VSS.MasterData.Project.WebAPI.Common.Utilities;
using VSS.MasterData.Repositories;
using VSS.Visionlink.Interfaces.Core.Events.MasterData.Models;
using ProjectDatabaseModel = VSS.Productivity3D.Project.Abstractions.Models.DatabaseModels.Project;

namespace VSS.MasterData.Project.WebAPI.Common.Executors
{
  /// <summary>
  /// The executor which creates a project - appropriate for v2 and v4 controllers
  /// </summary>
  public class CreateProjectExecutor : RequestExecutorContainer
  {
    /// <summary>
    /// Save for potential rollback
    /// </summary>
    protected string subscriptionUidAssigned;

    /// <summary>
    /// Processes the CreateProjectEvent
    /// </summary>
    protected override async Task<ContractExecutionResult> ProcessAsyncEx<T>(T item)
    {
      var createProjectEvent = CastRequestObjectTo<CreateProjectEvent>(item, errorCode: 68);

      ProjectRequestHelper.ValidateProjectBoundary(createProjectEvent.ProjectBoundary, serviceExceptionHandler);
      await ProjectRequestHelper.ValidateCoordSystemInProductivity3D(createProjectEvent,
        serviceExceptionHandler, customHeaders, productivity3dV1ProxyCoord).ConfigureAwait(false);

      log.LogDebug($"Testing if there are overlapping projects for project {createProjectEvent.ProjectName}");
      await ProjectRequestHelper.DoesProjectOverlap(createProjectEvent.CustomerUID,
        createProjectEvent.ProjectUID,
        createProjectEvent.ProjectStartDate, createProjectEvent.ProjectEndDate, createProjectEvent.ProjectBoundary,
        log, serviceExceptionHandler, projectRepo);

      // Write to WM first to obtain their ProjectTRN to use as ProjectUid for our DB etc
      var createProjectRequestModel = AutoMapperUtility.Automapper.Map<CreateProjectRequestModel>(createProjectEvent);
      createProjectRequestModel.boundary = RepositoryHelper.MapProjectBoundary(createProjectEvent.ProjectBoundary);

      var response = await projectCwsClient.CreateProject(createProjectRequestModel);
      if (response != null) // todoMaverick exception
        createProjectEvent.ProjectUID = response.Id;
      // todoMaverick what about exception/other error
      

      // now making changes, potentially needing rollback 
      //  order changes to minimize rollback
      //    if CreateProjectInDb fails then nothing is done
      //    if CreateCoordSystem fails then project is deleted
      //    if AssociateProjectSubscription fails ditto
      createProjectEvent = await CreateProjectInDb(createProjectEvent).ConfigureAwait(false);
      await ProjectRequestHelper.CreateCoordSystemInProductivity3dAndTcc(
        createProjectEvent.ProjectUID, createProjectEvent.ShortRaptorProjectId, createProjectEvent.CoordinateSystemFileName,
        createProjectEvent.CoordinateSystemFileContent, true, log, serviceExceptionHandler, customerUid, customHeaders,
        projectRepo, productivity3dV1ProxyCoord, configStore, fileRepo, dataOceanClient, authn).ConfigureAwait(false);
      log.LogDebug($"CreateProject: Created project {createProjectEvent.ProjectUID}");
      
      log.LogDebug("CreateProject. completed successfully");
      return new ContractExecutionResult();
    }

    protected override ContractExecutionResult ProcessEx<T>(T item)
    {
      throw new NotImplementedException();
    }


    /// <summary>
    /// Creates a project. Handles both old and new project boundary formats.
    /// </summary>
    /// <param name="project">The create project event</param>
    /// <returns></returns>
    private async Task<CreateProjectEvent> CreateProjectInDb(CreateProjectEvent project)
    {
      log.LogDebug(
        $"Creating the project in the DB {JsonConvert.SerializeObject(project)}");

      var isCreated = 0;
      try
      {
        isCreated = await projectRepo.StoreEvent(project).ConfigureAwait(false);
      }
      catch (Exception e)
      {
        serviceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 61, "projectRepo.storeCreateProject", e.Message);
      }

      if (isCreated == 0)
        serviceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 61);

      log.LogDebug(
        $"Created the project in DB. IsCreated: {isCreated}. projectUid: {project.ProjectUID} shortRaptorProjectID: {project.ShortRaptorProjectId}");

      if (project.ShortRaptorProjectId <= 0)
      {
        ProjectDatabaseModel existing = null;
        try
        {
          existing = await projectRepo.GetProjectOnly(project.ProjectUID.ToString()).ConfigureAwait(false);
        }
        catch (Exception e)
        {
          serviceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 42, "projectRepo.GetProjectOnly", e.Message);
        }
        if (existing != null && existing.ShortRaptorProjectId > 0)
          project.ShortRaptorProjectId = existing.ShortRaptorProjectId;
        else
        {
          serviceExceptionHandler.ThrowServiceException(HttpStatusCode.InternalServerError, 42);
        }
      }

      log.LogDebug($"Using Legacy projectId {project.ShortRaptorProjectId} for project {project.ProjectName}");
      
      return project; // shortRaptorProjectID may have been added
    }
  }
}
