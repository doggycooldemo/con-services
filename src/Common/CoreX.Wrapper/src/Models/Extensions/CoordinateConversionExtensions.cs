﻿using CoreX.Types;
using CoreXModels;

namespace CoreX.Extensions
{
  public static class CoordinateConversionExtensions
  {
    /// <summary>
    /// Converts the multi dimensional array of doubles from the Coordinate service to an array of <see cref="NEE"/> objects.
    /// </summary>
    public static NEE[] ToNEEArray(this double[,] arrayData)
    {
      var result = new NEE[arrayData.Length / 3];

      for (var i = 0; i < arrayData.Length / 3; i++)
      {
        result[i] = new NEE
        {
          North = arrayData[i, 1],
          East = arrayData[i, 0],
          Elevation = arrayData[i, 2]
        };
      }

      return result;
    }

    /// <summary>
    /// Converts the multi dimensional array of doubles from the Coordinate service to an array of <see cref="LLH"/> objects.
    /// </summary>
    /// <remarks>
    /// Note the deliberate order change, with Longitude preceeding Latitude in the sequence we read
    /// from the LLH data.
    /// </remarks>
    public static LLH[] ToLLHArray(this double[,] arrayData)
    {
      var result = new LLH[arrayData.Length / 3];

      for (var i = 0; i < arrayData.Length / 3; i++)
      {
        result[i] = new LLH
        {
          Longitude = arrayData[i, 0],
          Latitude = arrayData[i, 1],
          Height = arrayData[i, 2]
        };
      }

      return result;
    }

    /// <summary>
    /// Converts an XYZ object to a LLH coordinate object.
    /// </summary>
    public static LLH ToLLH(this XYZ data) => new LLH
    {
      Latitude = data.Y,
      Longitude = data.X,
      Height = data.Z
    };

    /// <summary>
    /// Converts an array of XYZ coordinate data to NEE formatted objects.
    /// </summary>
    public static LLH[] ToLLH(this XYZ[] data)
    {
      var result = new LLH[data.Length];

      for (var i = 0; i < data.Length; i++)
      {
        result[i] = new LLH
        {
          Latitude = data[i].Y,
          Longitude = data[i].X,
          Height = data[i].Z
        };
      }

      return result;
    }

    /// <summary>
    /// Converts an XYZ object to an NEE coordinate object.
    /// </summary>
    public static NEE ToNEE(this XYZ data) => new NEE
    {
      North = data.Y,
      East = data.X,
      Elevation = data.Z
    };

    /// <summary>
    /// Converts an array of XYZ coordinate data to NEE formatted objects.
    /// </summary>
    public static NEE[] ToNEE(this XYZ[] data)
    {
      var result = new NEE[data.Length];

      for (var i = 0; i < data.Length; i++)
      {
        result[i] = new NEE
        {
          North = data[i].Y,
          East = data[i].X,
          Elevation = data[i].Z
        };
      }

      return result;
    }

    /// <summary>
    /// Converts an array of WSG84 point coordinate data to LLH formatted objects.
    /// </summary>
    public static LLH[] ToLLH(this WGS84Point[] data, InputAs inputAs)
    {
      var result = new LLH[data.Length];

      var inDegrees = inputAs == InputAs.Degrees;

      for (var i = 0; i < data.Length; i++)
      {
        result[i] = new LLH
        {
          Latitude = inDegrees ? data[i].Lat.DegreesToRadians() : data[i].Lat,
          Longitude = inDegrees ? data[i].Lon.DegreesToRadians() : data[i].Lon,
          Height = data[i].Height
        };
      }

      return result;
    }
  }
}
