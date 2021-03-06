﻿using System;
using Apache.Ignite.Core.Compute;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx.Synchronous;
using VSS.TRex.Designs.TTM;
using VSS.TRex.Exports.Surfaces.Executors;
using VSS.TRex.GridFabric.ComputeFuncs;
using VSS.TRex.IO.Helpers;
using VSS.TRex.Servers;
using VSS.TRex.Storage.Models;

namespace VSS.TRex.Exports.Surfaces.GridFabric
{
  /// <summary>
  /// The grid compute function responsible for coordinating sub grids comprising an exported TIN surface in response to
  /// a client server instance requesting it.
  /// </summary>
  public class TINSurfaceRequestComputeFunc : BaseComputeFunc, IComputeFunc<TINSurfaceRequestArgument, TINSurfaceResult>
  {
    private static readonly ILogger _log = Logging.Logger.CreateLogger<TINSurfaceRequestComputeFunc>();

    /// <summary>
    /// Default no-arg constructor that orients the request to the available servers on the immutable grid projection
    /// </summary>
    public TINSurfaceRequestComputeFunc()
    {
    }

    public TINSurfaceResult Invoke(TINSurfaceRequestArgument arg)
    {
      _log.LogInformation("In TINSurfaceRequestComputeFunc.Invoke()");

      try
      {
        // Export requests can be a significant resource commitment. Ensure TPaaS will be listening...
        PerformTPaaSRequestLivelinessCheck(arg);

        // Supply the TRex ID of the Ignite node currently running this code to permit processing contexts to send
        // sub grid results to it.
        arg.TRexNodeID = TRexNodeID.ThisNodeID(StorageMutability.Immutable);

        _log.LogInformation($"Assigned TRexNodeId from local node is {arg.TRexNodeID}");

        var request = new TINSurfaceExportExecutor(arg.ProjectID, arg.Filters, arg.Tolerance, arg.TRexNodeID, arg.LiftParams);

        _log.LogInformation("Executing request.ExecuteAsync()");

        if (!request.ExecuteAsync().WaitAndUnwrapException())
          _log.LogError("Request execution failed");

        var result = new TINSurfaceResult();
        using var ms = RecyclableMemoryStreamManagerHelper.Manager.GetStream();
        if (request.SurfaceSubGridsResponse.TIN != null)
        {
          request.SurfaceSubGridsResponse.TIN.SaveToStream(Consts.DefaultCoordinateResolution, Consts.DefaultElevationResolution, false, ms);
          result.data = ms.ToArray();
        }

        return result;
      }
      catch (Exception e)
      {
        _log.LogError(e, "Exception requesting a TTM surface");
        return new TINSurfaceResult { ResultStatus = Types.RequestErrorStatus.Exception };
      }
      finally
      {
        _log.LogInformation("Exiting TINSurfaceRequestComputeFunc.Invoke()");
      }
    }
  }
}
