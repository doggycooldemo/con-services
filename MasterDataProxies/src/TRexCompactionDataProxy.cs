﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VSS.ConfigurationStore;
using VSS.MasterData.Models.ResultHandling.Abstractions;
using VSS.MasterData.Proxies.Interfaces;
using VSS.Productivity3D.Models.Models;
using VSS.Productivity3D.Models.ResultHandling;

namespace VSS.MasterData.Proxies
{
  /// <summary>
  /// Proxy to access the TRex Gateway WebAPIs.
  /// </summary>
  public class TRexCompactionDataProxy : BaseProxy, ITRexCompactionDataProxy
  {
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="configurationStore"></param>
    /// <param name="logger"></param>
    public TRexCompactionDataProxy(IConfigurationStore configurationStore, ILoggerFactory logger) : base(configurationStore, logger)
    {
    }

    /// <summary>
    /// Sends a request to get CMV % Change statistics from the TRex database.
    /// </summary>
    /// <param name="cmvChangeDetailsRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    public async Task<ContractExecutionResult> SendCMVChangeDetailsRequest(CMVChangeDetailsRequest cmvChangeDetailsRequest, IDictionary<string, string> customHeaders = null)
    {
      var request = JsonConvert.SerializeObject(cmvChangeDetailsRequest);

      log.LogDebug($"{nameof(SendCMVChangeDetailsRequest)}: Sending the request: {request}");

      return await SendRequestPost<CMVChangeSummaryResult>(request, customHeaders, "/cmv/percentchange");
    }

    /// <summary>
    /// Sends a request to get CMV Details statistics from the TRex database.
    /// </summary>
    /// <param name="cmvDetailsRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    public async Task<ContractExecutionResult> SendCMVDetailsRequest(CMVDetailsRequest cmvDetailsRequest, IDictionary<string, string> customHeaders = null)
    {
      var request = JsonConvert.SerializeObject(cmvDetailsRequest);

      log.LogDebug($"{nameof(SendCMVDetailsRequest)}: Sending the request: {request}");

      return await SendRequestPost<CMVDetailedResult>(request, customHeaders, "/cmv/details");
    }

    /// <summary>
    /// Sends a request to get CMV Summary statistics from the TRex database.
    /// </summary>
    /// <param name="cmvSummaryRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    public async Task<ContractExecutionResult> SendCMVSummaryRequest(CMVSummaryRequest cmvSummaryRequest, IDictionary<string, string> customHeaders = null)
    {
      var request = JsonConvert.SerializeObject(cmvSummaryRequest);

      log.LogDebug($"{nameof(SendCMVSummaryRequest)}: Sending the request: {request}");

      return await SendRequestPost<CMVSummaryResult>(request, customHeaders, "/cmv/summary");
    }

    /// <summary>
    /// Sends a request to get Pass Count Details statistics from the TRex database.
    /// </summary>
    /// <param name="pcDetailsRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    public async Task<ContractExecutionResult> SendPassCountDetailsRequest(PassCountDetailsRequest pcDetailsRequest, IDictionary<string, string> customHeaders = null)
    {
      var request = JsonConvert.SerializeObject(pcDetailsRequest);

      log.LogDebug($"{nameof(SendPassCountDetailsRequest)}: Sending the request: {request}");

      return await SendRequestPost<PassCountDetailedResult>(request, customHeaders, "/passcounts/details");
    }

    /// <summary>
    /// Sends a request to get Pass Count Summary statistics from the TRex database.
    /// </summary>
    /// <param name="pcSummaryRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    public async Task<ContractExecutionResult> SendPassCountSummaryRequest(PassCountSummaryRequest pcSummaryRequest, IDictionary<string, string> customHeaders = null)
    {
      var request = JsonConvert.SerializeObject(pcSummaryRequest);

      log.LogDebug($"{nameof(SendPassCountSummaryRequest)}: Sending the request: {request}");

      return await SendRequestPost<PassCountSummaryResult>(request, customHeaders, "/passcounts/summary");
    }

