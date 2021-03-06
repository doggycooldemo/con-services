﻿using System;
using Apache.Ignite.Core.Binary;
using VSS.TRex.Common;
using VSS.TRex.Common.Interfaces;
using VSS.TRex.DI;
using VSS.TRex.GridFabric.Interfaces;

namespace VSS.TRex.TAGFiles.Models
{
  /// <summary>
  /// Represents a segment that has been stored in the persistent layer as a result on TAG file processing that
  /// has subsequently been updated with a later TAG file generated update.
  /// </summary>
  public class SegmentRetirementQueueItem : VersionCheckedBinarizableSerializationBase
  {
    public const byte VERSION_NUMBER = 1;

    /// <summary>
    /// The project this segment retirement queue item refers to
    /// </summary>
    public Guid ProjectUID;

    /// <summary>
    /// The date at which the segment to be retired was inserted into the buffer queue. 
    /// </summary>
//    [QuerySqlField(IsIndexed = true)]
    public long InsertUTCAsLong;

    /// <summary>
    /// The list of keys of the sub grid and segment streams to be retired.
    /// This list is submitted as a single collection of retirement items per integration update epoch in the TAG file processor
    /// </summary>
    public ISubGridSpatialAffinityKey[] SegmentKeys;

    public override void InternalToBinary(IBinaryRawWriter writer)
    {
      VersionSerializationHelper.EmitVersionByte(writer, VERSION_NUMBER);

      writer.WriteGuid(ProjectUID);
      writer.WriteLong(InsertUTCAsLong);

      writer.WriteBoolean(SegmentKeys != null);
      if (SegmentKeys != null)
      {
        writer.WriteInt(SegmentKeys.Length);

        for (var i = 0; i < SegmentKeys.Length; i++)
          ((IFromToBinary) SegmentKeys[i]).ToBinary(writer);
      }
    }

    public override void InternalFromBinary(IBinaryRawReader reader)
    {
      var version = VersionSerializationHelper.CheckVersionByte(reader, VERSION_NUMBER);

      if (version == 1)
      {
        ProjectUID = reader.ReadGuid() ?? Guid.Empty;
        InsertUTCAsLong = reader.ReadLong();

        if (reader.ReadBoolean())
        {
          var numKeys = reader.ReadInt();
          SegmentKeys = new ISubGridSpatialAffinityKey[numKeys];

          var keyFactory = DIContext.Obtain<ISubGridSpatialAffinityKeyFactory>();

          for (var i = 0; i < numKeys; i++)
          {
            SegmentKeys[i] = keyFactory.NewInstance();
            ((IFromToBinary) SegmentKeys[i]).FromBinary(reader);
          }
        }
      }
    }
  }
}
