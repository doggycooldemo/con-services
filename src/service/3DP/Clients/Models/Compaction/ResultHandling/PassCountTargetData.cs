﻿using Newtonsoft.Json;

namespace VSS.Productivity3D.Productivity3D.Models.Compaction.ResultHandling
{
  /// <summary>
  /// Pass count target data returned by PassCounts request.
  /// </summary>
  public class PassCountTargetData
  {
    /// <summary>
    /// If the pass count value is constant, this is the minimum constant value of all pass count targets in the processed data.
    /// </summary>
    [JsonProperty(PropertyName = "minPassCountMachineTarget")]
    public double MinPassCountMachineTarget { get; set; }
    /// <summary>
    /// If the pass count value is constant, this is the maximum constant value of all pass count targets in the processed data.
    /// </summary>
    [JsonProperty(PropertyName = "maxPassCountMachineTarget")]
    public double MaxPassCountMachineTarget { get; set; }
    /// <summary>
    /// Are the pass count target values applying to all processed cells varying?
    /// </summary>
    [JsonProperty(PropertyName = "targetVaries")]
    public bool TargetVaries { get; set; }
  }
}
