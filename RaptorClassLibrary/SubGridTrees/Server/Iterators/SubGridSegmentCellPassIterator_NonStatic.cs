﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSS.VisionLink.Raptor.SubGridTrees.Interfaces;
using VSS.VisionLink.Raptor.SubGridTrees.Server.Interfaces;
using VSS.VisionLink.Raptor.Utilities;

namespace VSS.VisionLink.Raptor.SubGridTrees.Server.Iterators
{  
    /// <summary>
    /// Iterates through cells in subgrids in a cell pass by cell pass manner.
    /// This version only operates on non-static cell passes
    /// </summary>
    public class SubGridSegmentCellPassIterator_NonStatic : SubGridSegmentCellPassIterator_Base, ISubGridSegmentCellPassIterator
    {
        /// <summary>
        /// Default no-arg constructor
        /// </summary>
        public SubGridSegmentCellPassIterator_NonStatic() : base()
        {
        }

        /// <summary>
        /// Construct a cell pass iterator using a given segment iterator and an optional maximum number of passes to return
        /// in the course of the iteration
        /// </summary>
        /// <param name="iterator"></param>
        public SubGridSegmentCellPassIterator_NonStatic(ISubGridSegmentIterator iterator, int maxNumberOfPassesToReturn = int.MaxValue) : base(iterator, maxNumberOfPassesToReturn)
        {
        }

        /// <summary>
        /// Provides non-static cell pass specific initialisation for the next segment
        /// </summary>
        /// <param name="direction"></param>
        protected override void InitialiseForNewSegment(IterationDirection direction)
        {
            if (SegmentIterator.IterationDirection == IterationDirection.Forwards)
            {
                cellInSegmentIndex = -1;
                finishCellInSegmentIndex = SegmentIterator.CurrentSubGridSegment.PassesData.PassData[cellX, cellY].PassCount;

                cellPassIterationDirectionIncrement = 1;
            }
            else
            {
                cellInSegmentIndex = SegmentIterator.CurrentSubGridSegment.PassesData.PassData[cellX, cellY].PassCount;
                finishCellInSegmentIndex = -1;

                cellPassIterationDirectionIncrement = -1;
            }
        }

        /// <summary>
        /// Provides non-static cell pass extract semantics for the currnet cell pass in the iteration
        /// </summary>
        /// <returns></returns>
        protected override CellPass ExtractCellPass()
        {
            return SegmentIterator.CurrentSubGridSegment.PassesData.PassData[cellX, cellY].Passes[cellInSegmentIndex];
        }
    }
}
