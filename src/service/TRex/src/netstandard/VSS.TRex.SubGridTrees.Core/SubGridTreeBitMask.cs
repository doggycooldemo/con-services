﻿using System;
using Amazon.S3.Model.Internal.MarshallTransformations;
using VSS.TRex.Geometry;
using VSS.TRex.SubGridTrees.Core;
using VSS.TRex.SubGridTrees.Factories;
using VSS.TRex.SubGridTrees.Interfaces;
using VSS.TRex.SubGridTrees.Types;

namespace VSS.TRex.SubGridTrees
{
  /// <summary>
    /// SubGridTreeBitMask implements a sub grid tree whose sole contents is a single
    /// bit per cell in the leaf nodes. The intention of this SubGridTree descendant
    /// is for tasks such as tracking cells modified since last read, or cells present
    /// in a SubGridTree cache etc. It maintains both per cell and per leaf flags.
    /// </summary>
  public class SubGridTreeBitMask : SubGridTree, ISubGridTreeBitMask
  {
        /// <summary>
        /// Constructor that defaults levels, cell size and sub grid factory 
        /// </summary>
        public SubGridTreeBitMask() : this(SubGridTreeConsts.DefaultCellSize)
        {          
        }

        /// <summary>
        /// Constructor that defaults levels, cell size and sub grid factory 
        /// </summary>
        public SubGridTreeBitMask(double cellSize) : this(SubGridTreeConsts.SubGridTreeLevels, cellSize)
        {
        }

        /// <summary>
        /// Constructor that sets levels and cell size with a default factory
        /// </summary>
        public SubGridTreeBitMask(byte numLevels, double cellSize) : base(numLevels, cellSize,
                                  new SubGridFactory<SubGridTreeNodeBitmapSubGrid, SubGridTreeLeafBitmapSubGrid>())
        {
        }

        /// <summary>
        /// Performs the fundamental GetCell operation that returns a boolean value noting the state of the 
        /// bit in the tree at the [CellX, CellY] location
        /// </summary>
        public bool GetCell(int CellX, int CellY)
        {
            var SubGrid = LocateSubGridContaining(CellX, CellY, numLevels);

            if (SubGrid == null)
                return false;

            SubGrid.GetSubGridCellIndex(CellX, CellY, out byte SubGridX, out byte SubGridY);

            return ((SubGridTreeLeafBitmapSubGrid)SubGrid).Bits.BitSet(SubGridX, SubGridY);
        }

        /// <inheritdoc />
        /// <summary>
        /// Performs the fundamental SetCell operation that sets the state of bit in the tree at the 
        /// [CellX, CellY] location according to the boolean value parameter
        /// </summary>
        public void SetCell(int CellX, int CellY, bool Value)
        {
            var SubGrid = ConstructPathToCell(CellX, CellY, SubGridPathConstructionType.CreateLeaf);

            SubGrid.GetSubGridCellIndex(CellX, CellY, out byte SubGridX, out byte SubGridY);
            ((SubGridTreeLeafBitmapSubGrid)SubGrid).Bits.SetBitValue(SubGridX, SubGridY, Value);
        }

        /// <summary>
        /// Calculates the integer bounding rectangle within the bit mask sub grid that encloses all bits that
        /// are set to 1 (true)
        /// </summary>
        private BoundingIntegerExtent2D ComputeCellsExtents()
        {
            var SubGridCellsExtents = new BoundingIntegerExtent2D();
            SubGridCellsExtents.SetInverted();

            ScanAllSubGrids(x => 
            {
                SubGridCellsExtents.Include(((SubGridTreeLeafBitmapSubGrid)x).ComputeCellsExtents());
                return true;
            });

            return SubGridCellsExtents;
        }

        /// <summary>
        /// Default array indexer for the bits in the sub grid tree mask
        /// </summary>
        public bool this[int CellX, int CellY]
        {
            get => GetCell(CellX, CellY);
            set => SetCell(CellX, CellY, value);
        }

        /// <summary>
        /// RemoveLeafOwningCell locates the leaf sub grid that contains the OTG cell identified by CellX and CellY and removes it from the
        /// sub grid tree.
        /// </summary>
        public void RemoveLeafOwningCell(int CellX, int CellY)
        {
            var SubGrid = LocateSubGridContaining(CellX, CellY, (byte)(numLevels - 1));

            if (SubGrid == null)
                return;

            SubGrid.GetSubGridCellIndex(CellX, CellY, out byte SubGridX, out byte SubGridY);

            // Free the node containing the bits for the cells in the leaf
            ((SubGridTreeNodeBitmapSubGrid)SubGrid).SetSubGrid(SubGridX, SubGridY, null);
        }

