﻿using System.Threading.Tasks;
using VSS.Productivity3D.Common.Interfaces;
using VSS.Productivity3D.Common.Models;
using VSS.Productivity3D.WebApi.Models.Compaction.Models.Reports;

namespace VSS.Productivity3D.WebApi.Models.Compaction.Helpers
{
  public interface ICompactionReportGridRequestHelper
  {
    CompactionReportGridRequestHelper SetRaptorClient(IASNodeClient raptorClient);

    Task<CompactionReportGridRequest> CreateCompactionReportGridRequest(
      bool reportElevation,
      bool reportCMV,
      bool reportMDP,
      bool reportPassCount,
      bool reportTemperature,
      bool reportCutFill,
      DesignDescriptor designFile,
      double? gridInerval,
      GridReportOption gridReportOption,
      double startNorthing,
      double startEasting,
      double endNorthing,
      double endEasting,
      double azimuth);
  }
}