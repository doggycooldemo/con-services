﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestUtility;

namespace WebApiTests
{
  [TestClass]
  public class FileImportTests
  {
    private readonly Msg msg = new Msg();
    private const string PROJECT_DB_SCHEMA_NAME = "VSS-MasterData-Project-Only";

    [TestMethod]
    public void TestNoFileUploads()
    {
      const string testName = "File Import 1";
      msg.Title(testName, "Create standard project and customer then get imported files - There should be none.");
      var ts = new TestSupport();
      var legacyProjectId = ts.SetLegacyProjectId();
      var projectUid = Guid.NewGuid().ToString();
      var customerUid = Guid.NewGuid();
      var tccOrg = Guid.NewGuid();
      var subscriptionUid = Guid.NewGuid();
      var startDateTime = ts.FirstEventDate;
      var endDateTime = new DateTime(9999, 12, 31);
      var startDate = startDateTime.ToString("yyyy-MM-dd");
      var endDate = endDateTime.ToString("yyyy-MM-dd");
      const string geometryWkt = "POLYGON((-121.347189366818 38.8361907402694,-121.349260032177 38.8361656688414,-121.349217116833 38.8387897637231,-121.347275197506 38.8387145521594,-121.347189366818 38.8361907402694,-121.347189366818 38.8361907402694))";
      var eventsArray = new[] {
       "| TableName           | EventDate   | CustomerUID   | Name       | fk_CustomerTypeID | SubscriptionUID   | fk_CustomerUID | fk_ServiceTypeID | StartDate   | EndDate        | fk_ProjectUID | TCCOrgID | fk_SubscriptionUID |",
      $"| Customer            | 0d+09:00:00 | {customerUid} | {testName} | 1                 |                   |                |                  |             |                |               |          |                    |",
      $"| CustomerTccOrg      | 0d+09:00:00 | {customerUid} |            |                   |                   |                |                  |             |                |               | {tccOrg} |                    |",
      $"| Subscription        | 0d+09:10:00 |               |            |                   | {subscriptionUid} | {customerUid}  | 19               | {startDate} | {endDate}      |               |          |                    |",
      $"| ProjectSubscription | 0d+09:20:00 |               |            |                   |                   |                |                  | {startDate} |                | {projectUid}  |          | {subscriptionUid}  |"};
      ts.PublishEventCollection(eventsArray);

      var projectsArray = new[] {
         "| TableName           | EventDate   | ProjectUID   | LegacyProjectID   | Name       | fk_ProjectTypeID | ProjectTimeZone           | LandfillTimeZone          | StartDate   | EndDate   | GeometryWKT   |",
        $"| Project             | 0d+09:10:00 | {projectUid} | {legacyProjectId} | {testName} | 0                | New Zealand Standard Time | New Zealand Standard Time | {startDate} | {endDate} | {geometryWkt} |"};
      ts.PublishEventCollection(projectsArray);
      var importFile = new ImportFile();
      var expectedResults = importFile.expectedImportFileDescriptorsListResult;
      var uri = ts.GetBaseUri() + $"api/v4/importedfiles?projectUid={projectUid}";

      var filesResult = importFile.GetImportedFilesFromWebApi(uri, customerUid);
      Assert.IsTrue(filesResult.ImportedFileDescriptors.Count == expectedResults.ImportedFileDescriptors.Count, " Expected number of fields does not match actual");
      CollectionAssert.AreEqual(expectedResults.ImportedFileDescriptors, filesResult.ImportedFileDescriptors);
    }

    [TestMethod]
    public void TestImportSvlFile()
    {
      const string testName = "File Import 2";
      msg.Title(testName, "Create standard project and customer then upload svl file");
      var ts = new TestSupport();
      var importFile = new ImportFile();
      var legacyProjectId = ts.SetLegacyProjectId();
      var projectUid = Guid.NewGuid().ToString();
      var customerUid = Guid.NewGuid();
      var tccOrg = Guid.NewGuid();
      var subscriptionUid = Guid.NewGuid();
      var startDateTime = ts.FirstEventDate;
      var endDateTime = new DateTime(9999, 12, 31);
      var startDate = startDateTime.ToString("yyyy-MM-dd");
      var endDate = endDateTime.ToString("yyyy-MM-dd");
      const string geometryWkt = "POLYGON((-121.347189366818 38.8361907402694,-121.349260032177 38.8361656688414,-121.349217116833 38.8387897637231,-121.347275197506 38.8387145521594,-121.347189366818 38.8361907402694,-121.347189366818 38.8361907402694))";
      var eventsArray = new[] {
       "| TableName           | EventDate   | CustomerUID   | Name       | fk_CustomerTypeID | SubscriptionUID   | fk_CustomerUID | fk_ServiceTypeID | StartDate   | EndDate        | fk_ProjectUID | TCCOrgID | fk_SubscriptionUID |",
      $"| Customer            | 0d+09:00:00 | {customerUid} | {testName} | 1                 |                   |                |                  |             |                |               |          |                    |",
      $"| CustomerTccOrg      | 0d+09:00:00 | {customerUid} |            |                   |                   |                |                  |             |                |               | {tccOrg} |                    |",
      $"| Subscription        | 0d+09:10:00 |               |            |                   | {subscriptionUid} | {customerUid}  | 19               | {startDate} | {endDate}      |               |          |                    |",
      $"| ProjectSubscription | 0d+09:20:00 |               |            |                   |                   |                |                  | {startDate} |                | {projectUid}  |          | {subscriptionUid}  |"};
      ts.PublishEventCollection(eventsArray);

      ts.IsPublishToWebApi = true;
      var projectEventArray = new[] {
       "| EventType          | EventDate   | ProjectUID   | ProjectID         | ProjectName | ProjectType | ProjectTimezone           | ProjectStartDate                            | ProjectEndDate                             | ProjectBoundary | CustomerUID   | CustomerID        | IsArchived | CoordinateSystem      | Description |",
      $"| CreateProjectEvent | 0d+09:00:00 | {projectUid} | {legacyProjectId} | {testName}  | Standard    | New Zealand Standard Time | {startDateTime:yyyy-MM-ddTHH:mm:ss.fffffff} | {endDateTime:yyyy-MM-ddTHH:mm:ss.fffffff}  | {geometryWkt}   | {customerUid} | {legacyProjectId} | false      | BootCampDimensions.dc | {testName}  |"};
      ts.PublishEventCollection(projectEventArray);
      var importFileArray = new[] {
       "| EventType              | ProjectUid   | CustomerUid   | Name                      | ImportedFileType | FileCreatedUtc  | FileUpdatedUtc             | ImportedBy                 | IsActivated | MinZoomLevel | MaxZoomLevel |",
      $"| ImportedFileDescriptor | {projectUid} | {customerUid} | {TestFile.TestAlignment1} | 3                | {startDateTime} | {startDateTime.AddDays(5)} | testProjectMDM@trimble.com | true        | 15           | 19           |"};
      var filesResult = importFile.SendImportedFilesToWebApi(ts, importFileArray, 1);
      ts.CompareTheActualImportFileWithExpected(filesResult.ImportedFileDescriptor, importFile.expectedImportFileDescriptorSingleResult.ImportedFileDescriptor, true);
    }

    [TestMethod]
    public void TestImport2SvlFiles()
    {
      const string testName = "File Import 3";
      msg.Title(testName, "Create standard project and customer then upload two alignment files");
      var ts = new TestSupport();
      var importFile = new ImportFile();
      var legacyProjectId = ts.SetLegacyProjectId();
      var projectUid = Guid.NewGuid().ToString();
      var customerUid = Guid.NewGuid();
      var tccOrg = Guid.NewGuid();
      var subscriptionUid = Guid.NewGuid();
      var startDateTime = ts.FirstEventDate;
      var endDateTime = new DateTime(9999, 12, 31);
      var startDate = startDateTime.ToString("yyyy-MM-dd");
      var endDate = endDateTime.ToString("yyyy-MM-dd");
      const string geometryWkt = "POLYGON((-121.347189366818 38.8361907402694,-121.349260032177 38.8361656688414,-121.349217116833 38.8387897637231,-121.347275197506 38.8387145521594,-121.347189366818 38.8361907402694,-121.347189366818 38.8361907402694))";
      var eventsArray = new[] {
         "| TableName           | EventDate   | CustomerUID   | Name       | fk_CustomerTypeID | SubscriptionUID   | fk_CustomerUID | fk_ServiceTypeID | StartDate   | EndDate        | fk_ProjectUID | TCCOrgID | fk_SubscriptionUID |",
        $"| Customer            | 0d+09:00:00 | {customerUid} | {testName} | 1                 |                   |                |                  |             |                |               |          |                    |",
        $"| CustomerTccOrg      | 0d+09:00:00 | {customerUid} |            |                   |                   |                |                  |             |                |               | {tccOrg} |                    |",
        $"| Subscription        | 0d+09:10:00 |               |            |                   | {subscriptionUid} | {customerUid}  | 19               | {startDate} | {endDate}      |               |          |                    |",
        $"| ProjectSubscription | 0d+09:20:00 |               |            |                   |                   |                |                  | {startDate} |                | {projectUid}  |          | {subscriptionUid}  |"};
      ts.PublishEventCollection(eventsArray);

      ts.IsPublishToWebApi = true;
      var projectEventArray = new[] {
        "| EventType          | EventDate   | ProjectUID   | ProjectID         | ProjectName | ProjectType | ProjectTimezone           | ProjectStartDate                            | ProjectEndDate                             | ProjectBoundary | CustomerUID   | CustomerID        | IsArchived | CoordinateSystem      | Description |",
       $"| CreateProjectEvent | 0d+09:00:00 | {projectUid} | {legacyProjectId} | {testName}  | Standard    | New Zealand Standard Time | {startDateTime:yyyy-MM-ddTHH:mm:ss.fffffff} | {endDateTime:yyyy-MM-ddTHH:mm:ss.fffffff}  | {geometryWkt}   | {customerUid} | {legacyProjectId} | false      | BootCampDimensions.dc | {testName}  |"};
      ts.PublishEventCollection(projectEventArray);

      var importFileArray = new[] {
        "| EventType              | ProjectUid   | CustomerUid   | Name                      | ImportedFileType | FileCreatedUtc  | FileUpdatedUtc             | ImportedBy                 | IsActivated | MinZoomLevel | MaxZoomLevel |",
       $"| ImportedFileDescriptor | {projectUid} | {customerUid} | {TestFile.TestAlignment1} | 3                | {startDateTime} | {startDateTime.AddDays(5)} | testProjectMDM@trimble.com | true        | 15           | 19           |",
       $"| ImportedFileDescriptor | {projectUid} | {customerUid} | {TestFile.TestAlignment2} | 3                | {startDateTime} | {startDateTime.AddDays(5)} | testProjectMDM@trimble.com | true        | 15           | 19           |"};
      var filesResult = importFile.SendImportedFilesToWebApi(ts, importFileArray, 1);
      var expectedResult1 = importFile.expectedImportFileDescriptorSingleResult.ImportedFileDescriptor;
      ts.CompareTheActualImportFileWithExpected(filesResult.ImportedFileDescriptor, expectedResult1, true);

      var filesResult2 = importFile.SendImportedFilesToWebApi(ts, importFileArray, 2);
      var expectedResult2 = importFile.expectedImportFileDescriptorSingleResult.ImportedFileDescriptor;
      ts.CompareTheActualImportFileWithExpected(filesResult2.ImportedFileDescriptor, expectedResult2, true);

      var importFileList = importFile.GetImportedFilesFromWebApi(ts.GetBaseUri() + $"api/v4/importedfiles?projectUid={projectUid}", customerUid);
      Assert.IsTrue(importFileList.ImportedFileDescriptors.Count == 2, "Expected 2 imported files but got " + importFileList.ImportedFileDescriptors.Count);
      ts.CompareTheActualImportFileWithExpected(importFileList.ImportedFileDescriptors[0], expectedResult1, true);
      ts.CompareTheActualImportFileWithExpected(importFileList.ImportedFileDescriptors[1], expectedResult2, true);
    }

