﻿using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.CompilerServices;
using VSS.TRex.DataSmoothing;
using VSS.TRex.Rendering.Executors.Tasks;
using VSS.TRex.Rendering.Palettes.Interfaces;
using VSS.TRex.SubGridTrees.Client;
using VSS.TRex.SubGridTrees.Client.Interfaces;

namespace VSS.TRex.Rendering.Displayers
{
  public abstract class ProductionPVMConsistentDisplayer<TP, TS, TC> : ProductionPVMDisplayer, IProductionPVMConsistentDisplayer
    where TP : class, IPlanViewPalette
    where TS : GenericClientLeafSubGrid<TC>, IClientLeafSubGrid
  {
    public TP Palette;

    /// <summary>
    /// A palette set accessor for use when a palette is only known by its IPlanViewPalette interface
    /// </summary>
    public override void SetPalette(IPlanViewPalette palette)
    {
      Palette = palette as TP;
    }

    /// <summary>
    /// Copy of value store provided to render from
    /// </summary>
    public TC[,] ValueStore;

    /// <summary>
    /// A palette get accessor for use when only the IPlanViewPalette is knowable in the accessing context
    /// </summary>
    public override IPlanViewPalette GetPalette() => Palette;

    /// <summary>
    /// Constructs a PVM task accumulator tailored to accumulate cell information to be rendered by this displayer
    /// Note: This intentionally does not pin the bounds of the accumulator to the bounds of the rendered map view
    /// as map view rotation and smoothing operations may require large areas of data to be requested to supply the final
    /// rendered outcome.
    /// </summary>
    /// <param name="valueStoreCellSizeX">The world X dimension size of cells in the value store</param>
    /// <param name="valueStoreCellSizeY">The world X dimension size of cells in the value store</param>
    /// <param name="cellsWidth">The number of cells in the X axis in the value store</param>
    /// <param name="cellsHeight">The number of cells in the X axis in the value store</param>
    /// <param name="originX">The world coordinate origin on the X axis of the area covered by the value store</param>
    /// <param name="originY">The world coordinate origin on the X axis of the area covered by the value store</param>
    /// <param name="worldX">The world coordinate width of the area covered by the value store</param>
    /// <param name="worldY">The world coordinate width of the area covered by the value store</param>
    /// <param name="sourceCellSize">The (square) size of the cells data elements are extracted from in the source data set</param>
    public IPVMTaskAccumulator GetPVMTaskAccumulator(double valueStoreCellSizeX, double valueStoreCellSizeY,
      int cellsWidth, int cellsHeight,
      double originX, double originY, double worldX, double worldY, double sourceCellSize
      )
    {
      _taskAccumulator = new PVMTaskAccumulator<TC, TS>(valueStoreCellSizeX, valueStoreCellSizeY, cellsWidth, cellsHeight, originX, originY, worldX, worldY, sourceCellSize);
      return _taskAccumulator;
    }

    private PVMTaskAccumulator<TC, TS> _taskAccumulator;

    public IDataSmoother DataSmoother { get; set; }

    /// <summary>
    /// Performs a 'consistent' render across a 2D array of collated values from queried sub grids.
    /// Effectively this treats the passed array as if it were a sub grid of that size and renders it as
    /// such against the MapView.
    /// This function should be called just once to render the entire set of data for a tile
    /// </summary>
    public bool PerformConsistentRender()
    {
      if (_taskAccumulator == null)
      {
        throw new ArgumentException("Task accumulator not available");
      }

      ValueStore = _taskAccumulator?.ValueStore;

      if (ValueStore == null)
      {
        // There is no data to render, return success
        return true;
      }

      // If there is a defined elevation smoother for ths rendering context then use it to modify the data assembled
      // for the tile to be rendered and replace the value store with the result of the smooth operation;

      ValueStore = (DataSmoother as IArrayDataSmoother<TC>)?.Smooth(ValueStore) ?? ValueStore;

      // Draw the cells in the grid in stripes, starting from the southern most
      // row in the grid and progressing from the western end to the eastern end
      // (ie: bottom to top, left to right)
      // If data smoothing has occured, inset the range of values to be drawn by the additional border size 
      // requirement of the supplied data smoother

      var insetSize = DataSmoother?.AdditionalBorderSize ?? 0;

      // Directly construct the required pixel array by iterating across the pixels in the target
      // image and mapping them to the locations in the accumulator array

      var pixels = new int[MapView.BitmapCanvas.Width * MapView.BitmapCanvas.Height];
      var index = 0;

      for (int i = 0, limit_i = MapView.BitmapCanvas.Height; i < limit_i; i++)
      {
        for (int j = 0, limit_j = MapView.BitmapCanvas.Width; j < limit_j; j++)
        {
          MapView.Un_rotate_point(MapView.OriginX + j * MapView.XPixelSize, MapView.OriginY + i * MapView.YPixelSize, out var ptx, out var pty);

          east_col = (int)Math.Truncate((ptx - _taskAccumulator.OriginX) / _taskAccumulator.ValueStoreCellSizeX);
          north_row = (int)Math.Truncate((pty - _taskAccumulator.OriginY) / _taskAccumulator.ValueStoreCellSizeY);

          if (east_col >= 0 && east_col < _taskAccumulator.CellsWidth &&
              north_row >= 0 && north_row < _taskAccumulator.CellsHeight)
          {
            pixels[index++] = DoGetDisplayColour().ToArgb();
          }
          else
          {
            pixels[index++] = 0; // Is 0 valid here?
          }
        }
      }

      MapView.DrawFromPixelArray(MapView.BitmapCanvas.Width, MapView.BitmapCanvas.Height, pixels);

    /*  
      DoIterate(_taskAccumulator.ValueStoreCellSizeX, _taskAccumulator.ValueStoreCellSizeY,
        _taskAccumulator.OriginX, _taskAccumulator.OriginY,
        _taskAccumulator.WorldX, _taskAccumulator.WorldY,
        insetSize, insetSize,
        ValueStore.GetLength(0) - insetSize - 1, ValueStore.GetLength(1) - insetSize - 1);
      */

      return true;
    }
  }
}
