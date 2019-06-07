﻿using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.MefExtensions;
using Microsoft.Practices.Prism.Modularity;
using Morph.Contracts;
using Morph.Core.Utility;
using Morph.Module.Services;
using Morph.Services.Engine.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Windows;
using Microsoft.Practices.ServiceLocation;
using Morph.Services.Core.Interfaces;

namespace DrainageTest
{
  class BootStrapper : MefBootstrapper
  {
    protected override void ConfigureAggregateCatalog()
    {
      base.ConfigureAggregateCatalog();
      this.AggregateCatalog.Catalogs.Add((ComposablePartCatalog)new AssemblyCatalog(typeof(Program).Assembly));
      this.AggregateCatalog.Catalogs.Add((ComposablePartCatalog)new AssemblyCatalog(typeof(InterfaceTypes).Assembly));
      this.AggregateCatalog.Catalogs.Add((ComposablePartCatalog)new AssemblyCatalog(typeof(UnitConverter).Assembly));
      this.AggregateCatalog.Catalogs.Add((ComposablePartCatalog)new AssemblyCatalog(typeof(ServicesModule).Assembly));
      this.AggregateCatalog.Catalogs.Add((ComposablePartCatalog)new AssemblyCatalog(typeof(EngineService).Assembly));
    }

    protected override DependencyObject CreateShell()
    {
      return (DependencyObject)null;
    }

    public new void Run()
    {
      this.Logger = this.CreateLogger();
      if (this.Logger == null)
        throw new InvalidOperationException("Resources.NullLoggerFacadeException");
      this.Logger.Log("Resources.LoggerWasCreatedSuccessfully", Category.Debug, Priority.Low);
      this.Logger.Log("Resources.CreatingModuleCatalog", Category.Debug, Priority.Low);
      this.ModuleCatalog = this.CreateModuleCatalog();
      if (this.ModuleCatalog == null)
        throw new InvalidOperationException("Resources.NullModuleCatalogException");
      this.Logger.Log("Resources.ConfiguringModuleCatalog", Category.Debug, Priority.Low);
      this.ConfigureModuleCatalog();
      this.Logger.Log("Resources.CreatingCatalogForMEF", Category.Debug, Priority.Low);
      this.AggregateCatalog = this.CreateAggregateCatalog();
      this.Logger.Log("Resources.ConfiguringCatalogForMEF", Category.Debug, Priority.Low);
      this.ConfigureAggregateCatalog();
      this.RegisterDefaultTypesIfMissing();
      this.Logger.Log("Resources.CreatingMefContainer", Category.Debug, Priority.Low);
      this.Container = this.CreateContainer();
      if (this.Container == null)
        throw new InvalidOperationException("Resources.NullCompositionContainerException");
      this.Logger.Log("Resources.ConfiguringMefContainer", Category.Debug, Priority.Low);
      this.ConfigureContainer();
    //  this.Container.ComposeExportedValue<ILogger>(this.Logger as ILogger);
      this.Logger.Log("Resources.ConfiguringServiceLocatorSingleton", Category.Debug, Priority.Low);
      this.ConfigureServiceLocator();
      IEnumerable<Lazy<object, object>> exports =
        this.Container.GetExports(typeof(IModuleManager), (Type)null, (string)null);

      if (exports != null && exports.Count<Lazy<object, object>>() > 0)
      {
        this.Logger.Log("Resources.InitializingModules", Category.Debug, Priority.Low);
        this.InitializeModules(); 
      }

      this.Logger.Log("Resources.BootstrapperSequenceCompleted", Category.Debug, Priority.Low);
    }
  }
}
