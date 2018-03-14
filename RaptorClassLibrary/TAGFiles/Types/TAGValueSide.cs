﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSS.VisionLink.Raptor.TAGFiles.Types
{
    /// <summary>
    /// Enumeration used to denote values that have left and right sides, such as blade tip positions
    /// </summary>
    public enum TAGValueSide
    {
        /// <summary>
        /// Left hand blade tip, wheel, track, drum etc
        /// </summary>
        Left,

        /// <summary>
        /// Right hand blade tip, wheel, track, drum etc
        /// </summary>
        Right
    }
}
