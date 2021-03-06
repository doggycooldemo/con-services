﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VSS.Common.Exceptions;
using VSS.MasterData.Models.Handlers;
using VSS.MasterData.Models.Interfaces;
using VSS.MasterData.Models.ResultHandling.Abstractions;
using VSS.MasterData.Models.Utilities;
using VSS.Productivity3D.Project.Abstractions.Models.Interfaces;
using VSS.Productivity3D.Project.Abstractions.Models.Validation;

namespace VSS.Productivity3D.Project.Abstractions.Models.ResultsHandling
{
  /// <summary>
  /// 3D Productivity project settings. Used in Raptor calculations.
  /// </summary>
  public class CompactionProjectSettings : IValidatable, IDefaultSettings
  {
    /// <summary>
    /// Value to convert from km/h to cm/s
    /// </summary>
    private static readonly double KM_HR_TO_CM_SEC = 27.77777778; //1.0 / 3600 * 100000;

    #region Properties

    /// <summary>
    /// Flag to determine if machine target pass count or custom target range is used. Default is true.
    /// </summary>
    [JsonProperty(PropertyName = "useMachineTargetPassCount", Required = Required.Default)]
    public bool? useMachineTargetPassCount { get; private set; } = true;
    /// <summary>
    /// The minimum target pass count when overriding the machine target value
    /// </summary>
    [Range(MIN_PASS_COUNT, MAX_PASS_COUNT)]
    [JsonProperty(PropertyName = "customTargetPassCountMinimum", Required = Required.Default)]
    public int? customTargetPassCountMinimum { get; private set; }
    /// <summary>
    /// The maximum target pass count when overriding the machine target value
    /// </summary>
    [Range(MIN_PASS_COUNT, MAX_PASS_COUNT)]
    [JsonProperty(PropertyName = "customTargetPassCountMaximum", Required = Required.Default)]
    public int? customTargetPassCountMaximum { get; private set; }
    /// <summary>
    /// Flag to determine if machine target temperature or custom target range is used. Default is true.
    /// </summary>
    [JsonProperty(PropertyName = "useMachineTargetTemperature", Required = Required.Default)]
    public bool? useMachineTargetTemperature { get; private set; } = true;
    /// <summary>
    /// The minimum target temperature (°C) when overriding the machine target value
    /// </summary>
    [Range(MIN_TEMPERATURE, MAX_TEMPERATURE)]
    [JsonProperty(PropertyName = "customTargetTemperatureMinimum", Required = Required.Default)]
    public double? customTargetTemperatureMinimum { get; private set; }
    /// <summary>
    /// The maximum target temperature (°C) when overriding the machine target value
    /// </summary>
    [Range(MIN_TEMPERATURE, MAX_TEMPERATURE)]
    [JsonProperty(PropertyName = "customTargetTemperatureMaximum", Required = Required.Default)]
    public double? customTargetTemperatureMaximum { get; private set; }
    /// <summary>
    /// Flag to determine if machine target CMV or custom target value is used. Default is true.
    /// </summary>
    [JsonProperty(PropertyName = "useMachineTargetCmv", Required = Required.Default)]
    public bool? useMachineTargetCmv { get; private set; } = true;
    /// <summary>
    /// The target CMV value when overriding the machine target value
    /// </summary>
    [Range(MIN_RAW_CMV, MAX_RAW_CMV)]
    [JsonProperty(PropertyName = "customTargetCmv", Required = Required.Default)]
    public double? customTargetCmv { get; private set; }
    /// <summary>
    /// Flag to determine if machine target MDP or custom target value is used. Default is true.
    /// </summary>
    [JsonProperty(PropertyName = "useMachineTargetMdp", Required = Required.Default)]
    public bool? useMachineTargetMdp { get; private set; } = true;
    /// <summary>
    /// The target MDP value when overriding the machine target value
    /// </summary>
    [Range(MIN_RAW_MDP, MAX_RAW_MDP)]
    [JsonProperty(PropertyName = "customTargetMdp", Required = Required.Default)]
    public double? customTargetMdp { get; private set; }
    /// <summary>
    /// Flag to determine if the default CMV % range or custom target range is used. Default is true.
    /// </summary>
    [JsonProperty(PropertyName = "useDefaultTargetRangeCmvPercent", Required = Required.Default)]
    public bool? useDefaultTargetRangeCmvPercent { get; private set; } = true;
    /// <summary>
    /// The minimum target CMV % when overriding the default target range
    /// </summary>
    [Range(MIN_CMV_PERCENT, MAX_CMV_PERCENT)]
    [JsonProperty(PropertyName = "customTargetCmvPercentMinimum", Required = Required.Default)]
    public double? customTargetCmvPercentMinimum { get; private set; }
    /// <summary>
    /// The maximum target CMV % when overriding the default target range
    /// </summary>
    [Range(MIN_CMV_PERCENT, MAX_CMV_PERCENT)]
    [JsonProperty(PropertyName = "customTargetCmvPercentMaximum", Required = Required.Default)]
    public double? customTargetCmvPercentMaximum { get; private set; }
    /// <summary>
    /// Flag to determine if the default MDP % range or custom target range is used. Default is true.
    /// </summary>
    [JsonProperty(PropertyName = "useDefaultTargetRangeMdpPercent", Required = Required.Default)]
    public bool? useDefaultTargetRangeMdpPercent { get; private set; } = true;
    /// <summary>
    /// The minimum target MDP % when overriding the default target range
    /// </summary>
    [Range(MIN_MDP_PERCENT, MAX_MDP_PERCENT)]
    [JsonProperty(PropertyName = "customTargetMdpPercentMinimum", Required = Required.Default)]
    public double? customTargetMdpPercentMinimum { get; private set; }
    /// <summary>
    /// The maximum target MDP % when overriding the default target range
    /// </summary>
    [Range(MIN_MDP_PERCENT, MAX_MDP_PERCENT)]
    [JsonProperty(PropertyName = "customTargetMdpPercentMaximum", Required = Required.Default)]
    public double? customTargetMdpPercentMaximum { get; private set; }
    /// <summary>
    /// Flag to determine if the default speed range or custom target range is used. Default is true.
    /// </summary>
    [JsonProperty(PropertyName = "useDefaultTargetRangeSpeed", Required = Required.Default)]
    public bool? useDefaultTargetRangeSpeed { get; private set; } = true;
    /// <summary>
    /// The minimum target speed (km/h)  when overriding the default target range
    /// </summary>
    [Range(MIN_SPEED, MAX_SPEED)]
    [JsonProperty(PropertyName = "customTargetSpeedMinimum", Required = Required.Default)]
    public double? customTargetSpeedMinimum { get; private set; }
    /// <summary>
    /// The maximum target speed (km/h) when overriding the default target range
    /// </summary>
    [Range(MIN_SPEED, MAX_SPEED)]
    [JsonProperty(PropertyName = "customTargetSpeedMaximum", Required = Required.Default)]
    public double? customTargetSpeedMaximum { get; private set; }
    /// <summary>
    /// Flag to determine if default cut-fill tolerances or custom tolerances are used. Default is true.
    /// </summary>
    [JsonProperty(PropertyName = "useDefaultCutFillTolerances", Required = Required.Default)]
    public bool? useDefaultCutFillTolerances { get; private set; } = true;
    /// <summary>
    /// The collection of custom cut-fill tolerances (m) when overriding the defaults. Values are in descending order, highest cut to lowest fill.
    /// There must be 7 values (3 cut, on grade value = 0, and 3 fill).
    /// </summary>
    [JsonProperty(PropertyName = "customCutFillTolerances", Required = Required.Default)]
    public List<double> customCutFillTolerances { get; private set; }
    /// <summary>
    /// Flag to determine if default shrinkage % and bulking % or custom values are used. Default is true.
    /// </summary>
    [JsonProperty(PropertyName = "useDefaultVolumeShrinkageBulking", Required = Required.Default)]
    public bool? useDefaultVolumeShrinkageBulking { get; private set; } = true;
    /// <summary>
    /// The shrinkage % when overriding the default value.
    /// </summary>
    [Range(MIN_SHRINKAGE, MAX_SHRINKAGE)]
    [JsonProperty(PropertyName = "customShrinkagePercent", Required = Required.Default)]
    public double? customShrinkagePercent { get; private set; }
    /// <summary>
    /// The bulking % when overriding the default value.
    /// </summary>
    [Range(MIN_BULKING, MAX_BULKING)]
    [JsonProperty(PropertyName = "customBulkingPercent", Required = Required.Default)]
    public double? customBulkingPercent { get; private set; }
    /// <summary>
    /// Flag to determine if machine default pass count details settings or custom settings are used. Default is true.
    /// </summary>
    [JsonProperty(PropertyName = "useDefaultPassCountTargets", Required = Required.Default)]
    public bool? useDefaultPassCountTargets { get; private set; } = true;
    /// <summary>
    /// The collection of pass count targets when overriding the defaults. Values are in ascending order.
    /// There must be 8 values and the first value must be 1.
    /// </summary>
    [JsonProperty(PropertyName = "customPassCountTargets", Required = Required.Default)]
    public List<int> customPassCountTargets { get; private set; }
    /// <summary>
    /// Flag to determine if machine default CMV details settings or custom settings are used. Default is true.
    /// </summary>
    [JsonProperty(PropertyName = "useDefaultCMVTargets", Required = Required.Default)]
    public bool? useDefaultCMVTargets { get; private set; } = true;
    /// <summary>
    /// The collection of CMV targets when overriding the defaults. Values are in ascending order.
    /// There must be 5 values and the first value must be 0.
    /// </summary>
    [JsonProperty(PropertyName = "customCMVTargets", Required = Required.Default)]
    public List<int> customCMVTargets { get; private set; }
    /// <summary>
    /// Flag to determine if machine default temperature details settings or custom settings are used. Default is true.
    /// </summary>
    [JsonProperty(PropertyName = "useDefaultTemperatureTargets", Required = Required.Default)]
    public bool? useDefaultTemperatureTargets { get; private set; } = true;
    /// <summary>
    /// The collection of temperature targets when overriding the defaults. Values are in ascending order.
    /// There must be 5 values and the first value must be 0.
    /// </summary>
    [JsonProperty(PropertyName = "customTemperatureTargets", Required = Required.Default)]
    public List<double> customTemperatureTargets { get; private set; }
    #endregion

