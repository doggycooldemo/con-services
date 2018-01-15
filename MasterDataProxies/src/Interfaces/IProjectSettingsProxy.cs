﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace VSS.MasterData.Proxies.Interfaces
{
  public interface IProjectSettingsProxy : ICacheProxy
  {
    Task<string> GetProjectSettings(string projectUid, string userId, IDictionary<string, string> customHeaders);
  }
}
