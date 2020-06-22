﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VSS.TRex.DI;
using VSS.TRex.GridFabric;
using VSS.TRex.GridFabric.Interfaces;
using VSS.TRex.SiteModels.Interfaces;
using VSS.TRex.SiteModels.Interfaces.Executors;
using VSS.TRex.Storage.Interfaces;
using VSS.TRex.TAGFiles.Models;

namespace VSS.TRex.SiteModels
{
  /// <summary>
  /// Manages the life cycle of activities in the project rebuilder across the set of projects being rebuilt
  /// </summary>
  public class SiteModelRebuilderManager : ISiteModelRebuilderManager
  {
    private static ILogger _log = Logging.Logger.CreateLogger<SiteModelRebuilderManager>();

    /// <summary>
    /// The collection of rebuilder the manager is looking after
    /// </summary>
    private Dictionary<Guid, (ISiteModelRebuilder, Task<IRebuildSiteModelMetaData>)> Rebuilders = new Dictionary<Guid, (ISiteModelRebuilder, Task<IRebuildSiteModelMetaData>)>();

    /// <summary>
    /// The storage proxy cache for the rebuilder to use for tracking metadata
    /// </summary>
    private IStorageProxyCache<INonSpatialAffinityKey, IRebuildSiteModelMetaData> MetadataCache { get; }

    /// <summary>
    /// The storage proxy cache for the rebuilder to use to store names of TAG files requested from S3
    /// </summary>
    public IStorageProxyCache<INonSpatialAffinityKey, ISerialisedByteArrayWrapper> FilesCache { get; }

    public SiteModelRebuilderManager()
    {
      MetadataCache = DIContext.Obtain<Func<RebuildSiteModelCacheType, IStorageProxyCacheCommit>>()(RebuildSiteModelCacheType.Metadata)
        as IStorageProxyCache<INonSpatialAffinityKey, IRebuildSiteModelMetaData>;

      FilesCache = DIContext.Obtain<Func<RebuildSiteModelCacheType, IStorageProxyCacheCommit>>()(RebuildSiteModelCacheType.KeyCollections)
         as IStorageProxyCache<INonSpatialAffinityKey, ISerialisedByteArrayWrapper>;
    }

    public bool Rebuild(Guid projectUid, bool archiveTAGFiles)
    {
      _log.LogInformation($"Site model rebuilder executing rebuild for proejct {projectUid}, archiving tag files = {archiveTAGFiles}");

      // Check if there is an existing rebuilder 
      if (Rebuilders.TryGetValue(projectUid, out var existingRebuilder))
      {
        _log.LogError($"A site model rebuilder for project {projectUid} is already present, current phase is {existingRebuilder.Item1.Metadata.Phase}");
        return false;
      }

      var rebuilder = DIContext.Obtain<Func<Guid, bool, ISiteModelRebuilder>>()(projectUid, archiveTAGFiles);
      // Inject cahces
      rebuilder.MetadataCache = MetadataCache;
      rebuilder.FilesCache = FilesCache;

      lock (Rebuilders)
      {
        Rebuilders.Add(projectUid, (rebuilder, rebuilder.ExecuteAsync()));
      }
      return true;
    }

    /// <summary>
    /// The total number of rebuilders being managed by the rebuild project manager
    /// </summary>
    public int RebuildCount()
    {
      lock (Rebuilders)
      {
        return Rebuilders.Count;
      }
    }

    /// <summary>
    /// Supplies a vector of meta data state relating to project builders present in the manager
    /// </summary>
    /// <returns></returns>
    public List<IRebuildSiteModelMetaData> GetRebuildersState()
    {
      lock (Rebuilders)
      {
        return Rebuilders.Values.Select(x => x.Item1.Metadata).ToList();
      }
    }

    /// <summary>
    /// Handles an event generated from the TAG file processor that a TAG file has been processed with the notifiy rebuilder flag set on it
    /// </summary>
    /// <param name="projectUid"></param>
    /// <param name="responseItem"></param>
    public void TAGFileProcessed(Guid projectUid, IProcessTAGFileResponseItem[] responseItems)
    {
      lock (Rebuilders)
      {
        if (Rebuilders.TryGetValue(projectUid, out var rebuilder))
        {
          _log.LogWarning($"Site model rebuilder manager received {responseItems.Length} TAG file notifications for {projectUid}");

          rebuilder.Item1.TAGFilesProcessed(responseItems);
        }
        else
        {
          _log.LogWarning($"Site model rebuilder manager found no active rebuilder for project {projectUid}");
        }
      }
    }
  }
}
