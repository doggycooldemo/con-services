﻿using System;
using Microsoft.Extensions.DependencyInjection;
using VSS.ConfigurationStore;
using VSS.TRex.DI;
using VSS.TRex.Rendering.Abstractions;
using VSS.TRex.Rendering.Implementations.Core2;
using VSS.TRex.SubGridTrees.Client;

namespace VSS.TRex.Tests.TestFixtures
{
  public class DILoggingFixture : IDisposable
  {
    private static object Lock = new object();

    public DILoggingFixture()
    {
      lock (Lock)
      {
        DIBuilder
          .New()
          .AddLogging()
          .Add(x => x.AddSingleton<IConfigurationStore, GenericConfiguration>())
          .Add(x => x.AddSingleton(ClientLeafSubgridFactoryFactory.CreateClientSubGridFactory()))
          .Add(x => x.AddSingleton<IRenderingFactory>(new RenderingFactory()))
          .Complete();
      }
    }

    public void Dispose()
    {
    } // Nothing needing doing 
  }
}
