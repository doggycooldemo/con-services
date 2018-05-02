﻿using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VSS.Log4Net.Extensions;
#if NET_4_7
using Topshelf;
using System.Diagnostics;
#endif

namespace VSS.Productivity3D.Scheduler.WebApi
{
  /// <summary>
  /// VSS.Productivity3D.Scheduler program
  /// </summary>
  public class Program
  {
    /// <summary>
    /// VSS.Productivity3D.Filter main
    /// </summary>
    public static void Main(string[] args)
    {
#if  NET_4_7
      var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
      var pathToContentRoot = Path.GetDirectoryName(pathToExe);
#endif

      var kestrelConfig = new ConfigurationBuilder()
#if NET_4_7
        .SetBasePath(pathToContentRoot)
#endif
        .AddJsonFile("kestrelsettings.json", optional: true, reloadOnChange: false)
        .Build();

#if NET_4_7 //To run the service use https://docs.microsoft.com/en-us/aspnet/core/hosting/windows-service
      HostFactory.Run(x =>
      {
        x.Service<FilterContainer>(s =>
        {
          s.ConstructUsing(name => new FilterContainer());
          s.WhenStarted(tc => tc.Start(kestrelConfig));
          s.WhenStopped(tc => tc.Stop());
        });
        x.RunAsLocalSystem();

        x.SetDescription("Filter Scheduler WebAPI, containing various controllers. NET 4.7 port.");
        x.SetDisplayName("FilterSchedulerWebAPINet47");
        x.SetServiceName("FilterSchedulerWebAPINet47");
        x.EnableServiceRecovery(c =>
        {
          c.RestartService(1);
          c.OnCrashOnly();
        });
      });
#else
      var host = new WebHostBuilder()
        .UseKestrel()
        .UseContentRoot(Directory.GetCurrentDirectory())
        .ConfigureLogging(builder =>
        {
          Log4NetProvider.RepoName = Startup.LOGGER_REPO_NAME;
          builder.Services.AddSingleton<ILoggerProvider, Log4NetProvider>();
          builder.SetMinimumLevel(LogLevel.Trace);
        })
        .UseIISIntegration()
        .UseStartup<Startup>()
        .Build();

      host.Run();
#endif
    }
  }

#if NET_4_7
  internal class FilterContainer
  {
    private IWebHost _webHost;

    /// <summary>
    /// 
    /// </summary>
    public void Start(IConfiguration config)
    {
      var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
      var pathToContentRoot = Path.GetDirectoryName(pathToExe);

      _webHost = new WebHostBuilder()
        .UseKestrel()
        .UseConfiguration(config)
        //TODO For some reason setting configuration for a topshelf service does not work
        .UseUrls(config["server.urls"])
        .UseContentRoot(pathToContentRoot)
        .ConfigureLogging(builder =>
        {
          Log4NetProvider.RepoName = Startup.LOGGER_REPO_NAME;
          builder.Services.AddSingleton<ILoggerProvider, Log4NetProvider>();
          builder.SetMinimumLevel(LogLevel.Trace);
        })
        .UseStartup<Startup>()
        .Build();

      _webHost.Start();
    }

    /// <summary>
    /// 
    /// </summary>
    public void Stop()
    {
      _webHost?.Dispose();
    }
  }
#endif
}