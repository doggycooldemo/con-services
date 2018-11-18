﻿using VSS.TRex.Designs.GridFabric.Responses;
using VSS.TRex.Designs.Models;
using VSS.TRex.Tests.TestFixtures;
using Xunit;

namespace VSS.TRex.Tests.BinarizableSerialization.Designs
{
  public class ToFromBinary_CalculateDesignProfileResponse : IClassFixture<DILoggingFixture>
  {
    [Fact]
    public void Test_CalculateDesignProfileResponse_Simple()
    {
      SimpleBinarizableInstanceTester.TestClass<CalculateDesignProfileResponse>("Empty CalculateDesignProfileResponse not same after round trip serialisation");
    }

    [Fact]
    public void Test_CalculateDesignProfileResponse()
    {
      var response = new CalculateDesignProfileResponse
      {
        Profile = new [] {new XYZS(0, 0, 0, 0, 0), new XYZS(100, 101, 102, 103, 104) }
      };

      SimpleBinarizableInstanceTester.TestClass(response, "Custom CalculateDesignProfileResponse not same after round trip serialisation");
    }
  }
}
