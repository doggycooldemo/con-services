﻿using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Cluster;
using Apache.Ignite.Core.Messaging;
using VSS.TRex.DI;
using VSS.TRex.GridFabric;
using VSS.TRex.GridFabric.Arguments;
using VSS.TRex.GridFabric.Grids;
using VSS.TRex.GridFabric.Models;
using VSS.TRex.GridFabric.Responses;
using VSS.TRex.IO.Helpers;
using VSS.TRex.SubGridTrees.Client.Interfaces;

namespace VSS.TRex.SubGrids.Executors
{
  /// <summary>
  /// The closure/function that implements sub grid request processing on compute nodes
  /// </summary>
  public class SubGridsRequestComputeFuncBase_Executor_Progressive<TSubGridsRequestArgument, TSubGridRequestsResponse> :
                  SubGridsRequestComputeFuncBase_Executor_Base<TSubGridsRequestArgument, TSubGridRequestsResponse>
    where TSubGridsRequestArgument : SubGridsRequestArgument
    where TSubGridRequestsResponse : SubGridRequestsResponse, new()
  {
    private static readonly ILogger Log = Logging.Logger.CreateLogger<SubGridsRequestComputeFuncBase_Executor_Progressive<TSubGridsRequestArgument, TSubGridRequestsResponse>>();

    private IMessaging rmtMsg;
    private string tRexNodeIDAsString = string.Empty;

    /// <summary>
    /// Processes a sub grid result that consists of a client leaf sub grid for each of the filters in the request
    /// </summary>
    /// <param name="results"></param>
    /// <param name="resultCount"></param>

    protected override void ProcessSubGridRequestResult(IClientLeafSubGrid[][] results, int resultCount)
    {
      // Package the resulting sub grids into the MemoryStream
      using (var MS = RecyclableMemoryStreamManagerHelper.Manager.GetStream())
      {
        using (var writer = new BinaryWriter(MS, Encoding.UTF8, true))
        {
          writer.Write(resultCount);

          for (int i = 0; i < resultCount; i++)
          {
            writer.Write(results[i].Length);
            foreach (IClientLeafSubGrid result in results[i])
            {
              writer.Write(result != null);
              result?.Write(writer);
            }
          }
        }

        // ... and send it to the message topic in the compute func
        // Log.InfoFormat("Sending result to {0} ({1} receivers) - First = {2}/{3}", 
        //                localArg.MessageTopic, rmtMsg.ClusterGroup.GetNodes().Count, 
        //                rmtMsg.ClusterGroup.GetNodes().Where(x => x.GetAttributes().Where(a => a.Key.StartsWith(ServerRoles.ROLE_ATTRIBUTE_NAME)).Count() > 0).Aggregate("|", (s1, s2) => s1 + s2 + "|"),
        //                rmtMsg.ClusterGroup.GetNodes().First().GetAttribute<string>("TRexNodeId"));
        rmtMsg.Send(new SerialisedByteArrayWrapper(MS.ToArray()), localArg.MessageTopic);
      }
    }

    /// <summary>
    /// Transforms the internal aggregation state into the desired response for the request
    /// </summary>
    /// <returns></returns>
    protected override TSubGridRequestsResponse AcquireComputationResult()
    {
      return new TSubGridRequestsResponse();
    }

    /// <summary>
    /// Set up Ignite elements for progressive sub grid requests
    /// </summary>
    protected override bool EstablishRequiredIgniteContext(out SubGridRequestsResponseResult contextEstablishmentResponse)
    {
      contextEstablishmentResponse = SubGridRequestsResponseResult.OK;

      IIgnite Ignite = DIContext.Obtain<ITRexGridFactory>().Grid(TRexGrids.ImmutableGridName());
      IClusterGroup group = Ignite?.GetCluster().ForAttribute("TRexNodeId", tRexNodeIDAsString);

      if (group == null)
      {
        contextEstablishmentResponse = SubGridRequestsResponseResult.NoIgniteGroupProjection;
        return false;
      }

      Log.LogInformation($"Message group has {group.GetNodes().Count} members");

      rmtMsg = group.GetMessaging();

      if (rmtMsg == null)
      {
        contextEstablishmentResponse = SubGridRequestsResponseResult.NoIgniteGroupProjection;
        return false;
      }

      return true;
    }

    /// <summary>
    /// Capture elements from the argument relevant to progressive sub grid requests
    /// </summary>
    /// <param name="arg"></param>
    public override void UnpackArgument(SubGridsRequestArgument arg)
    {
      base.UnpackArgument(arg);

      tRexNodeIDAsString = arg.TRexNodeID;

      Log.LogInformation($"TRexNodeIDAsString is {tRexNodeIDAsString} in UnpackArgument()");
    }
  }
}
