﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using VSS.TRex.Designs.TTM.Optimised;
using VSS.TRex.Geometry;
using VSS.TRex.SubGridTrees;
using VSS.TRex.SubGridTrees.Types;
using VSS.TRex.SubGridTrees.Utilities;
using VSS.TRex.Utilities;

namespace VSS.TRex.Designs
{
    /// <summary>
    /// A design comprised of a Triangulated Irregular Network TIN surface, comsumed from a Timble TIN Model file
    /// </summary>
    public class TTMDesign : DesignBase
    {
        private static readonly ILogger Log = Logging.Logger.CreateLogger(MethodBase.GetCurrentMethod().DeclaringType?.Name);

        private TrimbleTINModel FData;

       // Removed, this is the old quadtree spatial index, not performant enough
       // public TTMQuadTree QuadTreeSpatialIndex; 

        private double FMinHeight;
        private double FMaxHeight;
        private double FCellSize;
        private SubGridTreeSubGridExistenceBitMask FSubgridIndex;

        private void SwapVertices(ref XYZ A, ref XYZ B) => MinMax.Swap(ref A, ref B);

        public TrimbleTINModel Data { get { return FData; } }

        private Triangle[] TriangleItems;
        private XYZ[] VertexItems;  

    // Removed, this is the 'list of triangles per subgrid' spatial index model
    // private GenericSubGridTree<List<Triangle>> FSpatialIndex;
    // public GenericSubGridTree<List<Triangle>> SpatialIndex { get { return FSpatialIndex; } }

    public struct TriangleArrayReference
        {
            public uint TriangleArrayIndex;
            public uint Count;
        }

        private int[] FSpatialIndexOptimisedTriangles;

        private GenericSubGridTree<TriangleArrayReference> FSpatialIndexOptimised;
        public GenericSubGridTree<TriangleArrayReference> SpatialIndexOptimised { get { return FSpatialIndexOptimised; } }  

        private void AddTrianglePieceToElevationPatch(XYZ H1, XYZ H2, XYZ V,
                                                      Triangle Tri,
                                                      bool SingleRowOnly,
                                                      double OriginX, double OriginY,
                                                      double CellSize,
                                                      float[,] Patch,
                                                      double OffSet,
                                                      ref int ValueCount)
        {
            double H1Slope, H2Slope;
            int HCellIndexY, VCellIndexY;
            int Delta;

            try
            {
                // H1 and H2 describe the horizontal portion of the triangle piece
                // V describes the vertex above, or below the horizontal line

                // Ensure H1 is left of H2 and take local copies of the vertex ordinates
                if (H1.X > H2.X)
                {
                    SwapVertices(ref H1, ref H2);
                }

                double H1X = H1.X;
                double H1Y = H1.Y;
                double H2X = H2.X;
                double H2Y = H2.Y;
                double VX = V.X;
                double VY = V.Y;

                // HalfMinorCellSize is half of the cell size of the on-the-ground cells that
                // will be compared against the TIN design surface during cut fill operations.
                // As the sample point for a cell is the center point of the cell then there is
                // no need to include a half cell width outer boundary of each cell in the subgrid
                // index. A small epsilon value is deducted from the half cell size value to prevent
                // numeric imprecision
                double HalfCellSize = CellSize / 2;
                double HalfMinorCellSize = HalfCellSize - 0.001;

                double PatchSize = SubGridTree.SubGridTreeDimension * CellSize;
                double TopEdge = OriginY + PatchSize;
                double RightEdge = OriginX + PatchSize;

                double OriginXPlusHalfCell = OriginX + HalfMinorCellSize;
                double OriginYPlusHalfCell = OriginY + HalfMinorCellSize;

                double TopEdgeLessHalfCell = TopEdge - HalfMinorCellSize;
                double RightEdgeLessHalfCell = RightEdge - HalfMinorCellSize;

                // Check to see if the triangle piece being considered could possibly intersect
                // the extent of the patch (or any of the cell center positions at which the
                // spot elevation are calculated).
                if (((H1X > RightEdgeLessHalfCell) && (VX > RightEdgeLessHalfCell)) ||
                    ((H2X < OriginXPlusHalfCell) && (VX < OriginXPlusHalfCell)) ||
                    ((H1Y > TopEdgeLessHalfCell) && (H2Y > TopEdgeLessHalfCell) && (VY > TopEdgeLessHalfCell)) ||
                    ((H1Y < OriginYPlusHalfCell) && (H2Y < OriginYPlusHalfCell) && (VY < OriginYPlusHalfCell)))
                {
                    // The triangle piece cannot intersect the patch
                    return;
                }

                int PatchOriginCellIndexX = (int)Math.Floor(OriginXPlusHalfCell / CellSize);
                int PatchOriginCellIndexY = (int)Math.Floor(OriginYPlusHalfCell / CellSize);
                int PatchCellLimitIndexX = PatchOriginCellIndexX + SubGridTree.SubGridTreeDimension - 1;

                // Work out 'Y' range and step direction of the triangle piece.
                double YRange = VY - H1Y;
                int YStep = Math.Sign(YRange);

                try
                {
                    if (SingleRowOnly)
                    {
                        H1Slope = 0;
                        H2Slope = 0;
                    }
                    else
                    {
                        H1Slope = (VX - H1X) / Math.Abs(YRange);
                        H2Slope = (H2X - VX) / Math.Abs(YRange);
                    }
                }
                catch
                {
                    H1Slope = 0;
                    H2Slope = 0;
                }

                double H1SlopeTimesCellSize = H1Slope * CellSize;
                double H2SlopeTimesCellSize = H2Slope * CellSize;

                double AbsH1SlopeTimesCellSize = Math.Abs(H1SlopeTimesCellSize) + 0.001;
                double AbsH2SlopeTimesCellSize = Math.Abs(H2SlopeTimesCellSize) + 0.001;

                // ProcessingCellYIndex is used to ensure that each 'row' of cells is adjacent to the
                // previous row to ensure a row of cells is not skipped in the event that
                // H1 and H2 vertices lie on the boundary of two cells which may cause numeric
                // imprecision when the H1 and H2 vertices are updated after scanning across the
                // cells in the row.

                VCellIndexY = (int)Math.Floor(VY / CellSize);
                HCellIndexY = (int)Math.Floor(H1Y / CellSize);

                int VCellPatchIndex = VCellIndexY - PatchOriginCellIndexY;
                int HCellPatchIndex = HCellIndexY - PatchOriginCellIndexY;

                int NumCellRowsToProcess = Math.Abs(VCellPatchIndex - HCellPatchIndex) + 1;

                int ProcessingCellYIndex = HCellPatchIndex;

                // Determine how many rows of cells there are between ProcessingCellYIndex and
                // the extent covered by the subgrid. Shift the H1X/H2X/etc values appropriately,
                // also clamping the starting cell row index to the patch
                if (HCellPatchIndex < 0)
                {
                    if (YStep == -1) // There's nothing more to be done here
                    {
                        return;
                    }

                    Delta = -HCellPatchIndex;
                    H1X = H1X + Delta * H1SlopeTimesCellSize;
                    H2X = H2X - Delta * H2SlopeTimesCellSize;

                    NumCellRowsToProcess -= Delta;
                    ProcessingCellYIndex = 0;
                }
                else
                {
                    if (HCellPatchIndex >= SubGridTree.SubGridTreeDimension)
                    {
                        if (YStep == 1) // There's nothing more to be done here
                        {
                            return;
                        }

                        Delta = (HCellPatchIndex - SubGridTree.SubGridTreeDimension) + 1;
                        H1X = H1X + Delta * H1SlopeTimesCellSize;
                        H2X = H2X - Delta * H2SlopeTimesCellSize;

                        NumCellRowsToProcess -= Delta;
                        ProcessingCellYIndex = SubGridTree.SubGridTreeDimension - 1;
                    }
                }

                // Clamp the ending cell row to be processed to the patch
                if (VCellPatchIndex < 0)
                {
                    if (YStep == 1)
                    {
                        return; // Nothing more to do here
                    }

                    NumCellRowsToProcess -= -VCellPatchIndex;
                }
                else if (VCellPatchIndex >= SubGridTree.SubGridTreeDimension)
                {
                    if (YStep == -1)
                    {
                        return; // Nothing more to do here
                    }

                    NumCellRowsToProcess -= ((VCellPatchIndex - SubGridTree.SubGridTreeDimension) + 1);
                }

                if (NumCellRowsToProcess == 0)
                {
                    return;
                }

                // Widen the H1/H2 spread to adequately cover the cells in the interval
                // as iterating across just this interval will leave cells on the extreme
                // edges missed out from the spot elevation calcs

                H1X -= AbsH1SlopeTimesCellSize;
                H2X += AbsH2SlopeTimesCellSize;

                // Note: H1X & H2X are modified in the loop after this location

                // Repeatedly scan over rows of cells that cover the triangle piece checking
                // if they cover the body of the triangle
                do //repeat
                {
                    // Calculate the positions of the left and right cell indices in the coordinate space of the
                    // triangle piece
                    int LeftCellIndexX = (int)Math.Floor(H1X / CellSize);
                    int RightCellIndexX = (int)Math.Floor(H2X / CellSize) + 1;

                    // Clip the calculated cell indices against the coordinate space of the patch
                    if (LeftCellIndexX < PatchOriginCellIndexX)
                        LeftCellIndexX = PatchOriginCellIndexX;
                    if (RightCellIndexX > PatchCellLimitIndexX)
                        RightCellIndexX = PatchCellLimitIndexX;

                    if (LeftCellIndexX <= RightCellIndexX)
                    {
                        double Y = ((PatchOriginCellIndexY + ProcessingCellYIndex) * CellSize) + HalfCellSize;

                        for (int I = LeftCellIndexX; I < RightCellIndexX; I++)
                        {
                            double Z = GetHeight(Tri, I * CellSize + HalfCellSize, Y);

                            if (Z != Common.Consts.NullReal)
                            {
                                if (Patch[I - PatchOriginCellIndexX, ProcessingCellYIndex] == Common.Consts.NullHeight)
                                {
                                    ValueCount++;
                                    Patch[I - PatchOriginCellIndexX, ProcessingCellYIndex] = (float)(Z + OffSet);
                                }
                            }
                        }
                    }

                    // Recalculate the left and right cell indexors for the next row of cells to be scanned across the triangle.
                    H1X += H1SlopeTimesCellSize;
                    H2X -= H2SlopeTimesCellSize;

                    NumCellRowsToProcess--;
                    ProcessingCellYIndex += YStep;

                    //      if (NumCellRowsToProcess > 0) and not InRange(ProcessingCellYIndex, 0, kSubGridTreeDimension - 1) then
                    //        SIGLogMessage.PublishNoODS(Self, Format('ProcessingCellYIndex (%d) out of range', [ProcessingCellYIndex]), slmcException);
                }
                while ((NumCellRowsToProcess > 0) && !SingleRowOnly); // or not InRange(ProcessingCellYIndex, 0, kSubGridTreeDimension - 1);
            }
            catch // (Exception E)
            {
                //SIGLogMessage.PublishNoODS(Self, Format('Exception ''%s'' raised in TTTMDesign.AddTrianglePieceToSubgridIndex', [E.Message]), slmcException);
                throw;
            }
        }

