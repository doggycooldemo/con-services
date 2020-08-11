﻿
/******************************************
 Adding a new tag? update these units and follow the pattern

 TagConstants
    public const string CCV = "CCV";

 TagFile.cs
   private void CreateTagfileDictionary()

 EpochRecord.cs

 AscciiParser.cs

 DictionaryItem.cs

 CreateTagfileDictionary

 TagfileTest.cs
   TestTagFileCreation for testing new tag
 *****************************************/
using TagFiles.Common;

/// <summary>
/// Acts like a state machine for each epoch
/// </summary>
/// 
namespace TagFiles.Parser
{

  public class EpochRecord
  {

    public bool HasHeader = false;
    public bool HasTime = false;
    public bool HasDeltaTime = false;
    public bool HasPrevTime = false;
    public bool HasWeek = false;
    public bool HasCoordSys = false;
    public bool HasLEB = false;
    public bool HasLNB = false;
    public bool HasLHB = false;
    public bool HasREB = false;
    public bool HasRNB = false;
    public bool HasRHB = false;
    public bool HasGPM = false;
    public bool HasBOG = false;
    public bool HasDES = false;
    public bool HasLAT = false;
    public bool HasLON = false;
    public bool HasHGT = false;
    public bool HasMID = false;
    public bool HasMSD = false;
    public bool HasMTP = false;
    public bool HasHDG = false;
    public bool HasSER = false;
    public bool HasUTM = false;
    public bool HasRadioSerial = false;
    public bool HasRadioType = false;
    public bool HasMappingMode = false;
    public bool HasAppVersion = false;
    public bool HasValidPosition = false;
    public bool HasCST = false;
    public bool HasCCV = false;
    public bool HasTargetCCV = false;
    public bool HasMDP = false;
    public bool HasTargetMDP = false;
    public bool HasFlags = false;
    public bool HasTemperature = false;
    public bool HasDirection = false;
    public bool HasTargetPasses = false;
    public bool HasTargetThickness = false;
    public bool HasTempMin = false;
    public bool HasTempMax = false;

    // Fields

    /// <summary>
    /// Time
    /// </summary>
    private uint _Time = 0;
    public uint Time
    {
      get => _Time;
      set
      {
        _Time = value;

        // workout time delta
        if (HasPrevTime)
        {
          DeltaTime = value - _PrevTime;
          if (DeltaTime < 0)
            DeltaTime = 0;
        }

        PrevTime = value;
        HasTime = true;
      }
    }

    /// <summary>
    /// Time delta
    /// </summary>
    private uint _DeltaTime = 0;
    public uint DeltaTime
    {
      get => _DeltaTime;
      set
      {
        _DeltaTime = value;
        HasDeltaTime = true;
      }
    }

    /// <summary>
    /// PrevTime
    /// </summary>
    private uint _PrevTime = 0;
    public uint PrevTime
    {
      get => _PrevTime;
      set
      {
        _PrevTime = value;
        HasPrevTime = true;
      }
    }

    /// <summary>
    /// Week
    /// </summary>
    private uint _Week = 0;
    public uint Week
    {
      get => _Week;
      set
      {
        _Week = value;
        HasWeek = true;
      }
    }

    /// <summary>
    /// Left Easting Blade
    /// </summary>
    private double _LEB = 0;
    public double LEB
    {
      get => _LEB;
      set
      {
        _LEB = value;
        HasLEB = true;
      }
    }

    /// <summary>
    /// Left Northing Blade
    /// </summary>
    private double _LNB = 0;
    public double LNB
    {
      get => _LNB;
      set
      {
        _LNB = value;
        HasLNB = true;
      }
    }

    /// <summary>
    /// Left Height Blade
    /// </summary>
    private double _LHB = 0;
    public double LHB
    {
      get => _LHB;
      set
      {
        _LHB = value;
        HasLHB = true;
      }
    }

    /// <summary>
    /// Right Easting Blade
    /// </summary>
    private double _REB = 0;
    public double REB
    {
      get => _REB;
      set
      {
        _REB = value;
        HasREB = true;
      }
    }

