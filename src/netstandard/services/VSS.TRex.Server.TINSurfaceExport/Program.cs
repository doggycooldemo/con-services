﻿using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VSS.TRex.DI;
using VSS.TRex.ExistenceMaps.Interfaces;
using VSS.TRex.Exports.Servers.Client;
using VSS.TRex.Services.Designs;
using VSS.TRex.SiteModels.Interfaces;
using VSS.TRex.Storage;
using VSS.TRex.Storage.Interfaces;
using VSS.TRex.Storage.Models;
using VSS.TRex.SurveyedSurfaces;
using VSS.TRex.SurveyedSurfaces.Interfaces;

//[assembly: ForceAssemblyReference(typeof(VSS.TRex.Geometry.BoundingIntegerExtent2D))]

namespace VSS.TRex.Server.TINSurfaceExport
{
  class Program
  {
    private static void DependencyInjection()
    {
        DIBuilder.New()
        .AddLogging()
        .Add(x => x.AddSingleton<IStorageProxyFactory>(new StorageProxyFactory()))
        .Add(x => x.AddTransient<ISurveyedSurfaces>(factory => new SurveyedSurfaces.SurveyedSurfaces()))
        .Add(x => x.AddSingleton<ISurveyedSurfaceFactory>(new SurveyedSurfaceFactory()))
        .Build()
        .Add(x => x.AddSingleton<ISiteModels>(new SiteModels.SiteModels()))
        .Add(x => x.AddSingleton<IDesignsService>(new DesignsService(StorageMutability.Immutable)))
        .Add(x => x.AddSingleton<IExistenceMaps>(new ExistenceMaps.ExistenceMaps()))
        .Complete();
    }

    private static void EnsureAssemblyDependenciesAreLoaded(ILogger Log)
    {
      // This static array ensures that all required assemblies are included into the artifacts by the linker
      Type[] AssemblyDependencies =
      {
        typeof(VSS.TRex.Geometry.BoundingIntegerExtent2D),
        typeof(VSS.TRex.Exports.Patches.PatchResult),
        typeof(VSS.TRex.GridFabric.BaseIgniteClass),
        typeof(VSS.TRex.Common.SubGridsPipelinedReponseBase),
        typeof(VSS.TRex.Logging.Logger),
        typeof(VSS.TRex.DI.DIContext),
        typeof(VSS.TRex.Storage.StorageProxy),
        typeof(VSS.TRex.SiteModels.SiteModel),
        typeof(VSS.TRex.Cells.CellEvents),
        typeof(VSS.TRex.Compression.AttributeValueModifiers),
        typeof(VSS.TRex.CoordinateSystems.LLH),
        typeof(VSS.TRex.Designs.DesignBase),
        typeof(VSS.TRex.Designs.TTM.HashOrdinate),
        typeof(VSS.TRex.Designs.TTM.Optimised.HeaderConsts),
        typeof(VSS.TRex.Events.CellPassFastEventLookerUpper),
        typeof(VSS.TRex.ExistenceMaps.ExistenceMaps),
        typeof(VSS.TRex.Filters.CellPassAttributeFilter),
        typeof(VSS.TRex.GridFabric.BaseIgniteClass),
        typeof(VSS.TRex.Machines.Machine),
        typeof(VSS.TRex.Pipelines.PipelineProcessor),
        typeof(VSS.TRex.Profiling.CellLiftBuilder),
        typeof(VSS.TRex.Rendering.PlanViewTileRenderer),
        typeof(VSS.TRex.Services.Designs.DesignsService),
        typeof(VSS.TRex.Services.SurveyedSurfaces.SurveyedSurfaceService),
        typeof(VSS.TRex.SubGrids.CutFillUtilities),
        typeof(VSS.TRex.SubGridTrees.Client.ClientCMVLeafSubGrid),
        typeof(VSS.TRex.SubGridTrees.Core.Utilities.SubGridUtilities),
        typeof(VSS.TRex.SubGridTrees.Server.MutabilityConverter),
        typeof(VSS.TRex.SurveyedSurfaces.SurveyedSurface)
      };

      foreach (var asmType in AssemblyDependencies)
        if (asmType.Assembly == null)
          Log.LogError($"Assembly for type {asmType} has not been loaded.");
    }

    static void Main(string[] args)
    {
      DependencyInjection();

      ILogger Log = Logging.Logger.CreateLogger<Program>();

      EnsureAssemblyDependenciesAreLoaded(Log);

      Log.LogInformation("Creating service");
      Log.LogDebug("Creating service");

      var server = new TINSurfaceExportRequestServer();
      Console.WriteLine("Press anykey to exit");
      Console.ReadLine();
    }
  }
}
