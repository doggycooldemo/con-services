﻿using System;
using FluentAssertions;
using VSS.MasterData.Models.Models;
using VSS.TRex.Cells;
using VSS.TRex.Common;
using VSS.TRex.Common.Types;
using VSS.TRex.Filters;
using VSS.TRex.Machines;
using VSS.TRex.Storage.Models;
using VSS.TRex.SubGridTrees.Core.Utilities;
using VSS.TRex.SubGridTrees.Interfaces;
using VSS.TRex.Tests.TestFixtures;
using VSS.TRex.Types;
using Xunit;
using ElevationType = VSS.TRex.Common.Types.ElevationType;

namespace VSS.TRex.Tests.Filters
{
        public class CellPassAttributeFilterTests : IClassFixture<DILoggingFixture>
  {
        [Fact()]
        public void Test_CellPassAttributeFilter_CellPassAttributeFilter()
        {
            CellPassAttributeFilter filter = new CellPassAttributeFilter();

            Assert.False(filter.AnyFilterSelections || filter.AnyMachineEventFilterSelections || filter.AnyNonMachineEventFilterSelections,
                "Filter flags set for default filter");
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_Prepare()
        {
            CellPassAttributeFilter filter = new CellPassAttributeFilter();

            filter.ClearFilter();

            Assert.False(filter.AnyFilterSelections, "AnyFilterSelections not false");
            Assert.False(filter.AnyMachineEventFilterSelections, "AnyMachineEventFilterSelections not false");
            Assert.False(filter.AnyNonMachineEventFilterSelections, "AnyNonMachineEventFilterSelections not false");

            filter.HasTimeFilter = true;

            Assert.True(filter.AnyFilterSelections, "AnyFilterSelections not true after adding time filter");
            Assert.False(filter.AnyMachineEventFilterSelections, "AnyMachineEventFilterSelections not false");
            Assert.True(filter.AnyNonMachineEventFilterSelections, "AnyNonMachineEventFilterSelections not true after adding time filter");

            filter.ClearFilter();
            filter.HasPositioningTechFilter = true;

            Assert.True(filter.AnyFilterSelections, "AnyFilterSelections not true");
            Assert.True(filter.AnyMachineEventFilterSelections, "AnyMachineEventFilterSelections not true");
            Assert.False(filter.AnyNonMachineEventFilterSelections, "AnyNonMachineEventFilterSelections true");
        }

        private void Test_CellPassAttributeFilter_ClearFilterAll_CheckByAspect(Action<CellPassAttributeFilter, bool> setAspect, 
          Func<CellPassAttributeFilter, bool> getAspect)
        {
          CellPassAttributeFilter filter = new CellPassAttributeFilter();

          filter.AnyFilterSelections.Should().BeFalse();
          getAspect.Invoke(filter).Should().BeFalse();
          setAspect.Invoke(filter, true);

          filter.AnyFilterSelections.Should().BeTrue();
          getAspect.Invoke(filter).Should().BeTrue();

          filter.HasTimeFilter = true;
          filter.ClearFilter();

          filter.AnyFilterSelections.Should().BeFalse();
          filter.HasTimeFilter.Should().BeFalse();
        }

        [Fact]
        public void Test_CellPassAttributeFilter_ClearFilter()
        {
          Test_CellPassAttributeFilter_ClearFilterAll_CheckByAspect((filter, state) => filter.HasTimeFilter = state, filter => filter.HasTimeFilter);
          Test_CellPassAttributeFilter_ClearFilterAll_CheckByAspect((filter, state) => filter.HasCompactionMachinesOnlyFilter = state, filter => filter.HasCompactionMachinesOnlyFilter);
          Test_CellPassAttributeFilter_ClearFilterAll_CheckByAspect((filter, state) => filter.HasDesignFilter = state, filter => filter.HasDesignFilter);
          Test_CellPassAttributeFilter_ClearFilterAll_CheckByAspect((filter, state) => filter.HasElevationMappingModeFilter = state, filter => filter.HasElevationMappingModeFilter);
          Test_CellPassAttributeFilter_ClearFilterAll_CheckByAspect((filter, state) => filter.HasElevationRangeFilter = state, filter => filter.HasElevationRangeFilter);
          Test_CellPassAttributeFilter_ClearFilterAll_CheckByAspect((filter, state) => filter.HasElevationTypeFilter = state, filter => filter.HasElevationTypeFilter);
          Test_CellPassAttributeFilter_ClearFilterAll_CheckByAspect((filter, state) => filter.HasGCSGuidanceModeFilter = state, filter => filter.HasGCSGuidanceModeFilter);
          Test_CellPassAttributeFilter_ClearFilterAll_CheckByAspect((filter, state) => filter.HasGPSAccuracyFilter = state, filter => filter.HasGPSAccuracyFilter);
          Test_CellPassAttributeFilter_ClearFilterAll_CheckByAspect((filter, state) => filter.HasLayerIDFilter = state, filter => filter.HasLayerIDFilter);
          Test_CellPassAttributeFilter_ClearFilterAll_CheckByAspect((filter, state) => filter.HasGPSToleranceFilter = state, filter => filter.HasGPSToleranceFilter);
          Test_CellPassAttributeFilter_ClearFilterAll_CheckByAspect((filter, state) => filter.HasLayerStateFilter = state, filter => filter.HasLayerStateFilter);
          Test_CellPassAttributeFilter_ClearFilterAll_CheckByAspect((filter, state) => filter.HasMachineDirectionFilter = state, filter => filter.HasMachineDirectionFilter);
          Test_CellPassAttributeFilter_ClearFilterAll_CheckByAspect((filter, state) => filter.HasMachineFilter = state, filter => filter.HasMachineFilter);
          Test_CellPassAttributeFilter_ClearFilterAll_CheckByAspect((filter, state) => filter.HasPassTypeFilter = state, filter => filter.HasPassTypeFilter);
          Test_CellPassAttributeFilter_ClearFilterAll_CheckByAspect((filter, state) => filter.HasVibeStateFilter = state, filter => filter.HasVibeStateFilter);
          Test_CellPassAttributeFilter_ClearFilterAll_CheckByAspect((filter, state) => filter.HasPassCountRangeFilter = state, filter => filter.HasPassCountRangeFilter);
          Test_CellPassAttributeFilter_ClearFilterAll_CheckByAspect((filter, state) => filter.HasTemperatureRangeFilter = state, filter => filter.HasTemperatureRangeFilter);
          Test_CellPassAttributeFilter_ClearFilterAll_CheckByAspect((filter, state) => filter.HasPositioningTechFilter = state, filter => filter.HasPositioningTechFilter);
        }

        private void Test_CellPassAttributeFilter_ClearFilter_Aspect
            (string name, Action<CellPassAttributeFilter> setState, Func<CellPassAttributeFilter, bool> checkSetState,
                         Action<CellPassAttributeFilter> clearState, Func<CellPassAttributeFilter, bool> checkClearState)
        {
            CellPassAttributeFilter filter = new CellPassAttributeFilter();

            Assert.True(checkClearState(filter), $"[{name}] State set when expected to be not set (1)");

            setState(filter);
            Assert.True(checkSetState(filter), $"[{name}] State not set when expected to be set");

            clearState(filter);
            Assert.True(checkClearState(filter), $"[{name}] State set when expected to be not set (2)");
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_ClearVibeState()
        {
            Test_CellPassAttributeFilter_ClearFilter_Aspect("VibeState", 
                                                            x => { x.HasVibeStateFilter = true; x.VibeState = VibrationState.Off; },
                                                            x => x.HasVibeStateFilter && x.VibeState == VibrationState.Off,
                                                            x => { x.ClearVibeState(); }, 
                                                            x => !x.HasVibeStateFilter && x.VibeState == VibrationState.Invalid);
        }

        /* Possible obsolete functionality...
        private void Test_CellPassAttributeFilter_CompareTo_Aspect(string name, Action<CellPassAttributeFilter> SetState)
        {
            CellPassAttributeFilter filter1 = new CellPassAttributeFilter();
            CellPassAttributeFilter filter2 = new CellPassAttributeFilter();

            SetState(filter1);
            Assert.Equal(-1, filter1.CompareTo(filter2));

            SetState(filter2);
            Assert.Equal(0, filter1.CompareTo(filter2));
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_CompareTo_Time()
        {
            Test_CellPassAttributeFilter_CompareTo_Aspect("Time", x => { x.HasTimeFilter = true; x.StartTime = DateTime.SpecifyKind(new DateTime(2000, 1, 1), DateTimeKind.Utc); });
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_CompareTo_CompactionMachines()
        {
            Test_CellPassAttributeFilter_CompareTo_Aspect("CompactionMachinesOnly", x => { x.HasCompactionMachinesOnlyFilter = true; });
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_CompareTo_DesignNameID()
        {
            Test_CellPassAttributeFilter_CompareTo_Aspect("Time", x => { x.HasDesignFilter = true; x.DesignNameID = 10; });
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_CompareTo_ElevationRange()
        {
            Test_CellPassAttributeFilter_CompareTo_Aspect("ElevationRange_Design",
                x =>
                {
                    x.HasElevationRangeFilter = true;
                    x.ElevationRangeDesignUID = Guid.Empty;
                    x.ElevationRangeOffset = 10;
                    x.ElevationRangeThickness = 1;
                });

            Test_CellPassAttributeFilter_CompareTo_Aspect("ElevationRange_Level",
                x =>
                {
                    x.HasElevationRangeFilter = true;
                    x.ElevationRangeLevel = 100;
                    x.ElevationRangeOffset = 10;
                    x.ElevationRangeThickness = 1;
                });
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_CompareTo_ElevationType()
        {
            Test_CellPassAttributeFilter_CompareTo_Aspect("ElevationType", x => { x.HasElevationTypeFilter = true; x.ElevationType = ElevationType.Highest; });
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_CompareTo_GCSGuidanceMode()
        {
            Test_CellPassAttributeFilter_CompareTo_Aspect("GCSGuidanceMode", x => { x.HasGCSGuidanceModeFilter = true; x.GCSGuidanceMode = MachineAutomaticsMode.Automatics; });
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_CompareTo_GPSAccuracy()
        {
            Test_CellPassAttributeFilter_CompareTo_Aspect("GPSAccuracy", x => { x.HasGPSAccuracyFilter = true; x.GPSAccuracy = GPSAccuracy.Medium; });
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_CompareTo_GPSTolerance()
        {
            Test_CellPassAttributeFilter_CompareTo_Aspect("GPSTolerance", x => { x.HasGPSToleranceFilter = true; x.GPSTolerance = 1000; });
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_CompareTo_LayerID()
        {
            Test_CellPassAttributeFilter_CompareTo_Aspect("LayerID", x => { x.HasLayerIDFilter = true; x.LayerID = 10; });
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_CompareTo_LayerState()
        {
            Test_CellPassAttributeFilter_CompareTo_Aspect("LayerState On", x => { x.HasLayerStateFilter = true; x.LayerState = LayerState.On; });
            Test_CellPassAttributeFilter_CompareTo_Aspect("LayerState Off", x => { x.HasLayerStateFilter = true; x.LayerState = LayerState.Off; });
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_CompareTo_MachineDirection()
        {
            Test_CellPassAttributeFilter_CompareTo_Aspect("MachineDirection forward", x => { x.HasMachineDirectionFilter = true; x.MachineDirection = MachineDirection.Forward; });
            Test_CellPassAttributeFilter_CompareTo_Aspect("MachineDirection reverse", x => { x.HasMachineDirectionFilter = true; x.MachineDirection = MachineDirection.Reverse; });
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_CompareTo_Machine()
        {
            Test_CellPassAttributeFilter_CompareTo_Aspect("Machine true", x => { x.HasMachineFilter = true; });

            Test_CellPassAttributeFilter_CompareTo_Aspect("Machine true with list", x => { x.HasMachineFilter = true; x.MachineIDs = new short[] { 1 };});
            Test_CellPassAttributeFilter_CompareTo_Aspect("Machine true with list", x => { x.HasMachineFilter = true; x.MachineIDs = new short [] {1, 2, 3};});
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_CompareTo_ElevationMappingMode()
        {
            Test_CellPassAttributeFilter_CompareTo_Aspect("ElevationMappingMode true", x => { x.HasElevationMappingModeFilter = true; x.ElevationMappingMode = ElevationMappingMode.MinimumElevation; });
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_CompareTo_PassType()
        {
            Test_CellPassAttributeFilter_CompareTo_Aspect("PassType", x => { x.HasPassTypeFilter = true; x.PassTypeSet = PassTypeSet.Front; });
            Test_CellPassAttributeFilter_CompareTo_Aspect("PassType", x => { x.HasPassTypeFilter = true; x.PassTypeSet = PassTypeSet.Rear; });
            Test_CellPassAttributeFilter_CompareTo_Aspect("PassType", x => { x.HasPassTypeFilter = true; x.PassTypeSet = PassTypeSet.Track; });
            Test_CellPassAttributeFilter_CompareTo_Aspect("PassType", x => { x.HasPassTypeFilter = true; x.PassTypeSet = PassTypeSet.Wheel; });
            Test_CellPassAttributeFilter_CompareTo_Aspect("PassType", x => { x.HasPassTypeFilter = true; x.PassTypeSet = PassTypeSet.Front | PassTypeSet.Rear | PassTypeSet.Wheel | PassTypeSet.Track; });
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_CompareTo_PositioningTech()
        {
            Test_CellPassAttributeFilter_CompareTo_Aspect("PositioningTech GPS", x => { x.HasPositioningTechFilter = true; x.PositioningTech = PositioningTech.GPS; });
            Test_CellPassAttributeFilter_CompareTo_Aspect("PositioningTech UTS", x => { x.HasPositioningTechFilter = true; x.PositioningTech = PositioningTech.UTS; });
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_CompareTo_VibeState()
        {
            Test_CellPassAttributeFilter_CompareTo_Aspect("VibeState Off", x => { x.HasVibeStateFilter = true; x.VibeState = VibrationState.Off; });
            Test_CellPassAttributeFilter_CompareTo_Aspect("VibeState On", x => { x.HasVibeStateFilter = true; x.VibeState = VibrationState.On; });
        }


        [Fact()]
        public void Test_CellPassAttributeFilter_CompareTo_Temperature()
        {
          Test_CellPassAttributeFilter_CompareTo_Aspect("Temperature", x => { x.HasTemperatureRangeFilter = true; x.MaterialTemperatureMin = 10;
                                                                         x.MaterialTemperatureMax = 40;
                                                                       });
        }


        [Fact()]
        public void Test_CellPassAttributeFilter_CompareTo_PassCountRange()
        {
          Test_CellPassAttributeFilter_CompareTo_Aspect("PassCountRange", x => {
                                                                         x.HasPassCountRangeFilter = true; x.PassCountRangeMin = 1;
                                                                         x.PassCountRangeMax = 4;
                                                                       });
        }
        */

        [Fact()]
        public void Test_CellPassAttributeFilter_ClearDesigns()
        {
            Test_CellPassAttributeFilter_ClearFilter_Aspect("Designs",
                                                            x => { x.HasDesignFilter = true; x.DesignNameID = 42; },
                                                            x => x.HasDesignFilter && x.DesignNameID == 42,
                                                            x => { x.ClearDesigns(); },
                                                            x => !x.HasDesignFilter && x.DesignNameID == Consts.kNoDesignNameID);
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_ClearElevationRange()
        {
            Test_CellPassAttributeFilter_ClearFilter_Aspect("ElevationRange Design",
                                                            x => { x.HasElevationRangeFilter = true;
                                                                   x.ElevationRangeDesign.DesignID = Guid.NewGuid();
                                                                   x.ElevationRangeDesign.Offset = 1.5;
                                                                   x.ElevationRangeOffset = 10;
                                                                   x.ElevationRangeThickness = 1;
                                                            },
                                                            x => x.HasElevationRangeFilter && x.ElevationRangeDesign.DesignID != Guid.Empty && x.ElevationRangeDesign.Offset == 1.5 && x.ElevationRangeOffset == 10 && x.ElevationRangeThickness == 1,
                                                            x => { x.ClearElevationRange(); },
                                                            x => !x.HasElevationRangeFilter && x.ElevationRangeDesign.DesignID == Guid.Empty && x.ElevationRangeDesign.Offset == 0 && x.ElevationRangeOffset == Consts.NullDouble && x.ElevationRangeThickness == Consts.NullDouble);

            Test_CellPassAttributeFilter_ClearFilter_Aspect("ElevationRange Level",
                                                            x => {
                                                                x.HasElevationRangeFilter = true;
                                                                x.ElevationRangeLevel = 100;
                                                                x.ElevationRangeOffset = 10;
                                                                x.ElevationRangeThickness = 1;
                                                            },
                                                            x => x.HasElevationRangeFilter && x.ElevationRangeLevel == 100 && x.ElevationRangeOffset == 10 && x.ElevationRangeThickness == 1,
                                                            x => { x.ClearElevationRange(); },
                                                            x => !x.HasElevationRangeFilter && x.ElevationRangeLevel == Consts.NullDouble && x.ElevationRangeLevel == Consts.NullDouble && x.ElevationRangeThickness == Consts.NullDouble);
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_ClearElevationRangeFilterInitialization()
        {
            var filterAnnex = new CellPassAttributeFilterProcessingAnnex
            {
                ElevationRangeIsInitialized = true,
                ElevationRangeDesignElevations = new float[SubGridTreeConsts.SubGridTreeDimension, SubGridTreeConsts.SubGridTreeDimension]
            };

            SubGridUtilities.SubGridDimensionalIterator((x, y) => filterAnnex.ElevationRangeDesignElevations[x, y] = Consts.NullHeight);

            filterAnnex.ClearElevationRangeFilterInitialization();

            Assert.True(filterAnnex.ElevationRangeIsInitialized == false && filterAnnex.ElevationRangeDesignElevations == null);
        }

        [Fact]
        public void Test_CellPassAttributeFilter_ClearElevationType()
        {
          Test_CellPassAttributeFilter_ClearFilter_Aspect("ElevationType",
            x => {
              x.HasElevationTypeFilter = true;
              x.ElevationType = ElevationType.First;
            },
            x => x.HasElevationTypeFilter && x.ElevationType == ElevationType.First,
            x => { x.ClearElevationType(); },
            x => !x.HasElevationTypeFilter && x.ElevationType == ElevationType.Last);
        }

        [Fact]
        public void Test_CellPassAttributeFilter_ClearGPSAccuracy()
        {
          Test_CellPassAttributeFilter_ClearFilter_Aspect("GPSAccuracy",
            x => {
              x.HasGPSAccuracyFilter = true;
              x.GPSAccuracy = GPSAccuracy.Fine;
            },
            x => x.HasGPSAccuracyFilter && x.GPSAccuracy == GPSAccuracy.Fine,
            x => { x.ClearGPSAccuracy(); },
            x => !x.HasGPSAccuracyFilter && x.GPSAccuracy == GPSAccuracy.Unknown);
        }

        [Fact]
        public void Test_CellPassAttributeFilter_ClearGPSTolerance()
        {
          Test_CellPassAttributeFilter_ClearFilter_Aspect("GPSTolerance",
            x => {
              x.HasGPSToleranceFilter = true;
              x.GPSTolerance = 123;
            },
            x => x.HasGPSToleranceFilter && x.GPSTolerance == 123,
            x => { x.ClearGPSTolerance(); },
            x => !x.HasGPSToleranceFilter && x.GPSTolerance == Consts.kMaxGPSAccuracyErrorLimit);
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_ClearGuidanceMode()
        {
            Test_CellPassAttributeFilter_ClearFilter_Aspect("GuidanceMode",
                                                            x => { x.HasGCSGuidanceModeFilter = true; x.GCSGuidanceMode = AutomaticsType.Automatics; },
                                                            x => x.HasGCSGuidanceModeFilter && x.GCSGuidanceMode == AutomaticsType.Automatics,
                                                            x => { x.ClearGuidanceMode(); },
                                                            x => !x.HasGCSGuidanceModeFilter && x.GCSGuidanceMode == AutomaticsType.Unknown);
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_ClearLayerID()
        {
            Test_CellPassAttributeFilter_ClearFilter_Aspect("LayerID",
                                                            x => { x.HasLayerIDFilter = true; x.LayerID = 42; },
                                                            x => x.HasLayerIDFilter && x.LayerID == 42,
                                                            x => { x.ClearLayerID(); },
                                                            x => !x.HasLayerIDFilter && x.LayerID == CellEvents.NullLayerID);
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_ClearLayerState()
        {
            Test_CellPassAttributeFilter_ClearFilter_Aspect("Layer State",
                                                            x => { x.HasLayerStateFilter = true; },
                                                            x => x.HasLayerStateFilter,
                                                            x => { x.ClearLayerState(); },
                                                            x => !x.HasLayerStateFilter);
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_Assign()
        {
          CellPassAttributeFilter filter1 = new CellPassAttributeFilter();
          CellPassAttributeFilter filter2 = new CellPassAttributeFilter();
          filter1.ClearFilter();
          filter2.ClearFilter();
          filter1.MaterialTemperatureMin = 10;
          filter1.MaterialTemperatureMax = 30;
          filter1.HasTemperatureRangeFilter = true;
          filter1.FilterTemperatureByLastPass = true;
          filter1.MachinesList = new Guid[]{Guid.NewGuid(), Guid.NewGuid(), };

          //Assert.Equal(-1, filter1.CompareTo(filter2));
          filter2.Assign(filter1);
          //Assert.Equal(0, filter1.CompareTo(filter2));

          filter1.Should().BeEquivalentTo(filter2);
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_ClearCompactionMachineOnlyRestriction()
        {
            Test_CellPassAttributeFilter_ClearFilter_Aspect("Compaction only",
                                                            x => { x.HasCompactionMachinesOnlyFilter = true; },
                                                            x => x.HasCompactionMachinesOnlyFilter,
                                                            x => { x.ClearCompactionMachineOnlyRestriction(); },
                                                            x => !x.HasCompactionMachinesOnlyFilter);
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_ClearMachineDirection()
        {
            Test_CellPassAttributeFilter_ClearFilter_Aspect("MachineDirection",
                                                            x => { x.HasMachineDirectionFilter = true; x.MachineDirection = MachineDirection.Reverse; },
                                                            x => x.HasMachineDirectionFilter,
                                                            x => { x.ClearMachineDirection(); },
                                                            x => !x.HasMachineDirectionFilter && x.MachineDirection == MachineDirection.Unknown);
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_ClearMachines()
        {
          Test_CellPassAttributeFilter_ClearFilter_Aspect("Machines",
            x => { x.HasMachineFilter = true; },
            x => x.HasMachineFilter,
            x => { x.ClearMachines(); },
            x => !x.HasMachineFilter && x.MachinesList.Length == 0);
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_ClearMinElevationMapping()
        {
            Test_CellPassAttributeFilter_ClearFilter_Aspect("ElevationMappingMode",
                                                            x => { x.HasElevationMappingModeFilter = true; },
                                                            x => x.HasElevationMappingModeFilter,
                                                            x => { x.ClearMinElevationMapping(); },
                                                            x => !x.HasElevationMappingModeFilter && x.ElevationMappingMode == ElevationMappingMode.LatestElevation);
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_ClearPassType()
        {
            Test_CellPassAttributeFilter_ClearFilter_Aspect("PassType",
                                                            x => { x.HasPassTypeFilter = true; x.PassTypeSet = PassTypeSet.Front | PassTypeSet.Rear; },
                                                            x => x.HasPassTypeFilter,
                                                            x => { x.ClearPassType(); },
                                                            x => !x.HasPassTypeFilter && x.PassTypeSet == PassTypeSet.None);
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_ClearPositioningTech()
        {
            Test_CellPassAttributeFilter_ClearFilter_Aspect("PositioningTech",
                                                            x => { x.HasPositioningTechFilter = true; x.PositioningTech = PositioningTech.GPS; },
                                                            x => x.HasPositioningTechFilter,
                                                            x => { x.ClearPositioningTech(); },
                                                            x => !x.HasPositioningTechFilter);
        }

        [Fact]
        public void Test_CellPassAttributeFilter_ClearSurveyedSurfaceExclusionList()
        {
          Test_CellPassAttributeFilter_ClearFilter_Aspect("SurveyedSurfaceExclusionList",
            x => { x.SurveyedSurfaceExclusionList = new [] {Guid.Empty}; },
            x => x.SurveyedSurfaceExclusionList.Length == 1 && x.SurveyedSurfaceExclusionList[0] == Guid.Empty,
            x => { x.ClearSurveyedSurfaceExclusionList(); },
            x => (x.SurveyedSurfaceExclusionList?.Length ?? 0) == 0);
        }

        [Fact()]
        public void Test_CellPassAttributeFilter_ClearTime()
        {
            Test_CellPassAttributeFilter_ClearFilter_Aspect("Time",
                                                            x => { x.HasTimeFilter = true;
                                                                   x.StartTime = DateTime.SpecifyKind(new DateTime(2000, 1, 1), DateTimeKind.Utc);
                                                                   x.EndTime = DateTime.SpecifyKind(new DateTime(2000, 1, 2), DateTimeKind.Utc); },
                                                            x => x.HasTimeFilter,
                                                            x => { x.ClearTime(); },
                                                            x => !x.HasTimeFilter && 
                                                                  x.StartTime == TRex.Common.Consts.MIN_DATETIME_AS_UTC 
                                                                  && x.EndTime == TRex.Common.Consts.MAX_DATETIME_AS_UTC);
        }

        [Fact(Skip = "Not Implemented")]
        public void Test_CellPassAttributeFilter_FilterPass()
        {
            Assert.True(false);

        }

        [Fact(Skip = "Not Implemented")]
        public void Test_CellPassAttributeFilter_FilterPassTest1()
        {
            Assert.True(false);

        }

        [Fact(Skip = "Not Implemented")]
        public void Test_CellPassAttributeFilter_FilterPassUsingElevationRange()
        {
            Assert.True(false);

        }

        [Fact(Skip = "Not Implemented")]
        public void Test_CellPassAttributeFilter_FilterPassUsingTimeOnly()
        {
            Assert.True(false);

        }

        [Fact(Skip = "Not Implemented")]
        public void Test_CellPassAttributeFilter_FilterPass_MachineEvents()
        {
            Assert.True(false);

        }

        [Fact(Skip = "Not Implemented")]
        public void Test_CellPassAttributeFilter_FilterPass_NoMachineEvents()
        {
            Assert.True(false);

        }

        [Fact(Skip = "Not Implemented")]
        public void Test_CellPassAttributeFilter_FiltersElevation()
        {
            Assert.True(false);

        }

        [Fact(Skip = "Not Implemented")]
        public void Test_CellPassAttributeFilter_FiltersElevationTest1()
        {
            Assert.True(false);

        }

        [Fact(Skip = "Not Implemented")]
        public void Test_CellPassAttributeFilter_FilterSinglePass()
        {
            Assert.True(false);

        }

        [Fact(Skip = "Not Implemented")]
        public void Test_CellPassAttributeFilter_InitialiseFilteringForCell()
        {
            Assert.True(false);

        }

        [Fact(Skip = "Not Implemented")]
        public void Test_CellPassAttributeFilter_InitialiseElevationRangeFilter()
        {
            Assert.True(false);

        }

        [Fact()]
        public void Test_CellPassAttributeFilter_InitialiseMachineIDsSet()
        {
            CellPassAttributeFilter filter = new CellPassAttributeFilter();

            Assert.False(filter.HasMachineFilter, "Machine filter set");
            Assert.True(filter.MachinesList == null || filter.MachinesList.Length == 0, "Machine filter contains machines");
        }

        [Fact]
        public void Test_CellPassAttributeFilter_IsTimeRangeFilter()
        {
            CellPassAttributeFilter filter = new CellPassAttributeFilter();

            Assert.False(filter.IsTimeRangeFilter(), "Time range set");

            filter.HasTimeFilter = true;
            Assert.False(filter.IsTimeRangeFilter(), "Time range set");

            filter.StartTime = DateTime.SpecifyKind(new DateTime(2000, 1, 1), DateTimeKind.Utc);
            Assert.True(filter.IsTimeRangeFilter(), "Time range not set");
        }

        [Fact(Skip = "Not Implemented")]
        public void Test_CellPassAttributeFilter_FilterMultiplePasses()
        {
            Assert.True(false);

        }

        [Fact(Skip = "Not Implemented")]
        public void Test_CellPassAttributeFilter_GetMachineIDsSet()
        {
          var machineID1 = Guid.NewGuid();
          var machineID2 = Guid.NewGuid();
          var siteModel = new TRex.SiteModels.SiteModel(StorageMutability.Immutable);
          siteModel.Machines.Add(new Machine
          {
            ID = machineID1,
            InternalSiteModelMachineIndex = 0
          });
          siteModel.Machines.Add(new Machine
          {
            ID = machineID2,
            InternalSiteModelMachineIndex = 5
          });

          var data = new FilterSet(
            new CombinedFilter
            {
              AttributeFilter =
              {
               SiteModel = siteModel,
               MachinesList = new Guid[]{machineID1, machineID2}
              }
            });
      Assert.True(false);

        }
        
    /* Possible obsolete functioality
    [Fact]
    public void Test_CellPassAttributeFilter_MachineIDListsComparison()
    {
      short[] list1 = null;
      short[] list2 = {1, 2};
      short[] list3 = {2, 3};
      short[] list4 = {2, 3};

      CellPassAttributeFilter.MachineIDListsComparison(list1, list1).Should().Be(0);
      CellPassAttributeFilter.MachineIDListsComparison(list1, list2).Should().Be(0);
      CellPassAttributeFilter.MachineIDListsComparison(list2, list1).Should().Be(0);

      CellPassAttributeFilter.MachineIDListsComparison(list2, list3).Should().Be(-1);
      CellPassAttributeFilter.MachineIDListsComparison(list3, list2).Should().Be(1);
      CellPassAttributeFilter.MachineIDListsComparison(list3, list4).Should().Be(1);
    }
    */

    //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< Tests involving cell passes >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>



  }
}

