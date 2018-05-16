﻿using VSS.TRex.GridFabric.Grids;

namespace VSS.TRex.GridFabric
{
    public class BaseImmutableIgniteClass : BaseIgniteClass
    {
        /// <summary>
        /// Default no-arg constructor that sets up cluster and compute projections available for use
        /// </summary>
        public BaseImmutableIgniteClass(string role) : base(TRexGrids.ImmutableGridName(), role)
        {
        }
    }
}
