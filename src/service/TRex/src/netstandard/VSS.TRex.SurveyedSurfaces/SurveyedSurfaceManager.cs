﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using VSS.TRex.Common.Exceptions;
using VSS.TRex.Common.Utilities.ExtensionMethods;
using VSS.TRex.Designs.Models;
using VSS.TRex.DI;
using VSS.TRex.ExistenceMaps.GridFabric.Requests;
using VSS.TRex.Geometry;
using VSS.TRex.SiteModels.Interfaces;
using VSS.TRex.SiteModels.Interfaces.Events;
using VSS.TRex.Storage.Interfaces;
using VSS.TRex.Storage.Models;
using VSS.TRex.SubGridTrees.Interfaces;
using VSS.TRex.SurveyedSurfaces.Interfaces;
using VSS.TRex.Types;

namespace VSS.TRex.SurveyedSurfaces
{
  /// <summary>
  /// The surveyed surface manager responsible for orchestrating access and mutations against the surveyed surfaces held for a project.
  /// </summary>
  public class SurveyedSurfaceManager : ISurveyedSurfaceManager
  {
    private static readonly ILogger _log = Logging.Logger.CreateLogger<SurveyedSurfaceManager>();

    private readonly IStorageProxy _writeStorageProxy;
    private readonly IStorageProxy _readStorageProxy;

    public const string SURVEYED_SURFACE_STREAM_NAME = "SurveyedSurfaces";

    /// <summary>
    /// Constructs an instance using the supplied storage proxy
    /// </summary>
    public SurveyedSurfaceManager(StorageMutability mutability)
    {
      _writeStorageProxy = DIContext.Obtain<ISiteModels>().PrimaryMutableStorageProxy;
      _readStorageProxy = DIContext.Obtain<ISiteModels>().PrimaryStorageProxy(mutability);
    }

    /// <summary>
    /// Loads the set of surveyed surfaces for a site model. If none exist and empty list is returned.
    /// </summary>
    private ISurveyedSurfaces Load(Guid siteModelUid)
    {
      _log.LogInformation($"Loading surveyed surfaces for project {siteModelUid}");

      var ss = DIContext.ObtainRequired<ISurveyedSurfaces>();

      try
      {
        _readStorageProxy.ReadStreamFromPersistentStore(siteModelUid, SURVEYED_SURFACE_STREAM_NAME, FileSystemStreamType.SurveyedSurfaces, out var ms);

        if (ms != null)
        {
          using (ms)
          {
            ss.FromStream(ms);
          }
        }
      }
      catch (KeyNotFoundException)
      {
        /* This is OK, the element is not present in the cache yet */
      }
      catch (Exception e)
      {
        throw new TRexException("Exception reading Alignment cache element from Ignite", e);
      }

      _log.LogInformation($"Loaded {ss.Count} surveyed surfaces for project {siteModelUid}");

      return ss;
    }

    /// <summary>
    /// Stores the list of surveyed surfaces for a site model
    /// </summary>
    private void Store(Guid siteModelUid, ISurveyedSurfaces surveyedSurfaces)
    {
      using var stream = surveyedSurfaces.ToStream();
      if (_writeStorageProxy.WriteStreamToPersistentStore(siteModelUid, SURVEYED_SURFACE_STREAM_NAME,
        FileSystemStreamType.SurveyedSurfaces, stream, this) == FileSystemErrorStatus.OK)
      {
        _writeStorageProxy.Commit();
      }

      // Notify the  grid listeners that attributes of this site model have changed.
      var sender = DIContext.Obtain<ISiteModelAttributesChangedEventSender>();
      sender.ModelAttributesChanged(SiteModelNotificationEventGridMutability.NotifyAll, siteModelUid, surveyedSurfacesChanged: true);
    }

