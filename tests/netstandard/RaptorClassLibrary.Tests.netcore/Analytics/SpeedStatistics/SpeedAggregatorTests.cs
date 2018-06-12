﻿using System;
using VSS.TRex.Tests.netcore.Analytics.Common;
using VSS.TRex.Analytics.SpeedStatistics;
using VSS.TRex.Cells;
using VSS.TRex.SubGridTrees.Client;
using VSS.TRex.SubGridTrees.Interfaces;
using VSS.TRex.Types;
using Xunit;

namespace VSS.TRex.Tests.netcore.Analytics.SpeedStatistics
{
  public class SpeedAggregatorTests : BaseTests
  {
    [Fact]
    public void Test_SpeedAggregator_Creation()
    {
      var aggregator = new SpeedAggregator();

      Assert.True(aggregator.SiteModelID == Guid.Empty, "Invalid initial value for SiteModelID.");
      Assert.True(aggregator.CellSize < TOLERANCE, "Invalid initial value for CellSize.");
      Assert.True(aggregator.SummaryCellsScanned == 0, "Invalid initial value for SummaryCellsScanned.");
      Assert.True(aggregator.CellsScannedOverTarget == 0, "Invalid initial value for CellsScannedOverTarget.");
      Assert.True(aggregator.CellsScannedAtTarget == 0, "Invalid initial value for CellsScannedAtTarget.");
      Assert.True(aggregator.CellsScannedUnderTarget == 0, "Invalid initial value for CellsScannedUnderTarget.");
      Assert.True(aggregator.IsTargetValueConstant, "Invalid initial value for IsTargetValueConstant.");
      Assert.True(!aggregator.MissingTargetValue, "Invalid initial value for MissingTargetValue.");
      Assert.True(aggregator.TargetMachineSpeed.Max == CellPass.NullMachineSpeed, "Invalid initial value for TargetMachineSpeed.Max.");
      Assert.True(aggregator.TargetMachineSpeed.Min == CellPass.NullMachineSpeed, "Invalid initial value for TargetMachineSpeed.Min.");
    }

    [Fact]
    public void Test_SpeedAggregator_ProcessResult_NoAggregation()
    {
      var aggregator = new SpeedAggregator();

      var clientGrid = ClientLeafSubgridFactoryFactory.Factory().GetSubGrid(GridDataType.MachineSpeedTarget) as ClientMachineTargetSpeedLeafSubGrid;

      clientGrid.FillWithTestPattern();

      var dLength = clientGrid.Cells.Length;
      var length = (short)Math.Sqrt(dLength);
      aggregator.CellSize = CELL_SIZE;
      aggregator.TargetMachineSpeed = new MachineSpeedExtendedRecord((ushort)(length - 1), (ushort)(length - 1));

      IClientLeafSubGrid[][] subGrids = new[] { new[] { clientGrid } };

      aggregator.ProcessSubgridResult(subGrids);

      Assert.True(aggregator.SummaryCellsScanned == dLength, "Invalid value for SummaryCellsScanned.");
      Assert.True(Math.Abs(aggregator.SummaryProcessedArea - dLength * Math.Pow(aggregator.CellSize, 2)) < TOLERANCE, "Invalid value for SummaryProcessedArea.");
      Assert.True(aggregator.CellsScannedAtTarget == length, "Invalid value for CellsScannedAtTarget.");
      Assert.True(aggregator.CellsScannedOverTarget == (dLength - length) / 2, "Invalid value for CellsScannedOverTarget.");
      Assert.True(aggregator.CellsScannedUnderTarget == (dLength - length) / 2, "Invalid value for CellsScannedUnderTarget.");
    }

    [Fact]
    public void Test_SpeedAggregator_ProcessResult_WithAggregation()
    {
      var aggregator = new SpeedAggregator();

      var clientGrid = ClientLeafSubgridFactoryFactory.Factory().GetSubGrid(GridDataType.MachineSpeedTarget) as ClientMachineTargetSpeedLeafSubGrid;

      clientGrid.FillWithTestPattern();

      var dLength = clientGrid.Cells.Length;
      var length = (short)Math.Sqrt(dLength);
      aggregator.CellSize = CELL_SIZE;
      aggregator.TargetMachineSpeed = new MachineSpeedExtendedRecord((ushort)(length - 1), (ushort)(length - 1));

      IClientLeafSubGrid[][] subGrids = new[] { new[] { clientGrid } };

      aggregator.ProcessSubgridResult(subGrids);

      // Other aggregator...
      var otherAggregator = new SpeedAggregator();

      otherAggregator.CellSize = CELL_SIZE;
      otherAggregator.TargetMachineSpeed = new MachineSpeedExtendedRecord((ushort)(length - 1), (ushort)(length - 1));

      otherAggregator.ProcessSubgridResult(subGrids);

      aggregator.AggregateWith(otherAggregator);

      Assert.True(aggregator.SummaryCellsScanned == dLength * 2, "Invalid value for SummaryCellsScanned.");
      Assert.True(Math.Abs(aggregator.SummaryProcessedArea - 2 * dLength * Math.Pow(aggregator.CellSize, 2)) < TOLERANCE, "Invalid value for SummaryProcessedArea.");
      Assert.True(aggregator.CellsScannedAtTarget == length * 2, "Invalid value for CellsScannedAtTarget.");
      Assert.True(aggregator.CellsScannedOverTarget == dLength - length, "Invalid value for CellsScannedOverTarget.");
      Assert.True(aggregator.CellsScannedUnderTarget == dLength - length, "Invalid value for CellsScannedUnderTarget.");
    }
  }
}