    [TestMethod]
    public void TestImportSurfaceFile()
    {
      const string testName = "File Import 4";
      msg.Title(testName, "Create standard project and customer then upload design surface file");
      var ts = new TestSupport();
      var importFile = new ImportFile();
      var legacyProjectId = ts.SetLegacyProjectId();
      var projectUid = Guid.NewGuid().ToString();
      var customerUid = Guid.NewGuid();
      var tccOrg = Guid.NewGuid();
      var subscriptionUid = Guid.NewGuid();
      var startDateTime = ts.FirstEventDate.ToUniversalTime();
      var endDateTime = new DateTime(9999, 12, 31);
      var startDate = startDateTime.ToString("yyyy-MM-dd");
      var endDate = endDateTime.ToString("yyyy-MM-dd");
      const string geometryWkt = "POLYGON((-121.347189366818 38.8361907402694,-121.349260032177 38.8361656688414,-121.349217116833 38.8387897637231,-121.347275197506 38.8387145521594,-121.347189366818 38.8361907402694,-121.347189366818 38.8361907402694))";
      var eventsArray = new[] {
        "| TableName           | EventDate   | CustomerUID   | Name       | fk_CustomerTypeID | SubscriptionUID   | fk_CustomerUID | fk_ServiceTypeID | StartDate   | EndDate        | fk_ProjectUID | TCCOrgID | fk_SubscriptionUID |",
        $"| Customer            | 0d+09:00:00 | {customerUid} | {testName} | 1                 |                   |                |                  |             |                |               |          |                    |",
        $"| CustomerTccOrg      | 0d+09:00:00 | {customerUid} |            |                   |                   |                |                  |             |                |               | {tccOrg} |                    |",
        $"| Subscription        | 0d+09:10:00 |               |            |                   | {subscriptionUid} | {customerUid}  | 19               | {startDate} | {endDate}      |               |          |                    |",
        $"| ProjectSubscription | 0d+09:20:00 |               |            |                   |                   |                |                  | {startDate} |                | {projectUid}  |          | {subscriptionUid}  |"};
      ts.PublishEventCollection(eventsArray);

      ts.IsPublishToWebApi = true;
      var projectEventArray = new[] {
         "| EventType          | EventDate   | ProjectUID   | ProjectID         | ProjectName | ProjectType | ProjectTimezone           | ProjectStartDate                            | ProjectEndDate                             | ProjectBoundary | CustomerUID   | CustomerID        | IsArchived | CoordinateSystem      | Description |",
        $"| CreateProjectEvent | 0d+09:00:00 | {projectUid} | {legacyProjectId} | {testName}  | Standard    | New Zealand Standard Time | {startDateTime:yyyy-MM-ddTHH:mm:ss.fffffff} | {endDateTime:yyyy-MM-ddTHH:mm:ss.fffffff}  | {geometryWkt}   | {customerUid} | {legacyProjectId} | false      | BootCampDimensions.dc | {testName}  |"};
      ts.PublishEventCollection(projectEventArray);
      var importFileArray = new[] {
         "| EventType              | ProjectUid   | CustomerUid   | Name                          | ImportedFileType | FileCreatedUtc  | FileUpdatedUtc             | ImportedBy                 | IsActivated | MinZoomLevel | MaxZoomLevel |",
        $"| ImportedFileDescriptor | {projectUid} | {customerUid} | {TestFile.TestDesignSurface1} | 1                | {startDateTime} | {startDateTime.AddDays(5)} | testProjectMDM@trimble.com | true        | 15           | 19           |"};
      var filesResult = importFile.SendImportedFilesToWebApi(ts, importFileArray, 1);
      ts.CompareTheActualImportFileWithExpected(filesResult.ImportedFileDescriptor, importFile.expectedImportFileDescriptorSingleResult.ImportedFileDescriptor, true);
    }

    [TestMethod]
    public void TestImport2SurfaceFiles()
    {
      const string testName = "File Import 5";
      msg.Title(testName, "Create standard project and customer then upload two Design surface files");
      var ts = new TestSupport();
      var importFile = new ImportFile();
      var legacyProjectId = ts.SetLegacyProjectId();
      var projectUid = Guid.NewGuid().ToString();
      var customerUid = Guid.NewGuid();
      var tccOrg = Guid.NewGuid();
      var subscriptionUid = Guid.NewGuid();
      var startDateTime = ts.FirstEventDate;
      var endDateTime = new DateTime(9999, 12, 31);
      var startDate = startDateTime.ToString("yyyy-MM-dd");
      var endDate = endDateTime.ToString("yyyy-MM-dd");
      const string geometryWkt = "POLYGON((-121.347189366818 38.8361907402694,-121.349260032177 38.8361656688414,-121.349217116833 38.8387897637231,-121.347275197506 38.8387145521594,-121.347189366818 38.8361907402694,-121.347189366818 38.8361907402694))";
      var eventsArray = new[] {
         "| TableName           | EventDate   | CustomerUID   | Name       | fk_CustomerTypeID | SubscriptionUID   | fk_CustomerUID | fk_ServiceTypeID | StartDate   | EndDate        | fk_ProjectUID | TCCOrgID | fk_SubscriptionUID |",
        $"| Customer            | 0d+09:00:00 | {customerUid} | {testName} | 1                 |                   |                |                  |             |                |               |          |                    |",
        $"| CustomerTccOrg      | 0d+09:00:00 | {customerUid} |            |                   |                   |                |                  |             |                |               | {tccOrg} |                    |",
        $"| Subscription        | 0d+09:10:00 |               |            |                   | {subscriptionUid} | {customerUid}  | 19               | {startDate} | {endDate}      |               |          |                    |",
        $"| ProjectSubscription | 0d+09:20:00 |               |            |                   |                   |                |                  | {startDate} |                | {projectUid}  |          | {subscriptionUid}  |"};
      ts.PublishEventCollection(eventsArray);

      ts.IsPublishToWebApi = true;
      var projectEventArray = new[] {
         "| EventType          | EventDate   | ProjectUID   | ProjectID         | ProjectName | ProjectType | ProjectTimezone           | ProjectStartDate                            | ProjectEndDate                             | ProjectBoundary | CustomerUID   | CustomerID        |IsArchived | CoordinateSystem      | Description |",
        $"| CreateProjectEvent | 0d+09:00:00 | {projectUid} | {legacyProjectId} | {testName}  | Standard    | New Zealand Standard Time | {startDateTime:yyyy-MM-ddTHH:mm:ss.fffffff} | {endDateTime:yyyy-MM-ddTHH:mm:ss.fffffff}  | {geometryWkt}   | {customerUid} | {legacyProjectId} |false      | BootCampDimensions.dc | {testName}  |"};
      ts.PublishEventCollection(projectEventArray);

      var importFileArray = new[] {
         "| EventType              | ProjectUid   | CustomerUid   | Name                          | ImportedFileType | FileCreatedUtc  | FileUpdatedUtc             | ImportedBy                 | IsActivated | MinZoomLevel | MaxZoomLevel |",
        $"| ImportedFileDescriptor | {projectUid} | {customerUid} | {TestFile.TestDesignSurface1} | 1                | {startDateTime} | {startDateTime.AddDays(5)} | testProjectMDM@trimble.com | true        | 15           | 19           |",
        $"| ImportedFileDescriptor | {projectUid} | {customerUid} | {TestFile.TestDesignSurface2} | 1                | {startDateTime} | {startDateTime.AddDays(5)} | testProjectMDM@trimble.com | true        | 15           | 19           |"};
      var filesResult = importFile.SendImportedFilesToWebApi(ts, importFileArray, 1);
      var expectedResult1 = importFile.expectedImportFileDescriptorSingleResult.ImportedFileDescriptor;
      ts.CompareTheActualImportFileWithExpected(filesResult.ImportedFileDescriptor, expectedResult1, true);

      var filesResult2 = importFile.SendImportedFilesToWebApi(ts, importFileArray, 2);
      var expectedResult2 = importFile.expectedImportFileDescriptorSingleResult.ImportedFileDescriptor;
      ts.CompareTheActualImportFileWithExpected(filesResult2.ImportedFileDescriptor, expectedResult2, true);

      var importFileList = importFile.GetImportedFilesFromWebApi(ts.GetBaseUri() + $"api/v4/importedfiles?projectUid={projectUid}", customerUid);
      Assert.IsTrue(importFileList.ImportedFileDescriptors.Count == 2, "Expected 2 imported files but got " + importFileList.ImportedFileDescriptors.Count);
      ts.CompareTheActualImportFileWithExpected(importFileList.ImportedFileDescriptors[0], expectedResult1, true);
      ts.CompareTheActualImportFileWithExpected(importFileList.ImportedFileDescriptors[1], expectedResult2, true);
    }

