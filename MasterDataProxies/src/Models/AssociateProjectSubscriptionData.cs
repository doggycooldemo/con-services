﻿using System;

namespace VSS.Productivity3D.MasterDataProxies.Models
{
    public class AssociateProjectSubscriptionData
    {
        public Guid SubscriptionUID { get; set; }
        public Guid ProjectUID { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ActionUTC => DateTime.UtcNow;
    }
}
