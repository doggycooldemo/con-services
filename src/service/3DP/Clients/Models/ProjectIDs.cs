﻿using System;
using System.Net;
using Newtonsoft.Json;
using VSS.Common.Exceptions;
using VSS.MasterData.Models.ResultHandling.Abstractions;

namespace VSS.Productivity3D.Productivity3D.Models
{
  /// <summary>
  /// Raptor data model/project identifier.
  /// </summary>
  public class ProjectIDs
  {
    /// <summary>
    /// The project to use for both TRex and Raptor
    /// </summary>
    [JsonProperty(PropertyName = "projectId", Required = Required.Always)]
    public long ProjectId { get; set; }

    /// <summary>
    /// A project unique identifier.
    /// </summary>
    [JsonProperty(PropertyName = "projectUid", Required = Required.Always)]
    public Guid ProjectUid { get; set; }

    /// <summary>
    /// Default public constructor.
    /// </summary>
    public ProjectIDs()
    { }

    /// <summary>
    /// Overload constructor with parameters.
    /// </summary>
    public ProjectIDs(long projectId, Guid projectUid)
    {
      ProjectId = projectId;
      ProjectUid = projectUid;
    }

    /// <summary>
    /// Validation method.
    /// </summary>
    public void Validate()
    {
      if (!Guid.TryParseExact(ProjectUid.ToString(), "D", out Guid _) || ProjectUid == Guid.Empty)
      {
        throw new ServiceException(HttpStatusCode.BadRequest, 
          new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError, 
            "ProjectUid must be provided"));
      }

      // we generate IDs from GUIDS now, 0 and -1 are invalid
      if (ProjectId == 0 || ProjectId == -1)
      {
        throw new ServiceException(HttpStatusCode.BadRequest,
          new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
            "ProjectId must be provided"));
      }
    }
  }
}
