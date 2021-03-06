﻿using System;
using System.Linq;
using System.Threading.Tasks;
using CoreX.Interfaces;
using FluentAssertions;
using VSS.Productivity3D.Models.Enums;
using VSS.Productivity3D.Models.ResultHandling;
using VSS.TRex.CellDatum.GridFabric.Arguments;
using VSS.TRex.CellDatum.GridFabric.ComputeFuncs;
using VSS.TRex.CellDatum.GridFabric.Requests;
using VSS.TRex.CellDatum.GridFabric.Responses;
using VSS.TRex.Cells;
using VSS.TRex.Common.Models;
using VSS.TRex.Common.Records;
using VSS.TRex.Designs.GridFabric.Arguments;
using VSS.TRex.Designs.GridFabric.ComputeFuncs;
using VSS.TRex.Designs.GridFabric.Responses;
using VSS.TRex.Designs.Models;
using VSS.TRex.DI;
using VSS.TRex.Filters;
using VSS.TRex.Geometry;
using VSS.TRex.GridFabric.Affinity;
using VSS.TRex.SiteModels.Interfaces;
using VSS.TRex.SubGrids.GridFabric.ComputeFuncs;
using VSS.TRex.SubGrids.Interfaces;
using VSS.TRex.SubGridTrees.Interfaces;
using VSS.TRex.Tests.TestFixtures;
using VSS.TRex.Types;
using VSS.TRex.Types.CellPasses;
using Xunit;
using Consts = VSS.TRex.Common.Consts;

namespace VSS.TRex.Tests.CellDatum.GridFabric
{
  [UnitTestCoveredRequest(RequestType = typeof(CellDatumRequest_ClusterCompute))]
  [UnitTestCoveredRequest(RequestType = typeof(CellDatumRequest_ApplicationService))]
  public class CellDatumRequestTests : IClassFixture<DITAGFileAndSubGridRequestsWithIgniteFixture>
  {

    private void AddDesignProfilerGridRouting()
    {
      //This is specific to cell datum i.e. what the cell datum cluster compute will call in the design profiler
      IgniteMock.Immutable.AddApplicationGridRouting<CalculateDesignElevationSpotComputeFunc, CalculateDesignElevationSpotArgument, CalculateDesignElevationSpotResponse>();
    }

    private void AddApplicationGridRouting() => IgniteMock.Immutable.AddApplicationGridRouting<CellDatumRequestComputeFunc_ApplicationService, CellDatumRequestArgument_ApplicationService, CellDatumResponse_ApplicationService>();

    private void AddClusterComputeGridRouting()
    {
      IgniteMock.Immutable.AddClusterComputeSpatialAffinityGridRouting<CellDatumRequestComputeFunc_ClusterCompute, CellDatumRequestArgument_ClusterCompute, CellDatumResponse_ClusterCompute>();
      IgniteMock.Immutable.AddClusterComputeGridRouting<SubGridProgressiveResponseRequestComputeFunc, ISubGridProgressiveResponseRequestComputeFuncArgument, bool>();
    }


    private ISiteModel BuildModelForSingleCellDatum(DateTime baseTime, bool heightOnly = false)
    {
      var siteModel = DITAGFileAndSubGridRequestsWithIgniteFixture.NewEmptyModel();
      var bulldozerMachineIndex = siteModel.Machines.Locate("Bulldozer", false).InternalSiteModelMachineIndex;
      siteModel.MachinesTargetValues[bulldozerMachineIndex].VibrationStateEvents.PutValueAtDate(Consts.MIN_DATETIME_AS_UTC, VibrationState.On);

      var cellPasses = Enumerable.Range(1, 10).Select(x =>
        new CellPass
        {
          InternalSiteModelMachineIndex = bulldozerMachineIndex,
          Time = baseTime.AddMinutes(x),
          Height = 1.0f + x * 0.5f,
          PassType = PassType.Front,
          CCV = heightOnly ? CellPassConsts.NullCCV : (short)(10 + 10 * x),
          MachineSpeed = heightOnly ? Consts.NullMachineSpeed : (ushort)(650 + x),
          MDP = heightOnly ? CellPassConsts.NullMDP : (short)(20 + 20 * x),
          MaterialTemperature = heightOnly ? CellPassConsts.NullMaterialTemperatureValue : (ushort)(1000 + x)
        }).ToArray();

      //The subgrid tree extents are 1 << 30 or ~ 1 billion.
      //The default origin offset (SubGridTreeConsts.DefaultIndexOriginOffset) is ~500 million.
      //So we are placing the cell at the world origin (N/E) and default cell size of 0.34 Meters
      DITAGFileAndSubGridRequestsFixture.AddSingleCellWithPasses
        (siteModel, SubGridTreeConsts.DefaultIndexOriginOffset, SubGridTreeConsts.DefaultIndexOriginOffset, cellPasses, 1, cellPasses.Length);
      DITAGFileAndSubGridRequestsFixture.ConvertSiteModelToImmutable(siteModel);

      //Add the machine targets for summaries
      var minUTCDate = Consts.MIN_DATETIME_AS_UTC;
      siteModel.MachinesTargetValues[bulldozerMachineIndex].TargetPassCountStateEvents.PutValueAtDate(minUTCDate, 10);
      siteModel.MachinesTargetValues[bulldozerMachineIndex].TargetCCVStateEvents.PutValueAtDate(minUTCDate, 220);
      siteModel.MachinesTargetValues[bulldozerMachineIndex].TargetMDPStateEvents.PutValueAtDate(minUTCDate, 880);
      siteModel.MachinesTargetValues[bulldozerMachineIndex].TargetMinMaterialTemperature.PutValueAtDate(minUTCDate, 900);
      siteModel.MachinesTargetValues[bulldozerMachineIndex].TargetMaxMaterialTemperature.PutValueAtDate(minUTCDate, 1200);

      return siteModel;
    }

