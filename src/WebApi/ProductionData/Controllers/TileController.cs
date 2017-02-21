﻿using System.Linq;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using VSS.Raptor.Service.Common.Filters;
using VSS.Raptor.Service.Common.Filters.Authentication;
using VSS.Raptor.Service.Common.Interfaces;
using VSS.Raptor.Service.WebApiModels.ProductionData.Contracts;
using VSS.Raptor.Service.WebApiModels.ProductionData.Executors;
using VSS.Raptor.Service.WebApiModels.ProductionData.Models;
using VSS.Raptor.Service.WebApiModels.ProductionData.ResultHandling;


namespace VSS.Raptor.Service.WebApi.ProductionData.Controllers
{
 
  public class TileController : Controller, ITileContract
  {
    /// <summary>
    /// Raptor client for use by executor
    /// </summary>
    private readonly IASNodeClient raptorClient;

    /// <summary>
    /// Logger for logging
    /// </summary>
    private readonly ILogger log;

    /// <summary>
    /// Logger factory for use by executor
    /// </summary>
    private readonly ILoggerFactory logger;

    /// <summary>
    /// Constructor with injected raptor client and logger
    /// </summary>
    /// <param name="raptorClient">Raptor client</param>
    /// <param name="logger">Logger</param>
    public TileController(IASNodeClient raptorClient, ILoggerFactory logger)
    {
      this.raptorClient = raptorClient;
      this.logger = logger;
      this.log = logger.CreateLogger<TileController>();
    }

    /// <summary>
    /// Supplies tiles of rendered overlays for a number of different thematic sets of data held in a project such as elevation, compaction, temperature, cut/fill, volumes etc
    /// </summary>
    /// <param name="request">A representation of the tile rendering request.</param>
    /// <returns>An HTTP response containing an error code is there is a failure, or a PNG image if the request suceeds.</returns>
    /// <executor>TilesExecutor</executor> 
    [ProjectIdVerifier]
    [NotLandFillProjectVerifier]
    [ProjectUidVerifier]
    [NotLandFillProjectWithUIDVerifier]
    [System.Web.Http.Route("api/v1/tiles")]
    [System.Web.Http.HttpPost]

    public dynamic Post([System.Web.Http.FromBody] TileRequest request)
    {
      request.Validate();
      var tileResult = RequestExecutorContainer.Build<TilesExecutor>(logger, raptorClient, null).Process(request) as TileResult;

      //if (Request.Headers["Accept"].Contains(new MediaTypeWithQualityHeaderValue("image/png")))
      var acceptHeader = Request.Headers["Accept"];
      if (!StringValues.IsNullOrEmpty(acceptHeader) && acceptHeader.Contains("image/png"))
      {
        return new BinaryImageResponseContainer()
        {
          payload = tileResult.TileData,
          code = tileResult.TileOutsideProjectExtents.ToString()
        };
      }

      return tileResult;
    }

    /// <summary>
    /// This requests returns raw array of bytes with PNG without any diagnostic information. If it fails refer to the request with disgnostic info.
    /// Supplies tiles of rendered overlays for a number of different thematic sets of data held in a project such as elevation, compaction, temperature, cut/fill, volumes etc
    /// </summary>
    /// <param name="request">A representation of the tile rendering request.</param>
    /// <returns>An HTTP response containing an error code is there is a failure, or a PNG image if the request succeeds. If the size of a pixel in the rendered tile coveres more than 10.88 meters in width or height, then the pixel will be rendered in a 'representational style' where black (currently, but there is a work item to allow this to be configurable) is used to indicate the presense of data. Representational style rendering performs no filtering what so ever on the data.10.88 meters is 32 (number of cells across a subgrid) * 0.34 (default width in meters of a single cell).</returns>
    /// <executor>TilesExecutor</executor> 
    [ProjectIdVerifier]
    [NotLandFillProjectVerifier]
    [ProjectUidVerifier]
    [NotLandFillProjectWithUIDVerifier]
    [System.Web.Http.Route("api/v1/tiles/png")]
    [System.Web.Http.HttpPost]

    public BinaryImageResponseContainer PostRaw([System.Web.Http.FromBody] TileRequest request)
    {
      request.Validate();
      var tileResult = RequestExecutorContainer.Build<TilesExecutor>(logger, raptorClient, null).Process(request) as TileResult;
      if (tileResult != null)
        return new BinaryImageResponseContainer()
        {
          payload = tileResult.TileData,
          code = tileResult.TileOutsideProjectExtents.ToString()
        };
      return null;
    }
  }
}