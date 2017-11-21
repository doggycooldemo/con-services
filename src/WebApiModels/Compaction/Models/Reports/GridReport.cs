﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using ASNodeRaptorReports;
using Newtonsoft.Json;
using VSS.Productivity3D.WebApi.Models.Common;

namespace VSS.Productivity3D.WebApi.Models.Compaction.Models.Reports
{
  /// <summary>
  /// Defines all the grid report production data values that are returned from Raptor.
  /// </summary>
  /// 
  public class GridReport
  {
    /// <summary>
    /// The report's 'start' time from a time based filter.
    /// </summary>
    [JsonProperty(Required = Required.Default)]
    public DateTime? StartTime { get; private set; }

    /// <summary>
    /// The report's 'end' time from a time based filter.
    /// </summary>
    [JsonProperty(Required = Required.Default)]
    public DateTime? EndTime { get; private set; }

    /// <summary>
    /// Grid report rows
    /// </summary>
    /// 
    [JsonProperty(Required = Required.Default)]
    public GridRow[] Rows { get; private set; }

    /// <summary>
    /// Creates an instance of the GridReport class.
    /// </summary>
    /// <param name="rows">Grid rows.</param>
    /// <param name="startTime">The report's 'start' time.</param>
    /// <param name="endTime">The report's 'end' time.</param>
    /// <returns>An instance of the GridReport class.</returns>
    /// 
    public static GridReport CreateGridReport(DateTime startTime, DateTime endTime, GridRow[] rows)
    {
      return new GridReport() { Rows = rows };
    }

    /// <summary>
    /// Serialises an instance of the GridReport class to a JSON string.
    /// </summary>
    /// <returns>A JSON representation of the GridReport class instance.</returns>
    /// 
    public string ToJsonString()
    {
      return JsonConvert.SerializeObject(this);
    }
  }
  

  /// <summary>
  /// Defines a grid report row.
  /// </summary>
  /// 
  public class GridRow
  {
    /// <summary>
    /// Northing value
    /// </summary>
    /// 
    [JsonProperty(Required = Required.Default)]
    public double Northing { get; private set; }

    /// <summary>
    /// Easting value
    /// </summary>
    /// 
    [JsonProperty(Required = Required.Default)]
    public double Easting { get; private set; }

    /// <summary>
    /// Elevation value
    /// </summary>
    /// 
    [JsonProperty(Required = Required.Default)]
    public double Elevation { get; private set; }

    /// <summary>
    /// CutFill value
    /// </summary>
    /// 
    [JsonProperty(Required = Required.Default)]
    public double CutFill { get; private set; }

    /// <summary>
    /// CMV value
    /// </summary>
    /// 
    [JsonProperty(Required = Required.Default)]
    public short CMV { get; private set; }

    /// <summary>
    /// MDP value
    /// </summary>
    /// 
    [JsonProperty(Required = Required.Default)]
    public short MDP { get; private set; }

    /// <summary>
    /// Pass Count value
    /// </summary>
    /// 
    [JsonProperty(Required = Required.Default)]
    public short PassCount { get; private set; }

    /// <summary>
    /// Temperature value
    /// </summary>
    /// 
    [JsonProperty(Required = Required.Default)]
    public short Temperature { get; private set; }

    /// <summary>
    /// Indicates whether Elevation values are included in the report.
    /// </summary>
    ///
    [JsonIgnore]
    [JsonProperty( Required = Required.Default)]
    public bool ElevationReport { get; private set; }

    /// <summary>
    /// Indicates whether Cut/Fill values are included in the report.
    /// </summary>
    /// 
    [JsonIgnore]
    [JsonProperty(Required = Required.Default)]
    public bool CutFillReport { get; private set; }

    /// <summary>
    /// Indicates whether CMV values are included in the report.
    /// </summary>
    /// 
    [JsonIgnore]
    [JsonProperty(Required = Required.Default)]
    public bool CMVReport { get; private set; }

    /// <summary>
    /// Indicates whether MDP values are included in the report.
    /// </summary>
    /// 
    [JsonIgnore]
    [JsonProperty( Required = Required.Default)]
    public bool MDPReport { get; private set; }

    /// <summary>
    /// Indicates wehther Pass Count values are included in the report.
    /// </summary>
    /// 
    [JsonIgnore]
    [JsonProperty(Required = Required.Default)]
    public bool PassCountReport { get; private set; }

    /// <summary>
    /// Indicates whether Temperature values are included in the report.
    /// </summary>
    /// 
    [JsonIgnore]
    [JsonProperty(Required = Required.Default)]
    public bool TemperatureReport { get; private set; }

    #region For JSON Serialization
    public bool ShouldSerializeElevation()
    {
      return ElevationReport;
    }
    public bool ShouldSerializeCutFill()
    {
      return CutFillReport;
    }
    public bool ShouldSerializeCMV()
    {
      return CMVReport && CMV != VelociraptorConstants.NO_CCV;
    }
    public bool ShouldSerializeMDP()
    {
      return MDPReport && MDP != VelociraptorConstants.NO_MDP;
    }
    public bool ShouldSerializePassCount()
    {
      return PassCountReport;
    }
    public bool ShouldSerializeTemperature()
    {
      return TemperatureReport && Temperature != VelociraptorConstants.NO_TEMPERATURE;
    }
    #endregion

    /// <summary>
    /// Sets flags that indicates which of the reported values present in the report.
    /// </summary> 
    /// <param name="reportElevation"></param>
    /// <param name="reportCutFill"></param>
    /// <param name="reportCMV"></param>
    /// <param name="reportMDP"></param>
    /// <param name="reportPassCount"></param>
    /// <param name="reportTemperature"></param>
    /// 
    public void SetReportFlags(
      bool reportElevation,
      bool reportCutFill,
      bool reportCMV,
      bool reportMDP,
      bool reportPassCount,
      bool reportTemperature
    )
    {
      ElevationReport = reportElevation;
      CutFillReport = reportCutFill;
      CMVReport = reportCMV;
      MDPReport = reportMDP;
      PassCountReport = reportPassCount;
      TemperatureReport = reportTemperature;
    }

    /// <summary>
    /// Create an instance of the GridRoW class.
    /// </summary>
    /// <param name="northing"></param>
    /// <param name="easting"></param>
    /// <param name="elevation"></param>
    /// <param name="cutFill"></param>
    /// <param name="cmv"></param>
    /// <param name="mdp"></param>
    /// <param name="passCount"></param>
    /// <param name="temperature"></param>
    /// <returns>An instance of the GridRow class.</returns>
    /// 
    public static GridRow CreateGridRow(
      double northing,
      double easting,
      double elevation,
      double cutFill,
      short cmv,
      short mdp,
      short passCount,
      short temperature)
    {
      return new GridRow()
      {
        Northing = northing,
        Easting = easting,
        Elevation = elevation,
        CutFill = cutFill,
        CMV = cmv,
        MDP = mdp,
        PassCount = passCount,
        Temperature = temperature
      };
    }
  }
}
