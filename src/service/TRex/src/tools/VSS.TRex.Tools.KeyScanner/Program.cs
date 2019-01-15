﻿using System;
using Microsoft.Extensions.DependencyInjection;
using VSS.ConfigurationStore;
using VSS.TRex.DI;
using VSS.TRex.GridFabric.Grids;
using VSS.TRex.GridFabric.Servers.Client;
using VSS.TRex.Storage;
using VSS.TRex.Storage.Interfaces;
using VSS.TRex.Storage.Models;

namespace VSS.TRex.Tools.KeyScanner
{
  class Program
  {
    static void Main(string[] args)
    {
      DIBuilder
        .New()
        .AddLogging()
        .Add(x => x.AddSingleton<IConfigurationStore, GenericConfiguration>())
        .Add(x => x.AddSingleton<ITRexGridFactory>(new TRexGridFactory()))
        .Add(x => x.AddSingleton<IStorageProxyFactory>(new StorageProxyFactory()))
        .Add(VSS.TRex.Storage.Utilities.DIUtilities.AddProxyCacheFactoriesToDI)
        .Build()
        .Add(x => x.AddSingleton(new ImmutableClientServer("Webtools-Immutable")))
        .Add(x => x.AddSingleton(new MutableClientServer("Webtools-Mutable")))
        .Build()
        .Complete();

      var ks = new KeyScanner();

      Console.WriteLine("Starting to dump mutable grid keys");
      Console.WriteLine("##################################");

      ks.dumpKeysToFile(StorageMutability.Mutable, @"C:\Temp\AllTRexIgniteCacheKeys = mutable.txt");
      Console.WriteLine("----> Mutable grid key dump complete");

      Console.WriteLine("Starting to dump immutable grid keys");
      Console.WriteLine("####################################");
      ks.dumpKeysToFile(StorageMutability.Immutable, @"C:\Temp\AllTRexIgniteCacheKeys = immutable.txt");
      Console.WriteLine("----> Immutable grid key dump complete");

      Console.WriteLine("Completed dump of grid keys");
      Console.WriteLine("###########################");
    }
  }
}
