﻿using System.IO;
using System.Text;
using System.Threading.Tasks;
using VSS.TRex.GridFabric;
using VSS.TRex.GridFabric.Arguments;
using VSS.TRex.GridFabric.Models;
using VSS.TRex.GridFabric.Responses;
using VSS.TRex.IO.Helpers;
using VSS.TRex.SubGrids.GridFabric.Arguments;
using VSS.TRex.SubGrids.GridFabric.Requests;
using VSS.TRex.SubGrids.Interfaces;
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
   // private static readonly ILogger Log = Logging.Logger.CreateLogger<SubGridsRequestComputeFuncBase_Executor_Progressive<TSubGridsRequestArgument, TSubGridRequestsResponse>>();

    private ISubGridProgressiveResponseRequest _responseRequest;

    /// <summary>
    /// Processes a sub grid result that consists of a client leaf sub grid for each of the filters in the request
    /// </summary>
    /// <param name="results"></param>
    /// <param name="resultCount"></param>

    protected override Task<bool> ProcessSubGridRequestResult(IClientLeafSubGrid[][] results, int resultCount)
    {
      // Package the resulting sub grids into the MemoryStream
      using (var ms = RecyclableMemoryStreamManagerHelper.Manager.GetStream())
      {
        using (var writer = new BinaryWriter(ms, Encoding.UTF8, true))
        {
          writer.Write(resultCount);

          for (var i = 0; i < resultCount; i++)
          {
            writer.Write(results[i].Length);
            foreach (var result in results[i])
            {
              writer.Write(result != null);
              result?.Write(writer);
            }
          }
        }

        // ... and send it to the request
        return _responseRequest.ExecuteAsync(new SubGridProgressiveResponseRequestComputeFuncArgument
        {
          NodeId = localArg.OriginatingIgniteNodeId, 
          RequestDescriptor = localArg.RequestID, 
          ExternalDescriptor = localArg.ExternalDescriptor, 
          Payload = new SerialisedByteArrayWrapper(ms.ToArray())
        });
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

      _responseRequest = new SubGridProgressiveResponseRequest(localArg.OriginatingIgniteNodeId);

      return true;
    }
  }
}
