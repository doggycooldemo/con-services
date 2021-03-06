﻿using System;
using VSS.TRex.SubGridTrees.Interfaces;

namespace VSS.TRex.SubGridTrees.Factories
{
    /// <summary>
    /// The factory responsible for creating sub grids within a sub grid tree.
    /// </summary>
    /// <typeparam name="Node"></typeparam>
    /// <typeparam name="Leaf"></typeparam>
    public class SubGridFactory<Node, Leaf> : ISubGridFactory 
        where Node : INodeSubGrid, new()
        where Leaf : ILeafSubGrid, new()
    {
        /// <summary>
        /// Construct either a node or a leaf sub grid for the given sub grid tree at the given level using the generic
        /// types Node and Leaf.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="treeLevel"></param>
        /// <returns>An ISubGrid interface representing the newly created node or leaf sub grid</returns>
        public virtual ISubGrid GetSubGrid(ISubGridTree tree, byte treeLevel)
        {
            // Ensure the requested tree level is valid for the given tree
            if (treeLevel < 1 || treeLevel > tree.NumLevels)
            {
                throw new ArgumentException($"Invalid treeLevel in sub grid factory: {treeLevel}, range is 1-{tree.NumLevels}", nameof(treeLevel));
            }

            if (treeLevel < tree.NumLevels)
            {
                return new Node
                {
                    Owner = tree,
                    Level = treeLevel
                };
            }

            return new Leaf
            {
                Owner = tree,
                Level = treeLevel
            };
        }
    }
}
