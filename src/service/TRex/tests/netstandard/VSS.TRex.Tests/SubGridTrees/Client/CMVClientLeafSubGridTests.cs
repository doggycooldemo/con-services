﻿using Microsoft.Extensions.Logging;
using VSS.TRex.SubGridTrees.Client;
using VSS.TRex.SubGridTrees.Client.Types;
using VSS.TRex.SubGridTrees.Core.Utilities;
using VSS.TRex.Tests.TestFixtures;
using VSS.TRex.Types;
using Xunit;

namespace VSS.TRex.Tests.SubGridTrees.Client
{
  /// <summary>
  /// Includes tests not covered in GenericClientLeafSubGridTests
  /// </summary>
  public class CMVClientLeafSubGridTests : IClassFixture<DILoggingFixture>
  {
    private static readonly ILogger _log = VSS.TRex.Logging.Logger.CreateLogger<CMVClientLeafSubGridTests>();

    [Fact]
    public void Test_NullCells()
    {
      var cell = new SubGridCellPassDataCMVEntryRecord();
      cell.Clear();

      var clientGrid = ClientLeafSubGridFactoryFactory.CreateClientSubGridFactory().GetSubGrid(GridDataType.CCV) as ClientCMVLeafSubGrid;
      SubGridUtilities.SubGridDimensionalIterator((x, y) => Assert.True(clientGrid.Cells[x, y].Equals(cell)));
    }

    [Fact]
    public void Test_NullCell()
    {
      var clientGrid = ClientLeafSubGridFactoryFactory.CreateClientSubGridFactory().GetSubGrid(GridDataType.CCV) as ClientCMVLeafSubGrid;

      clientGrid.Cells[0, 0] = clientGrid.NullCell();
      Assert.False(clientGrid.CellHasValue(0, 0), "Cell not set to correct null value");
    }


    [Fact]
    public void DumpToLog()
    {
      var clientGrid = ClientLeafSubGridFactoryFactory.CreateClientSubGridFactory().GetSubGrid(GridDataType.CCV) as ClientCMVLeafSubGrid;
      clientGrid.DumpToLog(_log, clientGrid.ToString());
    }
  }
}
