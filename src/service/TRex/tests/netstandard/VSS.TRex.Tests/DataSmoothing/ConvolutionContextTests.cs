﻿using FluentAssertions;
using VSS.TRex.DataSmoothing;
using VSS.TRex.SubGridTrees;
using VSS.TRex.SubGridTrees.Core;
using VSS.TRex.SubGridTrees.Interfaces;
using VSS.TRex.SubGridTrees.Types;
using VSS.TRex.Tests.TestFixtures;
using VSS.TRex.Types.CellPasses;
using Xunit;

namespace VSS.TRex.Tests.DataSmoothing
{
  public class ConvolutionContextTests : IClassFixture<DILoggingFixture>
  {
    private GenericSubGridTree<float, GenericLeafSubGrid_Float> ConstructSingleSubGridElevationSubGridTreeAtOrigin(float elevation)
    {
      var tree = new GenericSubGridTree<float, GenericLeafSubGrid_Float>();

      var subGrid = tree.ConstructPathToCell(SubGridTreeConsts.DefaultIndexOriginOffset, SubGridTreeConsts.DefaultIndexOriginOffset,
             SubGridPathConstructionType.CreateLeaf) as GenericLeafSubGrid<float>;
      DataSmoothingTestUtilities.ConstructElevationSubGrid(subGrid, elevation);

      return tree;
    }

    [Fact]
    public void Creation()
    {
      var context = new ConvolutionSubGridContext<GenericLeafSubGrid<float>, float>();
      context.Should().NotBeNull();
    }

    [Fact]
    public void SingleSubGrid_SameElevation()
    {
      var tree = ConstructSingleSubGridElevationSubGridTreeAtOrigin(0.0f);
      var subGrid = tree.LocateSubGridContaining(SubGridTreeConsts.DefaultIndexOriginOffset, SubGridTreeConsts.DefaultIndexOriginOffset, SubGridTreeConsts.SubGridTreeLevels) as GenericLeafSubGrid<float>;
      subGrid.Should().NotBeNull();

      // Create the context with a single sub grid and no neighbors
      var context = new ConvolutionSubGridContext<GenericLeafSubGrid<float>, float>(subGrid, CellPassConsts.NullHeight);
      context.Should().NotBeNull();

      // Check all acquired values in the single sub grid are zero
      for (var i = 0; i < SubGridTreeConsts.SubGridTreeDimension; i++)
      {
        for (var j = 0; j < SubGridTreeConsts.SubGridTreeDimension; j++)
        {
          context.Value(i, j).Should().Be(0.0f);
        }
      }

      // Check all values in the sub grids surrounding the single sub grid are null
      for (var i = -SubGridTreeConsts.SubGridTreeDimension; i < 2 * SubGridTreeConsts.SubGridTreeDimension; i++)
      {
        for (var j = -SubGridTreeConsts.SubGridTreeDimension; j < 2 * SubGridTreeConsts.SubGridTreeDimension; j++)
        {
          if (i >= 0 && i < SubGridTreeConsts.SubGridTreeDimension && j >= 0 && j < SubGridTreeConsts.SubGridTreeDimension)
            context.Value(i, j).Should().Be(0.0f);
          else
            context.Value(i, j).Should().Be(CellPassConsts.NullHeight);
        }
      }
    }
  }
}
