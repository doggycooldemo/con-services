﻿using System;
using Microsoft.Extensions.DependencyInjection;
using VSS.Productivity3D.Models.Enums;
using VSS.TRex.DataSmoothing;
using VSS.TRex.DI;
using VSS.TRex.Rendering.Abstractions;
using VSS.TRex.Rendering.Implementations.Core2;

namespace VSS.TRex.Tests.TestFixtures
{
  public class DIRenderingFixture : DITAGFileAndSubGridRequestsWithIgniteFixture
  {
    private static IDataSmoother TileRenderingSmootherFactoryMethod(DisplayMode key)
    {
      return key switch
      {
        DisplayMode.Height => new ElevationArraySmoother(new ConvolutionTools<float>(), ConvolutionMaskSize.Mask3X3, NullInfillMode.InfillNullValues),
        DisplayMode.CutFill => new ElevationArraySmoother(new ConvolutionTools<float>(), ConvolutionMaskSize.Mask3X3, NullInfillMode.InfillNullValues),
        _ => null
      };
    }

    public DIRenderingFixture()
    {
      DIBuilder
        .Continue()
        .Add(x => x.AddSingleton<IRenderingFactory>(new RenderingFactory()))
        .Add(x => x.AddSingleton<Func<DisplayMode, IDataSmoother>>(provider => TileRenderingSmootherFactoryMethod))
        .Complete();
    }
  }
}
