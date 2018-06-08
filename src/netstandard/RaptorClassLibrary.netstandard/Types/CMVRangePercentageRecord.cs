﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VSS.TRex.Cells;
using VSS.TRex.Common;

namespace VSS.TRex.Types
{
  public struct CMVRangePercentageRecord
  {
    /// <summary>
    /// Minimum CMV percentage range value.
    /// </summary>
    public double Min { get; set; }
    /// <summary>
    /// Maximum CMV percentage range value.
    /// </summary>
    public double Max { get; set; }

    /// <summary>
    /// Constractor with arguments.
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    public CMVRangePercentageRecord(double min, double max)
    {
      Min = min;
      Max = max;
    }

    /// <summary>
    /// Initialises the Min and Max properties with null values.
    /// </summary>
    public void Clear()
    {
      Min = Consts.NullDouble;
      Max = Consts.NullDouble;
    }

    /// <summary>
    /// Serialises content of the cell to the writer
    /// </summary>
    /// <param name="writer"></param>
    public void Write(BinaryWriter writer)
    {
      writer.Write(Min);
      writer.Write(Max);
    }

    /// <summary>
    /// Serialises comtent of the cell from the writer
    /// </summary>
    /// <param name="reader"></param>
    public void Read(BinaryReader reader)
    {
      Min = reader.ReadUInt16();
      Max = reader.ReadUInt16();
    }
  }
}