        private void AddTrianglePieceToSubgridIndex(SubGridTree index,
                                                    int sourceTriangle,
                                                    Func<SubGridTree, uint, uint, bool> leafSatisfied,
                                                    Action<SubGridTree, uint, uint, int> includeTriangleInLeaf,
                                                    XYZ H1, XYZ H2, XYZ V, bool SingleRowOnly)
        {
            uint TestLeftSubGridX, TestRightSubGridX; 
            double H1Slope, H2Slope;
            bool LastRow = false;
            bool WasLastRow = false;

            try
            {
                // H1 and H2 describe the horizontal portion of the triangle piece
                // V describes the vertex above, or below the horizontal line

                // Ensure H1 is left of H2 and take local copies of the vertex ordinates
                if (H1.X > H2.X)
                    SwapVertices(ref H1, ref H2);

                double H1X = H1.X;
                double H1Y = H1.Y;
                double H2X = H2.X;
                double H2Y = H2.Y;

                // Work out 'Y' range and step direction of the triangle piece.
                double YRange = V.Y - H1Y;
                int YStep = Math.Sign(YRange);
                double YStepTimesCellSize = YStep * index.CellSize;

                try
                {
                    if (SingleRowOnly)
                    {
                        H1Slope = 0;
                        H2Slope = 0;
                    }
                    else
                    {
                        H1Slope = (V.X - H1X) / Math.Abs(YRange);
                        H2Slope = (H2X - V.X) / Math.Abs(YRange);
                    }
                }
                catch
                {
                    H1Slope = 0;
                    H2Slope = 0;
                }

                double H1SlopeTimesCellSize = H1Slope * index.CellSize;
                double H2SlopeTimesCellSize = H2Slope * index.CellSize;

                bool FirstRow = true;
                uint PrevLeftSubGridX = uint.MaxValue;
                uint PrevRightSubGridX = uint.MinValue;

                // Determine the start and end rows
                index.CalculateIndexOfCellContainingPosition(H1X, H1Y, out _, out uint SubgridStartY);
                index.CalculateIndexOfCellContainingPosition(V.X, V.Y, out _, out uint SubgridEndY);

                if (SubgridStartY == SubgridEndY)
                {
                    SingleRowOnly = true;
                }

                // OverrideSubGridY is used to ensure that each 'row' of subgrids is adjacent to the
                // previous row to ensure a row of subgrids is not skipped in the event that
                // H1 and H2 vertices lie on the boundary of two subgrids which may cause numeric
                // imprecision when the H1 and H2 vertices are updated after scanning across the
                // subgrids in the row.
                uint OverrideSubGridY = SubgridStartY;

                // Repeatedly scan over rows of subgrids that cover the triangle piece checking
                // if they cover the body of the triangle
                do
                {
                    index.CalculateIndexOfCellContainingPosition(H1X, H1Y, out uint LeftSubGridX, out _);
                    index.CalculateIndexOfCellContainingPosition(H2X, H2Y, out uint RightSubGridX, out _);

                    if (LeftSubGridX > RightSubGridX)
                        MinMax.Swap(ref LeftSubGridX, ref RightSubGridX);

                    // Bracket the calculate left and right subgrid indices with the previous left and
                    // right subgrid indices to ensure subgrids included via shallow grazing
                    // of near horizontal triangle edges are taken into consideration as each
                    // subsequent row of subgrids is scanned.

                    TestLeftSubGridX = FirstRow ? LeftSubGridX : (PrevLeftSubGridX < LeftSubGridX) ? PrevLeftSubGridX : LeftSubGridX;
                    TestRightSubGridX = FirstRow ? RightSubGridX : (PrevRightSubGridX > RightSubGridX) ? PrevRightSubGridX : RightSubGridX;

                    BoundingWorldExtent3D Extents;

                    // Scan 'central' portion of subgrids between the two end points
                    for (uint I = TestLeftSubGridX; I <= TestRightSubGridX; I++)
                    {
                        if (!leafSatisfied(index, I, OverrideSubGridY))
                        {
                            Extents = index.GetCellExtents(I, OverrideSubGridY);

                            if (SubGridIntersectsTriangle(Extents, H1, H2, V))
                                includeTriangleInLeaf(index, I, OverrideSubGridY, sourceTriangle);
                        }
                    }

                    // Scan to the left from the left most point until subgrids no longer intersect the triangle
                    uint SubGridX = TestLeftSubGridX;
                    do
                    {
                        SubGridX--;

                        Extents = index.GetCellExtents(SubGridX, OverrideSubGridY);

                        if (!SubGridIntersectsTriangle(Extents, H1, H2, V))
                            break;

                        if (!leafSatisfied(index, SubGridX, OverrideSubGridY))
                            includeTriangleInLeaf(index, SubGridX, OverrideSubGridY, sourceTriangle);
                    } while (true);

                    // Scan to the right from the right most point until subgrids no longer intersect the triangle
                    SubGridX = TestRightSubGridX;
                    do
                    {
                        SubGridX++;

                        Extents = index.GetCellExtents(SubGridX, OverrideSubGridY);

                        if (!SubGridIntersectsTriangle(Extents, H1, H2, V))
                            break;

                        if (!leafSatisfied(index, SubGridX, OverrideSubGridY))
                            includeTriangleInLeaf(index, SubGridX, OverrideSubGridY, sourceTriangle);
                    } while (true);

                    FirstRow = false;

                    H1X += H1SlopeTimesCellSize;
                    H1Y += YStepTimesCellSize;

                    H2X -= H2SlopeTimesCellSize;
                    H2Y += YStepTimesCellSize;

                    PrevLeftSubGridX = LeftSubGridX;
                    PrevRightSubGridX = RightSubGridX;

                    OverrideSubGridY = (uint)(OverrideSubGridY + YStep);

                    WasLastRow = LastRow;
                    LastRow = OverrideSubGridY == SubgridEndY; // H2X < H1X;
                }
                while (!WasLastRow && !SingleRowOnly);
            }
            catch // (Exception E)
            {
                throw;
                //SIGLogMessage.PublishNoODS(Self, Format('Exception ''%s'' raised in TTTMDesign.AddTrianglePieceToSubgridIndex', [E.Message]), slmcException);
            }
        }

