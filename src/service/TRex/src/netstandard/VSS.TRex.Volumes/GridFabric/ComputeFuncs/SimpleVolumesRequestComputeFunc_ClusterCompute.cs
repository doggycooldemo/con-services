﻿using System;
using Apache.Ignite.Core.Compute;
using Microsoft.Extensions.Logging;
using VSS.TRex.GridFabric.ComputeFuncs;
using VSS.TRex.GridFabric.Models;
using VSS.TRex.Volumes.Executors;
using VSS.TRex.Volumes.GridFabric.Arguments;
using VSS.TRex.Volumes.GridFabric.Responses;

namespace VSS.TRex.Volumes.GridFabric.ComputeFuncs
{
  /// <summary>
  /// The simple volumes compute function that runs in the context of the cluster compute nodes. This function
  /// performs a volumes calculation across the partitions on this node only.
  /// </summary>
  public class SimpleVolumesRequestComputeFunc_ClusterCompute : BaseComputeFunc, IComputeFunc<SimpleVolumesRequestArgument, SimpleVolumesResponse>
  {
    private static readonly ILogger _log = Logging.Logger.CreateLogger<SimpleVolumesRequestComputeFunc_ClusterCompute>();

    /// <summary>
    /// Default no-arg constructor that orients the request to the available servers on the immutable grid projection
    /// </summary>
    public SimpleVolumesRequestComputeFunc_ClusterCompute()
    {
    }

    /// <summary>
    /// Invoke the simple volumes request locally on this node
    /// </summary>
    public SimpleVolumesResponse Invoke(SimpleVolumesRequestArgument arg)
    {
      _log.LogInformation("In SimpleVolumesRequestComputeFunc_ClusterCompute.Invoke()");

      try
      {
        using var simpleVolumes = new ComputeSimpleVolumes_Coordinator
            (arg.ProjectID,
             arg.LiftParams,
             arg.VolumeType,
             arg.BaseFilter,
             arg.TopFilter,
             arg.BaseDesign,
             arg.TopDesign,
             arg.AdditionalSpatialFilter,
             arg.CutTolerance,
             arg.FillTolerance);

        _log.LogInformation("Executing simpleVolumes.Execute()");

        return simpleVolumes.Execute();
      }
      catch (Exception e)
      {
        _log.LogError(e, "Exception requesting progressive volume at cluster compute layer");
        return new SimpleVolumesResponse { ResponseCode = SubGridRequestsResponseResult.Exception };
      }
      finally
      {
        _log.LogInformation("Exiting SimpleVolumesRequestComputeFunc_ClusterCompute.Invoke()");
      }
    }
  }
}
