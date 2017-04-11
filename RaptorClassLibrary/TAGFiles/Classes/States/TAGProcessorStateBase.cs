﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSS.VisionLink.Raptor;
using VSS.VisionLink.Raptor.Geometry;
using VSS.VisionLink.Raptor.Common;
using VSS.VisionLink.Raptor.Types;
using VSS.VisionLink.Raptor.TAGFiles.Types;

namespace VSS.VisionLink.Raptor.TAGFiles.Classes
{
    /// <summary>
    /// TAGProcessorStateBase implements a basic TAG value reader sink with all the
    /// state informaiton read from tag files representing as-built and compaction
    /// recorded information.
    /// </summary>
    public class TAGProcessorStateBase
    {
        ////////////////////////Private properties
        private bool HaveSeenFirstDataTime { get; set; } = false;
        private DateTime DataTimePrevious { get; set; } = DateTime.MinValue;

        private XYZ LeftPoint = XYZ.Null;
        private XYZ RightPoint = XYZ.Null;
        private XYZ LeftTrackPoint = XYZ.Null;
        private XYZ RightTrackPoint = XYZ.Null;
        private XYZ LeftWheelPoint = XYZ.Null;
        private XYZ RightWheelPoint= XYZ.Null;
        private XYZ LeftRearPoint = XYZ.Null;
        private XYZ RightRearPoint = XYZ.Null;

        // Declarations that hold values from read records

        private string _Design = "";
        //        private string _MachineID = "";
        //        private byte _MachineType = 0;
        //        private string _HardwareID = "";
        //        private uint _Sequence = 0;

        private short _ICCCVTargetValue = CellPass.NullCCV;
        private short _ICMDPTargetValue = CellPass.NullMDP;
        private short _ICCCATargetValue = CellPass.NullCCA;
        private ushort _ICPassTargetValue = 0;
        private ushort _ICLayerIDValue = CellPass.NullLayerID;

        private MachineGear _ICGear = CellPass.NullMachineGear;
        private byte _ICMode = ICModeFlags.ICUnknownInvalidMC024SensorFlag;
        private byte _ICSonic3D = CellPass.Null3DSonic;

        private CompactionSensorType _ICSensorType = CompactionSensorType.NoSensor;
        private ushort _ICTempWarningLevelMinValue = CellPass.NullMaterialTemp;
        private ushort _ICTempWarningLevelMaxValue = CellPass.NullMaterialTemp;

        private short _RMVJumpThreshold = CellPass.NullRMV;

        // Proofing runs declarations...
        private string _StartProofing = "";    // Proofing run name...
        private DateTime _StartProofingDataTime = DateTime.MinValue;

        // Declarations for processing state information
        private float _ICTargetLiftThickness = CellPass.NullOverridingTargetLiftThicknessValue;

        // FApplicationVersion is the version of the application reported in the
        // TAG file returned by the machine.
        private string _ApplicationVersion = "";

        // The control state members contain the control state flags set for five different
        // automatics controls supported by the GCS900 machine control system
        int _ControlStateLeftLift = MachineControlStateFlags.NullGCSControlState;
        int _ControlStateRightLift = MachineControlStateFlags.NullGCSControlState;
        int _ControlStateLift = MachineControlStateFlags.NullGCSControlState;
        int _ControlStateTilt = MachineControlStateFlags.NullGCSControlState;
        int _ControlStateSideShift = MachineControlStateFlags.NullGCSControlState;

        // FAutomaticesMode records the machine automatic control state as defined by
        // the 5 GCS900 control state flag sets. It is currently defined as a simple
        // on/off switch. The UpdateAutomaticsMode method examines the individial
        // control states and sets the value of this accordingly.
        MachineAutomaticsMode _AutomaticsMode = MachineAutomaticsMode.Unknown;

        private byte _UTMZone = CellPass.NullUTMZone;
        private CoordinateSystemType _CSType = CoordinateSystemType.NoCoordSystem;

        private bool _ResearchData = false;
        private bool _UsingCCA = false;

        ////////////////////////Private procedures

