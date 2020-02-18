﻿using System;
using VSS.TRex.SubGridTrees.Client;
using VSS.TRex.SubGridTrees.Client.Interfaces;
using VSS.TRex.SubGridTrees.Interfaces;

namespace VSS.TRex.Rendering.Executors.Tasks
{
  public class PVMTaskAccumulator<TC, TS> : IPVMTaskAccumulator where TS : GenericClientLeafSubGrid<TC>
  {
    public TC[,] ValueStore;

    /// <summary>
    /// The cell size of the cells contained in the accumulated sub grids
    /// </summary>
    private readonly double _sourceCellSize;

    /// <summary>
    /// The X and Y dimensions of the cells in the value accumulator array
    /// </summary>
    private double _valueStoreCellSizeX, _valueStoreCellSizeY;

    /// <summary>
    /// The number of cells wide and high in the accumulator array
    /// </summary>
    private readonly int _cellsWidth, _cellsHeight;

    public readonly double WorldX;
    public readonly double WorldY;
    public readonly double OriginX;
    public readonly double OriginY;

    private int _stepX, _stepY;
    private double _stepXIncrement, _stepYIncrement;
    private double _stepXIncrementOverTwo, _stepYIncrementOverTwo;

    private void InitialiseValueStore(TC nullCellValue)
    {
      ValueStore = new TC[_cellsWidth, _cellsHeight];

      // Initialise value store to the supplied null values to ensure colour choosing from the palette is
      // correct for values that are not populated from inbound subgrids
      for (var i = 0; i < _cellsWidth; i++)
      {
        for (var j = 0; j < _cellsHeight; j++)
        {
          ValueStore[i, j] = nullCellValue;
        }
      }
    }

    private void CalculateAccumulatorParameters()
    {
      var stepsPerPixelX = (WorldX / _cellsWidth) / _sourceCellSize;
      var stepsPerPixelY = (WorldY / _cellsHeight) / _sourceCellSize;

      _stepX = Math.Max(1, (int)Math.Truncate(stepsPerPixelX));
      _stepY = Math.Max(1, (int)Math.Truncate(stepsPerPixelY));

      _stepXIncrement = _stepX * _sourceCellSize;
      _stepYIncrement = _stepY * _sourceCellSize;

      _stepXIncrementOverTwo = _stepXIncrement / 2;
      _stepYIncrementOverTwo = _stepYIncrement / 2;
    }

    /// <summary>
    /// Constructor that instantiates intermediary value storage for the PVM rendering task
    /// </summary>
    /// <param name="valueStoreCellSizeX">The world X dimension size of cells in the value store</param>
    /// <param name="valueStoreCellSizeY">The world X dimension size of cells in the value store</param>
    /// <param name="cellsWidth">The number of cells 'wide' (x ordinate) in the set of cell values requested</param>
    /// <param name="cellsHeight">The number of cells 'high' (y ordinate) in the set of cell values requested</param>
    /// <param name="worldX">The world coordinate width (X axis) of the value store</param>
    /// <param name="worldY">The world coordinate width (X axis) of the value store</param>
    /// <param name="originX">The default north oriented world X coordinate or the _valueStore origin</param>
    /// <param name="originY">The default north oriented world Y coordinate or the _valueStore origin</param>
    /// <param name="sourceCellSize">The (square) size of the underlying cells in the site model that is the source of rendered data</param>
    public PVMTaskAccumulator(double valueStoreCellSizeX, double valueStoreCellSizeY, int cellsWidth, int cellsHeight,
      double worldX, double worldY,
      double originX, double originY,
      double sourceCellSize)
    {
      _valueStoreCellSizeX = valueStoreCellSizeX;
      _valueStoreCellSizeY = valueStoreCellSizeY;
      _cellsWidth = cellsWidth;
      _cellsHeight = cellsHeight;
      OriginX = originX;
      OriginY = originY;
      WorldX = worldX;
      WorldY = worldY;
      _sourceCellSize = sourceCellSize;
    }

    /// <summary>
    /// Transcribes values of interest from subgrids into a contiguous collection of values
    /// </summary>
    /// <param name="subGridResponses"></param>
    /// <returns>Whether the content of subGridResponses contained a transcribable sub grid</returns>
    public bool Transcribe(IClientLeafSubGrid[] subGridResponses)
    {
      var subGrid = subGridResponses?.Length == 1 ? subGridResponses[0] as TS : null;
      var cells = subGrid?.Cells;

      if (cells == null)
      {
        return false;
      }

      if (ValueStore == null)
      {
        CalculateAccumulatorParameters();
        InitialiseValueStore(subGrid.NullCell());
      }

      // Calculate the world coordinate location of the origin (bottom left corner) of this sub grid
      subGridResponses[0].CalculateWorldOrigin(out var subGridWorldOriginX, out var subGridWorldOriginY);

      // Skip-Iterate through the cells assigning them to the value store

      var temp = subGridWorldOriginY / _stepYIncrement;
      var currentNorth = (Math.Truncate(temp) * _stepYIncrement) - _stepYIncrementOverTwo;
      var northRow = (int) Math.Floor((currentNorth - subGridWorldOriginY) / _sourceCellSize);
      while (northRow < 0)
      {
        northRow += _stepY;
        currentNorth += _stepYIncrement;
      }

      var valueStoreY = (int)Math.Floor((currentNorth - OriginY) / _valueStoreCellSizeY);
      while (northRow < SubGridTreeConsts.SubGridTreeDimension)
      {
        if (valueStoreY >= 0 && valueStoreY < _cellsHeight)
        {
          temp = subGridWorldOriginX / _stepXIncrement;
          var currentEast = (Math.Truncate(temp) * _stepXIncrement) - _stepXIncrementOverTwo;
          var eastCol = (int) Math.Floor((currentEast - subGridWorldOriginX) / _sourceCellSize);

          while (eastCol < 0)
          {
            eastCol += _stepX;
            currentEast += _stepXIncrement;
          }

          var valueStoreX = (int)Math.Floor((currentEast - OriginX) / _valueStoreCellSizeX);

          while (eastCol < SubGridTreeConsts.SubGridTreeDimension)
          {
            // Transcribe the value at [east_col, north_row] in the subgrid in to the matching location in the value store
            if (valueStoreX >= 0 && valueStoreX < _cellsWidth)
            {
              ValueStore[valueStoreX, valueStoreY] = cells[eastCol, northRow];
            }

            currentEast += _stepXIncrement;
            eastCol += _stepX;
            valueStoreX++;
          }
        }

        currentNorth += _stepYIncrement;
        northRow += _stepY;
        valueStoreY++;
      }

      return true;
    }
  }
}
