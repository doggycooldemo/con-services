﻿using System.IO;
using VSS.TRex.Cells;
using VSS.TRex.Filters;
using VSS.TRex.SubGridTrees.Interfaces;
using VSS.TRex.SubGridTrees.Utilities;
using VSS.TRex.Types;

namespace VSS.TRex.SubGridTrees.Client
{
  public class ClientMachineTargetSpeedLeafSubGrid : GenericClientLeafSubGrid<MachineSpeedExtendedRecord>
	{
		/// <summary>
		/// First pass map records which cells hold cell pass machine speed targets that were derived
		/// from the first pass a machine made over the corresponding cell
		/// </summary>
		public SubGridTreeBitmapSubGridBits FirstPassMap = new SubGridTreeBitmapSubGridBits(SubGridBitsCreationOptions.Unfilled);

		/// <summary>
		/// Constructor. Set the grid to MachineSpeedTarget.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="parent"></param>
		/// <param name="level"></param>
		/// <param name="cellSize"></param>
		/// <param name="indexOriginOffset"></param>
		public ClientMachineTargetSpeedLeafSubGrid(ISubGridTree owner, ISubGrid parent, byte level, double cellSize, uint indexOriginOffset) : base(owner, parent, level, cellSize, indexOriginOffset)
		{
			_gridDataType = GridDataType.MachineSpeedTarget;
		}

	  /// <summary>
	  /// Speed target subgrids require lift processing...
	  /// </summary>
	  /// <returns></returns>
	  public override bool WantsLiftProcessingResults() => true;

    /// <summary>
    /// Determine if a filtered machine speed targets value is valid (not null)
    /// </summary>
    /// <param name="filteredValue"></param>
    /// <returns></returns>
    public override bool AssignableFilteredValueIsNull(ref FilteredPassData filteredValue) => filteredValue.FilteredPass.MachineSpeed == CellPass.NullMachineSpeed;

		/// <summary>
		/// Assign filtered machine speed targets value from a filtered pass to a cell
		/// </summary>
		/// <param name="cellX"></param>
		/// <param name="cellY"></param>
		/// <param name="Context"></param>
		public override void AssignFilteredValue(byte cellX, byte cellY, FilteredValueAssignmentContext Context)
		{
			Cells[cellX, cellY].Min = Context.CellProfile.CellMinSpeed;
		  Cells[cellX, cellY].Max = Context.CellProfile.CellMaxSpeed;
    }

	  /// <summary>
	  /// Determines if the machine speed at the cell location is null or not.
	  /// </summary>
	  /// <param name="cellX"></param>
	  /// <param name="cellY"></param>
	  /// <returns></returns>
	  public override bool CellHasValue(byte cellX, byte cellY) => (Cells[cellX, cellY].Min != CellPass.NullMachineSpeed) || (Cells[cellX, cellY].Max != CellPass.NullMachineSpeed);

	  /// <summary>
	  /// Sets all min/max cell machine speeds to null and clears the first pass and sureyed surface pass maps
	  /// </summary>
	  public override void Clear()
	  {
	    base.Clear();

	    // TODO: Optimisation: Use PassData_MachineSpeed_Null assignment as in current gen;
      ForEach((x, y) =>
	    {
	      Cells[x, y].Min = CellPass.NullMachineSpeed;
	      Cells[x, y].Max = CellPass.NullMachineSpeed;
      });

	    FirstPassMap.Clear();
	  }

	  /// <summary>
	  /// Dumps machine speeds from subgrid to the log
	  /// </summary>
	  /// <param name="title"></param>
	  public override void DumpToLog(string title)
	  {
	    base.DumpToLog(title);
	  }

	  /// <summary>
    /// Write the contents of the Items array using the supplied writer
    /// This is an unimplemented override; a generic BinaryReader based implementation is not provided. 
    /// Override to implement if needed.
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="buffer"></param>
    public override void Write(BinaryWriter writer, byte[] buffer)
	  {
	    base.Write(writer, buffer);

	    FirstPassMap.Write(writer, buffer);

	    SubGridUtilities.SubGridDimensionalIterator((x, y) => Cells[x, y].Write(writer));
	  }

	  /// <summary>
	  /// Fill the items array by reading the binary representation using the provided reader. 
	  /// This is an unimplemented override; a generic BinaryReader based implementation is not provided. 
	  /// Override to implement if needed.
	  /// </summary>
	  /// <param name="reader"></param>
	  /// <param name="buffer"></param>
	  public override void Read(BinaryReader reader, byte[] buffer)
	  {
	    base.Read(reader, buffer);

	    FirstPassMap.Read(reader, buffer);

	    SubGridUtilities.SubGridDimensionalIterator((x, y) => Cells[x, y].Read(reader));
	  }
  }
}
