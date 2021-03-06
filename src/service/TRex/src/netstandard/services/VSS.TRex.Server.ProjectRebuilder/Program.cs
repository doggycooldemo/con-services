﻿using System;
using System.Threading;
using System.Threading.Tasks;
using CoreX.Interfaces;
using CoreX.Wrapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VSS.AWS.TransferProxy;
using VSS.AWS.TransferProxy.Interfaces;
using VSS.Common.Abstractions.Configuration;
using VSS.ConfigurationStore;
using VSS.TRex.Common;
using VSS.TRex.Common.Exceptions;
using VSS.TRex.Common.HeartbeatLoggers;
using VSS.TRex.Common.Interfaces;
using VSS.TRex.Common.Interfaces.Interfaces;
using VSS.TRex.DI;
using VSS.TRex.GridFabric;
using VSS.TRex.GridFabric.Grids;
using VSS.TRex.GridFabric.Interfaces;
using VSS.TRex.GridFabric.Models.Servers;
using VSS.TRex.GridFabric.Servers.Client;
using VSS.TRex.SiteModels;
using VSS.TRex.SiteModels.Executors;
using VSS.TRex.SiteModels.Heartbeats;
using VSS.TRex.SiteModels.Interfaces;
using VSS.TRex.SiteModels.Interfaces.Executors;
using VSS.TRex.Storage;
using VSS.TRex.Storage.Interfaces;
using VSS.TRex.Storage.Models;

namespace VSS.TRex.Server.ProjectRebuilder
{
  class Program
  {
    private static IStorageProxyCacheCommit CacheFactory(RebuildSiteModelCacheType cacheType)
    {
      return cacheType switch
      {
        RebuildSiteModelCacheType.Metadata =>
          new StorageProxyCache<INonSpatialAffinityKey, IRebuildSiteModelMetaData>(DIContext.Obtain<ITRexGridFactory>().Grid(StorageMutability.Mutable)?
            .GetCache<INonSpatialAffinityKey, IRebuildSiteModelMetaData>(TRexCaches.SiteModelRebuilderMetaDataCacheName())),

        RebuildSiteModelCacheType.KeyCollections =>
          new StorageProxyCache<INonSpatialAffinityKey, ISerialisedByteArrayWrapper>(DIContext.Obtain<ITRexGridFactory>().Grid(StorageMutability.Mutable)?
            .GetCache<INonSpatialAffinityKey, ISerialisedByteArrayWrapper>(TRexCaches.SiteModelRebuilderFileKeyCollectionsCacheName())),

        _ => throw new TRexException($"Unknown rebuild site model cache type: {cacheType}")
      };
    }

    private static void DependencyInjection()
    {
      DIBuilder.New()
        .AddLogging()
        .Add(x => x.AddSingleton<IConfigurationStore, GenericConfiguration>())
        .Add(x => x.AddSingleton<Func<RebuildSiteModelCacheType, IStorageProxyCacheCommit>>(CacheFactory))

        .Build()
        .Add(x => x.AddSingleton<ICoreXWrapper, CoreXWrapper>())
        .Add(VSS.TRex.IO.DIUtilities.AddPoolCachesToDI)
        .Add(TRexGridFactory.AddGridFactoriesToDI)
        .Add(VSS.TRex.Storage.Utilities.DIUtilities.AddProxyCacheFactoriesToDI)
        .Build()
        .Add(x => x.AddSingleton<ISiteModels>(new SiteModels.SiteModels(StorageMutability.Mutable)))
        .Add(x => x.AddSingleton<ISiteModelFactory>(new SiteModelFactory()))

        .Add(x => x.AddSingleton<ITRexHeartBeatLogger, TRexHeartBeatLogger>())
        .Add(x => x.AddSingleton<ITransferProxyFactory, TransferProxyFactory>())
        .Add(x => x.AddSingleton<Func<Guid, bool, TransferProxyType, ISiteModelRebuilder>>(_ => (projectUid, archiveTAGFiles, transferProxyType) => new SiteModelRebuilder(projectUid, archiveTAGFiles, transferProxyType)))
        .Add(x => x.AddSingleton<ISiteModelRebuilderManager, SiteModelRebuilderManager>())
        .Add(x => x.AddSingleton<Func<TransferProxyType, IS3FileTransfer>>(_ => proxyType => new S3FileTransfer(proxyType)))
        //.Add(x => x.AddSingleton<IRebuildSiteModelTAGNotifierListener, RebuildSiteModelTAGNotifierListener>())

        .Complete();
    }

