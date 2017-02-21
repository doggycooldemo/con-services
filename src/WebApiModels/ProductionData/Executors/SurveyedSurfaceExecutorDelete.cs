﻿using System;
using Microsoft.Extensions.Logging;
using VLPDDecls;
using VSS.Raptor.Service.WebApiModels.ProductionData.Models;
using VSS.Raptor.Service.Common.Contracts;
using VSS.Raptor.Service.Common.Interfaces;
using VSS.Raptor.Service.Common.Models;

namespace VSS.Raptor.Service.WebApiModels.ProductionData.Executors
{
  /// <summary>
  /// Executes DELETE method on Surveyed Surfaces resource.
  /// </summary>
  /// 
  public class SurveyedSurfaceExecutorDelete : SurveyedSurfaceExecutor
  {
    /// <summary>
    /// This constructor allows us to mock raptorClient
    /// </summary>
    /// <param name="raptorClient"></param>
    /// 
    public SurveyedSurfaceExecutorDelete(ILoggerFactory logger, IASNodeClient raptorClient)
        : base(logger, raptorClient)
    {
    }

    /// <summary>
    /// Default constructor for RequestExecutorContainer.Build
    /// </summary>
    public SurveyedSurfaceExecutorDelete()
    {
    }

    /// <summary>
    /// Sends a DELETE request to Production Data Server (PDS) client.
    /// </summary>
    /// <param name="item">DELETE request description.</param>
    /// <param name="surveyedSurfaces">Returned list of Surveyed Surfaces.</param>
    /// <returns>True if the processed request from PDS was successful, false - otherwise.</returns>
    /// 
    protected override bool SendRequestToPdsClient(object item, out TSurveyedSurfaceDetails[] surveyedSurfaces)
    {
      surveyedSurfaces = null;

      ProjectID projectId = (item as Tuple<ProjectID, DataID>).Item1;
      DataID surveyedSurfaceId= (item as Tuple<ProjectID, DataID>).Item2;

      return raptorClient.DiscardGroundSurfaceFileDetails(projectId.projectId ?? -1, surveyedSurfaceId.dataId);
    }

    /// <summary>
    /// Returns an instance of the ContractExecutionResult class as DELETE method execution result.
    /// </summary>
    /// <returns>An instance of the ContractExecutionResult class.</returns>
    /// 
    protected override ContractExecutionResult ExecutionResult(SurveyedSurfaceDetails[] surveyedSurfaces)
    {
      return new ContractExecutionResult(ContractExecutionStatesEnum.ExecutedSuccessfully, "Surveyed Surface data successfully deleted.");
    }
  }
}