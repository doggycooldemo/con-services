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
using VSS.TRex.Alignments;
using VSS.TRex.Alignments.Interfaces;
using VSS.TRex.Common;
using VSS.TRex.Common.Exceptions;
using VSS.TRex.Common.HeartbeatLoggers;
using VSS.TRex.Common.Interfaces;
using VSS.TRex.Designs;
using VSS.TRex.Designs.Factories;
using VSS.TRex.Designs.GridFabric.Events;
using VSS.TRex.Designs.Interfaces;
using VSS.TRex.Designs.Servers.Client;
using VSS.TRex.DI;
using VSS.TRex.Events;
using VSS.TRex.Events.Interfaces;
using VSS.TRex.Filters;
using VSS.TRex.Filters.Interfaces;
using VSS.TRex.GridFabric.Grids;
using VSS.TRex.SiteModels;
using VSS.TRex.SiteModels.GridFabric.Events;
using VSS.TRex.SiteModels.Interfaces;
using VSS.TRex.SiteModels.Interfaces.Events;
using VSS.TRex.Storage.Models;
using VSS.TRex.SubGridTrees.Client;
using VSS.TRex.SubGridTrees.Client.Interfaces;
using VSS.TRex.SurveyedSurfaces;
using VSS.TRex.SurveyedSurfaces.Interfaces;

namespace VSS.TRex.Server.DesignElevation
{
  class Program
  {
    private static void DependencyInjection()
    {
      DIBuilder
        .New()
        .AddLogging()
        .Add(x => x.AddSingleton<IConfigurationStore, GenericConfiguration>())
        .Build()
        .Add(x => x.AddSingleton<ITransferProxyFactory>(factory => new TransferProxyFactory(factory.GetRequiredService<IConfigurationStore>(), factory.GetRequiredService<ILoggerFactory>())))
        .Add(x => x.AddSingleton<ICoreXWrapper, CoreXWrapper>())
        .Add(VSS.TRex.IO.DIUtilities.AddPoolCachesToDI)
        .Add(VSS.TRex.Cells.DIUtilities.AddPoolCachesToDI)
        .Add(TRexGridFactory.AddGridFactoriesToDI)
        .Add(VSS.TRex.Storage.Utilities.DIUtilities.AddProxyCacheFactoriesToDI)
        .Build()
        .Add(x => x.AddTransient<IFilterSet>(factory => new FilterSet()))
        .Add(x => x.AddSingleton<ISiteModels>(new SiteModels.SiteModels(StorageMutability.Immutable)))
        .Add(x => x.AddSingleton<ISiteModelFactory>(new SiteModelFactory()))
        .Add(ExistenceMaps.ExistenceMaps.AddExistenceMapFactoriesToDI)
        .Add(x => x.AddSingleton<IProductionEventsFactory>(new ProductionEventsFactory()))
        .Build()
        .Add(x => x.AddSingleton(new CalculateDesignElevationsServer()))
        .Add(X => X.AddTransient<IDesigns>(factory => new Designs.Storage.Designs()))
        .Add(x => x.AddSingleton<IDesignFiles>(new DesignFiles()))
        .Add(x => x.AddSingleton<IDesignManager>(factory => new DesignManager(StorageMutability.Immutable)))
        .Add(x => x.AddTransient<ISurveyedSurfaces>(factory => new SurveyedSurfaces.SurveyedSurfaces()))
        .Add(x => x.AddSingleton<ISurveyedSurfaceManager>(factory => new SurveyedSurfaceManager(StorageMutability.Immutable)))
        .Add(x => x.AddTransient<IAlignments>(factory => new Alignments.Alignments()))
        .Add(x => x.AddSingleton<IAlignmentManager>(factory => new AlignmentManager(StorageMutability.Immutable)))
        //.Add(x => x.AddSingleton<IDesignChangedEventListener>(new DesignChangedEventListener(TRexGrids.ImmutableGridName())))
        .Add(x => x.AddSingleton<ITRexHeartBeatLogger>(new TRexHeartBeatLogger()))
        .Add(x => x.AddSingleton<IOptimisedTTMProfilerFactory>(new OptimisedTTMProfilerFactory()))
        .Add(x => x.AddSingleton<IDesignClassFactory>(new DesignClassFactory()))
        .Add(x => x.AddSingleton<IClientLeafSubGridFactory>(ClientLeafSubGridFactoryFactory.CreateClientSubGridFactory()))

        // Register the listener for site model attribute change notifications
        //.Add(x => x.AddSingleton<ISiteModelAttributesChangedEventListener>(new SiteModelAttributesChangedEventListener(TRexGrids.ImmutableGridName())))

        .Complete();
    }

