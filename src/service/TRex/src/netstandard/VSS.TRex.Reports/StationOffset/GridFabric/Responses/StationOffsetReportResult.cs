﻿using System.IO;
using System.Text;
using VSS.Productivity3D.Models.Models.Reports;

namespace VSS.TRex.Reports.StationOffset.GridFabric.Responses
{
  /// <summary>
  /// Contains the prepared result for the client to consume.
  /// 
  /// Note that this structure needs to be look like TRaptorReportsPackager
  ///   to be deserialized by it. One day... TRaptorReportsPackeger will
  ///   go away and StationOffsetReportResult could be deserialized in 3dp using it instead.
  /// </summary>
  public class StationOffsetReportResult
  {
    public ReportReturnCode ReturnCode; // == TRaptorReportReturnCode
    public ReportType ReportType; // == TRaptorReportType
    public StationOffsetReportData_ApplicationService GriddedData { get; set; }


    public StationOffsetReportResult()
    {
      Clear();
    }

    public void Clear()
    {
      ReturnCode = ReportReturnCode.NoError;
      ReportType = ReportType.None;
      GriddedData = new StationOffsetReportData_ApplicationService();
      GriddedData.Clear();
    }

    public StationOffsetReportResult(ReportType reportType)
    {
      ReportType = reportType;
      GriddedData = new StationOffsetReportData_ApplicationService();
    }

    public byte[] Write()
    {
      using (var ms = new MemoryStream())
      {
        using (var bw = new BinaryWriter(ms, Encoding.UTF8, true))
        {
          bw.Write((int)ReturnCode);
          bw.Write((int)ReportType);

          GriddedData.Write(bw);
        }

        return ms.ToArray();
      }
    }

    public Stream Read(byte[] byteArray)
    {
      using (var ms = new MemoryStream(byteArray))
      {
        using (var reader = new BinaryReader(ms, Encoding.UTF8, true))
        {
          ReturnCode = (ReportReturnCode)reader.ReadInt32();
          ReportType = (ReportType)reader.ReadInt32();

          GriddedData.Read(reader);
        }

        return ms;
      }
    }
  }
}
