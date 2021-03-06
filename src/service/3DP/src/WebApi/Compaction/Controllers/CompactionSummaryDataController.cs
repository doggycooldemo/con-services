﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VSS.Common.Abstractions.Configuration;
using VSS.Common.Exceptions;
using VSS.MasterData.Models.ResultHandling.Abstractions;
using VSS.Productivity3D.Common.Filters.Authentication;
using VSS.Productivity3D.Common.Interfaces;
using VSS.Productivity3D.Filter.Abstractions.Models;
using VSS.Productivity3D.Models.Enums;
using VSS.Productivity3D.Models.ResultHandling;
using VSS.Productivity3D.Productivity3D.Models.Compaction.ResultHandling;
using VSS.Productivity3D.Project.Abstractions.Interfaces;
using VSS.Productivity3D.WebApi.Compaction.ActionServices;
using VSS.Productivity3D.WebApi.Models.Compaction.ResultHandling;
using VSS.Productivity3D.WebApi.Models.Factories.ProductionData;
using VSS.Productivity3D.WebApi.Models.Report.Executors;
using VSS.Productivity3D.WebApi.Models.Report.Models;
using VSS.Serilog.Extensions;

namespace VSS.Productivity3D.WebApi.Compaction.Controllers
{
  /// <summary>
  /// Controller for getting production data for summary data requests
  /// </summary>
  /// <remarks>
  /// There is no response caching for this provided because at the moment the caching middleware doesn't handle requests with more than one filter.
  /// </remarks>
  [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
  public class CompactionSummaryDataController : CompactionDataBaseController
  {
    /// <summary>
    /// Default constructor.
    /// </summary>
    public CompactionSummaryDataController(IConfigurationStore configStore, IFileImportProxy fileImportProxy, ICompactionSettingsManager settingsManager, IProductionDataRequestFactory requestFactory)
      : base(configStore, fileImportProxy, settingsManager, requestFactory)
    { }

    /// <summary>
    /// Get CMV summary for the specified project and date range.
    /// </summary>
    [ProjectVerifier]
    [Route("api/v2/cmv/summary")]
    [HttpGet]
    public async Task<ActionResult<CompactionCmvSummaryResult>> GetCmvSummary(
      [FromQuery] Guid projectUid,
      [FromQuery] Guid? filterUid)
    {
      Log.LogInformation($"{nameof(GetCmvSummary)}: " + Request.QueryString);

      var request = await GetCmvRequest(projectUid, filterUid);
      request.Validate();

      var validationResult = await ValidateFilterAgainstProjectExtents(projectUid, filterUid);

      if (!validationResult.isValidFilterForProjectExtents)
        return Ok(new CompactionCmvSummaryResult());
      
      Log.LogDebug($"{nameof(GetCmvSummary)} request: " + JsonConvert.SerializeObject(request));
      try
      {
        var result = await RequestExecutorContainerFactory.Build<SummaryCMVExecutor>(LoggerFactory,
            configStore: ConfigStore, trexCompactionDataProxy: TRexCompactionDataProxy, customHeaders: CustomHeaders,
            userId: GetUserId(), fileImportProxy: FileImportProxy)
              .ProcessAsync(request) as CMVSummaryResult;

        var cmvSummaryResult = new CompactionCmvSummaryResult(result, request.CmvSettings);
        Log.LogInformation($"{nameof(GetCmvSummary)} result: " + JsonConvert.SerializeObject(cmvSummaryResult));

        await SetCacheControlPolicy(projectUid);

        return Ok(cmvSummaryResult);
      }
      catch (ServiceException exception)
      {
        Log.LogError($"{nameof(GetCmvSummary)}: {exception.GetResult.Message} ({exception.GetResult.Code})");
        return BadRequest(new ContractExecutionResult(exception.GetResult.Code, exception.GetResult.Message));
      }
      finally
      {
        Log.LogInformation($"{nameof(GetCmvSummary)} returned: " + Response.StatusCode);
      }
    }

    /// <summary>
    /// Get MDP summary for the specified project and date range.
    /// </summary>
    /// when the filter layer method is OffsetFromDesign or OffsetFromProfile.
    [ProjectVerifier]
    [Route("api/v2/mdp/summary")]
    [HttpGet]
    public async Task<ActionResult<CompactionMdpSummaryResult>> GetMdpSummary(
      [FromQuery] Guid projectUid,
      [FromQuery] Guid? filterUid)
    {
      Log.LogInformation($"{nameof(GetMdpSummary)}: " + Request.QueryString);

      var validationResult = await ValidateFilterAgainstProjectExtents(projectUid, filterUid);

      if (!validationResult.isValidFilterForProjectExtents)
        return Ok(new CompactionMdpSummaryResult());

      var filter = validationResult.filterResult == null ? GetCompactionFilter(projectUid, filterUid) : Task.FromResult(validationResult.filterResult);

      var projectId = GetLegacyProjectId(projectUid);
      var projectSettings = GetProjectSettingsTargets(projectUid);

      await Task.WhenAll(filter, projectId, projectSettings);

      var mdpSettings = SettingsManager.CompactionMdpSettings(projectSettings.Result);
      var liftSettings = SettingsManager.CompactionLiftBuildSettings(projectSettings.Result);

      var request = new MDPRequest(projectId.Result, projectUid, null, mdpSettings, liftSettings, filter.Result, -1, null, null, null);
      request.Validate();

      try
      {
        var result = await RequestExecutorContainerFactory.Build<SummaryMDPExecutor>(LoggerFactory,
            configStore: ConfigStore, trexCompactionDataProxy: TRexCompactionDataProxy, customHeaders: CustomHeaders,
            userId: GetUserId(), fileImportProxy: FileImportProxy)
                     .ProcessAsync(request) as MDPSummaryResult;

        var mdpSummaryResult = new CompactionMdpSummaryResult(result, mdpSettings);
        Log.LogInformation($"{nameof(GetMdpSummary)} result: " + JsonConvert.SerializeObject(mdpSummaryResult));

        await SetCacheControlPolicy(projectUid);

        return Ok(mdpSummaryResult);
      }
      catch (ServiceException exception)
      {
        Log.LogError($"{nameof(GetMdpSummary)}: {exception.GetResult.Message} ({exception.GetResult.Code})");
        return BadRequest(new ContractExecutionResult(exception.GetResult.Code, exception.GetResult.Message));
      }
      finally
      {
        Log.LogInformation($"{nameof(GetMdpSummary)} returned: " + Response.StatusCode);
      }
    }

    /// <summary>
    /// Get pass count summary for the specified project and date range.
    /// </summary>
    [ProjectVerifier]
    [Route("api/v2/passcounts/summary")]
    [HttpGet]
    public async Task<ActionResult<CompactionPassCountSummaryResult>> GetPassCountSummary(
      [FromQuery] Guid projectUid,
      [FromQuery] Guid? filterUid)
    {
      Log.LogInformation($"{nameof(GetPassCountSummary)}: " + Request.QueryString);

      var validationResult = await ValidateFilterAgainstProjectExtents(projectUid, filterUid);

      if (!validationResult.isValidFilterForProjectExtents)
        return Ok(new CompactionPassCountSummaryResult());

      var request = await GetPassCountRequest(projectUid, filterUid, validationResult.filterResult, isSummary: true);
      request.Validate();

      try
      {
        var result = await RequestExecutorContainerFactory
                     .Build<SummaryPassCountsExecutor>(LoggerFactory,
        configStore: ConfigStore, trexCompactionDataProxy: TRexCompactionDataProxy, customHeaders: CustomHeaders,
        userId: GetUserId(), fileImportProxy: FileImportProxy)
                     .ProcessAsync(request) as PassCountSummaryResult;

        var passCountSummaryResult = new CompactionPassCountSummaryResult(result);

        Log.LogInformation($"{nameof(GetPassCountSummary)} result: " + JsonConvert.SerializeObject(passCountSummaryResult));

        await SetCacheControlPolicy(projectUid);

        return Ok(passCountSummaryResult);
      }
      catch (ServiceException exception)
      {
        Log.LogError($"{nameof(GetPassCountSummary)}: {exception.GetResult.Message} ({exception.GetResult.Code})");
        return BadRequest(new ContractExecutionResult(exception.GetResult.Code, exception.GetResult.Message));
      }
      finally
      {
        Log.LogInformation($"{nameof(GetPassCountSummary)} returned: " + Response.StatusCode);
      }
    }

    /// <summary>
    /// Get Temperature summary for the specified project and date range.
    /// </summary>
    [ProjectVerifier]
    [Route("api/v2/temperature/summary")]
    [HttpGet]
    public async Task<ActionResult<CompactionTemperatureSummaryResult>> GetTemperatureSummary(
      [FromQuery] Guid projectUid,
      [FromQuery] Guid? filterUid)
    {
      Log.LogInformation($"{nameof(GetTemperatureSummary)}: " + Request.QueryString);

      var validationResult = await ValidateFilterAgainstProjectExtents(projectUid, filterUid);
      if (!validationResult.isValidFilterForProjectExtents)
        return Ok(new CompactionTemperatureSummaryResult());

      var projectId = GetLegacyProjectId(projectUid);
      var projectSettings = GetProjectSettingsTargets(projectUid);

      var filter = validationResult.filterResult == null ? GetCompactionFilter(projectUid, filterUid) : Task.FromResult(validationResult.filterResult);

      await Task.WhenAll(projectId, projectSettings, filter);

      var temperatureSettings = SettingsManager.CompactionTemperatureSettings(projectSettings.Result, false);
      var liftSettings = SettingsManager.CompactionLiftBuildSettings(projectSettings.Result);
     
      var request = new TemperatureRequest(projectId.Result, projectUid, null, temperatureSettings, liftSettings, filter.Result, -1, null, null, null);
      request.Validate();

      try
      {
        var result = await RequestExecutorContainerFactory
                     .Build<SummaryTemperatureExecutor>(LoggerFactory,
            configStore: ConfigStore, trexCompactionDataProxy: TRexCompactionDataProxy, customHeaders: CustomHeaders,
            userId: GetUserId(), fileImportProxy: FileImportProxy)
                     .ProcessAsync(request) as TemperatureSummaryResult;

        var temperatureSummaryResult = new CompactionTemperatureSummaryResult(result);
        Log.LogInformation($"{nameof(GetTemperatureSummary)} result: " + JsonConvert.SerializeObject(temperatureSummaryResult));

        await SetCacheControlPolicy(projectUid);

        return Ok(temperatureSummaryResult);
      }
      catch (ServiceException exception)
      {
        Log.LogError($"{nameof(GetTemperatureSummary)}: {exception.GetResult.Message} ({exception.GetResult.Code})");
        return BadRequest(new ContractExecutionResult(exception.GetResult.Code, exception.GetResult.Message));
      }
      finally
      {
        Log.LogInformation($"{nameof(GetTemperatureSummary)} returned: " + Response.StatusCode);
      }
    }

    /// <summary>
    /// Get Speed summary for the specified project and date range. Either legacy project ID or project UID must be provided.
    /// </summary>
    [ProjectVerifier]
    [Route("api/v2/speed/summary")]
    [HttpGet]
    public async Task<ActionResult<CompactionSpeedSummaryResult>> GetSpeedSummary(
      [FromQuery] Guid projectUid,
      [FromQuery] Guid? filterUid)
    {
      Log.LogInformation($"{nameof(GetSpeedSummary)}: " + Request.QueryString);

      var validationResult = await ValidateFilterAgainstProjectExtents(projectUid, filterUid);
      if (!validationResult.isValidFilterForProjectExtents)
        return Ok(new CompactionSpeedSummaryResult());

      var projectId = GetLegacyProjectId(projectUid);
      var projectSettings = GetProjectSettingsTargets(projectUid);

      var filter = validationResult.filterResult == null ? GetCompactionFilter(projectUid, filterUid) : Task.FromResult(validationResult.filterResult);

      await Task.WhenAll(projectId, projectSettings, filter);

      var liftSettings = SettingsManager.CompactionLiftBuildSettings(projectSettings.Result);
      
      var request = new SummarySpeedRequest(projectId.Result, projectUid, null, liftSettings, filter.Result, -1);
      request.Validate();

      try
      {
        var result = await RequestExecutorContainerFactory
                     .Build<SummarySpeedExecutor>(LoggerFactory,
            configStore: ConfigStore, trexCompactionDataProxy: TRexCompactionDataProxy, customHeaders: CustomHeaders,
            userId: GetUserId(), fileImportProxy: FileImportProxy)
                     .ProcessAsync(request) as SpeedSummaryResult;

        var speedSummaryResult = new CompactionSpeedSummaryResult(result, liftSettings.MachineSpeedTarget);
        Log.LogInformation($"{nameof(GetSpeedSummary)} result: " + JsonConvert.SerializeObject(speedSummaryResult));

        await SetCacheControlPolicy(projectUid);

        return Ok(speedSummaryResult);
      }
      catch (ServiceException exception)
      {
        Log.LogError($"{nameof(GetSpeedSummary)}: {exception.GetResult.Message} ({exception.GetResult.Code})");
        return BadRequest(new ContractExecutionResult(exception.GetResult.Code, exception.GetResult.Message));
      }
      finally
      {
        Log.LogInformation($"{nameof(GetSpeedSummary)} returned: " + Response.StatusCode);
      }
    }

    /// <summary>
    /// Queries design and filter information from the project given undifferentiated UID identifiers for base and top contexts
    /// </summary>
    private async Task<(DesignDescriptor, DesignDescriptor, FilterResult, FilterResult)> 
      ResolveDesignAndFilterReferences(ISummaryDataHelper summaryDataHelper, Guid projectUid, Guid baseUid, Guid topUid)
    {
      DesignDescriptor baseDesign = null;
      DesignDescriptor topDesign = null;
      FilterResult baseFilter = null;
      FilterResult topFilter = null;

      // Base filter...
      var baseFilterDescriptor = await summaryDataHelper.WithSwallowExceptionExecute(async () => await GetFilterDescriptor(projectUid, baseUid));
      if (baseFilterDescriptor == null)
        baseDesign = await summaryDataHelper.WithSwallowExceptionExecute(async () => await GetAndValidateDesignDescriptor(projectUid, baseUid));
      else
        baseFilter = await summaryDataHelper.WithSwallowExceptionExecute(async () => await GetCompactionFilter(projectUid, baseUid));

      // Top filter...
      var topFilterDescriptor = await summaryDataHelper.WithSwallowExceptionExecute(async () => await GetFilterDescriptor(projectUid, topUid));
      if (topFilterDescriptor == null)
        topDesign = await summaryDataHelper.WithSwallowExceptionExecute(async () => await GetAndValidateDesignDescriptor(projectUid, topUid));
      else
        topFilter = await summaryDataHelper.WithSwallowExceptionExecute(async () => await GetCompactionFilter(projectUid, topUid));

      return (baseDesign, topDesign, baseFilter, topFilter);
    }

    /// <summary>
    /// Get the summary volumes report for two surfaces, producing either ground to ground, ground to design or design to ground results.
    /// </summary>
    /// <param name="summaryDataHelper">Volume Summary helper.</param>
    /// <param name="projectUid">The project uid.</param>
    /// <param name="baseUid">The uid for the base surface, either a filter or design.</param>
    /// <param name="topUid">The uid for the top surface, either a filter or design.</param>
    /// <param name="explicitFilters">If true this turns off any implicit filter transformations for summary volumes</param>
    [ProjectVerifier]
    [Route("api/v2/volumes/summary")]
    [HttpGet]
    public async Task<ActionResult<ContractExecutionResult>> GetSummaryVolumes(
      [FromServices] ISummaryDataHelper summaryDataHelper,
      [FromQuery] Guid projectUid,
      [FromQuery] Guid baseUid,
      [FromQuery] Guid topUid,
      [FromQuery] bool explicitFilters = false)
    {
      Log.LogInformation($"{nameof(GetSummaryVolumes)}: " + Request.QueryString);

      if (baseUid == Guid.Empty || topUid == Guid.Empty)
        return BadRequest(new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError, "Invalid surface parameter(s)."));

      var (baseDesign, topDesign, baseFilter, topFilter) = await ResolveDesignAndFilterReferences(summaryDataHelper, projectUid, baseUid, topUid);

      if (baseFilter == null && baseDesign == null || topFilter == null && topDesign == null)
        return BadRequest(new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError, "Can not resolve either baseSurface or topSurface"));

      var volumeCalcType = summaryDataHelper.GetVolumesType(baseFilter, topFilter);

      if (volumeCalcType == VolumesType.None)
        return BadRequest(new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError, "Missing volumes calculation type"));

