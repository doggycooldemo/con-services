﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using VSS.MasterData.Models.Models;
using VSS.Productivity3D.Models.Enums;
using VSS.Productivity3D.Models.ResultHandling;
using VSS.Productivity3D.Productivity3D.Models.Compaction.ResultHandling;
using VSS.Productivity3D.Productivity3D.Models.Coord.ResultHandling;
using VSS.Productivity3D.Productivity3D.Models.Notification.ResultHandling;
using VSS.Productivity3D.Project.Abstractions.Models;
using VSS.VisionLink.Interfaces.Events.MasterData.Models;

namespace VSS.Productivity3D.Productivity3D.Abstractions.Interfaces
{
  public interface IProductivity3dProxy
  {

    Task<Stream> GetLineworkFromAlignment(Guid projectUid, Guid alignmentUid,
      IDictionary<string, string> customHeaders);

    Task<CoordinateSystemSettingsResult> CoordinateSystemValidate(byte[] coordinateSystemFileContent,
      string coordinateSystemFilename,
      IDictionary<string, string> customHeaders = null);

    Task<CoordinateSystemSettingsResult> CoordinateSystemPost(long legacyProjectId, byte[] coordinateSystemFileContent,
      string coordinateSystemFilename,
      IDictionary<string, string> customHeaders = null);

    Task<AddFileResult> AddFile(Guid projectUid, ImportedFileType fileType, Guid fileUid, string fileDescriptor,
      long fileId, DxfUnitsType dxfUnitsType, IDictionary<string, string> customHeaders = null);

    Task<BaseDataResult> DeleteFile(Guid projectUid, ImportedFileType fileType, Guid fileUid, string fileDescriptor,
      long fileId, long? legacyFileId, IDictionary<string, string> customHeaders = null);

    Task<ProjectStatisticsResult> GetProjectStatistics(Guid projectUid,
      IDictionary<string, string> customHeaders = null);

    Task<BaseDataResult> UpdateFiles(Guid projectUid, IEnumerable<Guid> fileUids,
      IDictionary<string, string> customHeaders = null);

    Task<BaseDataResult> NotifyImportedFileChange(Guid projectUid, Guid fileUid,
      IDictionary<string, string> customHeaders = null);

    Task<BaseDataResult> ValidateProjectSettings(Guid projectUid, string projectSettings,
      IDictionary<string, string> customHeaders = null);

    Task<BaseDataResult> ValidateProjectSettings(Guid projectUid, string projectSettings,
      ProjectSettingsType settingsType, IDictionary<string, string> customHeaders = null);

    Task<BaseDataResult> ValidateProjectSettings(ProjectSettingsRequest request, 
	  IDictionary<string, string> customHeaders = null);
    Task<BaseDataResult> NotifyFilterChange(Guid filterUid, Guid projectUid,
      IDictionary<string, string> customHeaders = null);

    Task<BaseDataResult> UploadTagFile(string filename, byte[] data, string orgId = null,
      IDictionary<string, string> customHeaders = null);

    Task<T> ExecuteGenericV1Request<T>(string route, object payload,
      IDictionary<string, string> customHeaders = null);

    Task<T> ExecuteGenericV1Request<T>(string route, string query, IDictionary<string, string> customHeaders = null);

    Task<T> ExecuteGenericV2Request<T>(string route, HttpMethod method, Stream body = null, IDictionary<string, string> customHeaders = null);

    Task<BaseDataResult> InvalidateCache(string projectUid,
      IDictionary<string, string> customHeaders = null);

    Task<AlignmentPointsResult> GetAlignmentPoints(Guid projectUid, Guid alignmentUid, IDictionary<string, string> customHeaders = null);
    Task<PointsListResult> GetAlignmentPointsList(Guid projectUid, IDictionary<string, string> customHeaders = null);
    Task<PointsListResult> GetDesignBoundaryPoints(Guid projectUid, Guid designUid,
      IDictionary<string, string> customHeaders = null);

    Task<PointsListResult> GetFilterPoints(Guid projectUid, Guid filterUid,
      IDictionary<string, string> customHeaders = null);

    Task<PointsListResult> GetFilterPointsList(Guid projectUid, Guid? filterUid, Guid? baseUid, Guid? topUid, FilterBoundaryType boundaryType,
      IDictionary<string, string> customHeaders = null);

    Task<byte[]> GetProductionDataTile(Guid projectUid, Guid? filterUid, Guid? cutFillDesignUid, ushort width,
      ushort height, string bbox, DisplayMode mode, Guid? baseUid, Guid? topUid, VolumeCalcType? volCalcType,
      IDictionary<string, string> customHeaders = null, bool explicitFilters = false);

    Task<string> GetBoundingBox(Guid projectUid, TileOverlayType[] overlays, Guid? filterUid, Guid? cutFillDesignUid,
      Guid? baseUid, Guid? topUid, VolumeCalcType? volCalcType, IDictionary<string, string> customHeaders = null);

  }
}