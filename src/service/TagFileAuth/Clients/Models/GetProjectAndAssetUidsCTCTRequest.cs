﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using VSS.MasterData.Models.Models;

namespace VSS.Productivity3D.TagFileAuth.Models
{
  /// <summary>
  /// TFA v2 endpoint to retrieve ProjectUid and/or AssetUid for a tagfile
  ///      this is used by TRex Gateway and possibly etal
  /// </summary>
  public class GetProjectAndAssetUidsCTCTRequest 
  {
    /// <summary>
    /// The EC520 serial number of the machine from the tagfile.
    /// </summary>
    [JsonProperty(PropertyName = "ec520Serial", Required = Required.Default)]
    public string Ec520Serial { get; set; }

    /// <summary>
    /// The SNM94n radio serial number of the machine from the tagfile.
    /// </summary>
    [JsonProperty(PropertyName = "radioSerial", Required = Required.Default)]
    public string RadioSerial { get; set; }

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
    private GetProjectAndAssetUidsCTCTRequest()
    { }

    /// <summary>
    /// Create instance of GetProjectAndAssetUidsCTCTRequest
    /// </summary>
    public GetProjectAndAssetUidsCTCTRequest
    (string ec520Serial, string radioSerial, string tccOrgUid,
      double latitude, double longitude, DateTime timeOfPosition)
    {
      Ec520Serial = ec520Serial;
      RadioSerial = radioSerial;
      TccOrgUid = tccOrgUid;
      Latitude = latitude;
      Longitude = longitude;
      TimeOfPosition = timeOfPosition;
    }

    public int Validate(bool validateForCTCT = false)
    {
      if (validateForCTCT && string.IsNullOrEmpty(Ec520Serial))
        return 51;

      if (string.IsNullOrEmpty(RadioSerial) && string.IsNullOrEmpty(Ec520Serial) && string.IsNullOrEmpty(TccOrgUid))
        return 37;

      if (Latitude < -90 || Latitude > 90)
        return 21;

      if (Longitude < -180 || Longitude > 180)
        return 22;

      if (!(TimeOfPosition > DateTime.UtcNow.AddYears(-50) && TimeOfPosition <= DateTime.UtcNow.AddDays(30)))
        return 23;

      return 0;
    }
  }
}
