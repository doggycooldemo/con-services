﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VSS.Common.Abstractions.Configuration;
using VSS.Common.Exceptions;
using VSS.MasterData.Models.Handlers;
using VSS.MasterData.Models.Models;
using VSS.MasterData.Models.ResultHandling.Abstractions;
using VSS.Productivity3D.Models.Models.Files;
using VSS.TRex.CoordinateSystems;
using VSS.TRex.DI;
using VSS.TRex.Files.DXF;
using VSS.TRex.Gateway.Common.ResultHandling;
using VSS.TRex.Geometry;
using VSS.TRex.Types;

namespace VSS.TRex.Gateway.Common.Executors.Files
{
  /// <summary>
  /// Processes the request to get boundaries from DXF
  /// </summary>
  public class ExtractDXFBoundariesExecutor : BaseExecutor
  {
    public ExtractDXFBoundariesExecutor(IConfigurationStore configStore, ILoggerFactory logger,
  IServiceExceptionHandler exceptionHandler)
  : base(configStore, logger, exceptionHandler)
    {
    }

    /// <summary>
    /// Default constructor for RequestExecutorContainer.Build
    /// </summary>
    public ExtractDXFBoundariesExecutor()
    {
    }

    protected override async Task<ContractExecutionResult> ProcessAsyncEx<T>(T item)
    {
      var request = item as DXFBoundariesRequest;
      //      var siteModel = GetSiteModel(request.ProjectUid);

      //      if (siteModel == null)
      //        throw new ServiceException(HttpStatusCode.BadRequest, new ContractExecutionResult(ContractExecutionStatesEnum.InternalProcessingError, $"Unknown site model {request.ProjectUid}"));

      if (!File.Exists(request.FileName))
        throw new ServiceException(HttpStatusCode.BadRequest, new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError, $"File {request.FileName} does not exist"));

      var result = DXFFileUtilities.RequestBoundariesFromLineWork(request.FileName, request.FileUnits, request.MaxBoundaries, out var boundaries);

      if (result != DXFUtilitiesResult.OK)
        throw new ServiceException(HttpStatusCode.BadRequest, new ContractExecutionResult(ContractExecutionStatesEnum.InternalProcessingError, $"Error processing file: {result}"));

      if (boundaries == null)
        throw new ServiceException(HttpStatusCode.BadRequest, new ContractExecutionResult(ContractExecutionStatesEnum.InternalProcessingError, $"Internal request successful, but returned boundaries are null"));

      return await ConvertResult(boundaries, request.CSIB);
    }

    protected async Task<DXFBoundaryResult> ConvertResult(PolyLineBoundaries boundaries, string csib)
    {
      // Convert grid coordinates into WGS: assemble and convert
      var coordinates = boundaries.Boundaries.SelectMany(x => x.Boundary.Points).Select(pt => new XYZ(pt.X, pt.Y,0.0)).ToArray();

      // Perform conversion
      var conversionResult = await DIContext.Obtain<IConvertCoordinates>().NEEToLLH(csib, coordinates);

      if (conversionResult.ErrorCode != RequestErrorStatus.OK)
      {
        throw new ServiceException(HttpStatusCode.BadRequest, new ContractExecutionResult(ContractExecutionStatesEnum.InternalProcessingError, "Failed to convert grid coordinates of boundaries to WGS"));
      }

      // Recopy converted coordinates into boundaries
      var indexer = 0;
      for (var i = 0; i < boundaries.Boundaries.Count; i++)
      {
        var boundary = boundaries.Boundaries[i].Boundary;
        for (var j = 0; j < boundary.NumVertices; j++)
        {
          boundary.Points[j] = new FencePoint(conversionResult.LLHCoordinates[indexer].X, conversionResult.LLHCoordinates[indexer].Y, 0.0);
          indexer++;
        }
      }

      // Construct response
      return new DXFBoundaryResult(ContractExecutionStatesEnum.ExecutedSuccessfully, "Success",
        boundaries.Boundaries.Select(x =>
        new DXFBoundaryResultItem(x.Boundary.Points.Select(pt =>
          new WGSPoint(pt.X, pt.Y)).ToList(), x.Type, x.Name)).ToList());
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