    /// <summary>
    /// Righ Northing Blade
    /// </summary>
    private double _RNB = 0;
    public double RNB
    {
      get => _RNB;
      set
      {
        _RNB = value;
        HasRNB = true;
      }
    }

    /// <summary>
    /// Right Height Blade
    /// </summary>
    private double _RHB = 0;
    public double RHB
    {
      get => _RHB;
      set
      {
        _RHB = value;
        HasRHB = true;
      }
    }

    /// <summary>
    /// GPS Mode
    /// </summary>
    private ushort _GPM = 0;
    public ushort GPM
    {
      get => _GPM;
      set
      {
        _GPM = value;
        HasGPM = true;
      }
    }

    /// <summary>
    /// Blade On Ground
    /// </summary>
    private ushort _BOG = 0;
    public ushort BOG
    {
      get => _BOG;
      set
      {
        _BOG = value;
        HasBOG = true;
      }
    }

    /// <summary>
    /// Design
    /// </summary>
    private string _Design = "";
    public string Design
    {
      get => _Design;
      set
      {
        _Design = value;
        HasDES = true;
      }
    }

    /// <summary>
    /// Latitude
    /// </summary>
    private double _LAT = 0;
    public double LAT
    {
      get => _LAT;
      set
      {
        _LAT = value;
        HasLAT = true;
      }
    }

    /// <summary>
    /// Longitude
    /// </summary>
    private double _LON = 0;
    public double LON
    {
      get => _LON;
      set
      {
        _LON = value;
        HasLON = true;
      }
    }

    /// <summary>
    /// Height
    /// </summary>
    private double _HGT = 0;
    public double HGT
    {
      get => _HGT;
      set
      {
        _HGT = value;
        HasHGT = true;
      }
    }

    /// <summary>
    /// Machine ID
    /// </summary>
    private string _MID = "";
    public string MID
    {
      get => _MID;
      set
      {
        _MID = value;
        HasMID = true;
      }
    }

    /// <summary>
    /// Machine Speed
    /// </summary>
    private double _MSD = 0;
    public double MSD
    {
      get => _MSD;
      set
      {
        _MSD = value;
        HasMSD = true;
      }
    }

    /// <summary>
    /// Machine Type
    /// </summary>
    private byte _MTP = 0;
    public byte MTP
    {
      get => _MTP;
      set
      {
        _MTP = value;
        HasMTP = true;
      }
    }

    /// <summary>
    /// Heading
    /// </summary>
    private double _HDG = 0;
    public double HDG
    {
      get => _HDG;
      set
      {
        _HDG = value;
        HasHDG = true;
      }
    }

    /// <summary>
    /// Radio Serial
    /// </summary>
    private string _RadioSerial = "";
    public string RadioSerial
    {
      get => _RadioSerial;
      set
      {
        _RadioSerial = value;
        HasRadioSerial = true;
      }
    }

    /// <summary>
    /// Radio Type
    /// </summary>
    private string _RadioType = "torch";
    public string RadioType
    {
      get => _RadioType;
      set
      {
        _RadioType = value;
        HasRadioType = true;
      }
    }

    /// <summary>
    /// Coordinate System
    /// </summary>
    private ushort _CoordSys = 0;
    public ushort CoordSys
    {
      get => _CoordSys;
      set
      {
        _CoordSys = value;
        HasCoordSys = true;
      }
    }

    /// <summary>
    /// Coordinate System
    /// </summary>
    private ushort _ValidPosition = 1;
    public ushort ValidPosition
    {
      get => _ValidPosition;
      set
      {
        _ValidPosition = value;
        HasValidPosition = true;
      }
    }

    /// <summary>
    /// UTM Zone
    /// </summary>
    private byte _UTM = 0;
    public byte UTM
    {
      get => _UTM;
      set
      {
        _UTM = value;
        HasUTM = true;
      }
    }

