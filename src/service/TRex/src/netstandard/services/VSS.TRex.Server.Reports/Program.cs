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
using VSS.TRex.Designs.GridFabric.Events;
using VSS.TRex.Designs.Interfaces;
using VSS.TRex.DI;
using VSS.TRex.Exports.CSV.Executors.Tasks;
using VSS.TRex.Exports.Servers.Client;
using VSS.TRex.Filters;
using VSS.TRex.Filters.Interfaces;
using VSS.TRex.GridFabric.Grids;
using VSS.TRex.Pipelines;
using VSS.TRex.Pipelines.Factories;
using VSS.TRex.Pipelines.Interfaces;
using VSS.TRex.Pipelines.Interfaces.Tasks;
using VSS.TRex.Reports.Gridded.Executors.Tasks;
using VSS.TRex.Reports.Servers.Client;
using VSS.TRex.SiteModels;
using VSS.TRex.SiteModels.Interfaces;
using VSS.TRex.Storage;
using VSS.TRex.Storage.Models;
using VSS.TRex.SubGrids;
using VSS.TRex.SubGrids.GridFabric.Arguments;
using VSS.TRex.SubGrids.Interfaces;
using VSS.TRex.SubGrids.Responses;
using VSS.TRex.SubGridTrees.Client;
using VSS.TRex.SubGridTrees.Client.Interfaces;
using VSS.TRex.SubGridTrees.Server;
using VSS.TRex.SubGridTrees.Server.Interfaces;
using VSS.TRex.SurveyedSurfaces;
using VSS.TRex.SurveyedSurfaces.Interfaces;

namespace VSS.TRex.Server.Reports
{
  class Program
  {
    private static ISubGridPipelineBase SubGridPipelineFactoryMethod(PipelineProcessorPipelineStyle key)
    {
      return key switch
      {
        PipelineProcessorPipelineStyle.DefaultProgressive => new SubGridPipelineProgressive<SubGridsRequestArgument, SubGridRequestsResponse>(),
        _ => null
      };
    }

    private static ITRexTask SubGridTaskFactoryMethod(PipelineProcessorTaskStyle key)
    {
      return key switch
      {
        PipelineProcessorTaskStyle.GriddedReport => new GriddedReportTask(),
        PipelineProcessorTaskStyle.CSVExport => new CSVExportTask(),
        _ => null
      };
    }

