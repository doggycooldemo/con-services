﻿using System.IO;
using VSS.TRex.Types.CellPasses;
using VSS.TRex.Events.Models;
using VSS.TRex.Filters.Models;
using VSS.TRex.SubGridTrees.Client.Interfaces;
using VSS.TRex.SubGridTrees.Interfaces;
using VSS.TRex.SubGridTrees.Core.Utilities;
using VSS.TRex.Types;

namespace VSS.TRex.SubGridTrees.Client
{
  /// <summary>
  /// The content of each cell in a Pass Count client leaf sub grid. Each cell stores an elevation only.
  /// </summary>
  public class ClientPassCountLeafSubGrid : GenericClientLeafSubGrid<SubGridCellPassDataPassCountEntryRecord>, IClientPassCountLeafSubGrid
  {
    /// <summary>
    /// Initialise the null cell values for the client sub grid
    /// </summary>
    static ClientPassCountLeafSubGrid()
    {
      SubGridUtilities.SubGridDimensionalIterator((x, y) => NullCells[x, y] = SubGridCellPassDataPassCountEntryRecord.NullValue);
    }

    /// <summary>
    /// Pass Count sub grids require lift processing...
    /// </summary>
    /// <returns></returns>
    public override bool WantsLiftProcessingResults() => true;

    private void Initialise()
    {
      EventPopulationFlags |= PopulationControlFlags.WantsTargetPassCountValues;

      _gridDataType = GridDataType.PassCount;
    }

    /// <summary>
    /// Constructs a default client sub grid with no owner or parent, at the standard leaf bottom sub grid level,
    /// and using the default cell size and index origin offset
    /// </summary>
    public ClientPassCountLeafSubGrid()
    {
      Initialise();
    }

    /*
    /// <summary>
    /// Constructor. Set the grid to Pass Count.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="parent"></param>
    /// <param name="level"></param>
    /// <param name="cellSize"></param>
    /// <param name="indexOriginOffset"></param>
    public ClientPassCountLeafSubGrid(ISubGridTree owner, ISubGrid parent, byte level, double cellSize, uint indexOriginOffset) : base(owner, parent, level, cellSize, indexOriginOffset)
    {
      Initialise();
    }
    */

    /// <summary>
    /// Determines if the Pass Count at the cell location is null or not.
    /// </summary>
    /// <param name="cellX"></param>
    /// <param name="cellY"></param>
    /// <returns></returns>
    public override bool CellHasValue(byte cellX, byte cellY) => Cells[cellX, cellY].MeasuredPassCount != CellPassConsts.NullPassCountValue;

    /// <summary>
    /// Sets all cell Pass Counts to null and clears the first pass and surveyed surface pass maps
    /// </summary>
    public override void Clear()
    {
      base.Clear();
    }

    /// <summary>
    /// Assign filtered Pass Count value from a filtered pass to a cell
    /// </summary>
    /// <param name="cellX"></param>
    /// <param name="cellY"></param>
    /// <param name="Context"></param>
    public override void AssignFilteredValue(byte cellX, byte cellY, FilteredValueAssignmentContext Context)
    {
      Cells[cellX, cellY].MeasuredPassCount = (ushort) Context.FilteredValue.PassCount;
      Cells[cellX, cellY].TargetPassCount = Context.FilteredValue.FilteredPassData.TargetValues.TargetPassCount;
    }

    /// <summary>
    /// Determine if a filtered CMV is valid (not null)
    /// </summary>
    /// <param name="filteredValue"></param>
    /// <returns></returns>
    public override bool AssignableFilteredValueIsNull(ref FilteredPassData filteredValue) => false;

    /// <summary>
    /// Fills the contents of the client leaf subgrid with a known, non-null test pattern of values
    /// </summary>
    public override void FillWithTestPattern()
    {
      ForEach((x, y) => Cells[x, y] = new SubGridCellPassDataPassCountEntryRecord { MeasuredPassCount = (ushort) (x + 1), TargetPassCount = (ushort) (y + 1) });
    }

    /// <summary>
    /// Determines if the leaf content of this subgrid is equal to 'other'
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public override bool LeafContentEquals(IClientLeafSubGrid other)
    {
      bool result = true;

      IGenericClientLeafSubGrid<SubGridCellPassDataPassCountEntryRecord> _other = (IGenericClientLeafSubGrid<SubGridCellPassDataPassCountEntryRecord>)other;
      ForEach((x, y) => result &= Cells[x, y].Equals(_other.Cells[x, y]));

      return result;
    }

    /// <summary>
    /// Provides a copy of the null value defined for cells in this client leaf subgrid
    /// </summary>
    /// <returns></returns>
    public override SubGridCellPassDataPassCountEntryRecord NullCell() => SubGridCellPassDataPassCountEntryRecord.NullValue;

    /// <summary>
    /// Write the contents of the Items array using the supplied writer
    /// This is an unimplemented override; a generic BinaryReader based implementation is not provided. 
    /// Override to implement if needed.
    /// </summary>
    /// <param name="writer"></param>
    public override void Write(BinaryWriter writer)
    {
      base.Write(writer);

      SubGridUtilities.SubGridDimensionalIterator((x, y) => Cells[x, y].Write(writer));
    }

    /// <summary>
    /// Fill the items array by reading the binary representation using the provided reader. 
    /// This is an unimplemented override; a generic BinaryReader based implementation is not provided. 
    /// Override to implement if needed.
    /// </summary>
    /// <param name="reader"></param>
    public override void Read(BinaryReader reader)
    {
      base.Read(reader);

      SubGridUtilities.SubGridDimensionalIterator((x, y) => Cells[x, y].Read(reader));
    }

    /// <summary>
    /// Return an indicative size for memory consumption of this class to be used in cache tracking
    /// </summary>
    /// <returns></returns>
    public override int IndicativeSizeInBytes()
    {
      return base.IndicativeSizeInBytes() +
             SubGridTreeConsts.SubGridTreeCellsPerSubGrid * SubGridCellPassDataPassCountEntryRecord.IndicativeSizeInBytes();
    }

    public override void DumpToLog()
    {
      base.DumpToLog();
    }
  }
}
