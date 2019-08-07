﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VSS.Common.Abstractions.Configuration;
using VSS.ConfigurationStore;
using VSS.TRex.DI;
using VSS.TRex.GridFabric.Affinity;
using VSS.TRex.GridFabric.Grids;
using VSS.TRex.Logging;
using VSS.TRex.Machines;
using VSS.TRex.SiteModels;
using VSS.TRex.SiteModels.Interfaces;
using VSS.TRex.Storage.Models;
using VSS.TRex.SubGridTrees.Server;
using VSS.TRex.Types;

namespace VSS.TRex.Tools.ProjectExtractor
{
  public class Program
  {
    private static ILogger Log;

    private static void DependencyInjection()
    {
      DIBuilder.New()
        .AddLogging()
        .Add(x => x.AddSingleton<IConfigurationStore, GenericConfiguration>())
        .Build()
        .Add(TRexGridFactory.AddGridFactoriesToDI)
        .Add(x => x.AddSingleton<ISiteModels>(new SiteModels.SiteModels()))
        .Add(x => x.AddSingleton<ISiteModelFactory>(new SiteModelFactory()))
        .Complete();
    }

    static void Main(string[] args)
    {
      if (args.Length < 2)
        Console.WriteLine("Project Extractor: Usage: <ProjectUid> <RootProjectFolder>");

      if (!Guid.TryParse(args[0], out Guid projectUid))
      {
        Log.LogError($"Project UID {args[0]} is not a valid GUID");
        Console.WriteLine($"Project UID {args[0]} is not a valid GUID");
      }

      var RootOutputFolder = args[1];
      if (!Directory.Exists(RootOutputFolder))
      {
        Log.LogError($"Output location {RootOutputFolder} does not exist");
        Console.WriteLine($"Output location {RootOutputFolder} does not exist");
      }

      var ProjectOutputFolder = Path.Combine(RootOutputFolder, projectUid.ToString());

      DependencyInjection();

      try
      {
        Log = Logger.CreateLogger<Program>();

        var siteModel = DIContext.Obtain<ISiteModels>().GetSiteModel(projectUid, false);

        if (siteModel == null)
        {
          Log.LogError($"Project {projectUid} does not exist");
          Console.WriteLine($"Project {projectUid} does not exist");
          return;
        }

        siteModel.SetStorageRepresentationToSupply(StorageMutability.Mutable);

        Console.WriteLine($"Scanning project: {projectUid}");
        Console.WriteLine($"Outputting project data to: {ProjectOutputFolder}");

        var extractor = new Extractor(siteModel, ProjectOutputFolder);

        extractor.ExtractSiteModelCoreMetaData();
        extractor.ExtractCoordinateSystem();
        extractor.ExtractMachines();
        extractor.ExtractExistenceMap();
        extractor.ExtractEventData();
        extractor.ExtractSpatialData();
        extractor.ExtractChangeMaps();

        Log.LogInformation($"Extraction of data for project {siteModel.ID} complete");
        Console.WriteLine($"Extraction of data for project {siteModel.ID} complete");
      }
      finally
      {
        DIContext.Obtain<ITRexGridFactory>()?.StopGrids();
      }
    }
  }
}
