﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VSS.Common.Abstractions.Clients.CWS.Interfaces;
using VSS.Common.Abstractions.Clients.CWS.Models;
using VSS.Common.Abstractions.ServiceDiscovery.Enums;

namespace CCSS.CWS.Client.UnitTests.Mocked
{
  [TestClass]
  public class ProjectMockedTests : BaseTestClass
  {
    protected override IServiceCollection SetupTestServices(IServiceCollection services)
    {
      services.AddSingleton(mockWebRequest.Object);
      services.AddSingleton(mockServiceResolution.Object);
      services.AddTransient<ICwsProjectClient, CwsProjectClient>();

      return services;
    }

    [TestMethod]
    public void CreateProjectTest()
    {
      var customerUid = new Guid("560c2a6c-6b7e-48d8-b1a5-e4009e2d4c97");
      //var accountTrn = "trn::profilex:us-west-2:account:{customerUid}";
      string expectedProjectUid = "560c2a6c-6b7e-48d8-b1a5-e4009e2d4c97";
      string expectedProjectTrn = $"trn::profilex:us-west-2:project:{expectedProjectUid}";
      
      var createProjectRequestModel = new CreateProjectRequestModel
      {
        accountId = customerUid.ToString(),
        projectName = "my first project",        
        boundary = new ProjectBoundary()
        {
          type = "Polygon",
          coordinates = new List<double[,]>() { { new double[2, 2] { { 180, 90 }, { 180, 90 } } } } 
        }
      };

      var createProjectResponseModel = new CreateProjectResponseModel
      {
        Id = expectedProjectTrn
      };
      const string route = "/projects";
      var expectedUrl = $"{baseUrl}{route}";
      mockServiceResolution.Setup(m => m.ResolveRemoteServiceEndpoint(
        It.IsAny<string>(), It.IsAny<ApiType>(), It.IsAny<ApiVersion>(), route, It.IsAny<IList<KeyValuePair<string, string>>>())).Returns(Task.FromResult(expectedUrl));

      MockUtilities.TestRequestSendsCorrectJson("Create a project", mockWebRequest, null, expectedUrl, HttpMethod.Post, createProjectResponseModel, async () =>
      {
        var client = ServiceProvider.GetRequiredService<ICwsProjectClient>();
        var result = await client.CreateProject(createProjectRequestModel);

        Assert.IsNotNull(result, "No result from posting my project");
        Assert.AreEqual(expectedProjectUid, result.Id);
        return true;
      });
    }

    [TestMethod]
    public void UpdateProjectDetailsTest()
    {
      var customerUid = new Guid("560c2a6c-6b7e-48d8-b1a5-e4009e2d4c97");
      //var accountTrn = "trn::profilex:us-west-2:account:{customerUid}";
      string projectUid = "560c2a6c-6b7e-48d8-b1a5-e4009e2d4c97";
      string projectTrn = $"trn::profilex:us-west-2:project:{projectUid}";

      var updateProjectDetailsRequestModel = new UpdateProjectDetailsRequestModel
      {        
        projectName = "my updated project"
      };

      string route = $"/projects/{projectTrn}";
      var expectedUrl = $"{baseUrl}{route}";
      mockServiceResolution.Setup(m => m.ResolveRemoteServiceEndpoint(
        It.IsAny<string>(), It.IsAny<ApiType>(), It.IsAny<ApiVersion>(), route, It.IsAny<IList<KeyValuePair<string, string>>>())).Returns(Task.FromResult(expectedUrl));

      MockUtilities.TestRequestSendsCorrectJson("Update a projects details", mockWebRequest, null, expectedUrl, HttpMethod.Post, updateProjectDetailsRequestModel, async () =>
      {
        var client = ServiceProvider.GetRequiredService<ICwsProjectClient>();
        await client.UpdateProjectDetails(new Guid(projectUid), updateProjectDetailsRequestModel);

        return true;
      });
    }

    [TestMethod]
    public void UpdateProjectBoundaryTest()
    {
      var customerUid = new Guid("560c2a6c-6b7e-48d8-b1a5-e4009e2d4c97");
      //var accountTrn = "trn::profilex:us-west-2:account:{customerUid}";
      string projectUid = "560c2a6c-6b7e-48d8-b1a5-e4009e2d4c97";
      string projectTrn = $"trn::profilex:us-west-2:project:{projectUid}";

      var projectBoundary = new ProjectBoundary()
      {
        type = "Polygon",
        coordinates = new List<double[,]>() { { new double[2, 2] { { 180, 90 }, { 180, 90 } } } }
      };

      string route = $"/projects/{projectTrn}/boundary";
      var expectedUrl = $"{baseUrl}{route}";
      mockServiceResolution.Setup(m => m.ResolveRemoteServiceEndpoint(
        It.IsAny<string>(), It.IsAny<ApiType>(), It.IsAny<ApiVersion>(), route, It.IsAny<IList<KeyValuePair<string, string>>>())).Returns(Task.FromResult(expectedUrl));

      MockUtilities.TestRequestSendsCorrectJson("Update a projects boundary", mockWebRequest, null, expectedUrl, HttpMethod.Post, projectBoundary, async () =>
      {
        var client = ServiceProvider.GetRequiredService<ICwsProjectClient>();
        await client.UpdateProjectBoundary(new Guid(projectUid), projectBoundary);

        return true;
      });
    }

  }
}