    [TestMethod]
    public void TestImportSurveyedSurfaceFile()
    {
      const string testName = "File Import 4";
      msg.Title(testName, "Create standard project and customer then upload surveyed surface file");
      var ts = new TestSupport();
      var importFile = new ImportFile();
      var legacyProjectId = ts.SetLegacyProjectId();
      var projectUid = Guid.NewGuid().ToString();
      var customerUid = Guid.NewGuid();
      var tccOrg = Guid.NewGuid();
      var subscriptionUid = Guid.NewGuid();
      var startDateTime = ts.FirstEventDate;
      var endDateTime = new DateTime(9999, 12, 31);
      var startDate = startDateTime.ToString("yyyy-MM-dd");
      var endDate = endDateTime.ToString("yyyy-MM-dd");
      const string geometryWkt = "POLYGON((-121.347189366818 38.8361907402694,-121.349260032177 38.8361656688414,-121.349217116833 38.8387897637231,-121.347275197506 38.8387145521594,-121.347189366818 38.8361907402694,-121.347189366818 38.8361907402694))";
      var eventsArray = new[] {
        "| TableName            | EventDate   | CustomerUID   | Name       | fk_CustomerTypeID | SubscriptionUID   | fk_CustomerUID | fk_ServiceTypeID | StartDate   | EndDate        | fk_ProjectUID | TCCOrgID | fk_SubscriptionUID |",
        $"| Customer            | 0d+09:00:00 | {customerUid} | {testName} | 1                 |                   |                |                  |             |                |               |          |                    |",
        $"| CustomerTccOrg      | 0d+09:00:00 | {customerUid} |            |                   |                   |                |                  |             |                |               | {tccOrg} |                    |",
        $"| Subscription        | 0d+09:10:00 |               |            |                   | {subscriptionUid} | {customerUid}  | 19               | {startDate} | {endDate}      |               |          |                    |",
        $"| ProjectSubscription | 0d+09:20:00 |               |            |                   |                   |                |                  | {startDate} |                | {projectUid}  |          | {subscriptionUid}  |"};
      ts.PublishEventCollection(eventsArray);

      ts.IsPublishToWebApi = true;
      var projectEventArray = new[] {
         "| EventType          | EventDate   | ProjectUID   | ProjectID         | ProjectName | ProjectType | ProjectTimezone           | ProjectStartDate                            | ProjectEndDate                             | ProjectBoundary | CustomerUID   | CustomerID        |IsArchived | CoordinateSystem      | Description |",
        $"| CreateProjectEvent | 0d+09:00:00 | {projectUid} | {legacyProjectId} | {testName}  | Standard    | New Zealand Standard Time | {startDateTime:yyyy-MM-ddTHH:mm:ss.fffffff} | {endDateTime:yyyy-MM-ddTHH:mm:ss.fffffff}  | {geometryWkt}   | {customerUid} | {legacyProjectId} |false      | BootCampDimensions.dc | {testName}  |"};
      ts.PublishEventCollection(projectEventArray);
      var importFileArray = new[] {
         "| EventType              | ProjectUid   | CustomerUid   | Name                          | ImportedFileType | FileCreatedUtc  | FileUpdatedUtc             | ImportedBy                 | SurveyedUtc     | IsActivated | MinZoomLevel | MaxZoomLevel |",
        $"| ImportedFileDescriptor | {projectUid} | {customerUid} | {TestFile.TestDesignSurface1} | 2                | {startDateTime} | {startDateTime.AddDays(5)} | testProjectMDM@trimble.com | {startDateTime} | true        | 0            | 0            |"};
      var filesResult = importFile.SendImportedFilesToWebApi(ts, importFileArray, 1);
      ts.CompareTheActualImportFileWithExpected(filesResult.ImportedFileDescriptor, importFile.expectedImportFileDescriptorSingleResult.ImportedFileDescriptor, true);
    }

    [TestMethod]
    public void TestImport2SurveyedSurfaceFiles()
    {
      const string testName = "File Import 5";
      msg.Title(testName, "Create standard project and customer then upload two Surveryed surface files");
      var ts = new TestSupport();
      var importFile = new ImportFile();
      var legacyProjectId = ts.SetLegacyProjectId();
      var projectUid = Guid.NewGuid().ToString();
      var customerUid = Guid.NewGuid();
      var tccOrg = Guid.NewGuid();
      var subscriptionUid = Guid.NewGuid();
      var startDateTime = ts.FirstEventDate;
      var endDateTime = new DateTime(9999, 12, 31);
      var startDate = startDateTime.ToString("yyyy-MM-dd");
      var endDate = endDateTime.ToString("yyyy-MM-dd");
      const string geometryWkt = "POLYGON((-121.347189366818 38.8361907402694,-121.349260032177 38.8361656688414,-121.349217116833 38.8387897637231,-121.347275197506 38.8387145521594,-121.347189366818 38.8361907402694,-121.347189366818 38.8361907402694))";
      var eventsArray = new[] {
        "| TableName           | EventDate   | CustomerUID   | Name      | fk_CustomerTypeID | SubscriptionUID   | fk_CustomerUID | fk_ServiceTypeID | StartDate   | EndDate        | fk_ProjectUID | TCCOrgID | fk_SubscriptionUID |",
      $"| Customer            | 0d+09:00:00 | {customerUid} | {testName} | 1                 |                   |                |                  |             |                |               |          |                    |",
      $"| CustomerTccOrg      | 0d+09:00:00 | {customerUid} |            |                   |                   |                |                  |             |                |               | {tccOrg} |                    |",
      $"| Subscription        | 0d+09:10:00 |               |            |                   | {subscriptionUid} | {customerUid}  | 19               | {startDate} | {endDate}      |               |          |                    |",
      $"| ProjectSubscription | 0d+09:20:00 |               |            |                   |                   |                |                  | {startDate} |                | {projectUid}  |          | {subscriptionUid}  |"};
      ts.PublishEventCollection(eventsArray);

      ts.IsPublishToWebApi = true;
      var projectEventArray = new[] {
       "| EventType          | EventDate   | ProjectUID   | ProjectID         | ProjectName | ProjectType | ProjectTimezone           | ProjectStartDate                            | ProjectEndDate                             | ProjectBoundary | CustomerUID   | CustomerID        |IsArchived | CoordinateSystem      | Description |",
      $"| CreateProjectEvent | 0d+09:00:00 | {projectUid} | {legacyProjectId} | {testName}  | Standard    | New Zealand Standard Time | {startDateTime:yyyy-MM-ddTHH:mm:ss.fffffff} | {endDateTime:yyyy-MM-ddTHH:mm:ss.fffffff}  | {geometryWkt}   | {customerUid} | {legacyProjectId} |false      | BootCampDimensions.dc | {testName}  |"};
      ts.PublishEventCollection(projectEventArray);

      var importFileArray = new[] {
       "| EventType              | ProjectUid   | CustomerUid   | Name                          | ImportedFileType | FileCreatedUtc  | FileUpdatedUtc             | ImportedBy                 | SurveyedUtc     | IsActivated | MinZoomLevel | MaxZoomLevel |",
      $"| ImportedFileDescriptor | {projectUid} | {customerUid} | {TestFile.TestDesignSurface1} | 2                | {startDateTime} | {startDateTime.AddDays(5)} | testProjectMDM@trimble.com | {startDateTime} | true        | 0            | 0            |",
      $"| ImportedFileDescriptor | {projectUid} | {customerUid} | {TestFile.TestDesignSurface2} | 2                | {startDateTime} | {startDateTime.AddDays(5)} | testProjectMDM@trimble.com | {startDateTime} | true        | 0            | 0            |"};
      var filesResult = importFile.SendImportedFilesToWebApi(ts, importFileArray, 1);
      var expectedResult1 = importFile.expectedImportFileDescriptorSingleResult.ImportedFileDescriptor;
      ts.CompareTheActualImportFileWithExpected(filesResult.ImportedFileDescriptor, expectedResult1, true);

      var filesResult2 = importFile.SendImportedFilesToWebApi(ts, importFileArray, 2);
      var expectedResult2 = importFile.expectedImportFileDescriptorSingleResult.ImportedFileDescriptor;
      ts.CompareTheActualImportFileWithExpected(filesResult2.ImportedFileDescriptor, expectedResult2, true);

      var importFileList = importFile.GetImportedFilesFromWebApi(ts.GetBaseUri() + $"api/v4/importedfiles?projectUid={projectUid}", customerUid);
      Assert.IsTrue(importFileList.ImportedFileDescriptors.Count == 2, "Expected 2 imported files but got " + importFileList.ImportedFileDescriptors.Count);
      ts.CompareTheActualImportFileWithExpected(importFileList.ImportedFileDescriptors[0], expectedResult1, true);
      ts.CompareTheActualImportFileWithExpected(importFileList.ImportedFileDescriptors[1], expectedResult2, true);
    }

