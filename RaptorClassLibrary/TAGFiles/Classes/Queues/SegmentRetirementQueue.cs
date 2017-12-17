﻿using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Cache.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSS.VisionLink.Raptor.GridFabric.Grids;

namespace VSS.VisionLink.Raptor.TAGFiles.Classes.Queues
{
    public class SegmentRetirementQueue
    {
        private ICache<long, SegmentRetirementQueueItem> QueueCache = null;

        /// <summary>
        /// Name of the cache holding the segments in the data model that need to be retired due to being
        /// replaced by small cloven segments as a result of TAG file processing
        /// </summary>
        private static string SEGMENT_RETIREMENT_QUEUE_CACHE_NAME = "SegmentRetirementQueue";

        private void Add(DateTime date, string value)
        {
            QueueCache.Put(date.ToBinary(), new SegmentRetirementQueueItem(date.ToBinary(), value));
        }

        public SegmentRetirementQueue()
        {
            IIgnite Ignite = Ignition.GetIgnite(RaptorGrids.RaptorGridName());
            QueueCache = Ignite.GetOrCreateCache<long, SegmentRetirementQueueItem>(
                new CacheConfiguration
                {
                    Name = SEGMENT_RETIREMENT_QUEUE_CACHE_NAME,
                    QueryEntities = new[] 
                    {
                        new QueryEntity(typeof(long), typeof(SegmentRetirementQueueItem))
                    },
                    KeepBinaryInStore = true
                });
        }

        public IEnumerable<SegmentRetirementQueueItem> Query(DateTime earlierThan)
        {
            var sql = new SqlQuery(typeof(SegmentRetirementQueueItem), $"_key < {earlierThan.ToBinary().ToString()}");
            var cursor = QueueCache.Query(sql);

            try
            {
                return QueueCache.Query(sql).Select(x => x.Value).ToArray();
            }
            catch
            {
                return null;
            }
        }

/*
Add(DateTime.Now, "First");
Add(DateTime.Now, "Second");
Add(DateTime.Now, "Third");
Add(DateTime.Now, "Fourth");
Add(DateTime.Now, "Fifth");
*/ 
    }
}
