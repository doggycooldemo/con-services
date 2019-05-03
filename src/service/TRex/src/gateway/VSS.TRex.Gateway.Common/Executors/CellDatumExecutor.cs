﻿using System;
using Microsoft.Extensions.Logging;
using VSS.ConfigurationStore;
using VSS.MasterData.Models.Handlers;
using VSS.MasterData.Models.ResultHandling.Abstractions;
using VSS.Productivity3D.Models.Models;
using VSS.Productivity3D.Models.ResultHandling;
using VSS.TRex.CellDatum.GridFabric.Arguments;
using VSS.TRex.CellDatum.GridFabric.Requests;
using VSS.TRex.Filters;
using VSS.TRex.Gateway.Common.Converters;
using VSS.TRex.Geometry;
using VSS.TRex.Types;

namespace VSS.TRex.Gateway.Common.Executors
{
  /// <summary>
  /// Processes the request to get a cell datum.
  /// </summary>
  public class CellDatumExecutor : BaseExecutor
  {
    public CellDatumExecutor(IConfigurationStore configStore, ILoggerFactory logger,
      IServiceExceptionHandler exceptionHandler)
      : base(configStore, logger, exceptionHandler)
    {
    }

    /// <summary>
    /// Default constructor for RequestExecutorContainer.Build
    /// </summary>
    public CellDatumExecutor()
    {
    }

    protected override ContractExecutionResult ProcessEx<T>(T item)
    {
      CellDatumTRexRequest request = item as CellDatumTRexRequest;

      if (request == null)
        ThrowRequestTypeCastException<CellDatumTRexRequest>();

      var siteModel = GetSiteModel(request.ProjectUid);

      var filter = ConvertFilter(request.Filter, siteModel);
      var cellDatumRequest = new CellDatumRequest_ApplicationService();
      var response = cellDatumRequest.Execute(new CellDatumRequestArgument_ApplicationService
      {
        ProjectID = siteModel.ID,
        Filters =  new FilterSet(filter),
        Mode = request.DisplayMode,
        CoordsAreGrid = request.CoordsAreGrid,
        Point = request.CoordsAreGrid  ? AutoMapperUtility.Automapper.Map<XYZ>(request.GridPoint) : AutoMapperUtility.Automapper.Map<XYZ>(request.LLPoint),
        ReferenceDesign.DesignID = request.DesignUid ?? Guid.Empty,
        ReferenceDesign.Offset = request.Offset ?? 0,        
      });

      return new CompactionCellDatumResult(response.DisplayMode, response.ReturnCode, response.Value, response.TimeStampUTC, response.Northing, response.Easting);
    }

  }
}
