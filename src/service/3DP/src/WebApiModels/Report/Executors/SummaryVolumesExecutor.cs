﻿using System.Threading.Tasks;
using VSS.MasterData.Models.ResultHandling.Abstractions;
using VSS.Productivity3D.Common.Filters.Utilities;
using VSS.Productivity3D.Common.Interfaces;
using VSS.Productivity3D.Models.Enums;
using VSS.Productivity3D.Models.Models;
using VSS.Productivity3D.Productivity3D.Models.Compaction.ResultHandling;
using VSS.Productivity3D.WebApi.Models.Report.Models;

namespace VSS.Productivity3D.WebApi.Models.Report.Executors
{
  public class SummaryVolumesExecutor : RequestExecutorContainer
  {
    /// <summary>
    /// Default constructor for RequestExecutorContainer.Build
    /// </summary>
    public SummaryVolumesExecutor()
    {
      ProcessErrorCodes();
    }

    protected override async Task<ContractExecutionResult> ProcessAsyncEx<T>(T item)
    {
      try
      {
        var request = CastRequestObjectTo<SummaryVolumesRequest>(item);

        var baseFilter = request.BaseFilter;
        var topFilter = request.TopFilter;
        if (request.VolumeCalcType == VolumesType.Between2Filters)
        {
          if (!request.ExplicitFilters)
          {
            var adjusted = FilterUtilities.AdjustFilterToFilter(request.BaseFilter, request.TopFilter);
            baseFilter = adjusted.baseFilter;
            topFilter = adjusted.topFilter;
          }
        }
        else
        {
          var adjusted = FilterUtilities.ReconcileTopFilterAndVolumeComputationMode(baseFilter, topFilter, request.VolumeCalcType);
          baseFilter = adjusted.baseFilter;
          topFilter = adjusted.topFilter;
        }

        var summaryVolumesRequest = new SummaryVolumesDataRequest(
            request.ProjectUid,
            baseFilter,
            topFilter,
            request.BaseDesignDescriptor.FileUid,
            request.BaseDesignDescriptor.Offset,
            request.TopDesignDescriptor.FileUid,
            request.TopDesignDescriptor.Offset,
            request.VolumeCalcType);

          return await trexCompactionDataProxy.SendDataPostRequest<SummaryVolumesResult, SummaryVolumesDataRequest>(summaryVolumesRequest, "/volumes/summary", customHeaders);
      }
      finally
      {
        ContractExecutionStates.ClearDynamic();
      }
    }

    protected sealed override void ProcessErrorCodes()
    {
    }
  }
}