    #region Cluster Compute
    private CellDatumRequestArgument_ClusterCompute CreateCellDatumRequestArgument_ClusterCompute(ISiteModel siteModel, DesignOffset referenceDesign, DisplayMode mode, IOverrideParameters overrides)
    {
      //The single cell is at world origin
      var coords = new XYZ(0.1, 0.1);
      siteModel.Grid.CalculateIndexOfCellContainingPosition(coords.X, coords.Y, out int OTGCellX, out int OTGCellY);

      return new CellDatumRequestArgument_ClusterCompute
      {
        ProjectID = siteModel.ID,
        Filters = new FilterSet(new CombinedFilter()),
        Mode = mode,
        NEECoords = coords,
        OTGCellX = OTGCellX,
        OTGCellY = OTGCellY,
        ReferenceDesign = referenceDesign,
        Overrides = overrides
      };
    }

    [Fact]
    public void Test_CellDatumRequest_ClusterCompute_Creation()
    {
      var request = new CellDatumRequest_ClusterCompute();
      request.Should().NotBeNull();
    }

    [Fact]
    public async Task Test_CellDatumRequest_ClusterCompute_Execute_EmptySiteModel()
    {
      AddApplicationGridRouting();
      AddClusterComputeGridRouting();

      var siteModel = DITAGFileAndSubGridRequestsWithIgniteFixture.NewEmptyModel();
      var request = new CellDatumRequest_ClusterCompute();

      var response = await request.ExecuteAsync(CreateCellDatumRequestArgument_ClusterCompute(siteModel, new DesignOffset(), DisplayMode.Height, new OverrideParameters()), new SubGridSpatialAffinityKey());

      response.Should().NotBeNull();
      Assert.Equal(CellDatumReturnCode.NoValueFound, response.ReturnCode);
    }

