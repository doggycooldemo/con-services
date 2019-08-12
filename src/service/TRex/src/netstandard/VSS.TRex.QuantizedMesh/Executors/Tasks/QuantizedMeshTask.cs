﻿using System;
using Microsoft.Extensions.Logging;
using VSS.TRex.Common.CellPasses;
using VSS.TRex.Pipelines.Tasks;
using VSS.TRex.QuantizedMesh.Models;
using VSS.TRex.SubGridTrees;
using VSS.TRex.SubGridTrees.Client;
using VSS.TRex.SubGridTrees.Client.Interfaces;
using VSS.TRex.SubGridTrees.Interfaces;

namespace VSS.TRex.QuantizedMesh.Executors.Tasks
{
  /// <summary>
  /// A Task specialized towards rendering quantized mesh tiles
  /// </summary>
  public class QuantizedMeshTask : PipelinedSubGridTask
  {
    private static readonly ILogger Log = Logging.Logger.CreateLogger<QuantizedMeshTask>();

    /// <summary>
    /// The action (via a delegate) this task will perform on each of the sub grids transferred to it
    /// </summary>
    public QuantizedMeshTask()
    {
    }

    public GriddedElevDataRow[,] GriddedElevDataArray;
    public double GridIntervalX;
    public double GridIntervalY;
    public int GridSize;
    public double TileMinX;
    public double TileMinY;
    public double TileMaxX;
    public double TileMaxY;
    public float MinElevation = float.PositiveInfinity;
    public float MaxElevation = float.NegativeInfinity;
    public float LowestElevation = 0;
    public double TotalSampled = 0;
    public double TotalUsed = 0;
    private int TileRangeMinX;
    private int TileRangeMinY;
    private int TileRangeMaxX;
    private int TileRangeMaxY;

    /// <summary>
    /// Populate result grid from processed subgrid
    /// </summary>
    /// <param name="subGrid"></param>
    private void DummyExtractRequiredValues(ClientHeightLeafSubGrid subGrid)
    {
      var cnt = 0.0;
      var cnt2 = 0.0;
      // var eff = 0.0;

      // Calculate cell range we are interested in
      subGrid.CalculateWorldOrigin(out double subGridWorldOriginX, out double subGridWorldOriginY);

      var topX = subGridWorldOriginX + subGrid.CellSize * SubGridTreeConsts.SubGridTreeDimension;
      var topY = subGridWorldOriginY + subGrid.CellSize * SubGridTreeConsts.SubGridTreeDimension;

      double rangeMinX = (subGridWorldOriginX - TileMinX) / GridIntervalX;
      if (Math.Truncate(rangeMinX) != rangeMinX)
        TileRangeMinX = (int)(Math.Truncate(rangeMinX)) + 1;
      if (TileRangeMinX < 0)
        TileRangeMinX = 0;

      double rangeMinY = (subGridWorldOriginY - TileMinY) / GridIntervalY;
      if (Math.Truncate(rangeMinY) != rangeMinY)
        TileRangeMinY = (int)(Math.Truncate(rangeMinY)) + 1;
      if (TileRangeMinY < 0)
        TileRangeMinY = 0;

      double rangeMaxX = (topX - TileMinX) / GridIntervalX;
      if (Math.Truncate(rangeMaxX) != rangeMaxX)
        TileRangeMaxX = (int)(Math.Truncate(rangeMaxX));
      if (TileRangeMaxX > GridSize - 1)
        TileRangeMaxX = GridSize - 1;

      double rangeMaxY = (topY - TileMinY) / GridIntervalY;
      if (Math.Truncate(rangeMaxY) != rangeMaxY)
        TileRangeMaxY = (int)(Math.Truncate(rangeMaxY));
      if (TileRangeMaxY > GridSize - 1)
        TileRangeMaxY = GridSize - 1;

      for (int y = TileRangeMinY; y <= TileRangeMaxY; y++)
        for (int x = TileRangeMinX; x <= TileRangeMaxX; x++)
        {
          cnt2++;
          SubGridTree.CalculateIndexOfCellContainingPosition(GriddedElevDataArray[x, y].Easting, GriddedElevDataArray[x, y].Northing,
          subGrid.CellSize,
          subGrid.IndexOriginOffset,
          out int CellX, out int CellY);
          subGrid.GetOTGLeafSubGridCellIndex(CellX, CellY, out var subGridX, out var subGridY);
          if (subGrid.Cells[subGridX, subGridY] != CellPassConsts.NullHeight)
          {
            cnt++;
            GriddedElevDataArray[x, y].Elevation = subGrid.Cells[subGridX, subGridY];
            if (GriddedElevDataArray[x, y].Elevation < MinElevation)
              MinElevation = GriddedElevDataArray[x, y].Elevation;
            if (GriddedElevDataArray[x, y].Elevation > MaxElevation)
              MaxElevation = GriddedElevDataArray[x, y].Elevation;
          }
        }

      //  if (cnt2 > 0)
      //  {
      //    eff = (cnt / cnt2) * 100;
      //    Log.LogDebug($"Processing Subgrid. TotalAvail:{subGrid.CountNonNullCells()}, Sampled:{cnt2}, Used:{cnt} Eff:{Math.Round(eff, 2)}");
      //  }

      // Statistics
      TotalSampled = TotalSampled + cnt2;
      TotalUsed = TotalUsed + cnt;
    }


    /// <summary>
    /// Accept a sub grid response from the processing engine and incorporate into the result for the request.
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    public override bool TransferResponse(object response)
    {
      bool result = false;

      if (base.TransferResponse(response))
      {
        if (!(response is IClientLeafSubGrid[] subGridResponses) || subGridResponses.Length == 0)
        {
          Log.LogWarning("No sub grid responses returned");
        }
        else
        {
          foreach (var subGrid in subGridResponses)
          {
            if (subGrid is ClientHeightLeafSubGrid leafSubGrid)
              DummyExtractRequiredValues(leafSubGrid);
          }
          result = true;
        }
      }

      return result;
    }
  }

}
