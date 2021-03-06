﻿using VSS.TRex.Geometry;
using VSS.TRex.TAGFiles.Classes.States;
using VSS.TRex.TAGFiles.Types;

namespace VSS.TRex.TAGFiles.Classes.ValueMatcher.Ordinates
{
    public class TAGWheelOrdinateValueMatcher : TAGValueMatcher
    {
        public TAGWheelOrdinateValueMatcher()
        {
        }

        private static readonly string[] valueTypes = { TAGValueNames.kTagFileEastingWheelTag, TAGValueNames.kTagFileNorthingWheelTag, TAGValueNames.kTagFileElevationWheelTag };

        public override string[] MatchedValueTypes() => valueTypes;

        public override bool ProcessIntegerValue(TAGValueMatcherState state, TAGProcessorStateBase valueSink,
          TAGDictionaryItem valueType, int value)
        {
            // Position value is integer number of millimeters offset from the current position

            if (!state.HaveSeenAnAbsoluteWheelPosition)
            {
                return false;
            }

            if (valueType.Name == TAGValueNames.kTagFileEastingWheelTag)
            {
                if (state.WheelSide == TAGValueSide.Left)
                {
                    valueSink.DataWheelLeft.X += (double)value / 1000;
                }
                else
                {
                    valueSink.DataWheelRight.X += (double)value / 1000;
                }

                return true;
            }

            if (valueType.Name == TAGValueNames.kTagFileNorthingWheelTag)
            {
                if (state.WheelSide == TAGValueSide.Left)
                {
                    valueSink.DataWheelLeft.Y += (double)value / 1000;
                }
                else
                {
                    valueSink.DataWheelRight.Y += +(double)value / 1000;
                }

                return true;
            }

            if (valueType.Name == TAGValueNames.kTagFileElevationWheelTag)
            {
                if (state.WheelSide == TAGValueSide.Left)
                {
                    valueSink.DataWheelLeft.Z += (double)value / 1000;
                }
                else
                {
                    valueSink.DataWheelRight.Z += (double)value / 1000;
                }

                return true;
            }

            return false;
        }

        public override bool ProcessDoubleValue(TAGValueMatcherState state, TAGProcessorStateBase valueSink,
          TAGDictionaryItem valueType, double value)
        {
            state.HaveSeenAnAbsoluteWheelPosition = true;

            if (valueType.Name == TAGValueNames.kTagFileEastingWheelTag)
            {
                if (state.WheelSide == TAGValueSide.Left)
                {
                    valueSink.DataWheelLeft.X = value;
                }
                else
                {
                    valueSink.DataWheelRight.X = value;
                }

                return true;
            }

            if (valueType.Name == TAGValueNames.kTagFileNorthingWheelTag)
            {
                if (state.WheelSide == TAGValueSide.Left)
                {
                    valueSink.DataWheelLeft.Y = value;
                }
                else
                {
                    valueSink.DataWheelRight.Y = value;
                }

                return true;
            }

            if (valueType.Name == TAGValueNames.kTagFileElevationWheelTag)
            {
                if (state.WheelSide == TAGValueSide.Left)
                {
                    valueSink.DataWheelLeft.Z = value;
                }
                else
                {
                    valueSink.DataWheelRight.Z = value;
                }

                return true;
            }

            return false;
        }

        public override bool ProcessEmptyValue(TAGValueMatcherState state, TAGProcessorStateBase valueSink,
          TAGDictionaryItem valueType)
        {
            state.HaveSeenAnAbsoluteWheelPosition = false;

            if (state.WheelSide == TAGValueSide.Left)
            {
                valueSink.DataWheelLeft = XYZ.Null;
            }
            else
            {
                valueSink.DataWheelRight = XYZ.Null;
            }

            return true;
        }
    }
}

