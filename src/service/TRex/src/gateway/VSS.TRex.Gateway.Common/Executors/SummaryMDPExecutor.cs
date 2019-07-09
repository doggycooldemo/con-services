﻿using Microsoft.Extensions.Logging;
using VSS.Common.Abstractions.Configuration;
using VSS.ConfigurationStore;
using VSS.MasterData.Models.Handlers;
using VSS.MasterData.Models.ResultHandling.Abstractions;
using VSS.Productivity3D.Models.Models;
using VSS.TRex.Analytics.MDPStatistics;
using VSS.TRex.Analytics.MDPStatistics.GridFabric;
using VSS.TRex.Common.Models;
using VSS.TRex.Common.Records;
using VSS.TRex.Filters;
using VSS.TRex.Filters.Models;
using VSS.TRex.Gateway.Common.Converters;
using VSS.TRex.Types;
using MDPSummaryResult = VSS.TRex.Analytics.MDPStatistics.MDPStatisticsResult;
using SummaryResult = VSS.Productivity3D.Models.ResultHandling.MDPSummaryResult;

namespace VSS.TRex.Gateway.Common.Executors
{
  /// <summary>
  /// Processes the request to get MDP summary.
  /// </summary>
  public class SummaryMDPExecutor : BaseExecutor
  {
    public SummaryMDPExecutor(IConfigurationStore configStore, ILoggerFactory logger,
      IServiceExceptionHandler exceptionHandler)
      : base(configStore, logger, exceptionHandler)
    {
    }

    /// <summary>
    /// Default constructor for RequestExecutorContainer.Build
    /// </summary>
    public SummaryMDPExecutor()
    {
    }

    protected override ContractExecutionResult ProcessEx<T>(T item)
    {
      var request = item as MDPSummaryRequest;

      if (request == null)
        ThrowRequestTypeCastException<MDPSummaryRequest>();

      var siteModel = GetSiteModel(request.ProjectUid);

      var filter = ConvertFilter(request.Filter, siteModel);

      var operation = new MDPStatisticsOperation();
      var overrides = request.Overrides;
      var mdpSummaryResult = operation.Execute(
        new MDPStatisticsArgument()
        {
          ProjectID = siteModel.ID,
          Filters = new FilterSet(filter),
          MDPPercentageRange = new MDPRangePercentageRecord(overrides.MinMDPPercent, overrides.MaxMDPPercent),
          OverrideMachineMDP = overrides.OverrideTargetMDP,
          OverridingMachineMDP = overrides.MdpTarget,
          Overrides = AutoMapperUtility.Automapper.Map<OverrideParameters>(request.Overrides)
        }
      );

      if (mdpSummaryResult != null)
      {
        if (mdpSummaryResult.ResultStatus == RequestErrorStatus.OK)
          return ConvertResult(mdpSummaryResult);

        throw CreateServiceException<SummaryMDPExecutor>(mdpSummaryResult.ResultStatus);
      }

      throw CreateServiceException<SummaryMDPExecutor>();
    }

    private SummaryResult ConvertResult(MDPStatisticsResult summary)
    {
      return new SummaryResult(
        summary.WithinTargetPercent,
        summary.ConstantTargetMDP,
        summary.IsTargetMDPConstant,
        summary.AboveTargetPercent,
        (short)summary.ReturnCode,
        summary.TotalAreaCoveredSqMeters,
        summary.BelowTargetPercent);
    }

  }
}
