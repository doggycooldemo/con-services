﻿using System;
using Newtonsoft.Json;
using ProductionDataSvc.AcceptanceTests.Models;
using RaptorSvcAcceptTestsCommon.Models;

namespace ProductionDataSvc.AcceptanceTests
{
  public class MachineDesignDetails : MachineDetails, IEquatable<MachineDesignDetails>
  {
    #region Members
    public DesignName[] designs { get; set; }
    #endregion

    #region Equality test
    public bool Equals(MachineDesignDetails other)
    {
      if (other == null)
        return false;

      if (this.designs.Length != other.designs.Length)
        return false;

      for (int i = 0; i < this.designs.Length; i++)
      {
        if (!this.designs[i].Equals(other.designs[i]))
          return false;
      }
      return this.AssetId == other.AssetId &&
             this.MachineName == other.MachineName &&
             this.IsJohnDoe == other.IsJohnDoe;
    }

    public static bool operator ==(MachineDesignDetails a, MachineDesignDetails b)
    {
      if ((object)a == null || (object)b == null)
        return Object.Equals(a, b);

      return a.Equals(b);
    }

    public static bool operator !=(MachineDesignDetails a, MachineDesignDetails b)
    {
      return !(a == b);
    }

    public override bool Equals(object obj)
    {
      return obj is MachineDesignDetails && this == (MachineDesignDetails)obj;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
    #endregion

    #region ToString override
    /// <summary>
    /// ToString override
    /// </summary>
    /// <returns>A string representation.</returns>
    public override string ToString()
    {
      return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
    #endregion

  }
}
