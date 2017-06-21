﻿
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TCCFileAccess;
using VSS.Raptor.Service.Common.Contracts;
using VSS.Raptor.Service.Common.Interfaces;
using VSS.Raptor.Service.Common.Models;
using VSS.Raptor.Service.Common.ResultHandling;
using VSS.Raptor.Service.WebApiModels.Notification.Models;
using VSS.VisionLink.Interfaces.Events.MasterData.Models;
using WebApiModels.Interfaces;
using WebApiModels.Notification.Helpers;

namespace VSS.Raptor.Service.WebApiModels.Notification.Executors
{
  /// <summary>
  /// Processes the request to delete a file.
  /// Action taken depends on the file type.
  /// </summary>
  public class DeleteFileExecutor : RequestExecutorContainer
  {
    /// <summary>
    /// This constructor allows us to mock raptorClient
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="raptorClient"></param>
    /// <param name="fileRepository"></param>
    /// <param name="tileGenerator"></param>
    public DeleteFileExecutor(ILoggerFactory logger, IASNodeClient raptorClient, IFileRepository fileRepository, ITileGenerator tileGenerator) : 
      base(logger, raptorClient, null, null, fileRepository, tileGenerator)
    {
    }

    /// <summary>
    /// Default constructor for RequestExecutorContainer.Build
    /// </summary>
    public DeleteFileExecutor()
    {
    }

    /// <summary>
    /// Populates ContractExecutionStates with Production Data Server error messages.
    /// </summary>
    /// 
    protected override void ProcessErrorCodes()
    {
      RaptorResult.AddErrorMessages(ContractExecutionStates);
    }

    protected override ContractExecutionResult ProcessEx<T>(T item)
    {
      throw new NotImplementedException("Use the asynchronous form of this method");
    }

    protected override async Task<ContractExecutionResult> ProcessAsyncEx<T>(T item)
    {
      try
      {
        ProjectFileDescriptor request = item as ProjectFileDescriptor;
        ImportedFileType fileType = FileUtils.GetFileType(request.File.fileName);

        if (fileType == ImportedFileType.DesignSurface ||
            fileType == ImportedFileType.Alignment ||
            fileType == ImportedFileType.Linework)
        {
          var suffix = FileUtils.GeneratedFileSuffix(fileType);
          //Delete generated files
          bool success = await DeleteGeneratedFile(request.projectId.Value, request.File, suffix, FileUtils.PROJECTION_FILE_EXTENSION) &&
                         await DeleteGeneratedFile(request.projectId.Value, request.File, suffix, FileUtils.HORIZONTAL_ADJUSTMENT_FILE_EXTENSION);
          if (fileType != ImportedFileType.Linework)
          {
            success = success && await DeleteGeneratedFile(request.projectId.Value, request.File, suffix, FileUtils.DXF_FILE_EXTENSION);
          }
          if (!success)
          {
            throw new ServiceException(HttpStatusCode.BadRequest,
              new ContractExecutionResult(ContractExecutionStatesEnum.InternalProcessingError,
                "Failed to delete generated files"));
          }
          //Delete tiles 
          string generatedName = FileUtils.GeneratedFileName(request.File.fileName, suffix, FileUtils.DXF_FILE_EXTENSION);
          await tileGenerator.DeleteDxfTiles(request.projectId.Value, generatedName, request.File).ConfigureAwait(false);
        }


        //If surveyed surface, delete it in Raptor
        if (fileType == ImportedFileType.SurveyedSurface)
        {
          log.LogDebug("Discarding ground surface file in Raptor");
          if (!raptorClient.DiscardGroundSurfaceFileDetails(request.projectId.Value, request.FileId))
          {
            throw new ServiceException(HttpStatusCode.BadRequest,
              new ContractExecutionResult(ContractExecutionStatesEnum.InternalProcessingError,
                "Failed to discard ground surface file"));
          }
        }

        return new ContractExecutionResult(ContractExecutionStatesEnum.ExecutedSuccessfully, "Delete file notification successful");        
      }
      finally
      {
        ContractExecutionStates.ClearDynamic();
      }

    }

    /// <summary>
    /// Delete a generated file associated with the specified file
    /// </summary>
    /// <param name="projectId">The id of the project to which the file belongs</param>
    /// <param name="fileDescr">The original file</param>
    /// <param name="suffix">The suffix applied to the file name to get the generated file name</param>
    /// <param name="extension">The file extension of the generated file</param>
    /// <returns>True if the file is successfully deleted, false otherwise</returns>
    private async Task<bool> DeleteGeneratedFile(long projectId, FileDescriptor fileDescr, string suffix, string extension)
    {
      string generatedName = FileUtils.GeneratedFileName(fileDescr.fileName, suffix, extension);
      log.LogDebug("Deleting generated file {0}", generatedName);
      string fullName = Path.Combine(fileDescr.path, generatedName);
      if (await fileRepo.FileExists(fileDescr.filespaceId, fullName))
      {
        if (!await fileRepo.DeleteFile(fileDescr.filespaceId, fullName))
        {
          log.LogWarning("Failed to delete file {0} for project {1}", generatedName, projectId);
          throw new ServiceException(HttpStatusCode.BadRequest,
            new ContractExecutionResult(ContractExecutionStatesEnum.InternalProcessingError,
              "Failed to delete associated file " + generatedName));
        }
        return true;
      }
      return true;//TODO: Is this what we want if file not there?
    }

 
  }
}