      var projectId = await GetLegacyProjectId(projectUid);
      var request = SummaryVolumesRequest.CreateAndValidate(projectId, projectUid, baseFilter, topFilter, baseDesign, topDesign, volumeCalcType, explicitFilters);

      CompactionVolumesSummaryResult volumesSummaryResult;

      try
      {
        var result = await RequestExecutorContainerFactory
                     .Build<SummaryVolumesExecutorV2>(LoggerFactory,
            configStore: ConfigStore, trexCompactionDataProxy: TRexCompactionDataProxy, customHeaders: CustomHeaders,
            userId: GetUserId(), fileImportProxy: FileImportProxy)
                     .ProcessAsync(request) as SummaryVolumesResult;

        if (result == null) return Ok(new CompactionVolumesSummaryResult(0, "No production data found"));

        volumesSummaryResult = CompactionVolumesSummaryResult.Create(result, await GetProjectSettingsTargets(projectUid));
      }
      catch (ServiceException exception)
      {
        Log.LogError($"{nameof(GetSummaryVolumes)}: {exception.GetResult.Message} ({exception.GetResult.Code})");
        return BadRequest(new ContractExecutionResult(exception.GetResult.Code, exception.GetResult.Message));
      }
      finally
      {
        Log.LogInformation($"{nameof(GetSummaryVolumes)} returned: " + Response.StatusCode);
      }

