﻿using VSS.TRex.GridFabric.Grids;
using VSS.TRex.GridFabric.Requests;
using VSS.TRex.Servers;

namespace VSS.TRex.GridFabric.ComputeFuncs
{
    /// <summary>
    ///  Represents a request that can be made against the design profiler cluster group in the Raptor grid
    /// </summary>
    public class CacheComputeComputeFunc<TArgument, TResponse> : BaseRequest<TArgument, TResponse>
    {
        /// <summary>
        /// Default no-arg constructor that sets up cluster and compute projections available for use
        /// </summary>
        public CacheComputeComputeFunc() : base(TRexGrids.ImmutableGridName(), ServerRoles.PSNODE)
        {
        }
    }
}
