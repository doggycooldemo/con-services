﻿using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VSS.Common.Abstractions.Configuration;
using VSS.Common.Exceptions;
using VSS.MasterData.Models.Handlers;
using VSS.MasterData.Models.ResultHandling.Abstractions;
using VSS.Productivity3D.Models.Models.Profiling;
using VSS.Productivity3D.Models.ResultHandling.Profiling;
using VSS.TRex.Common;
using VSS.TRex.Common.Models;
using VSS.TRex.Designs.GridFabric.Arguments;
using VSS.TRex.Designs.GridFabric.Responses;
using VSS.TRex.Designs.Models;
using XYZS = VSS.Productivity3D.Models.ResultHandling.XYZS;


namespace VSS.TRex.Gateway.Common.Executors.Design
{
  /// <summary>
  /// Processes the request to get design profile lines.
  /// </summary>
  public class DesignProfileExecutor : BaseExecutor
  {
    public DesignProfileExecutor(IConfigurationStore configStore, ILoggerFactory logger,
      IServiceExceptionHandler exceptionHandler)
      : base(configStore, logger, exceptionHandler)
    {
    }

    /// <summary>
    /// Default constructor for RequestExecutorContainer.Build
    /// </summary>
    public DesignProfileExecutor()
    {
    }

    protected override async Task<ContractExecutionResult> ProcessAsyncEx<T>(T item)
    {
      var request = item as DesignProfileRequest;
      
      if (request == null)
        ThrowRequestTypeCastException<DesignProfileRequest>();

      var siteModel = GetSiteModel(request.ProjectUid);

      var designProfileRequest = new VSS.TRex.Designs.GridFabric.Requests.DesignProfileRequest();
      var referenceDesign = new DesignOffset(request.DesignUid, request.Offset);

      var designProfileResponse = await designProfileRequest.ExecuteAsync(new CalculateDesignProfileArgument
      {
        ProjectID = siteModel.ID,
        ReferenceDesign = referenceDesign,
        CellSize = siteModel.CellSize,
        StartPoint = new WGS84Point(request.StartX, request.StartY, request.PositionsAreGrid ? Consts.NullDouble : 0),//coord conversion requires elevation set
        EndPoint = new WGS84Point(request.EndX, request.EndY, request.PositionsAreGrid ? Consts.NullDouble : 0),
        PositionsAreGrid = request.PositionsAreGrid
      });

      if (designProfileResponse != null)
        return ConvertResult(designProfileResponse);

      throw new ServiceException(HttpStatusCode.BadRequest, new ContractExecutionResult(ContractExecutionStatesEnum.FailedToGetResults,
        "Failed to get requested Design Profile data"));
    }

    /// <summary>
    /// Converts CalculateDesignProfileResponse into CalculateDesignProfileResult data.
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    private DesignProfileResult ConvertResult(CalculateDesignProfileResponse result)
    {
      return new DesignProfileResult(result.Profile.Select(x => new XYZS
      {
        X = x.X, Y = x.Y, Z = x.Z, Station = x.Station
      }).ToList());
    }

    /// <summary>
    /// Processes the tile request synchronously.
    /// </summary>
    protected override ContractExecutionResult ProcessEx<T>(T item)
    {
      throw new NotImplementedException("Use the asynchronous form of this method");
    }
  }
}
