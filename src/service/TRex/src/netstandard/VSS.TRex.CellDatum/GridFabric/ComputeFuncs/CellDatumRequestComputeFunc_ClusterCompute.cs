﻿using System;
using Apache.Ignite.Core.Binary;
using Apache.Ignite.Core.Compute;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx.Synchronous;
using VSS.Productivity3D.Models.ResultHandling;
using VSS.TRex.CellDatum.Executors;
using VSS.TRex.CellDatum.GridFabric.Arguments;
using VSS.TRex.CellDatum.GridFabric.Responses;
using VSS.TRex.Common;
using VSS.TRex.GridFabric.Affinity;
using VSS.TRex.GridFabric.ComputeFuncs;
using VSS.TRex.GridFabric.Interfaces;

namespace VSS.TRex.CellDatum.GridFabric.ComputeFuncs
{
  /// <summary>
  /// This compute func operates in the context of an application server that reaches out to the compute cluster to 
  /// perform sub grid processing.
  /// </summary>
  public class CellDatumRequestComputeFunc_ClusterCompute : BaseComputeFunc, IComputeFuncArgument<CellDatumRequestArgument_ClusterCompute>, IComputeFunc<CellDatumResponse_ClusterCompute>
  {
    private const byte VERSION_NUMBER = 1;

    private static readonly ILogger _log = Logging.Logger.CreateLogger<CellDatumRequestComputeFunc_ClusterCompute>();

    public CellDatumRequestArgument_ClusterCompute Argument { get; set; }

    public CellDatumResponse_ClusterCompute Invoke()
    {
      _log.LogInformation("In CellDatumRequestComputeFunc_ClusterCompute.Invoke()");

      try
      {
        // Ensure TPaaS will be listening as this is usually supporting user UX centric mouse hover requests
        PerformTPaaSRequestLivelinessCheck(Argument);

        var request = new CellDatumComputeFuncExecutor_ClusterCompute();

        _log.LogInformation("Executing CellDatumRequestComputeFunc_ClusterCompute.ExecuteAsync()");

        if (Argument == null)
          throw new ArgumentException("Argument for ComputeFunc must be provided");

        return request.ExecuteAsync
        (
          Argument,
          new SubGridSpatialAffinityKey(SubGridSpatialAffinityKey.DEFAULT_SPATIAL_AFFINITY_VERSION_NUMBER_TICKS, Argument.ProjectID, Argument.OTGCellX, Argument.OTGCellY)
        ).WaitAndUnwrapException();
      }
      catch (Exception e)
      {
        _log.LogError(e, "Exception computing cell datum response on cluster");
        return new CellDatumResponse_ClusterCompute { ReturnCode = CellDatumReturnCode.UnexpectedError, TimeStampUTC = DateTime.MinValue, Value = 0.0};
      }
      finally
      {
        _log.LogInformation("Exiting CellDatumRequestComputeFunc_ClusterCompute.Invoke()");
      }
    }

    public override void InternalToBinary(IBinaryRawWriter writer)
    {
      base.InternalToBinary(writer);

      VersionSerializationHelper.EmitVersionByte(writer, VERSION_NUMBER);

      writer.WriteBoolean(Argument != null);
      Argument?.ToBinary(writer);
    }

    public override void InternalFromBinary(IBinaryRawReader reader)
    {
      base.InternalFromBinary(reader);

      var version = VersionSerializationHelper.CheckVersionByte(reader, VERSION_NUMBER);

      if (version == 1)
      {
        if (reader.ReadBoolean())
        {
          Argument = new CellDatumRequestArgument_ClusterCompute();
          Argument.FromBinary(reader);
        }
      }
    }
  }
}
