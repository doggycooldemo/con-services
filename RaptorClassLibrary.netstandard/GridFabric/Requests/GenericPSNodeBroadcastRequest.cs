﻿using Apache.Ignite.Core.Compute;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VSS.VisionLink.Raptor.Designs.GridFabric.Requests;
using VSS.VisionLink.Raptor.GridFabric.Requests.Interfaces;

namespace VSS.VisionLink.Raptor.GridFabric.Requests
{
    /// <summary>
    /// Provides a highly genericised class for making mapReduce style requests to the 'PSNode' compute cluster.
    /// This class coordinates broadcasting of requests to the compute cluster and reduce the final result
    /// from the per-partition results returned to it.
    /// </summary>
    /// <typeparam name="TArgument"></typeparam>
    /// <typeparam name="TComputeFunc"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class GenericPSNodeBroadcastRequest<TArgument, TComputeFunc, TResponse> : CacheComputePoolRequest<TArgument, TResponse>
        where TComputeFunc : IComputeFunc<TArgument, TResponse>, new()
        where TResponse : IResponseAggregateWith<TResponse>, new()
    {
        /// <summary>
        /// Executes a request genericised through it's templated types
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public override TResponse Execute(TArgument arg)
        {
            // Construct the function to be used
            IComputeFunc<TArgument, TResponse> func = new TComputeFunc();

            // Broadcast the request to the compute pool and assemble a list of the results
            Task<ICollection<TResponse>> taskResult = _Compute?.BroadcastAsync(func, arg);

            // Reduce the set of results to a single aggreted result and send the result back
            // If there is no task result then return an empty response
            return taskResult?.Result?.Count > 0 ? taskResult.Result.Aggregate((first, second) => first.AggregateWith(second)) : new TResponse();
        }
    }
}