    #region Construction
    /// <summary>
    /// Default constructor
    /// </summary>
    public CompactionProjectSettings()
    { }

    /// <summary>
    /// Constructor with parameters.
    /// </summary>
    public CompactionProjectSettings(
      bool? useMachineTargetPassCount = null,
      int? customTargetPassCountMinimum = null,
      int? customTargetPassCountMaximum = null,
      bool? useMachineTargetTemperature = null,
      double? customTargetTemperatureMinimum = null,
      double? customTargetTemperatureMaximum = null,
      bool? useMachineTargetCmv = null,
      double? customTargetCmv = null,
      bool? useMachineTargetMdp = null,
      double? customTargetMdp = null,
      bool? useDefaultTargetRangeCmvPercent = null,
      double? customTargetCmvPercentMinimum = null,
      double? customTargetCmvPercentMaximum = null,
      bool? useDefaultTargetRangeMdpPercent = null,
      double? customTargetMdpPercentMinimum = null,
      double? customTargetMdpPercentMaximum = null,
      bool? useDefaultTargetRangeSpeed = null,
      double? customTargetSpeedMinimum = null,
      double? customTargetSpeedMaximum = null,
      bool? useDefaultCutFillTolerances = null,
      List<double> customCutFillTolerances = null,
      bool? useDefaultVolumeShrinkageBulking = null,
      double? customShrinkagePercent = null,
      double? customBulkingPercent = null,
      bool? useDefaultPassCountTargets = null,
      List<int> customPassCountTargets = null,
      bool? useDefaultCMVTargets = null,
      List<int> customCMVTargets = null,
      bool? useDefaultTemperatureTargets = null,
      List<double> customTemperatureTargets = null)
    {
      this.useMachineTargetPassCount = useMachineTargetPassCount;
      this.customTargetPassCountMinimum = customTargetPassCountMinimum;
      this.customTargetPassCountMaximum = customTargetPassCountMaximum;
      this.useMachineTargetTemperature = useMachineTargetTemperature;
      this.customTargetTemperatureMinimum = customTargetTemperatureMinimum;
      this.customTargetTemperatureMaximum = customTargetTemperatureMaximum;
      this.useMachineTargetCmv = useMachineTargetCmv;
      this.customTargetCmv = customTargetCmv;
      this.useMachineTargetMdp = useMachineTargetMdp;
      this.customTargetMdp = customTargetMdp;
      this.useDefaultTargetRangeCmvPercent = useDefaultTargetRangeCmvPercent;
      this.customTargetCmvPercentMinimum = customTargetCmvPercentMinimum;
      this.customTargetCmvPercentMaximum = customTargetCmvPercentMaximum;
      this.useDefaultTargetRangeMdpPercent = useDefaultTargetRangeMdpPercent;
      this.customTargetMdpPercentMinimum = customTargetMdpPercentMinimum;
      this.customTargetMdpPercentMaximum = customTargetMdpPercentMaximum;
      this.useDefaultTargetRangeSpeed = useDefaultTargetRangeSpeed;
      this.customTargetSpeedMinimum = customTargetSpeedMinimum;
      this.customTargetSpeedMaximum = customTargetSpeedMaximum;
      this.useDefaultCutFillTolerances = useDefaultCutFillTolerances;
      this.customCutFillTolerances = customCutFillTolerances;
      this.useDefaultVolumeShrinkageBulking = useDefaultVolumeShrinkageBulking;
      this.customShrinkagePercent = customShrinkagePercent;
      this.customBulkingPercent = customBulkingPercent;
      this.useDefaultPassCountTargets = useDefaultPassCountTargets;
      this.customPassCountTargets = customPassCountTargets;
      this.useDefaultCMVTargets = useDefaultCMVTargets;
      this.customCMVTargets = customCMVTargets;
      this.useDefaultTemperatureTargets = useDefaultTemperatureTargets;
      this.customTemperatureTargets = customTemperatureTargets;
    }

