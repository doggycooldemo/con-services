﻿using System.Collections.Generic;
using System.Threading.Tasks;
using MasterDataProxies.ResultHandling;

namespace MasterDataProxies.Interfaces
{
  public interface ICustomerProxy
  {
    Task<CustomerDataResult> GetCustomersForMe(string userUid, IDictionary < string, string> customHeaders);
  }
}
