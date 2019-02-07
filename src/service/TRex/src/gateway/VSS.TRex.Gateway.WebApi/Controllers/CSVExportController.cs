﻿using System;
using System.IO;
using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VSS.ConfigurationStore;
using VSS.MasterData.Models.Handlers;
using VSS.Productivity3D.Models.Models;
using VSS.Productivity3D.Models.ResultHandling;
using VSS.TRex.Exports.Surfaces.Requestors;
using VSS.TRex.Gateway.Common.Executors;
using VSS.TRex.Gateway.Common.Requests;
using VSS.TRex.Gateway.Common.ResultHandling;
using FileSystem = System.IO.File;


namespace VSS.TRex.Gateway.WebApi.Controllers
{

  /// <summary>
  /// The controller for generating TIN surfaces decimated from elevation data
  /// </summary>
  public class CSVExportController : BaseController
  {
    /// <summary>
    /// Constructor for TIN surface export controller
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <param name="exceptionHandler"></param>
    /// <param name="configStore"></param>
    /// <param name="tINSurfaceExportRequestor"></param>
    public CSVExportController(ILoggerFactory loggerFactory, IServiceExceptionHandler exceptionHandler,
      IConfigurationStore configStore, ITINSurfaceExportRequestor tINSurfaceExportRequestor)
      : base(loggerFactory, loggerFactory.CreateLogger<CSVExportController>(), exceptionHandler, configStore)
    {
    }

    /// <summary>
    /// Web service end point controller for veta export of csv file
    /// </summary>
    /// <param name="compactionExportRequest"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("api/v1/export/veta")]
    public CompactionExportResult GetVetaExport([FromBody] CompactionVetaExportRequest compactionVetaExportRequest)
    {
      Log.LogInformation($"{nameof(GetVetaExport)}: {Request.QueryString}");

      throw new NotImplementedException();

      //Log.LogDebug($"Accept header is {Request.Headers["Accept"]}");

      //compactionVetaExportRequest.Validate();

      //var result = WithServiceExceptionTryExecute(() =>
      //  RequestExecutorContainer
      //    .Build<TINSurfaceExportExecutor>(ConfigStore, LoggerFactory, ServiceExceptionHandler)
      //    .Process(compactionVetaExportRequest) as  /* todoJeannie */ TINSurfaceExportResult);

      //const string CSV_EXTENSION = ".csv";
      //const string ZIP_EXTENSION = ".zip";

      //// todoJeannie on s3?
      //var fullFileName = BuildFullFilePath(compactionVetaExportRequest.FileName, ZIP_EXTENSION);

      //if (FileSystem.Exists(fullFileName))
      //  FileSystem.Delete(fullFileName);

      //using (var zipFile = ZipFile.Open(fullFileName, ZipArchiveMode.Create))
      //{
      //  var entry = zipFile.CreateEntry(compactionVetaExportRequest.FileName + CSV_EXTENSION);
      //  using (var stream = entry.Open())
      //    new MemoryStream(result?.TINData).CopyTo(stream);
      //}

      //return new CompactionExportResult(fullFileName);
    }

    /// <summary>
    /// Web service end point controller for PassCount export of csv file
    /// </summary>
    /// <param name="compactionPassCountExportRequest"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("api/v1/export/passcount")]
    public CompactionExportResult GetPassCountExport([FromBody] CompactionPassCountExportRequest compactionPassCountExportRequest)
    {
      Log.LogInformation($"{nameof(GetPassCountExport)}: {Request.QueryString}");

      throw new NotImplementedException();

      //Log.LogDebug($"Accept header is {Request.Headers["Accept"]}");

      //compactionPassCountExportRequest.Validate();

      //var result = WithServiceExceptionTryExecute(() =>
      //  RequestExecutorContainer
      //    .Build<TINSurfaceExportExecutor>(ConfigStore, LoggerFactory, ServiceExceptionHandler)
      //    .Process(compactionPassCountExportRequest) as  /* todoJeannie */ TINSurfaceExportResult);

      //const string CSV_EXTENSION = ".csv";
      //const string ZIP_EXTENSION = ".zip";

      //// todoJeannie on s3?
      //var fullFileName = BuildFullFilePath(compactionPassCountExportRequest.FileName, ZIP_EXTENSION);

      //if (FileSystem.Exists(fullFileName))
      //  FileSystem.Delete(fullFileName);

      //using (var zipFile = ZipFile.Open(fullFileName, ZipArchiveMode.Create))
      //{
      //  var entry = zipFile.CreateEntry(compactionPassCountExportRequest.FileName + CSV_EXTENSION);
      //  using (var stream = entry.Open())
      //    new MemoryStream(result?.TINData).CopyTo(stream);
      //}

      //return new CompactionExportResult(fullFileName);
    }

    private string BuildFullFilePath(string filename, string extension)
    {
      return Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(filename) + extension);
    }


  }
}