    public static CompactionProjectSettings DefaultSettings =>
        new CompactionProjectSettings
        (
          useMachineTargetPassCount: true,
          customTargetPassCountMinimum: 6,
          customTargetPassCountMaximum: 6,
          useMachineTargetTemperature: true,
          customTargetTemperatureMinimum: 65.0,
          customTargetTemperatureMaximum: 175.0,
          useMachineTargetCmv: true,
          customTargetCmv: 70,
          useMachineTargetMdp: true,
          customTargetMdp: 70,useDefaultTargetRangeCmvPercent: true,
          customTargetCmvPercentMinimum: 80.0,
          customTargetCmvPercentMaximum: 130.0,
          useDefaultTargetRangeMdpPercent: true,
          customTargetMdpPercentMinimum: 80.0,
          customTargetMdpPercentMaximum: 130.0,
          useDefaultTargetRangeSpeed: true,
          customTargetSpeedMinimum: 5.0,
          customTargetSpeedMaximum: 10.0,
          useDefaultCutFillTolerances: true,
          customCutFillTolerances: new List<double> { 0.2, 0.1, 0.05, 0, -0.05, -0.1, -0.2 },
          useDefaultVolumeShrinkageBulking: true,
          customShrinkagePercent: 0.0,
          customBulkingPercent: 0.0,
          useDefaultPassCountTargets: true,
          customPassCountTargets: new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 },
          useDefaultCMVTargets: true,
          customCMVTargets: new List<int> { 0, 40, 80, 120, 150 },
          useDefaultTemperatureTargets: true,
          customTemperatureTargets: new List<double> { 0, 50, 100, 150, 200, 250, 300 }
        );

