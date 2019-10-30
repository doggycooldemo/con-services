﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using VSS.Common.Exceptions;
using VSS.MasterData.Models.Models;

namespace VSS.Productivity3D.TagFileAuth.Models
{
  /// <summary>
  /// TFA v2 endpoint to retrieve ProjectUid and/or AssetUid for a tagfile
  ///      this is used by TRex Gateway and possibly etal
  /// </summary>
  public class GetProjectAndAssetUidsRequest 
  {
    /// <summary>
    /// if ProjectUid is supplied, this is a 'manual update'
    /// </summary>
    [JsonProperty(PropertyName = "projectUid", Required = Required.Default)]
    public string ProjectUid { get; set; }

    /// <summary>
    /// The device type of the machine. Valid values any, but normally 6=SNM940 (torch machines).
    /// </summary>
    [JsonProperty(PropertyName = "deviceType", Required = Required.Always)]
    public int DeviceType { get; set; }

    /// <summary>
    /// The SNM94n radio serial number of the machine from the tagfile.
    /// </summary>
    [JsonProperty(PropertyName = "radioSerial", Required = Required.Default)]
    public string RadioSerial { get; set; }

    /// <summary>
    /// The EC520 serial number of the machine from the tagfile.
    /// </summary>
    [JsonProperty(PropertyName = "ec520Serial", Required = Required.Default)]
    public string Ec520Serial { get; set; }

    /// <summary>
    /// Date and time the asset was at the given location. 
    /// </summary>
    [JsonProperty(PropertyName = "tccOrgUid", Required = Required.Default)]
    public string TccOrgUid { get; set; }


    /// <summary>
    /// WGS84 latitude in decimal degrees. 
    /// </summary>
    [JsonProperty(PropertyName = "latitude", Required = Required.Always)]
    public double Latitude { get; set; }

    /// <summary>
    /// WGS84 longitude in decimal degrees. 
    /// </summary>    
    [JsonProperty(PropertyName = "longitude", Required = Required.Always)]
    public double Longitude { get; set; }

    /// <summary>
    /// Date and time the asset was at the given location. 
    /// </summary>
    [JsonProperty(PropertyName = "timeOfPosition", Required = Required.Always)]
    public DateTime TimeOfPosition { get; set; }

    /// <summary>
    /// Private constructor
    /// </summary>
    private GetProjectAndAssetUidsRequest()
    { }

    /// <summary>
    /// Create instance of GetProjectAndAssetUidsRequest
    /// </summary>
    public GetProjectAndAssetUidsRequest
    ( string projectUid, int deviceType, string radioSerial, string ec520Serial, 
      string tccOrgUid,
      double latitude, double longitude, DateTime timeOfPosition)
    {
      ProjectUid = projectUid;
      DeviceType = deviceType;
      RadioSerial = radioSerial;
      Ec520Serial = ec520Serial;
      TccOrgUid = tccOrgUid;
      Latitude = latitude;
      Longitude = longitude;
      TimeOfPosition = timeOfPosition;
    }


    /// <summary>
    /// 
    /// workflow #1 TFHarvester Auto import
    ///       projectUid is NOT supplied
    ///    There must be a way to identify a customer
    ///     a) radioSerial and DeviceType or
    ///     b) ec520serial or
    ///     c) tccOrgId
    ///             Validate at least one exists
    ///             Validate correct subscription/s
    ///
    /// workflow #2 Manual import
    ///       projectUid is supplied
    ///          RadioSerial/ec520Serial must also be supplied - else error
    ///             Validate BOTH exist
    ///             Validate correct subscription/s  
    ///
    /// workflow #3 DirectSubmission from CTCT device
    ///      same as #1
    /// 
    /// </summary>
    public void Validate()
    {
      // if it has a projectUid, then it's a manual import and must have either assetUid or radio/dt
      if (!string.IsNullOrEmpty(ProjectUid) && !Guid.TryParseExact(ProjectUid, "D", out var projectUid))
        throw new ServiceException(System.Net.HttpStatusCode.BadRequest, GetProjectAndAssetUidsResult.FormatResult(uniqueCode: 36)); 

      var allowedDeviceTypes = new List<int>() { (int)DeviceTypeEnum.MANUALDEVICE, (int)DeviceTypeEnum.SNM940, (int)DeviceTypeEnum.SNM941, (int)DeviceTypeEnum.EC520 };
      var isDeviceTypeValid = allowedDeviceTypes.Contains(DeviceType);

      if (!isDeviceTypeValid)
        throw new ServiceException(System.Net.HttpStatusCode.BadRequest, GetProjectAndAssetUidsResult.FormatResult(uniqueCode: 30));

      if (string.IsNullOrEmpty(ProjectUid) && string.IsNullOrEmpty(RadioSerial) && string.IsNullOrEmpty(Ec520Serial) && string.IsNullOrEmpty(TccOrgUid))
        throw new ServiceException(System.Net.HttpStatusCode.BadRequest, GetProjectAndAssetUidsResult.FormatResult(uniqueCode: 37));
      
      if (Latitude < -90 || Latitude > 90)
        throw new ServiceException(System.Net.HttpStatusCode.BadRequest, GetProjectAndAssetUidsResult.FormatResult(uniqueCode: 21));
      
      if (Longitude < -180 || Longitude > 180)
        throw new ServiceException(System.Net.HttpStatusCode.BadRequest, GetProjectAndAssetUidsResult.FormatResult(uniqueCode: 22));
      
      if (!(TimeOfPosition > DateTime.UtcNow.AddYears(-50) && TimeOfPosition <= DateTime.UtcNow.AddDays(30)))
        throw new ServiceException(System.Net.HttpStatusCode.BadRequest, GetProjectAndAssetUidsResult.FormatResult(uniqueCode: 23));
    }
  }
}