    [TestMethod]
    public void TestImportTheSameFileTwice()
    {
      const string testName = "File Import 6";
      msg.Title(testName, "Create standard project then upload two alignment files that are the same name and content");
      var ts = new TestSupport();
      var importFile = new ImportFile();
      var legacyProjectId = ts.SetLegacyProjectId();
      var projectUid = Guid.NewGuid().ToString();
      var customerUid = Guid.NewGuid();
      var tccOrg = Guid.NewGuid();
      var subscriptionUid = Guid.NewGuid();
      var startDateTime = ts.FirstEventDate;
      var endDateTime = new DateTime(9999, 12, 31);
      var startDate = startDateTime.ToString("yyyy-MM-dd");
      var endDate = endDateTime.ToString("yyyy-MM-dd");
      const string geometryWkt = "POLYGON((-121.347189366818 38.8361907402694,-121.349260032177 38.8361656688414,-121.349217116833 38.8387897637231,-121.347275197506 38.8387145521594,-121.347189366818 38.8361907402694,-121.347189366818 38.8361907402694))";
      var eventsArray = new[] {
        "| TableName           | EventDate   | CustomerUID   | Name       | fk_CustomerTypeID | SubscriptionUID   | fk_CustomerUID | fk_ServiceTypeID | StartDate   | EndDate        | fk_ProjectUID | TCCOrgID | fk_SubscriptionUID |",
       $"| Customer            | 0d+09:00:00 | {customerUid} | {testName} | 1                 |                   |                |                  |             |                |               |          |                    |",
       $"| CustomerTccOrg      | 0d+09:00:00 | {customerUid} |            |                   |                   |                |                  |             |                |               | {tccOrg} |                    |",
       $"| Subscription        | 0d+09:10:00 |               |            |                   | {subscriptionUid} | {customerUid}  | 19               | {startDate} | {endDate}      |               |          |                    |",
       $"| ProjectSubscription | 0d+09:20:00 |               |            |                   |                   |                |                  | {startDate} |                | {projectUid}  |          | {subscriptionUid}  |"};
      ts.PublishEventCollection(eventsArray);

      ts.IsPublishToWebApi = true;
      var projectEventArray = new[] {
        "| EventType          | EventDate   | ProjectUID   | ProjectID         | ProjectName | ProjectType | ProjectTimezone           | ProjectStartDate                            | ProjectEndDate                             | ProjectBoundary | CustomerUID   | CustomerID        |IsArchived | CoordinateSystem      | Description |",
       $"| CreateProjectEvent | 0d+09:00:00 | {projectUid} | {legacyProjectId} | {testName}  | Standard    | New Zealand Standard Time | {startDateTime:yyyy-MM-ddTHH:mm:ss.fffffff} | {endDateTime:yyyy-MM-ddTHH:mm:ss.fffffff}  | {geometryWkt}   | {customerUid} | {legacyProjectId} |false      | BootCampDimensions.dc | {testName}  |"};
      ts.PublishEventCollection(projectEventArray);

      var importFileArray = new[] {
       "| EventType              | ProjectUid   | CustomerUid   | Name                      | ImportedFileType | FileCreatedUtc  | FileUpdatedUtc             | ImportedBy                 | IsActivated | MinZoomLevel | MaxZoomLevel |",
      $"| ImportedFileDescriptor | {projectUid} | {customerUid} | {TestFile.TestAlignment1} | 3                | {startDateTime} | {startDateTime.AddDays(5)} | testProjectMDM@trimble.com | true        | 15           | 19           |",
      $"| ImportedFileDescriptor | {projectUid} | {customerUid} | {TestFile.TestAlignment1} | 3                | {startDateTime} | {startDateTime.AddDays(5)} | testProjectMDM@trimble.com | true        | 15           | 19           |"};
      var filesResult = importFile.SendImportedFilesToWebApi(ts, importFileArray, 1);
      var expectedResult1 = importFile.expectedImportFileDescriptorSingleResult.ImportedFileDescriptor;
      ts.CompareTheActualImportFileWithExpected(filesResult.ImportedFileDescriptor, expectedResult1, true);
      Assert.AreEqual(1, filesResult.ImportedFileDescriptor.ImportedFileHistory.Count, "Expected 1 imported file History but got " + filesResult.ImportedFileDescriptor.ImportedFileHistory.Count);

      var filesResult2 = importFile.SendImportedFilesToWebApi(ts, importFileArray, 2);
      Assert.IsTrue(filesResult2.Message == "CreateImportedFileV4. The file has already been created.", "Expecting a message: CreateImportedFileV4.The file has already been created.");
      var importFileList = importFile.GetImportedFilesFromWebApi(ts.GetBaseUri() + $"api/v4/importedfiles?projectUid={projectUid}", customerUid);
      Assert.IsTrue(importFileList.ImportedFileDescriptors.Count == 1, "Expected 1 imported files but got " + importFileList.ImportedFileDescriptors.Count);
      ts.CompareTheActualImportFileWithExpected(importFileList.ImportedFileDescriptors[0], expectedResult1, true);
      Assert.AreEqual(1, filesResult.ImportedFileDescriptor.ImportedFileHistory.Count, "Expected 1 imported file History but got " + filesResult.ImportedFileDescriptor.ImportedFileHistory.Count);
    }

    [TestMethod]
    public void TestImportANewFileThenUpdateTheAlignmentFile()
    {
      const string testName = "File Import 7";
      msg.Title(testName, "Create standard project then upload a new alignment file. Then update alignment file");
      var ts = new TestSupport();
      var importFile = new ImportFile();
      var legacyProjectId = ts.SetLegacyProjectId();
      var projectUid = Guid.NewGuid().ToString();
      var customerUid = Guid.NewGuid();
      var tccOrg = Guid.NewGuid();
      var subscriptionUid = Guid.NewGuid();
      var startDateTime = ts.FirstEventDate;
      var endDateTime = new DateTime(9999, 12, 31);
      var startDate = startDateTime.ToString("yyyy-MM-dd");
      var endDate = endDateTime.ToString("yyyy-MM-dd");
      const string geometryWkt = "POLYGON((-121.347189366818 38.8361907402694,-121.349260032177 38.8361656688414,-121.349217116833 38.8387897637231,-121.347275197506 38.8387145521594,-121.347189366818 38.8361907402694,-121.347189366818 38.8361907402694))";
      var eventsArray = new[] {
        "| TableName           | EventDate   | CustomerUID   | Name       | fk_CustomerTypeID | SubscriptionUID   | fk_CustomerUID | fk_ServiceTypeID | StartDate   | EndDate        | fk_ProjectUID | TCCOrgID | fk_SubscriptionUID |",
       $"| Customer            | 0d+09:00:00 | {customerUid} | {testName} | 1                 |                   |                |                  |             |                |               |          |                    |",
       $"| CustomerTccOrg      | 0d+09:00:00 | {customerUid} |            |                   |                   |                |                  |             |                |               | {tccOrg} |                    |",
       $"| Subscription        | 0d+09:10:00 |               |            |                   | {subscriptionUid} | {customerUid}  | 19               | {startDate} | {endDate}      |               |          |                    |",
       $"| ProjectSubscription | 0d+09:20:00 |               |            |                   |                   |                |                  | {startDate} |                | {projectUid}  |          | {subscriptionUid}  |"};
      ts.PublishEventCollection(eventsArray);

      ts.IsPublishToWebApi = true;
      var projectEventArray = new[] {
        "| EventType          | EventDate   | ProjectUID   | ProjectID         | ProjectName | ProjectType | ProjectTimezone           | ProjectStartDate                            | ProjectEndDate                             | ProjectBoundary | CustomerUID   | CustomerID        |IsArchived | CoordinateSystem      | Description |",
       $"| CreateProjectEvent | 0d+09:00:00 | {projectUid} | {legacyProjectId} | {testName}  | Standard    | New Zealand Standard Time | {startDateTime:yyyy-MM-ddTHH:mm:ss.fffffff} | {endDateTime:yyyy-MM-ddTHH:mm:ss.fffffff}  | {geometryWkt}   | {customerUid} | {legacyProjectId} |false      | BootCampDimensions.dc | {testName}  |"};
      ts.PublishEventCollection(projectEventArray);

      var importFileArray = new[] {
        "| EventType              | ProjectUid   | CustomerUid   | Name                      | ImportedFileType | FileCreatedUtc  | FileUpdatedUtc             | ImportedBy                 | IsActivated | MinZoomLevel | MaxZoomLevel |",
       $"| ImportedFileDescriptor | {projectUid} | {customerUid} | {TestFile.TestAlignment1} | 3                | {startDateTime} | {startDateTime.AddDays(5)} | testProjectMDM@trimble.com | true        | 15           | 19           |",
       $"| ImportedFileDescriptor | {projectUid} | {customerUid} | {TestFile.TestAlignment1} | 3                | {startDateTime} | {startDateTime.AddDays(5)} | testProjectMDM@trimble.com | true        | 15           | 19           |"};
      var filesResult = importFile.SendImportedFilesToWebApi(ts, importFileArray, 1);
      var expectedResult1 = importFile.expectedImportFileDescriptorSingleResult.ImportedFileDescriptor;
      ts.CompareTheActualImportFileWithExpected(filesResult.ImportedFileDescriptor, expectedResult1, true);

      var filesResult2 = importFile.SendImportedFilesToWebApi(ts, importFileArray, 2, "PUT");
      var expectedResult2 = importFile.expectedImportFileDescriptorSingleResult.ImportedFileDescriptor;
      var importFileList = importFile.GetImportedFilesFromWebApi(ts.GetBaseUri() + $"api/v4/importedfiles?projectUid={projectUid}", customerUid);
      Assert.IsTrue(importFileList.ImportedFileDescriptors.Count == 1, "Expected 1 imported files but got " + importFileList.ImportedFileDescriptors.Count);
      ts.CompareTheActualImportFileWithExpected(importFileList.ImportedFileDescriptors[0], expectedResult2, true);
    }


