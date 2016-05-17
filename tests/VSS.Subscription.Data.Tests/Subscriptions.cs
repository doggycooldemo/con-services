﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using VSS.Geofence.Data;
using VSS.Project.Data;
using VSS.VisionLink.Interfaces.Events.MasterData.Models;

namespace VSS.Subscription.Data.Tests
{
  [TestClass]
  public class Subscriptions
  {
    private readonly MySqlSubscriptionRepository _subscriptionService;
    private readonly MySqlProjectRepository _projectService;
    private readonly MySqlGeofenceRepository _geofenceService;

    public Subscriptions()
    {
      _subscriptionService = new MySqlSubscriptionRepository();
      _projectService = new MySqlProjectRepository();
      _projectService.SetInTransactionState(true);
      _geofenceService = new MySqlGeofenceRepository();
      _geofenceService.SetInTransactionState(true);
    }

    private CreateProjectSubscriptionEvent GetNewCreateProjectSubscriptionEvent()
    {
      return new CreateProjectSubscriptionEvent()
      {
        SubscriptionUID = Guid.NewGuid(),
        CustomerUID = Guid.NewGuid(),
        SubscriptionType = "Landfill",
        StartDate = DateTime.UtcNow.AddDays(-1).Date,
        EndDate = DateTime.UtcNow.AddDays(1).Date,
        ActionUTC = DateTime.UtcNow,
        ReceivedUTC = DateTime.UtcNow.AddMilliseconds(1000)
      };
    }

    private UpdateProjectSubscriptionEvent GetNewUpdateProjectSubscriptionEvent(Guid subscriptionUID, Guid customerUID, string subscriptionType, DateTime startDate, DateTime endDate, DateTime lastActionedUTC)
    {
      return new UpdateProjectSubscriptionEvent()
      {
        SubscriptionUID = subscriptionUID,
        CustomerUID = customerUID,
        SubscriptionType = subscriptionType,
        StartDate = startDate,
        EndDate = endDate,
        ActionUTC = lastActionedUTC,
        ReceivedUTC = DateTime.UtcNow.AddMilliseconds(100)
      };
    }

    private AssociateProjectSubscriptionEvent GetNewAssociateProjectSubscriptionEvent(Guid subscriptionUID, Guid projectUID, DateTime effectiveDate, DateTime receivedUTC)
    {
      return new AssociateProjectSubscriptionEvent()
      {
        SubscriptionUID = subscriptionUID,
        ProjectUID = projectUID,
        EffectiveDate = effectiveDate,
        ActionUTC = DateTime.UtcNow,
        ReceivedUTC = receivedUTC
      };
    }

    private CreateProjectEvent GetNewCreateProjectEvent()
    {
      return new CreateProjectEvent()
      {
        ProjectUID = Guid.NewGuid(),
        ProjectID = 123,
        ProjectName = "Test Project",
        ProjectTimezone = "New Zealand Standard Time",
        ProjectType = ProjectType.LandFill,
        ProjectStartDate = DateTime.UtcNow.AddDays(-1).Date,
        ProjectEndDate = DateTime.UtcNow.AddDays(1).Date,
        ActionUTC = DateTime.UtcNow,
        ReceivedUTC = DateTime.UtcNow.AddMilliseconds(1000)
      };
    }

    [TestMethod]
    public void CreateNewProjectSubscription_Succeeds()
    {
      _subscriptionService.InRollbackTransaction<object>(o =>
      {
        var createProjectSubscriptionEvent = GetNewCreateProjectSubscriptionEvent();
        var savedSubscriptionUID = createProjectSubscriptionEvent.SubscriptionUID;
        var upsertCount = _subscriptionService.StoreSubscription(createProjectSubscriptionEvent, _projectService, _geofenceService);
        Assert.IsTrue(upsertCount == 1, "Failed to create a Landfill project subscription!");

        var subscription = _subscriptionService.GetSubscription(createProjectSubscriptionEvent.SubscriptionUID.ToString());
        Assert.IsNotNull(subscription, "Failed to get the created project subscription!");

        createProjectSubscriptionEvent = GetNewCreateProjectSubscriptionEvent();
        createProjectSubscriptionEvent.SubscriptionUID = savedSubscriptionUID;

        upsertCount = _subscriptionService.StoreSubscription(createProjectSubscriptionEvent, _projectService, _geofenceService);
        Assert.IsTrue(upsertCount == 1, "Failed to update the existing Landfill project subscription!");

        return null;
      });
    }

