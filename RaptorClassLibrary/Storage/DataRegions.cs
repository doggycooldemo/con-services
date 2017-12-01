﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSS.VisionLink.Raptor.Storage
{
    /// <summary>
    /// Handles actions and configuration related to data regions introduced in Ignite 2.3 to allow per-cache control of persistency, memory usage etc
    /// </summary>
    public static class DataRegions
    {
        /// <summary>
        /// The name of the default data region (an undifferentiated data region in the grid)
        /// </summary>
        public const string DEFAULT_DATA_REGION = "Default";

        /// <summary>
        /// The data region to place mutable spatial data cache information into
        /// </summary>
        public const string MUTABLE_SPATIAL_DATA_REGION = DEFAULT_DATA_REGION;

        /// <summary>
        /// The data region to place immutable spatial data cache information into
        /// </summary>
        public const string IMMUTABLE_SPATIAL_DATA_REGION = DEFAULT_DATA_REGION;

        /// <summary>
        /// The data region to place mutable nonspatial data cache information into
        /// </summary>
        public const string MUTABLE_NONSPATIAL_DATA_REGION = DEFAULT_DATA_REGION;

        /// <summary>
        /// The data region to place immutable nonspatial data cache information into
        /// </summary>
        public const string IMMUTABLE_NONSPATIAL_DATA_REGION = DEFAULT_DATA_REGION;

        /// <summary>
        /// THe data region to place spatial subgrid existance maps (usually computed from TTM topological designs and surveyed surfaces)
        /// </summary>
        public const string SPATIAL_EXISTENCEMAP_DATA_REGION = DEFAULT_DATA_REGION;
    }
}
