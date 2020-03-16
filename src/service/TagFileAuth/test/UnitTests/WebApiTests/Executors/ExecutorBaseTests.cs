﻿using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog;
using VSS.Common.Abstractions.Configuration;
using VSS.ConfigurationStore;
using VSS.Productivity3D.Project.Abstractions.Interfaces;
using VSS.Productivity3D.TagFileAuth.Models;
using VSS.Serilog.Extensions;

namespace WebApiTests.Executors
{
  [TestClass]
  public class ExecutorBaseTests
  {
    protected IConfigurationStore ConfigStore;

    protected IServiceProvider ServiceProvider;
    protected Mock<IProjectProxy> projectProxy;
    protected Mock<ICustomerProxy> customerProxy;
    protected Mock<IDeviceProxy> deviceProxy;
    protected static ContractExecutionStatesEnum ContractExecutionStatesEnum = new ContractExecutionStatesEnum();

    [TestInitialize]
    public virtual void InitTest()
    {
      var serviceCollection = new ServiceCollection();
      
      serviceCollection
        .AddLogging()
        .AddSingleton(new LoggerFactory().AddSerilog(SerilogExtensions.Configure("VSS.TagFileAuth.WepApiTests.log")))
        .AddSingleton<IConfigurationStore, GenericConfiguration>();
      ServiceProvider = serviceCollection.BuildServiceProvider();
      ConfigStore = ServiceProvider.GetRequiredService<IConfigurationStore>();

      projectProxy = new Mock<IProjectProxy>();
      customerProxy = new Mock<ICustomerProxy>();
      deviceProxy = new Mock<IDeviceProxy>();
    }
  
  }
}
