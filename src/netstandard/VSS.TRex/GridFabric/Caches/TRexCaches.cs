﻿using VSS.TRex.Storage.Models;

namespace VSS.TRex.GridFabric.Caches
{
    /// <summary>
    /// Sptial grid cache provides logic to determine which of the spatial data grid caches an application should read data from
    /// depending on it settings in TRexServerConfig
    /// </summary>
    public static class TRexCaches
    {
        private const string kSpatialMutable = "Spatial-Mutable";
        private const string kSpatialImmutable = "Spatial-Immutable";
        private const string kSpatialImmutableCompressed = "Spatial-Immutable-Compressed";

        private const string kNonSpatialMutable = "NonSpatial-Mutable";
        private const string kNonSpatialImmutable = "NonSpatial-Immutable";
        private const string kNonSpatialImmutableCompressed = "NonSpatial-Immutable"; // Same as compressed as there is currently no distinction

        private const string kDesignTopologyExistenceMaps = "DesignTopologyExistenceMaps";

        private const string kTAGFileBufferQueueCacheName = "TAGFileBufferQueue";

        /// <summary>
        /// Returns the name of the spatial grid cache to use to locate cell and cell pass information
        /// </summary>
        public static string MutableSpatialCacheName() => kSpatialMutable;
/*
         {
            if (TRexServerConfig.Instance().UseMutableSpatialData)
            {
                return kSpatialMutable;
            }

            if (TRexServerConfig.Instance().CompressImmutableSpatialData)
            {
                return kSpatialImmutableCompressed;
            }

            return kSpatialImmutable;
        }
*/
        /// <summary>
        /// Returns the name of the spatial grid cache to use to store mutable cell and cell pass information
        /// </summary>
        /// <returns></returns>
        public static string ImmutableSpatialCacheName()
        {
            if (TRexServerConfig.Instance().CompressImmutableSpatialData)
            {
                return kSpatialImmutableCompressed;
            }

            return kSpatialImmutable;
        }

        /// <summary>
        /// Returns the name of the event grid cache to use to locate machine event and other non spatial information
        /// </summary>
        public static string MutableNonSpatialCacheName() => kNonSpatialMutable;
/*
         {
            if (TRexServerConfig.Instance().UseMutableNonSpatialData)
            {
                return kNonSpatialMutable;
            }

            if (TRexServerConfig.Instance().CompressImmutableNonSpatialData)
            {
                return kNonSpatialImmutableCompressed;
            }

            return kNonSpatialImmutable;
        }
*/

        /// <summary>
        /// Returns the name of the spatial grid cache to use to store immutable cell and cell pass information
        /// </summary>
        /// <returns></returns>
        public static string ImmutableNonSpatialCacheName()
        {
            if (TRexServerConfig.Instance().CompressImmutableNonSpatialData)
            {
                return kNonSpatialImmutableCompressed;
            }

            return kNonSpatialImmutable;
        }

        public static string SpatialCacheName(StorageMutability Mutability) => Mutability == StorageMutability.Mutable ? MutableSpatialCacheName() : ImmutableSpatialCacheName();

        public static string NonSpatialCacheName(StorageMutability Mutability) => Mutability == StorageMutability.Mutable ? MutableNonSpatialCacheName() : ImmutableNonSpatialCacheName();

        /// <summary>
        /// Returns the name of of the design topology existence maps
        /// </summary>
        /// <returns></returns>
        public static string DesignTopologyExistenceMapsCacheName() => kDesignTopologyExistenceMaps;

        /// <summary>
        /// Name of the cache holding queued & buffered TAG files awaiting processing
        /// </summary>
        /// <returns></returns>
        public static string TAGFileBufferQueueCacheName() => kTAGFileBufferQueueCacheName;

        /// <summary>
        /// Name of the cache holding the segments in the data model that need to be retired due to being
        /// replaced by small cloven segments as a result of TAG file processing
        /// </summary>
        public static string SegmentRetirementQueueCacheName() => "SegmentRetirementQueue";
    }
}
