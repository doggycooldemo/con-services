﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Repositories;
using WebApiModels.Executors;
using WebApiModels.Models;
using WebApiModels.ResultHandling;

namespace WebApi.Controllers
{
  public class AssetController : Controller
  {
    private readonly IRepositoryFactory factory;
    private readonly ILogger log;

    public AssetController(IRepositoryFactory factory, ILogger<AssetController> logger)
    {
      this.factory = factory;
      this.log = logger;
    }

    /// <summary>
    ///   Gets the legacyAssetID and serviceType satisfying requirements
    ///      for the device or project - whichever is provided
    ///  
    ///      ProjectID is -1 for auto processing of tag files and non-zero for manual processing.
    ///      Radio serial may not be present in the tag file. The logic below replaces the 'john doe' handling in Raptor for these tag files.
    ///      Special case: Allow manual import of tag file if user has manual 3D subscription.
    /// </summary>
    /// <returns>AssetId and/or serviceType and True for success, 
    ///           AssetId==-1, serviceType = 0 and False for failure</returns>
    /// <executor>AssetIdExecutor</executor>
    [Route("api/v1/asset/getId")]
    [HttpPost]
    public GetAssetIdResult GetAssetId([FromBody]GetAssetIdRequest request)
    {
      log.LogDebug("GetAssetId: request:{0}", JsonConvert.SerializeObject(request) );            
      request.Validate();

      var result = RequestExecutorContainer.Build<AssetIdExecutor>(factory, log).Process(request) as GetAssetIdResult;

      if (result.result)
      {
        var infoMessage = string.Format("GetAssetId: was processed successfully: Request {0} Result {1}", JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(result));
        log.LogInformation(infoMessage);
      }
      else
      {
        var errorMessage = string.Format("GetAssetId: failed to be processed: Request {0} Result {1}", JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(result));
        log.LogError(errorMessage);
      }

      return result;
    }
  }
}