    // This static array ensures that all required assemblies are included into the artifacts by the linker
    private static void EnsureAssemblyDependenciesAreLoaded()
    {
      // This static array ensures that all required assemblies are included into the artifacts by the linker
      Type[] AssemblyDependencies =
      {
        typeof(VSS.TRex.Geometry.BoundingIntegerExtent2D),
        typeof(VSS.TRex.GridFabric.BaseIgniteClass),
        typeof(VSS.TRex.Common.SubGridsPipelinedResponseBase),
        typeof(VSS.TRex.Logging.Logger),
        typeof(VSS.TRex.DI.DIContext),
        typeof(VSS.TRex.Cells.CellEvents),
        typeof(VSS.TRex.Designs.DesignBase),
        typeof(VSS.TRex.Designs.TTM.HashOrdinate),
        typeof(VSS.TRex.Designs.TTM.Optimised.HeaderConsts),
        typeof(VSS.TRex.SurveyedSurfaces.SurveyedSurfaces),
        typeof(VSS.TRex.ExistenceMaps.ExistenceMaps),
        typeof(VSS.TRex.GridFabric.BaseIgniteClass),
        typeof(VSS.TRex.SubGridTrees.Client.ClientCMVLeafSubGrid),
        typeof(VSS.TRex.SubGridTrees.Core.Utilities.SubGridUtilities),
        typeof(VSS.TRex.SiteModelChangeMaps.GridFabric.Services.SiteModelChangeProcessorService)
      };

      foreach (var asmType in AssemblyDependencies)
        if (asmType.Assembly == null)
          Console.WriteLine($"Assembly for type {asmType} has not been loaded.");
    }

    private static void DoServiceInitialisation()
    {
      // Start listening to site model change notifications
      //DIContext.Obtain<ISiteModelAttributesChangedEventListener>().StartListening();

      // Start listening to design state change notifications
      //DIContext.Obtain<IDesignChangedEventListener>().StartListening();

      // Register the heartbeat loggers
      DIContext.Obtain<ITRexHeartBeatLogger>().AddContext(new MemoryHeartBeatLogger());
      DIContext.Obtain<ITRexHeartBeatLogger>().AddContext(new DotnetThreadHeartBeatLogger());
      DIContext.Obtain<ITRexHeartBeatLogger>().AddContext(new IgniteNodeMetricsHeartBeatLogger(DIContext.Obtain<ITRexGridFactory>().Grid(StorageMutability.Immutable)));
    }

    static async Task<int> Main(string[] args)
    {
      try
      {
        Console.WriteLine($"TRex service starting at {DateTime.Now}");

        EnsureAssemblyDependenciesAreLoaded();
        DependencyInjection();

        var cancelTokenSource = new CancellationTokenSource();
        AppDomain.CurrentDomain.ProcessExit += (s, e) =>
        {
          Console.WriteLine("Exiting");
          DIContext.Obtain<ITRexGridFactory>().StopGrids();
          cancelTokenSource.Cancel();
        };

        AppDomain.CurrentDomain.UnhandledException += TRexAppDomainUnhandledExceptionHandler.Handler;

        DoServiceInitialisation();

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