    [TestMethod]
    public void CreateNewProjectSubscription_Fails()
    {
      _subscriptionService.InRollbackTransaction<object>(o =>
      {
        var createProjectSubscriptionEvent = GetNewCreateProjectSubscriptionEvent();
        createProjectSubscriptionEvent.SubscriptionType = "Unified Fleet";

        var upsertCount = _subscriptionService.StoreSubscription(createProjectSubscriptionEvent, _projectService, _geofenceService);
        Assert.IsFalse(upsertCount == 1, "Should fail failed to create a Landfill project subscription due to the wrong subscription's type!");

        var subscription = _subscriptionService.GetSubscription(createProjectSubscriptionEvent.SubscriptionUID.ToString());
        Assert.IsNull(subscription, "Should fail to get a project subscription!");

        return null;
      });
    }

    [TestMethod]
    public void UpsertSubscription_Fails()
    {
      var upsertCount = _subscriptionService.StoreSubscription(null, null, null);
      Assert.IsTrue(upsertCount == 0, "Should fail to upsert a subscription!");
    }

    [TestMethod]
    public void UpdateProjectSubscription_Succeeds()
    {
      _subscriptionService.InRollbackTransaction<object>(o =>
      {
        var createProjectSubscriptionEvent = GetNewCreateProjectSubscriptionEvent();
        var upsertCount = _subscriptionService.StoreSubscription(createProjectSubscriptionEvent, _projectService, _geofenceService);
        Assert.IsTrue(upsertCount == 1, "Failed to create a Landfill project subscription!");

        var updateProjectSubscriptionEvent = GetNewUpdateProjectSubscriptionEvent(createProjectSubscriptionEvent.SubscriptionUID,
                                                                                  createProjectSubscriptionEvent.CustomerUID,
                                                                                  createProjectSubscriptionEvent.SubscriptionType,
                                                                                  createProjectSubscriptionEvent.StartDate,
                                                                                  createProjectSubscriptionEvent.EndDate.AddDays(3),
                                                                                  DateTime.UtcNow);

        upsertCount = _subscriptionService.StoreSubscription(updateProjectSubscriptionEvent, _projectService, _geofenceService);
        Assert.IsTrue(upsertCount == 1, "Failed to update the existing Landfill project subscription!");

        var subscription = _subscriptionService.GetSubscription(createProjectSubscriptionEvent.SubscriptionUID.ToString());
        Assert.IsNotNull(subscription, "Failed to get the updated project subscription!");

        Assert.IsTrue(subscription.SubscriptionUID == updateProjectSubscriptionEvent.SubscriptionUID.ToString(), "Project Subscription SubscriptionUID should not be changed!");
        Assert.IsTrue(subscription.CustomerUID == updateProjectSubscriptionEvent.CustomerUID.ToString(), "Project Subscription CustomerUID should not be changed!");
        Assert.IsTrue(subscription.ServiceTypeID == _subscriptionService._serviceTypes[updateProjectSubscriptionEvent.SubscriptionType].ID, "Project Subscription Type should not be changed!");
        Assert.IsTrue(subscription.StartDate == createProjectSubscriptionEvent.StartDate, "Project Subscription Start Date of should not be changed!");
        Assert.IsTrue((subscription.EndDate - createProjectSubscriptionEvent.EndDate).Days == 3, "The End Date of the updated Project Subscription was incorectly updated!");
        Assert.IsTrue(subscription.LastActionedUTC > createProjectSubscriptionEvent.ActionUTC, "Project Subscription LastActionedUtc of the updated Project Subscription was incorectly updated!");

        return null;
      });
    }

