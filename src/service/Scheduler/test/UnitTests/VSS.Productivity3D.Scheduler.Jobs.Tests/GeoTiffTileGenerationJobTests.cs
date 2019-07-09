﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VSS.Common.Abstractions.Configuration;
using VSS.Common.Abstractions.ServiceDiscovery.Interfaces;
using VSS.Common.Exceptions;
using VSS.Pegasus.Client;
using VSS.Pegasus.Client.Models;
using VSS.Productivity.Push.Models.Notifications;
using VSS.Productivity3D.Push.Abstractions.Notifications;
using VSS.Productivity3D.Push.Clients.Notifications;
using VSS.Productivity3D.Scheduler.Jobs.DxfTileJob;
using VSS.Productivity3D.Scheduler.Jobs.DxfTileJob.Models;
using VSS.Productivity3D.Scheduler.Models;
using VSS.WebApi.Common;

namespace VSS.Productivity3D.Scheduler.Jobs.Tests
{
  [TestClass]
  public class GeoTiffTileGenerationJobTests
  {
    private ILoggerFactory loggerFactory;
    private IServiceProvider serviceProvider;

    [TestInitialize]
    public void TestInitialize()
    {
      var services = new ServiceCollection();
      serviceProvider = services
        .AddLogging()
        .BuildServiceProvider();

      loggerFactory = serviceProvider.GetService<ILoggerFactory>();
    }

    [TestMethod]
    public void CanSetupGeoTiffJob() => CreateGeoTiffJobWithMocks().Setup(null);

    [TestMethod]
    public void CanTearDownGeoTiffJob() => CreateGeoTiffJobWithMocks().TearDown(null);

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task CanRunGeoTiffJobSuccess(bool enableGeoTiffTileGeneration)
    {
      var request = new TileGenerationRequest
      {
        CustomerUid = Guid.NewGuid(),
        ProjectUid = Guid.NewGuid(),
        ImportedFileUid = Guid.NewGuid(),
        DataOceanRootFolder = "some folder",
        FileName = "a geotiff file",
      };

      var obj = JObject.Parse(JsonConvert.SerializeObject(request));
      var configStore = new Mock<IConfigurationStore>();

      configStore.Setup(x => x.GetValueBool("SCHEDULER_ENABLE_DXF_TILE_GENERATION"))
                 .Returns(enableGeoTiffTileGeneration);

      var mockPegasus = new Mock<IPegasusClient>();

      mockPegasus.Setup(x => x.GenerateGeoTiffTiles(
                           It.IsAny<string>(),
                           It.IsAny<Dictionary<string, string>>()))
                 .ReturnsAsync(new TileMetadata());

      var mockNotification = new Mock<INotificationHubClient>();

      mockNotification.Setup(n => n.Notify(It.IsAny<ProjectFileRasterTilesGeneratedNotification>()))
                      .Returns(Task.FromResult(default(object)));

      var mockTPaaSAuth = new Mock<ITPaaSApplicationAuthentication>();

      mockTPaaSAuth.Setup(t => t.GetApplicationBearerToken())
                   .Returns("this is a dummy bearer token");

      var job = new GeoTiffTileGenerationJob(configStore.Object, mockPegasus.Object, mockTPaaSAuth.Object, mockNotification.Object, loggerFactory);

      await job.Run(obj);

      var runTimes = enableGeoTiffTileGeneration ? Times.Once() : Times.Never();

      // Verify based on the value of SCHEDULER_ENABLE_DXF_TILE_GENERATION the execution of GenerateGeoTiffTiles().
      mockPegasus.Verify(x => x.GenerateGeoTiffTiles(
                           It.IsAny<string>(),
                           It.IsAny<Dictionary<string, string>>()), runTimes);
    }

    [TestMethod]
    public async Task CanRunGeoTiffJobFailureMissingRequest() => await Assert.ThrowsExceptionAsync<ServiceException>(() => CreateGeoTiffJobWithMocks().Run(null));

    [TestMethod]
    public async Task CanRunGeoTiffJobFailureWrongRequest()
    {
      var obj = JObject.Parse(JsonConvert.SerializeObject(new JobRequest())); //any model which is not TileGenerationRequest

      await Assert.ThrowsExceptionAsync<ServiceException>(() => CreateGeoTiffJobWithMocks().Run(obj));
    }

    private GeoTiffTileGenerationJob CreateGeoTiffJobWithMocks()
    {
      var configStore = new Mock<IConfigurationStore>();
      var mockPegasus = new Mock<IPegasusClient>();
      var mockTPaaSAuth = new Mock<ITPaaSApplicationAuthentication>();
      var mockProvider = new Mock<IServiceProvider>();
      var mockConfig = new Mock<IConfigurationStore>();
      var mockPushProxy = new Mock<IServiceResolution>();
      var mockNotification = new Mock<NotificationHubClient>(mockProvider.Object, mockConfig.Object, mockPushProxy.Object, loggerFactory);

      return new GeoTiffTileGenerationJob(configStore.Object, mockPegasus.Object, mockTPaaSAuth.Object, mockNotification.Object, loggerFactory);
    }
  }
}