﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Localization;
using VSS.Common.Abstractions.Clients.CWS.Models;

namespace CCSS.Geometry
{
  public static class GeometryConversion
  {
    private static string POLYGON = "POLYGON";

    public static string GetPolygonWKT(string boundary)
    {
      return GetPoints(boundary)?.ToPolygonWKT();
    }

    /// <summary>
    /// Map a 3dp project wkt boundary to the format required for cws Project API
    /// </summary>
    public static ProjectBoundary MapProjectBoundary(string boundary)
    {
      var boundaryPoints = GetPoints(boundary);
      if (boundaryPoints == null)
        return null;

      var pointsAsDoubleList = new List<double[]>(boundaryPoints.Count);
      for (var i = 0; i < boundaryPoints.Count; i++)
        pointsAsDoubleList.Add(item: new[] { boundaryPoints[i].X, boundaryPoints[i].Y });

      var cwsProjectBoundary = new ProjectBoundary();
      cwsProjectBoundary.type = "Polygon";
      cwsProjectBoundary.coordinates = new List<List<double[]>> { pointsAsDoubleList };

      return cwsProjectBoundary;
    }

    private static List<Point> GetPoints(string boundary)
    {
      //Polygon must start and end with the same point
      return string.IsNullOrEmpty(boundary) ? null : boundary.ParseGeometryData().ClosePolygonIfRequired();
    }

    /// <summary>
    /// Maps a CWS project boundary to a project WKT boundary
    /// </summary>
    public static string ProjectBoundaryToWKT(ProjectBoundary boundary)
    {
      //Should always be a boundary but just in case
      if (boundary == null || boundary.coordinates.Count == 0)
        return null;

      // CWS boundary is always closed ?
      return boundary.coordinates.ToPolygonWKT();
    }
  }

  internal static class ExtensionString
  {
    private static readonly Dictionary<string, string> _replacements = new Dictionary<string, string>();

    static ExtensionString()
    {
      _replacements["LINESTRING"] = "";
      _replacements["CIRCLE"] = "";
      _replacements["POLYGON"] = "";
      _replacements["POINT"] = "";
      _replacements["("] = "";
      _replacements[")"] = "";
    }

    public static List<Point> ClosePolygonIfRequired(this List<Point> s)
    {
      if (Equals(s.First(), s.Last()))
        return s;
      s.Add(s.First());
      return s;
    }

    public static string ToPolygonWKT(this List<Point> s)
    {
      var internalString = s.Select(p => p.WKTSubstring).Aggregate((i, j) => $"{i},{j}");
      return $"POLYGON(({internalString}))";
    }

    public static string ToPolygonWKT(this List<List<double[]>> list)
    {
      // Always just a single 2D array in the list which is the CWS polygon coordinates
      var coords = list[0];
      var rowCount = coords.Count;
      var wktCoords = new List<string>();
      for (var i = 0; i < rowCount; i++)
      {
        wktCoords.Add($"{coords[i][0]} {coords[i][1]}");
      }

      var internalString = wktCoords.Aggregate((i, j) => $"{i},{j}");
      return $"POLYGON(({internalString}))";
    }

    public static List<Point> ParseGeometryData(this string s)
    {
      if (string.IsNullOrEmpty(s))
        return new List<Point>();

      foreach (string to_replace in _replacements.Keys)
      {
        s = s.Replace(to_replace, _replacements[to_replace]);
      }

      var pointsArray = s.Split(',').Select(str => str.Trim()).ToArray();

      //gets x and y coordinates split by space, trims whitespace at pos 0, converts to double array
      var coordinates = pointsArray.Select(point => point.Trim().Split(null)
        .Where(v => !string.IsNullOrWhiteSpace(v)).ToArray());
      return coordinates.Select(p => new Point() {X = double.Parse(p[0]), Y = double.Parse(p[1])}).ToList();
    }
  }

  internal struct Point
  {
    public double X;
    public double Y;
    public string WKTSubstring => $"{X} {Y}";

    public override bool Equals(object obj)
    {
      var source = (Point)obj;
      return (source.X == X) && (source.Y == Y);
    }

    public override int GetHashCode()
    {
      return 0;
    }
  }
}