    [Theory]
    [InlineData(DisplayMode.PassCount, 10, false)]
    [InlineData(DisplayMode.PassCountSummary, 100.0, false)]
    [InlineData(DisplayMode.PassCountSummary, 125.0, true)]
    [InlineData(DisplayMode.CCV, 110, false)]
    [InlineData(DisplayMode.CCV, 110, true)]
    [InlineData(DisplayMode.CCVPercent, 50.0, false)]
    [InlineData(DisplayMode.CCVPercent, 25.0, true)]
    [InlineData(DisplayMode.CCVSummary, 50.0, false)]
    [InlineData(DisplayMode.CCVSummary, 25.0, true)]
    [InlineData(DisplayMode.CCVPercentSummary, 50.0, false)]
    [InlineData(DisplayMode.CCVPercentSummary, 25.0, true)]
    [InlineData(DisplayMode.CCVPercentChange, 10.0, false)]
    [InlineData(DisplayMode.MDP, 220, false)]
    [InlineData(DisplayMode.MDP, 220, true)]
    [InlineData(DisplayMode.MDPPercent, 25.0, false)]
    [InlineData(DisplayMode.MDPPercent, 50.0, true)]
    [InlineData(DisplayMode.MDPPercentSummary, 25.0, false)]
    [InlineData(DisplayMode.MDPPercentSummary, 50.0, true)]
    [InlineData(DisplayMode.MDPSummary, 25.0, false)]
    [InlineData(DisplayMode.MDPSummary, 50.0, true)]
    [InlineData(DisplayMode.Height, 6.0, false)]
    [InlineData(DisplayMode.TemperatureDetail, 101.0, false)]
    [InlineData(DisplayMode.TemperatureSummary, 101.0, false)]
    [InlineData(DisplayMode.MachineSpeed, 660, false)]
    [InlineData(DisplayMode.CutFill, 3.5, false)]//1.5 offset from 5
    public async Task Test_CellDatumRequest_ClusterCompute_Execute_SingleCellSiteModelLastPass(DisplayMode mode, double expectedValue, bool withOverrides)
    {
      AddApplicationGridRouting();
      AddClusterComputeGridRouting();
      AddDesignProfilerGridRouting();

      var baseTime = DateTime.UtcNow;
      var siteModel = BuildModelForSingleCellDatum(baseTime);
      var designUid = DITAGFileAndSubGridRequestsWithIgniteFixture.ConstructSingleFlatTriangleDesignAboutOrigin(ref siteModel, 1.0f);
      var referenceDesign = new DesignOffset(designUid, 1.5);
      var overrides = withOverrides
        ? new OverrideParameters
        {
          OverrideMachineCCV = true,
          OverridingMachineCCV = 440,
          OverrideMachineMDP = true,
          OverridingMachineMDP = 440,
          OverrideTargetPassCount = true,
          OverridingTargetPassCountRange = new PassCountRangeRecord(4, 8)
          //others not used in cell datum
        }
        : new OverrideParameters();
      var request = new CellDatumRequest_ClusterCompute();
      var arg = CreateCellDatumRequestArgument_ClusterCompute(siteModel, referenceDesign, mode, overrides);
      var response = await request.ExecuteAsync(arg, new SubGridSpatialAffinityKey(SubGridSpatialAffinityKey.DEFAULT_SPATIAL_AFFINITY_VERSION_NUMBER_TICKS, arg.ProjectID, arg.OTGCellX, arg.OTGCellY));

      response.Should().NotBeNull();
      Assert.Equal(CellDatumReturnCode.ValueFound, response.ReturnCode);
      Assert.Equal(expectedValue, response.Value);
      Assert.Equal(baseTime.AddMinutes(10), response.TimeStampUTC);
    }

    [Theory]
    [InlineData(DisplayMode.PassCount)]
    [InlineData(DisplayMode.PassCountSummary)]
    [InlineData(DisplayMode.CCV)]
    [InlineData(DisplayMode.CCVPercent)]
    [InlineData(DisplayMode.CCVPercentSummary)]
    [InlineData(DisplayMode.CCVPercentChange)]
    [InlineData(DisplayMode.MDP)]
    [InlineData(DisplayMode.MDPPercent)]
    [InlineData(DisplayMode.MDPPercentSummary)]
    [InlineData(DisplayMode.MDPSummary)]
    [InlineData(DisplayMode.Height)]
    [InlineData(DisplayMode.TemperatureDetail)]
    [InlineData(DisplayMode.TemperatureSummary)]
    [InlineData(DisplayMode.MachineSpeed)]
    [InlineData(DisplayMode.CutFill)]
    public async Task Test_CellDatumRequest_ClusterCompute_Execute_SingleCellSiteModelMinimalValues(DisplayMode mode)
    {
      AddApplicationGridRouting();
      AddClusterComputeGridRouting();
      AddDesignProfilerGridRouting();

      var baseTime = DateTime.UtcNow;
      var siteModel = BuildModelForSingleCellDatum(baseTime, true);
      var designUid = DITAGFileAndSubGridRequestsWithIgniteFixture.ConstructSingleFlatTriangleDesignAboutOrigin(ref siteModel, 1.0f);
      var referenceDesign = new DesignOffset(designUid, 0);
      var request = new CellDatumRequest_ClusterCompute();
      var arg = CreateCellDatumRequestArgument_ClusterCompute(siteModel, referenceDesign, mode, new OverrideParameters());
      var response = await request.ExecuteAsync(arg, new SubGridSpatialAffinityKey(SubGridSpatialAffinityKey.DEFAULT_SPATIAL_AFFINITY_VERSION_NUMBER_TICKS, arg.ProjectID, arg.OTGCellX, arg.OTGCellY));

      response.Should().NotBeNull();
      //Only elevation and pass count
      var expected = CellDatumReturnCode.NoValueFound;
      switch (mode)
      {
        case DisplayMode.Height:
        case DisplayMode.CutFill:
        case DisplayMode.PassCount:
        case DisplayMode.PassCountSummary:
          expected = CellDatumReturnCode.ValueFound;
          break;
      }
      Assert.Equal(expected, response.ReturnCode);
    }