        private void ScanCellsOverTriangle(SubGridTree tree,
                                           int triIndex,
                                           Func<SubGridTree, uint, uint, bool> leafSatisfied,
                                           Action<SubGridTree, uint, uint, int> includeTriangleInLeaf,
                                           Action<SubGridTree,
                                                  int, // sourceTriangle
                                                  Func<SubGridTree, uint, uint, bool>,  // leafSatisfied
                                                  Action<SubGridTree, uint, uint, int>, // includeTriangleInLeaf
                                             XYZ, XYZ, XYZ, bool> ProcessTrianglePiece)
        {
            Triangle Tri = TriangleItems[triIndex];

      // Split triangle into two pieces, a 'top' piece and a 'bottom' piece to simplify
      // scanning across the triangle. Split is always with a horizontal line

            XYZ[] SortVertices = new XYZ[]
            {
              VertexItems[Tri.Vertex0],
              VertexItems[Tri.Vertex1],
              VertexItems[Tri.Vertex2]
            };

            if (SortVertices[0].Y > SortVertices[1].Y) SwapVertices(ref SortVertices[0], ref SortVertices[1]);
            if (SortVertices[1].Y > SortVertices[2].Y) SwapVertices(ref SortVertices[1], ref SortVertices[2]);
            if (SortVertices[0].Y > SortVertices[1].Y) SwapVertices(ref SortVertices[0], ref SortVertices[1]);

            XYZ TopVertex = SortVertices[2];
            XYZ CentralVertex = SortVertices[1];
            XYZ BottomVertex = SortVertices[0];

            // now make sure leftmost vertex in in first array item
            if (SortVertices[0].X > SortVertices[1].X) SwapVertices(ref SortVertices[0], ref SortVertices[1]);
            if (SortVertices[1].X > SortVertices[2].X) SwapVertices(ref SortVertices[1], ref SortVertices[2]);
            if (SortVertices[0].X > SortVertices[1].X) SwapVertices(ref SortVertices[0], ref SortVertices[1]);

            XYZ LeftMostVertex = SortVertices[0];
            XYZ RightMostVertex = SortVertices[2];

            // Are top or bottom vertices coincident with the middle vertex
            bool BottomPieceOnly = Math.Abs(TopVertex.Y - CentralVertex.Y) < 0.0001;
            bool TopPieceOnly = Math.Abs(BottomVertex.Y - CentralVertex.Y) < 0.0001;

            if (TopPieceOnly && BottomPieceOnly) // It's a thin horizontal triangle
            {
                ProcessTrianglePiece(tree, triIndex, leafSatisfied, includeTriangleInLeaf, LeftMostVertex, RightMostVertex, CentralVertex, true);
            }
            else
            {
                if (!(TopPieceOnly || BottomPieceOnly))
                {
                    // Divide triangle in two with a hort line
                    // Find intersection point of triangle edge between top most and bottom most vertice
                    if (LineIntersection.LinesIntersect(LeftMostVertex.X - 1, CentralVertex.Y,
                                                        RightMostVertex.X + 1, CentralVertex.Y,
                                                        TopVertex.X, TopVertex.Y,
                                                        BottomVertex.X, BottomVertex.Y,
                                                        out double IntersectX, out double IntersectY, true, out _))
                    {
                        XYZ IntersectionVertex = new XYZ(IntersectX, IntersectY, 0);
                        ProcessTrianglePiece(tree, triIndex, leafSatisfied, includeTriangleInLeaf, CentralVertex, IntersectionVertex, TopVertex, false);
                        ProcessTrianglePiece(tree, triIndex, leafSatisfied, includeTriangleInLeaf, CentralVertex, IntersectionVertex, BottomVertex, false);
                    }
                    else
                    {
                        Log.LogWarning($"Triangle {Tri} failed to have intersection line calculated for it");
                    }
                }
                else
                {
                    if (TopPieceOnly)
                    {
                        ProcessTrianglePiece(tree, triIndex, leafSatisfied, includeTriangleInLeaf, BottomVertex, CentralVertex, TopVertex, false);
                    }
                    else // BottomPieceOnly
                    {
                        ProcessTrianglePiece(tree, triIndex, leafSatisfied, includeTriangleInLeaf, TopVertex, CentralVertex, BottomVertex, false);
                    }
                }
            }
        }

