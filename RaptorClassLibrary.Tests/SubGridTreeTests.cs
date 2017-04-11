﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VSS.VisionLink.Raptor;
using VSS.VisionLink.Raptor.SubGridTrees.Interfaces;
using VSS.VisionLink.Raptor.SubGridTrees;
using VSS.VisionLink.Raptor.SubGridTrees.Types;
using VSS.VisionLink.Raptor.Geometry;

namespace VSS.VisionLink.Raptor.RaptorClassLibrary.Tests
{
    [TestClass]
    public class SubGridTreeTests
    {
        [TestMethod]
        public void Test_SubGridTree_Creation()
        {
            // Create a tree with the default number of levels (representing cells at the on-the-ground level)
            ISubGridTree tree = new SubGridTree(SubGridTree.SubGridTreeLevels, 1.0, new SubGridFactory<NodeSubGrid, LeafSubGrid>());           
            Assert.IsTrue(tree != null, "Subgrid tree failed to construct");

            // Create a tree with one fewer than the default number of levels (representing cells which are subgrids at the on-the-ground level
            ISubGridTree tree2 = new SubGridTree(SubGridTree.SubGridTreeLevels - 1, 1.0, new SubGridFactory<NodeSubGrid, LeafSubGrid>());
            Assert.IsTrue(tree != null, "Subgrid tree failed to construct");
        }

        [TestMethod]
        public void Test_SubGridTree_Clear()
        {
            // Create a tree with the default number of levels (representing cells at the on-the-ground level)
            ISubGridTree tree = new SubGridTree(SubGridTree.SubGridTreeLevels, 1.0, new SubGridFactory<NodeSubGrid, LeafSubGrid>());

            int count;
            
            // Test clearing the tree with no elements in it
            tree.Clear();

            // Verify tree is empty
            count = 0;
            tree.ScanAllSubGrids(x => { count++; return true; });
            Assert.IsTrue(count == 0, "Subgrid tree is not empty");

            // Add a single subgrid to the tree and verify it can be cleared
            ISubGrid subgrid = new SubGrid(tree, null, 2); // Subgrid at second level, we will attach it as a child of root
            tree.Root.SetSubGrid(0, 0, subgrid);

            // Verify tree has a single subgrid other than root
            count = 0;
            tree.Root.ForEachSubGrid(x => { count++; return SubGridProcessNodeSubGridResult.OK; });
            Assert.IsTrue(count == 1, "Subgrid tree does not have a single child node in the root node");

            tree.Clear();

            // Verify tree is empty
            count = 0;
            tree.Root.ForEachSubGrid(x => { count++; return SubGridProcessNodeSubGridResult.OK; });
            Assert.IsTrue(count == 0, "Subgrid tree is not empty");
        }

        [TestMethod]
        public void Test_SubGridTree_InvalidCreation_TreeLevels()
        {
            // Test creating invalid subgrid trees
            ISubGridTree invalid = null;
            try
            {
                invalid = new SubGridTree(0, 1.0, new SubGridFactory<NodeSubGrid, LeafSubGrid>());
                Assert.Fail("SubGridTree permitted creation with invalid subgrid tree level");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentException, "Invalid exception raised for invalid argument to SubGridTree constructor");
            }

            try
            {
                invalid = new SubGridTree(10, 1.0, new SubGridFactory<NodeSubGrid, LeafSubGrid>());
                Assert.Fail("SubGridTree permitted creation with invalid subgrid tree level");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentException, "Invalid exception raised for invalid argument to SubGridTree constructor");
            }