    [Fact]
    public void Test_CellDatumRequest_ClusterCompute_Execute_MissingDesign()
    {
      AddApplicationGridRouting();
      AddClusterComputeGridRouting();

      var siteModel = DITAGFileAndSubGridRequestsWithIgniteFixture.NewEmptyModel();
      var request = new CellDatumRequest_ClusterCompute();

      Assert.ThrowsAsync<ArgumentException>(async () => await request.ExecuteAsync(CreateCellDatumRequestArgument_ClusterCompute(siteModel, new DesignOffset(Guid.NewGuid(), -0.5), DisplayMode.Height, new OverrideParameters()), new SubGridSpatialAffinityKey()));
    }

    [Fact]
    public async Task Test_CellDatumRequest_ClusterCompute_Execute_MissingSiteModel()
    {
      AddApplicationGridRouting();
      AddClusterComputeGridRouting();

      var siteModel = DITAGFileAndSubGridRequestsWithIgniteFixture.NewEmptyModel();
      var arg = CreateCellDatumRequestArgument_ClusterCompute(siteModel, new DesignOffset(), DisplayMode.Height, new OverrideParameters());
      arg.ProjectID = Guid.NewGuid();

      var request = new CellDatumRequest_ClusterCompute();
      var response = await request.ExecuteAsync(arg, new SubGridSpatialAffinityKey());

      response.Should().NotBeNull();
      Assert.Equal(CellDatumReturnCode.UnexpectedError, response.ReturnCode);
    }
    #endregion

    #region Application Service
    private CellDatumRequestArgument_ApplicationService CreateCellDatumRequestArgument_ApplicationService(ISiteModel siteModel, DesignOffset referenceDesign, DisplayMode mode, IOverrideParameters overrides)
    {
      //The single cell is at world origin
      var coords = new XYZ(0.1, 0.1, 0);

      return new CellDatumRequestArgument_ApplicationService
      {
        ProjectID = siteModel.ID,
        Filters = new FilterSet(new CombinedFilter()),
        Mode = mode,
        Point = coords,
        ReferenceDesign = referenceDesign,
        CoordsAreGrid = true,
        Overrides = overrides
      };
    }

    [Fact]
    public void Test_CellDatumRequest_ApplicationService_Creation()
    {
      var request = new CellDatumRequest_ApplicationService();
      request.Should().NotBeNull();
    }

    [Fact]
    public async Task Test_CellDatumRequest_ApplicationService_Execute_EmptySiteModel()
    {
      AddApplicationGridRouting();
      AddClusterComputeGridRouting();

      var siteModel = DITAGFileAndSubGridRequestsWithIgniteFixture.NewEmptyModel();
      var request = new CellDatumRequest_ApplicationService();
      var response = await request.ExecuteAsync(CreateCellDatumRequestArgument_ApplicationService(siteModel, new DesignOffset(), DisplayMode.Height, new OverrideParameters()));

      response.Should().NotBeNull();
      Assert.Equal(CellDatumReturnCode.NoValueFound, response.ReturnCode);
    }