        // CalculateMachineSpeed calculates the speed of the machine in meters per second
        private double CalculateMachineSpeed()
        {
            if (LeftPoint.IsNull || RightPoint.IsNull || DataLeft.IsNull || DataRight.IsNull)
            {
                return Consts.NullDouble;
            }

            XYZ CentrePointFrom = (LeftPoint + RightPoint) * 0.5;
            XYZ CentrePointTo = (DataLeft + DataRight) * 0.5;

            double DistanceTraveled = XYZ.Get3DLength(CentrePointFrom, CentrePointTo); // meters converted to kilometers...
            double TravelTime = ((TimeSpan)(DataTime - FirstDataTime)).TotalMilliseconds / 1000;   // milliseconds converted to seconds...

            return TravelTime > 0 ? DistanceTraveled / TravelTime : 0.0;
        }

        private bool GetLLHReceived() => (LLHLat != Consts.NullDouble) && (LLHLon != Consts.NullDouble) && (LLHHeight != Consts.NullDouble);

        private bool GetGPSBaseLLHReceived => (GPSBaseLat != Consts.NullDouble) && (GPSBaseLon != Consts.NullDouble) && (GPSBaseHeight != Consts.NullDouble);

        ///////////////////////////////////////// Protected properties
        protected bool HaveFirstEpoch { get; set; } = false;
        protected bool HaveFirstRearEpoch { get; set; } = false;
        protected bool HaveFirstTrackEpoch { get; set; } = false;
        protected bool HaveFirstWheelEpoch { get; set; } = false;

        // FWorkerID is the ID of this instance of a ST processor. It is used when
        // running multiple processors on different threads. It defaults to -1
//        protected int WorkerID { get; set; } = -1;

        ///////////////////// Protected procedures

        protected virtual void InitialiseAttributeAccumulators()
        {
            ICMachineSpeedValues.Add(DateTime.MinValue, Consts.NullDouble);
            ICCCVValues.Add(DateTime.MinValue, CellPass.NullCCV);
            ICRMVValues.Add(DateTime.MinValue, CellPass.NullRMV);
            ICFrequencys.Add(DateTime.MinValue, CellPass.NullFrequency);
            ICAmplitudes.Add(DateTime.MinValue, CellPass.NullAmplitude);
            GPSModes.Add(DateTime.MinValue, CellPass.NullGPSMode);

            // We will assume that the absence of an OnGround flag in the tag file shall
            // default to true wrt to the processing of the file.
            OnGrounds.Add(DateTime.MinValue, OnGroundState.YesLegacy);

            AgeOfCorrections.Add(DateTime.MinValue, (byte)0);

            VolkelMeasureRanges.Add(DateTime.MinValue, CellPass.NullVolkelMeasRange);
            VolkelMeasureUtilRanges.Add(DateTime.MinValue, CellPass.NullVolkelMeasUtilRange);
            ICMDPValues.Add(DateTime.MinValue, CellPass.NullMDP);
            ICTemperatureValues.Add(DateTime.MinValue, CellPass.NullMaterialTemp);
            ICCCAValues.Add(DateTime.MinValue, CellPass.NullCCA);
        }

        protected virtual void DiscardAllButLatestAttributeAccumulatorValues()
        {
            ICMachineSpeedValues.DiscardAllButLatest();
            ICCCVValues.DiscardAllButLatest();
            ICRMVValues.DiscardAllButLatest();
            ICFrequencys.DiscardAllButLatest();
            ICAmplitudes.DiscardAllButLatest();
            GPSModes.DiscardAllButLatest();
            OnGrounds.DiscardAllButLatest();
            AgeOfCorrections.DiscardAllButLatest();
            VolkelMeasureRanges.DiscardAllButLatest();
            VolkelMeasureUtilRanges.DiscardAllButLatest();
            ICMDPValues.DiscardAllButLatest();
            ICCCAValues.DiscardAllButLatest();
            ICTemperatureValues.DiscardAllButLatest();
        }

