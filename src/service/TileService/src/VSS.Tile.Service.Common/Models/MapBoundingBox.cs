﻿using VSS.MasterData.Models.Models;

namespace VSS.Tile.Service.Common.Models
{
  /// <summary>
  /// Model for map tile boundaing box. Lat/Lng are in radians.
  /// </summary>
  public class MapBoundingBox
  {
    public double minLat;
    public double minLng;
    public double maxLat;
    public double maxLng;

    public double centerLatDegrees => (minLat + (maxLat - minLat) / 2).LatRadiansToDegrees();
    public double centerLngDegrees => (minLng + (maxLng - minLng) / 2).LonRadiansToDegrees();

    public double minLatDegrees => minLat.LatRadiansToDegrees();
    public double minLngDegrees => minLng.LonRadiansToDegrees();
    public double maxLatDegrees => maxLat.LatRadiansToDegrees();
    public double maxLngDegrees => maxLng.LonRadiansToDegrees();

    public override string ToString()
    {
      return $"minLat:{minLat}/minLong:{minLng}/maxLat:{maxLat}/maxLong:{maxLng}";
    }
  }
}
