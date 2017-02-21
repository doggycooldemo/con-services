﻿using System.ComponentModel.DataAnnotations;
using System.Net;
using Newtonsoft.Json;
using VSS.Raptor.Service.Common.Contracts;
using VSS.Raptor.Service.Common.Filters.Validation;
using VSS.Raptor.Service.Common.Interfaces;
using VSS.Raptor.Service.Common.ResultHandling;

namespace VSS.Raptor.Service.Common.Models
{
  /// <summary>
  /// Description to identify a file by its location in TCC.
  /// </summary>
  public class FileDescriptor : IValidatable
  {
    /// <summary>
    /// The id of the filespace in TCC where the file is located.
    /// </summary>
    [JsonProperty(PropertyName = "filespaceId", Required = Required.Always)]
    public string filespaceId { get; private set; }

    /// <summary>
    /// The full path of the file.
    /// </summary>
    [MaxLength(MAX_PATH)]
    [JsonProperty(PropertyName = "path", Required = Required.Always)]
    public string path { get; private set; }

    /// <summary>
    /// The name of the file.
    /// </summary>
    [ValidFilename(MAX_FILE_NAME)] 
    [MaxLength(MAX_FILE_NAME)]
    [JsonProperty(PropertyName = "fileName", Required = Required.Always)]
    public string fileName { get; private set; }

   /// <summary>
    /// Private constructor
    /// </summary>
    private FileDescriptor()
    {}

    /// <summary>
    /// Create instance of FileDescriptor
    /// </summary>
    public static FileDescriptor CreateFileDescriptor
        (
          string filespaceId,
          string path,
          string fileName
        )
    {
      return new FileDescriptor
             {
               filespaceId = filespaceId,
               path = path,
               fileName = fileName
             };
    }

    public static FileDescriptor EmptyFileDescriptor
    {
      get { return emptyDescriptor; }
    }

    /// <summary>
    /// Create example instance of FileDescriptor to display in Help documentation.
    /// </summary>
    public static FileDescriptor HelpSample
    {
      get
      {
        return new FileDescriptor()
        {
          filespaceId = "u72003136-d859-4be8-86de-c559c841bf10",
          path = "BC Data/Sites/Integration10/Designs",
          fileName = "Cycleway.ttm"
        };
      }
    }

    /// <summary>
    /// Validates all properties
    /// </summary>
    public void Validate()
    {
      if (string.IsNullOrEmpty(this.filespaceId) || string.IsNullOrEmpty(this.path) ||
          string.IsNullOrEmpty(this.fileName))
      {
        throw new ServiceException(HttpStatusCode.BadRequest,
              new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
                  "Filespace Id, filespace name, path and file name are all required"));           
      }
        
    }

    private const int MAX_FILE_NAME = 1024;
    private const int MAX_PATH = 2048;

    private static FileDescriptor emptyDescriptor = new FileDescriptor
                                                    {
                                                        filespaceId = string.Empty,
                                                        path = string.Empty,
                                                        fileName = string.Empty
                                                    };



  }
}