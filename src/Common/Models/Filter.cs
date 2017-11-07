﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using VSS.Common.Exceptions;
using VSS.Common.ResultsHandling;
using VSS.MasterData.Models.Models;
using VSS.Productivity3D.Common.Interfaces;
using VSS.Productivity3D.Common.Utilities;

namespace VSS.Productivity3D.Common.Models
{
  /// <summary>
  /// Defines all the filter parameters that may be supplied as a part of a request. Filters control spatial, temporal and attribute aspects of the info
  /// Filter will override filter ID, if both are selected.
  /// </summary>
  public class Filter : IValidatable
  {
    /// <summary>
    /// The ID for a filter if stored in the Filters service. Not required or used in the proper functioning of a filter.
    /// Will be overridden by Filter, if Filter is also selected.
    /// </summary>
    [JsonProperty(PropertyName = "ID", Required = Required.Default)]
    public long? Id { get; set; }

    /// <summary>
    /// The name for a filter if stored in the Filters service. Not required or used in the proper functioning of a filter.
    /// </summary>
    [JsonProperty(PropertyName = "name", Required = Required.Default)]
    public string Name { get; private set; }

    /// <summary>
    /// The description for a filter if stored in the Filters service. Not required or used in the proper functioning of a filter.
    /// </summary>
    [JsonProperty(PropertyName = "description", Required = Required.Default)]
    public string Description { get; private set; }

    /// <summary>
    /// The 'start' time for a time based filter. Data recorded earlier to this time is not considered.
    /// Optional. If not present then there is no start time bound.
    /// </summary>
    [JsonProperty(PropertyName = "startUTC", Required = Required.Default)]
    public DateTime? StartUtc { get; private set; }

    /// <summary>
    /// The 'end' time for a time based filter. Data recorded after this time is not considered.
    /// Optional. If not present there is no end time bound.
    /// </summary>
    [JsonProperty(PropertyName = "endUTC", Required = Required.Default)]
    public DateTime? EndUtc { get; private set; }

    /// <summary>
    /// A machine reported design. Cell passes recorded when a machine did not have this design loaded at the time is not considered.
    /// May be null/empty, which indicates no restriction. 
    /// </summary>
    [JsonProperty(PropertyName = "onMachineDesignID", Required = Required.Default)]
    public long? OnMachineDesignId { get; private set; } //PDS not VL ID

    /// <summary>
    ///  A list of machine IDs. Cell passes recorded by machine other than those in this list are not considered.
    ///  May be null/empty, which indicates no restriction.
    /// </summary>
    [JsonProperty(PropertyName = "assetIDs", Required = Required.Default)]
    public List<long> AssetIDs { get; private set; }

    /// <summary>
    /// Only filter cell passes recorded when the vibratory drum was 'on'.  If set to null, returns all cell passes.  If true, returns only cell passes with the cell pass parameter and the drum was on.  If false, returns only cell passes with the cell pass parameter and the drum was off.
    /// </summary>
    [JsonProperty(PropertyName = "vibeStateOn", Required = Required.Default)]
    public bool? VibeStateOn { get; private set; }

    /// <summary>
    /// Only use cell passes recorded by compaction machines. If true, only return data recorded by compaction machines.  If false or null, returns all machines.
    /// </summary>
    [JsonProperty(PropertyName = "compactorDataOnly", Required = Required.Default)]
    public bool? CompactorDataOnly { get; private set; }

    /// <summary>
    /// Controls the cell pass from which to determine data based on its elevation.
    /// </summary>
    [JsonProperty(PropertyName = "elevationType", Required = Required.Default)]
    public ElevationType? ElevationType { get; private set; }

    /// <summary>
    /// A polygon to be used as a spatial filter boundary. The vertices are WGS84 positions
    /// </summary>
    [JsonProperty(PropertyName = "polygonLL", Required = Required.Default)]
    public List<WGSPoint> PolygonLl { get; private set; }

    /// <summary>
    /// A polygon to be used as a spatial filter boundary. The vertices are grid positions within the project grid coordinate system
    /// </summary>
    [JsonProperty(PropertyName = "polygonGrid", Required = Required.Default)]
    public List<Point> PolygonGrid { get; private set; }

    /// <summary>
    /// Only use cell passes recorded when the machine was driving in the forwards direction. If true, only returns machines travelling forward, if false, returns machines travelling in reverse, if null, returns all machines.
    /// </summary>
    [JsonProperty(PropertyName = "forwardDirection", Required = Required.Default)]
    public bool? ForwardDirection { get; private set; }

    /// <summary>
    /// The alignment file to be used as an alignment spatial filter
    /// </summary>
    [JsonProperty(PropertyName = "alignmentFile", Required = Required.Default)]
    public DesignDescriptor AlignmentFile { get; private set; }

