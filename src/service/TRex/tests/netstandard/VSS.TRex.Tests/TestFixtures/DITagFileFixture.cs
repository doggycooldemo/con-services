﻿using System;
using System.IO;
using Apache.Ignite.Core;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VSS.ConfigurationStore;
using VSS.TRex.Designs;
using VSS.TRex.Designs.Interfaces;
using VSS.TRex.DI;
using VSS.TRex.Events;
using VSS.TRex.Events.Interfaces;
using VSS.TRex.GridFabric.Factories;
using VSS.TRex.GridFabric.Interfaces;
using VSS.TRex.SiteModels;
using VSS.TRex.SiteModels.Interfaces;
using VSS.TRex.SiteModels.Interfaces.Events;
using VSS.TRex.Storage;
using VSS.TRex.Storage.Caches;
using VSS.TRex.Storage.Interfaces;
using VSS.TRex.Storage.Models;
using VSS.TRex.SubGridTrees.Server;
using VSS.TRex.SubGridTrees.Server.Interfaces;
using VSS.TRex.SurveyedSurfaces;
using VSS.TRex.SurveyedSurfaces.Interfaces;
using VSS.TRex.TAGFiles.Executors;
using Xunit;

namespace VSS.TRex.Tests.TestFixtures
{
  public class DITagFileFixture : IDisposable
  {
    private static readonly object Lock = new object();

    public static Guid NewSiteModelGuid => Guid.NewGuid();

    public static TAGFileConverter ReadTAGFile(string fileName)
    {
      var converter = new TAGFileConverter();

      Assert.True(converter.Execute(new FileStream(Path.Combine("TestData", "TAGFiles", fileName), FileMode.Open, FileAccess.Read)),
        "Converter execute returned false");

      return converter;
    }

    public static TAGFileConverter ReadTAGFileFullPath(string fileName)
    {
      var converter = new TAGFileConverter();

      Assert.True(converter.Execute(new FileStream(fileName, FileMode.Open, FileAccess.Read)),
        "Converter execute returned false");

      return converter;
    }

    private static void AddProxyCacheFactoriesToDI()
    {
      DIBuilder
        .Continue()

        // Add the factories for the storage proxy caches, both standard and transacted, for spatial and non spatial caches in TRex
        .Add(x => x.AddSingleton<Func<IIgnite, StorageMutability, IStorageProxyCache<ISubGridSpatialAffinityKey, byte[]>>>
        (factory => (ignite, mutability) => null))

        .Add(x => x.AddSingleton<Func<IIgnite, StorageMutability, IStorageProxyCache<INonSpatialAffinityKey, byte[]>>>
        (factory => (ignite, mutability) => null))

        .Add(x => x.AddSingleton<Func<IIgnite, StorageMutability, IStorageProxyCacheTransacted<ISubGridSpatialAffinityKey, byte[]>>>
        (factory => (ignite, mutability) => new StorageProxyCacheTransacted_TestHarness<ISubGridSpatialAffinityKey, byte[]>(ignite?.GetCache<ISubGridSpatialAffinityKey, byte[]>(TRexCaches.SpatialCacheName(mutability)))))

        .Add(x => x.AddSingleton<Func<IIgnite, StorageMutability, IStorageProxyCacheTransacted<INonSpatialAffinityKey, byte[]>>>
        (factory => (ignite, mutability) => new StorageProxyCacheTransacted_TestHarness<INonSpatialAffinityKey, byte[]>(ignite?.GetCache<INonSpatialAffinityKey, byte[]>(TRexCaches.SpatialCacheName(mutability)))));
    }

    public DITagFileFixture()
    {
      lock (Lock)
      {
        var mockSiteModelMetadataManager = new Mock<ISiteModelMetadataManager>();
        var mockSiteModelAttributesChangedEventSender = new Mock<ISiteModelAttributesChangedEventSender>();

        DIBuilder
          .New()
          .AddLogging()
          .Add(x => x.AddSingleton<IConfigurationStore, GenericConfiguration>())

          .Add(x => x.AddSingleton<IStorageProxyFactory>(new StorageProxyFactory()))
          .Add(x => AddProxyCacheFactoriesToDI())

          .Add(x => x.AddSingleton<ISubGridSpatialAffinityKeyFactory>(new SubGridSpatialAffinityKeyFactory()))

          .Add(x => x.AddSingleton<ISiteModels>(new SiteModels.SiteModels(() => DIContext.Obtain<IStorageProxyFactory>().MutableGridStorage())))

          .Add(x => x.AddTransient<ISurveyedSurfaces>(factory => new SurveyedSurfaces.SurveyedSurfaces()))

          .Add(x => x.AddSingleton<IProductionEventsFactory>(new ProductionEventsFactory()))
          .Add(x => x.AddSingleton<IMutabilityConverter>(new MutabilityConverter()))
          .Add(x => x.AddSingleton<ISiteModelMetadataManager>(mockSiteModelMetadataManager.Object))

          .Add(x => x.AddSingleton<IDesignManager>(factory => new DesignManager()))
          .Add(x => x.AddSingleton<ISurveyedSurfaceManager>(factory => new SurveyedSurfaceManager()))

          .Add(x => x.AddSingleton<ISiteModelAttributesChangedEventSender>(mockSiteModelAttributesChangedEventSender.Object))

          .Add(x => x.AddSingleton<ISiteModelFactory>(new SiteModelFactory()))
          .Complete();
      }
    }

    public void Dispose()
    {
      DIBuilder.Continue().Eject();
    } 
  }
}
