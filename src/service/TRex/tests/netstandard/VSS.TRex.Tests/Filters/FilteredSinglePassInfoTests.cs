﻿using System;
using VSS.TRex.Filters;
using VSS.TRex.Filters.Models;
using Xunit;

namespace VSS.TRex.Tests.Filters
{
        public class FilteredSinglePassInfoTests
    {
        [Fact]
        public void Test_FilteredSinglePass_Creation()
        {
            FilteredSinglePassInfo info = new FilteredSinglePassInfo();

            Assert.Equal(0, info.PassCount);
        }

        [Fact(Skip = "Not Implemented")]
        public void Test_FilteredSinglePass_Clear()
        {
            Assert.True(false);
        }
    }
}
