﻿using System;
using System.IO;
using VSS.TRex.Cells;
using VSS.TRex.SubGridTrees.Server.Interfaces;
using VSS.TRex.SubGridTrees.Interfaces;
using VSS.TRex.Types;

namespace VSS.TRex.SubGridTrees.Server
{
    public class SubGridCellLatestPassDataWrapper_NonStatic : SubGridCellLatestPassDataWrapperBase, ISubGridCellLatestPassDataWrapper
    {
        /// <summary>
        /// The array of 32x32 cells containing a cell pass representing the latest known values for a variety of cell attributes
        /// </summary>
        public readonly CellPass[,] PassData = new CellPass[SubGridTreeConsts.SubGridTreeDimension, SubGridTreeConsts.SubGridTreeDimension];

        /// <summary>
        /// Implement the last pass indexer from the interface.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public CellPass this[int x, int y]
        {
            get => PassData[x, y];
            set => PassData[x, y] = value;
        }

        /// <summary>
        /// Provides the 'NonStatic' behaviour for clearing the passes in the latest pass information
        /// </summary>
        public override void ClearPasses()
        {
            base.ClearPasses();

            for (int i = 0; i < SubGridTreeConsts.SubGridTreeDimension; i++)
            {
              for (int j = 0; j < SubGridTreeConsts.SubGridTreeDimension; j++)
              {
                PassData[i, j] = CellPass.CLEARED_CELL_PASS;
              }
            }
        }

      public bool HasCCVData() => true;

      public bool HasRMVData() => true;

      public bool HasFrequencyData() => true;

      public bool HasAmplitudeData() => true;

      public bool HasGPSModeData() => true;

      public bool HasTemperatureData() => true;

      public bool HasMDPData() => true;

      public bool HasCCAData() => true;

      public override void Read(BinaryReader reader)
      {
        base.Read(reader);

        // Read in the latest call passes themselves
        for (int i = 0; i < SubGridTreeConsts.SubGridTreeDimension; i++)
        {
          for (int j = 0; j < SubGridTreeConsts.SubGridTreeDimension; j++)
          {
            PassData[i, j].Read(reader);
          }
        }
      }

      /// <summary>
        /// ReadInternalMachineIndex will read the internal machine ID from the latest cell identified by the Row and Col
        /// </summary>
        /// <param name="Col"></param>
        /// <param name="Row"></param>
        /// <returns></returns>
        public short ReadInternalMachineIndex(int Col, int Row) => PassData[Col, Row].InternalSiteModelMachineIndex;

      
        /// <summary>
        /// ReadTime will read the Time from the latest cell identified by the Row and Col
        /// </summary>
        /// <param name="Col"></param>
        /// <param name="Row"></param>
        /// <returns></returns>
        public DateTime ReadTime(int Col, int Row) => PassData[Col, Row].Time;

        /// <summary>
        /// ReadHeight will read the Height from the latest cell identified by the Row and Col
        /// </summary>
        /// <param name="Col"></param>
        /// <param name="Row"></param>
        /// <returns></returns>
        public float ReadHeight(int Col, int Row) => PassData[Col, Row].Height;

        /// <summary>
        /// ReadCCV will read the CCV from the latest cell identified by the Row and Col
        /// </summary>
        /// <param name="Col"></param>
        /// <param name="Row"></param>
        /// <returns></returns>
        public short ReadCCV(int Col, int Row) => PassData[Col, Row].CCV;

        /// <summary>
        /// ReadRMV will read the RMV from the latest cell identified by the Row and Col
        /// </summary>
        /// <param name="Col"></param>
        /// <param name="Row"></param>
        /// <returns></returns>
        public short ReadRMV(int Col, int Row) => PassData[Col, Row].RMV;

        /// <summary>
        /// ReadFrequency will read the Frequency from the latest cell identified by the Row and Col
        /// </summary>
        /// <param name="Col"></param>
        /// <param name="Row"></param>
        /// <returns></returns>
        public ushort ReadFrequency(int Col, int Row) => PassData[Col, Row].Frequency;

        // ReadAmplitude will read the Amplitude from the latest cell identified by the Row and Col
        public ushort ReadAmplitude(int Col, int Row) => PassData[Col, Row].Amplitude;

        /// <summary>
        /// ReadCCA will read the CCA from the latest cell identified by the Row and Col
        /// </summary>
        /// <param name="Col"></param>
        /// <param name="Row"></param>
        /// <returns></returns>
        public byte ReadCCA(int Col, int Row) => PassData[Col, Row].CCA;

        /// <summary>
        /// ReadGPSMode will read the GPSMode from the latest cell identified by the Row and Col
        /// </summary>
        /// <param name="Col"></param>
        /// <param name="Row"></param>
        /// <returns></returns>
        public GPSMode ReadGPSMode(int Col, int Row) => PassData[Col, Row].gpsMode;

        /// <summary>
        /// ReadMDP will read the MDP from the latest cell identified by the Row and Col
        /// </summary>
        /// <param name="Col"></param>
        /// <param name="Row"></param>
        /// <returns></returns>
        public short ReadMDP(int Col, int Row) => PassData[Col, Row].MDP;

        /// <summary>
        /// ReadTemperature will read the Temperature from the latest cell identified by the Row and Col
        /// </summary>
        /// <param name="Col"></param>
        /// <param name="Row"></param>
        /// <returns></returns>
        public ushort ReadTemperature(int Col, int Row) => PassData[Col, Row].MaterialTemperature;

        /// <summary>
        /// Writes the contents of the NonStatic latest passes using a supplied BinaryWriter
        /// </summary>
        /// <param name="writer"></param>
        public override void Write(BinaryWriter writer)
        {
          base.Write(writer);

          // Write out the latest call passes themselves
          for (int i = 0; i < SubGridTreeConsts.SubGridTreeDimension; i++)
          {
            for (int j = 0; j < SubGridTreeConsts.SubGridTreeDimension; j++)
            {
              PassData[i, j].Write(writer);
            }
          }
        }
    }
}