        public override bool ComputeFilterPatch(double StartStn, double EndStn, double LeftOffset, double RightOffset,
                                                SubGridTreeBitmapSubGridBits Mask,
                                                ref SubGridTreeBitmapSubGridBits Patch,
                                                double OriginX, double OriginY,
                                                double CellSize,
                                                DesignDescriptor DesignDescriptor)
        {
            float[,] Heights = new float[SubGridTree.SubGridTreeDimension, SubGridTree.SubGridTreeDimension];

            if (InterpolateHeights(Heights, OriginX, OriginY, CellSize, DesignDescriptor.Offset))
            {
                // Iterate over the cell bitmask in Mask (ie: the cell this function is instructed to care about and remove cell fromn
                // that mask where there is no non-null elevation in the heights calculated by InterpolateHeights. Return the result
                // back as Patch. Use TempMask as local var capturable by the anonymous function...
                SubGridTreeBitmapSubGridBits TempMask = Mask;

                TempMask.ForEachSetBit((x, y) => { if (Heights[x, y] == Common.Consts.NullHeight) TempMask.ClearBit(x, y); });
                Patch = TempMask;

                //{$IFDEF DEBUG}
                //SIGLogMessage.PublishNoODS(Self, Format('Filter patch construction successful with %d bits', [Patch.CountBits]), slmcDebug);
                //{$ENDIF}

                return true;
            }
            else
            {
                //{$IFDEF DEBUG}
                //SIGLogMessage.PublishNoODS(Self, Format('Filter patch construction failed...', []), slmcDebug);
                //{$ENDIF}

                return false;
            }
        }

        /// <summary>
        /// Constructs the Subgrid existance map for the design
        /// </summary>
        /// <returns></returns>
        protected bool ConstructSubgridIndex()
        {
            // Read through all the triangles in the model and, for each triangle,
            // determine which subgrids intersect it and set the appropriate bits in the
            // subgrid index.
            try
            {
                Log.LogInformation($"In: Constructing subgrid index for design containing {FData.Triangles.Items.Length} triangles");

                try
                {
                    int triangleCount = FData.Triangles.Items.Length;
                    for (int triIndex = 0; triIndex < triangleCount; triIndex++)
                    {
                        ScanCellsOverTriangle(FSubgridIndex,
                                              triIndex,
                                              (tree, x, y) => (tree as SubGridTreeSubGridExistenceBitMask)[x, y],
                                              (tree, x, y, t) => (tree as SubGridTreeSubGridExistenceBitMask)[x, y] = true,
                                              AddTrianglePieceToSubgridIndex
                                              );
                    }
                }
                finally
                {
                    //SIGLogMessage.PublishNoODS(Self, Format('Out: Constructing subgrid index for design %s containing %d triangles', [SubgridIndexFileName, FData.Triangles.Count]), slmcMessage);
                }

                return true;
            }
            catch (Exception e)
            {
                Log.LogError($"Exception in TTTMDesign.ConstructSubgridIndex: {e}");
                return false;
            }
        }

        /// <summary>
        /// Constructor for a TTMDesign that takes the underlying cell size for the site model that will be used when interpolating heights from the design surface
        /// </summary>
        /// <param name="ACellSize"></param>
        public TTMDesign(double ACellSize)
        {
            FData = new TrimbleTINModel();
            TriangleItems = FData.Triangles.Items;
            VertexItems = FData.Vertices.Items;

            FCellSize = ACellSize;

            // Create a subgrid tree bit mask index that holds one bit per on-the-ground
            // subgrid that intersects at least one triangle in the TTM.
            FSubgridIndex = new SubGridTreeSubGridExistenceBitMask
            {
                CellSize = SubGridTree.SubGridTreeDimension * ACellSize
            };

            // Create a subgrid tree spatial index for triangles in the TTM
            // Removed, this is the 'list of triangles per subgrid' spatial index model
            // FSpatialIndex = new GenericSubGridTree<List<Triangle>>(SubGridTree.SubGridTreeLevels - 1, SubGridTree.SubGridTreeDimension * ACellSize);

            // Create the optimised subgrid tree spatial index that minmises the number of allocations in the final result.
            FSpatialIndexOptimised = new GenericSubGridTree<TriangleArrayReference>(SubGridTree.SubGridTreeLevels - 1, SubGridTree.SubGridTreeDimension * ACellSize);
        }

        /// <summary>
        /// Retrieves the ground extents of the TTM design 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public override void GetExtents(out double x1, out double y1, out double x2, out double y2)
        {
            x1 = FData.Header.MinimumEasting;
            y1 = FData.Header.MinimumNorthing;
            x2 = FData.Header.MaximumEasting;
            y2 = FData.Header.MaximumNorthing;
        }

