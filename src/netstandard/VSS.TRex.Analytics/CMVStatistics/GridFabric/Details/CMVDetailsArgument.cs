﻿using System;
using VSS.TRex.GridFabric.Models.Arguments;

namespace VSS.TRex.Analytics.CMVStatistics.GridFabric.Details
{
  /// <summary>
  /// Argument containing the parameters required for a CMV details request
  /// </summary>    
  [Serializable]
  public class CMVDetailsArgument : BaseApplicationServiceRequestArgument
  {
    /// <summary>
    /// CMV details values.
    /// </summary>
    public int[] CMVDetailValues { get; set; }
  }
}
