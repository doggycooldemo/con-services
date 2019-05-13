﻿using System;
using VSS.VisionLink.Interfaces.Events.MasterData.Interfaces;

namespace VSS.VisionLink.Interfaces.Events.MasterData.Models
{
  public class CreateImportedFileEvent : IProjectEvent
  {
    public Guid ImportedFileUID { get; set; }
    public long ImportedFileID { get; set; }
    public Guid CustomerUID { get; set; }
    public ImportedFileType ImportedFileType { get; set; }
    public string Name { get; set; }
    public string FileDescriptor { get; set; }
    public DateTime FileCreatedUtc { get; set; }
    public DateTime FileUpdatedUtc { get; set; }
    public string ImportedBy { get; set; }
    public DateTime? SurveyedUTC { get; set; }
    public DxfUnitsType DxfUnitsType { get; set; }
    public Guid ProjectUID { get; set; }
    public DateTime ActionUTC { get; set; }
    public DateTime ReceivedUTC { get; set; }
    public int MinZoomLevel { get; set; }
    public int MaxZoomLevel { get; set; }
    public double Offset { get; set; }
    public Guid? ParentUID { get; set; }

  }
}