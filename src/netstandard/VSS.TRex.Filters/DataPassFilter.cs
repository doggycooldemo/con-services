﻿using System;
using VSS.TRex.Cells;
using VSS.TRex.Common.CellPasses;
using VSS.TRex.Filters.Models;
using VSS.TRex.Profiling.Interfaces;
using VSS.TRex.Types;

namespace VSS.TRex.Filters
{
    /// <summary>
    /// DataPassFilter describes a filter that controls how the information held in the
    /// server data grid is prepared before returning it to the client to fulfill client
    /// grid data requests
    /// </summary>
    public abstract class DataPassFilter
    {
        /// <summary>
        /// RequestedGridDataType stores the type of grid data being requested at
        /// the time this filter is asked filter cell passes.
        /// </summary>
        public GridDataType RequestedGridDataType { get; set; } = GridDataType.All;

        public abstract bool FilterPass(ref CellPass passValue);
        public abstract bool FilterPass(ref FilteredPassData passValue);

        public DataPassFilter()
        {
        }

        public bool HasTimeFilter { get; set; }
        public bool HasMachineFilter { get; set; }
        public bool HasMachineDirectionFilter { get; set; }
        public bool HasDesignFilter { get; set; }
        public bool HasVibeStateFilter { get; set; }
        public bool HasLayerStateFilter { get; set; }
        public bool HasMinElevMappingFilter { get; set; }
        public bool HasElevationTypeFilter { get; set; }
        public bool HasGCSGuidanceModeFilter { get; set; } 
        public bool HasGPSAccuracyFilter { get; set; }
        public bool HasGPSToleranceFilter { get; set; }
        public bool HasPositioningTechFilter { get; set; } 
        public bool HasLayerIDFilter { get; set; }
        public bool HasElevationRangeFilter { get; set; }
        public bool HasPassTypeFilter { get; set; }

        public virtual bool IsTimeRangeFilter() => false;

        public bool HasCompactionMachinesOnlyFilter { get; set; }

        public bool HasTemperatureRangeFilter { get; set; }

        public bool FilterTemperatureByLastPass { get; set; }

        public bool HasPassCountRangeFilter { get; set; }

        public bool ExcludeSurveyedSurfaces()
        {
            return HasDesignFilter || HasMachineFilter || HasMachineDirectionFilter ||
            HasVibeStateFilter || HasCompactionMachinesOnlyFilter ||
            HasGPSAccuracyFilter || HasPassTypeFilter || HasTemperatureRangeFilter;
        }

        public string ActiveFiltersText() => "Not implemented";

        private bool PassIsAcceptable(ref CellPass PassValue)
        {
            // Certain types of grid attribute data requests may need us to select
            // a pass that is not the latest pass in the pass list. Such an instance is
            // when request CCV value where null CCV values are passed over in favour of
            // non-null CCV values in passes that are older in the pass list for the cell.

            // Important: Also see the CalculateLatestPassDataForPassStack() function in
            // TICServerSubGridTreeLeaf.CalculateLatestPassGrid() to ensure that the logic
            // here is consistent (or at least not contradictory) with the logic here.
            // The checks are duplicated as there may be different logic applied to the
            // selection of the 'latest' pass from a cell pass state versus selection of
            // an appropriate filtered pass given other filtering criteria in play.

            switch (RequestedGridDataType)
            {
                case GridDataType.CCV:
                    return PassValue.CCV != CellPassConsts.NullCCV;
                case GridDataType.MDP:
                    return PassValue.MDP != CellPassConsts.NullMDP;
                case GridDataType.RMV:
                    return PassValue.RMV != CellPassConsts.NullRMV;
                case GridDataType.Frequency:
                    return PassValue.Frequency != CellPassConsts.NullFrequency;
                case GridDataType.Amplitude:
                    return PassValue.Amplitude != CellPassConsts.NullAmplitude;
                case GridDataType.Temperature: 
                    return PassValue.MaterialTemperature != CellPassConsts.NullMaterialTemperatureValue;
                case GridDataType.TemperatureDetail:
                  return PassValue.MaterialTemperature != CellPassConsts.NullMaterialTemperatureValue;
                case GridDataType.GPSMode:
                    return PassValue.gpsMode != CellPassConsts.NullGPSMode;
                default:
                    return true;
            }
        }

