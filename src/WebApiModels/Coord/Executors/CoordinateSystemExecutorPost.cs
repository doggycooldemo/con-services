﻿
using System.IO;
using VSS.Raptor.Service.WebApiModels.Coord.Models;
using VSS.Raptor.Service.Common.Interfaces;
using ASNodeDecls;
using Microsoft.Extensions.Logging;
using VLPDDecls;
using VSS.Raptor.Service.WebApiModels.Interfaces;

namespace VSS.Raptor.Service.WebApiModels.Coord.Executors
{
    /// <summary>
    /// Post coordinate system definition file executor.
    /// </summary>
    /// 
    public class CoordinateSystemExecutorPost : CoordinateSystemExecutor
    {
      /// <summary>
      /// This constructor allows us to mock raptorClient
      /// </summary>
      /// <param name="raptorClient"></param>
      /// 
      public CoordinateSystemExecutorPost(ILoggerFactory logger, IASNodeClient raptorClient)
          : base(logger, raptorClient)
      {
      }

      /// <summary>
      /// Default constructor for RequestExecutorContainer.Build
      /// </summary>
      public CoordinateSystemExecutorPost()
      {
      }

      protected override TASNodeErrorStatus SendRequestToPDSClient(object item)
      {

      TASNodeErrorStatus code = TASNodeErrorStatus.asneUnknown;
      TCoordinateSystemSettings tempCoordSystemSettings = new TCoordinateSystemSettings();

      if (item is IIsProjectIDApplicable)
      {
        if ((item as IIsProjectIDApplicable).HasProjectID())
        {
          CoordinateSystemFile request = item as CoordinateSystemFile;
          code = raptorClient.PassSelectedCoordinateSystemFile(new MemoryStream(request.csFileContent), request.csFileName, request.projectId ?? -1, out tempCoordSystemSettings);
        }
        else
        {
          CoordinateSystemFileValidationRequest request = item as CoordinateSystemFileValidationRequest;
          code = raptorClient.PassSelectedCoordinateSystemFile(new MemoryStream(request.csFileContent), request.csFileName, -1, out tempCoordSystemSettings);
        };
      };      

      if (code == TASNodeErrorStatus.asneOK)
          coordSystemSettings = tempCoordSystemSettings;

      return code;
    }
  }
}