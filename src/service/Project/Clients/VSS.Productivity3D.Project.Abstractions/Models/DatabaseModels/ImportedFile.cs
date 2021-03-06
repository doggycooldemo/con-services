﻿using System;
using VSS.Productivity3D.Project.Abstractions.Extensions;
using VSS.Visionlink.Interfaces.Events.MasterData.Models;

namespace VSS.Productivity3D.Project.Abstractions.Models.DatabaseModels
{
  public class ImportedFile
  {
    public string ProjectUid { get; set; }
    public string ImportedFileUid { get; set; }
    public long ImportedFileId { get; set; }
    public long? LegacyImportedFileId { get; set; }
    public string CustomerUid { get; set; }
    public ImportedFileType ImportedFileType { get; set; }
    public string Name { get; set; }
    public string FileDescriptor { get; set; }

    // These 3 refer to the most recent udpates of the importedFile.
    // History is contained in ImportedFileHistory, and includes this most recent update.
    public DateTime FileCreatedUtc { get; set; }
    public DateTime FileUpdatedUtc { get; set; }
    public string ImportedBy { get; set; }

    public DateTime? SurveyedUtc { get; set; }
    public DxfUnitsType DxfUnitsType { get; set; }
    public int MinZoomLevel { get; set; }
    public int MaxZoomLevel { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime LastActionedUtc { get; set; }
    public ImportedFileHistory ImportedFileHistory { get; set; }
    public string ParentUid { get; set; }
    public double Offset { get; set; }


    public override bool Equals(object obj)
    {
      var otherImportedFile = obj as ImportedFile;
      if (otherImportedFile == null) return false;
      return otherImportedFile.ProjectUid == ProjectUid
             && otherImportedFile.ImportedFileUid == ImportedFileUid
             && otherImportedFile.ImportedFileId == ImportedFileId
             && otherImportedFile.LegacyImportedFileId == LegacyImportedFileId
             && otherImportedFile.CustomerUid == CustomerUid
             && otherImportedFile.ImportedFileType == ImportedFileType
             && otherImportedFile.Name == Name
             && otherImportedFile.FileDescriptor == FileDescriptor
             && otherImportedFile.FileCreatedUtc == FileCreatedUtc
             && otherImportedFile.FileUpdatedUtc == FileUpdatedUtc
             && otherImportedFile.ImportedBy == ImportedBy
             && otherImportedFile.IsDeleted == IsDeleted
             && otherImportedFile.SurveyedUtc == SurveyedUtc
             && otherImportedFile.DxfUnitsType == DxfUnitsType
             && otherImportedFile.LastActionedUtc == LastActionedUtc
             && otherImportedFile.MinZoomLevel == MinZoomLevel
             && otherImportedFile.MaxZoomLevel == MaxZoomLevel
             && otherImportedFile.ImportedFileHistory == ImportedFileHistory
             && otherImportedFile.ParentUid == ParentUid 
             && otherImportedFile.Offset.EqualsToNearestMillimeter(Offset);
    }

    public override int GetHashCode()
    {
      return 0;
    }
  }
}
