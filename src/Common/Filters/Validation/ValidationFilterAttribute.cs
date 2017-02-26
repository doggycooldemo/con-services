﻿using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using VSS.Raptor.Service.Common.Contracts;
using VSS.Raptor.Service.Common.ResultHandling;

namespace VSS.Raptor.Service.Common.Filters.Validation
{
    /// <summary>
    /// Attribute enabling obligatory validation of domain objects upon reception.
    /// </summary>
    public class ValidationFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Occurs before the action method is invoked. Validates the object.
        /// </summary>
        /// <param name="actionContext">The http action context.</param>
        /// <exception cref="ServiceException">Thrown when validation is not successfull.</exception>
        /// <exception cref="ContractExecutionResult">Built when exception is thrown.</exception>
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                throw new ServiceException(HttpStatusCode.BadRequest,
                        new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
                                JsonConvert.SerializeObject(actionContext.ModelState.Values)));
            }
        }
    }
}