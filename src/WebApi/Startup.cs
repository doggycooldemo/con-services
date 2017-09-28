﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.Swagger.Model;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using VSS.Log4Net.Extensions;
using VSS.MasterData.Models.FIlters;
using VSS.Productivity3D.Common.Extensions;
using VSS.Productivity3D.Common.Filters;
using VSS.Productivity3D.Common.Filters.Authentication;
using VSS.Productivity3D.Common.Interfaces;
using VSS.Productivity3D.WebApiModels.Compaction.Helpers;
using ValidationFilterAttribute = VSS.Productivity3D.Common.Filters.Validation.ValidationFilterAttribute;

namespace VSS.Productivity3D.WebApi
{
  /// <summary>
  /// 
  /// </summary>
  public partial class Startup
  {
    private const string loggerRepoName = "WebApi";
    private readonly bool isDevEnv;
    private IServiceCollection serviceCollection;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="env"></param>
    public Startup(IHostingEnvironment env)
    {
      var builder = new ConfigurationBuilder()
          .SetBasePath(env.ContentRootPath)
          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

      env.ConfigureLog4Net("log4net.xml", loggerRepoName);

      isDevEnv = env.IsEnvironment("Development");
  
      builder.AddEnvironmentVariables();
      Configuration = builder.Build();

      AutoMapperUtility.AutomapperConfiguration.AssertConfigurationIsValid();
    }

    /// <summary>
    /// 
    /// </summary>
    public IConfigurationRoot Configuration { get; }

    /// <summary>
    /// This method gets called by the runtime. Use this method to add services to the container.
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureServices(IServiceCollection services)
    {
      //Configure CORS
      services.AddCors(options =>
      {
        options.AddPolicy("VSS", builder => builder.AllowAnyOrigin()
                  .WithHeaders("Origin", "X-Requested-With", "Content-Type", "Accept", "Authorization",
                      "X-VisionLink-CustomerUid", "X-VisionLink-UserUid", "Cache-Control", "X-VisionLink-ClearCache")
                  .WithMethods("OPTIONS", "TRACE", "GET", "HEAD", "POST", "PUT", "DELETE"));
      });
      services.AddResponseCompression();
      // Add framework services.
      services.AddMemoryCache();

      services.AddCustomResponseCaching();

      services.AddMvc(
          config =>
          {
            config.Filters.Add(new ValidationFilterAttribute());
          });

      //Configure swagger
      services.AddSwaggerGen();

      services.ConfigureSwaggerGen(options =>
      {
        options.SingleApiVersion(new Info
        {
          Version = "v1",
          Title = "Raptor API",
          Description = "API for 3D compaction and volume data",
          TermsOfService = "None"
        });

        string pathToXml;

        var moduleName = typeof(Startup).GetTypeInfo().Assembly.ManifestModule.Name;
        var assemblyName = moduleName.Substring(0, moduleName.LastIndexOf('.'));

        if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), assemblyName + ".xml")))
          pathToXml = Directory.GetCurrentDirectory();
        else if (File.Exists(Path.Combine(AppContext.BaseDirectory, assemblyName + ".xml")))
          pathToXml = AppContext.BaseDirectory;
        else
        {
          var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
          pathToXml = Path.GetDirectoryName(pathToExe);
        }
        options.IncludeXmlComments(Path.Combine(pathToXml, assemblyName + ".xml"));

        options.IgnoreObsoleteProperties();
        options.DescribeAllEnumsAsStrings();
      });
      //Swagger documentation can be viewed with http://localhost:5000/swagger/ui/index.html   

      ConfigureApplicationServices(services);
    }

    /// <summary>
    /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    /// </summary>
    /// <param name="app"></param>
    /// <param name="env"></param>
    /// <param name="loggerFactory"></param>
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
      loggerFactory.AddConsole(Configuration.GetSection("Logging"));
      loggerFactory.AddDebug();
      loggerFactory.AddLog4Net(loggerRepoName);

      serviceCollection.AddSingleton(loggerFactory);
      var serviceProvider = serviceCollection.BuildServiceProvider();
      app.UseFilterMiddleware<ExceptionsTrap>();

      //Enable CORS before TID so OPTIONS works without authentication
      app.UseCors("VSS");

      app.UseFilterMiddleware<TIDAuthentication>();
      
      //Add stats
      if (Configuration["newrelic"] == "true")
        app.UseFilterMiddleware<NewRelicMiddleware>();

      app.UseResponseCompression();

      app.UseResponseCaching();
      app.UseMvc();

      app.UseSwagger();
      app.UseSwaggerUi();

      //Check if the configuration is correct and we are able to connect to Raptor
      var log = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
      log.LogInformation("Testing Raptor configuration with sending config request");
      try
      {
        serviceProvider.GetRequiredService<IASNodeClient>().RequestConfig(out string config);
        log.LogTrace("Received config {0}", config);
        if (config.Contains("Error retrieving Raptor config"))
          throw new Exception(config);
      }
      catch (Exception e)
      {
        log.LogError("Exception loading config: {0} at {1}", e.Message, e.StackTrace);
        log.LogCritical("Can't talk to Raptor for some reason - check configuration");
        Environment.Exit(138);
      }
    }
  }
}