﻿using ASNode.ThicknessSummary.RPC;
using ASNodeDecls;
using BoundingExtents;
using SVOICOptionsDecls;
using System;
using System.Net;
using VSS.Common.Exceptions;
using VSS.MasterData.Models.ResultHandling.Abstractions;
using VSS.Productivity3D.Common.Filters.Interfaces;
using VSS.Productivity3D.Common.Models;
using VSS.Productivity3D.Common.Proxies;
using VSS.Productivity3D.WebApi.Models.Report.Models;
using VSS.Productivity3D.WebApiModels.Report.Models;
using VSS.Productivity3D.WebApiModels.Report.ResultHandling;

namespace VSS.Productivity3D.WebApiModels.Report.Executors
{
  /// <summary>
  /// Builds Summary thickness report from Raptor
  /// </summary>
  public class SummaryThicknessExecutor : RequestExecutorContainer
  {
    private BoundingBox3DGrid ConvertExtents(T3DBoundingWorldExtent extents)
    {
      return BoundingBox3DGrid.CreatBoundingBox3DGrid(

          extents.MinX,
          extents.MinY,
          extents.MinZ,
          extents.MaxX,
          extents.MaxY,
          extents.MaxZ
          );
    }

    private SummaryThicknessResult ConvertResult(TASNodeThicknessSummaryResult result)
    {

      return SummaryThicknessResult.CreateSummaryThicknessResult
          (
              ConvertExtents(result.BoundingExtents),
              Math.Round(result.AboveTargetArea,5) ,
              Math.Round(result.BelowTargetArea,5) ,
              Math.Round(result.MatchTargetArea,5) ,
              Math.Round(result.NoCovegareArea, 5) 
          );
    }

    protected override ContractExecutionResult ProcessEx<T>(T item)
    {
      SummaryParametersBase request = item as SummaryParametersBase;
      TASNodeThicknessSummaryResult result = new TASNodeThicknessSummaryResult();

      bool success = raptorClient.GetSummaryThickness(request.projectId ?? -1,
        ASNodeRPC.__Global.Construct_TASNodeRequestDescriptor((Guid) (request.CallId ?? Guid.NewGuid()), 0,
          TASNodeCancellationDescriptorType.cdtVolumeSummary),
        RaptorConverters.ConvertFilter(request.BaseFilterId, request.BaseFilter, request.projectId, null, null),
        RaptorConverters.ConvertFilter(request.TopFilterId, request.TopFilter, request.projectId, null, null),
        RaptorConverters.ConvertFilter(request.AdditionalSpatialFilterId,
          request.AdditionalSpatialFilter, request.projectId, null, null),
        RaptorConverters.ConvertLift(request.LiftBuildSettings, TFilterLayerMethod.flmAutomatic),
        out result);
      if (success)
      {
        return ConvertResult(result);
      }

      throw new ServiceException(HttpStatusCode.BadRequest,
        new ContractExecutionResult(ContractExecutionStatesEnum.FailedToGetResults,
          "Failed to get requested thickness summary data"));
    }
  }
}