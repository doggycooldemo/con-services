﻿using System;
using Apache.Ignite.Core.Binary;
using VSS.Productivity3D.Models.Enums;
using VSS.TRex.Common;
using VSS.TRex.Common.Models;
using VSS.TRex.Designs.Models;
using VSS.TRex.Filters.Interfaces;
using VSS.TRex.Geometry;
using VSS.TRex.GridFabric.Arguments;
using VSS.TRex.GridFabric.ExtensionMethods;

namespace VSS.TRex.CellDatum.GridFabric.Arguments
{
  /// <summary>
  /// Argument containing the parameters required for a Cell Datum request
  /// </summary>    
  public class CellDatumRequestArgument_ApplicationService : BaseApplicationServiceRequestArgument
  {
    private const byte VERSION_NUMBER = 1;

    /// <summary>
    /// The datum type to return (eg: height, CMV, Temperature etc). 
    /// </summary>
    public DisplayMode Mode { get; set; } = DisplayMode.Height;

    /// <summary>
    /// A flag to indicate if a latitude/longitude or projected coordinate point has been provided
    /// </summary>
    public bool CoordsAreGrid { get; set; }

    /// <summary>
    /// The WGS84 latitude/longitude position or grid point in the project coordinate system to identify the cell from. 
    /// </summary>
    public XYZ Point { get; set; }

    public CellDatumRequestArgument_ApplicationService()
    { }

    public CellDatumRequestArgument_ApplicationService(
      Guid siteModelID,
      DisplayMode mode,
      bool coordsAreGrid,
      XYZ point,
      IFilterSet filters,
      DesignOffset referenceDesign,
      IOverrideParameters overrides)
    {
      ProjectID = siteModelID;
      Mode = mode;
      CoordsAreGrid = coordsAreGrid;
      Point = point;
      Filters = filters;
      ReferenceDesign = referenceDesign;
      Overrides = overrides;
    }

    /// <summary>
    /// Serialises content to the writer
    /// </summary>
    public override void InternalToBinary(IBinaryRawWriter writer)
    {
      base.InternalToBinary(writer);

      VersionSerializationHelper.EmitVersionByte(writer, VERSION_NUMBER);

      writer.WriteInt((int)Mode);
      writer.WriteBoolean(CoordsAreGrid);
      Point.ToBinary(writer);
    }

    /// <summary>
    /// Serialises content from the writer
    /// </summary>
    public override void InternalFromBinary(IBinaryRawReader reader)
    {
      base.InternalFromBinary(reader);

      var version = VersionSerializationHelper.CheckVersionByte(reader, VERSION_NUMBER);

      if (version == 1)
      {
        Mode = (DisplayMode) reader.ReadInt();
        CoordsAreGrid = reader.ReadBoolean();
        Point = Point.FromBinary(reader);
      }
    }
  }
}