        protected virtual void SetDataTime(DateTime value)
        {
            _DataTime = value;

            if (!HaveSeenFirstDataTime)
            {
                HaveSeenFirstDataTime = true;
                _FirstDataTime = _DataTime;
            }
        }
        protected virtual void SetDesign(string value) => _Design = value;
        protected virtual void SetICMode(byte value) => _ICMode = value;
        protected virtual void SetICGear(MachineGear value)
        {
            _ICGear = value;

            if (value != MachineGear.SensorFailedDeprecated)
            {
                GearValueReceived = true;
            }
        }
        protected virtual void SetICSonic3D(byte value) => _ICSonic3D = value;
        protected virtual void SetICCCVTargetValue(short value) => _ICCCVTargetValue = value;
        protected virtual void SetICMDPTargetValue(short value) => _ICMDPTargetValue = value;
        protected virtual void SetICPassTargetValue(ushort value) => _ICPassTargetValue = value;
        protected virtual void SetICLayerIDValue(ushort value) => _ICLayerIDValue = value;
        protected virtual void SetStartProofingDataTime(DateTime value) => _StartProofingDataTime = value;
        protected virtual void SetStartProofing(string value) => _StartProofing = value;
        protected virtual void SetICTargetLiftThickness(float value) => _ICTargetLiftThickness = value;
        protected virtual void SetApplicationVersion(string value) => _ApplicationVersion = value;
        protected virtual void SetAutomaticsMode(MachineAutomaticsMode value) => _AutomaticsMode = value;
        protected virtual void SetRMVJumpThresholdValue(short value) => _RMVJumpThreshold = value;
        protected virtual void SetICSensorType(CompactionSensorType value) => _ICSensorType = value;
        protected virtual void SetICTempWarningLevelMinValue(ushort value) => _ICTempWarningLevelMinValue = value;
        protected virtual void SetICTempWarningLevelMaxValue(ushort value) => _ICTempWarningLevelMaxValue = value;

        protected virtual void SetICCCATargetValue(short value) => _ICCCATargetValue = value;
        protected virtual void SetUTMZone(byte value) => _UTMZone = value;
        protected virtual void SetCSType(CoordinateSystemType value) => _CSType = value;

        protected void UpdateAutomaticsMode()
        {
            const int kMachineIsInAutomaticsModeFlags = MachineControlStateFlags.GCSControlStateAuto |
                                                        MachineControlStateFlags.GCSControlStateInActiveAuto |
                                                        MachineControlStateFlags.GCSControlStateAutoValueNotDriving;

            MachineAutomaticsMode OldAutomaticsModeState, NewAutomaticsModeState;

            OldAutomaticsModeState = _AutomaticsMode;

            //   implement unknown automatics state for initialisation purposes

            if (((_ControlStateLeftLift & kMachineIsInAutomaticsModeFlags) != 0) ||
              ((_ControlStateRightLift & kMachineIsInAutomaticsModeFlags) != 0) ||
               ((_ControlStateLift & kMachineIsInAutomaticsModeFlags) != 0) ||
               ((_ControlStateTilt & kMachineIsInAutomaticsModeFlags) != 0) ||
               ((_ControlStateSideShift & kMachineIsInAutomaticsModeFlags) != 0))
            {
                NewAutomaticsModeState = MachineAutomaticsMode.Automatics;
            }
            else
            {
                NewAutomaticsModeState = MachineAutomaticsMode.Manual;
            }

            if (OldAutomaticsModeState != NewAutomaticsModeState)
            {
                _AutomaticsMode = NewAutomaticsModeState;
            }
        }

        public void SetControlStateLeftLift(int value)
        {
            _ControlStateLeftLift = value;
            UpdateAutomaticsMode();
        }

        public void SetControlStateLift(int value)
        {
            _ControlStateLift = value;
            UpdateAutomaticsMode();
        }

        public void SetControlStateRightLift(int value)
        {
            _ControlStateRightLift = value;
            UpdateAutomaticsMode();
        }

        public void SetControlStateSideShift(int value)
        {
            _ControlStateSideShift = value;
            UpdateAutomaticsMode();
        }

        public void SetControlStateTilt(int value)
        {
            _ControlStateTilt = value;
            UpdateAutomaticsMode();
        }

        public virtual void SetMachineDirection(MachineDirection value)
        {
            if (!GearValueReceived)
            {
                if (value == MachineDirection.Forward)
                {
                    _ICGear = MachineGear.Forward;
                }
                else
                if (value == MachineDirection.Reverse)
                {
                    _ICGear = MachineGear.Reverse;
                }
            }
        }

