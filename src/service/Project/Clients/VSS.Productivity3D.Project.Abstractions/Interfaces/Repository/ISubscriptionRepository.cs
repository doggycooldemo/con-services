﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VSS.Productivity3D.Project.Abstractions.Models.DatabaseModels;
using VSS.VisionLink.Interfaces.Events.MasterData.Interfaces;

namespace VSS.Productivity3D.Project.Abstractions.Interfaces.Repository
{
  public interface ISubscriptionRepository
  {
    Task<Subscription> GetSubscription(string subscriptionUid);
    Task<IEnumerable<Subscription>> GetSubscriptionsByCustomer(string customerUid, DateTime validAtDate);
    Task<IEnumerable<Subscription>> GetProjectBasedSubscriptionsByCustomer(string customerUid, DateTime validAtDate);
    Task<IEnumerable<Subscription>> GetFreeProjectSubscriptionsByCustomer(string customerUid, DateTime validAtDate);
    Task<IEnumerable<Subscription>> GetSubscriptionsByAsset(string assetUid, DateTime validAtDate);
    Task<IEnumerable<Subscription>> GetSubscriptions_UnitTest(string subscriptionUid);
    Task<IEnumerable<ProjectSubscription>> GetProjectSubscriptions_UnitTest(string subscriptionUid);

    Task<int> StoreEvent(ISubscriptionEvent evt);
  }
}