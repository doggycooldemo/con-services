﻿using Apache.Ignite.Core.Binary;
using Apache.Ignite.Core.Cache;
using VSS.TRex.Common;
using VSS.TRex.Common.Exceptions;
using VSS.TRex.Common.Interfaces;
using VSS.TRex.GridFabric.Interfaces;
using VSS.TRex.TAGFiles.Models;

namespace VSS.TRex.TAGFiles.Classes.Queues
{
  public class SegmentRetirementQueueQueryFilter : ICacheEntryFilter<ISegmentRetirementQueueKey, SegmentRetirementQueueItem>, IBinarizable, IFromToBinary
  {
    private const byte VERSION_NUMBER = 1;

    public long retirementDateAsLong;

    public SegmentRetirementQueueQueryFilter()
    {
    }

    public SegmentRetirementQueueQueryFilter(long retirementDate)
    {
      retirementDateAsLong = retirementDate;
    }

    public bool Invoke(ICacheEntry<ISegmentRetirementQueueKey, SegmentRetirementQueueItem> entry)
    {
      return entry.Key.InsertUTCAsLong < retirementDateAsLong;
    }

    public void WriteBinary(IBinaryWriter writer) => ToBinary(writer.GetRawWriter());
    public void ReadBinary(IBinaryReader reader) => FromBinary(reader.GetRawReader());

    public void ToBinary(IBinaryRawWriter writer)
    {
      VersionSerializationHelper.EmitVersionByte(writer, VERSION_NUMBER);

      writer.WriteLong(retirementDateAsLong);
    }

    public void FromBinary(IBinaryRawReader reader)
    {
      VersionSerializationHelper.CheckVersionByte(reader, VERSION_NUMBER);

      retirementDateAsLong = reader.ReadLong();
    }
  }
}