    private static void DependencyInjection()
    {
      DIBuilder.New()
        .AddLogging()
        .Add(x => x.AddSingleton<IConfigurationStore, GenericConfiguration>())
        .Build()
        .Add(x => x.AddSingleton<ITransferProxyFactory>(factory => new TransferProxyFactory(factory.GetRequiredService<IConfigurationStore>(), factory.GetRequiredService<ILoggerFactory>())))
        .Add(x => x.AddSingleton<ICoreXWrapper, CoreXWrapper>())
        .Add(VSS.TRex.IO.DIUtilities.AddPoolCachesToDI)
        .Add(VSS.TRex.Cells.DIUtilities.AddPoolCachesToDI)
        .Add(TRexGridFactory.AddGridFactoriesToDI)
        .Add(x => x.AddSingleton<ISubGridCellLatestPassesDataWrapperFactory>(new SubGridCellLatestPassesDataWrapperFactory()))
        .Add(Storage.Utilities.DIUtilities.AddProxyCacheFactoriesToDI)
        .Build()
        .Add(x => x.AddTransient<ISurveyedSurfaces>(factory => new SurveyedSurfaces.SurveyedSurfaces()))
        .Add(x => x.AddSingleton<ISurveyedSurfaceFactory>(new SurveyedSurfaceFactory()))
        .Build()
        .Add(x => x.AddSingleton<ISiteModels>(new SiteModels.SiteModels(StorageMutability.Immutable)))
        .Add(x => x.AddSingleton<ISiteModelFactory>(new SiteModelFactory()))
        .Add(ExistenceMaps.ExistenceMaps.AddExistenceMapFactoriesToDI)
        .Add(x => x.AddSingleton<IPipelineProcessorFactory>(new PipelineProcessorFactory()))
        .Add(x => x.AddSingleton<Func<PipelineProcessorPipelineStyle, ISubGridPipelineBase>>(provider => SubGridPipelineFactoryMethod))
        .Add(x => x.AddTransient<IRequestAnalyser>(factory => new RequestAnalyser()))
        .Add(x => x.AddSingleton<Func<PipelineProcessorTaskStyle, ITRexTask>>(provider => SubGridTaskFactoryMethod))
        .Add(x => x.AddSingleton<IClientLeafSubGridFactory>(ClientLeafSubGridFactoryFactory.CreateClientSubGridFactory()))
        .Add(x => x.AddSingleton<Func<ISubGridRequestor>>(factory => () => new SubGridRequestor()))
        .Build()

        .Add(x => x.AddSingleton(new GriddedReportRequestServer()))
        // Note: There is no need to create a second report server type instances here as this will create two Ignite JVMs...
        // Use the CSVExportRequestServer if a separate service dedicated to CSV reports is required distinct from gridded reports
        //.Add(x => x.AddSingleton(new CSVExportRequestServer()))

        .Add(x => x.AddTransient<IDesigns>(factory => new Designs.Storage.Designs()))
        .Add(x => x.AddSingleton<IDesignFiles>(new DesignFiles()))
        .Add(x => x.AddSingleton<IDesignManager>(factory => new DesignManager(StorageMutability.Immutable)))
        .Add(x => x.AddSingleton<ISurveyedSurfaceManager>(factory => new SurveyedSurfaceManager(StorageMutability.Immutable)))
        //.Add(x => x.AddSingleton<IDesignChangedEventListener>(new DesignChangedEventListener(TRexGrids.ImmutableGridName())))
        .Add(x => x.AddTransient<IAlignments>(factory => new Alignments.Alignments()))
        .Add(x => x.AddSingleton<IAlignmentManager>(factory => new AlignmentManager(StorageMutability.Immutable)))
        .Add(x => x.AddTransient<IFilterSet>(factory => new FilterSet()))
        .Add(x => x.AddSingleton<IRequestorUtilities>(new RequestorUtilities()))
        .Add(x => x.AddSingleton<ITRexHeartBeatLogger>(new TRexHeartBeatLogger()))

        .Add(x => x.AddSingleton<IPipelineListenerMapper>(new PipelineListenerMapper()))

        .Complete();
    }

    private static void EnsureAssemblyDependenciesAreLoaded()
    {
      // This static array ensures that all required assemblies are included into the artifacts by the linker
      Type[] AssemblyDependencies =
      {
        typeof(Geometry.BoundingIntegerExtent2D),
        //typeof(GriddedReportResult),
        typeof(GridFabric.BaseIgniteClass),
        typeof(SubGridsPipelinedResponseBase),
        typeof(Logging.Logger),
        typeof(DIContext),
        typeof(StorageProxy),
        typeof(SiteModel),
        typeof(Cells.CellEvents),
        typeof(Compression.AttributeValueModifiers),
        typeof(CoreXModels.LLH),
        typeof(DesignBase),
        typeof(Designs.TTM.HashOrdinate),
        typeof(Designs.TTM.Optimised.HeaderConsts),
        typeof(Events.CellPassFastEventLookerUpper),
        typeof(ExistenceMaps.ExistenceMaps),
        typeof(CellPassAttributeFilter),
        typeof(GridFabric.BaseIgniteClass),
        typeof(Machines.Machine),
        typeof(PipelineProcessor<SubGridsRequestArgument>),
        typeof(Profiling.CellLiftBuilder),
        typeof(Rendering.PlanViewTileRenderer),
        typeof(SubGrids.CutFillUtilities),
        typeof(ClientCMVLeafSubGrid),
        typeof(SubGridTrees.Core.Utilities.SubGridUtilities),
        typeof(SubGridTrees.Server.MutabilityConverter),
        typeof(VSS.TRex.SurveyedSurfaces.SurveyedSurface),
        typeof(VSS.TRex.SiteModelChangeMaps.GridFabric.Services.SiteModelChangeProcessorService)
      };

      foreach (var asmType in AssemblyDependencies)
        if (asmType.Assembly == null)
          Console.WriteLine($"Assembly for type {asmType} has not been loaded.");
    }

    private static void DoServiceInitialisation()
    {
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

        var Log = Logging.Logger.CreateLogger<Program>();

        Log.LogInformation("Creating service");
        Log.LogDebug("Creating service");

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
