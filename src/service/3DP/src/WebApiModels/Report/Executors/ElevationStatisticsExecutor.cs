﻿using System;
using System.Threading.Tasks;
#if RAPTOR
using ASNode.ElevationStatistics.RPC;
using ASNodeDecls;
using BoundingExtents;
using SVOICOptionsDecls;
#endif
using VSS.MasterData.Models.Models;
using VSS.MasterData.Models.ResultHandling.Abstractions;
using VSS.Productivity3D.Common;
using VSS.Productivity3D.Common.Interfaces;
using VSS.Productivity3D.Common.Proxies;
using VSS.Productivity3D.Common.ResultHandling;
using VSS.Productivity3D.Models.Models;
using VSS.Productivity3D.Models.ResultHandling;
using VSS.Productivity3D.WebApi.Models.Compaction.AutoMapper;
using VSS.Productivity3D.WebApi.Models.Extensions;
using VSS.Productivity3D.WebApi.Models.Report.Models;

namespace VSS.Productivity3D.WebApi.Models.Report.Executors
{
  public class ElevationStatisticsExecutor : TbcExecutorHelper
  {
    /// <summary>
    /// Default constructor for RequestExecutorContainer.Build
    /// </summary>
    public ElevationStatisticsExecutor()
    {
      ProcessErrorCodes();
    }
#if RAPTOR
    private BoundingBox3DGrid ConvertExtents(T3DBoundingWorldExtent extents)
    {
      return new BoundingBox3DGrid(
              extents.MinX,
              extents.MinY,
              extents.MinZ,
              extents.MaxX,
              extents.MaxY,
              extents.MaxZ
              );
    }

    private ElevationStatisticsResult ConvertResult(TASNodeElevationStatisticsResult result)
    {
      return new ElevationStatisticsResult
      (
          ConvertExtents(result.BoundingExtents),
          result.MinElevation,
          result.MaxElevation,
          result.CoverageArea
      );
    }
#endif

    protected override async Task<ContractExecutionResult> ProcessAsyncEx<T>(T item)
    {
      var request = CastRequestObjectTo<ElevationStatisticsRequest>(item);
#if RAPTOR
      if (UseTRexGateway("ENABLE_TREX_GATEWAY_ELEVATION") || UseTRexGateway("ENABLE_TREX_GATEWAY_TILES"))
      {
#endif
      await PairUpAssetIdentifiers(request.ProjectUid.Value, request.Filter);
      await PairUpImportedFileIdentifiers(request.ProjectUid.Value, filter1: request.Filter);
      
      var elevationStatisticsRequest = new ElevationDataRequest(
          request.ProjectUid.Value, 
          request.Filter,
          AutoMapperUtility.Automapper.Map<OverridingTargets>(request.LiftBuildSettings),
          AutoMapperUtility.Automapper.Map<LiftSettings>(request.LiftBuildSettings));
        return await trexCompactionDataProxy.SendDataPostRequest<ElevationStatisticsResult, ElevationDataRequest>(elevationStatisticsRequest, "/elevationstatistics", customHeaders);
#if RAPTOR
      }
      //new TASNodeElevationStatisticsResult();

      var Filter = RaptorConverters.ConvertFilter(request.Filter, request.ProjectId, raptorClient);

      var raptorResult = raptorClient.GetElevationStatistics(request.ProjectId ?? VelociraptorConstants.NO_PROJECT_ID,
                           ASNodeRPC.__Global.Construct_TASNodeRequestDescriptor((Guid)(request.CallId ?? Guid.NewGuid()), 0,
                             TASNodeCancellationDescriptorType.cdtElevationStatistics),
                          Filter,
                          RaptorConverters.ConvertLift(request.LiftBuildSettings, TFilterLayerMethod.flmAutomatic),
                          out var result);

      if (raptorResult == TASNodeErrorStatus.asneOK)
        return ConvertResult(result);

      throw CreateServiceException<ElevationStatisticsExecutor>((int)raptorResult);
#endif
    }

    protected sealed override void ProcessErrorCodes()
    {
#if RAPTOR
      RaptorResult.AddErrorMessages(ContractExecutionStates);
#endif
    }
  }
}