    /// <summary>
    /// The starting station position on a alignment being used as a spatial filter. The value is expressed in meters.
    /// </summary>
    [Range(ValidationConstants.MIN_STATION, ValidationConstants.MAX_STATION)]
    [JsonProperty(PropertyName = "startStation", Required = Required.Default)]
    public double? StartStation { get; private set; }

    /// <summary>
    /// The ending station position on a alignment being used as a spatial filter. The value is expressed in meters.
    /// </summary>
    [Range(ValidationConstants.MIN_STATION, ValidationConstants.MAX_STATION)]
    [JsonProperty(PropertyName = "endStation", Required = Required.Default)]
    public double? EndStation { get; private set; }

    /// <summary>
    /// The left offset position on a alignment being used as a spatial filter. The value is expressed in meters.
    /// This value may be negative, in which case it will be to the right of the alignment.
    /// </summary>
    [Range(ValidationConstants.MIN_OFFSET, ValidationConstants.MAX_OFFSET)]
    [JsonProperty(PropertyName = "leftOffset", Required = Required.Default)]
    public double? LeftOffset { get; private set; }

    /// <summary>
    /// The right offset position on a alignment being used as a spatial filter. The value is expressed in meters.
    /// This value may be negative, in which case it will be to the left of the alignment.
    /// </summary>
    [Range(ValidationConstants.MIN_OFFSET, ValidationConstants.MAX_OFFSET)]
    [JsonProperty(PropertyName = "rightOffset", Required = Required.Default)]
    public double? RightOffset { get; private set; }

    /// <summary>
    /// Only consider cell passes recorded when the machine had the named design loaded.
    /// </summary>
    [JsonProperty(PropertyName = "machineDesignName", Required = Required.Default)]
    public string MachineDesignName { get; private set; }

    /// <summary>
    /// layerType indicates the layer analysis method to be used for determining layers from cell passes. Some of the layer types are implemented as a 
    /// 3D spatial volume inclusion implemented via 3D spatial filtering. Only cell passes whose three dimensional location falls within the bounds
    /// 3D spatial volume are considered. If it is required to apply layer filter, lift analysis method and corresponding parameters should be specified here.
    ///  Otherwise (build lifts but do not filter, only do production data analysis) Lift Layer Analysis setting should be specified in LiftBuildSettings.
    /// </summary>
    [JsonProperty(PropertyName = "layerType", Required = Required.Default)]
    public FilterLayerMethod? LayerType { get; private set; }

    /// <summary>The design or alignment file in the project that is to be used as a spatial filter when the filter layer method is OffsetFromDesign or OffsetFromProfile.
    /// 
    /// </summary>
    [JsonProperty(PropertyName = "designOrAlignmentFile", Required = Required.Default)]
    public DesignDescriptor DesignOrAlignmentFile { get; private set; }

    /// <summary>
    /// The elevation of the bench to be used as the datum elevation for LayerBenchElevation filter layer type. The value is expressed in meters.
    /// </summary>
    [Range(ValidationConstants.MIN_ELEVATION, ValidationConstants.MAX_ELEVATION)]
    [JsonProperty(PropertyName = "benchElevation", Required = Required.Default)]
    public double? BenchElevation { get; private set; }

    /// <summary>
    /// The number of the 3D spatial layer (determined through bench elevation and layer thickness or the tag file) to be used as the layer type filter. Layer 3 is then the third layer from the
    /// datum elevation where each layer has a thickness defined by the layerThickness member.
    /// </summary>
    [Range(ValidationConstants.MIN_LAYER_NUMBER, ValidationConstants.MAX_LAYER_NUMBER)]
    [JsonProperty(PropertyName = "layerNumber", Required = Required.Default)]
    public int? LayerNumber { get; private set; }

    /// <summary>
    /// The layer thickness to be used for layers determined spatially vie the layerType member. The value is expressed in meters.
    /// </summary>
    [Range(ValidationConstants.MIN_THICKNESS, ValidationConstants.MAX_THICKNESS)]
    [JsonProperty(PropertyName = "layerThickness", Required = Required.Default)]
    public double? LayerThickness { get; private set; }

    /// <summary>
    /// Cell passes are only considered if the machines that recorded them are included in this list of machines. Use machine ID (historically VL Asset ID), or Machine Name from tagfile, not both.
    /// This may be null, which is no restriction on machines. 
    /// </summary>
    [JsonProperty(PropertyName = "contributingMachines", Required = Required.Default)]
    public List<MachineDetails> ContributingMachines { get; private set; }

    /// <summary>
    /// A list of surveyed surfaces that have been added to the project which are to be excluded from consideration.
    /// </summary>
    [JsonProperty(PropertyName = "surveyedSurfaceExclusionList", Required = Required.Default)]
    public List<long> SurveyedSurfaceExclusionList { get; private set; }

