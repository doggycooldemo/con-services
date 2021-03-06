﻿using System;
using Newtonsoft.Json;
using VSS.MasterData.Models.Models;
using VSS.Visionlink.Interfaces.Events.MasterData.Models;

namespace VSS.MasterData.Project.WebAPI.Common.Models
{
  public abstract class ImportedFileBase
  { 
    public Guid ProjectUid { get; set; }
    public ImportedFileType ImportedFileType { get; set; }
    public FileDescriptor FileDescriptor { get; set; }

    public DateTime? SurveyedUtc { get; set; }
    public string DataOceanRootFolder { get; set; }

    public string DataOceanFileName { get; set; }

    /// <summary>
    /// Cannot delete a design (or alignment) which is used in a filter
    /// </summary>
    /// <remarks>
    /// When scheduled reports are implemented, extend this check to them as well.
    /// </remarks>
    [JsonIgnore]
    public bool IsDesignFileType =>
      IsTRexDesignFileType ||
      ImportedFileType == ImportedFileType.ReferenceSurface;

    /// <summary>
    /// Types of design files in TRex
    /// </summary>
    public bool IsTRexDesignFileType =>
      ImportedFileType == ImportedFileType.DesignSurface ||
      ImportedFileType == ImportedFileType.SurveyedSurface ||
      ImportedFileType == ImportedFileType.Alignment;
  }
}
