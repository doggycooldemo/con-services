﻿using System;
using System.IO;
using Microsoft.Extensions.Logging;
using VSS.Productivity3D.Models.Enums;
using VSS.TRex.Common.CellPasses;
using VSS.TRex.Events.Models;
using VSS.TRex.Filters.Models;
using VSS.TRex.Profiling;
using VSS.TRex.Profiling.Interfaces;
using VSS.TRex.Profiling.Models;
using VSS.TRex.SubGridTrees.Client.Interfaces;
using VSS.TRex.SubGridTrees.Core.Utilities;
using VSS.TRex.SubGridTrees.Interfaces;
using VSS.TRex.Types;

namespace VSS.TRex.SubGridTrees.Client
{
  public class ClientCellProfileLeafSubgrid : GenericClientLeafSubGrid<ClientCellProfileLeafSubgridRecord>
  {
    private static readonly ILogger Log = Logging.Logger.CreateLogger<ClientCellProfileLeafSubgrid>();

    /*
  TICClientSubGridTreeLeaf_CellProfile = class(TICClientSubGridTreeLeaf_Base<TICSubGridCellPassData_CellProfile_Entry>)
    Public
      Function CellHasValue(CellX, CellY : Integer) : Boolean; Override;
      Procedure Clear; Override;
      Procedure AssignFilteredValue(const CellX, CellY : Integer;
                                    const Context : TICSubGridFilteredValueAssignmentContext); Override;
  */

    /// <summary>
    /// Initialise the null cell values for the client subgrid
    /// </summary>
    static ClientCellProfileLeafSubgrid()
    {
      SubGridUtilities.SubGridDimensionalIterator((x, y) => NullCells[x, y] = new ClientCellProfileLeafSubgridRecord());
    }

    private void Initialise()
    {
      _gridDataType = GridDataType.CellProfile;

      EventPopulationFlags |= //PopulationControlFlags.WantLiftProcessingResults     | todo ???
        PopulationControlFlags.WantsTargetPassCountValues |
        PopulationControlFlags.WantsTargetCCVValues |
        PopulationControlFlags.WantsTargetMDPValues |
        // PopulationControlFlags.WantsEventGPSModeValues        |   todo??
        PopulationControlFlags.WantsEventGPSAccuracyValues |
        PopulationControlFlags.WantsTargetThicknessValues |
        PopulationControlFlags.WantsEventVibrationStateValues |
        PopulationControlFlags.WantsEventMachineGearValues |
        PopulationControlFlags.WantsEventDesignNameValues;
    }

    public ClientCellProfileLeafSubgrid() : base()
    {
      Initialise();
    }

    /// <summary>
    /// Constructor. Set the grid to HeightAndTime.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="parent"></param>
    /// <param name="level"></param>
    /// <param name="cellSize"></param>
    /// <param name="indexOriginOffset"></param>
    public ClientCellProfileLeafSubgrid(ISubGridTree owner, ISubGrid parent, byte level, double cellSize, uint indexOriginOffset) : base(owner, parent, level, cellSize, indexOriginOffset)
    {
      Initialise();
    }

    public override bool AssignableFilteredValueIsNull(ref FilteredPassData filteredValue)
    {
      switch (ProfileDisplayMode)
      {
        case DisplayMode.CCVPercent:
        case DisplayMode.CCVSummary:
        case DisplayMode.CCVPercentSummary:
          return filteredValue.FilteredPass.CCV == CellPassConsts.NullCCV;

        case DisplayMode.MDPPercent:
        case DisplayMode.MDPSummary:
        case DisplayMode.MDPPercentSummary:
          return filteredValue.FilteredPass.CCV == CellPassConsts.NullMDP;

        default:
          return filteredValue.FilteredPass.Time == DateTime.MinValue;
      }
    }

    public override void Clear()
    {
      Array.Copy(NullCells, Cells, SubGridTreeConsts.CellsPerSubgrid);

      TopLayerOnly = false;
      ProfileDisplayMode = DisplayMode.Height;
    }

    public override void FillWithTestPattern()
    {
      ForEach((x, y) =>
      {
        Cells[x, y] = new ClientCellProfileLeafSubgridRecord
        {
          LastPassTime = new DateTime(x * 1000 + y),
          PassCount = x + y
        };
      });
    }