    [TestMethod]
    public void UpdateProjectSubscription_Fails()
    {
      _subscriptionService.InRollbackTransaction<object>(o =>
      {
        var createProjectSubscriptionEvent = GetNewCreateProjectSubscriptionEvent();
        var upsertCount = _subscriptionService.StoreSubscription(createProjectSubscriptionEvent, _projectService, _geofenceService);
        Assert.IsTrue(upsertCount == 1, "Failed to create a Landfill project subscription!");

        var updateProjectSubscriptionEvent = GetNewUpdateProjectSubscriptionEvent(createProjectSubscriptionEvent.SubscriptionUID,
                                                                                  createProjectSubscriptionEvent.CustomerUID,
                                                                                  "Unified Fleet",
                                                                                  createProjectSubscriptionEvent.StartDate,
                                                                                  createProjectSubscriptionEvent.EndDate.AddMonths(3),
                                                                                  DateTime.UtcNow);

        upsertCount = _subscriptionService.StoreSubscription(updateProjectSubscriptionEvent, _projectService, _geofenceService);
        Assert.IsFalse(upsertCount == 1, "Should fail to update the existing Landfill project subscription due to the wrong subscription's type!");

        return null;
      });
    }
    
    /// <summary>
    /// AssociateProjectSubscription event arrives first and as there is neither CreateProjectSubscription event
    /// nor CreateProject event coming from the Kafka queue dummy Project and Subscription entries are created.
    /// </summary>
    [TestMethod]
    public void AssociateProjectSubscription_NoProjectAndSubscription_Succeeds()
    {
      _subscriptionService.InRollbackTransaction<object>(o =>
      {
        _projectService.SetConnection((MySqlConnection)o);
        _geofenceService.SetConnection((MySqlConnection)o);

        var associateProjectSubscriptionEvent = GetNewAssociateProjectSubscriptionEvent(Guid.NewGuid(), 
                                                                                        Guid.NewGuid(), 
                                                                                        DateTime.UtcNow, 
                                                                                        DateTime.UtcNow.AddMilliseconds(100));

        var upsertCount = _subscriptionService.StoreSubscription(associateProjectSubscriptionEvent, _projectService, _geofenceService);
        Assert.IsTrue(upsertCount == 1, "Failed to associate a subscription with a Landfill project!");

        var subscription = _subscriptionService.GetSubscription(associateProjectSubscriptionEvent.SubscriptionUID.ToString());
        Assert.IsNotNull(subscription, "Failed to get the subscription associated with a Landfill project!");

        var project = _projectService.GetProject(associateProjectSubscriptionEvent.ProjectUID.ToString());
        Assert.IsNotNull(project, "Failed to get the project associated with the subscription!");

        Assert.IsTrue(project.subscriptionUid == associateProjectSubscriptionEvent.SubscriptionUID.ToString(), "The accociated Project's SubscriptionUID does not match the Subscription's one!");

        return null;
      });
    }

    /// <summary>
    /// CreateProjectSubscription event arrives first, the AssociateProjectSubscription event is coming after
    /// and as there is no CreateProject event coming from the Kafka queue a dummy Project 
    /// entry is created.
    /// </summary>
    [TestMethod]
    public void AssociateProjectSubscription_NoProject_Succeeds()
    {
      _subscriptionService.InRollbackTransaction<object>(o =>
      {
        _projectService.SetConnection((MySqlConnection)o);
        _geofenceService.SetConnection((MySqlConnection)o);

        var createProjectSubscriptionEvent = GetNewCreateProjectSubscriptionEvent();
        var upsertCount = _subscriptionService.StoreSubscription(createProjectSubscriptionEvent, _projectService, _geofenceService);
        Assert.IsTrue(upsertCount == 1, "Failed to create a Landfill project subscription that is to be associated with a project!");

        var subscription = _subscriptionService.GetSubscription(createProjectSubscriptionEvent.SubscriptionUID.ToString());
        Assert.IsNotNull(subscription, "Failed to get the subscription that is to be associated with a project!");

        var associateProjectSubscriptionEvent = GetNewAssociateProjectSubscriptionEvent(createProjectSubscriptionEvent.SubscriptionUID,
                                                                                        Guid.NewGuid(),
                                                                                        DateTime.UtcNow,
                                                                                        DateTime.UtcNow.AddMilliseconds(100));

        upsertCount = _subscriptionService.StoreSubscription(associateProjectSubscriptionEvent, _projectService, _geofenceService);
        Assert.IsTrue(upsertCount == 1, "Failed to associate the existing subscription with a Landfill project!");

        subscription = _subscriptionService.GetSubscription(associateProjectSubscriptionEvent.SubscriptionUID.ToString());
        Assert.IsNotNull(subscription, "Failed to get the subscription associated with a Landfill project!");

        var project = _projectService.GetProject(associateProjectSubscriptionEvent.ProjectUID.ToString());
        Assert.IsNotNull(project, "Failed to get the project associated with the existing subscription!");

        Assert.IsTrue(project.subscriptionUid == associateProjectSubscriptionEvent.SubscriptionUID.ToString(), "The accociated Project's SubscriptionUID does not match the existing Subscription's one!");

        return null;
      });
    }