    [TestMethod]
    public void TestImportANewFileThenUpdateTheDesignSurfaceFile()
    {
      const string testName = "File Import 7";
      msg.Title(testName, "Create standard project then upload a new design surface file. Then update design surface file");
      var ts = new TestSupport();
      var importFile = new ImportFile();
      var legacyProjectId = ts.SetLegacyProjectId();
      var projectUid = Guid.NewGuid().ToString();
      var customerUid = Guid.NewGuid();
      var tccOrg = Guid.NewGuid();
      var subscriptionUid = Guid.NewGuid();
      var startDateTime = ts.FirstEventDate;
      var endDateTime = new DateTime(9999, 12, 31);
      var startDate = startDateTime.ToString("yyyy-MM-dd");
      var endDate = endDateTime.ToString("yyyy-MM-dd");
      const string geometryWkt = "POLYGON((-121.347189366818 38.8361907402694,-121.349260032177 38.8361656688414,-121.349217116833 38.8387897637231,-121.347275197506 38.8387145521594,-121.347189366818 38.8361907402694,-121.347189366818 38.8361907402694))";
      var eventsArray = new[] {
        "| TableName           | EventDate   | CustomerUID   | Name       | fk_CustomerTypeID | SubscriptionUID   | fk_CustomerUID | fk_ServiceTypeID | StartDate   | EndDate        | fk_ProjectUID | TCCOrgID | fk_SubscriptionUID |",
       $"| Customer            | 0d+09:00:00 | {customerUid} | {testName} | 1                 |                   |                |                  |             |                |               |          |                    |",
       $"| CustomerTccOrg      | 0d+09:00:00 | {customerUid} |            |                   |                   |                |                  |             |                |               | {tccOrg} |                    |",
       $"| Subscription        | 0d+09:10:00 |               |            |                   | {subscriptionUid} | {customerUid}  | 19               | {startDate} | {endDate}      |               |          |                    |",
       $"| ProjectSubscription | 0d+09:20:00 |               |            |                   |                   |                |                  | {startDate} |                | {projectUid}  |          | {subscriptionUid}  |"};
      ts.PublishEventCollection(eventsArray);

      ts.IsPublishToWebApi = true;
      var projectEventArray = new[] {
        "| EventType          | EventDate   | ProjectUID   | ProjectID         | ProjectName | ProjectType | ProjectTimezone           | ProjectStartDate                            | ProjectEndDate                             | ProjectBoundary | CustomerUID   | CustomerID        |IsArchived | CoordinateSystem      | Description |",
       $"| CreateProjectEvent | 0d+09:00:00 | {projectUid} | {legacyProjectId} | {testName}  | Standard    | New Zealand Standard Time | {startDateTime:yyyy-MM-ddTHH:mm:ss.fffffff} | {endDateTime:yyyy-MM-ddTHH:mm:ss.fffffff}  | {geometryWkt}   | {customerUid} | {legacyProjectId} |false      | BootCampDimensions.dc | {testName}  |"};
      ts.PublishEventCollection(projectEventArray);

      var importFileArray = new[] {
       "| EventType              | ProjectUid   | CustomerUid   | Name                          | ImportedFileType | FileCreatedUtc              | FileUpdatedUtc              | ImportedBy                 | IsActivated | MinZoomLevel | MaxZoomLevel |", // SurveyedUtc               |",
      $"| ImportedFileDescriptor | {projectUid} | {customerUid} | {TestFile.TestDesignSurface1} | 1                | {startDateTime}             | {startDateTime.AddDays(5)}  | testProjectMDM@trimble.com | true        | 15           | 19           |", // {startDateTime}           |",
      $"| ImportedFileDescriptor | {projectUid} | {customerUid} | {TestFile.TestDesignSurface1} | 1                | {startDateTime.AddDays(10)} | {startDateTime.AddDays(10)} | testProjectMDM@trimble.com | true        | 15           | 19           |" }; // {startDateTime.AddDays(5)}|"};
      var filesResult = importFile.SendImportedFilesToWebApi(ts, importFileArray, 1);
      var expectedResult1 = importFile.expectedImportFileDescriptorSingleResult.ImportedFileDescriptor;
      ts.CompareTheActualImportFileWithExpected(filesResult.ImportedFileDescriptor, expectedResult1, true);
      Assert.AreEqual(1, filesResult.ImportedFileDescriptor.ImportedFileHistory.Count, "Expected 1 imported file History but got " + filesResult.ImportedFileDescriptor.ImportedFileHistory.Count);

      var filesResult2 = importFile.SendImportedFilesToWebApi(ts, importFileArray, 2, "PUT");
      var expectedResult2 = importFile.expectedImportFileDescriptorSingleResult.ImportedFileDescriptor;
      var importFileList = importFile.GetImportedFilesFromWebApi(ts.GetBaseUri() + $"api/v4/importedfiles?projectUid={projectUid}", customerUid);
      Assert.IsTrue(importFileList.ImportedFileDescriptors.Count == 1, "Expected 1 imported files but got " + importFileList.ImportedFileDescriptors.Count);
      ts.CompareTheActualImportFileWithExpected(importFileList.ImportedFileDescriptors[0], expectedResult2, true);
      Assert.AreEqual(2, filesResult2.ImportedFileDescriptor.ImportedFileHistory.Count, "Expected 2 imported file History but got " + filesResult2.ImportedFileDescriptor.ImportedFileHistory.Count);
    }

    [TestMethod]
    public void TestImportANewFileThenUpdateTheDesignSurfaceFile_SameFileDates()
    {
      const string testName = "File Import 7";
      msg.Title(testName, "Create standard project then upload a new design surface file. Then update design surface file however leave same FileDates");
      var ts = new TestSupport();
      var importFile = new ImportFile();
      var legacyProjectId = ts.SetLegacyProjectId();
      var projectUid = Guid.NewGuid().ToString();
      var customerUid = Guid.NewGuid();
      var tccOrg = Guid.NewGuid();
      var subscriptionUid = Guid.NewGuid();
      var startDateTime = ts.FirstEventDate;
      var endDateTime = new DateTime(9999, 12, 31);
      var startDate = startDateTime.ToString("yyyy-MM-dd");
      var endDate = endDateTime.ToString("yyyy-MM-dd");
      const string geometryWkt = "POLYGON((-121.347189366818 38.8361907402694,-121.349260032177 38.8361656688414,-121.349217116833 38.8387897637231,-121.347275197506 38.8387145521594,-121.347189366818 38.8361907402694,-121.347189366818 38.8361907402694))";
      var eventsArray = new[] {
        "| TableName           | EventDate   | CustomerUID   | Name       | fk_CustomerTypeID | SubscriptionUID   | fk_CustomerUID | fk_ServiceTypeID | StartDate   | EndDate        | fk_ProjectUID | TCCOrgID | fk_SubscriptionUID |",
        $"| Customer            | 0d+09:00:00 | {customerUid} | {testName} | 1                 |                   |                |                  |             |                |               |          |                    |",
        $"| CustomerTccOrg      | 0d+09:00:00 | {customerUid} |            |                   |                   |                |                  |             |                |               | {tccOrg} |                    |",
        $"| Subscription        | 0d+09:10:00 |               |            |                   | {subscriptionUid} | {customerUid}  | 19               | {startDate} | {endDate}      |               |          |                    |",
        $"| ProjectSubscription | 0d+09:20:00 |               |            |                   |                   |                |                  | {startDate} |                | {projectUid}  |          | {subscriptionUid}  |"};
      ts.PublishEventCollection(eventsArray);

      ts.IsPublishToWebApi = true;
      var projectEventArray = new[] {
        "| EventType          | EventDate   | ProjectUID   | ProjectID         | ProjectName | ProjectType | ProjectTimezone           | ProjectStartDate                            | ProjectEndDate                             | ProjectBoundary | CustomerUID   | CustomerID        |IsArchived | CoordinateSystem      | Description |",
        $"| CreateProjectEvent | 0d+09:00:00 | {projectUid} | {legacyProjectId} | {testName}  | Standard    | New Zealand Standard Time | {startDateTime:yyyy-MM-ddTHH:mm:ss.fffffff} | {endDateTime:yyyy-MM-ddTHH:mm:ss.fffffff}  | {geometryWkt}   | {customerUid} | {legacyProjectId} |false      | BootCampDimensions.dc | {testName}  |"};
      ts.PublishEventCollection(projectEventArray);

      var importFileArray = new[] {
        "| EventType              | ProjectUid   | CustomerUid   | Name                          | ImportedFileType | FileCreatedUtc              | FileUpdatedUtc              | ImportedBy                 | IsActivated | MinZoomLevel | MaxZoomLevel |", // SurveyedUtc               |",
        $"| ImportedFileDescriptor | {projectUid} | {customerUid} | {TestFile.TestDesignSurface1} | 1                | {startDateTime}            | {startDateTime.AddDays(5)}  | testProjectMDM@trimble.com | true        | 15           | 19           |", // {startDateTime}           |",
        $"| ImportedFileDescriptor | {projectUid} | {customerUid} | {TestFile.TestDesignSurface1} | 1                | {startDateTime}            | {startDateTime.AddDays(5)}  | testProjectMDM@trimble.com | true        | 15           | 19           |", // {startDateTime}|"};
        $"| ImportedFileDescriptor | {projectUid} | {customerUid} | {TestFile.TestDesignSurface1} | 1                | {startDateTime}            | {startDateTime.AddDays(5)}  | testProjectMDM@trimble.comChanged | true  | 15          | 19           |" }; // {startDateTime}|"};

      var filesResult = importFile.SendImportedFilesToWebApi(ts, importFileArray, 1);
      var filesResult2 = importFile.SendImportedFilesToWebApi(ts, importFileArray, 2, "PUT");
      var filesResult3 = importFile.SendImportedFilesToWebApi(ts, importFileArray, 2, "PUT");
      Assert.AreEqual(1, filesResult3.ImportedFileDescriptor.ImportedFileHistory.Count, "Expected 1 imported file History but got " + filesResult3.ImportedFileDescriptor.ImportedFileHistory.Count);
    }

