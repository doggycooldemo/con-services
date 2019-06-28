﻿using FluentAssertions;
using VSS.TRex.Designs.GridFabric.Arguments;
using VSS.TRex.Designs.GridFabric.ComputeFuncs;
using VSS.TRex.Designs.GridFabric.Requests;
using VSS.TRex.Designs.GridFabric.Responses;
using VSS.TRex.Designs.Models;
using VSS.TRex.Filters;
using VSS.TRex.Geometry;
using VSS.TRex.SubGridTrees.Interfaces;
using VSS.TRex.Tests.TestFixtures;
using Xunit;

namespace VSS.TRex.Tests.Designs.GridFabric
{
  [UnitTestCoveredRequest(RequestType = typeof(DesignBoundaryRequest))]
  public class DesignBoundaryRequestTests : IClassFixture<DITAGFileAndSubGridRequestsWithIgniteFixture>
  {
    private void AddDesignProfilerGridRouting() => IgniteMock.AddApplicationGridRouting
      <DesignBoundaryComputeFunc, DesignBoundaryArgument, DesignBoundaryResponse>();

    [Fact]
    public void Test_DesignProfileRequest_Creation()
    {
      var request = new DesignBoundaryRequest();

      request.Should().NotBeNull();
    }

    [Fact]
    public void Test_DesignBoundaryRequest()
    {
      const int EXPECTED_BOUNDARY_COUNT = 1;
      const int EXPECTED_BOUNDARY_POINTS_COUNT = 6;

      AddDesignProfilerGridRouting();

      var siteModel = DITAGFileAndSubGridRequestsWithIgniteFixture.NewEmptyModel();
      var designUid = DITAGFileAndSubGridRequestsWithIgniteFixture.AddDesignToSiteModel(ref siteModel, TestHelper.CommonTestDataPath, "Bug36372.ttm", false);
      var referenceDesign = new DesignOffset(designUid, 0);

      var request = new DesignBoundaryRequest();
      var response = request.Execute(new DesignBoundaryArgument()
      {
        ProjectID = siteModel.ID,
        CellSize = SubGridTreeConsts.DefaultCellSize,
        ReferenceDesign = referenceDesign,
        Filters = new FilterSet(new CombinedFilter()),
        TRexNodeID = "UnitTest_TRexNodeID"
      });

      response.RequestResult.Should().Be(DesignProfilerRequestResult.OK);
      response.Boundary.Count.Should().Be(EXPECTED_BOUNDARY_COUNT);
      response.Boundary[0].Points.Count.Should().Be(EXPECTED_BOUNDARY_POINTS_COUNT);
    }
  }
}