        public MachineDirection GetMachineDirection()
        {
            if (_ICGear == MachineGear.Forward ||
                _ICGear == MachineGear.Forward2 ||
                _ICGear == MachineGear.Forward3 ||
                _ICGear == MachineGear.Forward4 ||
                _ICGear == MachineGear.Forward5)
            {
                return MachineDirection.Forward;
            }
            else
            if (_ICGear == MachineGear.Reverse ||
                _ICGear == MachineGear.Reverse2 ||
                _ICGear == MachineGear.Reverse3 ||
                _ICGear == MachineGear.Reverse4 ||
                _ICGear == MachineGear.Reverse5)
            {
                return MachineDirection.Reverse;
            }
            else
            {
                return MachineDirection.Unknown;
            }
        }

        public virtual void SetResearchData(bool value) => _ResearchData = value;
        public virtual void SetUsingCCA(bool value) => _UsingCCA = value;

        /// <summary>
        /// Determine if valid machine implement tip or front axle positions have been received
        /// </summary>
        /// <returns></returns>
        public bool HaveReceivedValidTipPositions => !(DataLeft.IsNull || DataRight.IsNull);

        /// <summary>
        /// Determine if valid machine track positions have been received
        /// </summary>
        /// <returns></returns>
        public bool HaveReceivedValidTrackPositions => !(DataTrackLeft.IsNull || DataTrackRight.IsNull);

        /// <summary>
        /// Determine if valid machine wheel positions have been received
        /// </summary>
        /// <returns></returns>
        public bool HaveReceivedValidWheelPositions => !(DataWheelLeft.IsNull || DataWheelRight.IsNull);

        /// <summary>
        /// Determine if valid rear axle positions have been received
        /// </summary>
        /// <returns></returns>
        public bool HaveReceivedValidRearPositions => !(DataRearLeft.IsNull || DataRearRight.IsNull);

        //////////////////// Public properties
        public List<UTMCoordPointPair> ConvertedBladePositions { get; set; } = new List<UTMCoordPointPair>();
        public List<UTMCoordPointPair> ConvertedRearAxlePositions { get; set; } = new List<UTMCoordPointPair>();
        public List<UTMCoordPointPair> ConvertedTrackPositions { get; set; } = new List<UTMCoordPointPair>();
        public List<UTMCoordPointPair> ConvertedWheelPositions { get; set; } = new List<UTMCoordPointPair>();

        // FFirstLeftPoint and FFirstRightPoint record the grid positions of the
        // first epoch in the TAG file. One use of this if for comparison against
        // a grid coordinate project boundary to see if the initial position lies
        // the project.
        public XYZ FirstAccurateLeftPoint = XYZ.Null;
        public XYZ FirstAccurateRightPoint = XYZ.Null;

        public bool HaveFirstAccurateGridEpochEndPoints { get; set; } = false;

        public int ProcessedEpochCount { get; set; } = 0;
        public int ProcessedCellPassesCount { get; set; } = 0;
        public int VisitedEpochCount { get; set; } = 0;

        public double OriginX { get; set; } = Consts.NullDouble;
        public double OriginY { get; set; } = Consts.NullDouble;
        public double OriginZ { get; set; } = Consts.NullDouble;
        public DateTime OriginTime { get; set; } = DateTime.MinValue;

        public short GPSWeekNumber { get; set; } = 0;
        public uint GPSWeekTime { get; set; } = 0;

        private DateTime _DataTime = DateTime.MinValue;
        public DateTime DataTime { get { return _DataTime; } set { SetDataTime(value); } }

        public DateTime _FirstDataTime;
        public DateTime FirstDataTime { get { return _FirstDataTime; } }

        public XYZ DataLeft = XYZ.Null;
        public XYZ DataRight = XYZ.Null;

        public XYZ DataTrackLeft = XYZ.Null;
        public XYZ DataTrackRight = XYZ.Null;


        public XYZ DataWheelLeft = XYZ.Null;
        public XYZ DataWheelRight = XYZ.Null;

        public XYZ DataRearLeft = XYZ.Null;
        public XYZ DataRearRight = XYZ.Null;

        public AccumulatedAttributes GPSModes { get; set; } = new AccumulatedAttributes();
        public AccumulatedAttributes OnGrounds { get; set; } = new AccumulatedAttributes();

        public AccumulatedAttributes AgeOfCorrections { get; set; } = new AccumulatedAttributes();

        //  ValidPosition is only used in terms of the most recent epoch and do not need to have the history of these
        // values maintained in a TAccumulatedAttributeList
        public byte ValidPosition { get; set; } = 0;