    /// <summary>
    /// Mapping Mode
    /// </summary>
    private ushort _MappingMode = ushort.MaxValue;
    public ushort MappingMode
    {
      get => _MappingMode;
      set
      {
        _MappingMode = value;
        HasMappingMode = true;
      }
    }

    /// <summary>
    /// Application Version
    /// </summary>
    private string _AppVersion = "";
    public string AppVersion
    {
      get => _AppVersion;
      set
      {
        _AppVersion = value;
        HasAppVersion = true;
      }
    }

    /// <summary>
    /// Machine serial id
    /// </summary>
    private string _Serial = "";
    public string Serial
    {
      get => _Serial;
      set
      {
        _Serial = value;
        HasSER = true;
      }
    }

    /// <summary>
    /// CCV compaction
    /// </summary>
    private uint _CCV = 0;
    public uint CCV
    {
      get => _CCV;
      set
      {
        _CCV = value;
        HasCCV = true;
      }
    }

    /// <summary>
    /// Target CCV
    /// </summary>
    private uint _TargetCCV = 0; // 12 bit
    public uint TargetCCV
    {
      get => _TargetCCV;
      set
      {
        _TargetCCV = value;
        HasTargetCCV = true;
      }
    }

    /// <summary>
    /// MDP Compaction
    /// </summary>
    private uint _MDP = 0;
    public uint MDP
    {
      get => _MDP;
      set
      {
        _MDP = value;
        HasMDP = true;
      }
    }

    /// <summary>
    /// Target MDP
    /// </summary>
    private uint _TargetMDP = 0; // 12 bit
    public uint TargetMDP
    {
      get => _TargetMDP;
      set
      {
        _TargetMDP = value;
        HasTargetMDP = true;
      }
    }

    /// <summary>
    /// Compaction Sensor Type
    /// </summary>
    private uint _CST = 0; // 8 bit
    public uint CST
    {
      get => _CST;
      set
      {
        _CST = value;
        HasCST = true;
      }
    }

    /// <summary>
    /// Temperature
    /// </summary>
    private uint _Temperature = 0; // 12 bit absolute
    public uint Temperature
    {
      get => _Temperature;
      set
      {
        _Temperature = value;
        HasTemperature = true;
      }
    }

    /// <summary>
    /// Machine Direction
    /// </summary>
    private uint _Direction = 1; // 4 bit absolute 1 = forward, 2=Reverse,3=unknown
    public uint Direction
    {
      get => _Direction;
      set
      {
        _Direction = value;
        HasDirection = true;
      }
    }

    /// <summary>
    /// Contains info like type of compaction and vibe state
    /// </summary>
    private uint _Flags = 1; // 4 bit absolute
    public uint Flags
    {
      get => _Flags;
      set
      {
        _Flags = value;
        HasFlags = true;
      }
    }

    /// <summary>
    /// Target Passcount
    /// </summary>
    private uint _TargetPasses = 0; // 12 bit
    public uint TargetPasses
    {
      get => _TargetPasses;
      set
      {
        _TargetPasses = value;
        HasTargetPasses = true;
      }
    }

    /// <summary>
    /// Target Thickness
    /// </summary>
    private uint _TargetThickness = 0; // 16 bit
    public uint TargetThickness
    {
      get => _TargetThickness;
      set
      {
        _TargetThickness = value;
        HasTargetThickness = true;
      }
    }

    /// <summary>
    /// Min target temperature
    /// </summary>
    private uint _TempMin = 0; // 12 bit
    public uint TempMin
    {
      get => _TempMin;
      set
      {
        _TempMin = value;
        HasTempMin = true;
      }
    }

    /// <summary>
    /// Max target temperature
    /// </summary>
    private uint _TempMax = 0; // 12 bit
    public uint TempMax
    {
      get => _TempMax;
      set
      {
        _TempMax = value;
        HasTempMax = true;
      }
    }


    public EpochRecord()
    {
      ClearEpoch();
    }

