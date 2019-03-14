﻿using VSS.TRex.Analytics.MDPStatistics.GridFabric;
using VSS.TRex.Tests.Analytics.Common;
using VSS.TRex.Types;
using Xunit;

namespace VSS.TRex.Tests.BinarizableSerialization.Analytics.Responses
{
  public class ToFromBinary_MDPStatisticsResponse
  {
    [Fact]
    public void Test_MDPStatisticsResponse_Simple()
    {
      SimpleBinarizableInstanceTester.TestClass<MDPStatisticsResponse>("Empty MDPStatisticsResponse not same after round trip serialisation");
    }

    [Fact]
    public void Test_MDPStatisticsResponse()
    {
      var response = new MDPStatisticsResponse()
      {
        ResultStatus = RequestErrorStatus.OK,
        CellSize = TestConsts.CELL_SIZE,
        CellsScannedOverTarget = TestConsts.CELLS_OVER_TARGET,
        CellsScannedAtTarget = TestConsts.CELLS_AT_TARGET,
        CellsScannedUnderTarget = TestConsts.CELLS_UNDER_TARGET,
        SummaryCellsScanned = TestConsts.CELLS_OVER_TARGET + TestConsts.CELLS_AT_TARGET + TestConsts.CELLS_UNDER_TARGET,
        IsTargetValueConstant = true,
        Counts = TestConsts.CountsArray,
        MissingTargetValue = false,
        LastTargetMDP = 500
      };

      SimpleBinarizableInstanceTester.TestClass(response, "Custom MDPStatisticsResponse not same after round trip serialisation");
    }
  }
}
