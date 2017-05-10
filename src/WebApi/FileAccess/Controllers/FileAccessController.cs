﻿using Microsoft.AspNetCore.Mvc;
using VSS.Raptor.Service.Common.Interfaces;
using Microsoft.Extensions.Logging;
using VSS.Raptor.Service.WebApiModels.FileAccess.ResultHandling;
using VSS.Raptor.Service.WebApiModels.FileAccess.Executors;
using VSS.Raptor.Service.Common.Models;
using TCCFileAccess;
using System.IO;
using VSS.Raptor.Service.Common.ResultHandling;
using Newtonsoft.Json;
using System.Net;
using MasterDataProxies.ResultHandling;

namespace VSS.Raptor.Service.WebApi.FileAccess.Controllers
{
    /// <summary>
    /// Controller for file access resources.
    /// </summary>
    public class FileAccessController : Controller
    {

        /// <summary>
        /// Logger for logging
        /// </summary>
        private readonly ILogger log;

        /// <summary>
        /// Logger factory for use by executor
        /// </summary>
        private readonly ILoggerFactory logger;

        /// <summary>
        /// Used to get a service provider
        /// </summary>
        private readonly IFileRepository fileAccess;

        /// <summary>
        /// Constructor with injected raptor client, logger and authenticated projects
        /// </summary>
        /// <param name="raptorClient">Raptor client</param>
        /// <param name="logger">Logger</param>
        /// <param name="authProjectsStore">Authenticated projects store</param>
        /// <param name="fileAccess">TCC file repository</param>
        public FileAccessController(ILoggerFactory logger,
            IFileRepository fileAccess)
        {

            this.logger = logger;
            this.log = logger.CreateLogger<FileAccessController>();
            this.fileAccess = fileAccess;
        }

        /// <summary>
        /// Gets requested file for Raptor from TCC and returns it as an image/png. 
        /// </summary>
        /// <param name="request">Details of the requested file</param>
        /// <returns>File contents as an image/png.
        /// </returns>
        /// <executor>RawFileAccessExecutor</executor>
        [Route("api/v1/rawfiles")]
        public FileResult PostRaw([FromBody] FileDescriptor request)
        {
            log.LogInformation("Get file from TCC as an image/png: " + JsonConvert.SerializeObject(request));
            try
            {
                request.Validate();
                RawFileAccessResult result = RequestExecutorContainer
                    .Build<RawFileAccessExecutor>(logger, null, fileAccess).Process(request) as RawFileAccessResult;
                if (result != null)
                {
                    return new FileStreamResult(new MemoryStream(result.fileContents), "image/png");
                }

                throw new ServiceException(HttpStatusCode.NoContent,
                    new ContractExecutionResult(ContractExecutionStatesEnum.FailedToGetResults,
                        "Raptor failed to return a file"));

            }
            finally
            {
                log.LogInformation("Get file from TCC as an image/png: " + Response.StatusCode);
            }
        }
    }
}
