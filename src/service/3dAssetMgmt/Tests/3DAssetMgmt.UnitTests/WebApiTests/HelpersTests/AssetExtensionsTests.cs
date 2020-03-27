﻿using System;
using VSS.MasterData.Repositories.DBModels;
using VSS.Productivity3D.AssetMgmt3D.Extensions;
using Xunit;

namespace AssetMgmt.UnitTests.WebApiTests.HelpersTests
{
  public class AssetExtensionsTests : TestBase
  {
    [Fact]
    public void Should_return_correct_result_When_given_good_inputs()
    {
      var assets = new []
                   {
                     new Asset { AssetUID = Guid.NewGuid().ToString() }
                   };

      var result = assets.ConvertDbAssetToDisplayModel();

      Assert.Equal(0, result.Code);
      Assert.Equal("success", result.Message);
      Assert.Single(result.assetIdentifiers);
    }
  }
}