    public void Defaults()
    {
      this.useMachineTargetPassCount = DefaultSettings.useMachineTargetPassCount;
      this.customTargetPassCountMinimum = DefaultSettings.customTargetPassCountMinimum;
      this.customTargetPassCountMaximum = DefaultSettings.customTargetPassCountMaximum;
      this.useMachineTargetTemperature = DefaultSettings.useMachineTargetTemperature;
      this.customTargetTemperatureMinimum = DefaultSettings.customTargetTemperatureMinimum;
      this.customTargetTemperatureMaximum = DefaultSettings.customTargetTemperatureMaximum;
      this.useMachineTargetCmv = DefaultSettings.useMachineTargetCmv;
      this.customTargetCmv = DefaultSettings.customTargetCmv;
      this.useMachineTargetMdp = DefaultSettings.useMachineTargetMdp;
      this.customTargetMdp = DefaultSettings.customTargetMdp;
      this.useDefaultTargetRangeCmvPercent = DefaultSettings.useDefaultTargetRangeCmvPercent;
      this.customTargetCmvPercentMinimum = DefaultSettings.customTargetCmvPercentMinimum;
      this.customTargetCmvPercentMaximum = DefaultSettings.customTargetCmvPercentMaximum;
      this.useDefaultTargetRangeMdpPercent = DefaultSettings.useDefaultTargetRangeMdpPercent;
      this.customTargetMdpPercentMinimum = DefaultSettings.customTargetMdpPercentMinimum;
      this.customTargetMdpPercentMaximum = DefaultSettings.customTargetMdpPercentMaximum;
      this.useDefaultTargetRangeSpeed = DefaultSettings.useDefaultTargetRangeSpeed;
      this.customTargetSpeedMinimum = DefaultSettings.customTargetSpeedMinimum;
      this.customTargetSpeedMaximum = DefaultSettings.customTargetSpeedMaximum;
      this.useDefaultCutFillTolerances = DefaultSettings.useDefaultCutFillTolerances;
      this.customCutFillTolerances = DefaultSettings.customCutFillTolerances;
      this.useDefaultVolumeShrinkageBulking = DefaultSettings.useDefaultVolumeShrinkageBulking;
      this.customShrinkagePercent = DefaultSettings.customShrinkagePercent;
      this.customBulkingPercent = DefaultSettings.customBulkingPercent;
      this.useDefaultPassCountTargets = DefaultSettings.useDefaultPassCountTargets;
      this.customPassCountTargets = DefaultSettings.customPassCountTargets;
      this.useDefaultCMVTargets = DefaultSettings.useDefaultCMVTargets;
      this.customCMVTargets = DefaultSettings.customCMVTargets;
      this.useDefaultTemperatureTargets = DefaultSettings.useDefaultTemperatureTargets;
      this.customTemperatureTargets = DefaultSettings.customTemperatureTargets;
    }
    #endregion

    #region Getters 

