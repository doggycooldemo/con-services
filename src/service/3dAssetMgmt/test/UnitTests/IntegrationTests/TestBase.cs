﻿using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using VSS.Serilog.Extensions;

namespace AssetMgmt.IntegrationTests
{
  public class TestBase
  {
    protected IServiceProvider ServiceProvider;


    protected ILoggerFactory Log => ServiceProvider.GetService<ILoggerFactory>();

    public TestBase()
    {
      ServiceProvider = new ServiceCollection()
                        .AddLogging()
                        .AddSingleton(new LoggerFactory().AddSerilog(SerilogExtensions.Configure("AssetMgmt.IntegrationTests.log")))
                        .BuildServiceProvider();
    }
  }
}
