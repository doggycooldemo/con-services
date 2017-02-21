﻿
using System.Linq;
using System.Net;
using Microsoft.Extensions.Logging;
using VLPDDecls;
using VSS.Raptor.Service.WebApiModels.Coord.Models;
using VSS.Raptor.Service.WebApiModels.Coord.ResultHandling;
using VSS.Raptor.Service.Common.Contracts;
using VSS.Raptor.Service.Common.Interfaces;
using VSS.Raptor.Service.Common.Proxies;
using VSS.Raptor.Service.Common.ResultHandling;

namespace VSS.Raptor.Service.WebApiModels.Coord.Executors
{
  /// <summary>
  /// Coordinate conversion executor.
  /// </summary>
  /// 
  public class CoordinateConversionExecutor : RequestExecutorContainer
  {
    /// <summary>
    /// This constructor allows us to mock raptorClient
    /// </summary>
    /// <param name="raptorClient"></param>
    /// 
    public CoordinateConversionExecutor(ILoggerFactory logger, IASNodeClient raptorClient) : base(logger, raptorClient)
    {
    }

    /// <summary>
    /// Default constructor for RequestExecutorContainer.Build
    /// </summary>
    public CoordinateConversionExecutor()
    {
    }

    /// <summary>
    /// Populates ContractExecutionStates with Production Data Server error messages.
    /// </summary>
    /// 
    protected override void ProcessErrorCodes()
    {
      RaptorResult.AddErrorMessages(ContractExecutionStates);
    }

    /// <summary>
    /// Coordinate conversion executor (Post).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item">A Domain object.</param>
    /// <returns></returns>
    /// 
    protected override ContractExecutionResult ProcessEx<T>(T item)
    {
      ContractExecutionResult result = null;

      if ((object)item != null)
      {
        try
        {
          CoordinateConversionRequest request = item as CoordinateConversionRequest;

          TWGS84FenceContainer latLongs = new TWGS84FenceContainer() { FencePoints = request.conversionCoordinates.Select(cc => TWGS84Point.Point(cc.x, cc.y)).ToArray() };

          TCoordPointList pointList;

          TCoordReturnCode code = raptorClient.GetGridCoordinates
            (
              request.projectId ?? -1, 
              latLongs, 
              request.conversionType == TwoDCoordinateConversionType.LatLonToNorthEast ? TCoordConversionType.ctLLHtoNEE : TCoordConversionType.ctNEEtoLLH, 
              out pointList
            );

          if (code == TCoordReturnCode.nercNoError)
            result = ExecutionResult(pointList.Points.Coords);
          else
          {
            throw new ServiceException(HttpStatusCode.BadRequest, new ContractExecutionResult(ContractExecutionStatesEnum.FailedToGetResults,
                                       string.Format("Failed to process coordinate conversion request with error: {0}.", ContractExecutionStates.FirstNameWithOffset((int)code))));
          }
        }
        finally
        {
          ContractExecutionStates.ClearDynamic();
        }
      }
      else
      {
        throw new ServiceException(HttpStatusCode.BadRequest,
          new ContractExecutionResult(ContractExecutionStatesEnum.InternalProcessingError,
            "No  coordinate conversion request sent."));
      }

      return result;
    }

    /// <summary>
    /// Returns an instance of the ContractExecutionResult class as POST method execution result.
    /// </summary>
    /// <returns>An instance of the ContractExecutionResult class.</returns>
    /// 
    private ContractExecutionResult ExecutionResult(TCoordPoint[] pointList)
    {
      TwoDConversionCoordinate[] convertedPoints = pointList != null ? pointList.Select(cp => TwoDConversionCoordinate.CreateTwoDConversionCoordinate(cp.X, cp.Y)).ToArray() : null;

      return CoordinateConversionResult.CreateCoordinateConversionResult(convertedPoints);
    }
  

  }
}