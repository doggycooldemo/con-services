﻿using System;
using System.Net;
using VSS.Common.Exceptions;
using VSS.MasterData.Models.ResultHandling.Abstractions;
using Microsoft.Extensions.Logging;
using VSS.TRex.DI;
using VSS.TRex.SiteModels.Interfaces;

namespace VSS.TRex.Gateway.Common.Helpers
{
  public static class GatewayHelper
  {
    private static readonly ILogger Log = Logging.Logger.CreateLogger("GatewayHelper");

    public static ISiteModel ValidateAndGetSiteModel(Guid projectUid, string method, bool createIfNotExists = false)
    {
      if (projectUid == Guid.Empty)
        throw new ServiceException(HttpStatusCode.BadRequest, new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError, "siteModel ID format is invalid"));

      var siteModel = DIContext.Obtain<ISiteModels>().GetSiteModel(projectUid, createIfNotExists);
      if (siteModel == null)
      {
        var message = $"{method}: SiteModel: {projectUid} not found {(createIfNotExists ? " and unable to be created" : "")}";
        Log.LogError(message);
        throw new ServiceException(HttpStatusCode.BadRequest, new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError, message));
      }

      return siteModel;
    }

    public static ISiteModel ValidateAndGetSiteModel(string projectUid, string method, bool createIfNotExists = false)
    {
      if (string.IsNullOrEmpty(projectUid))
        throw new ServiceException(HttpStatusCode.BadRequest, new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError, "siteModel ID format is invalid"));

      return ValidateAndGetSiteModel(Guid.Parse(projectUid), method, createIfNotExists);
    }
  }
}
