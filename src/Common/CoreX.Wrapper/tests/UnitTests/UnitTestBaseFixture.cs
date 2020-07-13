﻿using System;
using CoreX.Interfaces;
using CoreX.Wrapper.UnitTests.Types;
using Microsoft.Extensions.DependencyInjection;

namespace CoreX.Wrapper.UnitTests
{
  public class UnitTestBaseFixture : IDisposable
  {
    private readonly IServiceProvider _serviceProvider;

    public IConvertCoordinates ConvertCoordinates => _serviceProvider.GetRequiredService<IConvertCoordinates>();

    private string _csib = null;
    public string CSIB => _csib ??= CoreX.GetCSIBFromDCFile(DCFile.GetFilePath(DCFile.BOOTCAMP_2012));

    public UnitTestBaseFixture()
    {
      _serviceProvider = new ServiceCollection()
        .AddLogging()
        .AddSingleton<IConvertCoordinates, ConvertCoordinates>()
        .BuildServiceProvider();
    }

    public void Dispose()
    { }
  }
}
