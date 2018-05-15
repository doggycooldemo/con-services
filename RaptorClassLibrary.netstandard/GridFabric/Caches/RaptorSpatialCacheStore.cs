﻿using Apache.Ignite.Core.Cache.Store;
using System;
using System.IO;
using VSS.TRex.GridFabric.Affinity;

namespace VSS.TRex.GridFabric.Caches
{
    public class RaptorSpatialCacheStore : CacheStoreAdapter<SubGridSpatialAffinityKey, MemoryStream>
    {
        private TRexCacheStoreUtilities Utilities;

        public RaptorSpatialCacheStore(string mutabilitySuffix)
        {
            Utilities = new TRexCacheStoreUtilities(mutabilitySuffix);
        }

        public override void Delete(SubGridSpatialAffinityKey key)
        {
            Utilities.Delete(key.ToString());
        }

        public override MemoryStream Load(SubGridSpatialAffinityKey key)
        {
            return Utilities.Load(key.ToString());
        }

        public override void LoadCache(Action<SubGridSpatialAffinityKey, MemoryStream> act, params object[] args)
        {
            // Ignore - not a supported activity
            // throw new NotImplementedException();
        }

        public override void SessionEnd(bool commit)
        {
            // Ignore, nothign to do
            // throw new NotImplementedException();
        }

        public override void Write(SubGridSpatialAffinityKey key, MemoryStream val)
        {
            Utilities.Write(key.ToString(), val);
        }
    }
}