        /// <summary>
        /// Retrieves the elevation range of the vertices in the TTm design surface
        /// </summary>
        /// <param name="z1"></param>
        /// <param name="z2"></param>
        public override void GetHeightRange(out double z1, out double z2)
        {
            if (FMinHeight == Common.Consts.NullReal || FMaxHeight == Common.Consts.NullReal) // better calculate them
            {
                FMinHeight = 1E100;
                FMaxHeight = -1E100;

                foreach (var vertex in VertexItems)
                {
                    if (vertex.Z < FMinHeight) FMinHeight = vertex.Z;
                    if (vertex.Z > FMaxHeight) FMaxHeight = vertex.Z;
                }
            }

            z1 = FMinHeight;
            z2 = FMaxHeight;
        }

        public override bool HasElevationDataForSubGridPatch(double X, double Y)
        {
            FSubgridIndex.CalculateIndexOfCellContainingPosition(X, Y, out uint SubgridX, out uint SubgridY);
            return FSubgridIndex[SubgridX, SubgridY];
        }

        public override bool HasElevationDataForSubGridPatch(uint SubGridX, uint SubGridY) => FSubgridIndex[SubGridX, SubGridY];

        public override bool HasFiltrationDataForSubGridPatch(double X, double Y) => false;

        public override bool HasFiltrationDataForSubGridPatch(uint SubGridX, uint SubgridY) => false;

      private double GetHeight(Triangle tri, double X, double Y)
      {
        return XYZ.GetTriangleHeight(VertexItems[tri.Vertex0],
                                     VertexItems[tri.Vertex1],
                                     VertexItems[tri.Vertex2], X, Y);
      }

    private bool CheckHint(ref int hint, double x, double y, double offset, out double z)
        {
            z = GetHeight(TriangleItems[hint], x, y);
            if (z != Common.Consts.NullReal)
            {
                z += offset;
                return true;
            }

            hint = -1;
            return false;
        }

        /// <summary>
        /// Interpolates a single spot height fromn the design
        /// </summary>
        /// <param name="Hint"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Offset"></param>
        /// <param name="Z"></param>
        /// <returns></returns>
        public bool InterpolateHeight2(ref int Hint,
                                       double X, double Y,
                                       double Offset,
                                       out double Z)
        {
            // Member added for compilation only
            if (Hint != -1 && CheckHint(ref Hint, X, Y, Offset, out Z))
                return true;

            GenericSubGridTree<List<int>> FSpatialIndex = null;

            // Search in the subgrid triangle list for this subgrid from the spatial index
            FSpatialIndex.CalculateIndexOfCellContainingPosition(X, Y, out uint CellX, out uint CellY);

            List<int> cell = FSpatialIndex[CellX, CellY];

            if (cell == null)
            {
                // There are no triangles that can satisfy the query
                Z = Common.Consts.NullReal;
                return false;
            }
            
            // Search the triangles in the leaf to locate the one to interpolate height from
            foreach (int triIndex in cell)
            {
                Z = GetHeight(TriangleItems[triIndex], X, Y);

                if (Z != Common.Consts.NullReal)
                {
                    Hint = triIndex;
                    Z += Offset;
                    return true;
                }
            }

            Z = Common.Consts.NullReal;
            return false; 
        }

//      private uint maxArrayCount = 0;

        /// <summary>
        /// Interpolates a single spot height fromn the design, using the optimised spatial index
        /// </summary>
        /// <param name="Hint"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Offset"></param>
        /// <param name="Z"></param>
        /// <returns></returns>
        public override bool InterpolateHeight(ref int Hint,
                                      double X, double Y,
                                      double Offset,
                                      out double Z)
        {
            if (Hint != -1 && CheckHint(ref Hint, X, Y, Offset, out Z))
                return true;

            // Search in the subgrid triangle list for this subgrid from the spatial index

            FSpatialIndexOptimised.CalculateIndexOfCellContainingPosition(X, Y, out uint CellX, out uint CellY);

            TriangleArrayReference arrayReference = FSpatialIndexOptimised[CellX, CellY];

            if (arrayReference.Count == 0)
            {
                // There are no triangles that can satisfy the query
                Z = Common.Consts.NullReal;
                return false;
            }

//          if (maxArrayCount < arrayReference.Count)
//          {
//            maxArrayCount = arrayReference.Count;
//            Console.WriteLine($"Largest array list encountered with {maxArrayCount} triangles");
//          }

          // Search the triangles in the leaf to locate the one to interpolate height from
            uint limit = arrayReference.TriangleArrayIndex + arrayReference.Count;
            for (uint i = arrayReference.TriangleArrayIndex; i < limit; i++)
            {
                int triIndex = FSpatialIndexOptimisedTriangles[i];
                Z = GetHeight(TriangleItems[triIndex], X, Y);

                if (Z != Common.Consts.NullReal)
                {
                    Hint = triIndex;
                    Z += Offset;
                    return true;
                }
            }

            Z = Common.Consts.NullReal;
            return false;
    }

/* COMMENTED OUT AS A RESULT OF USE OF THE OPIMISED TTM FORMAT
        /// <summary>
        /// Interpolates a single spot height fromn the design
        /// </summary>
        /// <param name="SearchState"></param>
        /// <param name="Hint"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Offset"></param>
        /// <param name="Z"></param>
        /// <returns></returns>
        public bool InterpolateHeight1(ref TriangleQuadTree.Tsearch_state_rec SearchState,
                                       ref int Hint,
                                       double X, double Y,
                                       double Offset,
                                       out double Z)
        {
            // Local var added to allow compilation only - shadows the class scope member of same name
            TTMQuadTree QuadTreeSpatialIndex = null;

            if (Hint != -1 && CheckHint(ref Hint, X, Y, Offset, out Z))
                return true;
            
            SearchState.start_search(X - 0.1, Y - 0.1, X + 0.1, Y + 0.1, true, QuadTreeSpatialIndex);

            int eindex = 0;
            int iterationCount = 0;

            while (QuadTreeSpatialIndex.next_entity(ref SearchState, ref eindex, ref Hint))
            {
                iterationCount++;
                Z = ((Triangle)Hint).GetHeight(X, Y);

                if (Z != Common.Consts.NullReal)
                {
                    Z += Offset;
                    return true;
                }
            }

            Hint = null;
            Z = Common.Consts.NullReal;

            return false;
        }
*/

