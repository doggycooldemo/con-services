﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using VSS.Common.Abstractions.Configuration;
using VSS.DataOcean.Client;
using VSS.Productivity3D.Project.Abstractions.Models;
using VSS.Tile.Service.Common.Extensions;
using VSS.Tile.Service.Common.Helpers;
using VSS.Tile.Service.Common.Models;
using VSS.Visionlink.Interfaces.Events.MasterData.Models;
using VSS.WebApi.Common;
using MasterDataModels = VSS.MasterData.Models.Models;

namespace VSS.Tile.Service.Common.Services
{
  /// <summary>
  /// Provides DXF tile functionality for reports
  /// </summary>
  public class DxfTileService : IDxfTileService
  {
    private readonly IConfigurationStore config;
    private readonly IDataOceanClient dataOceanClient;
    private readonly ILogger log;
    private readonly ITPaaSApplicationAuthentication authn;
    private readonly string dataOceanRootFolder;

    public DxfTileService(IConfigurationStore configuration, IDataOceanClient dataOceanClient, ILoggerFactory logger, ITPaaSApplicationAuthentication authn)
    {
      config = configuration;
      this.dataOceanClient = dataOceanClient;
      log = logger.CreateLogger<DxfTileService>();
      this.authn = authn;

      const string DATA_OCEAN_ROOT_FOLDER_ID_KEY = "DATA_OCEAN_ROOT_FOLDER_ID";
      dataOceanRootFolder = configuration.GetValueString(DATA_OCEAN_ROOT_FOLDER_ID_KEY);
      if (string.IsNullOrEmpty(dataOceanRootFolder))
        throw new ArgumentException($"Missing environment variable {DATA_OCEAN_ROOT_FOLDER_ID_KEY}");
    }

    /// <summary>
    /// Gets a map tile with DXF linework on it. 
    /// </summary>
    /// <param name="parameters">Map parameters such as bounding box, tile size, zoom level etc.</param>
    /// <param name="dxfFiles">The list of DXF files to overlay tiles for</param>
    /// <returns>A bitmap</returns>
    public async Task<byte[]> GetDxfBitmap(MapParameters parameters, IEnumerable<FileData> dxfFiles)
    {
      log.LogInformation("GetDxfBitmap");

      byte[] overlayData = null;

      if (dxfFiles != null && dxfFiles.Any())
      {
        List<byte[]> tileList = new List<byte[]>();
        foreach (var dxfFile in dxfFiles)
        {
          if (dxfFile.ImportedFileType == ImportedFileType.Linework)
          {
            tileList.Add(await JoinDxfTiles(parameters, dxfFile));
          }
        }

        log.LogDebug("Overlaying DXF bitmaps");
        overlayData = TileServiceUtils.OverlayTiles(parameters, tileList);
      }

      return overlayData;
    }


