﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VSS.Common.Exceptions;
using VSS.Productivity3D.Scheduler.Jobs.DxfTileJob.Models;
using VSS.Visionlink.Interfaces.Events.MasterData.Models;

namespace VSS.Productivity3D.Scheduler.Jobs.Tests
{
  [TestClass]
  public class DxfTileGenerationRequestTests
  {
    [TestMethod]
    public void ValidateDxfTileGenerationRequestSuccess()
    {
      var request = CreateDxfTileGenerationRequest();
      request.Validate();
    }

    [TestMethod]
    public void ValidateDxfTileGenerationRequestMissingCustomerUid()
    {
      var request = CreateDxfTileGenerationRequest();
      request.CustomerUid = Guid.Empty;
      Assert.ThrowsException<ServiceException>(() => request.Validate());
    }

    [TestMethod]
    public void ValidateDxfTileGenerationRequestMissingProjectUid()
    {
      var request = CreateDxfTileGenerationRequest();
      request.ProjectUid = Guid.Empty;
      Assert.ThrowsException<ServiceException>(() => request.Validate());
    }

    [TestMethod]
    public void ValidateDxfTileGenerationRequestMissingImportedFileUid()
    {
      var request = CreateDxfTileGenerationRequest();
      request.ImportedFileUid = Guid.Empty;
      Assert.ThrowsException<ServiceException>(() => request.Validate());
    }

    [TestMethod]
    public void ValidateDxfTileGenerationRequestMissingRootFolder()
    {
      var request = CreateDxfTileGenerationRequest();
      request.DataOceanRootFolder = null;
      Assert.ThrowsException<ServiceException>(() => request.Validate());
    }

    [TestMethod]
    public void ValidateDxfTileGenerationRequestMissingDxfFileName()
    {
      var request = CreateDxfTileGenerationRequest();
      request.FileName = null;
      Assert.ThrowsException<ServiceException>(() => request.Validate());
    }

    [TestMethod]
    public void ValidateDxfTileGenerationRequestMissingDcFileName()
    {
      var request = CreateDxfTileGenerationRequest();
      request.DcFileName = null;
      Assert.ThrowsException<ServiceException>(() => request.Validate());
    }

    private DxfTileGenerationRequest CreateDxfTileGenerationRequest()
    {
      return new DxfTileGenerationRequest
      {
        CustomerUid = Guid.NewGuid(),
        ProjectUid = Guid.NewGuid(),
        ImportedFileUid = Guid.NewGuid(),
        DataOceanRootFolder = "some folder",
        FileName = "some dxf file",
        DcFileName = "some coord system file",
        DxfUnitsType = DxfUnitsType.Meters
      };
    }
  }
}