      /*
        /// <summary>
        /// Interpolates heights from the design for all the cells in a subgrid
        /// </summary>
        /// <param name="Patch"></param>
        /// <param name="OriginX"></param>
        /// <param name="OriginY"></param>
        /// <param name="CellSize"></param>
        /// <param name="Offset"></param>
        /// <returns></returns>
        public override bool InterpolateHeights(float[,] Patch,
                                                double OriginX, double OriginY,
                                                double CellSize,
                                                double Offset)
        {
            return InterpolateHeights3(Patch, OriginX, OriginY, CellSize, Offset);
        }
        */


/* COMMENTED OUT AS A RESULT OF USE OF THE OPIMISED TTM FORMAT

        /// <summary>
        /// Interpolates heights from the design for all the cells in a subgrid
        /// </summary>
        /// <param name="Patch"></param>
        /// <param name="OriginX"></param>
        /// <param name="OriginY"></param>
        /// <param name="CellSize"></param>
        /// <param name="Offset"></param>
        /// <returns></returns>
        public bool InterpolateHeights1(float[,] Patch, 
                                        double OriginX, double OriginY,
                                        double CellSize,
                                        double Offset)
        {
            int ValueCount = 0;
            object Hint = null;
            double HalfCellSize = CellSize / 2;
            double OriginXPlusHalfCellSize = OriginX + HalfCellSize;
            double OriginYPlusHalfCellSize = OriginY + HalfCellSize;

            TriangleQuadTree.Tsearch_state_rec SearchState = TriangleQuadTree.Tsearch_state_rec.Init();

            try
            {
                SubGridUtilities.SubGridDimensionalIterator((x, y) =>
                {
                    if (InterpolateHeight1(ref SearchState,
                                           ref Hint,
                                           OriginXPlusHalfCellSize + (CellSize * x),
                                           OriginYPlusHalfCellSize + (CellSize * y),
                                           Offset, out double Z))
                    {
                        Patch[x, y] = (float)Z;
                        ValueCount++;
                    }
                    else
                    {
                        Patch[x, y] = Common.Consts.NullHeight;
                    }
                });

                return ValueCount > 0;
            }
            catch (Exception e)
            {
                Log.LogError($"Exception {e} occurred in TTTMDesign.InterpolateHeights");
                return false;
            }
        }
*/

        /// <summary>
        /// Interpolates heights from the design for all the cells in a subgrid
        /// </summary>
        /// <param name="Patch"></param>
        /// <param name="OriginX"></param>
        /// <param name="OriginY"></param>
        /// <param name="CellSize"></param>
        /// <param name="Offset"></param>
        /// <returns></returns>
        public bool InterpolateHeights2(float[,] Patch,
                                        double OriginX, double OriginY,
                                        double CellSize,
                                        double Offset)
        {
            int ValueCount = 0;
            int Hint = -1;
            double HalfCellSize = CellSize / 2;
            double OriginXPlusHalfCellSize = OriginX + HalfCellSize;
            double OriginYPlusHalfCellSize = OriginY + HalfCellSize;

            try
            {
                SubGridUtilities.SubGridDimensionalIterator((x, y) =>
                {
                    if (InterpolateHeight2(ref Hint,
                                           OriginXPlusHalfCellSize + (CellSize * x),
                                           OriginYPlusHalfCellSize + (CellSize * y),
                                           Offset, out double Z))
                    {
                        Patch[x, y] = (float)Z;
                        ValueCount++;
                    }
                    else
                    {
                        Patch[x, y] = Common.Consts.NullHeight;
                    }
                });

                return ValueCount > 0;
            }
            catch (Exception e)
            {
                Log.LogError($"Exception {e} occurred in TTTMDesign.InterpolateHeights");

                return false;
            }
        }

        /// <summary>
        /// Interpolates heights from the design for all the cells in a subgrid
        /// </summary>
        /// <param name="Patch"></param>
        /// <param name="OriginX"></param>
        /// <param name="OriginY"></param>
        /// <param name="CellSize"></param>
        /// <param name="Offset"></param>
        /// <returns></returns>
        public override bool InterpolateHeights(float[,] Patch,
                                                double OriginX, double OriginY,
                                                double CellSize,
                                                double Offset)
        {
            int ValueCount = 0;
            int Hint = -1;
            double HalfCellSize = CellSize / 2;
            double OriginXPlusHalfCellSize = OriginX + HalfCellSize;
            double OriginYPlusHalfCellSize = OriginY + HalfCellSize;

            try
            {
                SubGridUtilities.SubGridDimensionalIterator((x, y) =>
                {
                    if (InterpolateHeight(ref Hint,
                                          OriginXPlusHalfCellSize + CellSize * x,
                                          OriginYPlusHalfCellSize + CellSize * y,
                                          Offset, out double Z))
                    {
                        Patch[x, y] = (float)Z;
                        ValueCount++;
                    }
                    else
                    {
                        Patch[x, y] = Common.Consts.NullHeight;
                    }
                });

                return ValueCount > 0;
            }
            catch (Exception e)
            {
                Log.LogError($"Exception {e} occurred in TTTMDesign.InterpolateHeights");
                return false;
            }
        }

      // public int TotalDuplicates = 0;

    /// <summary>
    /// Includes a triangle into the list of triangles that intersect the extent of a subgrid
    /// </summary>
    /// <param name="tree"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="triIndex"></param>
    private void IncludeTriangleInSubGridTreeIndex(GenericSubGridTree<List<int>> tree, uint x, uint y, int triIndex)
        {
            // Get subgrid from tree, creating the path and leaf if necessary
            GenericLeafSubGrid<List<int>> leaf = tree.ConstructPathToCell(x, y, SubGridPathConstructionType.CreateLeaf) as GenericLeafSubGrid<List<int>>;

            leaf.GetSubGridCellIndex(x, y, out byte SubGridX, out byte SubGridY);

            // Get the list of triangles for the given cell
            List<int> triangles = leaf.Items[SubGridX, SubGridY];

            // If there are none already create the list and assign it to the cell
            if (triangles == null)
            {
                triangles = new List<int>();
                leaf.Items[SubGridX, SubGridY] = triangles;
            }

            // Add the triangle to the cell, even if it is already there (duplicates will be taken care of later)
            triangles.Add(triIndex);
        }