    /// <summary>
    /// CreateProject event arrives first, the AssociateProjectSubscription event is coming after
    /// and as there is no CreateProjectSubscription event coming from the Kafka queue a dummy Subscription 
    /// entry is created.
    /// </summary>
    [TestMethod]
    public void AssociateProjectSubscription_NoSubscription_Succeeds()
    {
      _subscriptionService.InRollbackTransaction<object>(o =>
      {
        _projectService.SetConnection((MySqlConnection)o);
        _geofenceService.SetConnection((MySqlConnection)o);

        var createProjectEvent = GetNewCreateProjectEvent();
        var upsertCount = _projectService.StoreProject(createProjectEvent, _geofenceService);
        Assert.IsTrue(upsertCount == 1, "Failed to create a Landfill project that is to be associated with a subscription!");

        var project = _projectService.GetProject(createProjectEvent.ProjectUID.ToString());
        Assert.IsNotNull(project, "Failed to get the created Landfill project that is to be associated with a subscription!");

        var associateProjectSubscriptionEvent = GetNewAssociateProjectSubscriptionEvent(Guid.NewGuid(),
                                                                                        Guid.NewGuid(),
                                                                                        DateTime.UtcNow,
                                                                                        DateTime.UtcNow.AddMilliseconds(100));

        upsertCount = _subscriptionService.StoreSubscription(associateProjectSubscriptionEvent, _projectService, _geofenceService);
        Assert.IsTrue(upsertCount == 1, "Failed to associate the subscription with the existing Landfill project!");

        var subscription = _subscriptionService.GetSubscription(associateProjectSubscriptionEvent.SubscriptionUID.ToString());
        Assert.IsNotNull(subscription, "Failed to get the subscription associated with the existing Landfill project!");

        project = _projectService.GetProject(associateProjectSubscriptionEvent.ProjectUID.ToString());
        Assert.IsNotNull(project, "Failed to get the existing project associated with the subscription!");

        Assert.IsTrue(project.subscriptionUid == associateProjectSubscriptionEvent.SubscriptionUID.ToString(), "The accociated Project's SubscriptionUID does not match the existing Subscription's one!");

        return null;
      });
    }

    /// <summary>
    /// The events are consumed from the Kafka queue in the following order:
    /// 1. CreateProject
    /// 2. CreateProjectSubscription
    /// 3. AssociateProjectSubscription
    /// </summary>
    [TestMethod]
    public void AssociateProjectSubscription_Project_Subscription_Associate_Succeeds()
    {
      _subscriptionService.InRollbackTransaction<object>(o =>
      {
        _projectService.SetConnection((MySqlConnection)o);
        _geofenceService.SetConnection((MySqlConnection)o);
        
        // CreateProject event...
        var createProjectEvent = GetNewCreateProjectEvent();
        var upsertCount = _projectService.StoreProject(createProjectEvent, _geofenceService);
        Assert.IsTrue(upsertCount == 1, "Failed to create a Landfill project that is to be associated with a subscription!");

        var project = _projectService.GetProject(createProjectEvent.ProjectUID.ToString());
        Assert.IsNotNull(project, "Failed to get the created Landfill project that is to be associated with a subscription!");

        // CreateProjectSubscription event...
        var createProjectSubscriptionEvent = GetNewCreateProjectSubscriptionEvent();
        upsertCount = _subscriptionService.StoreSubscription(createProjectSubscriptionEvent, _projectService, _geofenceService);
        Assert.IsTrue(upsertCount == 1, "Failed to create a project subscription that is to be associated with a project!");

        var subscription = _subscriptionService.GetSubscription(createProjectSubscriptionEvent.SubscriptionUID.ToString());
        Assert.IsNotNull(subscription, "Failed to get the project subscription that is to be associated with a project!");

        // AssociateProjectSubscription event...
        var associateProjectSubscriptionEvent = GetNewAssociateProjectSubscriptionEvent(createProjectSubscriptionEvent.SubscriptionUID,
                                                                                        createProjectEvent.ProjectUID,
                                                                                        DateTime.UtcNow,
                                                                                        DateTime.UtcNow.AddMilliseconds(100));

        upsertCount = _subscriptionService.StoreSubscription(associateProjectSubscriptionEvent, _projectService, _geofenceService);
        Assert.IsTrue(upsertCount == 1, "Failed to associate the subscription with the existing Landfill project!");

        subscription = _subscriptionService.GetSubscription(associateProjectSubscriptionEvent.SubscriptionUID.ToString());
        Assert.IsNotNull(subscription, "Failed to get the subscription associated with the existing Landfill project!");

        project = _projectService.GetProject(associateProjectSubscriptionEvent.ProjectUID.ToString());
        Assert.IsNotNull(project, "Failed to get the existing project associated with the subscription!");

        Assert.IsTrue(project.subscriptionUid == associateProjectSubscriptionEvent.SubscriptionUID.ToString(), "The accociated Project's SubscriptionUID does not match the existing Subscription's one!");

        return null;
      });
    }

