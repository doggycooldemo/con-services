﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using VSS.VisionLink.Raptor.Executors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using VSS.VisionLink.Raptor.TAGFiles.Tests;
using VSS.VisionLink.Raptor.TAGFiles.Types;

namespace VSS.VisionLink.Raptor.Executors.Tests
{
    [TestClass()]
    public class TAGFileConverterTests
    {
        [TestMethod()]
        public void Test_TAGFileConverter_Creation()
        {
            TAGFileConverter converter = new TAGFileConverter();

            Assert.IsTrue(converter.Machine == null &&
                converter.SiteModel == null &&
                converter.SiteModelGridAggregator == null &&
                converter.MachineTargetValueChangesAggregator == null &&
                converter.ReadResult == TAGReadResult.NoError &&
                converter.ProcessedCellPassCount == 0 &&
                converter.ProcessedEpochCount == 0,
                "TAGFileConverter not created as expected");
        }

        [TestMethod()]
        public void Test_TAGFileConverter_Execute()
        {
            TAGFileConverter converter = new TAGFileConverter();

            Assert.IsTrue(converter.Execute(new FileStream(TAGTestConsts.TestDataFilePath() + "TAGFiles\\TestTAGFile.tag", FileMode.Open, FileAccess.Read)),
                "Converter execute returned false");

            Assert.IsTrue(converter.Machine != null &&
                converter.SiteModelGridAggregator != null &&
                converter.MachineTargetValueChangesAggregator != null &&
                converter.ReadResult == TAGReadResult.NoError &&
                converter.ProcessedCellPassCount == 16525 &&
                converter.ProcessedEpochCount == 1478,
                "TAGFileConverter did not execute as expected");
        }
    }
}