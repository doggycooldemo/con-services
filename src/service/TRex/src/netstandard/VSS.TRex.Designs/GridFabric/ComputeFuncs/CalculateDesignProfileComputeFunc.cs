﻿using Apache.Ignite.Core.Compute;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using VSS.TRex.Designs.Executors;
using VSS.TRex.Designs.GridFabric.Arguments;
using VSS.TRex.Designs.GridFabric.Responses;
using VSS.TRex.GridFabric.ComputeFuncs;

namespace VSS.TRex.Designs.GridFabric.ComputeFuncs
{
  /// <summary>
  /// Ignite ComputeFunc responsible for executing the design profile calculator
  /// </summary>
  public class CalculateDesignProfileComputeFunc : BaseComputeFunc, IComputeFunc<CalculateDesignProfileArgument, CalculateDesignProfileResponse>
  {
    private static readonly ILogger Log = Logging.Logger.CreateLogger(MethodBase.GetCurrentMethod().DeclaringType?.Name);

    public CalculateDesignProfileResponse Invoke(CalculateDesignProfileArgument arg)
    {
      var startDate = DateTime.UtcNow;
      try
      {
        Log.LogInformation($"In: {nameof(CalculateDesignProfileComputeFunc)}: Arg = {arg}");

        CalculateDesignProfile Executor = new CalculateDesignProfile();

        var profile = Executor.Execute(arg, out var calcResult);

        var result = new CalculateDesignProfileResponse
        {
          Profile = profile,
          RequestResult = calcResult
        };

        Log.LogInformation($"Profile result: {result.Profile?.Count ?? -1} vertices");
        return result;
      }
      catch (Exception E)
      {
        Log.LogError(E, "Exception:");
        return null;
      }
      finally
      {
        Log.LogInformation($"Out: CalculateDesignProfileComputeFunc in {DateTime.UtcNow - startDate}, Arg = {arg}");
      }
    }
  }
}
