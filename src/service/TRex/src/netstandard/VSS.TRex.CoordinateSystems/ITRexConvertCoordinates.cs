﻿using System.Threading.Tasks;
using CoreX.Models;

namespace VSS.TRex.CoordinateSystems
{
  public interface ITRexConvertCoordinates
  {
    /// <summary>
    /// Takes the content of a DC file as a byte array and uses the Trimble Coordinates Service to convert
    /// it into a coordinate system definition response object.
    /// </summary>
    Task<CoordinateSystemResponse> DCFileContentToCSD(string filePath, byte[] fileContent);

    /// <summary>
    /// Takes the CSIB string and uses the Trimble Coordinates Service to convert
    /// it into a coordinate system definition response object.
    /// </summary>
    Task<CoordinateSystemResponse> CSIBContentToCSD(string csib);
  }
}