        /// <summary>
        /// Build a spatial index for the triangles in the TIN surface by assigning each triangle to every subgrid it intersects with
        /// </summary>
        /// <returns></returns>
        private bool ConstructSpatialIndex()
        {
            // Read through all the triangles in the model and, for each triangle,
            // determine which subgrids in the index intersect it and add it to those subgrids
            try
            {
                var FSpatialIndex = new GenericSubGridTree<List<int>>(SubGridTree.SubGridTreeLevels - 1, SubGridTree.SubGridTreeDimension * FCellSize);

                Log.LogInformation($"In: Constructing subgrid index for design containing {FData.Triangles.Items.Length} triangles");
                try
                {
                    // Construct a subgrid tree containing list of triangles that intersect each on-the-ground subgrid
                    int triangleCount = FData.Triangles.Items.Length;
                    for (int triIndex = 0; triIndex < triangleCount; triIndex++)
                    {
                        ScanCellsOverTriangle(FSpatialIndex,
                                              triIndex,
                                              (tree, x, y) => false,
                                              (tree, x, y, t) => IncludeTriangleInSubGridTreeIndex(tree as GenericSubGridTree<List<int>>, x, y, t),
                                              AddTrianglePieceToSubgridIndex);
                    }

                    /////////////////////////////////////////////////
                    // Remove duplicate triangles added to the lists
                    /////////////////////////////////////////////////
                    BitArray uniques = new BitArray(TriangleItems.Length);

                    FSpatialIndex.ScanAllSubGrids(leaf =>
                    {
                      // Iterate across all cells in each (level 5) leaf subgrid. Each cell represents 
                      // a subgrid in the level 6 subgrid representing cells sampled across the surface at the
                      // core cell size for the project
                      SubGridUtilities.SubGridDimensionalIterator((x, y) =>
                      {
                        List<int> triList = FSpatialIndex[leaf.OriginX + x, leaf.OriginY + y];
                 
                        if (triList == null)
                          return;

                        uniques.SetAll(false);

                        int triListCount = triList.Count;
                        int uniqueCount = 0;
                        for (int i = 0; i < triListCount; i++)
                        {
                          var triIndex = triList[i];
                          if (!uniques[triIndex])
                          {
                            triList[uniqueCount++] = triIndex;
                            uniques[triIndex] = true;
                          }
                        }

                        if (uniqueCount < triListCount)
                          triList.RemoveRange(uniqueCount, triListCount - uniqueCount);
                      });
                 
                      return true;
                    });
                 
                    // Console.WriteLine($"Total duplicates encountered: {TotalDuplicates}");
                 
                    // Transform this subgrid tree into one where each on-the-ground subgrid is represented by an index and a number of triangles present in a
                    // a single list of triangles.
                 
                    // Count the numnber of triangle references present in the tree
                    int numTriangleReferences = 0;
                    FSpatialIndex.ForEach(x => { numTriangleReferences += x?.Count ?? 0; return true; });

                    // Create the single array
                    FSpatialIndexOptimisedTriangles = new int[numTriangleReferences];

                    // Copy all triangle lists into it, and add the appropriate reference blocks in the new tree.

                    uint copiedCount = 0;

                    /////////////////////////////////////////////////
                    // Iterate across all leaf subgrids
                    /////////////////////////////////////////////////
                 
                    FSpatialIndex.ScanAllSubGrids(leaf =>
                    {
                        // Iterate across all cells in each (level 5) leaf subgrid. Each cell represents 
                        // a subgrid in the level 6 subgrid representing cells sampled across the surface at the
                        // core cell size for the project
                        SubGridUtilities.SubGridDimensionalIterator((x, y) =>
                        {
                            List<int> triList = FSpatialIndex[leaf.OriginX + x, leaf.OriginY + y];

                            if (triList == null)
                                return;

                            // Copy triangles
                            Array.Copy(triList.ToArray(), 0, FSpatialIndexOptimisedTriangles, copiedCount, triList.Count);

                            // Add new entry for optimised tree
                            FSpatialIndexOptimised[leaf.OriginX + x, leaf.OriginY + y] = new TriangleArrayReference
                            {
                                TriangleArrayIndex = copiedCount,
                                Count = (uint)triList.Count
                            };

                            // Keep track of how may have been copied
                            copiedCount += (uint)triList.Count;
                        });

                        return true;
                    });
                }
                finally
                {
                    // Emit some logging indicating likely efficiency of index.
                    long sumTriangleReferences = 0;
                    long sumTriangleLists = 0;
                    long sumLeafSubGrids = 0;
                    long sumNodeSubGrids = 0;

                    FSpatialIndex.ScanAllSubGrids(l => { sumLeafSubGrids++; return true; },
                                                  n => { sumNodeSubGrids++; return SubGridProcessNodeSubGridResult.OK; });

                    FSpatialIndex.ForEach(x =>
                    {
                        sumTriangleLists++;
                        sumTriangleReferences += x?.Count ?? 0;
                        return true;
                    });

                    Log.LogInformation($"Constructed subgrid index for design containing {FData.Triangles.Items.Length} triangles, using {sumLeafSubGrids} leaf and {sumNodeSubGrids} node subgrids, {sumTriangleLists} triangle lists and {sumTriangleReferences} triangle references");
                }

                return true;
            }
            catch (Exception e)
            {
                Log.LogError($"Exception {e} in TTTMDesign.ConstructSpatialIndex");
                return false;
            }
        }

        /// <summary>
        /// Loads the TTM design from a TTM file, along with the subgrid existance map file if it exists (created otherwise)
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override DesignLoadResult LoadFromFile(string fileName)
        {
            try
            {
                FData.LoadFromFile(fileName);
                TriangleItems = FData.Triangles.Items;
                VertexItems = FData.Vertices.Items; 

                Log.LogInformation($"Loaded TTM file {fileName} containing {FData.Header.NumberOfTriangles} triangles and {FData.Header.NumberOfVertices} vertices.");

                FMinHeight = Common.Consts.NullReal;
                FMaxHeight = Common.Consts.NullReal;

                // Build the subgrid tree based spatial index
                ConstructSpatialIndex();

                if (!LoadSubgridIndexFile(fileName + Consts.kDesignSubgridIndexFileExt))
                    return DesignLoadResult.UnableToLoadSubgridIndex;

                Log.LogInformation($"Area: ({FData.Header.MinimumEasting}, {FData.Header.MinimumNorthing}) -> ({FData.Header.MaximumEasting}, {FData.Header.MaximumNorthing}): [{FData.Header.MaximumEasting - FData.Header.MinimumEasting} x {FData.Header.MaximumNorthing - FData.Header.MinimumNorthing}]");

                // Build the quadtree based spatial index
                // Removed, this is the old quadtree spatial index, not performant enough
                //QuadTreeSpatialIndex = new TTMQuadTree();
                //QuadTreeSpatialIndex.Initialise(FData, false);
                //Log.LogInformation($"Constructed quadtree spatial index using {QuadTreeSpatialIndex.BATree.Count} BTree blocks");

                return DesignLoadResult.Success;
            }
            catch (Exception E)
            {
                Log.LogError($"Exception {E} in LoadFromFile");
                return DesignLoadResult.UnknownFailure;
            }
        }

        /// <summary>
        /// Loads the subgrid existence map from a file
        /// </summary>
        /// <param name="SubgridIndexFileName"></param>
        /// <returns></returns>
        protected bool LoadSubgridIndex(string SubgridIndexFileName)
        {
            try
            {
                if (File.Exists(SubgridIndexFileName))
                {
                  using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(SubgridIndexFileName)))
                  {
                    using (BinaryReader reader = new BinaryReader(ms))
                    {
                      return SubGridTreePersistor.Read(FSubgridIndex, reader);
                    }
                  }
                }

