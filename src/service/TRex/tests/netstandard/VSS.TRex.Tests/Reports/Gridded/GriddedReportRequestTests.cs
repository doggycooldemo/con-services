﻿using System;
using System.Threading.Tasks;
using FluentAssertions;
using VSS.Productivity3D.Models.Models.Reports;
using VSS.Productivity3D.WebApi.Models.Compaction.Models.Reports;
using VSS.TRex.Cells;
using VSS.TRex.Common.Models;
using VSS.TRex.Filters;
using VSS.TRex.Reports.Gridded.GridFabric;
using VSS.TRex.SubGrids.GridFabric.Arguments;
using VSS.TRex.SubGrids.GridFabric.ComputeFuncs;
using VSS.TRex.SubGrids.Interfaces;
using VSS.TRex.SubGrids.Responses;
using VSS.TRex.SubGridTrees.Core.Utilities;
using VSS.TRex.SubGridTrees.Interfaces;
using VSS.TRex.Tests.TestFixtures;
using Xunit;

namespace VSS.TRex.Tests.Reports.Gridded
{
  [UnitTestCoveredRequest(RequestType = typeof(GriddedReportRequest))]
  public class GriddedReportRequestTests : IClassFixture<DITAGFileAndSubGridRequestsWithIgniteFixture>
  {
    [Fact]
    public void Creation()
    {
      var request = new GriddedReportRequest();
      request.Should().NotBeNull();
    }

    private void AddApplicationGridRouting() => IgniteMock.Immutable.AddApplicationGridRouting<GriddedReportRequestComputeFunc, GriddedReportRequestArgument, GriddedReportRequestResponse>();

    private void AddClusterComputeGridRouting()
    {
      IgniteMock.Immutable.AddClusterComputeGridRouting<SubGridsRequestComputeFuncProgressive<SubGridsRequestArgument, SubGridRequestsResponse>, SubGridsRequestArgument, SubGridRequestsResponse>();
      IgniteMock.Immutable.AddClusterComputeGridRouting<SubGridProgressiveResponseRequestComputeFunc, ISubGridProgressiveResponseRequestComputeFuncArgument, bool>();
    }

    private GriddedReportRequestArgument SimpleGriddedReportRequestArgument(Guid projectUid)
    {
      return new GriddedReportRequestArgument
      { 
        GridInterval = 2,
        Filters = new FilterSet(new CombinedFilter()),
        GridReportOption = GridReportOption.Automatic,
        StartNorthing = 800,
        StartEasting = 300,
        EndNorthing = 1000,
        EndEasting = 500,
        Azimuth = 0,
        TRexNodeID = Guid.NewGuid(),
        ProjectID = projectUid,
        Overrides = new OverrideParameters()
      };
    }

    [Fact]
    public async Task Execute_EmptySiteModel()
    {
      AddApplicationGridRouting();
      var siteModel = DITAGFileAndSubGridRequestsWithIgniteFixture.NewEmptyModel();
      var request = new GriddedReportRequest();
      var response = await request.ExecuteAsync(SimpleGriddedReportRequestArgument(siteModel.ID));

      response.Should().NotBeNull();
      //response.GriddedReportDataRowList.Should().Be(??)
      //response.ReturnCode.Should().Be(??)
    }

    [Theory]
    [InlineData(2.0, 36)]
    [InlineData(0.34, SubGridTreeConsts.CellsPerSubGrid)]
    public async Task Execute_SingleSubGridSingleCell_ConstantElevation(double interval, int expectedRows)
    {
      AddApplicationGridRouting();
      AddClusterComputeGridRouting();

      var siteModel = DITAGFileAndSubGridRequestsWithIgniteFixture.NewEmptyModel();

      var cellPasses = new CellPass[SubGridTreeConsts.SubGridTreeDimension, SubGridTreeConsts.SubGridTreeDimension][];
      var baseTime = DateTime.UtcNow;

      SubGridUtilities.SubGridDimensionalIterator((x, y) =>
      {
        cellPasses[x, y] = new[]
        {
          new CellPass
          {
            Time = baseTime,
            Height = 100.0f
          }
        };
      });

      DITAGFileAndSubGridRequestsFixture.AddSingleSubGridWithPasses(siteModel,
        SubGridTreeConsts.DefaultIndexOriginOffset, SubGridTreeConsts.DefaultIndexOriginOffset, cellPasses);

      var request = new GriddedReportRequest();
      var argument = SimpleGriddedReportRequestArgument(siteModel.ID);
      argument.GridInterval = interval;

      var response = await request.ExecuteAsync(argument);

      response.Should().NotBeNull();
      response.ReturnCode.Should().Be(ReportReturnCode.NoError);
      response.GriddedReportDataRowList.Should().NotBeNull();
      response.GriddedReportDataRowList.Count.Should().Be(expectedRows);
    }
  }
}