    /// <summary>
    /// Sends a request to get Cut/Fill Details statistics from the TRex database.
    /// </summary>
    /// <param name="cfDetailsRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    public async Task<ContractExecutionResult> SendCutFillDetailsRequest(CutFillDetailsRequest cfDetailsRequest, IDictionary<string, string> customHeaders = null)
    {
      var request = JsonConvert.SerializeObject(cfDetailsRequest);

      log.LogDebug($"{nameof(SendCutFillDetailsRequest)}: Sending the request: {request}");

      return await SendRequestPost<CompactionCutFillDetailedResult>(request, customHeaders, "/cutfill/details");
    }

    /// <summary>
    /// Sends a request to get MDP Summary statistics from the TRex database.
    /// </summary>
    /// <param name="mdpSummaryRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    public async Task<ContractExecutionResult> SendMDPSummaryRequest(MDPSummaryRequest mdpSummaryRequest, IDictionary<string, string> customHeaders = null)
    {
      var request = JsonConvert.SerializeObject(mdpSummaryRequest);

      log.LogDebug($"{nameof(SendMDPSummaryRequest)}: Sending the request: {request}");

      return await SendRequestPost<MDPSummaryResult>(request, customHeaders, "/mdp/summary");
    }

    /// <summary>
    /// Sends a request to get Material Temperature Summary statistics from the TRex database.
    /// </summary>
    /// <param name="temperatureSummaryRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    public async Task<ContractExecutionResult> SendTemperatureSummaryRequest(TemperatureSummaryRequest temperatureSummaryRequest,
      IDictionary<string, string> customHeaders = null)
    {
      var request = JsonConvert.SerializeObject(temperatureSummaryRequest);

      log.LogDebug($"{nameof(SendTemperatureSummaryRequest)}: Sending the request: {request}");

      return await SendRequestPost<TemperatureSummaryResult>(request, customHeaders, "/temperature/summary");
    }

    /// <summary>
    /// Sends a request to get Machine Speed Summary statistics from the TRex database.
    /// </summary>
    /// <param name="speedSummaryRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    public async Task<ContractExecutionResult> SendSpeedSummaryRequest(SpeedSummaryRequest speedSummaryRequest, IDictionary<string, string> customHeaders = null)
    {
      var request = JsonConvert.SerializeObject(speedSummaryRequest);

      log.LogDebug($"{nameof(SendSpeedSummaryRequest)}: Sending the request: {request}");

      return await SendRequestPost<SpeedSummaryResult>(request, customHeaders, "/speed/summary");
    }

    /// <summary>
    /// Sends a request to get production data tile from the TRex database.
    /// </summary>
    /// <param name="tileRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    public async Task<ContractExecutionResult> SendProductionDataTileRequest(TileRequest tileRequest, IDictionary<string, string> customHeaders = null)
    {
      var request = JsonConvert.SerializeObject(tileRequest);

      log.LogDebug($"{nameof(SendSpeedSummaryRequest)}: Sending the request: {request}");

      return await SendRequestPost<TileResult>(request, customHeaders, "/tile");
    }

    /// <summary>
    /// Sends a request to get Summary Volumes statistics from the TRex database.
    /// </summary>
    /// <param name="summaryVolumesRequest"></param>
    /// <param name="customHeaders"></param>
    /// <returns></returns>
    public async Task<ContractExecutionResult> SendSummaryVolumesRequest(SummaryVolumesDataRequest summaryVolumesRequest, IDictionary<string, string> customHeaders = null)
    {
      var request = JsonConvert.SerializeObject(summaryVolumesRequest);

      log.LogDebug($"{nameof(SendSummaryVolumesRequest)}: Sending the request: {request}");

      return await SendRequestPost<SummaryVolumesResult>(request, customHeaders, "/volumes/summary");
    }

    /// <summary>
    /// Executes a POST request against the TRex Gateway service.
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="customHeaders"></param>
    /// <param name="route"></param>
    /// <returns></returns>
    private async Task<T> SendRequestPost<T>(string payload, IDictionary<string, string> customHeaders, string route) where T : ContractExecutionResult
    {
      var response = await SendRequest<T>("TREX_GATEWAY_API_URL", payload, customHeaders, route, "POST", string.Empty);

      log.LogDebug($"{nameof(SendRequestPost)}: response: {(response == null ? null : JsonConvert.SerializeObject(response))}");

      return response;
    }
  }
}