                return false;
            }
            catch (Exception e)
            {
                Log.LogError($"Exception {e} in LoadSubgridIndex");

                return false;
            }
        }

        /// <summary>
        /// Loads a subgrid existence map for the design from a file
        /// </summary>
        /// <param name="SubgridIndexFileName"></param>
        /// <returns></returns>
        protected bool LoadSubgridIndexFile(string SubgridIndexFileName)
        {
            Log.LogInformation($"Loading subgrid index file {SubgridIndexFileName}");

            bool Result = LoadSubgridIndex(SubgridIndexFileName);

            if (!Result)
            {
                Result = ConstructSubgridIndex();

                if (Result)
                {
                    if (SaveSubgridIndex(SubgridIndexFileName))
                        Log.LogInformation($"Saved constructed subgrid index file {SubgridIndexFileName}");
                    else
                        Log.LogError($"Unable to save subgrid index file {SubgridIndexFileName} - continuing with unsaved index");
                }
                else
                    Log.LogError($"Unable to create and save subgrid index file {SubgridIndexFileName}");
            }

            return Result;
        }

        /// <summary>
        /// Daves a subgrid existence map for the design to a file
        /// </summary>
        /// <param name="SubgridIndexFileName"></param>
        /// <returns></returns>
        protected bool SaveSubgridIndex(string SubgridIndexFileName)
        {
            try
            {
                // Write the index out to a file
                using (FileStream fs = new FileStream(SubgridIndexFileName, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    using (BinaryWriter writer = new BinaryWriter(fs))
                    {
                        SubGridTreePersistor.Write(FSubgridIndex, writer);
                    }
                }

                if (!File.Exists(SubgridIndexFileName))
                {
                    Thread.Sleep(500); // Seems to be a Windows update problem hence introduce delay b4 checking again
                }

                /*
                if (!File.Exists(SubgridIndexFileName))
                {
                    try
                    {
                        FSubgridIndexStream.SaveToFile(SubgridIndexFileName);
                    }
                    catch (Exception E)
                    {
                        // When this occurs the file does appear to be saved hence warning only

                        // Readd when logging available
                        //SIGLogMessage.PublishNoODS(Self, 'Note: The following exception is reported as informational and may not signal an operational issue', slmcMessage);
                        //SIGLogMessage.PublishNoODS(Self, Format('Exception ''%s'' in %s.SaveSubgridIndex', [E.Message, Self.ClassName]), slmcWarning);
                    }
                }
                */

              return true;
            }
            catch (Exception e)
            {
                Log.LogError($"Exception {e} SaveSubgridIndex");
            }

            return false;
        }

        /// <summary>
        /// Determines if the bounds of a subgrid intersects a given triangle
        /// </summary>
        /// <param name="Extents"></param>
        /// <param name="H1"></param>
        /// <param name="H2"></param>
        /// <param name="V"></param>
        /// <returns></returns>
        private bool SubGridIntersectsTriangle(BoundingWorldExtent3D Extents, XYZ H1, XYZ H2, XYZ V)
        {
            // If any of the triangle vertices are in the cell extents then 'yes'
            if (Extents.Includes(H1.X, H1.Y) || Extents.Includes(H2.X, H2.Y) || Extents.Includes(V.X, V.Y))
            {
                return true;
            }

            // If any of the subgrid corners sit in the triangle then 'yes'
            {
                if (XYZ.PointInTriangle(H1, H2, V, Extents.MinX, Extents.MinY) ||
                    XYZ.PointInTriangle(H1, H2, V, Extents.MinX, Extents.MaxY) ||
                    XYZ.PointInTriangle(H1, H2, V, Extents.MaxX, Extents.MaxY) ||
                    XYZ.PointInTriangle(H1, H2, V, Extents.MaxX, Extents.MinY))
                {
                    return true;
                }
            }

            // If any of the extent and triangle lines intersect then also 'yes'
            if (LineIntersection.LinesIntersect(Extents.MinX, Extents.MinY, Extents.MaxX, Extents.MinY, H1.X, H1.Y, H2.X, H2.Y, out _, out _, false, out _) ||
                LineIntersection.LinesIntersect(Extents.MaxX, Extents.MinY, Extents.MaxX, Extents.MaxY, H1.X, H1.Y, H2.X, H2.Y, out _, out _, false, out _) ||
                LineIntersection.LinesIntersect(Extents.MinX, Extents.MaxY, Extents.MaxX, Extents.MaxY, H1.X, H1.Y, H2.X, H2.Y, out _, out _, false, out _) ||
                LineIntersection.LinesIntersect(Extents.MinX, Extents.MinY, Extents.MinX, Extents.MaxY, H1.X, H1.Y, H2.X, H2.Y, out _, out _, false, out _) ||

                LineIntersection.LinesIntersect(Extents.MinX, Extents.MinY, Extents.MaxX, Extents.MinY, H1.X, H1.Y, V.X, V.Y, out _, out _, false, out _) ||
                LineIntersection.LinesIntersect(Extents.MaxX, Extents.MinY, Extents.MaxX, Extents.MaxY, H1.X, H1.Y, V.X, V.Y, out _, out _, false, out _) ||
                LineIntersection.LinesIntersect(Extents.MinX, Extents.MaxY, Extents.MaxX, Extents.MaxY, H1.X, H1.Y, V.X, V.Y, out _, out _, false, out _) ||
                LineIntersection.LinesIntersect(Extents.MinX, Extents.MinY, Extents.MinX, Extents.MaxY, H1.X, H1.Y, V.X, V.Y, out _, out _, false, out _) ||

                LineIntersection.LinesIntersect(Extents.MinX, Extents.MinY, Extents.MaxX, Extents.MinY, V.X, V.Y, H2.X, H2.Y, out _, out _, false, out _) ||
                LineIntersection.LinesIntersect(Extents.MaxX, Extents.MinY, Extents.MaxX, Extents.MaxY, V.X, V.Y, H2.X, H2.Y, out _, out _, false, out _) ||
                LineIntersection.LinesIntersect(Extents.MinX, Extents.MaxY, Extents.MaxX, Extents.MaxY, V.X, V.Y, H2.X, H2.Y, out _, out _, false, out _) ||
                LineIntersection.LinesIntersect(Extents.MinX, Extents.MinY, Extents.MinX, Extents.MaxY, V.X, V.Y, H2.X, H2.Y, out _, out _, false, out _))
            {
                return true;
            };

            // Otherwise 'no'
            return false;
        }

        /// <summary>
        /// A reference to the internal subgrid existence map for the design
        /// </summary>
        /// <returns></returns>
        public override SubGridTreeSubGridExistenceBitMask SubgridOverlayIndex() => FSubgridIndex;
    }
}