        /// <summary>
        /// CountBits performs a scan of the sub grid bit mask tree and counts all the bits that are set within it
        /// </summary>
        public long CountBits()
        {
            long totalBits = 0;

            ScanAllSubGrids(x => 
            {
                totalBits += ((SubGridTreeLeafBitmapSubGrid)x).Bits.CountBits();
                return true;
            });

            return totalBits;
        }

        /// <summary>
        /// Calculates the world coordinate bounding rectangle within the bit mask sub grid that encloses all bits that
        /// are set to 1 (true)
        /// </summary>
        public BoundingWorldExtent3D ComputeCellsWorldExtents()
        {
            var SubGridCellsExtents = ComputeCellsExtents();

            if (SubGridCellsExtents.IsValidExtent)
            {
                GetCellCenterPosition(SubGridCellsExtents.MinX, SubGridCellsExtents.MinY, out double MinCX, out double MinCY);
                GetCellCenterPosition(SubGridCellsExtents.MaxX, SubGridCellsExtents.MaxY, out double MaxCX, out double MaxCY);

                return new BoundingWorldExtent3D(MinCX - CellSizeOver2, MinCY - CellSizeOver2,
                                                 MaxCX + CellSizeOver2, MaxCY + CellSizeOver2);
            }

            return BoundingWorldExtent3D.Inverted();
        }

        /// <summary>
        /// LeafExists determines if there is a leaf cell in the sub grid tree that contains the cell at address [CellX, CellY].
        /// </summary>
        public bool LeafExists(int CellX, int CellY) => ConstructPathToCell(CellX, CellY, SubGridPathConstructionType.ReturnExistingLeafOnly) != null;

        /// <summary>
        /// Takes a source SubGridBitMask instance and performs a bitwise OR of the contents of source against the
        /// contents of this instance, modifying the state of this sub grid bit mask tree to produce the result
        /// </summary>
        public void SetOp_OR(ISubGridTreeBitMask Source)
        {
            Source?.ScanAllSubGrids(x =>
            {
                var bitMapSubGrid = (SubGridTreeLeafBitmapSubGrid)ConstructPathToCell(x.OriginX, x.OriginY, SubGridPathConstructionType.CreateLeaf);

                // In this instance, x is a sub grid from the tree we are OR-ing with this
                // one, and BitMapSubGrid is a grid retrieved from this tree
                bitMapSubGrid.Bits.OrWith(((SubGridTreeLeafBitmapSubGrid)x).Bits);
                
                return true; // Keep the scan going
            });
        }

        /// <summary>
        /// Takes a source SubGridBitMask instance and performs a bitwise XOR of the contents of source against the
        /// contents of this instance, modifying the state of this sub grid bit mask tree to produce the result
        /// </summary>
        public void SetOp_XOR(ISubGridTreeBitMask Source)
        {
          Source?.ScanAllSubGrids(x =>
          {
            var bitMapSubGrid = (SubGridTreeLeafBitmapSubGrid)ConstructPathToCell(x.OriginX, x.OriginY, SubGridPathConstructionType.CreateLeaf);

            // In this instance, x is a sub grid from the tree we are OR-ing with this
            // one, and BitMapSubGrid is a grid retrieved from this tree
            bitMapSubGrid.Bits.XorWith(((SubGridTreeLeafBitmapSubGrid)x).Bits);

            return true; // Keep the scan going
          });
        }

