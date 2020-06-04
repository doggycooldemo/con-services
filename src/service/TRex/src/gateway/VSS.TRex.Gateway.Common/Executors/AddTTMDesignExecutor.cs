﻿using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VSS.AWS.TransferProxy;
using VSS.Common.Abstractions.Configuration;
using VSS.MasterData.Models.Handlers;
using VSS.MasterData.Models.ResultHandling.Abstractions;
using VSS.Productivity3D.Models.Models.Designs;
using VSS.TRex.Common;
using VSS.TRex.Common.Utilities;
using VSS.TRex.Designs;
using VSS.TRex.Designs.Interfaces;
using VSS.TRex.Designs.Models;
using VSS.TRex.DI;
using VSS.TRex.ExistenceMaps.Interfaces;
using VSS.TRex.Geometry;
using VSS.TRex.SubGridTrees.Interfaces;
using VSS.TRex.SurveyedSurfaces.Interfaces;
using VSS.TRex.Types;
using VSS.Visionlink.Interfaces.Events.MasterData.Models;
using Consts = VSS.TRex.ExistenceMaps.Interfaces.Consts;

namespace VSS.TRex.Gateway.Common.Executors
{
  public class AddTTMDesignExecutor : BaseExecutor
  {
    /// <summary>
    /// TagFileExecutor
    /// </summary>
    /// <param name="configStore"></param>
    /// <param name="logger"></param>
    /// <param name="exceptionHandler"></param>
    public AddTTMDesignExecutor(IConfigurationStore configStore,
        ILoggerFactory logger, IServiceExceptionHandler exceptionHandler) : base(configStore, logger, exceptionHandler)
    {
    }

    /// <summary>
    /// Default constructor for RequestExecutorContainer.Build
    /// </summary>
    public AddTTMDesignExecutor()
    {
    }

    /// <summary>
    /// Process add design request
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <returns></returns>
    protected override async Task<ContractExecutionResult> ProcessAsyncEx<T>(T item)
    {
      var request = CastRequestObjectTo<DesignRequest>(item);

      try
      {
        log.LogInformation($"#In# AddTTMDesignExecutor. Add design :{request.FileName}, Project:{request.ProjectUid}, DesignUid:{request.DesignUid}");

        // load core file from s3 to local
        var localPath = FilePathHelper.GetTempFolderForProject(request.ProjectUid);
        var localPathAndFileName = Path.Combine(new[] {localPath, request.FileName});
        
        var ttm = new TTMDesign(SubGridTreeConsts.DefaultCellSize);
        var designLoadResult = await ttm.LoadFromStorage(request.ProjectUid, request.FileName, localPath);
        if (designLoadResult != DesignLoadResult.Success)
        {
          log.LogError($"#Out# AddTTMDesignExecutor. Addition of design failed :{request.FileName}, Project:{request.ProjectUid}, DesignUid:{request.DesignUid}, designLoadResult: {designLoadResult.ToString()}");
          throw CreateServiceException<AddTTMDesignExecutor>
            (HttpStatusCode.InternalServerError, ContractExecutionStatesEnum.InternalProcessingError,
              RequestErrorStatus.DesignImportUnableToRetrieveFromS3, designLoadResult.ToString());
        }

        // This generates the 2 index files 
        designLoadResult = ttm.LoadFromFile(localPathAndFileName);
        if (designLoadResult != DesignLoadResult.Success)
        {
          log.LogError($"#Out# AddTTMDesignExecutor. Addition of design failed :{request.FileName}, Project:{request.ProjectUid}, DesignUid:{request.DesignUid}, designLoadResult: {designLoadResult.ToString()}");
          throw CreateServiceException<AddTTMDesignExecutor>
            (HttpStatusCode.InternalServerError, ContractExecutionStatesEnum.InternalProcessingError,
              RequestErrorStatus.DesignImportUnableToCreateDesign, designLoadResult.ToString());
        }

        var extents = new BoundingWorldExtent3D();
        ttm.GetExtents(out extents.MinX, out extents.MinY, out extents.MaxX, out extents.MaxY);
        ttm.GetHeightRange(out extents.MinZ, out extents.MaxZ);

        var existanceMaps = DIContext.Obtain<IExistenceMaps>();
        if (request.FileType == ImportedFileType.DesignSurface)
        {
          // Create the new designSurface in our site model
          var designSurface = DIContext.Obtain<IDesignManager>().Add(request.ProjectUid,
            new Designs.Models.DesignDescriptor(request.DesignUid, localPathAndFileName, request.FileName),
            extents);
          existanceMaps.SetExistenceMap(request.DesignUid, Consts.EXISTENCE_MAP_DESIGN_DESCRIPTOR, designSurface.ID, ttm.SubGridOverlayIndex());
        }

        if (request.FileType == ImportedFileType.SurveyedSurface)
        {
          // Create the new SurveyedSurface in our site model
          var surveyedSurface = DIContext.Obtain<ISurveyedSurfaceManager>().Add(request.ProjectUid,
            new Designs.Models.DesignDescriptor(request.DesignUid, localPathAndFileName, request.FileName),
            request.SurveyedUtc ?? TRex.Common.Consts.MIN_DATETIME_AS_UTC, // validation will have ensured this exists
            extents);
          existanceMaps.SetExistenceMap(request.DesignUid, Consts.EXISTENCE_SURVEYED_SURFACE_DESCRIPTOR, surveyedSurface.ID, ttm.SubGridOverlayIndex());
        }

        //  TTM.LoadFromFile() will have created these 3 files. We need to store them on S3 to reload cache when required
        var s3FileTransfer = new S3FileTransfer(TransferProxyType.DesignImport);

        s3FileTransfer.WriteFile(localPath, request.ProjectUid, request.FileName + Designs.TTM.Optimised.Consts.DESIGN_SUB_GRID_INDEX_FILE_EXTENSION);
        s3FileTransfer.WriteFile(localPath, request.ProjectUid, request.FileName + Designs.TTM.Optimised.Consts.DESIGN_SPATIAL_INDEX_FILE_EXTENSION);
        s3FileTransfer.WriteFile(localPath, request.ProjectUid, request.FileName + Designs.TTM.Optimised.Consts.DESIGN_BOUNDARY_FILE_EXTENSION);

        log.LogInformation($"#Out# AddTTMDesignExecutor. Processed add design :{request.FileName}, Project:{request.ProjectUid}, DesignUid:{request.DesignUid}");
      }
      catch (Exception e)
      {
        log.LogError(e, $"#Out# AddTTMDesignExecutor. Addition of design failed :{request.FileName}, Project:{request.ProjectUid}, DesignUid:{request.DesignUid}, Exception:");
        throw CreateServiceException<AddTTMDesignExecutor>
          (HttpStatusCode.InternalServerError, ContractExecutionStatesEnum.InternalProcessingError,
            RequestErrorStatus.DesignImportUnableToCreateDesign, e.Message);
      }

      return new ContractExecutionResult(); 
    }

    /// <summary>
    /// Processes the request synchronously.
    /// </summary>
    protected override ContractExecutionResult ProcessEx<T>(T item)
    {
      throw new NotImplementedException("Use the asynchronous form of this method");
    }
  }
}