    /// <summary>
    /// The events are consumed from the Kafka queue in the following order:
    /// 1. CreateProjectSubscription
    /// 2. CreateProject
    /// 3. AssociateProjectSubscription
    /// </summary>
    [TestMethod]
    public void AssociateProjectSubscription_Subscription_Project_Associate_Succeeds()
    {
      _subscriptionService.InRollbackTransaction<object>(o =>
      {
        _projectService.SetConnection((MySqlConnection)o);
        _geofenceService.SetConnection((MySqlConnection)o);

        // CreateProjectSubscription event...
        var createProjectSubscriptionEvent = GetNewCreateProjectSubscriptionEvent();
        var upsertCount = _subscriptionService.StoreSubscription(createProjectSubscriptionEvent, _projectService, _geofenceService);
        Assert.IsTrue(upsertCount == 1, "Failed to create a project subscription that is to be associated with a project!");

        var subscription = _subscriptionService.GetSubscription(createProjectSubscriptionEvent.SubscriptionUID.ToString());
        Assert.IsNotNull(subscription, "Failed to get the project subscription that is to be associated with a project!");

        // CreateProject event...
        var createProjectEvent = GetNewCreateProjectEvent();
        upsertCount = _projectService.StoreProject(createProjectEvent, _geofenceService);
        Assert.IsTrue(upsertCount == 1, "Failed to create a Landfill project that is to be associated with a subscription!");

        var project = _projectService.GetProject(createProjectEvent.ProjectUID.ToString());
        Assert.IsNotNull(project, "Failed to get the created Landfill project that is to be associated with a subscription!");

        // AssociateProjectSubscription event...
        var associateProjectSubscriptionEvent = GetNewAssociateProjectSubscriptionEvent(createProjectSubscriptionEvent.SubscriptionUID,
                                                                                        createProjectEvent.ProjectUID,
                                                                                        DateTime.UtcNow,
                                                                                        DateTime.UtcNow.AddMilliseconds(100));

        upsertCount = _subscriptionService.StoreSubscription(associateProjectSubscriptionEvent, _projectService, _geofenceService);
        Assert.IsTrue(upsertCount == 1, "Failed to associate the subscription with the existing Landfill project!");

        subscription = _subscriptionService.GetSubscription(associateProjectSubscriptionEvent.SubscriptionUID.ToString());
        Assert.IsNotNull(subscription, "Failed to get the subscription associated with the existing Landfill project!");

        project = _projectService.GetProject(associateProjectSubscriptionEvent.ProjectUID.ToString());
        Assert.IsNotNull(project, "Failed to get the existing project associated with the subscription!");

        Assert.IsTrue(project.subscriptionUid == associateProjectSubscriptionEvent.SubscriptionUID.ToString(), "The accociated Project's SubscriptionUID does not match the existing Subscription's one!");

        return null;
      });
    }

