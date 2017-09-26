﻿using VSS.Common.ResultsHandling;

namespace VSS.Productivity3D.WebApiModels.TagfileProcessing.ResultHandling
{
  /// <summary>
  /// REpresents response from the service after TAG file POST request
  /// </summary>
  public class TAGFilePostResult : ContractExecutionResult
  {
    /// <summary>
    /// Private constructor
    /// </summary>
    private TAGFilePostResult()
    { }

    /// <summary>
    /// Create instance of TAGFilePostResult
    /// </summary>
    public static TAGFilePostResult CreateTAGFilePostResult()
    {
      return new TAGFilePostResult();
    }
  }
}