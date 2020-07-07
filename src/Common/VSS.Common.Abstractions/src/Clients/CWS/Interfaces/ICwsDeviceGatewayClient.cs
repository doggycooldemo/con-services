﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VSS.Common.Abstractions.Clients.CWS.Models;

namespace VSS.Common.Abstractions.Clients.CWS.Interfaces
{
  public interface ICwsDeviceGatewayClient
  {
    Task<DeviceLKSResponseModel> GetDeviceLKS(string deviceName, IHeaderDictionary customHeaders = null);
    Task<DeviceLKSListResponseModel> GetDevicesLKSForProject(Guid projectUid, DateTime? earliestOfInterestUtc = null, IHeaderDictionary customHeaders = null);
  }
}
