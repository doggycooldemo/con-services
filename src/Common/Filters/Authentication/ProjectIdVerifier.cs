﻿using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Filters;
using VSS.Common.Exceptions;
using VSS.MasterData.Models.ResultHandling.Abstractions;
using VSS.Productivity3D.Common.Filters.Authentication.Models;

namespace VSS.Productivity3D.Common.Filters.Authentication
{
  /// <summary>
  /// Validation filter attribute for the ProjectId.
  /// </summary>
  public class ProjectIdVerifier : ActionFilterAttribute
  {
    private const string NAME = "projectId";

    /// <summary>
    /// Gets or sets whether the Filter will check for and reject archived Projects.
    /// </summary>
    public bool AllowArchivedState { get; set; }

    public override void OnActionExecuting(ActionExecutingContext actionContext)
    {
      object projectIdValue = null;

      if (actionContext.ActionArguments.ContainsKey("request"))
      {
        var request = actionContext.ActionArguments["request"];

        // Ignore any query parameter called 'request'.
        if (request.GetType() != typeof(string))
        {
          projectIdValue = request.GetType()
                                  .GetProperty(NAME, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                                  ?.GetValue(request);
        }
      }

      if (actionContext.ActionArguments.ContainsKey(NAME))
      {
        projectIdValue = actionContext.ActionArguments[NAME];
      }

      if (!(projectIdValue is long))
      {
        return;
      }

      // RaptorPrincipal will handle the failure case where project isn't found.
      var projectDescriptor = ((RaptorPrincipal) actionContext.HttpContext.User).GetProject((long)projectIdValue).Result;

      if (AllowArchivedState && projectDescriptor.IsArchived)
      {
        throw new ServiceException(HttpStatusCode.Unauthorized,
          new ContractExecutionResult(ContractExecutionStatesEnum.AuthError,
            "Don't have write access to the selected project."));
      }
    }
  }
}
