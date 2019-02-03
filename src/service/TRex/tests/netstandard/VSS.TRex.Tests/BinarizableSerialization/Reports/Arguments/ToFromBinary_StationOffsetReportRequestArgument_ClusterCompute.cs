﻿using System;
using VSS.TRex.Filters;
using Xunit;
using VSS.TRex.Common.Types;
using VSS.TRex.Tests.Analytics.Common;
using VSS.TRex.Reports.StationOffset.GridFabric.Arguments;
using System.Collections.Generic;
using VSS.TRex.Tests.BinarizableSerialization.Analytics;

namespace VSS.TRex.Tests.BinarizableSerialization.Reports.Arguments

{
  public class ToFromBinary_StationOffsetReportRequestArgument_ClusterCompute : BaseTests, IClassFixture<AnalyticsTestsDIFixture>
  {
    [Fact]
    public void Test_StationOffsetReportRequestArgument_ClusterCompute_Simple()
    {
      SimpleBinarizableInstanceTester.TestClass<StationOffsetReportRequestArgument_ClusterCompute>("Empty StationOffsetReportRequestArgument_ClusterCompute not same after round trip serialisation");
    }

    [Fact]
    public void Test_StationOffsetReportRequestArgument_ClusterCompute_Empty()
    {
      var request = new StationOffsetReportRequestArgument_ClusterCompute() { };

      SimpleBinarizableInstanceTester.TestClass(request, "Empty StationOffsetReportRequestArgument_ClusterCompute not same after round trip serialisation");
    }

    [Fact]
    public void Test_StationOffsetReportRequestArgument_ClusterCompute_WithContent()
    {
      var request = new StationOffsetReportRequestArgument_ClusterCompute()
      {
        ProjectID = Guid.NewGuid(),
        Filters = new FilterSet(new CombinedFilter()),
        ReportElevation = true,
        ReportCmv = true,
        ReportMdp = true,
        ReportPassCount = true,
        ReportTemperature = true,
        ReportCutFill = false,
        Points = new List<StationOffsetPoint>() { new StationOffsetPoint(100, -1, 808000, 406000)}
      };

      SimpleBinarizableInstanceTester.TestClass(request, "Empty StationOffsetReportRequestArgument_ClusterCompute not same after round trip serialisation");
    }
  }
}
