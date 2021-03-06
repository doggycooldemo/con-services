﻿using System;
using VSS.Productivity3D.Filter.Abstractions.Models;

namespace VSS.Productivity3D.Models.Models
{
  /// <summary>
  /// The request representation used to request MDP summary.
  /// </summary>
  public class TemperatureSummaryRequest : TRexBaseRequest
  {
    /// <summary>
    /// Default private constructor
    /// </summary>
    private TemperatureSummaryRequest()
    {
    }

    /// <summary>
    /// Overload constructor with parameters.
    /// </summary>
    public TemperatureSummaryRequest(
      Guid projectUid, 
      FilterResult filter, 
      TemperatureSettings temperatureSettings, 
      LiftSettings liftSettings)
    {
      ProjectUid = projectUid;
      Filter = filter;
      Overrides = new OverridingTargets(temperatureSettings: temperatureSettings);
      LiftSettings = liftSettings;
    }
  }
}
