﻿using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSS.VisionLink.Raptor.GridFabric.Affinity;
using VSS.VisionLink.Raptor.GridFabric.Caches;
using VSS.VisionLink.Raptor.SubGridTrees.Server;
using VSS.VisionLink.Raptor.Types;

namespace VSS.VisionLink.Raptor.Storage
{
    public class StorageProxy_IgniteBase
    {
        protected static IIgnite ignite = null;

        protected static ICache<SubGridSpatialAffinityKey, MemoryStream> mutableSpatialCache = null;
        protected static ICache<SubGridSpatialAffinityKey, MemoryStream> immutableSpatialCache = null;
        protected static ICache<String, MemoryStream> mutableNonSpatialCache = null;
        protected static ICache<String, MemoryStream> immutableNonSpatialCache = null;

        protected static Object LockObj = new Object();

        /// <summary>
        /// Controls whether the immutable cache can be used as the primary source of data to support reads.
        /// Of true, a cache item is read from the immutable cache. If not present there it will be read from the
        /// mutable cache and promoted into the immutable cache
        /// </summary>
        protected bool ReadFromImmutableDataCaches = RaptorServerConfig.Instance().ReadFromImmutableDataCaches;

        public StorageProxy_IgniteBase(string gridName)
        {
            if (ignite == null)
            {
                ignite = Ignition.TryGetIgnite(gridName);
            }
        }

        protected void EstablishMutableCaches()
        {
            if (ignite != null)
            {
                mutableSpatialCache = ignite.GetCache<SubGridSpatialAffinityKey, MemoryStream>(RaptorCaches.MutableSpatialCacheName());
                mutableNonSpatialCache = ignite.GetCache<String, MemoryStream>(RaptorCaches.MutableNonSpatialCacheName());
            }
        }

        protected void EstablishImmutableCaches()
        {
            if (ignite != null)
            {
                immutableSpatialCache = ignite.GetCache<SubGridSpatialAffinityKey, MemoryStream>(RaptorCaches.ImmutableSpatialCacheName());
                immutableNonSpatialCache = ignite.GetCache<String, MemoryStream>(RaptorCaches.ImmutableNonSpatialCacheName());
            }
        }
        /// <summary>
        /// Computes the cache key name for a given data model and a given named stream within that datamodel
        /// </summary>
        /// <param name="DataModelID"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        protected static string ComputeNamedStreamCacheKey(long DataModelID, string Name)
        {
            return String.Format("{0}-{1}", DataModelID, Name);
        }

        /// <summary>
        /// Computes the cache key name for the given data model and a given spatial data stream within that datamodel
        /// </summary>
        /// <param name="DataModelID"></param>
        /// <param name="Name"></param>
        /// <param name="SubgridX"></param>
        /// <param name="SubgridY"></param>
        /// <returns></returns>
        protected static string ComputeNamedStreamCacheKey(long DataModelID, string Name, uint SubgridX, uint SubgridY)
        {
            return String.Format("{0}-{1}-{2}-{3}", DataModelID, Name, SubgridX, SubgridY);
        }

        /// <summary>
        /// Supports taking a mutable version of a piece of data and transforming it into the immutable form if not present in the immutable cache
        /// </summary>
        /// <param name="mutableCache"></param>
        /// <param name="immutableCache"></param>
        /// <param name="cacheKey"></param>
        /// <param name="streamType"></param>
        /// <returns></returns>
        protected MemoryStream PerformNonSpatialImmutabilityConversion(ICache<String, MemoryStream> mutableCache,
                                                                       ICache<String, MemoryStream> immutableCache,
                                                                       string cacheKey,
                                                                       FileSystemStreamType streamType)
        {
            MemoryStream immutableStream = null;
            MemoryStream mutableStream = mutableCache.Get(cacheKey);

            // If successfully read, convert from the mutable to the immutable form and store it into the immutable cache
            if (mutableStream != null)
            {
                if (MutabilityConverter.ConvertToImmutable(streamType, mutableStream, out immutableStream) && (immutableStream != null))
                {
                    // Place the converted immutable item into the immutable cache
                    immutableCache.Put(cacheKey, immutableStream);
                }
                else
                {
                    // There was no immutable version of the requested information. Allow this to bubble up the stack...
                    // TODO Log the failure

                    immutableStream = null;
                }
            }

            return immutableStream;
        }

        /// <summary>
        /// Supports taking a mutable version of a piece of data and transforming it into the immutable form if not present in the immutable cache
        /// </summary>
        /// <param name="mutableCache"></param>
        /// <param name="immutableCache"></param>
        /// <param name="cacheKey"></param>
        /// <param name="streamType"></param>
        /// <returns></returns>
        protected MemoryStream PerformSpatialImmutabilityConversion(ICache<SubGridSpatialAffinityKey, MemoryStream> mutableCache,
                                                                  ICache<SubGridSpatialAffinityKey, MemoryStream> immutableCache,
                                                                  SubGridSpatialAffinityKey cacheKey,
                                                                  FileSystemStreamType streamType)
        {
            MemoryStream immutableStream = null;
            MemoryStream mutableStream = mutableCache.Get(cacheKey);

            // If successfully read, convert from the mutable to the immutable form and store it into the immutable cache
            if (mutableStream != null)
            {
                if (MutabilityConverter.ConvertToImmutable(streamType, mutableStream, out immutableStream) && (immutableStream != null))
                {
                    // Place the converted immutable item into the immutable cache
                    immutableCache.Put(cacheKey, immutableStream);
                }
                else
                {
                    // There was no immutable version of the requested information. Allow this to bubble up the stack...
                    // TODO Log the failure

                    immutableStream = null;
                }
            }

            return immutableStream;
        }
    }
}
