﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VSS.MasterData.Models.Models;
using VSS.Productivity3D.Models.Models;
using VSS.Productivity3D.Models.Models.Coords;
using VSS.Productivity3D.Models.Models.Profiling;
using VSS.Productivity3D.Models.Models.Reports;
using VSS.Productivity3D.Models.ResultHandling;
using VSS.Productivity3D.Models.ResultHandling.Coords;
using VSS.Productivity3D.Models.ResultHandling.Profiling;
using VSS.Productivity3D.WebApi.Models.Compaction.Models.Reports;

namespace VSS.MasterData.Proxies.Interfaces
{
  /// <summary>
  /// Proxy interface to access the TRex Gateway WebAPIs.
  /// </summary>
  public interface ITRexCompactionDataProxy
  {
    /// <summary>
    /// Sends a request to get CMV % Change statistics from the TRex database.
    /// </summary>
    /// <param name="cmvChangeDetailsRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    Task<CMVChangeSummaryResult> SendCMVChangeDetailsRequest(CMVChangeDetailsRequest cmvChangeDetailsRequest,
      IDictionary<string, string> customHeaders = null);

    /// <summary>
    /// Sends a request to get CMV Details statistics from the TRex database.
    /// </summary>
    /// <param name="cmvDetailsRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    Task<CMVDetailedResult> SendCMVDetailsRequest(CMVDetailsRequest cmvDetailsRequest,
      IDictionary<string, string> customHeaders = null);

    /// <summary>
    /// Sends a request to get CMV Summary statistics from the TRex database.
    /// </summary>
    /// <param name="cmvSummaryRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    Task<CMVSummaryResult> SendCMVSummaryRequest(CMVSummaryRequest cmvSummaryRequest,
      IDictionary<string, string> customHeaders = null);

    /// <summary>
    /// Sends a request to get Pass Count Details statistics from the TRex database.
    /// </summary>
    /// <param name="pcDetailsRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    Task<PassCountDetailedResult> SendPassCountDetailsRequest(PassCountDetailsRequest pcDetailsRequest,
      IDictionary<string, string> customHeaders = null);

    /// <summary>
    /// Sends a request to get Pass Count Summary statistics from the TRex database.
    /// </summary>
    /// <param name="pcSummaryRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    Task<PassCountSummaryResult> SendPassCountSummaryRequest(PassCountSummaryRequest pcSummaryRequest,
      IDictionary<string, string> customHeaders = null);

    /// <summary>
    /// Sends a request to get Cut/Fill Details statistics from the TRex database.
    /// </summary>
    /// <param name="cfDetailsRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    Task<CompactionCutFillDetailedResult> SendCutFillDetailsRequest(CutFillDetailsRequest cfDetailsRequest,
      IDictionary<string, string> customHeaders = null);

    /// <summary>
    /// Sends a request to get MDP Summary statistics from the TRex database.
    /// </summary>
    /// <param name="mdpSummaryRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    Task<MDPSummaryResult> SendMDPSummaryRequest(MDPSummaryRequest mdpSummaryRequest,
      IDictionary<string, string> customHeaders = null);

    /// <summary>
    /// Sends a request to get Material Temperature Summary statistics from the TRex database.
    /// </summary>
    /// <param name="temperatureSummaryRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    Task<TemperatureSummaryResult> SendTemperatureSummaryRequest(TemperatureSummaryRequest temperatureSummaryRequest,
      IDictionary<string, string> customHeaders = null);

    /// <summary>
    /// Sends a request to get Material Temperature Details statistics from the TRex database.
    /// </summary>
    /// <param name="temperatureDetailsRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    Task<TemperatureDetailResult> SendTemperatureDetailsRequest(TemperatureDetailRequest temperatureDetailsRequest,
    IDictionary<string, string> customHeaders = null);

    /// <summary>
    /// Sends a request to get Machine Speed Summary statistics from the TRex database.
    /// </summary>
    /// <param name="speedSummaryRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    Task<SpeedSummaryResult> SendSpeedSummaryRequest(SpeedSummaryRequest speedSummaryRequest,
      IDictionary<string, string> customHeaders = null);

    /// <summary>
    /// Sends a request to get CCA Summary statistics from the TRex database.
    /// </summary>
    /// <param name="ccaSummaryRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    Task<CCASummaryResult> SendCCASummaryRequest(CCASummaryRequest ccaSummaryRequest,
      IDictionary<string, string> customHeaders = null);

    /// <summary>
    /// Sends a request to get production data tile from the TRex database.
    /// </summary>
    /// <param name="tileRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    Task<Stream> SendProductionDataTileRequest(TileRequest tileRequest,
      IDictionary<string, string> customHeaders = null);

    /// <summary>
    /// Sends a request to get Summary Volumes statistics from the TRex database.
    /// </summary>
    /// <param name="summaryVolumesRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    Task<SummaryVolumesResult> SendSummaryVolumesRequest(SummaryVolumesDataRequest summaryVolumesRequest,
      IDictionary<string, string> customHeaders = null);

    /// <summary>
    /// Sends a request to get Summary Volumes profiling data from the TRex database.
    /// </summary>
    /// <param name="summaryVolumesProfileDataRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    Task<ProfileDataResult<SummaryVolumesProfileCell>> SendSummaryVolumesProfileDataRequest(SummaryVolumesProfileDataRequest summaryVolumesProfileDataRequest,
      IDictionary<string, string> customHeaders = null);

    /// <summary>
    /// Sends a request to get Production Data profiling data from the TRex database.
    /// </summary>
    /// <param name="productionDataProfileDataRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    Task<ProfileDataResult<ProfileCellData>> SendProductionDataProfileDataRequest(ProductionDataProfileDataRequest productionDataProfileDataRequest,
      IDictionary<string, string> customHeaders = null);

    /// <summary>
    /// Sends a request to get project extents for a site model from the TRex database.
    /// </summary>
    /// <param name="siteModelID"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    Task<BoundingBox3DGrid> SendProjectExtentsRequest(string siteModelID,
      IDictionary<string, string> customHeaders = null);

    /// <summary>
    /// Sends a request to get a TIN surface data from the TRex database.
    /// </summary>
    /// <param name="compactionSurfaceExportRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    Task<CompactionExportResult> SendSurfaceExportRequest(CompactionSurfaceExportRequest compactionSurfaceExportRequest,
      IDictionary<string, string> customHeaders = null);

    /// <summary>
    /// Sends a request to get Veta format .csv output from TRex.
    /// </summary>
    /// <param name="compactionVetaExportRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    Task<CompactionExportResult> SendVetaExportRequest(CompactionVetaExportRequest compactionVetaExportRequest,
      IDictionary<string, string> customHeaders = null);

    /// <summary>
    /// Sends a request to get PassCount .csv output from TRex.
    /// </summary>
    /// <param name="compactionPassCountExportRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    Task<CompactionExportResult> SendPassCountExportRequest(CompactionPassCountExportRequest compactionPassCountExportRequest,
      IDictionary<string, string> customHeaders = null);

    /// <summary>
    /// Sends a request to get production data patches from the TRex database.
    /// </summary>
    /// <param name="patchDataRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    Task<Stream> SendProductionDataPatchRequest(PatchDataRequest patchDataRequest,
      IDictionary<string, string> customHeaders = null);

    /// <summary>
    /// Sends a request to get station and offset report data from TRex.
    /// </summary>
    /// <param name="stationOffsetRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    Task<Stream> SendStationOffsetReportRequest(CompactionReportStationOffsetTRexRequest stationOffsetRequest,
      IDictionary<string, string> customHeaders = null);

    /// <summary>
    /// Sends a request to get grid report data from TRex.
    /// </summary>
    /// <param name="gridRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    Task<Stream> SendGridReportRequest(CompactionReportGridTRexRequest gridRequest,
      IDictionary<string, string> customHeaders = null);

    /// <summary>
    /// Sends a request to post Coordinate System Definition data to the TRex database.
    /// </summary>
    /// <param name="csdRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    Task<CoordinateSystemSettings> SendPostCSDataRequest(Productivity3D.Models.Models.Coords.CoordinateSystemFile csdRequest,
      IDictionary<string, string> customHeaders = null);

    /// <summary>
    /// Sends a request to validate Coordinate System Definition data to the TRex database.
    /// </summary>
    /// <param name="csdValidationRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    Task<CoordinateSystemSettings> SendCSDataValidationRequest(Productivity3D.Models.Models.Coords.CoordinateSystemFileValidationRequest csdValidationRequest,
      IDictionary<string, string> customHeaders = null);

    /// <summary>
    /// Sends a request to get Coordinate System Definition data from the TRex database.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    Task<CoordinateSystemSettings> SendGetCSDataRequest(ProjectID request,
      IDictionary<string, string> customHeaders = null);

    /// <summary>
    /// Sends a request to the TRex to convert a list of coordinates.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    Task<CoordinateConversionResult> SendCoordinateConversionRequest(CoordinateConversionRequest request,
      IDictionary<string, string> customHeaders = null);
  }
}
