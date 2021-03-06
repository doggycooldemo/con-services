﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VSS.Common.Abstractions.Configuration;
using VSS.MasterData.Models.Models;
using VSS.Productivity3D.Models.Enums;
using VSS.Productivity3D.TagFileAuth.Abstractions.Interfaces;
using VSS.Productivity3D.TagFileAuth.Models;
using VSS.Productivity3D.TagFileAuth.Models.ResultsHandling;
using VSS.TRex.DI;
using VSS.TRex.Events;
using VSS.TRex.Events.Interfaces;
using VSS.TRex.SiteModels;
using VSS.TRex.SiteModels.Interfaces;
using VSS.TRex.Storage.Interfaces;
using VSS.TRex.Storage.Models;
using VSS.TRex.SurveyedSurfaces.Interfaces;
using VSS.TRex.TAGFiles.Classes;
using VSS.TRex.TAGFiles.Classes.Validator;
using VSS.TRex.TAGFiles.Executors;
using VSS.TRex.Tests.TestFixtures;
using Xunit;

namespace TAGFiles.Tests
{
  public class TagfileValidatorTests : IClassFixture<DILoggingFixture>
  {
    [Fact]
    public void Test_TFASpecific_DIMocking()
    {
      SetupDITfa(true);

      var config = DIContext.Obtain<IConfigurationStore>();

      var tfaServiceEnabled = config.GetValueBool("ENABLE_TFA_SERVICE");
      Assert.True(tfaServiceEnabled);

      var minTagFileLength = config.GetValueInt("MIN_TAGFILE_LENGTH");
      Assert.Equal(100, minTagFileLength);
    }

    [Fact]
    public async Task Test_InvalidTagFile_TooSmall()
    {
      SetupDITfa(false);

      TagFileDetail td = new TagFileDetail
      {
        assetId = Guid.NewGuid(),
        projectId = Guid.NewGuid(),
        tagFileName = "Test.tag",
        tagFileContent = new byte[1],
        tccOrgId = "",
        IsJohnDoe = false
      };

      // Validate tagfile 
      var result = TagfileValidator.PreScanTagFile(td, out var tagFilePreScan);
      Assert.True(result.Code == (int)TRexTagFileResultCode.TRexInvalidTagfile, "Failed to return correct error code");
      Assert.Equal("TRexInvalidTagfile", result.Message);
      Assert.NotNull(tagFilePreScan);
    }

    [Fact]
    public async Task Test_InvalidTagFile_UnableToRead()
    {
      SetupDITfa(false);

      var td = new TagFileDetail()
      {
        assetId = Guid.NewGuid(),
        projectId = Guid.NewGuid(),
        tagFileName = "Test.tag",
        tagFileContent = new byte[101],
        tccOrgId = "",
        IsJohnDoe = false
      };

      // Validate tagfile 
      var result = TagfileValidator.PreScanTagFile(td, out var tagFilePreScan);
      Assert.True(result.Code == (int)TRexTagFileResultCode.TRexTagFileReaderError, "Failed to return correct error code");
      Assert.Equal("InvalidValueTypeID", result.Message);
      Assert.NotNull(tagFilePreScan);
    }

    [Fact]
    public async Task Test_HasAssetUid_TfaByPassed()
    {
      // note that assetId is available, thus this comes from the test tool TagFileSubmitted,
      // and although TFA is enabled, it won't be called
      SetupDITfa();

      byte[] tagContent;
      using (var tagFileStream =
        new FileStream(Path.Combine("TestData", "TAGFiles", "TestTAGFile.tag"),
          FileMode.Open, FileAccess.Read))
      {
        tagContent = new byte[tagFileStream.Length];
        tagFileStream.Read(tagContent, 0, (int)tagFileStream.Length);
      }

      var td = new TagFileDetail()
      {
        assetId = Guid.NewGuid(),
        projectId = Guid.NewGuid(),
        tagFileName = "Test.tag",
        tagFileContent = tagContent,
        tccOrgId = "",
        IsJohnDoe = false
      };

      var result = TagfileValidator.PreScanTagFile(td, out var tagFilePreScan);
      Assert.Equal("success", result.Message);
      Assert.NotNull(tagFilePreScan);
      Assert.Equal("0523J019SW", tagFilePreScan.HardwareID);

      result = await TagfileValidator.ValidSubmission(td, tagFilePreScan).ConfigureAwait(false);
      Assert.True(result.Code == (int)TRexTagFileResultCode.Valid, "Failed to return a Valid request");
      Assert.Equal("success", result.Message);
    }

