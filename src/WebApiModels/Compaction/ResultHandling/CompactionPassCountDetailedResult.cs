﻿using Newtonsoft.Json;
using System;
using System.Linq;
using VSS.MasterData.Models.ResultHandling.Abstractions;
using VSS.Productivity3D.Common.ResultHandling;
using VSS.Productivity3D.WebApi.Models.Report.ResultHandling;

namespace VSS.Productivity3D.WebApi.Models.Compaction.ResultHandling
{
  /// <summary>
  /// Represents result returned by Pass Count Details request for compaction.
  /// </summary>
  public class CompactionPassCountDetailedResult : ContractExecutionResult
  {
    [JsonProperty(PropertyName = "passCountDetailsData")]
    public PassCountDetailsData DetailedData { get; private set; }

    /// <summary>
    /// Private constructor
    /// </summary>
    private CompactionPassCountDetailedResult()
    { }

    /// <summary>
    /// Creates an instance of the CompactionPassCountDetailedResult class.
    /// </summary>
    /// <param name="result">An instance of the PassCountDetailedResult class.</param>
    /// <returns>An instance of the CompactionPassCountDetailedResult class.</returns>
    public static CompactionPassCountDetailedResult CreatePassCountDetailedResult(PassCountDetailedResult result)
    {
      const int noPassCountData = 0;


      //TODO remove this due to bug in Raptor
      /*if (Math.Abs(result.TotalCoverageArea - noPassCountData) < 0.001)
      {
        return new CompactionPassCountDetailedResult { DetailedData = new PassCountDetailsData { PassCountTarget = new PassCountTargetData() } };
      }*/

      return new CompactionPassCountDetailedResult
      {
        DetailedData = new PassCountDetailsData
        {
          Percents = result.percents.Skip(1).ToArray(), //don't return the pass count 0 value (see PassCountSettings)
          PassCountTarget = new PassCountTargetData
          {
            MinPassCountMachineTarget = result.constantTargetPassCountRange.min,
            MaxPassCountMachineTarget = result.constantTargetPassCountRange.max,
            TargetVaries = !result.isTargetPassCountConstant
          },
          TotalCoverageArea = result.TotalCoverageArea
        }
      };
    }

    /// <summary>
    /// Pass Count details data returned.
    /// </summary>
    public class PassCountDetailsData
    {
      /// <summary>
      /// Collection of passcount percentages where each element represents the percentage of the matching index passcount number provided in the 
      /// passCounts member of the pass count request representation.
      /// </summary>
      [JsonProperty(PropertyName = "percents")]
      public double[] Percents { get; set; }
      /// <summary>
      /// Gets the total coverage area for the production data - not the total area specified in filter
      /// </summary>
      /// <value>
      /// The total coverage area in sq meters.
      /// </value>
      [JsonProperty(PropertyName = "totalCoverageArea")]
      public double TotalCoverageArea { get; set; }
      /// <summary>
      /// Pass count machine target and whether it is constant or varies.
      /// </summary>
      [JsonProperty(PropertyName = "passCountTarget")]
      public PassCountTargetData PassCountTarget { get; set; }
    }
  }
}