    [Theory]
    [InlineData(DisplayMode.PassCount, 10)]
    [InlineData(DisplayMode.PassCountSummary, 100.0)]
    [InlineData(DisplayMode.CCV, 110)]
    [InlineData(DisplayMode.CCVPercent, 25.0)]
    [InlineData(DisplayMode.CCVSummary, 25.0)]
    [InlineData(DisplayMode.CCVPercentSummary, 25.0)]
    [InlineData(DisplayMode.CCVPercentChange, 10.0)]
    [InlineData(DisplayMode.MDP, 220)]
    [InlineData(DisplayMode.MDPPercent, 25.0)]
    [InlineData(DisplayMode.MDPPercentSummary, 25.0)]
    [InlineData(DisplayMode.MDPSummary, 25.0)]
    [InlineData(DisplayMode.Height, 6.0)]
    [InlineData(DisplayMode.TemperatureDetail, 101.0)]
    [InlineData(DisplayMode.TemperatureSummary, 101.0)]
    [InlineData(DisplayMode.MachineSpeed, 660)]
    [InlineData(DisplayMode.CutFill, 3.5)]//1.5 offset from 5
    public async Task Test_CellDatumRequest_ApplicationService_Execute_SingleCellSiteModelLastPass(DisplayMode mode, double expectedValue)
    {
      AddApplicationGridRouting();
      AddClusterComputeGridRouting();
      AddDesignProfilerGridRouting();

      var baseTime = DateTime.UtcNow;
      var siteModel = BuildModelForSingleCellDatum(baseTime);
      var designUid = DITAGFileAndSubGridRequestsWithIgniteFixture.ConstructSingleFlatTriangleDesignAboutOrigin(ref siteModel, 1.0f);
      var referenceDesign = new DesignOffset(designUid, 1.5);
      //Just do one override to test it's hooked up. The rest are tested in the cluster compute tests
      var overrides = new OverrideParameters
      {
        OverrideMachineCCV = true,
        OverridingMachineCCV = 440
      };
      var request = new CellDatumRequest_ApplicationService();
      var response = await request.ExecuteAsync(CreateCellDatumRequestArgument_ApplicationService(siteModel, referenceDesign, mode, overrides));

      response.Should().NotBeNull();
      Assert.Equal(CellDatumReturnCode.ValueFound, response.ReturnCode);
      Assert.Equal(expectedValue, response.Value);
      Assert.Equal(baseTime.AddMinutes(10), response.TimeStampUTC);
    }

    [Fact]
    public async Task Test_CellDatumRequest_ApplicationService_Execute_SingleCellSiteModel_LLH()
    {
      AddApplicationGridRouting();
      AddClusterComputeGridRouting();
      AddDesignProfilerGridRouting();

      var baseTime = DateTime.UtcNow;
      var siteModel = BuildModelForSingleCellDatum(baseTime);

      DITAGFileAndSubGridRequestsWithIgniteFixture.AddCSIBToSiteModel(ref siteModel, TestCommonConsts.DIMENSIONS_2012_DC_CSIB);
      siteModel.CSIB().Should().Be(TestCommonConsts.DIMENSIONS_2012_DC_CSIB);

      var arg = CreateCellDatumRequestArgument_ApplicationService(siteModel, new DesignOffset(), DisplayMode.Height, new OverrideParameters());
      arg.Point = DIContext.Obtain<ICoreXWrapper>().NEEToLLH(siteModel.CSIB(), arg.Point.ToCoreX_XYZ()).ToTRex_XYZ();
      arg.CoordsAreGrid = false;

      var request = new CellDatumRequest_ApplicationService();
      var response = await request.ExecuteAsync(arg);

      response.Should().NotBeNull();
      Assert.Equal(CellDatumReturnCode.ValueFound, response.ReturnCode);
      Assert.Equal(6.0, response.Value);
      Assert.Equal(baseTime.AddMinutes(10), response.TimeStampUTC);
    }

    [Fact]
    public async Task Test_CellDatumRequest_ApplicationService_Execute_SingleCellSiteModel_Outside()
    {
      AddApplicationGridRouting();
      AddClusterComputeGridRouting();
      AddDesignProfilerGridRouting();

      var baseTime = DateTime.UtcNow;
      var siteModel = BuildModelForSingleCellDatum(baseTime);

      var arg = CreateCellDatumRequestArgument_ApplicationService(siteModel, new DesignOffset(), DisplayMode.Height, new OverrideParameters());
      arg.Point = new XYZ(123456, 123456);

      var request = new CellDatumRequest_ApplicationService();
      var response = await request.ExecuteAsync(arg);

      response.Should().NotBeNull();
      Assert.Equal(CellDatumReturnCode.NoValueFound, response.ReturnCode);
    }

    [Fact]
    public async Task Test_CellDatumRequest_ApplicationService_Execute_MissingSiteModel()
    {
      AddApplicationGridRouting();
      AddClusterComputeGridRouting();

      var siteModel = DITAGFileAndSubGridRequestsWithIgniteFixture.NewEmptyModel();
      var arg = CreateCellDatumRequestArgument_ApplicationService(siteModel, new DesignOffset(), DisplayMode.Height, new OverrideParameters());
      arg.ProjectID = Guid.NewGuid();

      var request = new CellDatumRequest_ApplicationService();
      var response = await request.ExecuteAsync(arg);

      response.Should().NotBeNull();
      Assert.Equal(CellDatumReturnCode.UnexpectedError, response.ReturnCode);

    }
    #endregion

  }
}

