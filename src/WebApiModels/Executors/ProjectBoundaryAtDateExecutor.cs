﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Repositories.DBModels;
using System.Net;
using VSS.TagFileAuth.Service.WebApiModels.Models.RaptorServicesCommon;
using VSS.TagFileAuth.Service.WebApiModels.ResultHandling;

namespace VSS.TagFileAuth.Service.WebApiModels.Executors
{
  /// <summary>
  /// The executor which gets the project boundary of the project for the requested project id.
  /// </summary>
  public class ProjectBoundaryAtDateExecutor : RequestExecutorContainer
  {
    /// <summary>
    /// Processes the get project boundary request and finds active projects of the asset owner at the given date time.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <returns>a GetProjectBoundaryAtDateResult if successful</returns>      
    protected override ContractExecutionResult ProcessEx<T>(T item)
    {
      GetProjectBoundaryAtDateRequest request = item as GetProjectBoundaryAtDateRequest;
      log.LogDebug("ProjectBoundaryAtDateExecutor: Going to process request {0}", JsonConvert.SerializeObject(request));

      bool result = false;
      TWGS84FenceContainer projectBoundary = new TWGS84FenceContainer();
      Project project = null;

      project = LoadProject(request.projectId);
      log.LogDebug("ProjectBoundaryAtDateExecutor: Loaded project? {0}", JsonConvert.SerializeObject(project));

      if (project != null)
      {
        if (project.StartDate <= request.tagFileUTC.Date && request.tagFileUTC.Date <= project.EndDate &&
            !string.IsNullOrEmpty(project.GeometryWKT)
            )
        {
          projectBoundary.FencePoints = ParseBoundaryData(project.GeometryWKT);
          log.LogInformation("ProjectBoundaryAtDateExecutor: Loaded projectBoundary.FencePoints? {0}", JsonConvert.SerializeObject(projectBoundary.FencePoints));

          if (projectBoundary.FencePoints.Length > 0)
            result = true;
        }
      }

      try
      {
        return GetProjectBoundaryAtDateResult.CreateGetProjectBoundaryAtDateResult(result, projectBoundary);
      }
      catch
      {
        throw new ServiceException(HttpStatusCode.BadRequest,
          new ContractExecutionResult(ContractExecutionStatesEnum.InternalProcessingError, "Failed to get project boundary"));
      }

    }

  }
}
  