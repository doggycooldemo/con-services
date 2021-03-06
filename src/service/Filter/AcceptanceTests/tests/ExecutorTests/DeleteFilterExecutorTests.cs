﻿using ExecutorTests.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using VSS.Common.Exceptions;
using VSS.Productivity3D.Filter.Common.Executors;
using VSS.Visionlink.Interfaces.Events.MasterData.Models;

namespace ExecutorTests
{
  [TestClass]
  public class DeleteFilterExecutorTests : FilterRepositoryBase
  {
    [TestInitialize]
    public void ClassInit()
    {
      Setup();
    }

    [TestMethod]
    [DataRow(FilterType.Persistent)]
    [DataRow(FilterType.Transient)]
    [DataRow(FilterType.Report)]
    public async Task DeleteFilterExecutor_NoExistingFilter(FilterType filterType)
    {
      var request = CreateAndValidateRequest(name:"delete " + filterType, filterType: filterType, onlyFilterUid: true);

      var executor =
        RequestExecutorContainer.Build<DeleteFilterExecutor>(ConfigStore, Logger, ServiceExceptionHandler, FilterRepo, null, ProjectProxy, 
          productivity3dV2ProxyNotification:Productivity3dV2ProxyNotification, productivity3dV2ProxyCompaction:Productivity3dV2ProxyCompaction);
      var ex = await Assert.ThrowsExceptionAsync<ServiceException>(async () => await executor.ProcessAsync(request)).ConfigureAwait(false);
      Assert.AreNotEqual(-1, ex.GetContent.IndexOf("2011", StringComparison.Ordinal), "executor threw exception but incorrect code");
      Assert.AreNotEqual(-1, ex.GetContent.IndexOf("DeleteFilter failed. The requested filter does not exist, or does not belong to the requesting customer; project or user.", StringComparison.Ordinal), "executor threw exception but incorrect message");
    }

    [TestMethod]
    [DataRow(FilterType.Persistent)]
    [DataRow(FilterType.Transient)]
    [DataRow(FilterType.Report)]
    public async Task DeleteFilterExecutor_ExistingFilter(FilterType filterType)
    {
      var custUid = Guid.NewGuid();
      var userId = Guid.NewGuid();
      var projectUid = Guid.NewGuid();
      var filterUid = Guid.NewGuid();
      string name = "blah";
      string filterJson = "theJsonString";

      WriteEventToDb(new CreateFilterEvent
      {
        CustomerUID = custUid,
        UserID = userId.ToString(),
        ProjectUID = projectUid,
        FilterUID = filterUid,
        Name = name,
        FilterType = filterType,
        FilterJson = filterJson,
        ActionUTC = DateTime.UtcNow
      });

      var request = CreateAndValidateRequest(name: name, customerUid: custUid.ToString(), userId: userId.ToString(), projectUid: projectUid.ToString(), filterUid: filterUid.ToString(), filterType: filterType, onlyFilterUid: true);

      var executor =
        RequestExecutorContainer.Build<DeleteFilterExecutor>(ConfigStore, Logger, ServiceExceptionHandler, FilterRepo, null, ProjectProxy,
          productivity3dV2ProxyNotification: Productivity3dV2ProxyNotification, productivity3dV2ProxyCompaction: Productivity3dV2ProxyCompaction);
      var result = await executor.ProcessAsync(request);

      Assert.IsNotNull(result, "executor should always return a result");
      Assert.AreEqual(0, result.Code, "executor returned incorrect code");
      Assert.AreEqual("success", result.Message, "executor returned incorrect message");
    }
  }
}
