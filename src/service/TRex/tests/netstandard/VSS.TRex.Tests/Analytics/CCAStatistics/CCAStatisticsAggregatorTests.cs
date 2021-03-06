﻿using System;
using VSS.TRex.Analytics.CCAStatistics;
using VSS.TRex.Common;
using VSS.TRex.Types.CellPasses;
using VSS.TRex.SubGridTrees.Client;
using VSS.TRex.SubGridTrees.Client.Interfaces;
using VSS.TRex.Tests.Analytics.Common;
using VSS.TRex.Tests.TestFixtures;
using VSS.TRex.Types;
using Xunit;

namespace VSS.TRex.Tests.Analytics.CCAStatistics
{
  public class CCAStatisticsAggregatorTests : IClassFixture<DILoggingFixture>
  {
    [Fact]
    public void Test_CCAStatisticsAggregator_Creation()
    {
      var aggregator = new CCAStatisticsAggregator();

      Assert.True(aggregator.SiteModelID == Guid.Empty, "Invalid initial value for SiteModelID.");
      Assert.True(aggregator.CellSize < Consts.TOLERANCE_DIMENSION, "Invalid initial value for CellSize.");
      Assert.True(aggregator.SummaryCellsScanned == 0, "Invalid initial value for SummaryCellsScanned.");
      Assert.True(aggregator.CellsScannedOverTarget == 0, "Invalid initial value for CellsScannedOverTarget.");
      Assert.True(aggregator.CellsScannedAtTarget == 0, "Invalid initial value for CellsScannedAtTarget.");
      Assert.True(aggregator.CellsScannedUnderTarget == 0, "Invalid initial value for CellsScannedUnderTarget.");
      Assert.True(aggregator.IsTargetValueConstant, "Invalid initial value for IsTargetValueConstant.");
      Assert.True(!aggregator.MissingTargetValue, "Invalid initial value for MissingTargetValue.");
      Assert.True(aggregator.LastTargetCCA == CellPassConsts.NullCCA, "Invalid initial value for LastTargetCCA.");
    }

    [Fact]
    public void Test_CCAStatisticsAggregator_ProcessResult_NoAggregation()
    {
      var aggregator = new CCAStatisticsAggregator();

      var clientGrid = new ClientCCALeafSubGrid();

      clientGrid.FillWithTestPattern();

      var dLength = clientGrid.Cells.Length;
      aggregator.CellSize = TestConsts.CELL_SIZE;

      IClientLeafSubGrid[][] subGrids = new[] { new[] { clientGrid } };

      aggregator.ProcessSubGridResult(subGrids);

      Assert.True(aggregator.SummaryCellsScanned == dLength, "Invalid value for SummaryCellsScanned.");
      Assert.True(Math.Abs(aggregator.SummaryProcessedArea - dLength * Math.Pow(aggregator.CellSize, 2)) < Consts.TOLERANCE_DIMENSION, "Invalid value for SummaryProcessedArea.");
      Assert.True(aggregator.CellsScannedAtTarget == dLength, "Invalid value for CellsScannedAtTarget.");
      Assert.True(aggregator.CellsScannedOverTarget == 0, "Invalid value for CellsScannedOverTarget.");
      Assert.True(aggregator.CellsScannedUnderTarget == 0, "Invalid value for CellsScannedUnderTarget.");
    }

    [Fact]
    public void Test_CCAAggregator_ProcessResult_WithAggregation()
    {
      var aggregator = new CCAStatisticsAggregator();

      var clientGrid = ClientLeafSubGridFactoryFactory.CreateClientSubGridFactory().GetSubGrid(GridDataType.CCA) as ClientCCALeafSubGrid;

      clientGrid.FillWithTestPattern();

      var dLength = clientGrid.Cells.Length;
      aggregator.CellSize = TestConsts.CELL_SIZE;

      IClientLeafSubGrid[][] subGrids = new[] { new[] { clientGrid } };

      aggregator.ProcessSubGridResult(subGrids);

      // Other aggregator...
      var otherAggregator = new CCAStatisticsAggregator();

      otherAggregator.CellSize = TestConsts.CELL_SIZE;


      otherAggregator.ProcessSubGridResult(subGrids);

      aggregator.AggregateWith(otherAggregator);

      Assert.True(aggregator.SummaryCellsScanned == dLength * 2, "Invalid value for SummaryCellsScanned.");
      Assert.True(Math.Abs(aggregator.SummaryProcessedArea - 2 * dLength * Math.Pow(aggregator.CellSize, 2)) < Consts.TOLERANCE_DIMENSION, "Invalid value for SummaryProcessedArea.");
      Assert.True(aggregator.CellsScannedAtTarget == dLength * 2, "Invalid value for CellsScannedAtTarget.");
      Assert.True(aggregator.CellsScannedOverTarget == 0, "Invalid value for CellsScannedOverTarget.");
      Assert.True(aggregator.CellsScannedUnderTarget == 0, "Invalid value for CellsScannedUnderTarget.");
    }
  }
}
