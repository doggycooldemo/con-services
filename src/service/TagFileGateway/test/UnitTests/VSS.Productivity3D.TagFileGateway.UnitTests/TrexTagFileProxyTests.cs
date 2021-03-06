﻿using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using VSS.Common.Abstractions.Cache.Interfaces;
using VSS.Common.Abstractions.Configuration;
using VSS.Common.Abstractions.ServiceDiscovery.Interfaces;
using VSS.Common.Exceptions;
using VSS.MasterData.Models.Handlers;
using VSS.MasterData.Models.ResultHandling.Abstractions;
using VSS.MasterData.Proxies.Interfaces;
using VSS.Productivity3D.Models.Models;
using VSS.Serilog.Extensions;
using VSS.TRex.Gateway.Common.Abstractions;
using VSS.TRex.Gateway.Common.Proxy;
using Xunit;

namespace VSS.Productivity3D.TagFileGateway.UnitTests
{
  public class TagFileGatewayTests
  {
    private static IServiceProvider _serviceProvider;
    private static ILoggerFactory _logger;
    private static IHeaderDictionary _customHeaders;

    private static Mock<IConfigurationStore> _mockStore = new Mock<IConfigurationStore>();

    private static CompactionTagFileRequest request =
      new CompactionTagFileRequest
      {
        ProjectId = 554,
        ProjectUid = Guid.NewGuid(),
        FileName = "Machine Name--whatever--161230235959.tag",
        Data = new byte[] {0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9},
        OrgId = string.Empty
      };


    public TagFileGatewayTests()
    {
      _serviceProvider = new ServiceCollection()
        .AddLogging()
        .AddSingleton(new LoggerFactory().AddSerilog(SerilogExtensions.Configure("VSS.Productivity3D.WebApi.Tests.log")))
        .AddSingleton<IConfigurationStore>(_mockStore.Object)
        .AddTransient<IServiceExceptionHandler, ServiceExceptionHandler>()
        .AddTransient<ITRexTagFileProxy, TRexTagFileV2Proxy>() // Class under test
#if RAPTOR
        .AddTransient<IErrorCodesProvider, RaptorResult>()
#endif
        .BuildServiceProvider();

      _logger = _serviceProvider.GetRequiredService<ILoggerFactory>();
      _customHeaders = new HeaderDictionary();
    }

    [Fact]
    public void TestNonDirectSuccess()
    {
      // Setup a single tag file send
      var trexProxy = new Mock<TRexTagFileV2Proxy>(new Mock<IWebRequest>().Object, _mockStore.Object, _logger, new Mock<IDataCache>().Object, new Mock<IServiceResolution>().Object) {CallBase = true};
      trexProxy.Setup(m => m.SendTagFileRequest(It.Is<CompactionTagFileRequest>(r => r == request),
          It.Is<IHeaderDictionary>(d => Equals(d, _customHeaders)), "/tagfiles"))
        .Returns(Task.FromResult(new ContractExecutionResult(0)));

      var result = trexProxy.Object.SendTagFile(request, _customHeaders).Result;

      result.Should().NotBeNull();
      result.Code.Should().Be(0);

      // Validate we only tried to send the file once - with the correct values
      trexProxy.Verify(m => m.SendTagFileRequest(It.Is<CompactionTagFileRequest>(r => r == request),
        It.Is<IHeaderDictionary>(d => Equals(d, _customHeaders)), "/tagfiles"), Times.Once);
    }

    [Fact]
    public void TestNonZeroFailure()
    {
      var callCount = 0;
      // Setup a non zero result for first try, which should be returned
      var trexProxy = new Mock<TRexTagFileV2Proxy>(new Mock<IWebRequest>().Object, _mockStore.Object, _logger, new Mock<IDataCache>().Object, new Mock<IServiceResolution>().Object) {CallBase = true};
      trexProxy.Setup(m => m.SendTagFileRequest(It.Is<CompactionTagFileRequest>(r => r == request),
          It.Is<IHeaderDictionary>(d => Equals(d, _customHeaders)), "/tagfiles"))
        .Callback(() => callCount++)
        .Returns(() => { return Task.FromResult(new ContractExecutionResult(1)); });

      // Test
      var result = trexProxy.Object.SendTagFile(request, _customHeaders).Result;

      // Validate - should be ok
      result.Should().NotBeNull();
      result.Code.Should().Be(1);

      trexProxy.Verify(m => m.SendTagFileRequest(It.Is<CompactionTagFileRequest>(r => r == request),
        It.Is<IHeaderDictionary>(d => Equals(d, _customHeaders)), "/tagfiles"), Times.Exactly(1));
    }

    [Fact]
    public void TestExceptionFailure()
    {
      // Setup a non zero result for first try, then success on second try
      var trexProxy = new Mock<TRexTagFileV2Proxy>(new Mock<IWebRequest>().Object, _mockStore.Object, _logger, new Mock<IDataCache>().Object, new Mock<IServiceResolution>().Object) {CallBase = true};
      trexProxy.Setup(m => m.SendTagFileRequest(It.Is<CompactionTagFileRequest>(r => r == request),
          It.Is<IHeaderDictionary>(d => Equals(d, _customHeaders)), "/tagfiles"))
        .Returns(() => { throw new Exception("mock-message"); });

      var exception = false;
      try
      {
        var result = trexProxy.Object.SendTagFile(request, _customHeaders).Result;

      }
      catch (Exception e)
      {
        exception = true;
      }

      exception.Should().BeTrue();

      trexProxy.Verify(m => m.SendTagFileRequest(It.Is<CompactionTagFileRequest>(r => r == request),
        It.Is<IHeaderDictionary>(d => Equals(d, _customHeaders)), "/tagfiles"), Times.Exactly(1));
    }
  }
}
