﻿using System;
using System.IO;
using FluentAssertions;
using VSS.TRex.Common.Types;
using VSS.TRex.Filters;
using VSS.TRex.Tests.BinarizableSerialization;
using VSS.TRex.Types;
using Xunit;

namespace VSS.TRex.Tests.Filters
{
  public class CellPassAttributeFilterModelTests
  {
    [Fact]
    public void Creation()
    {
      var _ = new CellPassAttributeFilterModel();
    }

    [Fact]
    public void FromToBinary()
    {
      var data = new CellPassAttributeFilterModel
      {
        GPSAccuracy = GPSAccuracy.Fine,
        MachinesList = new Guid[] {Guid.NewGuid(), Guid.NewGuid() },
        SurveyedSurfaceExclusionList = new Guid[] { Guid.NewGuid(), Guid.NewGuid() },
        LayerID = 1,
        GPSTolerance = 2,
        HasDesignFilter = true,
        DesignNameID = 3,
        ElevationMappingMode = ElevationMappingMode.LatestElevation,
        ElevationRangeDesignUID = Guid.NewGuid(),
        ElevationRangeLevel = 4.0,
        ElevationRangeOffset = 5.0,
        ElevationRangeThickness = 6.0,
        ElevationType = ElevationType.Highest,
        EndTime = DateTime.MaxValue,
        FilterTemperatureByLastPass = true,
        GCSGuidanceMode = MachineAutomaticsMode.Manual,
        GPSAccuracyIsInclusive = true,
        GPSToleranceIsGreaterThan = true,
        HasCompactionMachinesOnlyFilter = true,
        HasElevationMappingModeFilter = true,
        HasElevationRangeFilter = true,
        HasElevationTypeFilter = true,
        HasGCSGuidanceModeFilter = true,
        HasGPSAccuracyFilter = true,
        HasGPSToleranceFilter = true,
        HasLayerIDFilter = true,
        HasLayerStateFilter = true,
        HasMachineDirectionFilter = true,
        HasMachineFilter = true,
        HasPassCountRangeFilter = true,
        HasPassTypeFilter = true,
        HasPositioningTechFilter = true,
        HasTemperatureRangeFilter = true,
        HasTimeFilter = true,
        HasVibeStateFilter = true,
        LayerState = LayerState.On,
        MachineDirection = MachineDirection.Reverse,
        MaterialTemperatureMax = 10,
        MaterialTemperatureMin = 11,
        PassCountRangeMax = 13,
        PassCountRangeMin = 12,
        PassTypeSet = PassTypeSet.Rear | PassTypeSet.Front,
        PositioningTech = PositioningTech.UTS,
        RequestedGridDataType = GridDataType.CellProfile,
        ReturnEarliestFilteredCellPass = true,
        StartTime = DateTime.MinValue,
        VibeState = VibrationState.On       
      };

      var writer = new TestBinaryWriter();
      data.ToBinary(writer);

      var data2 = new CellPassAttributeFilterModel();
      data2.FromBinary(new TestBinaryReader(writer._stream.BaseStream as MemoryStream));

      data2.Should().BeEquivalentTo(data2);
    }
  }
}