    // This static array ensures that all required assemblies are included into the artifacts by the linker
    private static void EnsureAssemblyDependenciesAreLoaded()
    {
      // This static array ensures that all required assemblies are included into the artifacts by the linker
      Type[] AssemblyDependencies =
      {
        typeof(VSS.TRex.DI.DIContext),
        typeof(VSS.TRex.Logging.Logger),
        typeof(VSS.TRex.GridFabric.BaseIgniteClass),
        typeof(VSS.TRex.SiteModels.SiteModel),
        typeof(VSS.TRex.Storage.StorageProxy),
        typeof(VSS.TRex.TAGFiles.GridFabric.NodeFilters.TAGProcessorRoleBasedNodeFilter),
        typeof(VSS.TRex.SiteModelChangeMaps.GridFabric.NodeFilters.SiteModelChangeProcessorRoleBasedNodeFilter)
      };

      foreach (var asmType in AssemblyDependencies)
      {
        if (asmType.FullName == "DummyTypeName")
          Console.WriteLine($"Assembly for type {asmType} has not been loaded.");
      }
    }

    private static async Task DoServiceInitialisation(ILogger log, CancellationTokenSource cancelTokenSource)
    {
      // Start listening to TAG file processing notifications
      // DIContext.Obtain<IRebuildSiteModelTAGNotifierListener>().StartListening();

      // Register the heartbeat loggers
      DIContext.Obtain<ITRexHeartBeatLogger>().AddContext(new MemoryHeartBeatLogger());
      DIContext.Obtain<ITRexHeartBeatLogger>().AddContext(new DotnetThreadHeartBeatLogger());

      // Wait until the grid is active
      DIContext.Obtain<IActivatePersistentGridServer>().WaitUntilGridActive(TRexGrids.MutableGridName());

      // Wait until caches are available
      while (!cancelTokenSource.IsCancellationRequested)
      {
        try
        {
          if (CacheFactory(RebuildSiteModelCacheType.Metadata) != null)
            break;
        }
        catch (ArgumentException)
        {
          // Failure to find the cache will throw an argument exception, so tolerate that failure, not others
        }

        log.LogInformation($"Waiting for cache {TRexCaches.SiteModelRebuilderMetaDataCacheName()} to become available");
        await Task.Delay(1000, cancelTokenSource.Token);
      }

      while (!cancelTokenSource.IsCancellationRequested)
      {
        try
        {
          if (CacheFactory(RebuildSiteModelCacheType.KeyCollections) != null)
            break;
        }
        catch (ArgumentException)
        {
          // Failure to find the cache will throw an argument exception, so tolerate that failure, not others
        }

        log.LogInformation($"Waiting for cache {TRexCaches.SiteModelRebuilderFileKeyCollectionsCacheName()} to become available");
        await Task.Delay(1000, cancelTokenSource.Token);
      }

      DIContext.Obtain<ITRexHeartBeatLogger>().AddContext(new IgniteNodeMetricsHeartBeatLogger(DIContext.Obtain<ITRexGridFactory>().Grid(StorageMutability.Mutable)));
      DIContext.Obtain<ITRexHeartBeatLogger>().AddContext(new SiteModelRebuilderHeartbeatLogger());

      // Tell the rebuilder manager to find any active rebuilders and start them off from where they left off
      await DIContext.Obtain<ISiteModelRebuilderManager>().BeginOperations();
    }

    static async Task<int> Main()
    {
      try
      {
        Console.WriteLine($"TRex service starting at {DateTime.Now}");

        EnsureAssemblyDependenciesAreLoaded();
        DependencyInjection();

        var log = Logging.Logger.CreateLogger<Program>();

        log.LogInformation("Creating service");
        log.LogDebug("Creating service");

        var server = new MutableClientServer(new[] { ServerRoles.PROJECT_REBUILDER_ROLE });

        var cancelTokenSource = new CancellationTokenSource();
        AppDomain.CurrentDomain.ProcessExit += (s, e) =>
        {
          Console.WriteLine("Exiting");
          DIContext.Obtain<ITRexGridFactory>().StopGrids();
          cancelTokenSource.Cancel();
        };

        AppDomain.CurrentDomain.UnhandledException += TRexAppDomainUnhandledExceptionHandler.Handler;

        await DoServiceInitialisation(log, cancelTokenSource);

        await Task.Delay(-1, cancelTokenSource.Token);
        return 0;
      }
      catch (TaskCanceledException)
      {
        // Don't care as this is thrown by Task.Delay()
        Console.WriteLine("Process exit via TaskCanceledException (SIGTERM)");
        return 0;
      }
      catch (Exception e)
      {
        Console.WriteLine($"Unhandled exception: {e}");
        Console.WriteLine($"Stack trace: {e.StackTrace}");
        return -1;
      }
    }
  }
}
