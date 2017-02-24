﻿using System.Net;
using VSS.TagFileAuth.Service.WebApiModels.ResultHandling;
using VSS.TagFileAuth.Service.WebApiModels.Interfaces;
using VSS.TagFileAuth.Service.WebApiModels.RaptorServicesCommon;
using VSS.TagFileAuth.Service.WebApiModels.Enums;

namespace VSS.TagFileAuth.Service.WebApiModels.Executors
{
  /// <summary>
  /// The executor which sends an alert if required for a tag file processing error.
  /// </summary>
  public class TagFileProcessingErrorExecutor : RequestExecutorContainer
  {
    /// <summary>
    /// Processes the tag file processing error request and creates an alert if required.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <returns>a TagFileProcessingErrorResult if successful</returns>      
    protected override ContractExecutionResult ProcessEx<T>(T item)
    {
      TagFileProcessingErrorRequest request = item as TagFileProcessingErrorRequest;

      bool result = false;

      if (request.assetId > 0)
      {
        //Log these errors
        /*
            request.error         alert type
         -2	UnknownProject	      UnableToDetermineProjectID
         -1 UnknownCell	          NoValidCellPassesInTagfile
          1	NoMatchingProjectDate	UnableToDetermineProjectID
          2	NoMatchingProjectArea	UnableToDetermineProjectID
          3	MultipleProjects	    UnableToDetermineProjectID
          4	InvalidSeedPosition	  UnableToDetermineProjectID
          5	InvalidOnGroundFlag	  NoValidCellPassesInTagfile
          6	InvalidPosition	      NoValidCellPassesInTagfile
         */

         result = request.error == TagFileErrorsEnum.None;
      }

      try
      {
        return TagFileProcessingErrorResult.CreateTagFileProcessingErrorResult(result);
      }
      catch
      {
        throw new ServiceException(HttpStatusCode.BadRequest,
          new ContractExecutionResult(ContractExecutionStatesEnum.InternalProcessingError, "Failed to create an alert for tag file processing error"));
      }

    }

    //protected override void ProcessErrorCodes()
    //{
    //  //Nothing to do
    //}
  }
}