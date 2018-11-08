﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VSS.Common.Exceptions;
using VSS.Productivity3D.Common.Models;
using VSS.Productivity3D.Common.ResultHandling;
using VSS.Productivity3D.Models.Enums;
using VSS.Productivity3D.Models.Models;
using VSS.Productivity3D.Models.Validation;
using VSS.Productivity3D.WebApi.Models.Report.Models;

namespace VSS.Productivity3D.WebApiTests.Report.Models
{
  [TestClass]
  public class PassCountsTests
  {
    [TestMethod]
    public void CanCreatePassCountsTest()
    {
      var validator = new DataAnnotationsValidator();
      PassCounts passCounts = new PassCounts(projectId, null, callId, passCountSettings, liftSettings, null, 0, null, null, null);
      ICollection<ValidationResult> results;
      Assert.IsTrue(validator.TryValidate(passCounts, out results));

      //missing project id
      passCounts = new PassCounts(0, null, callId, passCountSettings, liftSettings, null, 0, null, null, null);
      Assert.IsFalse(validator.TryValidate(passCounts, out results));
    }

    [TestMethod]
    public void ValidateSuccessTest()
    {
      PassCounts passCounts = new PassCounts(projectId, null, callId, passCountSettings, liftSettings, null, 0, null, null, null);
      passCounts.Validate();
    }


    [TestMethod]
    public void ValidateFailInvalidOverrideDatesTest()
    {
      //override startUTC > override end UTC
      PassCounts passCounts = new PassCounts(projectId, null, callId, passCountSettings, liftSettings, null, 0, new DateTime(2014, 1, 31), new DateTime(2014, 1, 1), null);
      Assert.ThrowsException<ServiceException>(() => passCounts.Validate());
    }

    [TestMethod]
    public void ValidateFailMissingOverrideDatesTest()
    {
      //missing override end UTC
      PassCounts passCounts = new PassCounts(projectId, null, callId, passCountSettings, liftSettings, null, 0, new DateTime(2014, 1, 1), null, null);
      Assert.ThrowsException<ServiceException>(() => passCounts.Validate());
    }

    private static readonly LiftThicknessTarget LiftThicknessTarget = new LiftThicknessTarget
    {
      AboveToleranceLiftThickness = (float)0.001,
      BelowToleranceLiftThickness = (float)0.002,
      TargetLiftThickness = (float)0.05
    };

    private long projectId = 1234;
    private Guid callId = new Guid();
    private PassCountSettings passCountSettings = PassCountSettings.CreatePassCountSettings(new int[] { 1, 3, 5, 10 });
    private LiftBuildSettings liftSettings = new LiftBuildSettings(
      new CCVRangePercentage(80, 110), false, 1.0, 2.0, 0.2f, LiftDetectionType.Automatic, LiftThicknessType.Compacted,
      new MDPRangePercentage(70, 120), false, null, null, null, null, null, null, LiftThicknessTarget, null);
  }
}
