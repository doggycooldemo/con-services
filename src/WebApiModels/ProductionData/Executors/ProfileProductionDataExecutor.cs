﻿using SVOICOptionsDecls;
using System;
using System.IO;
using System.Net;
using VSS.Common.Exceptions;
using VSS.Common.ResultsHandling;
using VSS.Productivity3D.Common.Filters.Interfaces;
using VSS.Productivity3D.Common.Proxies;
using VSS.Productivity3D.Common.Utilities;
using VSS.Productivity3D.WebApi.Models.ProductionData.Helpers;
using VSS.Productivity3D.WebApi.Models.ProductionData.Models;
using VSS.Productivity3D.WebApi.Models.ProductionData.ResultHandling;

namespace VSS.Productivity3D.WebApi.Models.ProductionData.Executors
{
  /// <summary>
  /// Get production data profile calculations executor.
  /// </summary>
  public class ProfileProductionDataExecutor : RequestExecutorContainer
  {
    private const int PROFILE_TYPE_NOT_REQUIRED = -1;

    private ProfileResult PerformProductionDataProfilePost(ProfileProductionDataRequest request)
    {
      MemoryStream memoryStream;

      if (!RaptorConverters.DesignDescriptor(request.alignmentDesign).IsNull())
      {
        var args = ASNode.RequestAlignmentProfile.RPC.__Global.Construct_RequestAlignmentProfile_Args(
          request.projectId ?? -1,
          PROFILE_TYPE_NOT_REQUIRED,
          request.startStation ?? ValidationConstants.MIN_STATION,
          request.endStation ?? ValidationConstants.MIN_STATION,
          RaptorConverters.DesignDescriptor(request.alignmentDesign),
          RaptorConverters.ConvertFilter(request.filterID, request.filter, request.projectId),
          RaptorConverters.ConvertLift(request.liftBuildSettings, TFilterLayerMethod.flmAutomatic),
          RaptorConverters.DesignDescriptor(request.alignmentDesign),
          request.returnAllPassesAndLayers);

        memoryStream = raptorClient.GetAlignmentProfile(args);
      }
      else
      {
        ProfilesHelper.convertProfileEndPositions(
          request.gridPoints,
          request.wgs84Points, 
          out VLPDDecls.TWGS84Point startPt,
          out VLPDDecls.TWGS84Point endPt,
          out bool positionsAreGrid);

        var args = ASNode.RequestProfile.RPC.__Global.Construct_RequestProfile_Args(
          request.projectId ?? -1,
          PROFILE_TYPE_NOT_REQUIRED,
          positionsAreGrid,
          startPt,
          endPt,
          RaptorConverters.ConvertFilter(request.filterID, request.filter, request.projectId),
          RaptorConverters.ConvertLift(request.liftBuildSettings, TFilterLayerMethod.flmAutomatic),
          RaptorConverters.DesignDescriptor(request.alignmentDesign),
          request.returnAllPassesAndLayers);

        memoryStream = raptorClient.GetProfile(args);
      }

      return memoryStream != null
        ? ProfilesHelper.convertProductionDataProfileResult(memoryStream, request.callId ?? Guid.NewGuid())
        : null; // TODO: return appropriate result
    }

    protected override ContractExecutionResult ProcessEx<T>(T item)
    {
      ContractExecutionResult result;
      try
      {
        var profileResult = PerformProductionDataProfilePost(item as ProfileProductionDataRequest);

        if (profileResult != null)
        {
          result = profileResult;
        }
        else
        {
          throw new ServiceException(HttpStatusCode.BadRequest, new ContractExecutionResult(ContractExecutionStatesEnum.FailedToGetResults, "Failed to get requested profile calculations."));
        }
      }
      finally
      {
        ContractExecutionStates.ClearDynamic();
      }

      return result;
    }
  }
}