﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Principal;
using Newtonsoft.Json;
using VSS.Common.Exceptions;
using VSS.MasterData.Models.Handlers;
using VSS.MasterData.Models.Internal;
using VSS.MasterData.Models.Models;
using VSS.Productivity3D.Filter.Common.Filters.Authentication;

namespace VSS.Productivity3D.Filter.Common.Models
{
  public class FilterRequestFull : FilterRequest
  {
    
    public ProjectData ProjectData { get; set; }

    public string CustomerUid { get; set; }

    public bool IsApplicationContext { get; set; }

    public string UserId { get; set; }

    public string ProjectUid { get; set; }

    public IDictionary<string, string> CustomHeaders { get; set; }

    //public static FilterRequestFull Create(IDictionary<string, string> customHeaders, string customerUid, bool isApplicationContext, string userId, string projectUid, FilterRequest request = null)
    //{
    //  return new FilterRequestFull
    //  {
    //    FilterUid = request?.FilterUid ?? string.Empty,
    //    Name = request?.Name ?? string.Empty,
    //    FilterJson = request?.FilterJson ?? string.Empty,
    //    CustomerUid = customerUid,
    //    IsApplicationContext = isApplicationContext,
    //    UserId = userId,
    //    ProjectUid = projectUid,
    //    CustomHeaders = customHeaders
    //  };
    //}

    public static FilterRequestFull Create(IDictionary<string, string> customHeaders, string customerUid, bool isApplicationContext, string userId, ProjectData projectData, FilterRequest request = null)
    {
      return new FilterRequestFull
      {
        FilterUid = request?.FilterUid ?? string.Empty,
        Name = request?.Name ?? string.Empty,
        FilterJson = request?.FilterJson ?? string.Empty,
        CustomerUid = customerUid,
        IsApplicationContext = isApplicationContext,
        UserId = userId,
        ProjectData = projectData,
        ProjectUid = projectData?.ProjectUid,
        CustomHeaders = customHeaders
      };
    }

    //public static FilterRequestFull Create(IDictionary<string, string> customHeaders, TIDCustomPrincipal customPrincipal, ProjectData projectData, FilterRequest request = null)
    //{
    //  return new FilterRequestFull
    //  {
    //    FilterUid = request?.FilterUid ?? string.Empty,
    //    Name = request?.Name ?? string.Empty,
    //    FilterJson = request?.FilterJson ?? string.Empty,
    //    CustomerUid = customPrincipal.CustomerUid,
    //    IsApplicationContext = customPrincipal.IsApplication,
    //    UserId = (customPrincipal?.Identity as GenericIdentity)?.Name,
    //    ProjectUid = projectData.ProjectUid,
    //    CustomHeaders = customHeaders
    //  };
    //}

    public override void Validate(IServiceExceptionHandler serviceExceptionHandler)
    {
      if (string.IsNullOrEmpty(CustomerUid) || Guid.TryParse(CustomerUid, out Guid _) == false)
      {
        serviceExceptionHandler.ThrowServiceException(HttpStatusCode.BadRequest, 27);
      }

      if (string.IsNullOrEmpty(UserId) || (IsApplicationContext == false && Guid.TryParse(UserId, out Guid _) == false))
      {
        serviceExceptionHandler.ThrowServiceException(HttpStatusCode.BadRequest, 28);
      }

      if (ProjectData == null || string.IsNullOrEmpty(ProjectUid) || Guid.TryParse(ProjectUid, out Guid _) == false)
      {
        serviceExceptionHandler.ThrowServiceException(HttpStatusCode.BadRequest, 1);
      }

      base.Validate(serviceExceptionHandler);
    }
  }
}