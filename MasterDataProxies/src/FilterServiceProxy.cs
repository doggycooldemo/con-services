﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using VSS.ConfigurationStore;
using VSS.MasterData.Models.Models;
using VSS.MasterData.Proxies.Interfaces;

namespace VSS.MasterData.Proxies
{
  public class FilterServiceProxy : BaseProxy, IFilterServiceProxy
  {
    public FilterServiceProxy(IConfigurationStore configurationStore, ILoggerFactory logger, IMemoryCache cache) : base(
      configurationStore, logger, cache)
    {
    }

    /// <summary>
    /// Gets the filter from the filter service.
    /// </summary>
    /// <param name="customerUid">The customer uid.</param>
    /// <param name="projectUid">The project uid.</param>
    /// <param name="filterUid">The filter uid.</param>
    /// <param name="customHeaders">The custom headers including JWT and customer context.</param>
    /// <returns></returns>
    public async Task<FilterDescriptor> GetFilter(string projectUid, string filterUid,
      IDictionary<string, string> customHeaders = null)
    {
      var result = await GetContainedMasterDataList<FilterData>(filterUid, "FILTER_CACHE_LIFE", "FILTER_API_URL",
        customHeaders, $"/{projectUid}/{filterUid}");
      if (result.Code == 0)
      {
        return result.filterDescriptor;
      }
      else
      {
        log.LogWarning("Failed to get Filter Descriptor: {0}, {1}", result.Code, result.Message);
        return null;
      }

    }

    /// <summary>
    /// Clears an item from the cache
    /// </summary>
    /// <param name="projectUid">The projectUid of the item to remove from the cache</param>
    public void ClearCacheItem(string projectUid)
    {
      ClearCacheItem<FilterData>(projectUid);
    }
  }
}