        /// <summary>
        /// Takes a source SubGridBitMask instance and performs a bitwise AND of the contents of source against the
        /// contents of this instance, modifying the state of this sub grid bit mask tree to produce the result
        /// </summary>
        /// <param name="Source"></param>
        public void SetOp_AND(ISubGridTreeBitMask Source)
        {
            /* Previous implementation iterated across the source, when only iteration across 'this' is required as
             * sub grids not present in this tree are implicitly 'false' so will never generate any true bits needing storing.
             * Similarly, sub grids in source that are not present in this will never generate any true bits requiring storing
            Source.ScanAllSubGrids(x =>
            {
                if (x == null)
                    return true; // Keep the scan going

                var bitMapSubGrid = (SubGridTreeLeafBitmapSubGrid)(ConstructPathToCell(x.OriginX, x.OriginY, SubGridPathConstructionType.CreateLeaf));
                if (bitMapSubGrid != null)
                    bitMapSubGrid.Bits = bitMapSubGrid.Bits & ((SubGridTreeLeafBitmapSubGrid)x).Bits;
                else
                    throw new TRexSubGridTreeException("Failed to create bit map sub grid in SetOp_AND");

                return true; // Keep the scan going
            });
            */

            // This implementation will be much more performant!
            ScanAllSubGrids(x =>
            {
                if (x != null)
                {
                    var bitMapSubGrid = (SubGridTreeLeafBitmapSubGrid)Source?.LocateSubGridContaining(x.OriginX, x.OriginY);
                    if (bitMapSubGrid == null)
                        ((SubGridTreeLeafBitmapSubGrid)x).Bits.Clear(); // No sub grid in source, clear sub grid in 'this'
                    else
                        ((SubGridTreeLeafBitmapSubGrid)x).Bits.AndWith(bitMapSubGrid.Bits);
                }

                return true; // Keep the scan going
            });
        }

        /// <summary>
        /// Takes a source SubGridBitMask instance and performs a bitwise AND of the bitwise NOTed contents of source against the
        /// contents of this instance, modifying the state of this sub grid bit mask tree to produce the result
        /// </summary>
        public void SetOp_ANDNOT(ISubGridTreeBitMask Source)
        {
          ScanAllSubGrids(x =>
          {
            if (x != null)
            {
              var bitMapSubGrid = (SubGridTreeLeafBitmapSubGrid)Source?.LocateSubGridContaining(x.OriginX, x.OriginY);
              if (bitMapSubGrid == null)
                ((SubGridTreeLeafBitmapSubGrid)x).Bits.Clear(); // No sub grid in source, clear sub grid in 'this'
              else
                ((SubGridTreeLeafBitmapSubGrid)x).Bits.AndNotWith(bitMapSubGrid.Bits);
            }
      
            return true; // Keep the scan going
          });
        }

        /// <summary>
        ///  ClearCellIfSet will set the value of a cell to false if the current
        /// value of cell is True. The function returns true if the cell was set
        /// and has been cleared
        /// </summary>
        public bool ClearCellIfSet(int CellX, int CellY)
        {
          var SubGrid = LocateSubGridContaining(CellX, CellY, numLevels);

          if (SubGrid == null)
              return false;

          var bitmapSubGrid = (SubGridTreeLeafBitmapSubGrid)SubGrid;

          bitmapSubGrid.GetSubGridCellIndex(CellX, CellY, out byte SubGridX, out byte SubGridY);

          if (bitmapSubGrid.Bits.BitSet(SubGridX, SubGridY))
          {
              bitmapSubGrid.Bits.ClearBit(SubGridX, SubGridY);
              return true;
          }

          return false;
        }

        /// <summary>
        /// Scan all the bits in the bit mask sub grid tree treating each set bit as the address of a sub grid
        /// call the supplied Action 'functor' with a leaf sub grid origin address calculated from each of the bits
        /// Note: As each bit represents an on-the-ground leaf sub grid, cell address of that bit is transformed
        /// from the level 5 (node) layer to the level 6 (leaf) layer
        /// </summary>
        public void ScanAllSetBitsAsSubGridAddresses(Action<SubGridCellAddress> functor)
        {
          ScanAllSubGrids(leaf =>
          {
            ((SubGridTreeLeafBitmapSubGrid)leaf).Bits.ForEachSetBit((x, y) =>
              functor(new SubGridCellAddress((leaf.OriginX + x) << SubGridTreeConsts.SubGridIndexBitsPerLevel,
                                             (leaf.OriginY + y) << SubGridTreeConsts.SubGridIndexBitsPerLevel))
            );

            return true;
          });
        }

    /// <summary>
    /// Calculate the memory used by the sub grid bit mask by traversing the tree and summing the sizes of
    /// node and leaf sub grids
    /// </summary>
    public long SizeOf()
    {
      long sum = 0;

      ScanAllSubGrids(leaf =>
      {
        sum += SubGridTreeLeafBitmapSubGrid.SizeOf();
        return true;
      },
      node =>
      {
        sum += ((NodeSubGrid)node).SizeOf();
        return SubGridProcessNodeSubGridResult.OK;
      });

      return sum;
    }

    public override string SerialisedHeaderName() => "ExistenceMap";

    public override int SerialisedVersion() => 1;
  }
}
