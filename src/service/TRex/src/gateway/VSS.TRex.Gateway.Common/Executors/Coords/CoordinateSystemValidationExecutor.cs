﻿using System.Net;
using Microsoft.Extensions.Logging;
using VSS.Common.Exceptions;
using VSS.ConfigurationStore;
using VSS.MasterData.Models.Handlers;
using VSS.MasterData.Models.Models;
using VSS.MasterData.Models.ResultHandling.Abstractions;
using VSS.TRex.CoordinateSystems;

namespace VSS.TRex.Gateway.Common.Executors.Coords
{
  /// <summary>
  /// Processes the request to validate coordinate system definition data.
  /// </summary>
  public class CoordinateSystemValidationExecutor : CoordinateSystemBaseExecutor
  {
    public CoordinateSystemValidationExecutor(IConfigurationStore configStore, ILoggerFactory logger, IServiceExceptionHandler exceptionHandler)
      : base(configStore, logger, exceptionHandler)
    {
    }

    /// <summary>
    /// Default constructor for RequestExecutorContainer.Build
    /// </summary>
    public CoordinateSystemValidationExecutor()
    {
    }

    protected override ContractExecutionResult ProcessEx<T>(T item)
    {
      var request = item as CoordinateSystemFileValidationRequest;

      if (request == null)
        ThrowRequestTypeCastException<CoordinateSystemFileValidationRequest>();

      var csd = ConvertCoordinates.DCFileContentToCSD(request.CSFileName, request.CSFileContent);

      if (csd.CoordinateSystem != null && csd.CoordinateSystem.ZoneInfo != null && csd.CoordinateSystem.DatumInfo != null)
        return ConvertResult(request.CSFileName, csd.CoordinateSystem);

      throw new ServiceException(HttpStatusCode.BadRequest, new ContractExecutionResult(ContractExecutionStatesEnum.FailedToGetResults,
        $"Failed to convert DC File {request.CSFileName} content to Coordinate System definition data."));
    }

  }
}
