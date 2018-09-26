﻿using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TAGProcServiceDecls;
using VLPDDecls;
using VSS.Common.Exceptions;
using VSS.MasterData.Models.ResultHandling.Abstractions;
using VSS.Productivity3D.Common.Interfaces;
using VSS.Productivity3D.Common.Proxies;
using VSS.Productivity3D.Common.ResultHandling;
using VSS.Productivity3D.Models.Enums;
using VSS.Productivity3D.Models.Models;
using VSS.Productivity3D.WebApi.Models.Common;
using VSS.Productivity3D.WebApi.Models.Compaction.Helpers;
using VSS.Productivity3D.WebApi.Models.TagfileProcessing.Models;
using VSS.Productivity3D.WebApi.Models.TagfileProcessing.ResultHandling;

namespace VSS.Productivity3D.WebApi.Models.TagfileProcessing.Executors
{
  /// <summary>
  /// For submitting TAG files to Raptor.
  /// </summary>
  public class TagFileNonDirectSubmissionExecutor : RequestExecutorContainer
  {
    /// <summary>
    /// Default constructor for RequestExecutorContainer.Build.
    /// </summary>
    public TagFileNonDirectSubmissionExecutor()
    {
      ProcessErrorCodes();
    }

    protected sealed override void ProcessErrorCodes()
    {
      RaptorResult.AddTagProcessorErrorMessages(ContractExecutionStates);
    }

    protected override async Task<ContractExecutionResult> ProcessAsyncEx<T>(T item)
    {

      var request = item as CompactionTagFileRequestExtended;
      var result = new ContractExecutionResult(ContractExecutionStatesEnum.InternalProcessingError,
        "3dPm Unknown exception.");

      if (!bool.TryParse(configStore.GetValueString("ENABLE_TREX_GATEWAY"), out var useTrexGateway))
      {
        useTrexGateway = false;
      }

      if (!bool.TryParse(configStore.GetValueString("ENABLE_RAPTOR_GATEWAY"), out var useRaptorGateway))
      {
        useRaptorGateway = true;
      }

      if (useTrexGateway)
      {
        request.Validate();

        // gobbles any exception
        result = await CallTRexEndpoint(request).ConfigureAwait(false);

        if (result.Code == 0)
        {
          log.LogDebug($"PostTagFile (NonDirect TRex): Successfully imported TAG file '{request.FileName}'.");
        }
        else
        {
          log.LogDebug(
            $"PostTagFile (NonDirect TRex): Failed to import TAG file '{request.FileName}', {result.Message}");
        }
      }

      if (useRaptorGateway)
      {
        // legacyProjectId must have been retrieved by here else GetLegacyProjectId() would have thrown exception
        var tfRequest = TagFileRequestLegacy.CreateTagFile(request.FileName, request.Data,
          request.ProjectId ?? VelociraptorConstants.NO_PROJECT_ID,
          request.Boundary,
          VelociraptorConstants.NO_MACHINE_ID, false, false, request.OrgId);
        tfRequest.Validate();

        if (tfRequest.ProjectId != VelociraptorConstants.NO_PROJECT_ID && tfRequest.Boundary == null)
        {
          throw new ServiceException(HttpStatusCode.BadRequest,
            new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
              "Failed to process tagfile with error: Manual tag file submissions must include a boundary fence."));
        }

        if (tfRequest.ProjectId == VelociraptorConstants.NO_PROJECT_ID && tfRequest.Boundary != null)
        {
          throw new ServiceException(HttpStatusCode.BadRequest,
            new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
              "Failed to process tagfile with error: Automatic tag file submissions cannot include boundary fence."));
        }

        result = CallRaptorEndpoint(tfRequest);
      }

      return result;
    }

    private async Task<ContractExecutionResult> CallTRexEndpoint(CompactionTagFileRequest request)
    {
      var returnResult = await TagFileHelper.SendTagFileToTRex(request,
        tRexTagFileProxy, log, customHeaders, false).ConfigureAwait(false);

      log.LogInformation($"PostTagFile (NonDirect TRex): result: {JsonConvert.SerializeObject(returnResult)}");

      // todo should the return be split as in CallRaptorEndpoint()
      //    to throw exception for some scenarios, if so which
      return returnResult;
    }

    private ContractExecutionResult CallRaptorEndpoint(TagFileRequestLegacy tfRequest)
    {
      try
      {
        var resultCode = (TTAGProcServerProcessResult) tagProcessor.ProjectDataServerTAGProcessorClient()
          .SubmitTAGFileToTAGFileProcessor
          (tfRequest.FileName,
            new MemoryStream(tfRequest.Data),
            tfRequest.ProjectId ?? VelociraptorConstants.NO_PROJECT_ID, 0, 0, tfRequest.MachineId ?? -1,
            tfRequest.Boundary != null
              ? RaptorConverters.convertWGS84Fence(tfRequest.Boundary)
              : TWGS84FenceContainer.Null(), tfRequest.TccOrgId);

        log.LogInformation($"PostTagFile (NonDirect Raptor): result {resultCode} {ContractExecutionStates.FirstNameWithOffset((int)resultCode)}");

        if (resultCode == TTAGProcServerProcessResult.tpsprOK)
        {
          return TagFilePostResult.Create();
        }
        else
        {
          if (resultCode == TTAGProcServerProcessResult.tpsprOnChooseMachineInvalidSubscriptions)
          {
            var result = new ContractExecutionResult(
              ContractExecutionStates.GetErrorNumberwithOffset((int)resultCode),
              $"Failed to process tagfile with error: {ContractExecutionStates.FirstNameWithOffset((int)resultCode)}");

            log.LogInformation(JsonConvert.SerializeObject(result));

            // Serialize the request ignoring the Data property so not to overwhelm the logs.
            var serializedRequest = JsonConvert.SerializeObject(
              tfRequest,
              Formatting.None,
              new JsonSerializerSettings
              {
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
                ContractResolver = new JsonContractPropertyResolver("Data")
              });

            log.LogInformation("TAG file submission request with file data omitted:" +
                               JsonConvert.SerializeObject(serializedRequest));

            return result;
          }

          throw new ServiceException(HttpStatusCode.BadRequest,
            new ContractExecutionResult(ContractExecutionStates.GetErrorNumberwithOffset((int) resultCode),
              $"Failed to process tagfile with error: {ContractExecutionStates.FirstNameWithOffset((int) resultCode)}"));
        }
      }
      finally
      {
        ContractExecutionStates.ClearDynamic();
      }
    }
    
    protected override ContractExecutionResult ProcessEx<T>(T item)
    {
      throw new NotImplementedException("Use the asynchronous form of this method");
    }
  }
}