    /// <summary>
    /// The selected cell pass to be used from the cell passes matching a filter is the earliest matching pass if true. If false, or not present the latest cell pass is used.
    /// This value may be null.
    /// </summary>
    [JsonProperty(PropertyName = "returnEarliest", Required = Required.Default)]
    public bool? ReturnEarliest { get; set; }

    /// <summary>
    /// Sets the GPS accuracy filtering aspect.
    /// </summary>
    /// <value>
    /// The GPS accuracy.
    /// </value>
    [JsonProperty(PropertyName = "gpsAccuracy", Required = Required.Default)]
    public GPSAccuracy? GpsAccuracy { get; private set; }

    /// <summary>
    /// Determines if the GPS accuracy filter is inclusive or not. If the value is true then each GPS accuracy level
    /// includes the level(s) below it. If false (the default) then the GPS accuracy level is for that value only.
    /// </summary>
    [JsonProperty(PropertyName = "gpsAccuracyIsInclusive", Required = Required.Default)]
    public bool? GpsAccuracyIsInclusive { get; private set; }

    /// <summary>
    /// Use cell passes generated from the primary machine implement (blade/drum(s)). 
    /// If true, data recorded from primary implement tracking is used in satisfying the request. 
    /// If false, returns no data where cell passes are recorded from primary implement positions. 
    /// If null, the same behaviour as True is applied.
    /// </summary>
    [JsonProperty(PropertyName = "implementMapping", Required = Required.Default)]
    public bool? BladeOnGround { get; private set; }

    /// <summary>
    /// Use cell passes generated from the machine track positions. 
    /// If true, data recorded from the machine track positions is used in satisfying the request. 
    /// If false, returns no data where cell passes are recorded from track positions. 
    /// If null, the same behaviour as False is applied.
    /// </summary>
    [JsonProperty(PropertyName = "trackMapping", Required = Required.Default)]
    public bool? TrackMapping { get; private set; }

    /// <summary>
    /// Use cell passes generated from the machine wheel positions. 
    /// If true, data recorded from the machine wheel positions is used in satisfying the request. 
    /// If false, returns no data where cell passes are recorded from wheel positions. 
    /// If null, the same behaviour as False is applied.
    /// </summary>
    [JsonProperty(PropertyName = "wheelMapping", Required = Required.Default)]
    public bool? WheelTracking { get; private set; }

    /// <summary>
    /// Private constructor
    /// </summary>
    private Filter()
    { }

    /// <summary>
    /// Create instance of Filter
    /// </summary>
    public static Filter CreateFilter
        (
        long? id,
        string name,
        string description,
        DateTime? startUtc,
        DateTime? endUtc,
        long? onMachineDesignId,
        List<long> assetIDs,
        bool? vibeStateOn,
        bool? compactorDataOnly,
        ElevationType? elevationType,
        List<WGSPoint> polygonLl,
        List<Point> polygonGrid,
        bool? forwardDirection,
        DesignDescriptor alignmentFile,
        double? startStation,
        double? endStation,
        double? leftOffset,
        double? rightOffset,
        string machineDesignName,
        FilterLayerMethod? layerType,
        DesignDescriptor designOrAlignmentFile,
        double? benchElevation,
        int? layerNumber,
        double? layerThickness,
        List<MachineDetails> contributingMachines,
        List<long> surveyedSurfaceExclusionList,
        bool? returnEarliest,
        GPSAccuracy? accuracy,
        bool? inclusive,
        bool? bladeOnGround,
        bool? trackMapping,
        bool? wheelTracking
        )
    {
      return new Filter
      {
        Id = id,
        Name = name,
        Description = description,
        StartUtc = startUtc,
        EndUtc = endUtc,
        OnMachineDesignId = onMachineDesignId,
        AssetIDs = assetIDs,
        VibeStateOn = vibeStateOn,
        CompactorDataOnly = compactorDataOnly,
        ElevationType = elevationType,
        PolygonLl = polygonLl,
        PolygonGrid = polygonGrid,
        ForwardDirection = forwardDirection,
        AlignmentFile = alignmentFile,
        StartStation = startStation,
        EndStation = endStation,
        LeftOffset = leftOffset,
        RightOffset = rightOffset,
        MachineDesignName = machineDesignName,
        LayerType = layerType,
        DesignOrAlignmentFile = designOrAlignmentFile,
        BenchElevation = benchElevation,
        LayerNumber = layerNumber,
        LayerThickness = layerThickness,
        ContributingMachines = contributingMachines,
        SurveyedSurfaceExclusionList = surveyedSurfaceExclusionList,
        ReturnEarliest = returnEarliest,
        GpsAccuracy = accuracy,
        GpsAccuracyIsInclusive = inclusive,
        BladeOnGround = bladeOnGround,
        TrackMapping = trackMapping,
        WheelTracking = wheelTracking
      };
    }