    /// <summary>
    /// The events are consumed from the Kafka queue in the following order:
    /// 1. AssociateProjectSubscription
    /// 2. CreateProject
    /// 3. CreateProjectSubscription
    /// </summary>
    [TestMethod]
    public void AssociateProjectSubscription_Associate_Project_Subscription_Succeeds()
    {
      _subscriptionService.InRollbackTransaction<object>(o =>
      {
        _projectService.SetConnection((MySqlConnection)o);
        _geofenceService.SetConnection((MySqlConnection)o);

        // AssociateProjectSubscription event...
        var associateProjectSubscriptionEvent = GetNewAssociateProjectSubscriptionEvent(Guid.NewGuid(),
                                                                                        Guid.NewGuid(),
                                                                                        DateTime.UtcNow,
                                                                                        DateTime.UtcNow.AddMilliseconds(100));

        var upsertCount = _subscriptionService.StoreSubscription(associateProjectSubscriptionEvent, _projectService, _geofenceService);
        Assert.IsTrue(upsertCount == 1, "Failed to associate the subscription with a Landfill project!");

        var subscription = _subscriptionService.GetSubscription(associateProjectSubscriptionEvent.SubscriptionUID.ToString());
        Assert.IsNotNull(subscription, "Failed to get the subscription to be associated with a Landfill project!");

        // CreateProject event...
        var createProjectEvent = GetNewCreateProjectEvent();
        createProjectEvent.ProjectUID = associateProjectSubscriptionEvent.ProjectUID;
        upsertCount = _projectService.StoreProject(createProjectEvent, _geofenceService);
        Assert.IsTrue(upsertCount == 1, "Failed to create a Landfill project that is to be associated with a subscription!");

        var project = _projectService.GetProject(createProjectEvent.ProjectUID.ToString());
        Assert.IsNotNull(project, "Failed to get the created Landfill project that is to be associated with a subscription!");

        // CreateProjectSubscription event...
        var createProjectSubscriptionEvent = GetNewCreateProjectSubscriptionEvent();
        createProjectSubscriptionEvent.SubscriptionUID = associateProjectSubscriptionEvent.SubscriptionUID;
        upsertCount = _subscriptionService.StoreSubscription(createProjectSubscriptionEvent, _projectService, _geofenceService);
        Assert.IsTrue(upsertCount == 1, "Failed to create a project subscription that is to be associated with the existing Landfill project!");

        subscription = _subscriptionService.GetSubscription(createProjectSubscriptionEvent.SubscriptionUID.ToString());
        Assert.IsNotNull(subscription, "Failed to get the project subscription that is to be associated with the existing Landfill project!");


        subscription = _subscriptionService.GetSubscription(associateProjectSubscriptionEvent.SubscriptionUID.ToString());
        Assert.IsNotNull(subscription, "Failed to get the subscription associated with the existing Landfill project!");

        project = _projectService.GetProject(associateProjectSubscriptionEvent.ProjectUID.ToString());
        Assert.IsNotNull(project, "Failed to get the existing project associated with the subscription!");

        Assert.IsTrue(project.subscriptionUid == associateProjectSubscriptionEvent.SubscriptionUID.ToString(), "The accociated Project's SubscriptionUID does not match the existing Subscription's one!");

        return null;
      });
    }