    [Fact]
    public async Task Test_ValidateOk()
    {
      var projectUid = Guid.NewGuid();
      var moqRequest = new GetProjectUidsRequest(projectUid.ToString(), string.Empty, 40, 50);
      var moqResult = new GetProjectUidsResult(projectUid.ToString(), null, null, 0, "success");
      SetupDITfa(true, moqRequest, moqResult);

      byte[] tagContent;
      using (var tagFileStream =
        new FileStream(Path.Combine("TestData", "TAGFiles", "TestTAGFile.tag"),
          FileMode.Open, FileAccess.Read))
      {
        tagContent = new byte[tagFileStream.Length];
        tagFileStream.Read(tagContent, 0, (int)tagFileStream.Length);
      }

      var td = new TagFileDetail()
      {
        assetId = null,
        projectId = projectUid,
        tagFileName = "Test.tag",
        tagFileContent = tagContent,
        tccOrgId = "",
        IsJohnDoe = false
      };

      var tagFilePreScan = new TAGFilePreScan();
      await using (var stream = new MemoryStream(td.tagFileContent))
        tagFilePreScan.Execute(stream);
      var result = await TagfileValidator.ValidSubmission(td, tagFilePreScan).ConfigureAwait(false);
      Assert.True(result.Code == (int)TRexTagFileResultCode.Valid, "Failed to return a Valid request");
      Assert.Equal("success", result.Message);
    }


    [Fact]
    public async Task Test_PlatformSerialNoValidationOk()
    {
      // This test ensures platformSerialNumber (serial/deviceid) is extracted and used in validation. Note this Tagfile has no Radio Serial id. Only Serial id. 
      var projectUid = Guid.NewGuid();
      var moqRequest = new GetProjectUidsRequest(projectUid.ToString(), string.Empty, 40, 50);
      var moqResult = new GetProjectUidsResult(projectUid.ToString(), null, null, 0, "success");
      SetupDITfa(true, moqRequest, moqResult);

      byte[] tagContent;
      using (var tagFileStream =
        new FileStream(Path.Combine("TestData", "TAGFiles", "2415J078SW-Serial-Test.tag"), FileMode.Open, FileAccess.Read))
      {
        tagContent = new byte[tagFileStream.Length];
        tagFileStream.Read(tagContent, 0, (int)tagFileStream.Length);
      }

      var td = new TagFileDetail()
      {
        assetId = null,
        projectId = null, // force validation on serial id
        tagFileName = "2415J078SW-Serial-Test.tag",
        tagFileContent = tagContent,
        tccOrgId = "",
        IsJohnDoe = false
      };

      var tagFilePreScan = new TAGFilePreScan();
      await using (var stream = new MemoryStream(td.tagFileContent))
        tagFilePreScan.Execute(stream);
      var result = await TagfileValidator.ValidSubmission(td, tagFilePreScan).ConfigureAwait(false);
      Assert.True(result.Code == (int)TRexTagFileResultCode.Valid, "Failed to return a Valid request");
      Assert.True(td.projectId != null, "Failed to return a Valid projectID");
      Assert.Equal("success", result.Message);
    }


