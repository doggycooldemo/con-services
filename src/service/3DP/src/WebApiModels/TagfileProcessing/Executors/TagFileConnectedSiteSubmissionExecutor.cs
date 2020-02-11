﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VSS.MasterData.Models.ResultHandling.Abstractions;
using VSS.Productivity3D.Common.Interfaces;
using VSS.Productivity3D.Models.Models;

namespace VSS.Productivity3D.WebApi.Models.TagfileProcessing.Executors
{
  public class TagFileConnectedSiteSubmissionExecutor : RequestExecutorContainer
  {
    public const string DISABLED_MESSAGE = "Connected Site Disabled";
    public const string DEFAULT_ERROR_MESSAGE = "Unknown exception.";
    protected override async Task<ContractExecutionResult> ProcessAsyncEx<T>(T item)
    {
      var request = CastRequestObjectTo<CompactionTagFileRequest>(item);
      
      var result = new ContractExecutionResult(ContractExecutionStatesEnum.ExecutedSuccessfully, DISABLED_MESSAGE);
      
      // Send the tagfile to the connected site gateway if enabled first, no project/subscription validation is required.
      if (configStore.GetValueBool("ENABLE_CONNECTED_SITE_GATEWAY") ?? false)
      {
        request.Validate();
        result = await CallConnectedSiteEndpoint(request);
      }
      return result;
    }

    private async Task<ContractExecutionResult> CallConnectedSiteEndpoint(CompactionTagFileRequest request)
    {
      // connectedSite is in tRexConnectedSiteProxy because it uses some tRex code
      var connectedSiteResult = await tRexConnectedSiteProxy.SendTagFileNonDirectToConnectedSite(request, customHeaders);
      log.LogInformation($"{nameof(CallConnectedSiteEndpoint)}: result: {JsonConvert.SerializeObject(connectedSiteResult)}");
      return connectedSiteResult;
    }
  }
}