    /// <summary>
    /// The events are consumed from the Kafka queue in the following order:
    /// 1. AssociateProjectSubscription
    /// 2. CreateProjectSubscription
    /// 3. CreateProject
    /// </summary>
    [TestMethod]
    public void AssociateProjectSubscription_Associate_Subscription_Project_Succeeds()
    {
      _subscriptionService.InRollbackTransaction<object>(o =>
      {
        _projectService.SetConnection((MySqlConnection)o);
        _geofenceService.SetConnection((MySqlConnection)o);

        // AssociateProjectSubscription event...
        var associateProjectSubscriptionEvent = GetNewAssociateProjectSubscriptionEvent(Guid.NewGuid(),
                                                                                        Guid.NewGuid(),
                                                                                        DateTime.UtcNow,
                                                                                        DateTime.UtcNow.AddMilliseconds(100));

        var upsertCount = _subscriptionService.StoreSubscription(associateProjectSubscriptionEvent, _projectService, _geofenceService);
        Assert.IsTrue(upsertCount == 1, "Failed to associate the subscription with a Landfill project!");

        var subscription = _subscriptionService.GetSubscription(associateProjectSubscriptionEvent.SubscriptionUID.ToString());
        Assert.IsNotNull(subscription, "Failed to get the subscription to be associated with a Landfill project!");

        // CreateProjectSubscription event...
        var createProjectSubscriptionEvent = GetNewCreateProjectSubscriptionEvent();
        createProjectSubscriptionEvent.SubscriptionUID = associateProjectSubscriptionEvent.SubscriptionUID;
        upsertCount = _subscriptionService.StoreSubscription(createProjectSubscriptionEvent, _projectService, _geofenceService);
        Assert.IsTrue(upsertCount == 1, "Failed to create a project subscription that is to be associated with the existing Landfill project!");

        subscription = _subscriptionService.GetSubscription(createProjectSubscriptionEvent.SubscriptionUID.ToString());
        Assert.IsNotNull(subscription, "Failed to get the project subscription that is to be associated with the existing Landfill project!");

        // CreateProject event...
        var createProjectEvent = GetNewCreateProjectEvent();
        createProjectEvent.ProjectUID = associateProjectSubscriptionEvent.ProjectUID;
        upsertCount = _projectService.StoreProject(createProjectEvent, _geofenceService);
        Assert.IsTrue(upsertCount == 1, "Failed to create a Landfill project that is to be associated with a subscription!");

        var project = _projectService.GetProject(createProjectEvent.ProjectUID.ToString());
        Assert.IsNotNull(project, "Failed to get the created Landfill project that is to be associated with a subscription!");


        subscription = _subscriptionService.GetSubscription(associateProjectSubscriptionEvent.SubscriptionUID.ToString());
        Assert.IsNotNull(subscription, "Failed to get the subscription associated with the existing Landfill project!");

        project = _projectService.GetProject(associateProjectSubscriptionEvent.ProjectUID.ToString());
        Assert.IsNotNull(project, "Failed to get the existing project associated with the subscription!");

        Assert.IsTrue(project.subscriptionUid == associateProjectSubscriptionEvent.SubscriptionUID.ToString(), "The accociated Project's SubscriptionUID does not match the existing Subscription's one!");

        return null;
      });
    }

    /// <summary>
    /// The events are consumed from the Kafka queue in the following order:
    /// 1. CreateProject
    /// 2. AssociateProjectSubscription
    /// 3. CreateProjectSubscription
    /// </summary>
    [TestMethod]
    public void AssociateProjectSubscription_Project_Associate_Subscription_Succeeds()
    {
      _subscriptionService.InRollbackTransaction<object>(o =>
      {
        _projectService.SetConnection((MySqlConnection)o);
        _geofenceService.SetConnection((MySqlConnection)o);

        // CreateProject event...
        var createProjectEvent = GetNewCreateProjectEvent();
        var upsertCount = _projectService.StoreProject(createProjectEvent, _geofenceService);
        Assert.IsTrue(upsertCount == 1, "Failed to create a Landfill project that is to be associated with a subscription!");

        var project = _projectService.GetProject(createProjectEvent.ProjectUID.ToString());
        Assert.IsNotNull(project, "Failed to get the created Landfill project that is to be associated with a subscription!");

        // AssociateProjectSubscription event...
        var associateProjectSubscriptionEvent = GetNewAssociateProjectSubscriptionEvent(Guid.NewGuid(),
                                                                                        createProjectEvent.ProjectUID,
                                                                                        DateTime.UtcNow,
                                                                                        DateTime.UtcNow.AddMilliseconds(100));

        upsertCount = _subscriptionService.StoreSubscription(associateProjectSubscriptionEvent, _projectService, _geofenceService);
        Assert.IsTrue(upsertCount == 1, "Failed to associate the subscription with a Landfill project!");

        var subscription = _subscriptionService.GetSubscription(associateProjectSubscriptionEvent.SubscriptionUID.ToString());
        Assert.IsNotNull(subscription, "Failed to get the subscription to be associated with a Landfill project!");

        // CreateProjectSubscription event...
        var createProjectSubscriptionEvent = GetNewCreateProjectSubscriptionEvent();
        createProjectSubscriptionEvent.SubscriptionUID = associateProjectSubscriptionEvent.SubscriptionUID;
        upsertCount = _subscriptionService.StoreSubscription(createProjectSubscriptionEvent, _projectService, _geofenceService);
        Assert.IsTrue(upsertCount == 1, "Failed to create a project subscription that is to be associated with the existing Landfill project!");

        subscription = _subscriptionService.GetSubscription(createProjectSubscriptionEvent.SubscriptionUID.ToString());
        Assert.IsNotNull(subscription, "Failed to get the project subscription that is to be associated with the existing Landfill project!");


        subscription = _subscriptionService.GetSubscription(associateProjectSubscriptionEvent.SubscriptionUID.ToString());
        Assert.IsNotNull(subscription, "Failed to get the subscription associated with the existing Landfill project!");

        project = _projectService.GetProject(associateProjectSubscriptionEvent.ProjectUID.ToString());
        Assert.IsNotNull(project, "Failed to get the existing project associated with the subscription!");

        Assert.IsTrue(project.subscriptionUid == associateProjectSubscriptionEvent.SubscriptionUID.ToString(), "The accociated Project's SubscriptionUID does not match the existing Subscription's one!");

        return null;
      });
    }

