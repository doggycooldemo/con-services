﻿using Apache.Ignite.Core.Binary;
using VSS.TRex.Common;
using VSS.TRex.GridFabric.Arguments;

namespace VSS.TRex.CoordinateSystems.GridFabric.Arguments
{
  /// <summary>
  /// Contains a coordinate system expressed as a CSIB (Coordinate System Information Block) encoded as a string to s project
  /// </summary>
  public class AddCoordinateSystemArgument : BaseApplicationServiceRequestArgument
  {
    private const byte VERSION_NUMBER = 1;

    /// <summary>
    /// The CSIB encoded as a string
    /// </summary>
    public string CSIB;

    /// <summary>
    /// Serializes content to the writer
    /// </summary>
    public override void InternalToBinary(IBinaryRawWriter writer)
    {
      base.InternalToBinary(writer);

      VersionSerializationHelper.EmitVersionByte(writer, VERSION_NUMBER);

      writer.WriteString(CSIB);
    }

    /// <summary>
    /// Serializes content from the writer
    /// </summary>
    public override void InternalFromBinary(IBinaryRawReader reader)
    {
      base.InternalFromBinary(reader);

      var version = VersionSerializationHelper.CheckVersionByte(reader, VERSION_NUMBER);

      if (version == 1)
      {
        CSIB = reader.ReadString();
      }
    }
  }
}