    /// <summary>
    /// Create instance of Filter specifically for excluding surveyed surfaces only
    /// </summary>
    public static Filter CreateFilter(List<long> surveyedSurfaceExclusionList)
    {
      return new Filter
      {
        SurveyedSurfaceExclusionList = surveyedSurfaceExclusionList
      };
    }

    /// <summary>
    /// Validates all properties
    /// </summary>
    public void Validate()
    {
      //Validate individual properties first
      if (this.PolygonLl != null)
      {
        foreach (var ll in this.PolygonLl)
          ll.Validate();
      }
      if (this.PolygonGrid != null)
      {
        foreach (var pt in this.PolygonGrid)
          pt.Validate();
      }
      this.AlignmentFile?.Validate();
      this.DesignOrAlignmentFile?.Validate();

      if (this.ContributingMachines != null)
      {
        foreach (var machine in this.ContributingMachines)
          machine.Validate();
      }

      //Check date range parts
      if (this.StartUtc.HasValue || this.EndUtc.HasValue)
      {
        if (this.StartUtc.HasValue && this.EndUtc.HasValue)
        {
          if (this.StartUtc.Value > this.EndUtc.Value)
          {
            throw new ServiceException(HttpStatusCode.BadRequest,
                new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
                    "StartUTC must be earlier than EndUTC"));
          }
        }
        else
        {
          throw new ServiceException(HttpStatusCode.BadRequest,
              new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
                  "If using a date range both dates must be provided"));
        }
      }

      //Check alignment filter parts
      if (this.AlignmentFile != null || this.StartStation.HasValue || this.EndStation.HasValue ||
          this.LeftOffset.HasValue || this.RightOffset.HasValue)
      {
        if (this.AlignmentFile == null || !this.StartStation.HasValue || !this.EndStation.HasValue ||
          !this.LeftOffset.HasValue || !this.RightOffset.HasValue)

          throw new ServiceException(HttpStatusCode.BadRequest,
              new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
                  "If using an alignment filter, alignment file, start and end station, left and right offset  must be provided"));

        this.AlignmentFile.Validate();
      }

      //Check layer filter parts
      if (this.LayerNumber.HasValue && !this.LayerType.HasValue)
      {
        throw new ServiceException(HttpStatusCode.BadRequest,
            new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
                "To use the layer number filter, layer type must be specified"));
      }

      if (this.LayerType.HasValue)
      {
        switch (this.LayerType.Value)
        {
          case FilterLayerMethod.OffsetFromDesign:
          case FilterLayerMethod.OffsetFromBench:
          case FilterLayerMethod.OffsetFromProfile:
            if (this.LayerType.Value == FilterLayerMethod.OffsetFromBench)
            {
              if (!this.BenchElevation.HasValue)
              {
                throw new ServiceException(HttpStatusCode.BadRequest,
                    new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
                        "If using an offset from bench filter, bench elevation must be provided"));
              }

            }
            else
            {
              if (this.DesignOrAlignmentFile == null)
              {
                throw new ServiceException(HttpStatusCode.BadRequest,
                    new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
                        "If using an offset from design or profile filter, design or alignment file must be provided"));
              }
              this.DesignOrAlignmentFile.Validate();
            }
            if (!this.LayerNumber.HasValue || !this.LayerThickness.HasValue)
            {
              throw new ServiceException(HttpStatusCode.BadRequest,
                  new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
                      "If using an offset from bench, design or alignment filter, layer number and layer thickness must be provided"));
            }
            break;
          case FilterLayerMethod.TagfileLayerNumber:
            if (!this.LayerNumber.HasValue)
            {
              throw new ServiceException(HttpStatusCode.BadRequest,
                  new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
                      "If using a tag file layer filter, layer number must be provided"));
            }
            break;
        }
      }

      //Check boundary if provided
      //Raptor handles any weird boundary you give it and automatically closes it if not closed already therefore we just need to check we have at least 3 points
      if (this.PolygonLl != null && this.PolygonLl.Count < 3)
      {
        throw new ServiceException(HttpStatusCode.BadRequest,
            new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
                "Too few points for filter polygon"));
      }

      if (this.PolygonGrid != null && this.PolygonGrid.Count < 3)
      {
        throw new ServiceException(HttpStatusCode.BadRequest,
             new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
                 "Too few points for filter polygon"));
      }

      if (this.PolygonLl != null && this.PolygonLl.Count > 0 && this.PolygonGrid != null && this.PolygonGrid.Count > 0)
      {
        throw new ServiceException(HttpStatusCode.BadRequest,
                 new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
                     "Only one type of filter boundary can be defined at one time"));
      }
    }
  }
}