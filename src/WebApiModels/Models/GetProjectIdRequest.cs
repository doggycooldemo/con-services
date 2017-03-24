﻿using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using WebApiModels.ResultHandling;

namespace WebApiModels.Models
{
  /// <summary>
  /// The request representation used to request the project Id that a specified asset is inside at a given location and date time.
  /// </summary>
  public class GetProjectIdRequest: ContractRequest
  {
    /// <summary>
    /// The id of the asset whose tagfile is to be processed. A value of -1 indicates 'none' so all assets are considered (depending on tccOrgId). 
    /// </summary>
    [Required]
    [JsonProperty(PropertyName = "assetId", Required = Required.Always)]
    public long assetId { get; set; }

    /// <summary>
    /// WGS84 latitude in decimal degrees. 
    /// </summary>
    [Required]
    [JsonProperty(PropertyName = "latitude", Required = Required.Always)]
    public double latitude { get; set; }

    /// <summary>
    /// WGS84 longitude in decimal degrees. 
    /// </summary>    
    [Required]
    [JsonProperty(PropertyName = "longitude", Required = Required.Always)]
    public double longitude { get; set; }

    /// <summary>
    /// Elevation in meters. 
    /// </summary>
    [Required]
    [JsonProperty(PropertyName = "height", Required = Required.Always)]
    public double height { get; set; }

    /// <summary>
    /// Date and time the asset was at the given location. 
    /// </summary>
    [Required]
    [JsonProperty(PropertyName = "timeOfPosition", Required = Required.Always)]
    public DateTime timeOfPosition { get; set; }

    /// <summary>
    /// Date and time the asset was at the given location. 
    /// </summary>
    [Required]
    [JsonProperty(PropertyName = "tccOrgUid", Required = Required.Always)]
    public string tccOrgUid { get; set; }


    /// <summary>
    /// Private constructor
    /// </summary>
    private GetProjectIdRequest()
    { }

    /// <summary>
    /// Create instance of GetProjectIdRequest
    /// </summary>
    public static GetProjectIdRequest CreateGetProjectIdRequest( long assetId, double latitude, double longitude, double height, DateTime timeOfPosition, string tccOrgUid)
    {
      return new GetProjectIdRequest
      {
        assetId = assetId,
        latitude = latitude,
        longitude = longitude,
        height = height,
        timeOfPosition = timeOfPosition,
        tccOrgUid = tccOrgUid
      };
    }

    
    /// <summary>
    /// Validates all properties
    /// </summary>
    public void Validate()
    {
      if (assetId <= 0 && string.IsNullOrEmpty(tccOrgUid))
      {
        throw new ServiceException(HttpStatusCode.BadRequest,
          new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
            String.Format("Must contain one or more of assetId {0} or tccOrgId {1}", assetId, tccOrgUid)));
      }

      if (latitude < -90 || latitude > 90)
      {
        throw new ServiceException(HttpStatusCode.BadRequest,
          new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
            String.Format("Latitude value of {0} should be between -90 degrees and 90 degrees", latitude)));
      }

      if (longitude < -180 || longitude > 180)
      {
        throw new ServiceException(HttpStatusCode.BadRequest,
          new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
            String.Format("Longitude value of {0} should be between -180 degrees and 180 degrees", longitude)));
      }

      if (!(timeOfPosition > DateTime.UtcNow.AddYears(-5) && timeOfPosition <= DateTime.UtcNow))
      {
        throw new ServiceException(HttpStatusCode.BadRequest,
          new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
            String.Format("timeOfPosition must have occured within last 5 years {0}", timeOfPosition)));
      }

    }
  }
}