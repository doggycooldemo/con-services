﻿using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Resource;
using Apache.Ignite.Core.Services;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VSS.VisionLink.Raptor.Geometry;
using VSS.VisionLink.Raptor.Surfaces;
using VSS.VisionLink.Raptor.Utilities.ExtensionMethods;

namespace VSS.VisionLink.Raptor.Services.Surfaces
{

    /// <summary>
    /// A test of how to manage surveyed surfaces
    /// </summary>
    public class SurveyedSurfaceService : BaseRaptorService, IService, ISurveyedSurfaceService
    {
        [NonSerialized]
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Cache storing sitemodel instances
        /// </summary>
        private ICache<String, Byte[]> mutableNonSpatialCache;

        /// <summary>
        /// Service name.
        /// </summary>
        private string _svcName;

        private string GridName;
        private string CacheName;

        public SurveyedSurfaceService() : base()
        {

        }

        public SurveyedSurfaceService(string gridName, string cacheName) : this()
        {
            GridName = gridName;
            CacheName = cacheName;
        }

        /// <summary>
        /// Add a new surveyed surface to a sitemodel
        /// </summary>
        /// <param name="SiteModelID"></param>
        /// <param name="designDescriptor"></param>
        /// <param name="AsAtDate"></param>
        public void Add(long SiteModelID, DesignDescriptor designDescriptor, DateTime asAtDate, BoundingWorldExtent3D extents )
        {
            mutableNonSpatialCache.Invoke(SurveyedSurfaces.CacheKey(SiteModelID), 
                                          new AddSurveyedSurfaceProcessor(), 
                                          new SurveyedSurface(SiteModelID, designDescriptor, asAtDate, extents));
        }

        /// <summary>
        /// Add a new surveyed surface to a sitemodel
        /// </summary>
        /// <param name="SiteModelID"></param>
        /// <param name="designDescriptor"></param>
        /// <param name="AsAtDate"></param>
        public void AddDirect(long SiteModelID, DesignDescriptor designDescriptor, DateTime asAtDate, BoundingWorldExtent3D extents, out long SuveyedSurfaceID)
        {
            // TODO: This should be done under a lock on the cache key. For now, we will live with the race condition

            string cacheKey = SurveyedSurfaces.CacheKey(SiteModelID);
            SuveyedSurfaceID = Guid.NewGuid().GetHashCode();

            // Get the surveyed surfaces, creating it if it does not exist
            SurveyedSurfaces ssList = new SurveyedSurfaces();

            try
            {
                ssList.FromBytes(mutableNonSpatialCache.Get(cacheKey));
            }
            catch (KeyNotFoundException)
            {
                // Swallow exception, the list will be empty
            }
            catch
            {
                throw;
            }

            // Add the new surveyed surface, generating a random ID from a GUID
            SurveyedSurface ss = ssList.AddSurveyedSurfaceDetails(SuveyedSurfaceID, designDescriptor, asAtDate, extents);

            // Put the list back into the cache with the new entry
            mutableNonSpatialCache.Put(cacheKey, ssList.ToBytes());
        }

        public SurveyedSurfaces List(long SiteModelID)
        {
            Log.InfoFormat($"Listing surveyed surfaces from {SurveyedSurfaces.CacheKey(SiteModelID)}");

            try
            {
                SurveyedSurfaces ss = new SurveyedSurfaces();
                ss.FromBytes(mutableNonSpatialCache.Get(SurveyedSurfaces.CacheKey(SiteModelID)));
                return ss;
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        public SurveyedSurfaces ListDirect(long SiteModelID) => List(SiteModelID);

        /// <summary>
        /// Defines the actions to take if the service is cancelled
        /// </summary>
        /// <param name="context"></param>
        public void Cancel(IServiceContext context)
        {
            mutableNonSpatialCache.Remove(_svcName);
        }

        /// <summary>
        /// Defines the actions to take when the service is first executed
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IServiceContext context)
        {
           Log.Info($"Executing Raptor Service 'SurveyedSurfaceService'");
        }

        /// <summary>
        /// Defines the actions to take when the service is initialised prior to execution
        /// </summary>
        /// <param name="context"></param>
        public void Init(IServiceContext context)
        {
            if (context != null)
            {
                _svcName = context.Name;
            }

            mutableNonSpatialCache = _ignite.GetCache<String, Byte[]>(CacheName /*RaptorCaches.MutableNonSpatialCacheName()*/);
        }

        /// <summary>
        /// Remove a given surveyed surface from a site model
        /// </summary>
        /// <param name="SiteModelID"></param>
        /// <param name="SurveySurfaceID"></param>
        /// <returns></returns>
        public bool Remove(long SiteModelID, long SurveySurfaceID)
        {
            try
            {
                return mutableNonSpatialCache.Invoke(SurveyedSurfaces.CacheKey(SiteModelID),
                                                     new RemoveSurveyedSurfaceProcessor(),
                                                     SurveySurfaceID);
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        /// <summary>
        /// Remove a given surveyed surface from a site model
        /// </summary>
        /// <param name="SiteModelID"></param>
        /// <param name="SurveySurfaceID"></param>
        /// <returns></returns>
        public bool RemoveDirect(long SiteModelID, long SurveySurfaceID)
        {
            // TODO: This should be done under a lock on the cache key. For now, we will live with the race condition

            try
            {
                string cacheKey = SurveyedSurfaces.CacheKey(SiteModelID);

                // Get the surveyed surfaces, creating it if it does not exist
                SurveyedSurfaces ssList = new SurveyedSurfaces();
                ssList.FromBytes(mutableNonSpatialCache.Get(cacheKey));

                if (ssList == null)
                {
                    ssList = new SurveyedSurfaces();
                }

                // Add the new surveyed surface, generating a random ID from a GUID
                bool result = ssList.RemoveSurveyedSurface(SurveySurfaceID);

                // Put the list back into the cache with the new entry
                if (result)
                {
                    mutableNonSpatialCache.Put(cacheKey, ssList.ToBytes());
                }

                return result;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }
    }
}
