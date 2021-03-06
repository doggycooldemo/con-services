﻿using VSS.TRex.GridFabric;

namespace VSS.TRex.Services
{
  /// <summary>
  /// The base class for services. This provides common aspects such as the injected Ignite instance
  /// </summary>
  public class BaseService : BaseIgniteClass
  {
    public BaseService()
    {
    }

    public BaseService(string gridName, string role) : base(gridName, role)
    {

    }
  }
}
