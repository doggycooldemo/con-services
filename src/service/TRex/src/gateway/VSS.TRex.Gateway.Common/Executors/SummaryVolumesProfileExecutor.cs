﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Logging;
using VSS.Common.Exceptions;
using VSS.ConfigurationStore;
using VSS.MasterData.Models.Handlers;
using VSS.MasterData.Models.ResultHandling.Abstractions;
using VSS.Productivity3D.Models.Enums;
using VSS.Productivity3D.Models.Models;
using VSS.Productivity3D.Models.Models.Profiling;
using VSS.Productivity3D.Models.ResultHandling.Profiling;
using VSS.TRex.Common;
using VSS.TRex.Common.Models;
using VSS.TRex.Filters;
using VSS.TRex.Profiling;
using VSS.TRex.Profiling.GridFabric.Arguments;
using VSS.TRex.Profiling.GridFabric.Requests;
using VSS.TRex.Profiling.GridFabric.Responses;
using VSS.TRex.Profiling.Models;
using VSS.TRex.Types;

namespace VSS.TRex.Gateway.Common.Executors
{
  /// <summary>
  /// Processes the request to get Summary Volumes profile.
  /// </summary>
  public class SummaryVolumesProfileExecutor : BaseExecutor
  {
    public SummaryVolumesProfileExecutor(IConfigurationStore configStore, ILoggerFactory logger,
      IServiceExceptionHandler exceptionHandler)
      : base(configStore, logger, exceptionHandler)
    {
    }

    /// <summary>
    /// Default constructor for RequestExecutorContainer.Build
    /// </summary>
    public SummaryVolumesProfileExecutor()
    {
    }

    protected override ContractExecutionResult ProcessEx<T>(T item)
    {
      SummaryVolumesProfileDataRequest request = item as SummaryVolumesProfileDataRequest;
      if (request == null)
        ThrowRequestTypeCastException<SummaryVolumesProfileDataRequest>();

      var siteModel = GetSiteModel(request.ProjectUid);
      var baseFilter = ConvertFilter(request.BaseFilter, siteModel);
      var topFilter = ConvertFilter(request.TopFilter, siteModel);

      ProfileRequestArgument_ApplicationService arg = new ProfileRequestArgument_ApplicationService
      {
        ProjectID = request.ProjectUid ?? Guid.Empty,
        ProfileTypeRequired = GridDataType.Height,
        ProfileStyle = ProfileStyle.SummaryVolume,
        PositionsAreGrid = request.PositionsAreGrid,
        Filters = new FilterSet(baseFilter, topFilter),
        ReferenceDesign.DesignID = request.ReferenceDesignUid ?? Guid.Empty,
        ReferenceDesign.Offset = request.ReferenceDesignOffset ?? 0,
        StartPoint = new WGS84Point(lon: request.StartX, lat: request.StartY),
        EndPoint = new WGS84Point(lon: request.EndX, lat: request.EndY),
        ReturnAllPassesAndLayers = false,
        VolumeType = ConvertVolumesType(request.VolumeCalcType)
      };

      // Compute a profile from the bottom left of the screen extents to the top right 
      var svRequest = new ProfileRequest_ApplicationService_SummaryVolumeProfileCell();

     // var Response = svRequest.Execute(arg);
      ProfileRequestResponse<SummaryVolumeProfileCell> response = svRequest.Execute(arg);

      if (response != null)
        return ConvertResult(response);

      throw new ServiceException(HttpStatusCode.BadRequest, new ContractExecutionResult(ContractExecutionStatesEnum.FailedToGetResults,
        "Failed to get requested Summary Volumes Profile data"));
    }

    /// <summary>
    /// Converts values of the VolumesType to ones of the VolumeComputationType.
    /// </summary>
    /// <param name="volumesType"></param>
    /// <returns></returns>
    private VolumeComputationType ConvertVolumesType(VolumesType volumesType)
    {
      switch (volumesType)
      {
        case VolumesType.None: return VolumeComputationType.None;
        case VolumesType.AboveLevel: return VolumeComputationType.AboveLevel;
        case VolumesType.Between2Levels: return VolumeComputationType.Between2Levels;
        case VolumesType.AboveFilter: return VolumeComputationType.AboveFilter;
        case VolumesType.Between2Filters: return VolumeComputationType.Between2Filters;
        case VolumesType.BetweenFilterAndDesign: return VolumeComputationType.BetweenFilterAndDesign;
        case VolumesType.BetweenDesignAndFilter: return VolumeComputationType.BetweenDesignAndFilter;
        default: throw new ArgumentException($"Unknown VolumesType {Convert.ToInt16(volumesType)}");
      }
    }

    /// <summary>
    /// Converts SummaryVolumesProfileResponse data into SummaryVolumesProfileResult data.
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    private ProfileDataResult<SummaryVolumesProfileCell> ConvertResult(ProfileRequestResponse<SummaryVolumeProfileCell> result)
    {
      List<SummaryVolumesProfileCell> profileCells = result.ProfileCells.Select(pc => 
        new SummaryVolumesProfileCell(
          pc.Station, 
          pc.InterceptLength, 
          pc.OTGCellX, 
          pc.OTGCellY, 
          pc.DesignElev, 
          pc.LastCellPassElevation1, 
          pc.LastCellPassElevation2))
        .ToList();

      return new ProfileDataResult<SummaryVolumesProfileCell>(result.GridDistanceBetweenProfilePoints, profileCells);
    }
  }
}
