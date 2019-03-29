﻿using System.Drawing;
using VSS.TRex.Common.CellPasses;
using VSS.TRex.Rendering.Palettes;
using VSS.TRex.SubGridTrees.Client;
using VSS.TRex.SubGridTrees.Interfaces;
using VSS.TRex.Types;

namespace VSS.TRex.Rendering.Displayers
{
  /// <summary>
  /// Plan View Map displayer renderer for material temperature summary information presented as rendered tiles
  /// </summary>
  public class PVMDisplayer_TemperatureSummary : PVMDisplayerBase
  {
    /// <summary>
    /// Renders material temperature summary data as tiles. 
    /// </summary>
    /// <param name="subGrid"></param>
    /// <returns></returns>
    protected override bool DoRenderSubGrid<T>(ISubGrid subGrid)
    {
      return base.DoRenderSubGrid<ClientTemperatureLeafSubGrid>(subGrid);
    }

    /// <summary>
    /// Queries the data at the current cell location and determines the colour that should be displayed there.
    /// </summary>
    /// <returns></returns>
    protected override Color DoGetDisplayColour()
    {
      var cellValue = ((ClientTemperatureLeafSubGrid)SubGrid).Cells[east_col, north_row];

      var temperatureLevels = new TemperatureWarningLevelsRecord(cellValue.TemperatureLevels.Min, cellValue.TemperatureLevels.Max);

      var returnedColour = cellValue.MeasuredTemperature == CellPassConsts.NullMaterialTemperatureValue ? Color.Empty : ((TemperatureSummaryPalette)Palette).ChooseColour(cellValue.MeasuredTemperature, temperatureLevels);

      return returnedColour;
    }
  }
}