    [TestMethod]
    public void TestImportANewFileThenUpdateTheSurveyedSurfaceFile()
    {
      const string testName = "File Import 7";
      msg.Title(testName, "Create standard project then upload a new surveyed surface file. Then update surveyed surface file");
      var ts = new TestSupport();
      var importFile = new ImportFile();
      var legacyProjectId = ts.SetLegacyProjectId();
      var projectUid = Guid.NewGuid().ToString();
      var customerUid = Guid.NewGuid();
      var tccOrg = Guid.NewGuid();
      var subscriptionUid = Guid.NewGuid();
      var startDateTime = ts.FirstEventDate;
      var endDateTime = new DateTime(9999, 12, 31);
      var startDate = startDateTime.ToString("yyyy-MM-dd");
      var endDate = endDateTime.ToString("yyyy-MM-dd");
      const string geometryWkt = "POLYGON((-121.347189366818 38.8361907402694,-121.349260032177 38.8361656688414,-121.349217116833 38.8387897637231,-121.347275197506 38.8387145521594,-121.347189366818 38.8361907402694,-121.347189366818 38.8361907402694))";
      var eventsArray = new[] {
        "| TableName           | EventDate   | CustomerUID   | Name       | fk_CustomerTypeID | SubscriptionUID   | fk_CustomerUID | fk_ServiceTypeID | StartDate   | EndDate        | fk_ProjectUID | TCCOrgID | fk_SubscriptionUID |",
       $"| Customer            | 0d+09:00:00 | {customerUid} | {testName} | 1                 |                   |                |                  |             |                |               |          |                    |",
       $"| CustomerTccOrg      | 0d+09:00:00 | {customerUid} |            |                   |                   |                |                  |             |                |               | {tccOrg} |                    |",
       $"| Subscription        | 0d+09:10:00 |               |            |                   | {subscriptionUid} | {customerUid}  | 19               | {startDate} | {endDate}      |               |          |                    |",
       $"| ProjectSubscription | 0d+09:20:00 |               |            |                   |                   |                |                  | {startDate} |                | {projectUid}  |          | {subscriptionUid}  |"};
      ts.PublishEventCollection(eventsArray);

      ts.IsPublishToWebApi = true;
      var projectEventArray = new[] {
        "| EventType          | EventDate   | ProjectUID   | ProjectID         | ProjectName | ProjectType | ProjectTimezone           | ProjectStartDate                            | ProjectEndDate                             | ProjectBoundary | CustomerUID   | CustomerID        |IsArchived | CoordinateSystem      | Description |",
       $"| CreateProjectEvent | 0d+09:00:00 | {projectUid} | {legacyProjectId} | {testName}  | Standard    | New Zealand Standard Time | {startDateTime:yyyy-MM-ddTHH:mm:ss.fffffff} | {endDateTime:yyyy-MM-ddTHH:mm:ss.fffffff}  | {geometryWkt}   | {customerUid} | {legacyProjectId} |false      | BootCampDimensions.dc | {testName}  |"};
      ts.PublishEventCollection(projectEventArray);

      var importFileArray = new[] {
       "| EventType              | ProjectUid   | CustomerUid   | Name                          | ImportedFileType | FileCreatedUtc              | FileUpdatedUtc              | ImportedBy                 | SurveyedUtc     | IsActivated | MinZoomLevel | MaxZoomLevel |",
      $"| ImportedFileDescriptor | {projectUid} | {customerUid} | {TestFile.TestDesignSurface1} | 2                | {startDateTime}             | {startDateTime.AddDays(5)}  | testProjectMDM@trimble.com | {startDateTime} | true        | 0            | 0            |",
      $"| ImportedFileDescriptor | {projectUid} | {customerUid} | {TestFile.TestDesignSurface1} | 2                | {startDateTime.AddDays(10)} | {startDateTime.AddDays(10)} | testProjectMDM@trimble.com | {startDateTime} | true        | 0            | 0            |"};
      var filesResult = importFile.SendImportedFilesToWebApi(ts, importFileArray, 1);
      var expectedResult1 = importFile.expectedImportFileDescriptorSingleResult.ImportedFileDescriptor;
      ts.CompareTheActualImportFileWithExpected(filesResult.ImportedFileDescriptor, expectedResult1, true);

      var filesResult2 = importFile.SendImportedFilesToWebApi(ts, importFileArray, 2, "PUT");
      var expectedResult2 = importFile.expectedImportFileDescriptorSingleResult.ImportedFileDescriptor;
      var importFileList = importFile.GetImportedFilesFromWebApi(ts.GetBaseUri() + $"api/v4/importedfiles?projectUid={projectUid}", customerUid);
      Assert.IsTrue(importFileList.ImportedFileDescriptors.Count == 1, "Expected 1 imported files but got " + importFileList.ImportedFileDescriptors.Count);
      ts.CompareTheActualImportFileWithExpected(importFileList.ImportedFileDescriptors[0], expectedResult2, true);
    }


    [TestMethod]
    public void TestImportANewFileThenDeleteTheAlignmentFile()
    {
      const string testName = "File Import 7";
      msg.Title(testName, "Create standard project then upload a new alignment file. Then delete alignment file");
      var ts = new TestSupport();
      var importFile = new ImportFile();
      var legacyProjectId = ts.SetLegacyProjectId();
      var projectUid = Guid.NewGuid().ToString();
      var customerUid = Guid.NewGuid();
      var tccOrg = Guid.NewGuid();
      var subscriptionUid = Guid.NewGuid();
      var startDateTime = ts.FirstEventDate;
      var endDateTime = new DateTime(9999, 12, 31);
      var startDate = startDateTime.ToString("yyyy-MM-dd");
      var endDate = endDateTime.ToString("yyyy-MM-dd");
      const string geometryWkt = "POLYGON((-121.347189366818 38.8361907402694,-121.349260032177 38.8361656688414,-121.349217116833 38.8387897637231,-121.347275197506 38.8387145521594,-121.347189366818 38.8361907402694,-121.347189366818 38.8361907402694))";
      var eventsArray = new[] {
         "| TableName           | EventDate   | CustomerUID   | Name       | fk_CustomerTypeID | SubscriptionUID   | fk_CustomerUID | fk_ServiceTypeID | StartDate   | EndDate        | fk_ProjectUID | TCCOrgID | fk_SubscriptionUID |",
        $"| Customer            | 0d+09:00:00 | {customerUid} | {testName} | 1                 |                   |                |                  |             |                |               |          |                    |",
        $"| CustomerTccOrg      | 0d+09:00:00 | {customerUid} |            |                   |                   |                |                  |             |                |               | {tccOrg} |                    |",
        $"| Subscription        | 0d+09:10:00 |               |            |                   | {subscriptionUid} | {customerUid}  | 19               | {startDate} | {endDate}      |               |          |                    |",
        $"| ProjectSubscription | 0d+09:20:00 |               |            |                   |                   |                |                  | {startDate} |                | {projectUid}  |          | {subscriptionUid}  |"};
      ts.PublishEventCollection(eventsArray);

      ts.IsPublishToWebApi = true;
      var projectEventArray = new[] {
        "| EventType           | EventDate   | ProjectUID   | ProjectID         | ProjectName | ProjectType | ProjectTimezone           | ProjectStartDate                            | ProjectEndDate                             | ProjectBoundary | CustomerUID   | CustomerID        |IsArchived | CoordinateSystem      | Description |",
        $"| CreateProjectEvent | 0d+09:00:00 | {projectUid} | {legacyProjectId} | {testName}  | Standard    | New Zealand Standard Time | {startDateTime:yyyy-MM-ddTHH:mm:ss.fffffff} | {endDateTime:yyyy-MM-ddTHH:mm:ss.fffffff}  | {geometryWkt}   | {customerUid} | {legacyProjectId} |false      | BootCampDimensions.dc | {testName}  |"};
      ts.PublishEventCollection(projectEventArray);

      var importFileArray = new[]
      {
        "| EventType              | ProjectUid   | CustomerUid   | Name                      | ImportedFileType | FileCreatedUtc  | FileUpdatedUtc             | ImportedBy                 | IsActivated | MinZoomLevel | MaxZoomLevel |",
       $"| ImportedFileDescriptor | {projectUid} | {customerUid} | {TestFile.TestAlignment1} | 3                | {startDateTime} | {startDateTime.AddDays(5)} | testProjectMDM@trimble.com | true        | 15           | 19           |"
      };
      var filesResult = importFile.SendImportedFilesToWebApi(ts, importFileArray, 1);
      var expectedResult1 = importFile.expectedImportFileDescriptorSingleResult.ImportedFileDescriptor;
      ts.CompareTheActualImportFileWithExpected(filesResult.ImportedFileDescriptor, expectedResult1, true);
      importFile.importedFileUid = filesResult.ImportedFileDescriptor.ImportedFileUid;

      var filesResult2 = importFile.SendImportedFilesToWebApi(ts, importFileArray, 1, "DELETE");
      var expectedResult2 = importFile.expectedImportFileDescriptorSingleResult.ImportedFileDescriptor;
      var importFileList = importFile.GetImportedFilesFromWebApi(ts.GetBaseUri() + $"api/v4/importedfiles?projectUid={projectUid}", customerUid);
      Assert.IsTrue(importFileList.ImportedFileDescriptors.Count == 0, "Expected 0 imported files but got " + importFileList.ImportedFileDescriptors.Count);
    }