    [Fact]
    public async Task Test_UsingNEE_ValidateOk()
    {
      var projectUid = Guid.NewGuid();
      var moqRequest = new GetProjectUidsRequest(projectUid.ToString(), "1639J101YU", 0, 0, 5876814.5384829007, 7562822.7801738745);
      var moqResult = new GetProjectUidsResult(projectUid.ToString(), string.Empty, string.Empty, 0, "success");
      SetupDITfa(true, moqRequest, moqResult);

      byte[] tagContent;
      using (var tagFileStream =
        new FileStream(Path.Combine("TestData", "TAGFiles", "SeedPosition-usingNEE.tag"),
          FileMode.Open, FileAccess.Read))
      {
        tagContent = new byte[tagFileStream.Length];
        tagFileStream.Read(tagContent, 0, (int)tagFileStream.Length);
      }

      var td = new TagFileDetail()
      {
        assetId = null,
        projectId = projectUid,
        tagFileName = "Bug ccssscon-401 NEE SeedPosition.tag",
        tagFileContent = tagContent,
        tccOrgId = "",
        IsJohnDoe = false
      };

      var tagFilePreScan = new TAGFilePreScan();
      await using (var stream = new MemoryStream(td.tagFileContent))
        tagFilePreScan.Execute(stream); 
      var result = await TagfileValidator.ValidSubmission(td, tagFilePreScan).ConfigureAwait(false);
      Assert.True(result.Code == (int)TRexTagFileResultCode.Valid, "Failed to return a Valid request");
      Assert.Equal("success", result.Message);
    }
    
    [Fact]
    public async Task Test_ValidateFailed_InvalidManualProjectType()
    {
      var projectUid = Guid.NewGuid();
      var moqRequest = new GetProjectUidsRequest(projectUid.ToString(), string.Empty, 0, 0);
      var moqResult = new GetProjectUidsResult(string.Empty, string.Empty, string.Empty, 3044, "Manual Import: cannot import to a Civil type project");
      SetupDITfa(true, moqRequest, moqResult);

      byte[] tagContent;
      using (var tagFileStream =
        new FileStream(Path.Combine("TestData", "TAGFiles", "TestTAGFile.tag"),
          FileMode.Open, FileAccess.Read))
      {
        tagContent = new byte[tagFileStream.Length];
        tagFileStream.Read(tagContent, 0, (int)tagFileStream.Length);
      }

      var td = new TagFileDetail()
      {
        assetId = null,
        projectId = projectUid,
        tagFileName = "Test.tag",
        tagFileContent = tagContent,
        tccOrgId = "",
        IsJohnDoe = false
      };

      var tagFilePreScan = new TAGFilePreScan();
      await using (var stream = new MemoryStream(td.tagFileContent))
        tagFilePreScan.Execute(stream);
      var result = await TagfileValidator.ValidSubmission(td, tagFilePreScan).ConfigureAwait(false);
      Assert.True(result.Code == 3044, "Failed to return correct error code");
      Assert.Equal("Manual Import: cannot import to a Civil type project", result.Message);
    }

    [Fact]
    public void Test_TagFileArchive()
    {
      SetupDITfa();

      byte[] tagContent;
      using (var tagFileStream =
        new FileStream(Path.Combine("TestData", "TAGFiles", "TestTAGFile.tag"),
          FileMode.Open, FileAccess.Read))
      {
        tagContent = new byte[tagFileStream.Length];
        tagFileStream.Read(tagContent, 0, (int)tagFileStream.Length);
      }

      var td = new TagFileDetail()
      {
        assetId = Guid.Parse("{00000000-0000-0000-0000-000000000001}"),
        projectId = Guid.Parse("{00000000-0000-0000-0000-000000000001}"),
        tagFileName = "Test.tag",
        tagFileContent = tagContent,
        tccOrgId = "",
        IsJohnDoe = false
      };

      Assert.True(TagFileRepository.ArchiveTagfileS3(td).Result, "Failed to archive tagfile");
    }


    [Fact]
    public void Test_TagFileArchive_Failed()
    {
      SetupDITfa();

      byte[] tagContent;
      using (var tagFileStream =
        new FileStream(Path.Combine("TestData", "TAGFiles", "TestTAGFile.tag"),
          FileMode.Open, FileAccess.Read))
      {
        tagContent = new byte[tagFileStream.Length];
        tagFileStream.Read(tagContent, 0, (int)tagFileStream.Length);
      }

      var td = new TagFileDetail()
      {
        assetId = Guid.Parse("{00000000-0000-0000-0000-000000000001}"),
        projectId = Guid.Parse("{00000000-0000-0000-0000-000000000001}"),
        tagFileName = "Test.tag",
        tagFileContent = null,
        tccOrgId = "",
        IsJohnDoe = false
      };

      Assert.False(TagFileRepository.ArchiveTagfileS3(td).Result, "Failed to validate null data archive");
    }

