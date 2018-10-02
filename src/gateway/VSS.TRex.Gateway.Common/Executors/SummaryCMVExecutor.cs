﻿using System.Net;
using Microsoft.Extensions.Logging;
using VSS.Common.Exceptions;
using VSS.ConfigurationStore;
using VSS.MasterData.Models.Handlers;
using VSS.MasterData.Models.ResultHandling.Abstractions;
using VSS.Productivity3D.Models.Models;
using VSS.TRex.Analytics.CMVStatistics;
using VSS.TRex.Analytics.CMVStatistics.GridFabric;
using VSS.TRex.Filters;
using VSS.TRex.Gateway.Common.Requests;
using VSS.TRex.Types;
using CMVStatisticsResult = VSS.TRex.Analytics.CMVStatistics.CMVStatisticsResult;
using SummaryResult = VSS.Productivity3D.Models.ResultHandling.CMVSummaryResult;

namespace VSS.TRex.Gateway.Common.Executors
{
  /// <summary>
  /// Processes the request to get CMV summary.
  /// </summary>
  public class SummaryCMVExecutor : BaseExecutor
  {
    public SummaryCMVExecutor(IConfigurationStore configStore, ILoggerFactory logger,
      IServiceExceptionHandler exceptionHandler)
      : base(configStore, logger, exceptionHandler)
    {
    }

    /// <summary>
    /// Default constructor for RequestExecutorContainer.Build
    /// </summary>
    public SummaryCMVExecutor()
    {
    }

    protected override ContractExecutionResult ProcessEx<T>(T item)
    {
      const string ERROR_MESSAGE = "Failed to get requested CMV summary data";

      CMVSummaryRequest request = item as CMVSummaryRequest;

      if (request == null)
        ThrowRequestTypeCastException(typeof(CMVSummaryRequest));
      
      var siteModel = GetSiteModel(request.ProjectUid);

      var filter = ConvertFilter(request.Filter, siteModel);

      CMVStatisticsOperation operation = new CMVStatisticsOperation();

      CMVStatisticsResult cmvSummaryResult = operation.Execute(
        new CMVStatisticsArgument()
        {
          ProjectID = siteModel.ID,
          Filters = new FilterSet(filter),
          CMVPercentageRange = new CMVRangePercentageRecord(request.MinCMVPercent, request.MaxCMVPercent),
          OverrideMachineCMV = request.OverrideTargetCMV,
          OverridingMachineCMV = request.CmvTarget
        }
      );

      if (cmvSummaryResult != null)
      {
        if (cmvSummaryResult.ResultStatus == RequestErrorStatus.OK)
          return ConvertResult(cmvSummaryResult);

        throw CreateServiceException(ERROR_MESSAGE, cmvSummaryResult.ResultStatus);
      }

      throw CreateServiceException(ERROR_MESSAGE);
    }

    private SummaryResult ConvertResult(CMVStatisticsResult summary)
    {
      return new SummaryResult(
        summary.WithinTargetPercent,
        summary.ConstantTargetCMV,
        summary.IsTargetCMVConstant,
        summary.AboveTargetPercent,
        (short) summary.ReturnCode,
        summary.TotalAreaCoveredSqMeters,
        summary.BelowTargetPercent);
    }
  }
}
