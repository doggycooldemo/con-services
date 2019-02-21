﻿using Apache.Ignite.Core.Binary;
using VSS.TRex.GridFabric.Arguments;

namespace VSS.TRex.CoordinateSystems.GridFabric.Arguments
{
  /// <summary>
  /// Contains a coordinate system expressed as a CSIB (Coordinate System Information Block) encoded as a string to s project
  /// </summary>
  public class AddCoordinateSystemArgument : BaseApplicationServiceRequestArgument
  {
    /// <summary>
    /// The CSIB encoded as a string
    /// </summary>
    public string CSIB;

    /// <summary>
    /// Serializes content to the writer
    /// </summary>
    /// <param name="writer"></param>
    public override void ToBinary(IBinaryRawWriter writer)
    {
      base.ToBinary(writer);

      writer.WriteString(CSIB);
    }

    /// <summary>
    /// Serializes content from the writer
    /// </summary>
    /// <param name="reader"></param>
    public override void FromBinary(IBinaryRawReader reader)
    {
      base.FromBinary(reader);

      CSIB = reader.ReadString();
    }
  }
}
