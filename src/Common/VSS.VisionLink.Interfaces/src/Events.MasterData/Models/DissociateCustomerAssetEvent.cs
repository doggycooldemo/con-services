﻿using System;
using VSS.VisionLink.Interfaces.Events.MasterData.Interfaces;

namespace VSS.VisionLink.Interfaces.Events.MasterData.Models
{
  public class DissociateCustomerAssetEvent : ICustomerAssetEvent
  {
    public Guid CustomerUID { get; set; }
    public Guid AssetUID { get; set; }
    public DateTime ActionUTC { get; set; }
    public DateTime ReceivedUTC { get; set; }
  }
}