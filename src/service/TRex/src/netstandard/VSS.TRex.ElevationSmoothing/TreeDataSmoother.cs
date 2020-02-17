﻿using System;
using VSS.TRex.SubGridTrees;
using VSS.TRex.SubGridTrees.Types;

namespace VSS.TRex.DataSmoothing
{
  /// <summary>
  /// Implements the capability to take a sub grid tree containing queried elevation data and apply algorithmic smoothing to the data
  /// </summary>
  public class TreeDataSmoother<TV> : ITreeDataSmoother<TV>
  {
    private readonly IConvolutionTools<TV> _convolutionTools;
    private readonly int _contextSize;
    private readonly IConvolutionAccumulator<TV> _accumulator;
    private readonly Func<IConvolutionAccumulator<TV>, int, IConvolver<TV>> _convolverFactory;

    public TreeDataSmoother(
      IConvolutionTools<TV> convolutionTools, int contextSize,
      IConvolutionAccumulator<TV> accumulator,
      Func<IConvolutionAccumulator<TV>, int, IConvolver<TV>> convolverFactory)
    {
      _convolutionTools = convolutionTools ?? throw new ArgumentException("ConvolutionTools is null", nameof(convolutionTools));
      _contextSize = contextSize;
      _accumulator = accumulator;
      _convolverFactory = convolverFactory;
    }

    public GenericSubGridTree<TV, GenericLeafSubGrid<TV>> Smooth(GenericSubGridTree<TV, GenericLeafSubGrid<TV>> source)
    {

      var result = new GenericSubGridTree<TV, GenericLeafSubGrid<TV>>(source.NumLevels, source.CellSize);
      var convolver = _convolverFactory(_accumulator, _contextSize);

      source.ScanAllSubGrids(leaf =>
      {
        var smoothedLeaf = result.ConstructPathToCell(leaf.OriginX, leaf.OriginY, SubGridPathConstructionType.CreateLeaf) as GenericLeafSubGrid<float>;
        _convolutionTools.Convolve(leaf as GenericLeafSubGrid<TV>, smoothedLeaf as GenericLeafSubGrid<TV>, convolver);
        return true;
      });

      return result;
    }
  }
}
