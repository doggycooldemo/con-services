﻿using System;
using System.Linq;
using Common.Repository;
using LandfillService.Common.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LandfillDatabase.Tests
{
  [TestClass]
  public class Machines : TestBase
  {

    [TestMethod]
    public void GetMachineId_Succeeds()
    {
      int legacyCustomerId;
      Guid customerUid;
      Guid userUid;
      Guid projectUid;
      Guid projectGeofenceUid;
      Guid landfillGeofenceUid;
      Guid subscriptionUid;
      var isCreatedOk = CreateAProjectWithLandfill(out legacyCustomerId,
        out customerUid, out userUid, out projectUid, out projectGeofenceUid, out landfillGeofenceUid,
        out subscriptionUid);
      Assert.IsTrue(isCreatedOk, "Failed to create a project.");

      var machineDetails = new MachineDetails()
      {
        assetId = 577777,
        machineName = "theMachineName",
        isJohnDoe = false
      };

      var machineUid = LandfillDb.GetMachineId(projectUid.ToString(), machineDetails);
      Assert.AreNotEqual(0, machineUid, "Failed to get the correct machine details.");
    }

    [TestMethod]
    public void GetMachine_Succeeds()
    {
      int legacyCustomerId;
      Guid customerUid;
      Guid userUid;
      Guid projectUid;
      Guid projectGeofenceUid;
      Guid landfillGeofenceUid;
      Guid subscriptionUid;
      var isCreatedOk = CreateAProjectWithLandfill(out legacyCustomerId,
        out customerUid, out userUid, out projectUid, out projectGeofenceUid, out landfillGeofenceUid,
        out subscriptionUid);
      Assert.IsTrue(isCreatedOk, "Failed to create a project.");

      var machineDetails = new MachineDetails()
      {
        assetId = 577777,
        machineName = "theMachineName",
        isJohnDoe = false
      };

      var machineId = LandfillDb.GetMachineId(projectUid.ToString(), machineDetails);
      Assert.AreNotEqual(0, machineId, "Failed to get the correct machine details.");

      var retrievedMachineDetails = LandfillDb.GetMachine(machineId);
      Assert.IsNotNull(retrievedMachineDetails, "Failed to get the correct machine details.");
      Assert.AreEqual(machineDetails.assetId, retrievedMachineDetails.assetId, "Failed to get the correct assetId.");
      Assert.AreEqual(machineDetails.machineName, retrievedMachineDetails.machineName, "Failed to get the correct machineName.");
      Assert.AreEqual(machineDetails.isJohnDoe, retrievedMachineDetails.isJohnDoe, "Failed to get the correct isJohnDoe.");
    }

  }
}
