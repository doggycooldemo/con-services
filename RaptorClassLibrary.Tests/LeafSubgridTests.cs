﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VSS.VisionLink.Raptor.SubGridTrees.Interfaces;
using VSS.VisionLink.Raptor.SubGridTrees;
using VSS.VisionLink.Raptor;

namespace VSS.VisionLink.Raptor.RaptorClassLibrary.Tests
{
    [TestClass]
    public class LeafSubgridTests
    {
        [TestMethod]
        public void Test_LeafSubgrid_Creation()
        {
            ISubGridTree tree = new SubGridTree(SubGridTree.SubGridTreeLevels, 1.0, new SubGridFactory<NodeSubGrid, LeafSubGrid>());
            ILeafSubGrid leaf = null;

            // Test creation of a leaf node without an owner tree
            try
            {
                leaf = new LeafSubGrid(null, null, (byte)(tree.NumLevels + 1));
                Assert.Fail("Was able to create a leaf subgrid with no owning tree");
            }
            catch (Exception)
            {
                // As expected
            }

            // Test creation of a leaf node at an inappropriate level
            try
            {
                leaf = new LeafSubGrid(tree, null, (byte)(tree.NumLevels + 1));
                Assert.Fail("Was able to create a leaf subgrid at an inappropriate level");
            }
            catch (Exception)
            {
                // As expected
            }

            leaf = new LeafSubGrid(tree, null, tree.NumLevels);

            Assert.IsTrue(leaf != null && leaf.Level == tree.NumLevels);
        }

        [TestMethod]
        public void Test_LeafSubgrid_IsEmpty()
        {
            ISubGridTree tree = new SubGridTree(SubGridTree.SubGridTreeLevels, 1.0, new SubGridFactory<NodeSubGrid, LeafSubGrid>());
            ILeafSubGrid leaf = new LeafSubGrid(tree, null, tree.NumLevels);

            // Base leaf classes don't implement CellHasValue(), so this call should fail with an exception
            try
            {
                bool isEmpty = leaf.IsEmpty();

                Assert.Fail("Base LeafSubGrid class did not throw an exception due to unimplemented CellHasValu()");
            } catch (Exception)
            {
                // As expected
            }
        }
    }
}
