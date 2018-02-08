﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASNodeRaptorReports;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VSS.Productivity3D.WebApi.Models.Common;
using VSS.Productivity3D.WebApi.Models.Compaction.Models.Reports;

namespace VSS.Productivity3D.WebApiTests.Compaction.Models
{
  [TestClass]
  public class ReportRowBaseTests
  {
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void ShouldSerializeElevation_returns_correct_result_When_Elevation_is_set(bool reportElevation)
    {
      var row = new GridRow();
      var request = CompactionReportGridRequest.CreateCompactionReportGridRequest(0, null, 0, null, reportElevation, false, false, false, false, false, null, null, GridReportOption.Unused, 0, 0, 0, 0, 0);

      row.SetReportFlags(request);

      Assert.AreEqual(reportElevation, row.ShouldSerializeElevation());
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void ShouldSerializeCutFill_returns_correct_result_When_CutFill_is_set(bool reportCutFill)
    {
      var row = new GridRow();
      var request = CompactionReportGridRequest.CreateCompactionReportGridRequest(0, null, 0, null, false, false, false, false, false, reportCutFill, null, null, GridReportOption.Unused, 0, 0, 0, 0, 0);

      row.SetReportFlags(request);

      Assert.AreEqual(reportCutFill, row.ShouldSerializeCutFill());
    }

    [TestMethod]
    [DataRow(false, (short)VelociraptorConstants.NO_CCV, false)]
    [DataRow(false, (short)150, false)]
    [DataRow(true, (short)150, true)]
    public void ShouldSerializeCMV_returns_correct_result_When_CMV_is_set(bool reportCmv, short cmv, bool expectedResult)
    {
      var row = GridRow.CreateRow(new TGridRow { CMV = cmv });
      var request = CompactionReportGridRequest.CreateCompactionReportGridRequest(0, null, 0, null, false, reportCmv, false, false, false, false, null, null, GridReportOption.Unused, 0, 0, 0, 0, 0);

      row.SetReportFlags(request);

      Assert.AreEqual(expectedResult, row.ShouldSerializeCMV());
    }

    [TestMethod]
    [DataRow(false, (short)VelociraptorConstants.NO_MDP, false)]
    [DataRow(false, (short)150, false)]
    [DataRow(true, (short)150, true)]
    public void ShouldSerializeMDP_returns_correct_result_When_MDP_is_set(bool reportMdp, short mdp, bool expectedResult)
    {
      var row = GridRow.CreateRow(new TGridRow { MDP = mdp });
      var request = CompactionReportGridRequest.CreateCompactionReportGridRequest(0, null, 0, null, false, false, reportMdp, false, false, false, null, null, GridReportOption.Unused, 0, 0, 0, 0, 0);

      row.SetReportFlags(request);

      Assert.AreEqual(expectedResult, row.ShouldSerializeMDP());
    }

    [TestMethod]
    [DataRow(false, (short)VelociraptorConstants.NO_PASSCOUNT, false)]
    [DataRow(false, (short)456, false)]
    [DataRow(true, (short)456, true)]
    public void ShouldSerializePassCount_returns_correct_result_When_PassCount_is_set(bool reportPasscount, short passCount, bool expectedResult)
    {
      var row = GridRow.CreateRow(new TGridRow { PassCount = passCount });
      var request = CompactionReportGridRequest.CreateCompactionReportGridRequest(0, null, 0, null, false, false, false, reportPasscount, false, false, null, null, GridReportOption.Unused, 0, 0, 0, 0, 0);

      row.SetReportFlags(request);

      Assert.AreEqual(expectedResult, row.ShouldSerializePassCount());
    }

    [TestMethod]
    [DataRow(false, (short)VelociraptorConstants.NO_TEMPERATURE, false)]
    [DataRow(false, (short)456, false)]
    [DataRow(true, (short)456, true)]
    public void ShouldSerializeTemperature_returns_correct_result_When_Temperature_is_set(bool reportTemperature, short temperature, bool expectedResult)
    {
      var row = GridRow.CreateRow(new TGridRow { Temperature = temperature});
      var request = CompactionReportGridRequest.CreateCompactionReportGridRequest(0, null, 0, null, false, false, false, false, reportTemperature, false, null, null, GridReportOption.Unused, 0, 0, 0, 0, 0);

      row.SetReportFlags(request);

      Assert.AreEqual(expectedResult, row.ShouldSerializeTemperature());
    }
    
    [TestMethod]
    [DataRow((short)150, (double)15)]
    [DataRow((short)456, (double)45.6)]
    public void MDP_returns_original_value_divided_by_10(short mdp, double expectedResult)
    {
      var row = GridRow.CreateRow(new TGridRow { MDP = mdp });
      var request = CompactionReportGridRequest.CreateCompactionReportGridRequest(0, null, 0, null, false, false, true, false, false, false, null, null, GridReportOption.Unused, 0, 0, 0, 0, 0);

      row.SetReportFlags(request);

      Assert.AreEqual(expectedResult, row.MDP);
    }

    [TestMethod]
    [DataRow((short)150, (double)15)]
    [DataRow((short)456, (double)45.6)]
    public void CMV_returns_original_value_divided_by_10(short cmv, double expectedResult)
    {
      var row = GridRow.CreateRow(new TGridRow { CMV = cmv });
      var request = CompactionReportGridRequest.CreateCompactionReportGridRequest(0, null, 0, null, false, true, false, false, false, false, null, null, GridReportOption.Unused, 0, 0, 0, 0, 0);

      row.SetReportFlags(request);

      Assert.AreEqual(expectedResult, row.CMV);
    }

    [TestMethod]
    [DataRow((short)150, (double)15)]
    [DataRow((short)456, (double)45.6)]
    public void Temperature_returns_original_value_divided_by_10(short temperature, double expectedResult)
    {
      var row = GridRow.CreateRow(new TGridRow { Temperature = temperature});
      var request = CompactionReportGridRequest.CreateCompactionReportGridRequest(0, null, 0, null, false, false, false, true, false, false, null, null, GridReportOption.Unused, 0, 0, 0, 0, 0);

      row.SetReportFlags(request);

      Assert.AreEqual(expectedResult, row.Temperature);
    }
  }
}