﻿using MasterDataProxies;
using MasterDataProxies.Interfaces;
using MasterDataProxies.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Principal;
using System.Threading.Tasks;
using VSS.Authentication.JWT;
using VSS.GenericConfiguration;

namespace ProjectWebApi.Filters
{
  /// <summary>
  /// authentication
  /// </summary>
  public class TIDAuthentication
  {
    private readonly RequestDelegate _next;
    private ILogger<TIDAuthentication> log;
    private readonly ICustomerProxy customerProxy;
    private readonly IConfigurationStore store;


    /// <summary>
    /// Initializes a new instance of the <see cref="TIDAuthentication"/> class.
    /// </summary>
    /// <param name="next">The next.</param>
    /// <param name="customerProxy">The customer proxy.</param>
    /// <param name="store">The store.</param>
    /// <param name="logger">The logger.</param>
    public TIDAuthentication(RequestDelegate next,
      ICustomerProxy customerProxy,
      IConfigurationStore store,
      ILoggerFactory logger)
    {
      log = logger.CreateLogger<TIDAuthentication>();
      this.customerProxy = customerProxy;
      _next = next;
      this.store = store;
    }

    /// <summary>
    /// Invokes the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns></returns>
    public async Task Invoke(HttpContext context)
    {
      if (!context.Request.Path.Value.Contains("/swagger/"))
      {
        bool isApplicationContext = false;
        string applicationName = "";
        string userUid = "";
        string userEmail = "";

        string authorization = context.Request.Headers["X-Jwt-Assertion"];
        string customerUid = context.Request.Headers["X-VisionLink-CustomerUID"];

        // If no authorization header found, nothing to process further
        if (string.IsNullOrEmpty(authorization) || string.IsNullOrEmpty(customerUid))
        {
          log.LogWarning("No account selected for the request");
          await SetResult("No account selected", context);
          return;
        }
        //log.LogTrace("JWT token used: {0}", authorization);
        try
        {
          var jwtToken = new TPaaSJWT(authorization);
          isApplicationContext = jwtToken.IsApplicationToken;
          applicationName = jwtToken.ApplicationName;
          userEmail = jwtToken.EmailAddress;
          userUid = isApplicationContext
            ? jwtToken.ApplicationId
            : jwtToken.UserUid.ToString();
        }
        catch (Exception e)
        {
          log.LogWarning("Invalid JWT token with exception {0}", e.Message);
          await SetResult("Invalid authentication", context);
          return;
        }

        //Set calling context Principal
        context.User = new TIDCustomPrincipal(new GenericIdentity(userUid), customerUid, userEmail, isApplicationContext);

        //If this is an application context do not validate user-customer
        if (isApplicationContext)
        {
          log.LogInformation(
            "Authorization: Calling context is Application Context for Customer: {0} Application: {1} ApplicationName: {2}",
            customerUid, userUid, applicationName);
          await _next.Invoke(context);
          return;
        }

        // User must have be authenticated against this customer
        try
        {
          CustomerDataResult customerResult =
            await customerProxy.GetCustomersForMe(userUid, context.Request.Headers.GetCustomHeaders());
          if (customerResult.status != 200 || customerResult.customer == null ||
              customerResult.customer.Count < 1 ||
              !customerResult.customer.Exists(x => x.uid == customerUid))
          {
            var error = $"User {userUid} is not authorized to configure this customer {customerUid}";
            log.LogWarning(error);
            await SetResult(error, context);
            return;
          }
        }
        catch (Exception e)
        {
          log.LogWarning(
            $"Unable to access the 'customerProxy.GetCustomersForMe' endpoint: {store.GetValueString("CUSTOMERSERVICE_API_URL")}. Message: {e.Message}.");
          await SetResult("Failed authentication", context);
          return;
        }

        log.LogInformation("Authorization: for Customer: {0} userUid: {1} userEmail: {2} allowed", customerUid, userUid,
          userEmail);
      }

      await _next.Invoke(context);
    }

    private async Task SetResult(string message, HttpContext context)
    {
      context.Response.StatusCode = 403;
      await context.Response.WriteAsync(message);
    }
  }

  /// <summary>
  /// 
  /// </summary>
  public static class TIDAuthenticationExtensions
  {
    /// <summary>
    /// Uses the tid authentication.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns></returns>
    public static IApplicationBuilder UseTIDAuthentication(this IApplicationBuilder builder)
    {
      return builder.UseMiddleware<TIDAuthentication>();
    }
  }
}
