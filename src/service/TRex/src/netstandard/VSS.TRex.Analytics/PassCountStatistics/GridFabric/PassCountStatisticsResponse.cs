﻿using Apache.Ignite.Core.Binary;
using VSS.TRex.Analytics.Foundation.GridFabric.Responses;
using VSS.TRex.Analytics.Foundation.Interfaces;
using VSS.TRex.Common;
using VSS.TRex.Common.Records;
using VSS.TRex.GridFabric.Interfaces;
using VSS.TRex.Types;

namespace VSS.TRex.Analytics.PassCountStatistics.GridFabric
{
  /// <summary>
  /// The response state returned from a Pass Count summary request
  /// </summary>
  public class PassCountStatisticsResponse : StatisticsAnalyticsResponse, IAggregateWith<PassCountStatisticsResponse>, 
    IAnalyticsOperationResponseResultConversion<PassCountStatisticsResult>
  {
    private static byte VERSION_NUMBER = 1;

    /// <summary>
    /// Holds last known good target Pass Count range values.
    /// </summary>
    public PassCountRangeRecord LastPassCountTargetRange;

    /// <summary>
    /// Serialises content to the writer
    /// </summary>
    public override void InternalToBinary(IBinaryRawWriter writer)
    {
      base.InternalToBinary(writer);

      VersionSerializationHelper.EmitVersionByte(writer, VERSION_NUMBER);

      LastPassCountTargetRange.ToBinary(writer);
    }

    /// <summary>
    /// Serialises content from the writer
    /// </summary>
    public override void InternalFromBinary(IBinaryRawReader reader)
    {
      base.InternalFromBinary(reader);

      var version = VersionSerializationHelper.CheckVersionByte(reader, VERSION_NUMBER);

      if (version == 1)
      {
        LastPassCountTargetRange.FromBinary(reader);
      }
    }

    /// <summary>
    /// Aggregate a set of Pass Count summary into this set and return the result.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    protected override void AggregateBaseDataWith(StatisticsAnalyticsResponse other)
    {
      base.AggregateBaseDataWith(other);

      LastPassCountTargetRange = ((PassCountStatisticsResponse) other).LastPassCountTargetRange;
    }

    public PassCountStatisticsResponse AggregateWith(PassCountStatisticsResponse other)
    {
      return base.AggregateWith(other) as PassCountStatisticsResponse;
    }

    public PassCountStatisticsResult ConstructResult()
    {
      return new PassCountStatisticsResult
      {
        IsTargetPassCountConstant = IsTargetValueConstant,
        ConstantTargetPassCountRange = IsTargetValueConstant ? LastPassCountTargetRange : new PassCountRangeRecord(0, 0),
        AboveTargetPercent = ValueOverTargetPercent,
        WithinTargetPercent = ValueAtTargetPercent,
        BelowTargetPercent = ValueUnderTargetPercent,
        TotalAreaCoveredSqMeters = SummaryProcessedArea,

        ReturnCode = MissingTargetValue ?
          (!(ValueOverTargetPercent < Consts.TOLERANCE_PERCENTAGE) && ValueAtTargetPercent < Consts.TOLERANCE_PERCENTAGE && ValueUnderTargetPercent < Consts.TOLERANCE_PERCENTAGE) ? MissingTargetDataResultType.PartialResult : MissingTargetDataResultType.PartialResultMissingTarget :
          SummaryCellsScanned == 0 ? MissingTargetDataResultType.NoProblems : MissingTargetDataResultType.NoResult,

        Counts = Counts,
        ResultStatus = ResultStatus
      };
    }
  }
}
