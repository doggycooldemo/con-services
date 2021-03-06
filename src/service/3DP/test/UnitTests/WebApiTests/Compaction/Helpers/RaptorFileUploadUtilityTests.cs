﻿using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using VSS.MasterData.Models.Models;
using VSS.Productivity3D.WebApi.Compaction.ActionServices;
using VSS.Serilog.Extensions;

namespace VSS.Productivity3D.WebApiTests.Compaction.Helpers
{
  [TestClass]
  public class RaptorFileUploadUtilityTests
  {
    private static IServiceProvider serviceProvider;
    private static ILoggerFactory logger;
    private static RaptorFileUploadUtility uploadUtility;

    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
      serviceProvider = new ServiceCollection()
        .AddLogging()
        .AddSingleton(new LoggerFactory().AddSerilog(SerilogExtensions.Configure("VSS.Productivity3D.WebApi.Tests.log")))
        .BuildServiceProvider();

      logger = serviceProvider.GetRequiredService<ILoggerFactory>();

      uploadUtility = new RaptorFileUploadUtility(logger);
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("  ")]
    [DataRow("notpresentfile")]
    public void DeleteFile_should_not_throw_When_file_doesnt_exist(string filename)
    {
      Assert.IsTrue(Task.Run(() => { uploadUtility.DeleteFile(filename); })
                        .Wait(TimeSpan.FromSeconds(1)));
    }

    [TestMethod]
    public void UploadFile_should_return_When_filesize_exceeds_max_limit()
    {
      var filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      var filename = Guid.NewGuid().ToString();

      Assert.IsNotNull(filePath);

      File.WriteAllBytes(Path.Combine(filePath, filename), new byte[20 * 1024 * 1024 + 1]);

      var fileDescriptor = FileDescriptor.CreateFileDescriptor("1", filePath, filename);

      var allBytes = File.ReadAllBytes(Path.Combine(fileDescriptor.Path, fileDescriptor.FileName));
      (bool success, string message) = uploadUtility.UploadFile(fileDescriptor, allBytes);

      Assert.IsFalse(success);
      Assert.IsNotNull(message);
    }
  }
}