    [TestMethod]
    public void KafkaConsumeTestImportANewAlignmentFile()
    {
      const string testName = "File Import 8";
      msg.Title(testName, "Kafka Consume Test Create standard project then upload a new alignment file.");
      var ts = new TestSupport();
      var projectConsumerMysql = new MySqlHelper();
      projectConsumerMysql.UpdateDbSchemaName(PROJECT_DB_SCHEMA_NAME);
      var importFile = new ImportFile();
      var legacyProjectId = ts.SetLegacyProjectId();
      var projectUid = Guid.NewGuid().ToString();
      var customerUid = Guid.NewGuid();
      var tccOrg = Guid.NewGuid();
      var subscriptionUid = Guid.NewGuid();
      var startDateTime = ts.FirstEventDate;
      var endDateTime = new DateTime(9999, 12, 31);
      var startDate = startDateTime.ToString("yyyy-MM-dd");
      var endDate = endDateTime.ToString("yyyy-MM-dd");
      const string geometryWkt = "POLYGON((-121.347189366818 38.8361907402694,-121.349260032177 38.8361656688414,-121.349217116833 38.8387897637231,-121.347275197506 38.8387145521594,-121.347189366818 38.8361907402694,-121.347189366818 38.8361907402694))";
      var eventsArray = new[] {
        "| TableName           | EventDate   | CustomerUID   | Name       | fk_CustomerTypeID | SubscriptionUID   | fk_CustomerUID | fk_ServiceTypeID | StartDate   | EndDate        | fk_ProjectUID | TCCOrgID | fk_SubscriptionUID |",
        $"| Customer            | 0d+09:00:00 | {customerUid} | {testName} | 1                 |                   |                |                  |             |                |               |          |                    |",
        $"| CustomerTccOrg      | 0d+09:00:00 | {customerUid} |            |                   |                   |                |                  |             |                |               | {tccOrg} |                    |",
        $"| Subscription        | 0d+09:10:00 |               |            |                   | {subscriptionUid} | {customerUid}  | 19               | {startDate} | {endDate}      |               |          |                    |",
        $"| ProjectSubscription | 0d+09:20:00 |               |            |                   |                   |                |                  | {startDate} |                | {projectUid}  |          | {subscriptionUid}  |"};
      ts.PublishEventCollection(eventsArray);

      ts.IsPublishToWebApi = true;
      var projectEventArray = new[] {
        "| EventType           | EventDate   | ProjectUID   | ProjectID         | ProjectName | ProjectType | ProjectTimezone           | ProjectStartDate                            | ProjectEndDate                             | ProjectBoundary | CustomerUID   | CustomerID        |IsArchived | CoordinateSystem      | Description |",
       $"| CreateProjectEvent | 0d+09:00:00 | {projectUid} | {legacyProjectId} | {testName}  | Standard    | New Zealand Standard Time | {startDateTime:yyyy-MM-ddTHH:mm:ss.fffffff} | {endDateTime:yyyy-MM-ddTHH:mm:ss.fffffff}  | {geometryWkt}   | {customerUid} | {legacyProjectId} |false      | BootCampDimensions.dc | {testName}  |"};
      ts.PublishEventCollection(projectEventArray);

      var importFileArray = new[]
      {
        "| EventType              | ProjectUid   | CustomerUid   | Name                      | ImportedFileType | FileCreatedUtc  | FileUpdatedUtc             | ImportedBy                 | IsActivated | MinZoomLevel | MaxZoomLevel |",
       $"| ImportedFileDescriptor | {projectUid} | {customerUid} | {TestFile.TestAlignment1} | 3                | {startDateTime} | {startDateTime.AddDays(5)} | testProjectMDM@trimble.com | true        | 15           | 19           |"
      };
      var filesResult = importFile.SendImportedFilesToWebApi(ts, importFileArray, 1);
      var expectedResult1 = importFile.expectedImportFileDescriptorSingleResult.ImportedFileDescriptor;
      ts.CompareTheActualImportFileWithExpected(filesResult.ImportedFileDescriptor, expectedResult1, true);
      importFile.importedFileUid = filesResult.ImportedFileDescriptor.ImportedFileUid;
      projectConsumerMysql.VerifyTestResultDatabaseRecordCount("ImportedFile", "fk_ProjectUID", 1, new Guid(projectUid));
    }