        public bool AnyFilterSelections { get; set; }

        public bool AnyMachineEventFilterSelections { get; set; }

        public bool AnyNonMachineEventFilterSelections { get; set; }

        /// <summary>
        /// Performs operations that prepares the filter for active use. Prepare() must be called prior to
        /// active use of the filter.
        /// </summary>
        public virtual void Prepare()
        {
            // Set the filter internal flag to indicate that there is at least one attribute filter configured.
            AnyFilterSelections =
                HasCompactionMachinesOnlyFilter ||
                HasDesignFilter ||
                HasElevationRangeFilter ||
                HasElevationTypeFilter ||
                HasGCSGuidanceModeFilter ||
                HasGPSAccuracyFilter ||
                HasGPSToleranceFilter ||
                HasLayerIDFilter ||
                HasLayerStateFilter ||
                HasMachineDirectionFilter ||
                HasMachineFilter ||
                HasMinElevMappingFilter ||
                HasPassTypeFilter ||
                HasPositioningTechFilter ||
                HasTimeFilter ||
                HasVibeStateFilter ||
                HasTemperatureRangeFilter ||
                HasPassCountRangeFilter;

            AnyMachineEventFilterSelections =
                HasDesignFilter ||
                HasVibeStateFilter ||
                HasMachineDirectionFilter ||
                HasMinElevMappingFilter ||
                HasGCSGuidanceModeFilter ||
                HasGPSAccuracyFilter ||
                HasGPSToleranceFilter ||
                HasPositioningTechFilter ||
                HasLayerIDFilter ||
                HasPassTypeFilter;

            AnyNonMachineEventFilterSelections =
                HasTimeFilter ||
                HasMachineFilter ||
                HasElevationRangeFilter ||
                HasCompactionMachinesOnlyFilter ||
                HasTemperatureRangeFilter;
        }

        // FilterSinglePass selects a single passes from the list of passes in
        // <PassValues> where <PassValues> contains the entire list of passes for
        // a cell in the database.
        public bool FilterSinglePass(CellPass[] passValues,
                                     int passValueCount,
                                     bool wantEarliestPass,
                                     ref FilteredSinglePassInfo filteredPassInfo,
                                     object /* IProfileCell */ profileCell,
                                     bool performAttributeSubFilter)
        {
            bool Accept;
            bool Result = false;

            if (passValueCount == 0)
            {
                return false;
            }

            bool CheckAttributes = performAttributeSubFilter && AnyFilterSelections;
            int AcceptedIndex = -1;

            if (wantEarliestPass)
            {
                for (int I = 0; I < passValueCount; I++)
                {
                    if (CheckAttributes && !FilterPass(ref passValues[I]))
                    {
                        return false;
                    }

                    Accept = profileCell == null 
                      ? PassIsAcceptable(ref passValues[I]) 
                      : PassIsAcceptable(ref passValues[I]) && !((IProfileCell)profileCell).IsInSupersededLayer(passValues[I]);

                    if (Accept)
                    {
                        AcceptedIndex = I;
                        Result = true;
                        break;
                    }
                }
            }
            else
            {
                for (int I = passValueCount - 1; I >= 0; I--)
                {
                    if (CheckAttributes && !FilterPass(ref passValues[I]))
                    {
                        return false;
                    }

                    Accept = profileCell == null
                      ? PassIsAcceptable(ref passValues[I])
                      : PassIsAcceptable(ref passValues[I]) && !((IProfileCell)profileCell).IsInSupersededLayer(passValues[I]);

                    if (Accept)
                    {
                        AcceptedIndex = I;
                        Result = true;
                        break;
                    }
                }
            }

            if (Result)
            {
                filteredPassInfo.FilteredPassData.FilteredPass = passValues[AcceptedIndex];
                filteredPassInfo.FilteredPassData.TargetValues.TargetCCV = 1;
                filteredPassInfo.FilteredPassData.TargetValues.TargetMDP = 1;
                filteredPassInfo.FilteredPassData.TargetValues.TargetPassCount = 1;
                filteredPassInfo.PassCount = passValueCount;
            }

            return Result;
        }