            try
            {
                invalid = new SubGridTree(10, 1.0, new SubGridFactory<NodeSubGrid, LeafSubGrid>());
                Assert.Fail("SubGridTree permitted creation with invalid subgrid tree level");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentException, "Invalid exception raised for invalid argument to SubGridTree constructor");
            }
        }

        [TestMethod]
        public void Test_SubGridTree_InvalidCreation_CellSize()
        {
            // Test creating invalid subgrid cell sizes
            ISubGridTree invalid = null;
            try
            {
                invalid = new SubGridTree(SubGridTree.SubGridTreeLevels, 0, new SubGridFactory<NodeSubGrid, LeafSubGrid>());
                Assert.Fail("SubGridTree permitted creation with invalid subgrid tree cell size");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentException, "Invalid exception raised for invalid argument to SubGridTree constructor");
            }

            try
            {
                invalid = new SubGridTree(SubGridTree.SubGridTreeLevels, 10000000, new SubGridFactory<NodeSubGrid, LeafSubGrid>());
                Assert.Fail("SubGridTree permitted creation with invalid subgrid tree cell size");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentException, "Invalid exception raised for invalid argument to SubGridTree constructor");
            }
        }

        [TestMethod]
        public void Test_SubGridTree_InvalidCreation_SubgridFactory()
        {
            // Test creating with invalid factory
            ISubGridTree invalid = null;
            try
            {
                invalid = new SubGridTree(SubGridTree.SubGridTreeLevels, 1.0, null);
                Assert.Fail("SubGridTree permitted creation with invalid subgrid tree cell size");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentException, "Invalid exception raised for invalid argument to SubGridTree constructor");
            }
        }

        [TestMethod]
        public void Test_SubGridTree_ScanSubGrids_WorldExtent()
        {
            Assert.Fail("Not Implemented");
        }

        [TestMethod]
        public void Test_SubGridTree_ScanSubGrids_CellExtent()
        {
            Assert.Fail("Not Implemented");
        }

        [TestMethod]
        public void Test_SubGridTree_ScanSubGrids_All()
        {
            Assert.Fail("Not Implemented");
        }

        [TestMethod]
        public void Test_SubGridTree_FullGridExtent()
        {
            ISubGridTree tree = new SubGridTree(SubGridTree.SubGridTreeLevels, 1.0, new SubGridFactory<NodeSubGrid, LeafSubGrid>());

            BoundingWorldExtent3D extent = tree.FullGridExtent();

            // World extents cover the entire possible coordiante space a real world coordinate system will use
            Assert.IsTrue(tree.CellSize == 1.0 &&
                          extent.MinX < extent.MaxX && extent.MinY < extent.MaxY &&
                          (Math.Abs(extent.MinX) - (tree.IndexOriginOffset * tree.CellSize)) < 0.1 &&
                          (Math.Abs(extent.MinY) - (tree.IndexOriginOffset * tree.CellSize)) < 0.1 &&
                          (Math.Abs(extent.MaxX) - (tree.IndexOriginOffset * tree.CellSize)) < 0.1 &&
                          (Math.Abs(extent.MaxX) - (tree.IndexOriginOffset * tree.CellSize)) < 0.1,
                          "World grid extents are not the expected size");
        }

        [TestMethod]
        public void Test_SubGridTree_FullCellExtent()
        {
            ISubGridTree tree = new SubGridTree(SubGridTree.SubGridTreeLevels, 1.0, new SubGridFactory<NodeSubGrid, LeafSubGrid>());

            BoundingIntegerExtent2D extent = tree.FullCellExtent();

            // Cell extents cover the non-negative (upper-right quadrant) cartesian coordinates that are used to address
            // the possible cell locations in a sub grid tree in the range (0, 0) -> (2^20, 2^30) where
            // tree.IndexOriginOffset should be 2^29
            Assert.IsTrue(extent.MinX == 0 && extent.MinY == 0 &&
                          extent.MaxX == (2 * tree.IndexOriginOffset) && extent.MaxY == (2 * tree.IndexOriginOffset),
                          "Cell grid extents are not the expected size");
        }

        [TestMethod]
        public void Test_SubGridTree_ConstructPathToCell_CreateLeaf()
        {
            ISubGridTree tree = new SubGridTree(SubGridTree.SubGridTreeLevels, 1.0, new SubGridFactory<NodeSubGrid, LeafSubGrid>());

            // Add a leaf node with ConstructPathToCell with leaf creation path type (in the bottom left corner 
            // of the cell address space and verify a new leaf subgrid came back)
            ISubGrid subgrid = tree.ConstructPathToCell(0, 0, SubGridPathConstructionType.CreateLeaf);

            Assert.IsTrue(subgrid != null, "ConstructPathToCell failed to create a new leaf");

            Assert.IsTrue(subgrid != null && subgrid.Level == tree.NumLevels && subgrid.Owner == tree && subgrid.Parent != null,
                "Subgrid added to tree is not correctly set up");
        }

        [TestMethod]
        public void Test_SubGridTree_ConstructPathToCell_ReturnExistingLeafOnly()
        {
            ISubGridTree tree = new SubGridTree(SubGridTree.SubGridTreeLevels, 1.0, new SubGridFactory<NodeSubGrid, LeafSubGrid>());

            // Add a leaf node with ConstructPathToCell with leaf creation path type (in the bottom left corner 
            // of the cell address space and verify a new leaf subgrid came back)
            ISubGrid subgrid = tree.ConstructPathToCell(0, 0, SubGridPathConstructionType.CreateLeaf);

            // Retrieve the newly created leaf node with ConstructPathToCell with existing leaf only path type 
            // (in the bottom left corner of the cell address space and verify a new leaf node came back)
            ISubGrid subgrid2 = tree.ConstructPathToCell(0, 0, SubGridPathConstructionType.ReturnExistingLeafOnly);

            Assert.IsTrue(subgrid != null, "Failed to retrieve leaf subgrid previously added");

            Assert.IsTrue(subgrid == subgrid2 && subgrid.Equals(subgrid2),
                "Retrieve leaf subgrid is not the same as the one added");
        }

        [TestMethod]
        public void Test_SubGridTree_ConstructPathToCell_CreatePathToLeaf()
        {
            ISubGridTree tree = new SubGridTree(SubGridTree.SubGridTreeLevels, 1.0, new SubGridFactory<NodeSubGrid, LeafSubGrid>());

            // Add a node subgrid with ConstructPathToCell with CreatePathToLeaf creation path type (in the bottom left corner 
            // of the cell address space and verify a new node subgrid came back)
            ISubGrid subgrid = tree.ConstructPathToCell(0, 0, SubGridPathConstructionType.CreatePathToLeaf);

            Assert.IsTrue(subgrid != null && subgrid.Level == tree.NumLevels - 1,
                          "Failed to create a node subgrid down to NumLevels - 1 in tree");
        }

        [TestMethod]
        public void Test_SubGridTree_CountLeafSubgridsInMemory()
        {
            ISubGridTree tree = new SubGridTree(SubGridTree.SubGridTreeLevels, 1.0, new SubGridFactory<NodeSubGrid, LeafSubGrid>());

            Assert.IsTrue(tree.CountLeafSubgridsInMemory() == 0, "Leaf subgrids in memory is not zero for empty/new tree.");

            // Add a leaf node and check the count is now 1
            ISubGrid subgrid = tree.ConstructPathToCell(0, 0, SubGridPathConstructionType.CreateLeaf);

            Assert.IsTrue(tree.CountLeafSubgridsInMemory() == 1, "Leaf subgrid count is not 1 as expected, it is {0}", tree.CountLeafSubgridsInMemory());
        }

        [TestMethod]
        public void Test_SubGridTree_CalculateIndexOfCellContainingPosition()
        {
            ISubGridTree tree = new SubGridTree(SubGridTree.SubGridTreeLevels, 1.0, new SubGridFactory<NodeSubGrid, LeafSubGrid>());

            uint CellX, CellY;

            // Find the location of the cell that contains the world position of (0.1, 0.1)
            Assert.IsTrue(tree.CalculateIndexOfCellContainingPosition(0.01, 0.01, out CellX, out CellY),
                "CalculateIndexOfCellContainingPosition failed to return a cell position for (0.01, 0.01)");

            // This location should be the cell exactly at the cell location of (IndexOriginOffset, IndexOriginOffset)
            // as this maps the center of the positive north east quadrant addressable cells onto the origin of the 
            // positive and negative world coordinate system

            Assert.IsTrue(CellX == tree.IndexOriginOffset && CellY == tree.IndexOriginOffset,
                          "Cell location not at the origin [IndexOriginOffset, IndexOriginOffset] as expected");
        }

        [TestMethod]
        public void Test_SubGridTree_LocateSubGridContaining_SpecificLevel()
        {
            Assert.Fail("Not Implemented");
        }

        [TestMethod]
        public void Test_SubGridTree_LocateSubGridContaining_BottomLevel()
        {
            Assert.Fail("Not Implemented");
        }

        [TestMethod]
        public void Test_SubGridTree_LocateSubGridContaining_LocateClosestSubGridContaining()
        {
            Assert.Fail("Not Implemented");
        }

        [TestMethod]
        public void Test_SubGridTree_GetCellCenterPosition()
        {
            ISubGridTree tree = new SubGridTree(SubGridTree.SubGridTreeLevels, 1.0, new SubGridFactory<NodeSubGrid, LeafSubGrid>());
            double cx, cy;

            // Get a cell at the origin of the world coordinate system and test its center location given a 1m cell size
            tree.GetCellCenterPosition(tree.IndexOriginOffset, tree.IndexOriginOffset, out cx, out cy);

            Assert.IsTrue(Math.Abs(cx) - 0.5 < 0.001 && Math.Abs(cy) - 0.5 < 0.001,
                "Cell center for (IndexOriginOffset, IndexOriginOffset) <> (0.5, 0.5) as expected");
        }

        [TestMethod]
        public void Test_SubGridTree_GetCellOriginPosition()
        {
            ISubGridTree tree = new SubGridTree(SubGridTree.SubGridTreeLevels, 1.0, new SubGridFactory<NodeSubGrid, LeafSubGrid>());
            double cx, cy;

            // Get a cell at the origin of the world coordinate system and test its origin location given a 1m cell size
            tree.GetCellOriginPosition(tree.IndexOriginOffset, tree.IndexOriginOffset, out cx, out cy);

            Assert.IsTrue(Math.Abs(cx) < 0.001 && Math.Abs(cy) < 0.001,
                "Cell origin for (IndexOriginOffset, IndexOriginOffset) <> (0.0, 0.0) as expected");
        }

        [TestMethod]
        public void Test_SubGridTree_GetCellExtents()
        {
            ISubGridTree tree = new SubGridTree(SubGridTree.SubGridTreeLevels, 1.0, new SubGridFactory<NodeSubGrid, LeafSubGrid>());

            // Get a cell at the origin of the world coordinate system and test its extents given a 1m cell size
            BoundingWorldExtent3D extent = tree.GetCellExtents(tree.IndexOriginOffset, tree.IndexOriginOffset);

            Assert.IsTrue(Math.Abs(extent.MinX) < 0.001 && Math.Abs(extent.MinY) < 0.001 &&
                          (Math.Abs(extent.MinX) - 1.0) < 0.001 && (Math.Abs(extent.MinY) - 1.0) < 0.001,
                          "Cell extents for (IndexOriginOffset, IndexOriginOffset) <> (0.0, 0.0 -> 1.0, 1.0) as expected");
        }

        [TestMethod]
        public void Test_SubGridTree_CreateUnattachedLeaf()
        {
            ISubGridTree tree = new SubGridTree(SubGridTree.SubGridTreeLevels, 1.0, new SubGridFactory<NodeSubGrid, LeafSubGrid>());

            // Ask for an unattached leaf subgrid and verify it checks out
            ILeafSubGrid leaf = tree.CreateUnattachedLeaf();

            Assert.IsTrue(leaf != null, "CreateUnattachedLeaf failed to produce a leaf subgrid");
            Assert.IsTrue(leaf.Level == tree.NumLevels && leaf.Owner == tree,
                "Leaf not configured correctly");
        }
    }
}
