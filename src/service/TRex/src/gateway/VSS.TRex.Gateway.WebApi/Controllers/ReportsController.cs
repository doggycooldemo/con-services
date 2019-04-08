﻿using System.IO;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VSS.Common.Exceptions;
using VSS.ConfigurationStore;
using VSS.MasterData.Models.Handlers;
using VSS.MasterData.Models.ResultHandling.Abstractions;
using VSS.Productivity3D.Models.Models.Reports;
using VSS.TRex.Gateway.Common.Executors;
using VSS.TRex.Gateway.Common.Helpers;
using VSS.TRex.Gateway.Common.ResultHandling;
using VSS.TRex.Gateway.WebApi.ActionServices;

namespace VSS.TRex.Gateway.WebApi.Controllers
{
  /// <summary>
  /// Controller for getting report data.
  /// </summary>
  public class ReportsController : BaseController
  {
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <param name="serviceExceptionHandler"></param>
    /// <param name="configStore"></param>
    public ReportsController(ILoggerFactory loggerFactory, IServiceExceptionHandler serviceExceptionHandler, IConfigurationStore configStore)
      : base(loggerFactory, loggerFactory.CreateLogger<DetailsDataController>(), serviceExceptionHandler, configStore)
    {
    }

    /// <summary>
    /// Get station-offset report stream for the specified project, filter etc.
    /// </summary>
    /// <param name="reportStationOffsetRequest"></param>
    /// <param name="reportDataValidationUtility"></param>
    /// <returns></returns>
    [Route("api/v1/report/stationoffset")]
    [HttpPost]
    public FileResult PostStationOffsetReport(
      [FromBody] CompactionReportStationOffsetTRexRequest reportStationOffsetRequest,
      [FromServices] IReportDataValidationUtility reportDataValidationUtility)
    {
      Log.LogInformation($"{nameof(PostStationOffsetReport)}: {Request.QueryString}");

      reportStationOffsetRequest.Validate();
      var siteModel = GatewayHelper.ValidateAndGetSiteModel(reportStationOffsetRequest.ProjectUid, nameof(PostStationOffsetReport));
      reportDataValidationUtility.ValidateData((object) reportStationOffsetRequest, siteModel);
      if (reportStationOffsetRequest.Filter != null && reportStationOffsetRequest.Filter.ContributingMachines != null)
        GatewayHelper.ValidateMachines(reportStationOffsetRequest.Filter.ContributingMachines.Select(m => m.AssetUid).ToList(), siteModel);

      var stationOffsetReportDataResult = WithServiceExceptionTryExecute(() =>
        RequestExecutorContainer
          .Build<StationOffsetReportExecutor>(ConfigStore, LoggerFactory, ServiceExceptionHandler)
          .Process(reportStationOffsetRequest) as GriddedReportDataResult);

      if (stationOffsetReportDataResult?.GriddedData == null)
      {
        var code = stationOffsetReportDataResult == null ? HttpStatusCode.BadRequest : HttpStatusCode.NoContent;
        var exCode = stationOffsetReportDataResult == null ? ContractExecutionStatesEnum.FailedToGetResults : ContractExecutionStatesEnum.ValidationError;

        throw new ServiceException(code, new ContractExecutionResult(exCode, $"Failed to get stationOffset report data for projectUid: {reportStationOffsetRequest.ProjectUid}"));
      }

      return new FileStreamResult(new MemoryStream(stationOffsetReportDataResult?.GriddedData), "application/octet-stream");
    }

    /// <summary>
    /// Get grid report for the specified project, filter etc.
    /// </summary>
    /// <param name="reportGridRequest"></param>
    /// <param name="reportDataValidationUtility"></param>
    /// <returns></returns>
    [Route("api/v1/report/grid")]
    [HttpPost]
    public FileResult PostGriddedReport(
      [FromBody] CompactionReportGridTRexRequest reportGridRequest,
      [FromServices] IReportDataValidationUtility reportDataValidationUtility)
    {
      Log.LogInformation($"{nameof(PostGriddedReport)}: {Request.QueryString}");

      reportGridRequest.Validate();
      var siteModel = GatewayHelper.ValidateAndGetSiteModel(reportGridRequest.ProjectUid, nameof(PostGriddedReport));
      reportDataValidationUtility.ValidateData((object)reportGridRequest, siteModel);
      if (reportGridRequest.Filter != null && reportGridRequest.Filter.ContributingMachines != null)
        GatewayHelper.ValidateMachines(reportGridRequest.Filter.ContributingMachines.Select(m => m.AssetUid).ToList(), siteModel);


      var griddedReportDataResult =  WithServiceExceptionTryExecute(() =>
        RequestExecutorContainer
          .Build<GriddedReportExecutor>(ConfigStore, LoggerFactory, ServiceExceptionHandler)
          .Process(reportGridRequest) as GriddedReportDataResult);

      if (griddedReportDataResult?.GriddedData == null)
      {
        var code = griddedReportDataResult == null ? HttpStatusCode.BadRequest : HttpStatusCode.NoContent;
        var exCode = griddedReportDataResult == null ? ContractExecutionStatesEnum.FailedToGetResults : ContractExecutionStatesEnum.ValidationError;

        throw new ServiceException(code, new ContractExecutionResult(exCode, $"Failed to get gridded report data for projectUid: {reportGridRequest.ProjectUid}"));
      }

      return new FileStreamResult(new MemoryStream(griddedReportDataResult?.GriddedData), "application/octet-stream");
    }
  }
}
