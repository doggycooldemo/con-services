﻿

using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;


namespace VSS.Productivity3D.Models.Models
{
  /// <summary>
  /// A fence (or boundary) polygon with vertices expressed as WGS84 points
  /// </summary>
  public class WGS84Fence
  {
    /// <summary>
    /// Default private constructor.
    /// </summary>
    private WGS84Fence()
    { }

    /// <summary>
    /// Array of WGS84 points defining the polygon. The polygon is implicitly closed (first and last points are not
    /// required to be the same).
    /// </summary>
  // todo  [MoreThanTwoPoints]
    [JsonProperty(PropertyName = "points", Required = Required.Always)]
    [Required]
    public WGSPoint3D[] Points { get; private set; }

    /// <summary>
    /// Overload constructor with parameters.
    /// </summary>
    /// <param name="wgsPoints">The WGS points.</param>
    public WGS84Fence(WGSPoint3D[] wgsPoints)
    {
      Points = wgsPoints;
    }
  }
}
