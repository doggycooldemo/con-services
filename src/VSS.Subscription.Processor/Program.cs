﻿using Autofac;
using log4net;
using System;
using System.Configuration;
using System.Net;
using System.Reflection;
using Topshelf;
using Topshelf.Runtime;
using VSS.Kafka.DotNetClient.Model;
using VSS.Project.Processor.Interfaces;
using VSS.Subscription.Data;
using VSS.Subscription.Data.Interfaces;
using VSS.Subscription.Processor.Consumer;
using VSS.Subscription.Processor.Interfaces;

namespace VSS.Subscription.Processor
{
  internal class Program
  {
    private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    protected static IContainer Container { get; set; }

    public static void Main(string[] args)
    {
      TopshelfExitCode exitCode = HostFactory.Run(c =>
      {
        c.SetServiceName("_MasterDataSubscriptionConsumer");
        c.SetDisplayName("_MasterDataSubscriptionConsumer");
        c.SetDescription("Service for processing subscription payloads from Kafka");
        c.RunAsLocalSystem();
        c.StartAutomatically();
        c.EnableServiceRecovery(cfg =>
        {
          cfg.RestartService(1);
          cfg.RestartService(1);
          cfg.RestartService(1);
        });
        c.Service<ServiceController>(s =>
        {
          s.ConstructUsing(ServiceFactory);
          s.WhenStarted(o => o.Start());
          s.WhenStopped(o => o.Stop());
        });
        // c.UseLog4Net();
      });
    }


    private static ServiceController ServiceFactory(HostSettings settings)
    {
      Log.Debug("SubscriptionProcessor: starting ServiceFactory");
      
      var builder = new ContainerBuilder();
      builder.RegisterType<ServiceController>()
        .AsSelf()
        .SingleInstance();
      
      string confluentBaseUrl = ConfigurationManager.AppSettings["KafkaServerUri"]; //["RestProxyBaseUrl"];
      if (string.IsNullOrWhiteSpace(confluentBaseUrl))
        throw new ArgumentNullException("RestProxy Base Url is empty");

      string kafkaTopicName = Settings.Default.TopicName;
      string consumerGroupName = Settings.Default.ConsumerGroupName;

      builder.RegisterType<SubscriptionProcessor>().As<ISubscriptionProcessor>().SingleInstance();
      builder.RegisterType<SubscriptionEventObserver>().As<IObserver<ConsumerInstanceResponse>>().SingleInstance();
      builder.RegisterType<MySqlSubscriptionRepository>().As<ISubscriptionService>().SingleInstance();
      
      builder.RegisterType<MySqlSubscriptionRepository>().As<IProjectProcessor>().SingleInstance();

      Container = builder.Build();
      return Container.Resolve<ServiceController>();
    }
  }
}