      if (Log.IsTraceEnabled())
        Log.LogTrace($"{nameof(GetSummaryVolumes)} result: " + JsonConvert.SerializeObject(volumesSummaryResult));

      return Ok(volumesSummaryResult);
    }

    /// <summary>
    /// Get the summary volumes report for two surfaces, producing either ground to ground, ground to design or design to ground results.
    /// </summary>
    /// <param name="summaryDataHelper">Volume Summary helper.</param>
    /// <param name="projectUid">The project uid.</param>
    /// <param name="baseUid">The uid for the base surface, either a filter or design.</param>
    /// <param name="topUid">The uid for the top surface, either a filter or design.</param>
    /// <param name="startDateUtc">The start date for the series of progressive volumes to be calculated from</param>
    /// <param name="endDateUtc">The end date for the series of progressive volumes to be calculated to</param>
    /// <param name="intervalSeconds">The interval between each calculated volume expressed as a whole number of seconds</param>
    [ProjectVerifier]
    [Route("api/v2/volumes/summary/progressive")]
    [HttpGet]
    public async Task<ActionResult<ContractExecutionResult>> GetProgressiveSummaryVolumes(
      [FromServices] ISummaryDataHelper summaryDataHelper,
      [FromQuery] Guid projectUid,
      [FromQuery] Guid baseUid,
      [FromQuery] Guid topUid,
      [FromQuery] double? cutTolerance,
      [FromQuery] double? fillTolerance,
      [FromQuery] Guid additionalSpatialFilterUid,
      [FromQuery] DateTime startDateUtc,
      [FromQuery] DateTime endDateUtc,
      [FromQuery] int intervalSeconds)
    {
      Log.LogInformation($"{nameof(GetSummaryVolumes)}: " + Request.QueryString);

      if (baseUid == Guid.Empty || topUid == Guid.Empty)
        return BadRequest(new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError, "Invalid surface parameter(s)."));

      var (baseDesign, topDesign, baseFilter, topFilter) = await ResolveDesignAndFilterReferences(summaryDataHelper, projectUid, baseUid, topUid);

      if (baseFilter == null && baseDesign == null || topFilter == null && topDesign == null)
        return BadRequest(new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError, "Can not resolve either baseSurface or topSurface"));

      var volumeCalcType = summaryDataHelper.GetVolumesType(baseFilter, topFilter);

      if (volumeCalcType == VolumesType.None)
        return BadRequest(new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError, "Missing volumes calculation type"));

      var additionalSpatialFilter = await summaryDataHelper.WithSwallowExceptionExecute(async () => await GetCompactionFilter(projectUid, additionalSpatialFilterUid));

      var projectId = await GetLegacyProjectId(projectUid);
      var request = ProgressiveSummaryVolumesRequest.CreateAndValidate(projectId, projectUid, baseFilter ?? topFilter, baseDesign, topDesign, volumeCalcType, cutTolerance, fillTolerance, additionalSpatialFilter, startDateUtc, endDateUtc, intervalSeconds);

      CompactionProgressiveVolumesSummaryResult volumesSummaryResult;

      try
      {
        var result = await RequestExecutorContainerFactory
                     .Build<ProgressiveSummaryVolumesExecutor>(LoggerFactory,
            configStore: ConfigStore, trexCompactionDataProxy: TRexCompactionDataProxy, customHeaders: CustomHeaders)
                     .ProcessAsync(request) as ProgressiveSummaryVolumesResult;

        if (result == null) return Ok(new CompactionProgressiveVolumesSummaryResult(0, "No production data found"));

        volumesSummaryResult = CompactionProgressiveVolumesSummaryResult.Create(result, await GetProjectSettingsTargets(projectUid));
      }
      catch (ServiceException exception)
      {
        Log.LogError($"{nameof(GetProgressiveSummaryVolumes)}: {exception.GetResult.Message} ({exception.GetResult.Code})");
        return BadRequest(new ContractExecutionResult(exception.GetResult.Code, exception.GetResult.Message));
      }
      finally
      {
        Log.LogInformation($"{nameof(GetProgressiveSummaryVolumes)} returned: " + Response.StatusCode);
      }

      if (Log.IsTraceEnabled())
        Log.LogTrace($"{nameof(GetProgressiveSummaryVolumes)} result: " + JsonConvert.SerializeObject(volumesSummaryResult));

      return Ok(volumesSummaryResult);
    }
  }
}