    [TestMethod]
    public void KafkaConsumeTestTestImportANewFileThenUpdateTheSurveyedSurfaceFile()
    {
      const string testName = "File Import 9";
      msg.Title(testName, "Kafka Consume Test Create standard project then upload a new surveyed surface file. ");
      var ts = new TestSupport();
      var projectConsumerMysql = new MySqlHelper();
      projectConsumerMysql.UpdateDbSchemaName(PROJECT_DB_SCHEMA_NAME);
      var importFile = new ImportFile();
      var legacyProjectId = ts.SetLegacyProjectId();
      var projectUid = Guid.NewGuid().ToString();
      var customerUid = Guid.NewGuid();
      var tccOrg = Guid.NewGuid();
      var subscriptionUid = Guid.NewGuid();
      var startDateTime = ts.FirstEventDate;
      var endDateTime = new DateTime(9999, 12, 31);
      var startDate = startDateTime.ToString("yyyy-MM-dd");
      var endDate = endDateTime.ToString("yyyy-MM-dd");
      const string geometryWkt = "POLYGON((-121.347189366818 38.8361907402694,-121.349260032177 38.8361656688414,-121.349217116833 38.8387897637231,-121.347275197506 38.8387145521594,-121.347189366818 38.8361907402694,-121.347189366818 38.8361907402694))";
      var eventsArray = new[] {
        "| TableName           | EventDate   | CustomerUID   | Name       | fk_CustomerTypeID | SubscriptionUID   | fk_CustomerUID | fk_ServiceTypeID | StartDate   | EndDate        | fk_ProjectUID | TCCOrgID | fk_SubscriptionUID |",
       $"| Customer            | 0d+09:00:00 | {customerUid} | {testName} | 1                 |                   |                |                  |             |                |               |          |                    |",
       $"| CustomerTccOrg      | 0d+09:00:00 | {customerUid} |            |                   |                   |                |                  |             |                |               | {tccOrg} |                    |",
       $"| Subscription        | 0d+09:10:00 |               |            |                   | {subscriptionUid} | {customerUid}  | 19               | {startDate} | {endDate}      |               |          |                    |",
       $"| ProjectSubscription | 0d+09:20:00 |               |            |                   |                   |                |                  | {startDate} |                | {projectUid}  |          | {subscriptionUid}  |"};
      ts.PublishEventCollection(eventsArray);

      ts.IsPublishToWebApi = true;
      var projectEventArray = new[] {
        "| EventType          | EventDate   | ProjectUID   | ProjectID         | ProjectName | ProjectType | ProjectTimezone           | ProjectStartDate                            | ProjectEndDate                             | ProjectBoundary | CustomerUID   | CustomerID        |IsArchived | CoordinateSystem      | Description |",
       $"| CreateProjectEvent | 0d+09:00:00 | {projectUid} | {legacyProjectId} | {testName}  | Standard    | New Zealand Standard Time | {startDateTime:yyyy-MM-ddTHH:mm:ss.fffffff} | {endDateTime:yyyy-MM-ddTHH:mm:ss.fffffff}  | {geometryWkt}   | {customerUid} | {legacyProjectId} |false      | BootCampDimensions.dc | {testName}  |"};
      ts.PublishEventCollection(projectEventArray);

      var importFileArray = new[] {
        "| EventType              | ProjectUid   | CustomerUid   | Name                          | ImportedFileType | FileCreatedUtc              | FileUpdatedUtc              | ImportedBy                 | SurveyedUtc     | IsActivated | MinZoomLevel | MaxZoomLevel |",
       $"| ImportedFileDescriptor | {projectUid} | {customerUid} | {TestFile.TestDesignSurface1} | 2                | {startDateTime}             | {startDateTime.AddDays(5)}  | testProjectMDM@trimble.com | {startDateTime} | true        | 0            | 0            |",
       $"| ImportedFileDescriptor | {projectUid} | {customerUid} | {TestFile.TestDesignSurface1} | 2                | {startDateTime.AddDays(10)} | {startDateTime.AddDays(10)} | testProjectMDM@trimble.com | {startDateTime} | true        | 0            | 0            |"};
      var filesResult = importFile.SendImportedFilesToWebApi(ts, importFileArray, 1);
      var expectedResult1 = importFile.expectedImportFileDescriptorSingleResult.ImportedFileDescriptor;
      ts.CompareTheActualImportFileWithExpected(filesResult.ImportedFileDescriptor, expectedResult1, true);

      projectConsumerMysql.VerifyTestResultDatabaseRecordCount("ImportedFile", "fk_ProjectUID", 1, new Guid(projectUid));

    }
    [TestMethod]
    public void TestImportdxfFileUsSurveyFeet()
    {
      const string testName = "File Import 10";
      msg.Title(testName, "Create standard project and customer then upload dxf file");
      var ts = new TestSupport();
      var importFile = new ImportFile();
      var legacyProjectId = ts.SetLegacyProjectId();
      var projectUid = Guid.NewGuid().ToString();
      var customerUid = Guid.NewGuid();
      var tccOrg = Guid.NewGuid();
      var subscriptionUid = Guid.NewGuid();
      var startDateTime = ts.FirstEventDate;
      var endDateTime = new DateTime(9999, 12, 31);
      var startDate = startDateTime.ToString("yyyy-MM-dd");
      var endDate = endDateTime.ToString("yyyy-MM-dd");
      const string geometryWkt = "POLYGON((-121.347189366818 38.8361907402694,-121.349260032177 38.8361656688414,-121.349217116833 38.8387897637231,-121.347275197506 38.8387145521594,-121.347189366818 38.8361907402694,-121.347189366818 38.8361907402694))";
      var eventsArray = new[] {
       "| TableName           | EventDate   | CustomerUID   | Name       | fk_CustomerTypeID | SubscriptionUID   | fk_CustomerUID | fk_ServiceTypeID | StartDate   | EndDate        | fk_ProjectUID | TCCOrgID | fk_SubscriptionUID |",
      $"| Customer            | 0d+09:00:00 | {customerUid} | {testName} | 1                 |                   |                |                  |             |                |               |          |                    |",
      $"| CustomerTccOrg      | 0d+09:00:00 | {customerUid} |            |                   |                   |                |                  |             |                |               | {tccOrg} |                    |",
      $"| Subscription        | 0d+09:10:00 |               |            |                   | {subscriptionUid} | {customerUid}  | 19               | {startDate} | {endDate}      |               |          |                    |",
      $"| ProjectSubscription | 0d+09:20:00 |               |            |                   |                   |                |                  | {startDate} |                | {projectUid}  |          | {subscriptionUid}  |"};
      ts.PublishEventCollection(eventsArray);

      ts.IsPublishToWebApi = true;
      var projectEventArray = new[] {
       "| EventType          | EventDate   | ProjectUID   | ProjectID         | ProjectName | ProjectType | ProjectTimezone           | ProjectStartDate                            | ProjectEndDate                             | ProjectBoundary | CustomerUID   | CustomerID        | IsArchived | CoordinateSystem      | Description |",
      $"| CreateProjectEvent | 0d+09:00:00 | {projectUid} | {legacyProjectId} | {testName}  | Standard    | New Zealand Standard Time | {startDateTime:yyyy-MM-ddTHH:mm:ss.fffffff} | {endDateTime:yyyy-MM-ddTHH:mm:ss.fffffff}  | {geometryWkt}   | {customerUid} | {legacyProjectId} | false      | BootCampDimensions.dc | {testName}  |"};
      ts.PublishEventCollection(projectEventArray);
      var importFileArray = new[] {
       "| EventType              | ProjectUid   | CustomerUid   | Name                   | ImportedFileType | FileCreatedUtc  | FileUpdatedUtc             | ImportedBy                 | IsActivated | DxfUnitsType | MinZoomLevel | MaxZoomLevel |",
      $"| ImportedFileDescriptor | {projectUid} | {customerUid} | {TestFile.TestDxFfile} | 0                | {startDateTime} | {startDateTime.AddDays(5)} | testProjectMDM@trimble.com | true        | 2            | 15           | 19           |"};
      var filesResult = importFile.SendImportedFilesToWebApi(ts, importFileArray, 1);
      ts.CompareTheActualImportFileWithExpected(filesResult.ImportedFileDescriptor, importFile.expectedImportFileDescriptorSingleResult.ImportedFileDescriptor, true);
    }
    [TestMethod]
    public void TestImportdxfFileImperial()
    {
      const string testName = "File Import 11";
      msg.Title(testName, "Create standard project and customer then upload dxf file in imperial");
      var ts = new TestSupport();
      var importFile = new ImportFile();
      var legacyProjectId = ts.SetLegacyProjectId();
      var projectUid = Guid.NewGuid().ToString();
      var customerUid = Guid.NewGuid();
      var tccOrg = Guid.NewGuid();
      var subscriptionUid = Guid.NewGuid();
      var startDateTime = ts.FirstEventDate;
      var endDateTime = new DateTime(9999, 12, 31);
      var startDate = startDateTime.ToString("yyyy-MM-dd");
      var endDate = endDateTime.ToString("yyyy-MM-dd");
      const string geometryWkt = "POLYGON((-121.347189366818 38.8361907402694,-121.349260032177 38.8361656688414,-121.349217116833 38.8387897637231,-121.347275197506 38.8387145521594,-121.347189366818 38.8361907402694,-121.347189366818 38.8361907402694))";
      var eventsArray = new[] {
       "| TableName           | EventDate   | CustomerUID   | Name       | fk_CustomerTypeID | SubscriptionUID   | fk_CustomerUID | fk_ServiceTypeID | StartDate   | EndDate        | fk_ProjectUID | TCCOrgID | fk_SubscriptionUID |",
      $"| Customer            | 0d+09:00:00 | {customerUid} | {testName} | 1                 |                   |                |                  |             |                |               |          |                    |",
      $"| CustomerTccOrg      | 0d+09:00:00 | {customerUid} |            |                   |                   |                |                  |             |                |               | {tccOrg} |                    |",
      $"| Subscription        | 0d+09:10:00 |               |            |                   | {subscriptionUid} | {customerUid}  | 19               | {startDate} | {endDate}      |               |          |                    |",
      $"| ProjectSubscription | 0d+09:20:00 |               |            |                   |                   |                |                  | {startDate} |                | {projectUid}  |          | {subscriptionUid}  |"};
      ts.PublishEventCollection(eventsArray);

      ts.IsPublishToWebApi = true;
      var projectEventArray = new[] {
       "| EventType          | EventDate   | ProjectUID   | ProjectID         | ProjectName | ProjectType | ProjectTimezone           | ProjectStartDate                            | ProjectEndDate                             | ProjectBoundary | CustomerUID   | CustomerID        | IsArchived | CoordinateSystem      | Description |",
      $"| CreateProjectEvent | 0d+09:00:00 | {projectUid} | {legacyProjectId} | {testName}  | Standard    | New Zealand Standard Time | {startDateTime:yyyy-MM-ddTHH:mm:ss.fffffff} | {endDateTime:yyyy-MM-ddTHH:mm:ss.fffffff}  | {geometryWkt}   | {customerUid} | {legacyProjectId} | false      | BootCampDimensions.dc | {testName}  |"};
      ts.PublishEventCollection(projectEventArray);
      var importFileArray = new[] {
       "| EventType              | ProjectUid   | CustomerUid   | Name                   | ImportedFileType | FileCreatedUtc  | FileUpdatedUtc             | ImportedBy                 | IsActivated | DxfUnitsType | MinZoomLevel | MaxZoomLevel |",
      $"| ImportedFileDescriptor | {projectUid} | {customerUid} | {TestFile.TestDxFfile} | 0                | {startDateTime} | {startDateTime.AddDays(5)} | testProjectMDM@trimble.com | true        | 1            | 15           | 19           |"};
      var filesResult = importFile.SendImportedFilesToWebApi(ts, importFileArray, 1);
      ts.CompareTheActualImportFileWithExpected(filesResult.ImportedFileDescriptor, importFile.expectedImportFileDescriptorSingleResult.ImportedFileDescriptor, true);
    }

    [TestMethod]
    public void TestImportdxfFileMetric()
    {
      const string testName = "File Import 12";
      msg.Title(testName, "Create standard project and customer then upload dxf file in metric");
      var ts = new TestSupport();
      var importFile = new ImportFile();
      var legacyProjectId = ts.SetLegacyProjectId();
      var projectUid = Guid.NewGuid().ToString();
      var customerUid = Guid.NewGuid();
      var tccOrg = Guid.NewGuid();
      var subscriptionUid = Guid.NewGuid();
      var startDateTime = ts.FirstEventDate;
      var endDateTime = new DateTime(9999, 12, 31);
      var startDate = startDateTime.ToString("yyyy-MM-dd");
      var endDate = endDateTime.ToString("yyyy-MM-dd");
      const string geometryWkt = "POLYGON((-121.347189366818 38.8361907402694,-121.349260032177 38.8361656688414,-121.349217116833 38.8387897637231,-121.347275197506 38.8387145521594,-121.347189366818 38.8361907402694,-121.347189366818 38.8361907402694))";
      var eventsArray = new[] {
       "| TableName           | EventDate   | CustomerUID   | Name       | fk_CustomerTypeID | SubscriptionUID   | fk_CustomerUID | fk_ServiceTypeID | StartDate   | EndDate        | fk_ProjectUID | TCCOrgID | fk_SubscriptionUID |",
      $"| Customer            | 0d+09:00:00 | {customerUid} | {testName} | 1                 |                   |                |                  |             |                |               |          |                    |",
      $"| CustomerTccOrg      | 0d+09:00:00 | {customerUid} |            |                   |                   |                |                  |             |                |               | {tccOrg} |                    |",
      $"| Subscription        | 0d+09:10:00 |               |            |                   | {subscriptionUid} | {customerUid}  | 19               | {startDate} | {endDate}      |               |          |                    |",
      $"| ProjectSubscription | 0d+09:20:00 |               |            |                   |                   |                |                  | {startDate} |                | {projectUid}  |          | {subscriptionUid}  |"};
      ts.PublishEventCollection(eventsArray);

      ts.IsPublishToWebApi = true;
      var projectEventArray = new[] {
       "| EventType          | EventDate   | ProjectUID   | ProjectID         | ProjectName | ProjectType | ProjectTimezone           | ProjectStartDate                            | ProjectEndDate                             | ProjectBoundary | CustomerUID   | CustomerID        | IsArchived | CoordinateSystem      | Description |",
      $"| CreateProjectEvent | 0d+09:00:00 | {projectUid} | {legacyProjectId} | {testName}  | Standard    | New Zealand Standard Time | {startDateTime:yyyy-MM-ddTHH:mm:ss.fffffff} | {endDateTime:yyyy-MM-ddTHH:mm:ss.fffffff}  | {geometryWkt}   | {customerUid} | {legacyProjectId} | false      | BootCampDimensions.dc | {testName}  |"};
      ts.PublishEventCollection(projectEventArray);
      var importFileArray = new[] {
       "| EventType              | ProjectUid   | CustomerUid   | Name                   | ImportedFileType | FileCreatedUtc  | FileUpdatedUtc             | ImportedBy                 | IsActivated | DxfUnitsType | MinZoomLevel | MaxZoomLevel |",
      $"| ImportedFileDescriptor | {projectUid} | {customerUid} | {TestFile.TestDxFfile} | 0                | {startDateTime} | {startDateTime.AddDays(5)} | testProjectMDM@trimble.com | true        | 0            | 15           | 19           |"};
      var filesResult = importFile.SendImportedFilesToWebApi(ts, importFileArray, 1);
      ts.CompareTheActualImportFileWithExpected(filesResult.ImportedFileDescriptor, importFile.expectedImportFileDescriptorSingleResult.ImportedFileDescriptor, true);
    }
  }
}