    /// <summary>
    /// Flag to determine if default or custom CMV used for summary
    /// </summary>
    public bool OverrideMachineTargetCmv => useMachineTargetCmv.HasValue && !useMachineTargetCmv.Value;
    /// <summary>
    /// Flag to determine if default or custom CMV targets used for details
    /// </summary>
    public bool OverrideDefaultCMVTargets => useDefaultCMVTargets.HasValue && !useDefaultCMVTargets.Value;
    /// <summary>
    /// Flag to determine if default or custom MDP used
    /// </summary>
    public bool OverrideMachineTargetMdp => useMachineTargetMdp.HasValue && !useMachineTargetMdp.Value;
    /// <summary>
    /// Flag to determine if default or custom Temperature targets used for details
    /// </summary>
    public bool OverrideDefaultTemperatureTargets => useDefaultTemperatureTargets.HasValue && !useDefaultTemperatureTargets.Value;
    /// <summary>
    /// Flag to determine if default or custom temperature used
    /// </summary>
    public bool OverrideMachineTargetTemperature => useMachineTargetTemperature.HasValue && !useMachineTargetTemperature.Value;
    /// <summary>
    /// Flag to determine if default or custom CMV % used
    /// </summary>
    public bool OverrideDefaultTargetRangeCmvPercent => useDefaultTargetRangeCmvPercent.HasValue &&
                                                        !useDefaultTargetRangeCmvPercent.Value;
    /// <summary>
    /// Flag to determine if default or custom MDP % used
    /// </summary>
    public bool OverrideDefaultTargetRangeMdpPercent => useDefaultTargetRangeMdpPercent.HasValue &&
                                                        !useDefaultTargetRangeMdpPercent.Value;
    /// <summary>
    /// Flag to determine if default or custom pass count target used for summary
    /// </summary>
    public bool OverrideMachineTargetPassCount => useMachineTargetPassCount.HasValue &&
                                                  !useMachineTargetPassCount.Value;
    /// <summary>
    /// Flag to determine if default or custom pass count targets used for details
    /// </summary>
    public bool OverrideDefaultPassCountTargets => useDefaultPassCountTargets.HasValue && !useDefaultPassCountTargets.Value;

    /// <summary>
    /// Flag to determine if default or custom cut-fill tolerances used for details
    /// </summary>
    public bool OverrideDefaultCutFillTolerances => useDefaultCutFillTolerances.HasValue && !useDefaultCutFillTolerances.Value;

    /// <summary>
    /// Flag to determine if default or custom speed used
    /// </summary>
    public bool OverrideDefaultTargetRangeSpeed => useDefaultTargetRangeSpeed.HasValue && !useDefaultTargetRangeSpeed.Value;
    /// <summary>
    /// Get CMV target as a value for Raptor (10ths)
    /// </summary>
    public double CustomTargetCmv => (double)(customTargetCmv ?? DefaultSettings.customTargetCmv) * 10;
    /// <summary>
    /// Get CMV target as a nullable value for Raptor (10ths) for lift buildsettings
    /// </summary>
    public short? NullableCustomTargetCmv => OverrideMachineTargetCmv && customTargetCmv.HasValue
      ? (short)(customTargetCmv.Value * 10)
      : (short?)null;
    /// <summary>
    /// Get the minimum CMV % target as a value for Raptor
    /// </summary>
    public double CustomTargetCmvPercentMinimum => OverrideDefaultTargetRangeCmvPercent && customTargetCmvPercentMinimum.HasValue ? customTargetCmvPercentMinimum.Value : (double)DefaultSettings.customTargetCmvPercentMinimum;
    /// <summary>
    /// Get the maximum CMV % target as a value for Raptor
    /// </summary>
    public double CustomTargetCmvPercentMaximum => OverrideDefaultTargetRangeCmvPercent && customTargetCmvPercentMaximum.HasValue ? customTargetCmvPercentMaximum.Value : (double)DefaultSettings.customTargetCmvPercentMaximum;
    /// <summary>
    /// Get MDP target as a value for Raptor (10ths)
    /// </summary>
    public double CustomTargetMdp => (double)(customTargetMdp ?? DefaultSettings.customTargetMdp) * 10;
    /// <summary>
    /// Get MDP target as a nullable value for Raptor (10ths) for lift buildsettings
    /// </summary>
    public short? NullableCustomTargetMdp => OverrideMachineTargetMdp && customTargetMdp.HasValue
      ? (short)(customTargetMdp.Value * 10)
      : (short?)null;
    /// <summary>
    /// Get the minimum MDP % target as a value for Raptor
    /// </summary>
    public double CustomTargetMdpPercentMinimum => OverrideDefaultTargetRangeMdpPercent && customTargetMdpPercentMinimum.HasValue ? customTargetMdpPercentMinimum.Value : (double)DefaultSettings.customTargetMdpPercentMinimum;
    /// <summary>
    /// Get the maximum MDP % target as a value for Raptor
    /// </summary>
    public double CustomTargetMdpPercentMaximum => OverrideDefaultTargetRangeMdpPercent && customTargetMdpPercentMaximum.HasValue ? customTargetMdpPercentMaximum.Value : (double)DefaultSettings.customTargetMdpPercentMaximum;
    /// <summary>
    /// Get the minimum temperature target as a value for Raptor in °C
    /// </summary>
    public double CustomTargetTemperatureMinimum => customTargetTemperatureMinimum ?? (double)DefaultSettings.customTargetTemperatureMinimum;
    /// <summary>
    /// Get the maximum temperature target as a value for Raptor in °C
    /// </summary>
    public double CustomTargetTemperatureMaximum => customTargetTemperatureMaximum ?? (double)DefaultSettings.customTargetTemperatureMaximum;