        // FilterMultiplePasses selects a set of passes from the list of passes from the
        // list of passes in <PassValues> where <PassValues> contains the entire
        // list of passes for a cell in the database.
        // in this case, all passes are automatically selected.
        public virtual bool FilterMultiplePasses(CellPass[] passValues,
                                                 int passValueCount,
                                                 ref FilteredMultiplePassInfo FilteredPassInfo)
        {
            if (passValueCount == 0) // There's nothing to do
            {
                return true;
            }
            // Note: We make an independent copy of the Passes array for the cell.
            // Simply doing CellPasses = Passes just copies a reference to the
            // array of passes.

            for (int I = 0; I < passValueCount; I++)
            {
                FilteredPassInfo.AddPass(passValues[I]);
            }

            return true;
        }

      /// <summary>
      /// FilterSinglePass selects a single passes from the list of passes in
      /// PassValues where PassValues contains the entire list of passes for
      /// a cell in the database.
      /// </summary>
      /// <param name="filteredPassValues"></param>
      /// <param name="passValueCount"></param>
      /// <param name="wantEarliestPass"></param>
      /// <param name="filteredPassInfo"></param>
      /// <param name="profileCell"></param>
      /// <param name="performAttributeSubFilter"></param>
      /// <returns></returns>
      public bool FilterSinglePass(FilteredPassData[] filteredPassValues,
                                   int passValueCount,
                                   bool wantEarliestPass,
                                   ref FilteredSinglePassInfo filteredPassInfo,
                                   object /*IProfileCell*/ profileCell,
                                   bool performAttributeSubFilter)
        {
            bool Accept;
            bool Result = false;

            if (passValueCount == 0)
            {
                return false;
            }

            bool CheckAttributes = performAttributeSubFilter && AnyFilterSelections;
            int AcceptedIndex = -1;

            if (wantEarliestPass)
            {
                for (int I = 0; I < passValueCount; I++)
                {
                    if (CheckAttributes && !FilterPass(ref filteredPassValues[I]))
                    {
                        return false;
                    }

                    Accept = profileCell == null
                      ? PassIsAcceptable(ref filteredPassValues[I].FilteredPass)
                      : PassIsAcceptable(ref filteredPassValues[I].FilteredPass) && !((IProfileCell)profileCell).IsInSupersededLayer(filteredPassValues[I].FilteredPass);

                    if (Accept)
                    {
                        AcceptedIndex = I;
                        Result = true;
                        break;
                    }
                }
            }
            else
            {
                for (int I = passValueCount - 1; I >= 0; I--)
                {
                    if (CheckAttributes && !FilterPass(ref filteredPassValues[I]))
                    {
                        return false;
                    }

                    Accept = profileCell == null
                      ? PassIsAcceptable(ref filteredPassValues[I].FilteredPass)
                      : PassIsAcceptable(ref filteredPassValues[I].FilteredPass) && !((IProfileCell)profileCell).IsInSupersededLayer(filteredPassValues[I].FilteredPass);

                    if (Accept)
                    {
                        AcceptedIndex = I;
                        Result = true;
                        break;
                    }
                }
            }

            if (Result)
            {
                filteredPassInfo.FilteredPassData = filteredPassValues[AcceptedIndex];
                filteredPassInfo.PassCount = passValueCount;
            }
            return Result;
        }
    }
}
