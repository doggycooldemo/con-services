﻿using System;

namespace ProductionDataSvc.AcceptanceTests.Models
{
  public class DesignNames : IEquatable<DesignNames>
  {
    public string designName { get; set; }
    public long designId { get; set; }

    #region Equality test
    public bool Equals(DesignNames other)
    {
      if (other == null)
        return false;

      return this.designId == other.designId &&
             this.designName == other.designName;
    }

    public static bool operator ==(DesignNames a, DesignNames b)
    {
      if ((object)a == null || (object)b == null)
        return Object.Equals(a, b);

      return a.Equals(b);
    }

    public static bool operator !=(DesignNames a, DesignNames b)
    {
      return !(a == b);
    }

    public override bool Equals(object obj)
    {
      return obj is DesignNames && this == (DesignNames)obj;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
    #endregion
  }
}
