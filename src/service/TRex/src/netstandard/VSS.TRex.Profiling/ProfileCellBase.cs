﻿using Apache.Ignite.Core.Binary;
using VSS.TRex.Common;
using VSS.TRex.Profiling.Interfaces;

namespace VSS.TRex.Profiling
{
  public abstract class ProfileCellBase : VersionCheckedBinarizableSerializationBase, IProfileCellBase
  {
    private const byte VERSION_NUMBER = 1;

    /// <summary>
    /// The real-world distance from the 'start' of the profile line drawn by the user;
    /// this is used to ensure that the client GUI correctly aligns the profile
    /// information drawn in the Long Section view with the profile line on the Plan View.
    /// </summary>
    public double Station { get; set; }

    /// <summary>
    /// The real-world length of that part of the profile line which crosses the underlying cell;
    /// used to determine the width of the profile column as displayed in the client GUI
    /// </summary>
    public double InterceptLength { get; set; }

    /// <summary>
    /// OTGCellX, OTGCellY is the on the ground index of the this particular grid cell
    /// </summary>
    public int OTGCellX { get; set; }

    /// <summary>
    /// OTGCellX, OTGCellY is the on the ground index of the this particular grid cell
    /// </summary>
    public int OTGCellY { get; set; }

    public float DesignElev { get; set; }

    public abstract bool IsNull();

    /// <summary>
    /// Serializes content to the writer
    /// </summary>
    public override void InternalToBinary(IBinaryRawWriter writer)
    {
      VersionSerializationHelper.EmitVersionByte(writer, VERSION_NUMBER);

      writer.WriteDouble(Station);
      writer.WriteDouble(InterceptLength);

      writer.WriteInt(OTGCellX);
      writer.WriteInt(OTGCellY);

      writer.WriteFloat(DesignElev);
    }

    /// <summary>
    /// Serializes content from the writer
    /// </summary>
    public override void InternalFromBinary(IBinaryRawReader reader)
    {
      var version = VersionSerializationHelper.CheckVersionByte(reader, VERSION_NUMBER);

      if (version == 1)
      {
        Station = reader.ReadDouble();
        InterceptLength = reader.ReadDouble();

        OTGCellX = reader.ReadInt();
        OTGCellY = reader.ReadInt();

        DesignElev = reader.ReadFloat();
      }
    }
  }
}
