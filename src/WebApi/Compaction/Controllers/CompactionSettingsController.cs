﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using VSS.Common.Exceptions;
using VSS.Common.ResultsHandling;
using VSS.MasterData.Proxies.Interfaces;
using VSS.Productivity3D.Common.Filters.Authentication;
using VSS.Productivity3D.Common.Models;

namespace VSS.Productivity3D.WebApi.Compaction.Controllers
{
  /// <summary>
  /// Controller for validating 3D project settings
  /// </summary>
  [ResponseCache(Duration = 180, VaryByQueryKeys = new[] { "*" })]
  public class CompactionSettingsController : Controller
  {
    /// <summary>
    /// Logger for logging
    /// </summary>
    private readonly ILogger log;

    /// <summary>
    /// Logger factory for use by executor
    /// </summary>
    private readonly ILoggerFactory logger;

    /// <summary>
    /// For getting project settings for a project
    /// </summary>
    private readonly IProjectSettingsProxy projectSettingsProxy;

    /// <summary>
    /// Constructor with dependency injection
    /// </summary>
    /// <param name="logger">Logger</param>
    /// <param name="projectSettingsProxy">Project settings proxy</param>
    public CompactionSettingsController(ILoggerFactory logger, IProjectSettingsProxy projectSettingsProxy)
    {
      this.logger = logger;
      this.log = logger.CreateLogger<CompactionSettingsController>();
      this.projectSettingsProxy = projectSettingsProxy;
    }

    /// <summary>
    /// Validates 3D project settings.
    /// </summary>
    /// <param name="projectUid">Project UID</param>
    /// <param name="projectSettings">Project settings to validate as a JSON object</param>
    /// <returns>ContractExecutionResult</returns>
    [ProjectUidVerifier]
    [Route("api/v2/compaction/validatesettings")]
    [HttpGet]
    public async Task<ContractExecutionResult> ValidateProjectSettings(
      [FromQuery] Guid projectUid,
      [FromQuery] string projectSettings)
    {
      log.LogInformation("ValidateProjectSettings: " + Request.QueryString);

      if (!string.IsNullOrEmpty(projectSettings))
      {
        var compactionSettings = GetProjectSettings(projectSettings);
        compactionSettings?.Validate();
        //It is assumed that the settings are about to be saved.
        //Clear the cache for these updated settings so we get the updated settings for compaction requests.
        log.LogDebug($"About to clear settings for project {projectUid}");
        projectSettingsProxy.ClearCacheItem<string>(projectUid.ToString());
      }

      log.LogInformation("ValidateProjectSettings returned: " + Response.StatusCode);
      return new ContractExecutionResult(ContractExecutionStatesEnum.ExecutedSuccessfully, "Project settings are valid");
    }

    /// <summary>
    /// Deserializes the project settings
    /// </summary>
    /// <param name="projectSettings">JSON representation of the project settings</param>
    /// <returns>The project settings instance</returns>
    private CompactionProjectSettings GetProjectSettings(string projectSettings)
    {
      CompactionProjectSettings ps = null;
      if (!string.IsNullOrEmpty(projectSettings))
      {
        try
        {
          ps = JsonConvert.DeserializeObject<CompactionProjectSettings>(projectSettings);
        }
        catch (Exception ex)
        {
          throw new ServiceException(HttpStatusCode.BadRequest,
            new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
              ex.Message));
        }
      }
      return ps;
    }
  }
}