        public bool MinElevMapping { get; set; } = false;
        public byte InAvoidZone { get; set; } = 0;

        public GPSAccuracy GPSAccuracy { get; set; } = GPSAccuracy.Unknown;
        public short GPSAccuracyErrorLimit { get; set; } = CellPass.NullGPSTolerance;

        public MachineDirection MachineDirection { get { return GetMachineDirection(); } set { SetMachineDirection(value); } }

        public byte MachineType { get; set; } = CellPass.MachineTypeNull;

        public DateTime UserTimeOffset { get; set; } = DateTime.MinValue;

        public string Design { get { return _Design; } set { SetDesign(value); } }

        public string MachineID { get; set; } = String.Empty;
        public string HardwareID { get; set; } = String.Empty;
        public uint Sequence { get; set; } = 0;

        public AccumulatedAttributes ICCCVValues { get; set; } = new AccumulatedAttributes();
        public AccumulatedAttributes ICMachineSpeedValues { get; set; } = new AccumulatedAttributes();

        public short ICCCVTargetValue { get { return _ICCCVTargetValue; } set { SetICCCVTargetValue(value); } }

        public ushort ICPassTargetValue { get { return _ICPassTargetValue; } set { SetICPassTargetValue(value); } }
        public ushort ICLayerIDValue { get { return _ICLayerIDValue; } set { SetICLayerIDValue(value); } }

        public AccumulatedAttributes ICRMVValues { get; set; } = new AccumulatedAttributes();

        public short ICRMVJumpthreshold { get { return _RMVJumpThreshold; } set { SetRMVJumpThresholdValue(value); } }

        public AccumulatedAttributes ICFrequencys { get; set; } = new AccumulatedAttributes();
        public AccumulatedAttributes ICAmplitudes { get; set; } = new AccumulatedAttributes();

        public MachineGear ICGear { get { return _ICGear; } set { SetICGear(value); } }
        public byte ICSonic3D { get { return _ICSonic3D; } set { SetICSonic3D(value); } }
        public byte ICMode { get { return _ICMode; } set { SetICMode(value); } }


        public CompactionSensorType ICSensorType { get { return _ICSensorType; } set { SetICSensorType(value); } }

        public AccumulatedAttributes ICMDPValues { get; set; } = new AccumulatedAttributes();

        public short ICMDPTargetValue { get { return _ICMDPTargetValue; } set { SetICMDPTargetValue(value); } }

        public AccumulatedAttributes ICCCAValues { get; set; } = new AccumulatedAttributes();

        public short ICCCATargetValue { get { return _ICCCATargetValue; } set { SetICCCATargetValue(value); } }
        public AccumulatedAttributes ICTemperatureValues { get; set; } = new AccumulatedAttributes();

        public ushort ICTempWarningLevelMinValue { get { return _ICTempWarningLevelMinValue; } set { SetICTempWarningLevelMinValue(value); } }
        public ushort ICTempWarningLevelMaxValue { get { return _ICTempWarningLevelMaxValue; } set { SetICTempWarningLevelMaxValue(value); } }

        public AccumulatedAttributes VolkelMeasureRanges { get; set; } = new AccumulatedAttributes();
        public AccumulatedAttributes VolkelMeasureUtilRanges { get; set; } = new AccumulatedAttributes();

        public string ApplicationVersion { get { return _ApplicationVersion; } set { SetApplicationVersion(value); } }

        public double CalculatedMachineSpeed { get; set; } = Consts.NullDouble;

        public string StartProofing { get { return _StartProofing; } set { SetStartProofing(value); } }

        /// <summary>
        /// Proofing time is GPS time in milliseconds
        /// </summary>
        public uint StartProofingTime { get; set; } = 0;
        public short StartProofingWeek { get; set; } = 0;
        public string EndProofingName { get; set; } = "";

        public DateTime StartProofingDataTime { get { return _StartProofingDataTime; } set { SetStartProofingDataTime(value); } }

        public bool HaveSeenAProofingStart { get; set; } = false;

        public float ICTargetLiftThickness { get { return _ICTargetLiftThickness; } set { SetICTargetLiftThickness(value); } }

