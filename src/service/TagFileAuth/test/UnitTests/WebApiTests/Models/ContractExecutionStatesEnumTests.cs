﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using VSS.Productivity3D.TagFileAuth.Models;

namespace WebApiTests.Models
{
  [TestClass]
  public class ContractExecutionStatesEnumTests
  {
    [TestMethod]
    public void DynamicAddwithOffsetTest()
    {
      var contractExecutionStatesEnum = new ContractExecutionStatesEnum();
      Assert.AreEqual(61, contractExecutionStatesEnum.DynamicCount);
      Assert.AreEqual("AssetId, if present, must be >= -1", contractExecutionStatesEnum.FirstNameWithOffset(2));
      Assert.AreEqual("DeviceType is invalid", contractExecutionStatesEnum.FirstNameWithOffset(30));
    }
  }
}
