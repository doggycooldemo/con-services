﻿using System.Drawing;
using VSS.TRex.Types.CellPasses;
using VSS.TRex.Rendering.Palettes;
using VSS.TRex.SubGridTrees.Client;
using VSS.TRex.SubGridTrees.Client.Types;

namespace VSS.TRex.Rendering.Displayers
{
  public class PVMDisplayer_CMVChange : PVMDisplayerBase<CMVChangePalette, ClientCMVLeafSubGrid, SubGridCellPassDataCMVEntryRecord>
  {
    /// <summary>
    /// Queries the data at the current cell location and determines the colour that should be displayed there.
    /// </summary>
    /// <returns></returns>
    public override Color DoGetDisplayColour()
    {
      var cellValue = ValueStore[east_col, north_row];

      return cellValue.MeasuredCMV == CellPassConsts.NullCCV ? Color.Empty : Palette.ChooseColour(cellValue);
    }
  }
}
