﻿using System;
using System.IO;

namespace VSS.TRex.SubGridTrees.Interfaces
{
  /// <summary>
  /// Interface defining basic interface methods for the base sub grid type underlying all sub grid types
  /// </summary>
  public interface ISubGrid
  {
    /// <summary>
    /// ‘Level’ in the sub grid tree in which this sub grid resides. Level 1 is the root node in the tree
    /// </summary>
    byte Level { get; set; }

    /// <summary>
    /// Grid cell X Origin of the bottom left hand cell in this sub grid. 
    /// Origin is wrt to cells of the spatial dimension held by this sub grid
    /// </summary>
    int OriginX { get; set; }

    /// <summary>
    /// Grid cell Y Origin of the bottom left hand cell in this sub grid. 
    /// Origin is wrt to cells of the spatial dimension held by this sub grid
    /// </summary>
    int OriginY { get; set; }

    /// <summary>
    /// Dirty property used to indicate the presence of changes that are not persisted.
    /// </summary>
    bool Dirty { get; }

    /// <summary>
    /// The owning sub grid tree that this su grid is a part of.
    /// </summary>
    ISubGridTree Owner { get; set; }

    /// <summary>
    /// The parent sub grid that owns this sub grid as a cell.
    /// </summary>
    ISubGrid Parent { get; set; }

    void SetDirty();

    int AxialCellCoverageByThisSubGrid();
    int AxialCellCoverageByChildSubGrid();
    bool ContainsOTGCell(int CellX, int CellY);
    void SetOriginPosition(int CellX, int CellY);
    void GetSubGridCellIndex(int CellX, int CellY, out byte SubGridX, out byte SubGridY);
    bool IsLeafSubGrid();
    string Moniker();
    ISubGrid GetSubGrid(int X, int Y);
    void SetSubGrid(int X, int Y, ISubGrid value);
    void CalculateWorldOrigin(out double WorldOriginX, out double WorldOriginY);
    void Clear();
    void AllChangesMigrated();
    bool IsEmpty();
    void RemoveFromParent();
    bool CellHasValue(byte CellX, byte CellY);
    int CountNonNullCells();
    void SetAbsoluteOriginPosition(int originX, int originY);
    void SetAbsoluteLevel(byte level);

    void Write(BinaryWriter writer);

    void Read(BinaryReader reader);

    SubGridCellAddress OriginAsCellAddress();

    byte[] ToBytes();
    void FromBytes(byte[] bytes);

    /// <summary>
    /// Perform an action over all cells in the sub grid by calling the supplied action lambda with the coordinates of each cell
    /// </summary>
    /// <param name="action"></param>
    void ForEach(Action<byte, byte> action);
    }
}
