﻿using System;
using Apache.Ignite.Core.Binary;
using VSS.TRex.GridFabric.Arguments;

namespace VSS.TRex.Designs.GridFabric.Arguments
{
  public class DesignSubGridRequestArgumentBase : BaseApplicationServiceRequestArgument
  {
    /// <summary>
    /// The offset to be applied to computed elevations
    /// </summary>
    public double Offset { get; set; }

    /// <summary>
    /// Default no-arg constructor
    /// </summary>
    public DesignSubGridRequestArgumentBase()
    {
    }

    /// <summary>
    /// Constructor taking the full state of the elevation patch computation operation
    /// </summary>
    /// <param name="siteModelID"></param>
    /// <param name="referenceDesignUID"></param>
    public DesignSubGridRequestArgumentBase(Guid siteModelID,
                                     Guid referenceDesignUID,
                                     double offset) : this()
    {
      ProjectID = siteModelID;
      ReferenceDesignUID = referenceDesignUID;
      Offset = offset;
    }

    /// <summary>
    /// Overloaded ToString to add argument properties
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return base.ToString() + $" -> SiteModel:{ProjectID}, Design:{ReferenceDesignUID}, Offset:{Offset}";
    }

    /// <summary>
    /// Serializes content to the writer
    /// </summary>
    /// <param name="writer"></param>
    public override void ToBinary(IBinaryRawWriter writer)
    {
      base.ToBinary(writer);

      writer.WriteGuid(ReferenceDesignUID);
      writer.WriteDouble(Offset);
    }

    /// <summary>
    /// Serializes content from the writer
    /// </summary>
    /// <param name="reader"></param>
    public override void FromBinary(IBinaryRawReader reader)
    {
      base.FromBinary(reader);

      ReferenceDesignUID = reader.ReadGuid() ?? Guid.Empty;
      Offset = reader.ReadDouble();
    }
  }
}
