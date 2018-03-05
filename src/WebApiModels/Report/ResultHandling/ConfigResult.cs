﻿using VSS.MasterData.Models.ResultHandling.Abstractions;

namespace VSS.Productivity3D.WebApiModels.Report.ResultHandling
{
  public class ConfigResult : ContractExecutionResult
  {
    /// <summary>
    /// Provides current Raptor configuration as XML.
    /// </summary>
    /// <value>
    /// The configuration.
    /// </value>
    public string Configuration { get; private set; }


    /// <summary>
    /// Private constructor
    /// </summary>
    private ConfigResult()
    { }

    /// <summary>
    /// Create instance of ConfigResult
    /// </summary>
    public static ConfigResult CreateConfigResult(string config)
    {
      return new ConfigResult
      {
        Configuration = config
      };
    }
  }
}