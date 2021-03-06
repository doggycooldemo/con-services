﻿using System;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using VSS.Common.Exceptions;
using VSS.MasterData.Models.ResultHandling.Abstractions;
using VSS.Productivity3D.Productivity3D.Models.Validation;

namespace VSS.Productivity3D.Productivity3D.Models
{
  /// <summary>
  /// Raptor data model/project identifier.
  /// </summary>
  public class ProjectID
  {
    /// <summary>
    /// The project to process the CS definition file into.
    /// </summary>
    [JsonProperty(PropertyName = "projectId", Required = Required.Default)]
    [ValidProjectId]
    public long? ProjectId { get; set; }

    /// <summary>
    /// A project unique identifier.
    /// </summary>
    [JsonProperty(PropertyName = "projectUid", Required = Required.Default)]
    [ValidProjectUID]
    public Guid? ProjectUid { get; set; }

    /// <summary>
    /// Default public constructor.
    /// </summary>
    public ProjectID()
    { }

    /// <summary>
    /// Overload constructor with parameters.
    /// </summary>
    /// <param name="projectId">The Raptor datamodel & legacy project identifier.</param>
    /// <param name="projectUid">The project UID.</param>
    public ProjectID(long? projectId, Guid? projectUid = null)
    {
      ProjectId = projectId;
      ProjectUid = projectUid;
    }

    /// <summary>
    /// Validation method.
    /// </summary>
    public virtual void Validate()
    {
      new DataAnnotationsValidator().TryValidate(this, out var results);

      if (results.Any())
      {
        throw new ServiceException(HttpStatusCode.BadRequest, new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError, results.FirstOrDefault()?.ErrorMessage));
      }
    }
  }
}
