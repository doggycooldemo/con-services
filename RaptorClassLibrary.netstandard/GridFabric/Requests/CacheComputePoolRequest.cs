﻿using VSS.VisionLink.Raptor.GridFabric.Grids;
using VSS.VisionLink.Raptor.GridFabric.Requests;
using VSS.VisionLink.Raptor.Servers;

namespace VSS.VisionLink.Raptor.Designs.GridFabric.Requests
{
    /// <summary>
    ///  Represents a request that can be made against the design profiler cluster group in the Raptor grid
    /// </summary>
    public class CacheComputePoolRequest<TArgument, TResponse> : BaseRaptorRequest<TArgument, TResponse>
    {
        /// <summary>
        /// Default no-arg constructor that sets up cluster and compute projections available for use
        /// </summary>
        public CacheComputePoolRequest() : base(RaptorGrids.RaptorImmutableGridName(), ServerRoles.PSNODE)
        {
        }
    }
}