    /// <summary>
    /// Clear data but not static header info
    /// </summary>
    public void ClearEpoch()
    {
      HasHeader = false;
      HasTime = false;
      HasDeltaTime = false;
      HasPrevTime = false;
      HasWeek = false;
      HasCoordSys = false;
      HasLEB = false;
      HasLNB = false;
      HasLHB = false;
      HasREB = false;
      HasRNB = false;
      HasRHB = false;
      HasGPM = false;
      HasBOG = false;
      HasLAT = false;
      HasLON = false;
      HasHGT = false;
      HasMID = false;
      HasMSD = false;
      HasMTP = false;
      HasHDG = false;
      HasSER = false;
      HasUTM = false;
      HasRadioSerial = false;
      HasRadioType = false;
      HasMappingMode = false;
      HasAppVersion = false;
      HasValidPosition = false;
      HasCCV = false;
      HasMDP = false;
      HasCST = false;
      HasTargetCCV = false;
      HasTargetMDP = false;
      HasFlags = false;
      HasTemperature = false;
      HasDirection = false;
      HasTargetThickness = false;
      HasTargetPasses = false;
      HasTempMin = false;
      HasTempMax = false;

    }

    public bool IsFullPositionEpoch()
    {
      return HasLEB & HasLHB & HasLNB & HasREB & HasRHB & HasRNB;
    }

    public bool IsHeaderEpoch()
    {
      return HasTime & HasWeek & HasCoordSys & HasAppVersion;
    }

    /// <summary>
    /// Used for an update Epoch
    /// </summary>
    /// <param name="eRec"></param>
    public void EpochCopy(ref EpochRecord eRec)
    {
      Time = eRec.Time;
      LEB = eRec.LEB;
      LNB = eRec.LNB;
      LHB = eRec.LHB;
      REB = eRec.REB;
      RNB = eRec.RNB;
      RHB = eRec.RHB;
      BOG = eRec.BOG;
      HDG = eRec.HDG;
      MSD = eRec.MSD;
      LAT = eRec.LAT;
      LON = eRec.LON;
      HGT = eRec.HGT;
      GPM = eRec.GPM;
      UTM = eRec.UTM;
      MTP = eRec.MTP;
      MID = eRec.MID;
      Serial = eRec.Serial;
      Design = eRec.Design;
      RadioSerial = eRec.RadioSerial;
      RadioType = eRec.RadioType;
      HasHeader = eRec.HasHeader;
      CCV = eRec.CCV;
      CST = eRec.CST;
      TargetCCV = eRec.TargetCCV;
      Flags = eRec.Flags;
      Temperature = eRec.Temperature;
      Direction = eRec.Direction;
      TargetMDP = eRec.TargetMDP;
      MDP = eRec.MDP;
      TargetThickness = eRec.TargetThickness;
      TargetPasses = eRec.TargetPasses;
      TempMin = eRec.TempMin;
      TempMax = eRec.TempMax;
    }

    /// <summary>
    /// Is the blade position different from the ref epoch
    /// </summary>
    /// <param name="eRec"></param>
    public bool BladePositionDifferent(ref EpochRecord eRec)
    {
      if (IsFullPositionEpoch() & eRec.IsFullPositionEpoch())
        return !(LEB == eRec.LEB & LNB == eRec.LNB & LHB == eRec.LHB & REB == eRec.REB & RNB == eRec.RNB & RHB == eRec.RHB);
      else
        return true;

    }

    /// <summary>
    /// Does epoch contain any change data
    /// </summary>
    public bool HasUpdateData()
    {
      return HasLEB || HasLHB || HasLNB || HasREB || HasRHB || HasRNB || HasGPM || HasBOG || HasDES || HasLAT || HasLON ||
             HasHGT || HasMSD || HasHDG || HasCCV || HasCST || HasTargetCCV || HasFlags || HasDirection || HasTemperature || 
             HasMDP || HasTargetMDP || HasTargetPasses || HasTargetThickness || HasTempMin || HasTempMax;

    }

  }
}
