﻿using System;
using Apache.Ignite.Core.Binary;
using VSS.TRex.Common;
using VSS.TRex.Common.Interfaces;
using VSS.TRex.GridFabric.Interfaces;
using VSS.TRex.Types;

namespace VSS.TRex.GridFabric.Affinity
{
  /// <summary>
  /// The key used to identify machine spatial data change maps within projects
  /// </summary>
  public class SiteModelMachineAffinityKey : ISiteModelMachineAffinityKey, IBinarizable, IFromToBinary, IEquatable<SiteModelMachineAffinityKey>
  {
    private const byte VERSION_NUMBER = 1;

    /// <summary>
    /// The project to process that TAG file into.
    /// This field also provides the affinity key mapping to the nodes in the mutable data grid
    /// </summary>
    public Guid ProjectUID { get; set; }

    public Guid AssetUID { get; set; }

    public FileSystemStreamType StreamType { get; set; }

    public SiteModelMachineAffinityKey()
    {
    }

    /// <summary>
    /// TAG File Buffer Queue key constructor taking project, asset and filename
    /// </summary>
    /// <param name="projectUID"></param>
    /// <param name="assetUID"></param>
    /// <param name="streamType"></param>
    public SiteModelMachineAffinityKey(Guid projectUID, Guid assetUID, FileSystemStreamType streamType)
    {
      ProjectUID = projectUID;
      AssetUID = assetUID;
      StreamType = streamType;
    }

    /// <summary>
    /// Provides string representation of the state of the key
    /// </summary>
    public override string ToString() => $"Project: {ProjectUID}, Asset: {AssetUID}, StreamType:{StreamType}";

    public void WriteBinary(IBinaryWriter writer) => ToBinary(writer.GetRawWriter());

    public void ReadBinary(IBinaryReader reader) => FromBinary(reader.GetRawReader());

    public void ToBinary(IBinaryRawWriter writer)
    {
      VersionSerializationHelper.EmitVersionByte(writer, VERSION_NUMBER);

      writer.WriteGuid(ProjectUID);
      writer.WriteGuid(AssetUID);
      writer.WriteInt((int)StreamType);
    }

    public void FromBinary(IBinaryRawReader reader)
    {
      var version = VersionSerializationHelper.CheckVersionByte(reader, VERSION_NUMBER);

      if (version == 1)
      {
        ProjectUID = reader.ReadGuid() ?? Guid.Empty;
        AssetUID = reader.ReadGuid() ?? Guid.Empty;
        StreamType = (FileSystemStreamType) reader.ReadInt();
      }
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = ProjectUID.GetHashCode();
        hashCode = (hashCode * 397) ^ AssetUID.GetHashCode();
        hashCode = (hashCode * 397) ^ (int) StreamType;
        return hashCode;
      }
    }

    public bool Equals(SiteModelMachineAffinityKey other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return ProjectUID.Equals(other.ProjectUID) && AssetUID.Equals(other.AssetUID) && StreamType == other.StreamType;
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != this.GetType()) return false;
      return Equals((SiteModelMachineAffinityKey) obj);
    }
  }
}
