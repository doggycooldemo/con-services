﻿using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using VSS.Common.Abstractions.Configuration;
using VSS.TRex.Common;
using VSS.TRex.Common.Exceptions;
using VSS.TRex.DI;
using VSS.TRex.SubGridTrees.Server.Interfaces;

namespace VSS.TRex.SubGridTrees.Server
{
  public class SubGridCellPassesDataSegments : ISubGridCellPassesDataSegments
  {
    private static readonly ILogger _log = Logging.Logger.CreateLogger<SubGridCellPassesDataSegments>();

    private static readonly bool _performSegmentAdditionIntegrityChecks = DIContext.Obtain<IConfigurationStore>().GetValueBool("DEBUG_PERFORMSEGMENT_ADDITIONALINTEGRITYCHECKS", Consts.DEBUG_PERFORMSEGMENT_ADDITIONALINTEGRITYCHECKS);

    public List<ISubGridCellPassesDataSegment> Items { get; set; } = new List<ISubGridCellPassesDataSegment>();

    public void Clear()
    {
      Items?.Clear();
    }

    public int Count => Items?.Count ?? 0;

    public ISubGridCellPassesDataSegment this[int index] => Items[index];

    public int Add(ISubGridCellPassesDataSegment item)
    {
      int Index = Count - 1;

      while (Index >= 0 && item.SegmentInfo.StartTime < Items[Index].SegmentInfo.StartTime)
      {
        Index--;
      }

      Index++;

      Items.Insert(Index, item);

      return Index;

      /*
       *  {$IFDEF DEBUG}
        Counter := 0;
        for Index := 0 to Count - 2 do
          if Items[Index].SegmentInfo.StartTime >= Items[Index + 1].SegmentInfo.StartTime then
            begin
              SIGLogMessage.PublishNoODS(Self, Format('Segment passes list out of order %.6f versus %.6f. Segment count = %d', { SKIP}
      [Items[Index].SegmentInfo.StartTime, Items[Index + 1].SegmentInfo.StartTime, Count]), slmcAssert);
              Inc(Counter);
      end;
        if Counter > 0 then
          DumpSegmentsToLog;
        {$ENDIF}
      */
    }

    /// <summary>
    /// Dumps the segment metadata for this sub grid to the log
    /// </summary>
    private void DumpSegmentsToLog()
    {
      for (int i = 0; i < Count; i++)
        _log.LogInformation($"Seg #{i}: {Items[i]}");
    }

    public ISubGridCellPassesDataSegment AddNewSegment(IServerLeafSubGrid subGrid,
      ISubGridCellPassesDataSegmentInfo segmentInfo)
    {
      if (segmentInfo == null)
        throw new TRexSubGridProcessingException($"Null segment info passed to AddNewSegment for sub grid {subGrid.Moniker()}");

      if (segmentInfo.Segment != null)
        throw new TRexSubGridProcessingException($"Segment info passed to AddNewSegment for sub grid {subGrid.Moniker()} already contains an allocated segment");

      var Result = new SubGridCellPassesDataSegment
      {
        Owner = subGrid,
        SegmentInfo = segmentInfo
      };
      segmentInfo.Segment = Result;

      for (int I = 0; I < Count; I++)
      {
        if (segmentInfo.EndTime <= Items[I].SegmentInfo.StartTime)
        {
          Items.Insert(I, Result);

          if (_performSegmentAdditionIntegrityChecks)
          {
            for (int J = 0; J < Count - 1; J++)
              if (Items[J].SegmentInfo.StartTime >= Items[J + 1].SegmentInfo.StartTime)
              {
                _log.LogError($"Segment passes list out of order {Items[J].SegmentInfo.StartTime} versus {Items[J + 1].SegmentInfo.StartTime}. Segment count = {Count}");
                DumpSegmentsToLog();
                throw new TRexSubGridProcessingException($"Segment passes list out of order {Items[J].SegmentInfo.StartTime} versus {Items[J + 1].SegmentInfo.StartTime}. Segment count = {Count}");
              }
          }

          return Result;
        }
      }

      // if we get to here, then the new segment is at the end of the list, so just add it to the end
      Add(Result);

      return Result;
    }
  }
}
