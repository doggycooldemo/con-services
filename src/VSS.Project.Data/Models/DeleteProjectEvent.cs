﻿using System;
using VSS.Project.Data.Interfaces;

namespace VSS.Project.Data.Models
{
  public class DeleteProjectEvent : IProjectEvent
  {
    public Guid ProjectUID { get; set; }
    public bool DeletePermanently { get; set; }
    public DateTime ActionUTC { get; set; }
    public DateTime ReceivedUTC { get; set; }
  }
}