    /// <summary>
    /// Get the minimum speed target as a value for Raptor in cm/s
    /// </summary>
    public ushort CustomTargetSpeedMinimum => (ushort)Math.Round((OverrideDefaultTargetRangeSpeed && customTargetSpeedMinimum.HasValue ?
      customTargetSpeedMinimum.Value : (double)DefaultSettings.customTargetSpeedMinimum) * KM_HR_TO_CM_SEC);
    /// <summary>
    /// Get the maximum speed target as a value for Raptor in cm/s
    /// </summary>
    public ushort CustomTargetSpeedMaximum => (ushort)Math.Round((OverrideDefaultTargetRangeSpeed && customTargetSpeedMaximum.HasValue ?
      customTargetSpeedMaximum.Value : (double)DefaultSettings.customTargetSpeedMaximum) * KM_HR_TO_CM_SEC);
    /// <summary>
    /// Get the pass count details targets as a value for Raptor
    /// </summary>
    public int[] CustomPassCounts => OverrideDefaultPassCountTargets &&
      customPassCountTargets != null && customPassCountTargets.Count > 0
      ? customPassCountTargets.ToArray()
      : DefaultSettings.customPassCountTargets.ToArray();

    /// <summary>
    /// Get the CMV details targets as a value for Raptor
    /// </summary>
    public int[] CustomCMVs => (OverrideDefaultCMVTargets && customCMVTargets != null && customCMVTargets.Count > 0
                                ? customCMVTargets
                                : DefaultSettings.customCMVTargets).Select(d => { d = d * 10; return d; }).ToArray();

    /// <summary>
    /// Get the Temperature details targets as a value for Raptor
    /// </summary>
    public double[] CustomTemperatures => (OverrideDefaultTemperatureTargets && customTemperatureTargets != null && customTemperatureTargets.Count > 0
      ? customTemperatureTargets
      : DefaultSettings.customTemperatureTargets).Select(d => { d = d * 10; return d; }).ToArray();


    /// <summary>
    /// Get the cut-fill details targets as a value for Raptor
    /// </summary>
    public double[] CustomCutFillTolerances => OverrideDefaultCutFillTolerances &&
                                       customCutFillTolerances != null && customCutFillTolerances.Count > 0
        ? customCutFillTolerances.ToArray()
        : DefaultSettings.customCutFillTolerances.ToArray();
    /// <summary>
    /// Get the minimum temperature warning level as a value for Raptor in 10ths of °C
    /// </summary>
    public ushort CustomTargetTemperatureWarningLevelMinimum => (ushort)Math.Round(CustomTargetTemperatureMinimum * 10);
    /// <summary>
    /// Get the maximum temperature warning level as a value for Raptor in 10ths of °C
    /// </summary>
    public ushort CustomTargetTemperatureWarningLevelMaximum => (ushort)Math.Round(CustomTargetTemperatureMaximum * 10);

    /// <summary>
    /// The CMV % change settings (no custom ones for now)
    /// </summary>
    public double[] CmvPercentChange => new double[] { -50.0, -20.0, -10.0, 0.0, 10.0, 20.0, 50.0 };
    /// <summary>
    /// Minimum CMV for Raptor (10ths)
    /// </summary>
    public short CmvMinimum => MIN_CMV_MDP_VALUE;
    /// <summary>
    /// Maximum CMV for Raptor (10ths)
    /// </summary>
    public short CmvMaximum => MAX_CMV_MDP_VALUE;
    /// <summary>
    /// Minimum MDP for Raptor (10ths)
    /// </summary>
    public short MdpMinimum => MIN_CMV_MDP_VALUE;
    /// <summary>
    /// Maximum MDP for Raptor (10ths)
    /// </summary>
    public short MdpMaximum => MAX_CMV_MDP_VALUE;

    public double VolumeBulkingPercent => customBulkingPercent ?? 0;

    public double VolumeShrinkagePercent => customShrinkagePercent ?? 0;


    #endregion

    #region Validation
    /// <summary>
    /// Validates all properties
    /// </summary>
    public void Validate(IServiceExceptionHandler serviceExceptionHandler)
    {
      var validator = new DataAnnotationsValidator();
      validator.TryValidate(this, out ICollection<ValidationResult> results);
      if (results.Any())
      {
        throw new ServiceException(HttpStatusCode.BadRequest, new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError, results.FirstOrDefault().ErrorMessage));
      }

      ValidateRange(useMachineTargetPassCount, customTargetPassCountMinimum, customTargetPassCountMaximum, "pass count");
      ValidateRange(useMachineTargetTemperature, customTargetTemperatureMinimum, customTargetTemperatureMaximum, "temperature");
      ValidateRange(useDefaultTargetRangeCmvPercent, customTargetCmvPercentMinimum, customTargetCmvPercentMaximum, "CMV %");
      ValidateRange(useDefaultTargetRangeMdpPercent, customTargetMdpPercentMinimum, customTargetMdpPercentMaximum, "MDP %");
      ValidateRange(useDefaultTargetRangeSpeed, customTargetSpeedMinimum, customTargetSpeedMaximum, "Speed");