    /// <summary>
    /// Joins standard size DXF tiles together to form one large tile for the report
    /// </summary>
    private async Task<byte[]> JoinDxfTiles(MapParameters parameters, FileData dxfFile)
    {
      log.LogDebug($"JoinDxfTiles: {dxfFile.ImportedFileUid}, {dxfFile.Name}");

      //Find the tiles that the bounding box fits into.
      MasterDataModels.Point tileTopLeft = MasterDataModels.WebMercatorProjection.PixelToTile(parameters.pixelTopLeft);
      MasterDataModels.Point pixelBottomRight = TileServiceUtils.LatLngToPixel(
        parameters.bbox.minLat, parameters.bbox.maxLng, parameters.numTiles);
      MasterDataModels.Point tileBottomRight = MasterDataModels.WebMercatorProjection.PixelToTile(pixelBottomRight);

      int xnumTiles = (int) (tileBottomRight.x - tileTopLeft.x) + 1;
      int ynumTiles = (int) (tileBottomRight.y - tileTopLeft.y) + 1;
      int width = xnumTiles * MasterDataModels.WebMercatorProjection.TILE_SIZE;
      int height = ynumTiles * MasterDataModels.WebMercatorProjection.TILE_SIZE;

      using (Image<Rgba32> tileBitmap = new Image<Rgba32>(width, height))
      {
        //Find the offset of the bounding box top left point inside the top left tile
        var point = new MasterDataModels.Point
        {
          x = tileTopLeft.x * MasterDataModels.WebMercatorProjection.TILE_SIZE,
          y = tileTopLeft.y * MasterDataModels.WebMercatorProjection.TILE_SIZE
        };
        //Clip to the actual bounding box within the tiles. 
        int clipWidth = parameters.mapWidth;
        int clipHeight = parameters.mapHeight;
        int xClipTopLeft = (int) (parameters.pixelTopLeft.x - point.x);
        int yClipTopLeft = (int) (parameters.pixelTopLeft.y - point.y);
        //Unlike System.Drawing, which allows the clipRect to have negative x, y and which moves as well as clips when used with DrawImage
        //as the source rectangle, ImageSharp does not respect negative values. So we will have to do extra work in this situation.
        if (xClipTopLeft < 0)
        {
          clipWidth += xClipTopLeft;
          xClipTopLeft = 0;
        }
        if (yClipTopLeft < 0)
        {
          clipHeight += yClipTopLeft;
          yClipTopLeft = 0;
        }
        var clipRect = new Rectangle(xClipTopLeft, yClipTopLeft, clipWidth, clipHeight);

        //Join all the DXF tiles into one large tile
        await JoinDataOceanTiles(dxfFile, tileTopLeft, tileBottomRight, tileBitmap, parameters.zoomLevel);

        //Now clip the large tile
        tileBitmap.Mutate(ctx => ctx.Crop(clipRect));
        if (clipWidth >= parameters.mapWidth && clipHeight >= parameters.mapHeight)
        {
          return tileBitmap.BitmapToByteArray();
        }

        //and resize it if required tile area was overlapping rather than within the large tile
        //(negative clip values above)
        using (Image<Rgba32> resizedBitmap = new Image<Rgba32>(parameters.mapWidth, parameters.mapHeight))
        {
          Point offset = new Point(parameters.mapWidth-clipWidth, parameters.mapHeight-clipHeight);
          resizedBitmap.Mutate(ctx => ctx.DrawImage(tileBitmap, PixelBlenderMode.Normal, 1f, offset));
          return resizedBitmap.BitmapToByteArray();
        }            
      }
    }
    private async Task JoinDataOceanTiles(FileData dxfFile, MasterDataModels.Point tileTopLeft, MasterDataModels.Point tileBottomRight, Image<Rgba32> tileBitmap, int zoomLevel)
    {
      var fileName = DataOceanFileUtil.DataOceanFileName(dxfFile.Name,
        dxfFile.ImportedFileType == ImportedFileType.SurveyedSurface || dxfFile.ImportedFileType == ImportedFileType.GeoTiff,
        Guid.Parse(dxfFile.ImportedFileUid), dxfFile.SurveyedUtc);
      fileName = DataOceanFileUtil.GeneratedFileName(fileName, dxfFile.ImportedFileType);
      var dataOceanFileUtil = new DataOceanFileUtil($"{DataOceanUtil.PathSeparator}{dataOceanRootFolder}{dxfFile.Path}{DataOceanUtil.PathSeparator}{fileName}");
      log.LogDebug($"{nameof(JoinDataOceanTiles)}: fileName: {fileName} dataOceanFileUtil.FullFileName {dataOceanFileUtil.FullFileName}");

      for (int yTile = (int)tileTopLeft.y; yTile <= (int)tileBottomRight.y; yTile++)
      {
        for (int xTile = (int)tileTopLeft.x; xTile <= (int)tileBottomRight.x; xTile++)
        {
          var targetFile = dataOceanFileUtil.GetTileFileName(zoomLevel, yTile, xTile);
          log.LogDebug($"JoinDxfTiles: getting tile {targetFile}");
          var file = await dataOceanClient.GetFile(targetFile, authn.CustomHeaders());
          if (file != null)
          {
            Image<Rgba32> tile = Image.Load<Rgba32>(file);

            Point offset = new Point(
              (xTile - (int)tileTopLeft.x) * MasterDataModels.WebMercatorProjection.TILE_SIZE,
              (yTile - (int)tileTopLeft.y) * MasterDataModels.WebMercatorProjection.TILE_SIZE);
            tileBitmap.Mutate(ctx => ctx.DrawImage(tile, PixelBlenderMode.Normal, 1f, offset));
          }
        }
      }
    }
  }

  public interface IDxfTileService
  {
    Task<byte[]> GetDxfBitmap(MapParameters parameters, IEnumerable<FileData> dxfFiles);
  }
}