    /// <summary>
    /// Add a new surveyed surface to a site model
    /// </summary>
    public ISurveyedSurface Add(Guid siteModelUid, DesignDescriptor designDescriptor, DateTime asAtDate, BoundingWorldExtent3D extents,
      ISubGridTreeBitMask existenceMap)
    {
      if (asAtDate.Kind != DateTimeKind.Utc)
        throw new ArgumentException("AsAtDate must be a UTC date time");

      if (extents == null)
        throw new ArgumentNullException(nameof(extents));

      if (existenceMap == null)
        throw new ArgumentNullException(nameof(existenceMap));

      var ss = Load(siteModelUid);
      var newSurveyedSurface = ss.AddSurveyedSurfaceDetails(designDescriptor.DesignID, designDescriptor, asAtDate, extents);

      // Store the existence map into the cache
      using var stream = existenceMap.ToStream();
      var fileName = BaseExistenceMapRequest.CacheKeyString(ExistenceMaps.Interfaces.Consts.EXISTENCE_SURVEYED_SURFACE_DESCRIPTOR, designDescriptor.DesignID);
      if (_writeStorageProxy.WriteStreamToPersistentStore(siteModelUid, fileName,
                                                          FileSystemStreamType.DesignTopologyExistenceMap, stream, existenceMap) != FileSystemErrorStatus.OK)
      {
        _log.LogError("Failed to write existence map to persistent store for key {fileName}");
        return null;
      }

      // Store performs Commit() operation
      Store(siteModelUid, ss);

      return newSurveyedSurface;
    }

    /// <summary>
    /// List the surveyed surfaces for a site model
    /// </summary>
    public ISurveyedSurfaces List(Guid siteModelUid)
    {
      _log.LogInformation($"Listing surveyed surfaces from site model {siteModelUid}");

      return Load(siteModelUid);
    }

    /// <summary>
    /// Remove a given surveyed surface from a site model
    /// </summary>
    public bool Remove(Guid siteModelUid, Guid surveySurfaceUid)
    {
      var ss = Load(siteModelUid);
      var result = ss.RemoveSurveyedSurface(surveySurfaceUid);

      if (result)
      {
        var removeMapResult = _writeStorageProxy.RemoveStreamFromPersistentStore(siteModelUid, FileSystemStreamType.DesignTopologyExistenceMap,
          BaseExistenceMapRequest.CacheKeyString(ExistenceMaps.Interfaces.Consts.EXISTENCE_SURVEYED_SURFACE_DESCRIPTOR, surveySurfaceUid));

        if (removeMapResult != FileSystemErrorStatus.OK)
        {
          _log.LogInformation($"Removing surveyed surface existence map for surveyed surface {surveySurfaceUid} project {siteModelUid} failed with error {removeMapResult}");
        }

        Store(siteModelUid, ss);
      }

      return result;
    }

    /// <summary>
    /// Remove the surveyed surface list for a site model from the persistent store
    /// </summary>
    public bool Remove(Guid siteModelUid, IStorageProxy storageProxy)
    {
      // First remove all the existence maps associated with the surveyed surfaces
      foreach(var surveyedSurface in Load(siteModelUid))
      {
        FileSystemErrorStatus fsResult;
        var filename = BaseExistenceMapRequest.CacheKeyString(ExistenceMaps.Interfaces.Consts.EXISTENCE_SURVEYED_SURFACE_DESCRIPTOR, surveyedSurface.ID);
        if ((fsResult = storageProxy.RemoveStreamFromPersistentStore(siteModelUid, FileSystemStreamType.DesignTopologyExistenceMap, filename)) != FileSystemErrorStatus.OK)
        {
          _log.LogWarning($"Unable to remove existence map for surveyed surface {surveyedSurface.ID}, filename = {filename}, in project {siteModelUid} with result: {fsResult}");
        }
      }

      // Then remove the surveyed surface list stream itself
      var result = storageProxy.RemoveStreamFromPersistentStore(siteModelUid, FileSystemStreamType.Designs, SURVEYED_SURFACE_STREAM_NAME);

      if (result != FileSystemErrorStatus.OK)
      {
        _log.LogInformation($"Removing surveyed surfaces list from project {siteModelUid} failed with error {result}");
      }

      return result == FileSystemErrorStatus.OK;
    }
  }
}
