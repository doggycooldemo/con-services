﻿using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace VSS.Serilog.Extensions
{
  public static class SerilogLoggingBuilderExtensions
  {
    /// <summary>
    /// Adds a custom logging provider.
    /// </summary>
    public static ILoggingBuilder AddProvider<T>(
      this ILoggingBuilder builder,
      Func<IServiceProvider, T> factory) where T : class, ILoggerProvider
    {
      if (builder == null)
      {
        throw new ArgumentNullException(nameof(builder));
      }

      // Because a default Microsoft logger is implemented by invoking the WebHost.CreateDefaultBuilder(args)
      // method in aspnetcore applications, we remove all providers here before adding Serilog.
      builder.ClearProviders();

      builder.Services.AddSingleton<ILoggerProvider, T>(factory);

      // Temporary fix to correct the Logging Builder not picking up the minimum logging level set in SerilogExtensions::Configure().
      builder.SetMinimumLevel(LogLevel.Debug);

      return builder;
    }
  }
}