    /// <summary>
    /// The events are consumed from the Kafka queue in the following order:
    /// 1. CreateProjectSubscription
    /// 2. AssociateProjectSubscription
    /// 3. CreateProject
    /// </summary>
    [TestMethod]
    public void AssociateProjectSubscription_Subscription_Associate_Project_Succeeds()
    {
      _subscriptionService.InRollbackTransaction<object>(o =>
      {
        _projectService.SetConnection((MySqlConnection)o);
        _geofenceService.SetConnection((MySqlConnection)o);

        // CreateProjectSubscription event...
        var createProjectSubscriptionEvent = GetNewCreateProjectSubscriptionEvent();
        var upsertCount = _subscriptionService.StoreSubscription(createProjectSubscriptionEvent, _projectService, _geofenceService);
        Assert.IsTrue(upsertCount == 1, "Failed to create a project subscription that is to be associated with the existing Landfill project!");

        var subscription = _subscriptionService.GetSubscription(createProjectSubscriptionEvent.SubscriptionUID.ToString());
        Assert.IsNotNull(subscription, "Failed to get the project subscription that is to be associated with the existing Landfill project!");

        // AssociateProjectSubscription event...
        var associateProjectSubscriptionEvent = GetNewAssociateProjectSubscriptionEvent(createProjectSubscriptionEvent.SubscriptionUID,
                                                                                        Guid.NewGuid(),
                                                                                        DateTime.UtcNow,
                                                                                        DateTime.UtcNow.AddMilliseconds(100));

        upsertCount = _subscriptionService.StoreSubscription(associateProjectSubscriptionEvent, _projectService, _geofenceService);
        Assert.IsTrue(upsertCount == 1, "Failed to associate the subscription with a Landfill project!");

        subscription = _subscriptionService.GetSubscription(associateProjectSubscriptionEvent.SubscriptionUID.ToString());
        Assert.IsNotNull(subscription, "Failed to get the subscription to be associated with a Landfill project!");

        // CreateProject event...
        var createProjectEvent = GetNewCreateProjectEvent();
        createProjectEvent.ProjectUID = associateProjectSubscriptionEvent.ProjectUID;
        upsertCount = _projectService.StoreProject(createProjectEvent, _geofenceService);
        Assert.IsTrue(upsertCount == 1, "Failed to create a Landfill project that is to be associated with a subscription!");

        var project = _projectService.GetProject(createProjectEvent.ProjectUID.ToString());
        Assert.IsNotNull(project, "Failed to get the created Landfill project that is to be associated with a subscription!");


        subscription = _subscriptionService.GetSubscription(associateProjectSubscriptionEvent.SubscriptionUID.ToString());
        Assert.IsNotNull(subscription, "Failed to get the subscription associated with the existing Landfill project!");

        project = _projectService.GetProject(associateProjectSubscriptionEvent.ProjectUID.ToString());
        Assert.IsNotNull(project, "Failed to get the existing project associated with the subscription!");

        Assert.IsTrue(project.subscriptionUid == associateProjectSubscriptionEvent.SubscriptionUID.ToString(), "The accociated Project's SubscriptionUID does not match the existing Subscription's one!");

        return null;
      });
    }
  }
}