    public override bool LeafContentEquals(IClientLeafSubGrid other)
    {
      bool result = true;

      IGenericClientLeafSubGrid<ClientCellProfileLeafSubgridRecord> _other = (IGenericClientLeafSubGrid<ClientCellProfileLeafSubgridRecord>)other;
      ForEach((x, y) => result &= Cells[x, y].Equals(_other.Cells[x, y]));

      return result;
    }

    public override ClientCellProfileLeafSubgridRecord NullCell()
    {
      return new ClientCellProfileLeafSubgridRecord();
    }

    public override bool CellHasValue(byte cellX, byte cellY) => Cells[cellX, cellY].LastPassTime != DateTime.MinValue;

    /// <summary>
    /// Assign filtered height value from a filtered pass to a cell
    /// </summary>
    /// <param name="cellX"></param>
    /// <param name="cellY"></param>
    /// <param name="Context"></param>
    public override void AssignFilteredValue(byte cellX, byte cellY, FilteredValueAssignmentContext context)
    {
      //  LastPass : TICFilteredPassData;
      double v1, v2;

      void CalculateCMVChange(IProfileCell profileCell /*; Todo: Lifebuildsettings
      LiftBuildSettings : TICLiftBuildSettings*/)
      {
        //      with ProfileCell, LiftBuildSettings do
        //      begin
        profileCell.CellCCV = CellPassConsts.NullCCV;
        profileCell.CellTargetCCV = CellPassConsts.NullCCV;
        profileCell.CellPreviousMeasuredCCV = CellPassConsts.NullCCV;
        profileCell.CellPreviousMeasuredTargetCCV = CellPassConsts.NullCCV;

        bool DataStillRequiredForCCV = true;

        for (int i = profileCell.Layers.Count() - 1; i >= 0; i--)
          //     with Layers[i] do
          if (profileCell.Layers[i].FilteredPassCount > 0)
          {
            if ((profileCell.Layers[i].Status & LayerStatus.Superseded) != 0 && !Dummy_LiftBuildSettings.IncludeSuperseded)
              continue;

            if (DataStillRequiredForCCV && profileCell.CellCCV == CellPassConsts.NullCCV && profileCell.Layers[i].CCV != CellPassConsts.NullCCV)
            {
              profileCell.CellCCV = profileCell.Layers[i].CCV;
              profileCell.CellCCVElev = profileCell.Layers[i].CCV_Elev;

              int PassSearchIdx = profileCell.Layers[i].CCV_CellPassIdx - 1;
              while (PassSearchIdx >= 0)
              {
                if (Dummy_LiftBuildSettings.CCVSummarizeTopLayerOnly && (PassSearchIdx < profileCell.Layers[i].StartCellPassIdx || PassSearchIdx > profileCell.Layers[i].EndCellPassIdx))
                  break;

                if (!profileCell.Layers.IsCellPassInSupersededLayer(PassSearchIdx) || Dummy_LiftBuildSettings.IncludeSuperseded)
                {
                  profileCell.CellPreviousMeasuredCCV = profileCell.Passes.FilteredPassData[PassSearchIdx].FilteredPass.CCV;
                  if (Dummy_LiftBuildSettings.OverrideMachineCCV)
                    profileCell.CellPreviousMeasuredTargetCCV = Dummy_LiftBuildSettings.OverridingMachineCCV;
                  else
                    profileCell.CellPreviousMeasuredTargetCCV = profileCell.Passes.FilteredPassData[PassSearchIdx].TargetValues.TargetCCV;
                  break;
                }

                PassSearchIdx--;
              }

              DataStillRequiredForCCV = false;
            }


            if (!DataStillRequiredForCCV)
              break;

            if (Dummy_LiftBuildSettings.CCVSummarizeTopLayerOnly)
              DataStillRequiredForCCV = false;
          }
      }

      if (context.CellProfile == null)
      {
        Log.LogError($"{nameof(AssignFilteredValue)}: Error=CellProfile not assigned.");
        return;
      }

      IProfileCell cellProfileFromContext = context.CellProfile as IProfileCell;
      FilteredPassData LastPass = cellProfileFromContext.Passes.FilteredPassData[cellProfileFromContext.Passes.PassCount - 1];

      //  with Cells[CellX, CellY], Context.CellProfile do
      //  begin

      //    with Context.ProbePositions[CellX, CellY] do
      //      begin
      Cells[cellX, cellY].CellXOffset = context.ProbePositions[cellX, cellY].XOffset;
      Cells[cellX, cellY].CellYOffset = context.ProbePositions[cellX, cellY].YOffset;
      //      end;

      Cells[cellX, cellY].LastPassTime = cellProfileFromContext.Passes.LastPassTime();
      Cells[cellX, cellY].PassCount = context.FilteredValue.PassCount;
      Cells[cellX, cellY].LastPassValidRadioLatency = cellProfileFromContext.Passes.LastPassValidRadioLatency();
      Cells[cellX, cellY].EventDesignNameID = LastPass.EventValues.EventDesignNameID;
      Cells[cellX, cellY].MachineID = LastPass.FilteredPass.MachineID;
      Cells[cellX, cellY].MachineSpeed = LastPass.FilteredPass.MachineSpeed;
      Cells[cellX, cellY].LastPassValidGPSMode = cellProfileFromContext.Passes.LastPassValidGPSMode();
      Cells[cellX, cellY].GPSTolerance = LastPass.EventValues.GPSTolerance;
      Cells[cellX, cellY].GPSAccuracy = LastPass.EventValues.GPSAccuracy;
      Cells[cellX, cellY].TargetPassCount = LastPass.TargetValues.TargetPassCount;
      Cells[cellX, cellY].TotalWholePasses = cellProfileFromContext.TotalNumberOfWholePasses(true); // include superseded layers
      Cells[cellX, cellY].TotalHalfPasses = cellProfileFromContext.TotalNumberOfHalfPasses(true); // include superseded layers
      Cells[cellX, cellY].LayersCount = cellProfileFromContext.Layers.Count;

      cellProfileFromContext.Passes.LastPassValidCCVDetails(out var lastPassValidCCV, out var _targetCCV); // get details from last VALID pass
      Cells[cellX, cellY].LastPassValidCCV = lastPassValidCCV;
      Cells[cellX, cellY].CellTargetCCV = _targetCCV;

      cellProfileFromContext.Passes.LastPassValidMDPDetails(LastPassValidMDP, TargetMDP); // get details from last VALID pass

      cellProfileFromContext.Passes.LastPassValidCCADetails(LastPassValidCCA, TargetCCA); // get details from last VALID pass

      Cells[cellX, cellY].LastPassValidRMV = cellProfileFromContext.Passes.LastPassValidRMV();
      Cells[cellX, cellY].LastPassValidFreq = cellProfileFromContext.Passes.LastPassValidFreq();
      Cells[cellX, cellY].LastPassValidAmp = cellProfileFromContext.Passes.LastPassValidAmp();
      Cells[cellX, cellY].TargetThickness = LastPass.TargetValues.TargetLiftThickness;
      Cells[cellX, cellY].EventMachineGear = LastPass.EventValues.EventMachineGear;
      Cells[cellX, cellY].EventVibrationState = LastPass.EventValues.EventVibrationState;
      Cells[cellX, cellY].LastPassValidTemperature = LastPass.FilteredPass.MaterialTemperature; // Bug32323 show only last pass temp.  Passes.LastPassValidTemperature;
      Cells[cellX, cellY].Height = LastPass.FilteredPass.Height;

      CalculateCMVChange(cellProfileFromContext /* todo: , context.LiftBuildSettings*/);
      cellProfileFromContext.CCVChange = 0;
      v2 = cellProfileFromContext.CellCCV;
      v1 = cellProfileFromContext.CellPreviousMeasuredCCV;

      if (v2 == CellPassConsts.NullCCV)
        cellProfileFromContext.CCVChange = CellPassConsts.NullCCV; // will force no result to show
      else if (v1 == CellPassConsts.NullCCV)
        cellProfileFromContext.CCVChange = 100; // %100 diff
      else
      {
        if (v1 == 0) // avoid div by 0 error
          cellProfileFromContext.CCVChange = 100;
        else
          cellProfileFromContext.CCVChange = (float) (((v2 - v1) / v1) * 100);
      }
    }

    /// <summary>
    /// Write the contents of the Items array using the supplied writer
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="buffer"></param>
    public override void Write(BinaryWriter writer, byte[] buffer)
    {
      base.Write(writer, buffer);

      SubGridUtilities.SubGridDimensionalIterator((x, y) => Cells[x, y].Write(writer));
    }

    /// <summary>
    /// Fill the items array by reading the binary representation using the provided reader. 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="buffer"></param>
    public override void Read(BinaryReader reader, byte[] buffer)
    {
      base.Read(reader, buffer);

      SubGridUtilities.SubGridDimensionalIterator((x, y) => Cells[x, y].Read(reader));
    }
  }
}
