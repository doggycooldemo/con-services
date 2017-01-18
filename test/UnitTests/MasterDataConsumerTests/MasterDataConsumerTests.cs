﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using VSS.Project.Service.Interfaces;
using VSS.Project.Service.Utils.Kafka;
using KafkaConsumer;
using VSS.VisionLink.Interfaces.Events.MasterData.Interfaces;
using VSS.Project.Service.Repositories;
using VSS.Project.Data;
using VSS.Customer.Data;
using VSS.Project.Service.Utils;
using System;
using VSS.Geofence.Data;

namespace MasterDataConsumer.Tests
{

  [TestClass]
  public class MasterDataConsumerTests
  {
    IServiceProvider serviceProvider = null;

    [TestInitialize]
    public void InitTest()
    {
      serviceProvider = new ServiceCollection()
          .AddTransient<IKafka, RdKafkaDriver>()
          .AddTransient<IKafkaConsumer<ISubscriptionEvent>, KafkaConsumer<ISubscriptionEvent>>()
          .AddTransient<IKafkaConsumer<IProjectEvent>, KafkaConsumer<IProjectEvent>>()
          .AddTransient<IKafkaConsumer<ICustomerEvent>, KafkaConsumer<ICustomerEvent>>()
          .AddTransient<IKafkaConsumer<IGeofenceEvent>, KafkaConsumer<IGeofenceEvent>>()
          .AddTransient<IMessageTypeResolver, MessageResolver>()
          .AddTransient<IRepositoryFactory, RepositoryFactory>()
          .AddTransient<IRepository<ISubscriptionEvent>, SubscriptionRepository>()
          .AddTransient<IRepository<IProjectEvent>, ProjectRepository>()
          .AddTransient<IRepository<ICustomerEvent>, CustomerRepository>()
          .AddTransient<IRepository<IGeofenceEvent>, GeofenceRepository>()
          .AddSingleton<IConfigurationStore, GenericConfiguration>()
          .BuildServiceProvider();
    }

    [TestMethod]
    public void CanCreateCustomerEventConsumer()
    {
      CreateCollection();

      var customerConsumer = serviceProvider.GetService<IKafkaConsumer<ICustomerEvent>>();
      Assert.IsNotNull(customerConsumer);

      customerConsumer.SetTopic("VSS.Interfaces.Events.MasterData.ICustomerEvent");
      var customerReturn = customerConsumer.StartProcessingAsync(new CancellationTokenSource());      
      Assert.IsNotNull(customerReturn);

      CleanCollection();
    }
        

    private void CreateCollection()
    {
      var serviceProvider = new ServiceCollection()
          .AddTransient<IKafka, RdKafkaDriver>()
          .AddTransient<IKafkaConsumer<ISubscriptionEvent>, KafkaConsumer<ISubscriptionEvent>>()
          .AddTransient<IKafkaConsumer<IProjectEvent>, KafkaConsumer<IProjectEvent>>()
          .AddTransient<IKafkaConsumer<ICustomerEvent>, KafkaConsumer<ICustomerEvent>>()
          .AddTransient<IMessageTypeResolver, MessageResolver>()
          .AddTransient<IRepositoryFactory, RepositoryFactory>()
          .AddTransient<IRepository<ISubscriptionEvent>, SubscriptionRepository>()
          .AddTransient<IRepository<IProjectEvent>, ProjectRepository>()
          .AddTransient<IRepository<ICustomerEvent>, CustomerRepository>()
          .AddSingleton<IConfigurationStore, GenericConfiguration>()
          .BuildServiceProvider();
      new DependencyInjectionProvider(serviceProvider);
    }

    private void CleanCollection()
    {
      DependencyInjectionProvider.CleanDependencyInjection();
    }

  }
}
