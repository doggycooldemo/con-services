﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSS.VisionLink.Raptor.Geometry;
using VSS.VisionLink.Raptor.Rendering.Palettes.Interfaces;
using VSS.VisionLink.Raptor.SiteModels;
using VSS.VisionLink.Raptor.Types;

namespace VSS.VisionLink.Raptor.Rendering.Palettes
{
    /// <summary>
    /// A factory responsible for determining the appropriate plan view map colour palette to use
    /// when rendering plan view tiles (Web Map Service tiles)
    /// </summary>
    public static class PVMPaletteFactory
    {
        public static IPlanViewPalette GetPallete(SiteModel siteModel, DisplayMode mode, BoundingWorldExtent3D spatialExtents)
        {
            switch (mode)
            {
                case DisplayMode.Height:
                    {
                        BoundingWorldExtent3D extent = siteModel.GetAdjustedDataModelSpatialExtents(new long[0]);
                        return new HeightPalette(extent.MinZ, extent.MaxZ);
                        //return new HeightPalette(spatialExtents.MinZ, spatialExtents.MaxZ);
                    }

                case DisplayMode.MachineSpeed:       return new SpeedPalette();
                case DisplayMode.CCV:                return new CMVPalette();
                case DisplayMode.TemperatureSummary: return new TemperaturePalette();
                case DisplayMode.CutFill:            return new CutFillPalette();

                default: // Just use the elevation palette as a default...
                    BoundingWorldExtent3D extent2 = siteModel.GetAdjustedDataModelSpatialExtents(new long[0]);
                    return new HeightPalette(extent2.MinZ, extent2.MaxZ);
                    //return new HeightPalette(spatialExtents.MinZ, spatialExtents.MaxZ);
            }

            /*
            Complicated legacy implementation follows:
              if Assigned(FDisplayPalettes) then
                case FMode of
                  icdmHeight             : FPalette := FDisplayPalettes.HeightPalette;
                  icdmCCV                : FPalette := FDisplayPalettes.CCVPalette;
                  icdmCCVPercent         : FPalette := FDisplayPalettes.CCVPercentPalette;
                  icdmLatency            : FPalette := FDisplayPalettes.RadioLatencyPalette;
                  icdmPassCount,
                  icdmPassCountSummary   : FPalette := FDisplayPalettes.PassCountPalette;
                  icdmRMV                : FPalette := FDisplayPalettes.RMVPalette;
                  icdmFrequency          : FPalette := FDisplayPalettes.FrequencyPalette;
                  icdmAmplitude          : FPalette := FDisplayPalettes.AmplitudePalette;
                  icdmCutFill            : FPalette := FDisplayPalettes.CutFillPalette;
                  icdmMoisture           : FPalette := FDisplayPalettes.MoisturePalette;
                  icdmTemperatureSummary :; // see below AJR 14974
                  icdmGPSMode            :; // See GPSMode palette assignment below
                  icdmCCVSummary         :; // See CCV summary palette assignment below
                  icdmCCVPercentSummary  :; // See CCV Percent summary palette assignment below
                  icdmCompactionCoverage : FPalette := Nil; // Single fixed colour
                  icdmVolumeCoverage     : FPalette := FDisplayPalettes.VolumeOverlayPalette;
                  icdmMDP                : FPalette := FDisplayPalettes.MDPPalette;
                  icdmMDPSummary         :;
                  icdmMDPPercent         : FPalette := FDisplayPalettes.MDPPercentPalette;
                  icdmMDPPercentSummary  :;
                  icdmMachineSpeed       : FPalette := FDisplayPalettes.MachineSpeedPalette;
                  icdmTargetSpeedSummary :;// FPalette := FDisplayPalettes.SpeedSummaryPalette;
            //    icdmCCA                : FPalette :=    does not seem to be need right now?
            //      icdmCCASummary       : FPalette :=   does not seem to be needed right now?

                end;

              if Assigned(FPalette) and (FPalette.TransitionColours.Count = 0) then
                FPalette.SetToDefaults;

              FDisplayer.Palette := FPalette; // CCA set up here

              // Assign specialty palettes that don't conform to the scaled palette base class
              case FMode of

                icdmTemperatureSummary:
                  begin
                    with (FDisplayer as TSVOICPVMDisplayer_TemperatureSummary) do
                      BuildTemperaturePaletteFromPaletteTransitions(FPalette);

                  end;
                icdmCCVSummary,
                icdmCCVPercentSummary:
                  begin
                    with (FDisplayer as TSVOICPVMDisplayer_CCV) do
                      BuildCCVSummaryPaletteFromPaletteTransitions(FPalette);
                  end;

                icdmGPSMode:
                  begin
                    with (FDisplayer as TSVOICPVMDisplayer_GPSMode) do
                      BuildGPSModeSummaryPaletteFromPaletteTransitions(FPalette)
                  end;

                icdmMDPSummary,
                icdmMDPPercentSummary:
                  begin
                    with (FDisplayer as TSVOICPVMDisplayer_MDP) do
                      BuildMDPSummaryPaletteFromPaletteTransitions(FPalette);
                  end;
                icdmTargetSpeedSummary:
                  begin
                    with (FDisplayer as TSVOICPVMDisplayer_SpeedSummary) do
                      BuildSpeedSummaryPaletteFromPaletteTransitions(FPalette);
                  end;
              end;
                         */

        }
    }
}
