﻿using System;
using System.Collections.Generic;
using KafkaConsumer.Kafka;
using log4netExtensions;
using MasterDataProxies;
using MasterDataProxies.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VSS.GenericConfiguration;
using VSS.Productivity3D.ProjectWebApiCommon.Internal;
using VSS.Productivity3D.Repo;
using VSS.VisionLink.Interfaces.Events.MasterData.Interfaces;
using VSS.VisionLink.Interfaces.Events.MasterData.Models;

namespace ExecutorTests
{
  public class ExecutorTestsBase
  {
    protected IServiceProvider serviceProvider;
    protected IConfigurationStore configStore;
    protected ILoggerFactory logger;
    protected IServiceExceptionHandler serviceExceptionHandler;
    protected ProjectRepository projectRepo;
    protected IRaptorProxy raptorProxy;
    protected Dictionary<string, string> customHeaders;
    protected IKafka producer;

    [TestInitialize]
    public virtual void InitTest()
    {
      var serviceCollection = new ServiceCollection();

      string loggerRepoName = "UnitTestLogTest";
      var logPath = System.IO.Directory.GetCurrentDirectory();
      Log4NetAspExtensions.ConfigureLog4Net(logPath, "log4nettest.xml", loggerRepoName);

      ILoggerFactory loggerFactory = new LoggerFactory();
      loggerFactory.AddDebug();
      loggerFactory.AddLog4Net(loggerRepoName);

      serviceCollection.AddLogging();
      serviceCollection.AddSingleton<ILoggerFactory>(loggerFactory)
        .AddSingleton<IConfigurationStore, GenericConfiguration>()
        .AddTransient<IRepository<IProjectEvent>, ProjectRepository>()
        .AddTransient<IServiceExceptionHandler, ServiceExceptionHandler>()
        .AddTransient<IRaptorProxy, RaptorProxy>()
        .AddSingleton<IKafka, RdKafkaDriver>(); 

      serviceProvider = serviceCollection.BuildServiceProvider();
      configStore = serviceProvider.GetRequiredService<IConfigurationStore>();
      logger = serviceProvider.GetRequiredService<ILoggerFactory>();
      serviceExceptionHandler = serviceProvider.GetRequiredService<IServiceExceptionHandler>();
      projectRepo = serviceProvider.GetRequiredService<IRepository<IProjectEvent>>() as ProjectRepository;
      customHeaders = new Dictionary<string, string>();
      producer = serviceProvider.GetRequiredService<IKafka>();
    }

    protected bool CreateProjectSettings(string projectUid, string settings)
    {
      DateTime actionUTC = new DateTime(2017, 1, 1, 2, 30, 3);

      var createProjectSettingsEvent = new UpdateProjectSettingsEvent()
      {
        ProjectUID = Guid.Parse(projectUid),
        Settings = settings,
        ActionUTC = actionUTC
      };

      projectRepo.StoreEvent(createProjectSettingsEvent).Wait();
      var g = projectRepo.GetProjectSettings(projectUid); g.Wait();
      return (g.Result != null ? true : false);
    }
  }
}
