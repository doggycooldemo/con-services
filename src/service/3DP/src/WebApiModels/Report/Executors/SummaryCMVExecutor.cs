﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
#if RAPTOR
using ASNodeDecls;
using VLPDDecls;
#endif
using VSS.MasterData.Models.ResultHandling.Abstractions;
using VSS.Productivity3D.Common;
using VSS.Productivity3D.Common.Interfaces;
using VSS.Productivity3D.Common.Models;
using VSS.Productivity3D.Common.Proxies;
using VSS.Productivity3D.Common.ResultHandling;
using VSS.Productivity3D.Models.Models;
using VSS.Productivity3D.Models.ResultHandling;
using VSS.Productivity3D.Productivity3D.Models;
using VSS.Productivity3D.Productivity3D.Models.Compaction;
using VSS.Productivity3D.Productivity3D.Models.Compaction.ResultHandling;
using VSS.Productivity3D.WebApi.Models.Compaction.AutoMapper;
using VSS.Productivity3D.WebApi.Models.Extensions;
using VSS.Productivity3D.WebApi.Models.Report.Models;

namespace VSS.Productivity3D.WebApi.Models.Report.Executors
{
  /// <summary>
  /// The executor which passes the summary CMV request to Raptor
  /// </summary>
  public class SummaryCMVExecutor : TbcExecutorHelper
  {
    /// <summary>
    /// Default constructor for RequestExecutorContainer.Build
    /// </summary>
    public SummaryCMVExecutor()
    {
      ProcessErrorCodes();
    }

    /// <summary>
    /// Processes the summary CMV request by passing the request to Raptor and returning the result.
    /// </summary>
    protected override async Task<ContractExecutionResult> ProcessAsyncEx<T>(T item)
    {
      try
      {
        var request = CastRequestObjectTo<CMVRequest>(item);
#if RAPTOR
        if (configStore.GetValueBool("ENABLE_TREX_GATEWAY_CMV") ?? false)
        {
#endif
        await PairUpAssetIdentifiers(request.ProjectUid.Value, request.Filter);
        await PairUpImportedFileIdentifiers(request.ProjectUid.Value, filter1: request.Filter);

        var cmvSummaryRequest = new CMVSummaryRequest(
            request.ProjectUid.Value,
            request.Filter,
            request.CmvSettings.CmvTarget,
            request.CmvSettings.OverrideTargetCMV,
            request.CmvSettings.MaxCMVPercent,
            request.CmvSettings.MinCMVPercent, AutoMapperUtility.Automapper.Map<LiftSettings>(request.LiftBuildSettings));
        log.LogDebug($"{nameof(SummaryCMVExecutor)} trexRequest {JsonConvert.SerializeObject(cmvSummaryRequest)}");

        return await trexCompactionDataProxy.SendDataPostRequest<CMVSummaryResult, CMVSummaryRequest>(cmvSummaryRequest, "/cmv/summary", customHeaders);
#if RAPTOR
        }

        var raptorFilter = RaptorConverters.ConvertFilter(request.Filter, request.ProjectId, raptorClient, request.OverrideStartUTC, request.OverrideEndUTC, request.OverrideAssetIds);

        var raptorResult = raptorClient.GetCMVSummary(request.ProjectId ?? VelociraptorConstants.NO_PROJECT_ID,
          ASNodeRPC.__Global.Construct_TASNodeRequestDescriptor(request.CallId ?? Guid.NewGuid(), 0, TASNodeCancellationDescriptorType.cdtCMVSummary),
          ConvertSettings(request.CmvSettings),
          raptorFilter,
          RaptorConverters.ConvertLift(request.LiftBuildSettings, raptorFilter.LayerMethod),
          out var cmvSummary);

        if (raptorResult == TASNodeErrorStatus.asneOK)
          return ConvertResult(cmvSummary);

        throw CreateServiceException<SummaryCMVExecutor>((int)raptorResult);
#endif
      }
      finally
      {
        ContractExecutionStates.ClearDynamic();
      }
    }

    protected sealed override void ProcessErrorCodes()
    {
#if RAPTOR
      RaptorResult.AddErrorMessages(ContractExecutionStates);
#endif
    }

#if RAPTOR
    private CMVSummaryResult ConvertResult(TCMVSummary summary)
    {
      return new CMVSummaryResult(
        summary.CompactedPercent,
        summary.ConstantTargetCMV,
        summary.IsTargetCMVConstant,
        summary.OverCompactedPercent,
        summary.ReturnCode,
        summary.TotalAreaCoveredSqMeters,
        summary.UnderCompactedPercent);
    }

    private TCMVSettings ConvertSettings(CMVSettings settings)
    {
      return new TCMVSettings
      {
        CMVTarget = settings.CmvTarget,
        IsSummary = true,
        MaxCMV = settings.MaxCMV,
        MaxCMVPercent = settings.MaxCMVPercent,
        MinCMV = settings.MinCMV,
        MinCMVPercent = settings.MinCMVPercent,
        OverrideTargetCMV = settings.OverrideTargetCMV
      };
    }
#endif

  }
}
