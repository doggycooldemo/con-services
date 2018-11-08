﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VSS.Log4Net.Extensions;
using VSS.TRex.Gateway.Common.Converters;
using VSS.WebApi.Common;

namespace VSS.TRex.Gateway.WebApi
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var webHost = BuildWebHost(args);

      webHost.Run();
    }
    
    public static IWebHost BuildWebHost(string[] args)
    {
      var kestrelConfig = new ConfigurationBuilder()
        .AddJsonFile("kestrelsettings.json", true, false).Build();

      AutoMapperUtility.AutomapperConfiguration.AssertConfigurationIsValid();

      return WebHost.CreateDefaultBuilder(args)
        .ConfigureLogging(builder =>
        {
          Log4NetProvider.RepoName = Startup.LOGGER_REPO_NAME;
          builder.Services.AddSingleton<ILoggerProvider, Log4NetProvider>();
          builder.SetMinimumLevel(LogLevel.Trace);
          builder.AddConfiguration(kestrelConfig);
        })
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
          var env = hostingContext.HostingEnvironment;
          env.ConfigureLog4Net(repoName: Startup.LOGGER_REPO_NAME, configFileRelativePath: "log4net.xml");

        })
        .UsePrometheus()
        .UseStartup<Startup>()
        .Build();
    }
  }
}