    private void SetupDITfa(bool enableTfaService = true, GetProjectUidsRequest getProjectUidsRequest = null, GetProjectUidsResult getProjectUidsResult = null)
    {
      // this setup includes the DITagFileFixture. Done here to try to avoid random test failures.

      var moqStorageProxy = new Mock<IStorageProxy>();

      var moqStorageProxyFactory = new Mock<IStorageProxyFactory>();
      moqStorageProxyFactory.Setup(mk => mk.Storage(StorageMutability.Immutable)).Returns(moqStorageProxy.Object);
      moqStorageProxyFactory.Setup(mk => mk.Storage(StorageMutability.Mutable)).Returns(moqStorageProxy.Object);
      moqStorageProxyFactory.Setup(mk => mk.MutableGridStorage()).Returns(moqStorageProxy.Object);
      moqStorageProxyFactory.Setup(mk => mk.ImmutableGridStorage()).Returns(moqStorageProxy.Object);

      var moqSurveyedSurfaces = new Mock<ISurveyedSurfaces>();

      var moqSiteModels = new Mock<ISiteModels>();
      moqSiteModels.Setup(mk => mk.PrimaryMutableStorageProxy).Returns(moqStorageProxy.Object);

      DIBuilder
        .Continue()
        .Add(x => x.AddSingleton<IStorageProxyFactory>(moqStorageProxyFactory.Object))
        .Add(x => x.AddSingleton<ISiteModels>(moqSiteModels.Object))

        .Add(x => x.AddSingleton<ISurveyedSurfaces>(moqSurveyedSurfaces.Object))
        .Add(x => x.AddSingleton<IProductionEventsFactory>(new ProductionEventsFactory()))
        .Build();

      var newSiteModelGuidTfa = Guid.NewGuid();

      ISiteModel mockedSiteModel = new SiteModel(newSiteModelGuidTfa, StorageMutability.Immutable);
      mockedSiteModel.SetStorageRepresentationToSupply(StorageMutability.Mutable);

      var moqSiteModelFactory = new Mock<ISiteModelFactory>();
      moqSiteModelFactory.Setup(mk => mk.NewSiteModel(StorageMutability.Mutable)).Returns(mockedSiteModel);

      moqSiteModels.Setup(mk => mk.GetSiteModel(newSiteModelGuidTfa)).Returns(mockedSiteModel);

      // Mock the new site model creation API to return just a new site model
      moqSiteModels.Setup(mk => mk.GetSiteModel(newSiteModelGuidTfa, true)).Returns(mockedSiteModel);

      //Moq doesn't support extension methods in IConfiguration/Root.
      var moqConfiguration = DIContext.Obtain<Mock<IConfigurationStore>>();
      var moqMinTagFileLength = 100;
      moqConfiguration.Setup(x => x.GetValueBool("ENABLE_TFA_SERVICE", It.IsAny<bool>())).Returns(enableTfaService);
      moqConfiguration.Setup(x => x.GetValueBool("ENABLE_TFA_SERVICE")).Returns(enableTfaService);
      moqConfiguration.Setup(x => x.GetValueInt("MIN_TAGFILE_LENGTH", It.IsAny<int>())).Returns(moqMinTagFileLength);
      moqConfiguration.Setup(x => x.GetValueInt("MIN_TAGFILE_LENGTH")).Returns(moqMinTagFileLength);

      var moqTfaProxy = new Mock<ITagFileAuthProjectV5Proxy>();
      if (enableTfaService && getProjectUidsRequest != null)
        moqTfaProxy.Setup(x => x.GetProjectUids(It.IsAny<GetProjectUidsRequest>(), It.IsAny<IHeaderDictionary>())).ReturnsAsync(getProjectUidsResult);

      DIBuilder
        .Continue()
        .Add(x => x.AddSingleton(moqConfiguration.Object))
        .Add(x => x.AddSingleton<ISiteModelFactory>(new SiteModelFactory()))
        .Add(x => x.AddSingleton(moqTfaProxy.Object))
        .Complete();
    }
  }
}
