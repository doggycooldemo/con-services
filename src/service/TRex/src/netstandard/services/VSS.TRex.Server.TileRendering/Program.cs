﻿using System;
using System.Threading;
using System.Threading.Tasks;
using CoreX.Interfaces;
using CoreX.Wrapper;
using Microsoft.Extensions.DependencyInjection;
using VSS.Common.Abstractions.Configuration;
using VSS.ConfigurationStore;
using VSS.MasterData.Proxies;
using VSS.MasterData.Proxies.Interfaces;
using VSS.Productivity3D.Models.Enums;
using VSS.Tpaas.Client.Clients;
using VSS.Tpaas.Client.RequestHandlers;
using VSS.TRex.Common;
using VSS.TRex.Common.Exceptions;
using VSS.TRex.Common.HeartbeatLoggers;
using VSS.TRex.Common.Interfaces;
using VSS.TRex.DataSmoothing;
using VSS.TRex.Designs;
using VSS.TRex.Designs.GridFabric.Events;
using VSS.TRex.Designs.Interfaces;
using VSS.TRex.DI;
using VSS.TRex.Filters;
using VSS.TRex.Filters.Interfaces;
using VSS.TRex.GridFabric.Grids;
using VSS.TRex.HttpClients;
using VSS.TRex.Pipelines;
using VSS.TRex.Pipelines.Factories;
using VSS.TRex.Pipelines.Interfaces;
using VSS.TRex.Pipelines.Interfaces.Tasks;
using VSS.TRex.Rendering.Executors.Tasks;
using VSS.TRex.Rendering.Servers.Client;
using VSS.TRex.SiteModels;
using VSS.TRex.SiteModels.GridFabric.Events;
using VSS.TRex.SiteModels.Interfaces;
using VSS.TRex.SiteModels.Interfaces.Events;
using VSS.TRex.Storage.Models;
using VSS.TRex.SubGrids.GridFabric.Arguments;
using VSS.TRex.SubGrids.Responses;
using VSS.TRex.SubGridTrees.Client;
using VSS.TRex.SubGridTrees.Client.Interfaces;
using VSS.TRex.SurveyedSurfaces;
using VSS.TRex.SurveyedSurfaces.Interfaces;
using VSS.TRex.Alignments;
using VSS.TRex.Alignments.Interfaces;

