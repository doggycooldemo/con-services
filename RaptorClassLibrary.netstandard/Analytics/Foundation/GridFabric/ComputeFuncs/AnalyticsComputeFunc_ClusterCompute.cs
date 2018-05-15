﻿using Apache.Ignite.Core.Compute;
using log4net;
using System;
using System.Reflection;
using VSS.TRex.Analytics.Coordinators;
using VSS.TRex.Analytics.GridFabric.Responses;
using VSS.TRex.GridFabric.Arguments;
using VSS.TRex.GridFabric.ComputeFuncs;
using VSS.TRex.GridFabric.Grids;
using VSS.TRex.GridFabric.Requests.Interfaces;
using VSS.TRex.Servers;

namespace VSS.TRex.Analytics.GridFabric.ComputeFuncs
{
    public class AnalyticsComputeFunc_ClusterCompute<TArgument, TResponse, TCoordinator> : BaseComputeFunc, IComputeFunc<TArgument, TResponse>
        where TArgument : BaseApplicationServiceRequestArgument
        where TResponse : BaseAnalyticsResponse, IAggregateWith<TResponse>, new()
        where TCoordinator : BaseAnalyticsCoordinator<TArgument, TResponse>, new()
    {
        [NonSerialized]
        // ReSharper disable once StaticMemberInGenericType
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public AnalyticsComputeFunc_ClusterCompute(string gridName, string role) : base(gridName, role)
        {
        }

        public AnalyticsComputeFunc_ClusterCompute() : this(TRexGrids.ImmutableGridName(), ServerRoles.PSNODE)
        {
        }

        /// <summary>
        /// Invoke the statistics request locally on this node
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public TResponse Invoke(TArgument arg)
        {
            Log.Info("In AnalyticsComputeFunc_ClusterCompute.Invoke()");

            try
            {
                Log.Info("Executing AnalyticsComputeFunc_ClusterCompute.Execute()");

                TCoordinator coordinator = new TCoordinator();
                return coordinator.Execute(arg);
            }
            finally
            {
                Log.Info("Exiting AnalyticsComputeFunc_ClusterCompute.Invoke()");
            }
        }
    }
}
