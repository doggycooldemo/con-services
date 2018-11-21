﻿using System;
using System.Diagnostics;
using Apache.Ignite.Core.Binary;
using VSS.TRex.Common.Exceptions;
using VSS.TRex.Filters.Interfaces;

namespace VSS.TRex.GridFabric.Arguments
{
  /// <summary>
  ///  Forms the base request argument state that specific application service request contexts may leverage. It's roles include
  ///  containing the identifier of a TRex Application Service Node that originated the request
  /// </summary>
  public class BaseApplicationServiceRequestArgument : BaseRequestArgument, IEquatable<BaseRequestArgument>
  {
    private const byte VERSION_NUMBER = 1;

    // TODO If desired: ExternalDescriptor :TASNodeRequestDescriptor

    /// <summary>
    /// The identifier of the TRex node responsible for issuing a request and to which messages containing responses
    /// should be sent on a message topic contained within the derived request. 
    /// </summary>
    public string TRexNodeID { get; set; } = string.Empty;

    /// <summary>
    /// The project the request is relevant to
    /// </summary>
    public Guid ProjectID { get; set; }

    /// <summary>
    /// The set of filters to be applied to the requested subgrids
    /// </summary>
    public IFilterSet Filters { get; set; }

    /// <summary>
    /// The design to be used in cases of cut/fill or DesignHeights subgrid requests
    /// </summary>
    public Guid ReferenceDesignID { get; set; } = Guid.Empty;

    // TODO  LiftBuildSettings  :TICLiftBuildSettings;

    public override void ToBinary(IBinaryRawWriter writer)
    {
      writer.WriteByte(VERSION_NUMBER);

      writer.WriteString(TRexNodeID);
      writer.WriteGuid(ProjectID);
      writer.WriteGuid(ReferenceDesignID);

      writer.WriteBoolean(Filters != null);

      Filters?.ToBinary(writer);
    }

    public override void FromBinary(IBinaryRawReader reader)
    {
      byte readVersionNumber = reader.ReadByte();

      if (readVersionNumber != VERSION_NUMBER)
        throw new TRexSerializationVersionException(VERSION_NUMBER, readVersionNumber);

      TRexNodeID = reader.ReadString();
      ProjectID = reader.ReadGuid() ?? Guid.Empty;
      ReferenceDesignID = reader.ReadGuid() ?? Guid.Empty;

      if (reader.ReadBoolean())
      {
        Filters = DI.DIContext.Obtain<IFilterSet>();
        Filters.FromBinary(reader);
      }
    }

    protected bool Equals(BaseApplicationServiceRequestArgument other)
    {
      return string.Equals(TRexNodeID, other.TRexNodeID) && 
             ProjectID.Equals(other.ProjectID) && 
             Equals(Filters, other.Filters) && 
             ReferenceDesignID.Equals(other.ReferenceDesignID);
    }

    public bool Equals(BaseRequestArgument other)
    {
      return Equals(other as BaseApplicationServiceRequestArgument);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != this.GetType()) return false;
      return Equals((BaseApplicationServiceRequestArgument) obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = (TRexNodeID != null ? TRexNodeID.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ ProjectID.GetHashCode();
        hashCode = (hashCode * 397) ^ (Filters != null ? Filters.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ ReferenceDesignID.GetHashCode();
        return hashCode;
      }
    }
  }
}
