﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VSS.Common.Abstractions.Configuration;
using VSS.ConfigurationStore;
using VSS.MasterData.Models.Handlers;
using VSS.MasterData.Models.ResultHandling.Abstractions;
using VSS.Productivity3D.Models.Models.Coords;
using VSS.Productivity3D.Models.ResultHandling.Coords;
using VSS.TRex.Gateway.Common.Executors;
using VSS.TRex.Gateway.Common.Executors.Coords;

namespace VSS.TRex.Mutable.Gateway.WebApi.Controllers
{
  /// <summary>
  /// Controller for adding coordinate system definition data to a site model/project
  /// and performing coordinates conversion. 
  /// </summary>
  public class CoordinateSystemController : BaseController
  {
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <param name="serviceExceptionHandler"></param>
    /// <param name="configStore"></param>
    public CoordinateSystemController(ILoggerFactory loggerFactory, IServiceExceptionHandler serviceExceptionHandler, IConfigurationStore configStore)
      : base(loggerFactory, loggerFactory.CreateLogger<CoordinateSystemController>(), serviceExceptionHandler, configStore)
    {
    }

    /// <summary>
    /// Posts a coordinate system (CS) definition file to a TRex's data model/project.
    /// </summary>
    [Route("api/v1/coordsystem")]
    [HttpPost]
    public Task<ContractExecutionResult> PostCoordinateSystem([FromBody] CoordinateSystemFile request)
    {
      Log.LogInformation($"{nameof(PostCoordinateSystem)}: {Request.QueryString}");

      request.Validate();

      return WithServiceExceptionTryExecuteAsync(() =>
        RequestExecutorContainer
          .Build<CoordinateSystemPostExecutor>(ConfigStore, LoggerFactory, ServiceExceptionHandler)
          .ProcessAsync(request));
    }
  }
}
