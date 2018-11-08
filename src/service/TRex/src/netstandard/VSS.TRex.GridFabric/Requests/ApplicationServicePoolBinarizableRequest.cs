﻿using VSS.TRex.GridFabric.Grids;
using VSS.TRex.GridFabric.Models.Servers;

namespace VSS.TRex.GridFabric.Requests
{
  /// <summary>
  ///  Represents a request that can be made against the design profiler cluster group in the TRex grid
  /// </summary>
  public abstract class ApplicationServicePoolBinarizableRequest<TArgument, TResponse> : BaseRequest<TArgument, TResponse>
  {
    /// <summary>
    /// Default no-arg constructor that sets up cluster and compute projections available for use
    /// </summary>
    public ApplicationServicePoolBinarizableRequest() : base(TRexGrids.ImmutableGridName(), ServerRoles.ASNODE)
    {
    }

    public ApplicationServicePoolBinarizableRequest(string gridName, string role) : base(gridName, role)
    {
    }
  }
}
