﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DesignProfiler.ComputeProfile.RPC;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog;
using VSS.Common.Abstractions.Configuration;
using VSS.Common.Exceptions;
using VSS.ConfigurationStore;
using VSS.MasterData.Models.Handlers;
using VSS.MasterData.Models.ResultHandling.Abstractions;
using VSS.Productivity3D.Common;
using VSS.Productivity3D.Common.Interfaces;
using VSS.Productivity3D.Common.ResultHandling;
using VSS.Productivity3D.Productivity3D.Models.Utilities;
using VSS.Productivity3D.WebApi.Models.Compaction.Executors;
using VSS.Productivity3D.WebApi.Models.Compaction.Models;
using VSS.Productivity3D.WebApi.Models.ProductionData.Models;
using VSS.Serilog.Extensions;
using VSS.Velociraptor.PDSInterface.DesignProfile;

namespace VSS.Productivity3D.WebApiTests.Compaction.Executors
{
  [TestClass]
  public class CompactionDesignProfileExecutorTests : ExecutorTestsBase
  {
    private static IServiceProvider serviceProvider;
    private static ILoggerFactory logger;

    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
      serviceProvider = new ServiceCollection()
        .AddLogging()
        .AddSingleton(new LoggerFactory().AddSerilog(SerilogExtensions.Configure("VSS.Productivity3D.WebApi.Tests.log")))
        .AddSingleton<IConfigurationStore, GenericConfiguration>()
        .AddTransient<IServiceExceptionHandler, ServiceExceptionHandler>()
        .AddTransient<IErrorCodesProvider, RaptorResult>().BuildServiceProvider();

      logger = serviceProvider.GetRequiredService<ILoggerFactory>();
    }

    [TestMethod]
    public void Should_return_empty_result_When_no_result_returned_from_Raptor()
    {
      var raptorClient = new Mock<IASNodeClient>();
      var configStore = new Mock<IConfigurationStore>();

      raptorClient
        .Setup(x => x.GetDesignProfile(It.IsAny<TDesignProfilerServiceRPCVerb_CalculateDesignProfile_Args>()))
        .Returns((MemoryStream)null);

      var request = new CompactionProfileDesignRequest(
        1234, null, null,  null, -1, null, new ProfileGridPoints(), new ProfileLLPoints(), ValidationConstants3D.MIN_STATION, ValidationConstants3D.MIN_STATION);

      var executor = RequestExecutorContainerFactory
        .Build<CompactionDesignProfileExecutor>(logger, raptorClient.Object, configStore: configStore.Object);
      var result = executor.ProcessAsync(request).Result as CompactionProfileResult<CompactionProfileVertex>;
      Assert.IsNotNull(result, ExecutorFailed);
      Assert.AreEqual(0, result.gridDistanceBetweenProfilePoints, WrongGridDistanceBetweenProfilePoints);
      Assert.AreEqual(0, result.results.Count, ResultsShouldBeEmpty);
    }

    [TestMethod]
    public async Task Should_return_correct_grid_distance_and_point_count()
    {
      var designProfile = new DesignProfile
      {
        vertices = new List<DesignProfileVertex>
        {
          new DesignProfileVertex { station = 0.000, elevation = 597.4387F },
          new DesignProfileVertex { station = 0.80197204271533173, elevation = 597.4356F },
          new DesignProfileVertex { station = 1.6069349835347948, elevation = 597.434265F }
        },
        GridDistanceBetweenProfilePoints = 13.951246308791798
      };
 
      var result = await MockGetProfile(designProfile);

      Assert.IsNotNull(result, ExecutorFailed);
      Assert.AreEqual(designProfile.GridDistanceBetweenProfilePoints, result.gridDistanceBetweenProfilePoints, WrongGridDistanceBetweenProfilePoints);
      Assert.AreEqual(designProfile.vertices.Count, result.results.Count, IncorrectNumberOfPoints);
    }

    [TestMethod]
    public async Task Should_fix_gap_points()
    {
      var designProfile = new DesignProfile
      {
        vertices = new List<DesignProfileVertex>
        {
          new DesignProfileVertex { station = 0.000, elevation = 597.4387F },
          new DesignProfileVertex { station = 0.80197204271533173, elevation = (float)VelociraptorConstants.NO_HEIGHT },
          new DesignProfileVertex { station = 1.6069349835347948, elevation = 597.434265F }
        },
        GridDistanceBetweenProfilePoints = 13.951246308791798
      };

      var result = await MockGetProfile(designProfile);

      Assert.IsNotNull(result, ExecutorFailed);
      Assert.AreEqual(designProfile.GridDistanceBetweenProfilePoints, result.gridDistanceBetweenProfilePoints, WrongGridDistanceBetweenProfilePoints);
      Assert.AreEqual(designProfile.vertices.Count-1, result.results.Count, IncorrectNumberOfPoints);
      Assert.AreEqual(ProfileCellType.Gap, result.results[0].cellType, "Wrong cellType 1");
      Assert.AreEqual(designProfile.vertices[0].station, result.results[0].station, "Wrong station 1");
      Assert.AreEqual(designProfile.vertices[0].elevation, result.results[0].elevation, "Wrong elevation 1");
      Assert.AreEqual(ProfileCellType.Edge, result.results[1].cellType, "Wrong cellType 2");
      Assert.AreEqual(designProfile.vertices[2].station, result.results[1].station, "Wrong station 2");
      Assert.AreEqual(designProfile.vertices[2].elevation, result.results[1].elevation, "Wrong elevation 2");
    }

    private async Task<CompactionProfileResult<CompactionProfileVertex>> MockGetProfile(DesignProfile designProfile)
    {
      var raptorClient = new Mock<IASNodeClient>();
      var configStore = new Mock<IConfigurationStore>();

      var ms = new MemoryStream();
      //No serialization so do it by hand
      BinaryWriter writer = new BinaryWriter(ms);     
      writer.Write(BitConverter.GetBytes(designProfile.vertices.Count));
      foreach (var vertex in designProfile.vertices)
      {
        writer.Write(BitConverter.GetBytes(vertex.station));
        writer.Write(BitConverter.GetBytes(vertex.elevation));
      }
      writer.Write(BitConverter.GetBytes(designProfile.GridDistanceBetweenProfilePoints));
      ms.Position = 0;
      raptorClient
        .Setup(x => x.GetDesignProfile(It.IsAny<TDesignProfilerServiceRPCVerb_CalculateDesignProfile_Args>()))
        .Returns(ms);

      var request = new CompactionProfileDesignRequest(
        1234, null, null, null, -1, null, new ProfileGridPoints(), new ProfileLLPoints(), ValidationConstants3D.MIN_STATION, ValidationConstants3D.MIN_STATION);

      var executor = RequestExecutorContainerFactory
        .Build<CompactionDesignProfileExecutor>(logger, raptorClient.Object, configStore: configStore.Object);
      var result = await executor.ProcessAsync(request) as CompactionProfileResult<CompactionProfileVertex>;
      return result;     
    }
  }
}