      ValidateValue(useMachineTargetCmv, customTargetCmv, "CMV");
      ValidateValue(useMachineTargetMdp, customTargetMdp, "MDP");
      ValidateValue(useDefaultVolumeShrinkageBulking, customBulkingPercent, "bulking %");
      ValidateValue(useDefaultVolumeShrinkageBulking, customShrinkagePercent, "shrinkage %");

      ValidateCutFill();
      ValidatePassCounts();
      ValidateCMVs();
      ValidateTemperatures();
    }

    /// <summary>
    /// Validates cut-fill values
    /// </summary>
    private void ValidateCutFill()
    {
      if (useDefaultCutFillTolerances.HasValue && !useDefaultCutFillTolerances.Value)
      {
        const int CUT_FILL_TOTAL = 7;
        if (customCutFillTolerances == null || customCutFillTolerances.Count != CUT_FILL_TOTAL)
        {
          throw new ServiceException(HttpStatusCode.BadRequest,
            new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
              $"Exactly {CUT_FILL_TOTAL} cut-fill tolerances must be specified"));
        }

        for (int i = 0; i < CUT_FILL_TOTAL; i++)
        {
          if (customCutFillTolerances[i] < MIN_CUT_FILL || customCutFillTolerances[i] > MAX_CUT_FILL)
          {
            throw new ServiceException(HttpStatusCode.BadRequest,
              new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
                $"Cut-fill tolerances must be between {MIN_CUT_FILL} and {MAX_CUT_FILL} meters"));
          }
        }

        for (int i = 1; i < CUT_FILL_TOTAL; i++)
        {
          if (customCutFillTolerances[i - 1] < customCutFillTolerances[i])
          {
            throw new ServiceException(HttpStatusCode.BadRequest,
              new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
                "Cut-fill tolerances must be in order of highest cut to lowest fill"));
          }
        }

        if (customCutFillTolerances[CUT_FILL_TOTAL / 2] != 0)
        {
          throw new ServiceException(HttpStatusCode.BadRequest,
            new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
              "On grade cut-fill tolerance must be 0"));
        }
      }
    }


    private void ValidatePassCounts()
    {
      if (useDefaultPassCountTargets.HasValue && !useDefaultPassCountTargets.Value)
      {
        const int PASS_COUNT_TOTAL = 8;
        if (customPassCountTargets == null || customPassCountTargets.Count != PASS_COUNT_TOTAL)
        {
          throw new ServiceException(HttpStatusCode.BadRequest,
            new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
              $"Exactly {PASS_COUNT_TOTAL} pass count targets must be specified"));
        }

        for (int i = 0; i < PASS_COUNT_TOTAL; i++)
        {
          if (customPassCountTargets[i] < MIN_PASS_COUNT || customPassCountTargets[i] > MAX_PASS_COUNT)
          {
            throw new ServiceException(HttpStatusCode.BadRequest,
              new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
                $"Pass count targets must be between {MIN_PASS_COUNT} and {MAX_PASS_COUNT}"));
          }
        }

        for (int i = 1; i < PASS_COUNT_TOTAL; i++)
        {
          if (customPassCountTargets[i - 1] > customPassCountTargets[i])
          {
            throw new ServiceException(HttpStatusCode.BadRequest,
              new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
                "Pass count targets must be in ascending order"));
          }
        }

        if (customPassCountTargets[0] != 1)
        {
          throw new ServiceException(HttpStatusCode.BadRequest,
            new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
              "First pass count target must be 1"));
        }
      }
    }

    private void ValidateCMVs()
    {
      if (useDefaultCMVTargets.HasValue && !useDefaultCMVTargets.Value)
      {
        const int CMV_TARGETS_TOTAL = 5;
        if (customCMVTargets == null || customCMVTargets.Count != CMV_TARGETS_TOTAL)
        {
          throw new ServiceException(HttpStatusCode.BadRequest,
            new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
              $"Exactly {CMV_TARGETS_TOTAL} CMV targets must be specified"));
        }

        for (int i = 0; i < CMV_TARGETS_TOTAL; i++)
        {
          if (customCMVTargets[i] < MIN_CMV_MDP_VALUE || customCMVTargets[i] > MAX_CMV_MDP_VALUE)
          {
            throw new ServiceException(HttpStatusCode.BadRequest,
              new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
                $"CMV targets must be between {MIN_CMV_MDP_VALUE} and {MAX_CMV_MDP_VALUE / 10}"));
          }
        }

        for (int i = 1; i < CMV_TARGETS_TOTAL; i++)
        {
          if (customCMVTargets[i - 1] > customCMVTargets[i])
          {
            throw new ServiceException(HttpStatusCode.BadRequest,
              new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
                "CMV targets must be in ascending order"));
          }
        }

        if (customCMVTargets[0] != 0)
        {
          throw new ServiceException(HttpStatusCode.BadRequest,
            new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
              "First CMV target must be 0"));
        }
      }
    }

    private void ValidateTemperatures()
    {
      if (useDefaultTemperatureTargets.HasValue && !useDefaultTemperatureTargets.Value)
      {
        const int TEMPERATURE_TARGETS_TOTAL = 7;
        if (customTemperatureTargets == null || customTemperatureTargets.Count != TEMPERATURE_TARGETS_TOTAL)
        {
          throw new ServiceException(HttpStatusCode.BadRequest,
            new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
              $"Exactly {TEMPERATURE_TARGETS_TOTAL} Temperature targets must be specified"));
        }

        for (int i = 0; i < TEMPERATURE_TARGETS_TOTAL; i++)
        {
          if (customTemperatureTargets[i] < ValidationConstants.MIN_TEMPERATURE || customTemperatureTargets[i] > ValidationConstants.MAX_TEMPERATURE)
          {
            throw new ServiceException(HttpStatusCode.BadRequest,
              new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
                $"Temperature targets must be between {ValidationConstants.MIN_TEMPERATURE} and {ValidationConstants.MAX_TEMPERATURE}"));
          }
        }

        for (int i = 1; i < TEMPERATURE_TARGETS_TOTAL; i++)
        {
          if (customTemperatureTargets[i - 1] > customTemperatureTargets[i])
          {
            throw new ServiceException(HttpStatusCode.BadRequest,
              new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
                "Temperature targets must be in ascending order"));
          }
        }

        if (customTemperatureTargets[0] != 0)
        {
          throw new ServiceException(HttpStatusCode.BadRequest,
            new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
              "First Temperature target must be 0"));
        }
      }


    }

    /// <summary>
    /// Validates a single target value
    /// </summary>
    /// <param name="useMachineTarget">Flag to indicate if machine/default target is used</param>
    /// <param name="customTargetValue">Custom target/default value</param>
    /// <param name="what">What is being validated for error message</param>
    private void ValidateValue(bool? useMachineTarget, double? customTargetValue, string what)
    {
      if (useMachineTarget.HasValue && !useMachineTarget.Value)
      {
        if (!customTargetValue.HasValue)
        {
          throw new ServiceException(HttpStatusCode.BadRequest,
            new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
              $"Custom target {what} value is required"));
        }
      }
    }

    /// <summary>
    /// Validates a target range
    /// </summary>
    /// <param name="useMachineTarget">Flag to indicate if machine/default target is used</param>
    /// <param name="customTargetMinimum">Custom target/default minimum value</param>
    /// <param name="customTargetMaxiumum">Custom target/default maximum value</param>
    /// <param name="what">What is being validated for error message</param>
    private void ValidateRange(bool? useMachineTarget, double? customTargetMinimum, double? customTargetMaxiumum, string what)
    {
      if (useMachineTarget.HasValue && !useMachineTarget.Value)
      {
        if (!customTargetMinimum.HasValue || !customTargetMaxiumum.HasValue)
        {
          throw new ServiceException(HttpStatusCode.BadRequest,
            new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
              $"Both minimum and maximum target {what} must be specified"));
        }
        if (customTargetMinimum.Value > customTargetMaxiumum.Value)
        {
          throw new ServiceException(HttpStatusCode.BadRequest,
            new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
              $"Minimum target {what} must be less than maximum"));
        }
      }
    }
    #endregion

    private const double MIN_TEMPERATURE = 1.0; //°C
    private const double MAX_TEMPERATURE = 1000.0;

    private const int MIN_PASS_COUNT = 1;
    private const int MAX_PASS_COUNT = 80;

    private const double MIN_RAW_CMV = 1.0;
    private const double MAX_RAW_CMV = 999.0;

    private const double MIN_RAW_MDP = 1.0;
    private const double MAX_RAW_MDP = 999.0;

    private const double MIN_CMV_PERCENT = 1.0;
    private const double MAX_CMV_PERCENT = 250.0;

    private const double MIN_MDP_PERCENT = 1.0;
    private const double MAX_MDP_PERCENT = 250.0;

    private const double MIN_SPEED = 1.0;   //km/h
    private const double MAX_SPEED = 100.0;

    private const double MIN_SHRINKAGE = 0.0;   //%
    private const double MAX_SHRINKAGE = 100.0;

    private const double MIN_BULKING = 0.0;   //%
    private const double MAX_BULKING = 100.0;

    private const double MIN_CUT_FILL = -400.0; //m
    private const double MAX_CUT_FILL = 400.0;

    private const short MIN_CMV_MDP_VALUE = 0;
    private const short MAX_CMV_MDP_VALUE = 5000;
  }
}

