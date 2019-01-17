﻿using System;
using System.IO;
using FluentAssertions;
using TAGFiles.Tests.Utilities;
using VSS.TRex.DI;
using VSS.TRex.SubGridTrees.Server.Interfaces;
using VSS.TRex.Tests.TestFixtures;
using Xunit;

namespace TAGFiles.Tests
{
  public class TAGFileCellPassTests : IClassFixture<DITagFileFixture>
  {
    private static readonly ICell_NonStatic_MutationHook hook = DIContext.Obtain<ICell_NonStatic_MutationHook>();

    [Fact]
    public void Test_TAGFileCellPassGeneration_Default()
    {
      // Setup the mutation hook to capture cell pass generation
      string cellPassFileName = $@"c:\temp\TRex-UnitTests-TAGFiles-CellPasses-{DateTime.Now.Ticks}.txt";
      var writer = new CellPassWriter(new StreamWriter(new FileStream(cellPassFileName, FileMode.CreateNew, FileAccess.ReadWrite)));
      hook.SetActions(writer); 

      // Read in the TAG file
      DITagFileFixture.ReadTAGFile("TestTAGFile.tag");

      // Close the writer and clean up the DI context
      hook.ClearActions();
      writer.Close();

      // Examine the result
      var lines = File.ReadAllLines(cellPassFileName);
      lines.Length.Should().Be(16525);
    }
  }
}