        public BoundingWorldExtent3D ProofingRunExtent = BoundingWorldExtent3D.Inverted();
        public BoundingWorldExtent3D DesignExtent = BoundingWorldExtent3D.Inverted();


        public int ControlStateLeftLift { get { return _ControlStateLeftLift; } set { SetControlStateLeftLift(value); } }
        public int ControlStateRightLift { get { return _ControlStateRightLift; } set { SetControlStateRightLift(value); } }
        public int ControlStateLift { get { return _ControlStateLift; } set { SetControlStateLift(value); } }
        public int ControlStateTilt { get { return _ControlStateTilt; } set { SetControlStateTilt(value); } }
        public int ControlStateSideShift { get { return _ControlStateSideShift; } set { SetControlStateSideShift(value); } }

        public MachineAutomaticsMode AutomaticsMode { get { return _AutomaticsMode; } set { SetAutomaticsMode(value); } }

        // FMachineWheelWidth records the width of wheels on wheeled machines.
        // Units are meters
        public double MachineWheelWidth { get; set; } = 0.0;

        // Indicates that we've received a machine Gear value from the tag file.
        // If not, then if we encounter a Direction value, use that to populate the machine Gear.
        public bool GearValueReceived { get; set; } = false;
        public PositioningTech PositioningTech { get; set; } = PositioningTech.Unknown;

        // Serial of the IP radio, expected to be unique for a given Radio Type
        public string RadioSerial { get; set; } = "";
        // Type of IP radio, e.g. torch
        public string RadioType { get; set; } = "";
        public double LLHLat { get; set; } = Consts.NullDouble;
        public double LLHLon { get; set; } = Consts.NullDouble;
        public double LLHHeight { get; set; } = Consts.NullDouble;

        public bool LLHReceived { get; set; } = false;

        public byte UTMZone { get { return _UTMZone; } set { SetUTMZone(value); } }
        public CoordinateSystemType CSType { get { return _CSType; } set { SetCSType(value); } }

        public double GPSBaseLat { get; set; } = Consts.NullDouble;
        public double GPSBaseLon { get; set; } = Consts.NullDouble;
        public double GPSBaseHeight { get; set; } = Consts.NullDouble;

        public bool IsCSIBCoordSystemTypeOnly { get; set; } = true;
        public byte UTMZoneAtFirstPosition { get; set; } = 0;

        public bool GPSBaseLLHReceived { get { return GetGPSBaseLLHReceived; } }

        public bool OnGroundFlagSet { get; set; } = false;

        public bool ResearchData { get { return _ResearchData; } set { SetResearchData(value); } }
        public bool UsingCCA { get { return _UsingCCA; } set { SetUsingCCA(value); } }

        //////////////////////// Public procedures

        public virtual void SetGPSMode(GPSMode value) => GPSModes.Add(DataTime, value);

        public virtual void SetOnGround(OnGroundState value)
        {
            if (value != OnGroundState.No)
            {
                OnGroundFlagSet = true;
            }
            OnGrounds.Add(DataTime, value);
        }
        public virtual void SetICCCVValue(short value) => ICCCVValues.Add(DataTime, value);
        public virtual void SetICMachineSpeedValue(double value) => ICMachineSpeedValues.Add(DataTime, value);
        public virtual void SetICFrequency(ushort value) => ICFrequencys.Add(DataTime, value);
        public virtual void SetICAmplitude(ushort value) => ICAmplitudes.Add(DataTime, value);
        public virtual void SetICRMVValue(short value) => ICRMVValues.Add(DataTime, value);
        public virtual void SetAgeOfCorrection(byte value) => AgeOfCorrections.Add(DataTime, value);
        public virtual void SetVolkelMeasRange(int value) => VolkelMeasureRanges.Add(DataTime, value);
        public virtual void SetVolkelMeasUtilRange(int value) => VolkelMeasureUtilRanges.Add(DataTime, value);
        public virtual void SetMinElevMappingState(bool value) => MinElevMapping = value;
        public virtual void SetInAvoidZoneState(byte value) => InAvoidZone = value;
        public virtual void SetPositioningTechState(PositioningTech value) => PositioningTech = value;
        public virtual void SetGPSAccuracyState(GPSAccuracy AccValue, short LimValue)
        {
            GPSAccuracy = AccValue;
            GPSAccuracyErrorLimit = LimValue;
        }
        public virtual void SetICMDPValue(short value) => ICMDPValues.Add(DataTime, value);
        public virtual void SetICCCAValue(short value) => ICCCAValues.Add(DataTime, value);
        public virtual void SetICTemperatureValue(ushort value) => ICTemperatureValues.Add(DataTime, value);

