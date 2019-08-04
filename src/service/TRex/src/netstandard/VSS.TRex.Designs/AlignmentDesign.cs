﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VSS.TRex.Common;
using VSS.TRex.Designs.Models;
using VSS.TRex.Geometry;
using VSS.TRex.SubGridTrees;
using VSS.TRex.SubGridTrees.Interfaces;
using SubGridUtilities = VSS.TRex.SubGridTrees.Core.Utilities.SubGridUtilities;

namespace VSS.TRex.Designs
{
  /// <summary>
  /// A design comprised of line work components which describe a road (or part of)
  /// </summary>

  public class AlignmentDesign : DesignBase
  {
    private static readonly ILogger Log = Logging.Logger.CreateLogger<AlignmentDesign>();

    public byte[] Data { get; set; } // assuming here there will be some kind of SDK model

    static AlignmentDesign()
    {
    }

    /// <summary>
    /// Constructor for a AlignmentDesign that takes the underlying cell size for the site model that will be used when interpolating heights from the design surface
    /// </summary>
    /// <param name="cellSize"></param>
    public AlignmentDesign(double cellSize)
    {
      // todo when SDK available
      Data = new byte[0];
    }


    /// <summary>
    /// Loads the Alignment from an Alignment file, along with the sub grid existence map file if it exists (created otherwise)
    /// </summary>
    /// <param name="localPathAndFileName"></param>
    /// <returns></returns>
    public override DesignLoadResult LoadFromFile(string localPathAndFileName, bool saveIndexFiles = true)
    {
      // todo when SDK available
      try
      {
        Data = File.ReadAllBytes(localPathAndFileName);
        Log.LogInformation($"Loaded alignment file {localPathAndFileName} containing byte count: {Data.Length}.");

        return DesignLoadResult.Success;
      }
      catch (Exception e)
      {
        Log.LogError(e, "Exception in LoadFromFile");
        return DesignLoadResult.UnknownFailure;
      }
    }

    /// <summary>
    /// Loads the Alignment file/s, from storage
    /// Includes design file and 2 index files (if they exist)
    /// </summary>
    /// <param name="siteModelUid"></param>
    /// <param name="fileName"></param>
    /// <param name="localPath"></param>
    /// <param name="loadIndices"></param>
    /// <returns></returns>
    public override async Task<DesignLoadResult> LoadFromStorage(Guid siteModelUid, string fileName, string localPath,
      bool loadIndices = false)
    {
      var isDownloaded = await S3FileTransfer.ReadFile(siteModelUid, fileName, localPath);
      if (!isDownloaded)
        return await Task.FromResult(DesignLoadResult.UnknownFailure);

      // todo when SDK available
      //if (loadIndices)
      //{
      //  isDownloaded = await S3FileTransfer.ReadFile(siteModelUid, (fileName + Consts.DESIGN_SUB_GRID_INDEX_FILE_EXTENSION), TRexServerConfig.PersistentCacheStoreLocation);
      //  if (!isDownloaded)
      //    return DesignLoadResult.UnableToLoadSubgridIndex;
      //
      //  isDownloaded = await S3FileTransfer.ReadFile(siteModelUid, (fileName + Consts.DESIGN_SPATIAL_INDEX_FILE_EXTENSION), TRexServerConfig.PersistentCacheStoreLocation);
      //  if (!isDownloaded)
      //    return DesignLoadResult.UnableToLoadSpatialIndex;
      //
      //  isDownloaded = await S3FileTransfer.ReadFile(siteModelUid, (fileName + Consts.DESIGN_BOUNDARY_FILE_EXTENSION), TRexServerConfig.PersistentCacheStoreLocation);
      //  if (!isDownloaded)
      //    return DesignLoadResult.UnableToLoadBoundary;
      //}

      return await Task.FromResult(DesignLoadResult.Success);
    }


    /// <summary>
    /// Retrieves the ground extents of the Alignment 
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    public override void GetExtents(out double x1, out double y1, out double x2, out double y2)
    {
      // todo when SDK available if appropriate
      x1 = 6;
      y1 = 34;
      x2 = 8;
      y2 = 38;
    }

    /// <summary>
    /// Retrieves the elevation range of the vertices in the alignment surface
    /// </summary>
    /// <param name="z1"></param>
    /// <param name="z2"></param>
    public override void GetHeightRange(out double z1, out double z2)
    {
      // todo when SDK available if appropriate
      z1 = Consts.NullDouble;
      z2 = Consts.NullDouble;
    }

    /// <summary>
    /// Interpolates a single spot height from the design, using the optimized spatial index
    /// </summary>
    /// <param name="Hint"></param>
    /// <param name="X"></param>
    /// <param name="Y"></param>
    /// <param name="Offset"></param>
    /// <param name="Z"></param>
    /// <returns></returns>
    public override bool InterpolateHeight(ref int Hint,
      double X, double Y,
      double Offset,
      out double Z)
    {
      // todo when SDK available if appropriate
      Z = Common.Consts.NullReal;
      return false;
    }

    /// <summary>
    /// Interpolates heights from the design for all the cells in a sub grid
    /// </summary>
    /// <param name="Patch"></param>
    /// <param name="OriginX"></param>
    /// <param name="OriginY"></param>
    /// <param name="CellSize"></param>
    /// <param name="Offset"></param>
    /// <returns></returns>
    public override bool InterpolateHeights(float[,] Patch, double OriginX, double OriginY, double CellSize, double Offset)
    {
      // todo when SDK available
      return false;
    }


    public override bool ComputeFilterPatch(double startStn, double endStn, double leftOffset, double rightOffset,
      SubGridTreeBitmapSubGridBits mask,
      SubGridTreeBitmapSubGridBits patch,
      double originX, double originY,
      double cellSize,
      double offset)
    {
      // todo when SDK available
      return false;
    }


    public override bool HasElevationDataForSubGridPatch(double X, double Y)
    {
      // todo when SDK available
      return false;
    }

    public override bool HasElevationDataForSubGridPatch(int SubGridX, int SubGridY) => false;

    public override bool HasFiltrationDataForSubGridPatch(double X, double Y) => false;

    public override bool HasFiltrationDataForSubGridPatch(int SubGridX, int SubGridY) => false;

    /// <summary>
    /// Computes the requested geometric profile over the design and returns the result
    /// as a vector of X, Y, Z, Station & TriangleIndex records
    /// </summary>
    /// <param name="profilePath"></param>
    /// <param name="cellSize"></param>
    /// <returns></returns>
    public override List<XYZS> ComputeProfile(XYZ[] profilePath, double cellSize)
    {
      // todo when SDK available
      return null;
    }

    /// <summary>
    /// Computes the requested boundary.
    /// </summary>
    /// <returns></returns>
    public override List<Fence> GetBoundary()
    {
      // todo when SDK available
      return null;
    }
  }
}
