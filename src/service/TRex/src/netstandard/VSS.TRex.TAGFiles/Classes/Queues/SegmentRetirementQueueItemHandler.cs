﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Apache.Ignite.Core.Cache;
using Microsoft.Extensions.Logging;
using VSS.Common.Abstractions.Configuration;
using VSS.TRex.DI;
using VSS.TRex.GridFabric.Interfaces;
using VSS.TRex.Storage.Interfaces;
using VSS.TRex.TAGFiles.Models;
using VSS.TRex.Types;

namespace VSS.TRex.TAGFiles.Classes.Queues
{
  public class SegmentRetirementQueueItemHandler
  {
    private static readonly ILogger _log = Logging.Logger.CreateLogger<SegmentRetirementQueueItemHandler>();

    private readonly bool _reportDetailedSegmentRetirementActivityToLog = DIContext.ObtainRequired<IConfigurationStore>().GetValueBool("TREX_REPORT_DETAILED_SEGMENT_RETIREMENT_ACTIVITY_TO_LOG", true);

    /// <summary>
    /// Takes a set of segment retirees and removes them from grid storage in both the mutable grid (the 'local' grid) and
    /// the immutable grid (that it is a client of).
    /// Once items are successfully removed from storage (or are no longer contained in storage) they are removed from the retirement queue.
    /// </summary>
    public bool Process(IStorageProxy storageProxy, ICache<ISegmentRetirementQueueKey, SegmentRetirementQueueItem> cache, IEnumerable<SegmentRetirementQueueItem> retirees)
    {
      try
      {
        var sw = Stopwatch.StartNew();

        var count = 0;
        storageProxy.Clear();

        // Process all entries in the retirees list, removing each in turn from the cache.
        if (cache == null)
        {
          _log.LogError($"Cache supplied to segment retirement queue processor is null. {retirees.Count()} retirement groups are pending removal. Aborting");
          return false;
        }

        if (retirees == null)
        {
          _log.LogError("Retirees list supplied to segment retirement queue processor is null. Aborting");
          return false;
        }

        if (storageProxy.ImmutableProxy == null)
        {
          _log.LogError("Immutable proxy not available in provided storage proxy. Aborting");
          return false;
        }

        var spatialSegmentCache = storageProxy.SpatialCache(FileSystemStreamType.SubGridSegment);
        var spatialImmutableSegmentCache = storageProxy.ImmutableProxy.SpatialCache(FileSystemStreamType.SubGridSegment);

        foreach (var group in retirees)
        {
          if (group == null)
          {
            _log.LogError("Retirees list supplied to segment retirement queue processor contains null items. Aborting");
            return false;
          }

          if (group.SegmentKeys == null || group.SegmentKeys.Length == 0)
          {
            _log.LogError("Retiree groups segment keys list is null or empty. Aborting");
            return false;
          }

          count += group.SegmentKeys.Length;

          _log.LogInformation($"Retiring a group containing {group.SegmentKeys.Length} keys");
          foreach (var key in group.SegmentKeys)
          {
            if (_reportDetailedSegmentRetirementActivityToLog)
              _log.LogInformation($"About to retire {key}");

            if (!spatialSegmentCache.Remove(key))
            {
              _log.LogError($"Mutable segment retirement cache removal for {key} returned false, aborting");
              return false;
            }

            if (!spatialImmutableSegmentCache.Remove(key))
            {
              _log.LogError($"Immutable segment retirement cache removal for {key} returned false, aborting");
              return false;
            }
          }
        }

        _log.LogInformation($"Prepared {count} retirees for removal in {sw.Elapsed}");

        sw = Stopwatch.StartNew();

        // Commit all the deletes for this retiree group
        if (storageProxy.Commit(out var numDeleted, out var numUpdated, out var numBytesWritten))
        {
          _log.LogInformation($"{count} retirees removed from queue cache, requiring {numDeleted} deletions, {numUpdated} updates with {numBytesWritten} bytes written in {sw.Elapsed}");
        }
        else
        {
          _log.LogInformation("Segment retirement commit failed");
          return false;
        }

        return true;
      }
      catch (Exception e)
      {
        _log.LogError(e, "Exception thrown while retiring segments:");
        throw;
      }
    }
  }
}
