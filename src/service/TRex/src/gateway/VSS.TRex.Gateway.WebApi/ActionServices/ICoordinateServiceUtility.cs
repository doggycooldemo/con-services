﻿using System.Collections.Generic;
using System.Threading.Tasks;
using VSS.Productivity3D.Models.Models;

namespace VSS.TRex.Gateway.WebApi.ActionServices
{
  public interface ICoordinateServiceUtility
  {
    /// <summary>
    /// Converts XYZ to LLH
    /// </summary>
    Task<int> PatchLLH(string CSIB, List<MachineStatus> machines);
  }
}