namespace VSS.TRex.Server.TileRendering
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
        PipelineProcessorTaskStyle.PVMRendering => new PVMRenderingTask(),
        _ => null
      };
    }

    private static IDataSmoother TileRenderingSmootherFactoryMethod(DisplayMode key)
    {
      var config = DIContext.Obtain<IConfigurationStore>();

      var smoothingActive = config.GetValueBool("TILE_RENDERING_DATA_SMOOTHING_ACTIVE", DataSmoothing.Consts.TILE_RENDERING_DATA_SMOOTHING_ACTIVE);

      if (!smoothingActive)
      {
        return null;
      }

      var convolutionMaskSize = (ConvolutionMaskSize)config.GetValueInt("TILE_RENDERING_DATA_SMOOTHING_MASK_SIZE", (int)DataSmoothing.Consts.TILE_RENDERING_DATA_SMOOTHING_MASK_SIZE);
      var nullInfillMode = (NullInfillMode)config.GetValueInt("TILE_RENDERING_DATA_SMOOTHING_NULL_INFILL_MODE", (int)DataSmoothing.Consts.TILE_RENDERING_DATA_SMOOTHING_NULL_INFILL_MODE);

      return key switch
      {
        DisplayMode.Height => new ElevationArraySmoother(new ConvolutionTools<float>(), convolutionMaskSize, nullInfillMode),
        _ => null
      };
    }

    private static void DependencyInjection()
    {
      DIBuilder
        .New()
        .AddLogging()
        .Add(x => x.AddSingleton<IConfigurationStore, GenericConfiguration>())
        .Build()
        .Add(x => x.AddSingleton<ICoreXWrapper, CoreXWrapper>())
        .Add(VSS.TRex.IO.DIUtilities.AddPoolCachesToDI)
        .Add(VSS.TRex.Cells.DIUtilities.AddPoolCachesToDI)
        .Add(TRexGridFactory.AddGridFactoriesToDI)
        .Add(VSS.TRex.Storage.Utilities.DIUtilities.AddProxyCacheFactoriesToDI)
        .Build()
        .Add(x => x.AddTransient<ISurveyedSurfaces>(factory => new SurveyedSurfaces.SurveyedSurfaces()))
        .Add(x => x.AddSingleton<ISurveyedSurfaceFactory>(new SurveyedSurfaceFactory()))
        .Build()
        .Add(x => x.AddSingleton<ISiteModels>(new SiteModels.SiteModels(StorageMutability.Immutable)))
        .Add(x => x.AddSingleton<ISiteModelFactory>(new SiteModelFactory()))

        .Add(TRex.ExistenceMaps.ExistenceMaps.AddExistenceMapFactoriesToDI)

        .Add(x => x.AddSingleton<IPipelineProcessorFactory>(new PipelineProcessorFactory()))
        .Add(x => x.AddSingleton<Func<PipelineProcessorPipelineStyle, ISubGridPipelineBase>>(provider => SubGridPipelineFactoryMethod))
        .Add(x => x.AddTransient<IRequestAnalyser>(factory => new RequestAnalyser()))
        .Add(x => x.AddSingleton<Func<PipelineProcessorTaskStyle, ITRexTask>>(provider => SubGridTaskFactoryMethod))
        .Add(x => x.AddSingleton<IClientLeafSubGridFactory>(ClientLeafSubGridFactoryFactory.CreateClientSubGridFactory()))
        .Build()
        .Add(x => x.AddSingleton(new TileRenderingServer()))
        .Add(x => x.AddTransient<IDesigns>(factory => new Designs.Storage.Designs()))
        .Add(x => x.AddSingleton<IDesignManager>(factory => new DesignManager(StorageMutability.Immutable)))
        //.Add(x => x.AddSingleton<IDesignChangedEventListener>(new DesignChangedEventListener(TRexGrids.ImmutableGridName())))
        .Add(x => x.AddSingleton<ISurveyedSurfaceManager>(factory => new SurveyedSurfaceManager(StorageMutability.Immutable)))
        .Add(x => x.AddTransient<IAlignments>(factory => new Alignments.Alignments()))
        .Add(x => x.AddSingleton<IAlignmentManager>(factory => new AlignmentManager(StorageMutability.Immutable)))

        // Register the listener for site model attribute change notifications
        //.Add(x => x.AddSingleton<ISiteModelAttributesChangedEventListener>(new SiteModelAttributesChangedEventListener(TRexGrids.ImmutableGridName())))

        .Add(x => x.AddSingleton<ITRexHeartBeatLogger>(new TRexHeartBeatLogger()))

        // Setup dependencies for communication with the Trimble Coordinate Service.
        .Add(x => x.AddSingleton<ITPaasProxy, TPaasProxy>())
        .Add(x => x.AddTransient<TRexTPaaSAuthenticatedRequestHandler>())
        .Add(x => x.AddTransient<TPaaSApplicationCredentialsRequestHandler>())

        .AddHttpClient<TPaaSClient>(client => client.BaseAddress = new Uri("https://identity-stg.trimble.com/i/oauth2/token"))
          .AddHttpMessageHandler<TPaaSApplicationCredentialsRequestHandler>()

        .Add(x => x.AddTransient<IFilterSet>(factory => new FilterSet()))

        .Add(x => x.AddSingleton<Func<DisplayMode, IDataSmoother>>(provider => TileRenderingSmootherFactoryMethod))

        .Add(x => x.AddSingleton<IPipelineListenerMapper>(new PipelineListenerMapper()))

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
        typeof(VSS.TRex.Storage.StorageProxy),
        typeof(VSS.TRex.SiteModels.SiteModel),
        typeof(VSS.TRex.Cells.CellEvents),
        typeof(CoreXModels.LLH),
        typeof(VSS.TRex.ExistenceMaps.ExistenceMaps),
        typeof(VSS.TRex.Filters.CellPassAttributeFilter),
        typeof(VSS.TRex.GridFabric.BaseIgniteClass),
        typeof(VSS.TRex.Machines.Machine),
        typeof(VSS.TRex.Pipelines.PipelineProcessor<SubGridsRequestArgument>),
        typeof(VSS.TRex.Rendering.PlanViewTileRenderer),
        typeof(VSS.TRex.SubGrids.CutFillUtilities),
        typeof(VSS.TRex.SubGridTrees.Client.ClientCMVLeafSubGrid),
        typeof(VSS.TRex.SubGridTrees.Core.Utilities.SubGridUtilities),
        typeof(VSS.TRex.SurveyedSurfaces.SurveyedSurface),
        typeof(VSS.TRex.Rendering.GridFabric.Responses.TileRenderResponse),
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
