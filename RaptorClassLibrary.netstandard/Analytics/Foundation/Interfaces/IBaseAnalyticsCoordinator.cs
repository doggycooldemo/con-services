﻿using VSS.TRex.Analytics.Aggregators;
using VSS.TRex.Analytics.GridFabric.Responses;
using VSS.TRex.GridFabric.Arguments;
using VSS.TRex.SiteModels;

namespace VSS.TRex.Analytics.Foundation.Interfaces
{
    public interface IBaseAnalyticsCoordinator<TArgument, TResponse> 
        where TArgument : BaseApplicationServiceRequestArgument 
        where TResponse : BaseAnalyticsResponse, new()
    {
        /// <summary>
        /// The SiteModel context for computing the result of the request
        /// </summary>
        SiteModel SiteModel { get; set; }

        /// <summary>
        /// Request descriptor used to track this request in different parts of the cluster compute
        /// </summary>
        long RequestDescriptor { get; set; }

        /// <summary>
        /// Execution method for the derived coordinator to override
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        TResponse Execute(TArgument arg);

        /// <summary>
        /// Constructs the aggegator to be used as the reduction function for the MapReduceReduce computation
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        AggregatorBase ConstructAggregator(TArgument argument);

        /// <summary>
        /// Constructs the computer responsible for orchestrating information requests, essentially the map part of the MapReduceReduce computation
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="aggregator"></param>
        /// <returns></returns>
        AnalyticsComputor ConstructComputor(TArgument argument, AggregatorBase aggregator);

        /// <summary>
        /// Transcribes the results of the computation from the internal response type to the external response type
        /// </summary>
        /// <param name="aggregator"></param>
        /// <param name="response"></param>
        void ReadOutResults(AggregatorBase aggregator, TResponse response);
    }
}
