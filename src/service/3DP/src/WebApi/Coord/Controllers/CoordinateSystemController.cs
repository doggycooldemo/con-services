﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VSS.Common.Exceptions;
using VSS.ConfigurationStore;
using VSS.MasterData.Models.ResultHandling.Abstractions;
using VSS.MasterData.Proxies;
using VSS.MasterData.Proxies.Interfaces;
using VSS.Productivity3D.Common.Filters.Authentication;
using VSS.Productivity3D.Common.Filters.Authentication.Models;
using VSS.Productivity3D.Common.Interfaces;
using VSS.Productivity3D.Models.Models;
using VSS.Productivity3D.Models.Models.Coords;
using VSS.Productivity3D.Models.ResultHandling.Coords;
using VSS.Productivity3D.WebApi.Models.Coord.Contracts;
using VSS.Productivity3D.WebApi.Models.Coord.Executors;

namespace VSS.Productivity3D.WebApi.Coord.Controllers
{
  /// <summary>
  /// Controller for the CoordinateSystemFile resource.
  /// </summary>
  [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
  public class CoordinateSystemController : Controller, ICoordinateSystemFileContract
  {
#if RAPTOR
    private readonly IASNodeClient raptorClient;
#endif
    private readonly ILoggerFactory logger;
    private IConfigurationStore configStore;
    private readonly ITRexCompactionDataProxy trexCompactionDataProxy;

    protected IDictionary<string, string> CustomHeaders => Request.Headers.GetCustomHeaders();

    /// <summary>
    /// Constructor with dependency injection
    /// </summary>
    public CoordinateSystemController(
#if RAPTOR
      IASNodeClient raptorClient, 
#endif
      ILoggerFactory logger, IConfigurationStore configStore, ITRexCompactionDataProxy trexCompactionDataProxy)
    {
#if RAPTOR
      this.raptorClient = raptorClient;
#endif
      this.logger = logger;
      this.configStore = configStore;
      this.trexCompactionDataProxy = trexCompactionDataProxy;
    }

    /// <summary>
    /// Posts a coordinate system (CS) definition file to a Raptor's data model/project.
    /// </summary>
    [PostRequestVerifier]
    [Route("api/v1/coordsystem")]
    [HttpPost]
    public CoordinateSystemSettings Post([FromBody]CoordinateSystemFile request)
    {
      request.Validate();

      return RequestExecutorContainerFactory.Build<CoordinateSystemExecutorPost>(logger,
#if RAPTOR
        raptorClient,
#endif
        configStore: configStore, trexCompactionDataProxy: trexCompactionDataProxy, customHeaders: CustomHeaders).Process(request) as CoordinateSystemSettings;
    }

    /// <summary>
    /// Posts a coordinate system (CS) definition file to a Raptor for validation.
    /// </summary>
    [PostRequestVerifier]
    [Route("api/v1/coordsystem/validation")]
    [HttpPost]
    public CoordinateSystemSettings PostValidate([FromBody]CoordinateSystemFileValidationRequest request)
    {
      request.Validate();

      return RequestExecutorContainerFactory.Build<CoordinateSystemExecutorPost>(logger,
#if RAPTOR
        raptorClient,
#endif
        configStore: configStore, trexCompactionDataProxy: trexCompactionDataProxy, customHeaders: CustomHeaders).Process(request) as CoordinateSystemSettings;
    }

    /// <summary>
    /// Gets a coordinate system (CS) definition assigned to a Raptor's data model/project.
    /// </summary>
    [Route("api/v1/projects/{projectId}/coordsystem")]
    [ProjectVerifier]
    [HttpGet]
    public CoordinateSystemSettings Get([FromRoute] long projectId)
    {
      var projectUid = ((RaptorPrincipal)User).GetProjectUid(projectId).Result;
      var request = new ProjectID(projectId, projectUid);

      request.Validate();

      return RequestExecutorContainerFactory.Build<CoordinateSystemExecutorGet>(logger,
#if RAPTOR
        raptorClient,
#endif 
        configStore: configStore, trexCompactionDataProxy: trexCompactionDataProxy, customHeaders: CustomHeaders).Process(request) as CoordinateSystemSettings;
    }

    /// <summary>
    /// Gets a coordinate system (CS) definition assigned to a Raptor's data model/project with a unique identifier.
    /// </summary>
    [Route("api/v2/projects/{projectUid}/coordsystem")]
    [ProjectVerifier]
    [HttpGet]
    public async Task<CoordinateSystemSettings> Get([FromRoute] Guid projectUid)
    {
      long projectId = await ((RaptorPrincipal) User).GetLegacyProjectId(projectUid);
      var request = new ProjectID(projectId, projectUid);

      request.Validate();

      return RequestExecutorContainerFactory.Build<CoordinateSystemExecutorGet>(logger,
#if RAPTOR
        raptorClient,
#endif
        configStore: configStore, trexCompactionDataProxy: trexCompactionDataProxy, customHeaders: CustomHeaders).Process(request) as CoordinateSystemSettings;
    }

    /// <summary>
    /// Posts a list of coordinates to a Raptor's data model/project for conversion.
    /// </summary>
    [PostRequestVerifier]
    [Route("api/v1/coordinateconversion")]
    [HttpPost]
    public CoordinateConversionResult Post([FromBody]CoordinateConversionRequest request)
    {
      request.Validate();

      return RequestExecutorContainerFactory.Build<CoordinateConversionExecutor>(logger,
#if RAPTOR
        raptorClient,
#endif
        configStore: configStore, trexCompactionDataProxy: trexCompactionDataProxy, customHeaders: CustomHeaders).Process(request) as CoordinateConversionResult;
    }
  }
}