        /// <summary>
        /// TAG Processor state base constructor. 
        /// Initialises the attribute consumers. All other state is intialised inline.
        /// </summary>
        public TAGProcessorStateBase()
        {
            InitialiseAttributeAccumulators();
        }

        /// <summary>
        /// ProcessContext performs processing across a
        /// context consisting of a pair of data epochs read in from the snail trail file.
        ///This function returns false if there was an error in processing the context.
        /// </summary>
        /// <returns></returns>
        public virtual bool ProcessEpochContext()
        {
            CalculatedMachineSpeed = CalculateMachineSpeed();

            LeftPoint = DataLeft;
            RightPoint = DataRight;

            LeftTrackPoint = DataTrackLeft;
            RightTrackPoint = DataTrackRight;

            LeftWheelPoint = DataWheelLeft;
            RightWheelPoint = DataWheelRight;

            LeftRearPoint = DataRearLeft;
            RightRearPoint = DataRearRight;

            GPSMode gPSMode = GPSModes.GetGPSModeAtDateTime(DataTime);

            // Check to see if the current blade epoch position is 'accurate' (essentially, not 'Autonomous' if GPS; we assume UTS is accurate enough regardless)
            // and if so save it for future use
            if (!HaveFirstAccurateGridEpochEndPoints &&
                (PositioningTech == PositioningTech.UTS) ||
                  ((PositioningTech == PositioningTech.GPS) &&
                   (gPSMode == GPSMode.Float || gPSMode == GPSMode.Fixed || gPSMode == GPSMode.DGPS || gPSMode == GPSMode.SBAS || gPSMode == GPSMode.LocationRTK)))
            {
                if (!LeftPoint.IsNullInPlan && !RightPoint.IsNullInPlan)
                {
                    FirstAccurateLeftPoint = LeftPoint;
                    FirstAccurateRightPoint = RightPoint;

                    HaveFirstAccurateGridEpochEndPoints = true;
                }
            }

            DataTimePrevious = DataTime;

            return true;
        }

        /// <summary>
        /// DoEpochStateEvent is called to handle epoch state events read from the
        /// production tag files.
        /// </summary>
        /// <returns></returns>
        public virtual bool DoEpochStateEvent(EpochStateEvent eventType)
        {
            // No processing is performed in the base class.
            return true;
        }

        /// <summary>
        /// Takes copies of the arrays of positions obtained from TAG values thathave been produced by transformation of the grid
        /// coordinate system they were measured in, into the coordinate system of the project the data is being processed into.
        /// </summary>
        /// <param name="ConvertedBladePositions"></param>
        /// <param name="ConvertedRearAxlePositions"></param>
        /// <param name="ConvertedTrackPositions"></param>
        /// <param name="ConvertedWheelPositions"></param>
        public void PopulateConvertedBladeAndRearTypePositions(List<UTMCoordPointPair> convertedBladePositions,
          List<UTMCoordPointPair> convertedRearAxlePositions, List<UTMCoordPointPair> convertedTrackPositions, List<UTMCoordPointPair> convertedWheelPositions)
        {
            ConvertedBladePositions = new List<UTMCoordPointPair>(convertedBladePositions);
            ConvertedRearAxlePositions = new List<UTMCoordPointPair>(convertedRearAxlePositions);
            ConvertedTrackPositions = new List<UTMCoordPointPair>(convertedTrackPositions);
            ConvertedWheelPositions = new List<UTMCoordPointPair>(convertedWheelPositions);
        }

        /// <summary>
        /// Get the latest known speed for the machine. This will come from machine reported speed values if available,
        /// otherwise the speed will be calculated from measurement epochs in the TAG value.
        /// </summary>
        /// <returns></returns>
        public double GetLatestMachineSpeed()
        {
            double result = (double)ICMachineSpeedValues.GetLatest();
            if (result == Consts.NullDouble)
            {
                result = CalculatedMachineSpeed;
            }

            return result;
        }
    }
}
       
    