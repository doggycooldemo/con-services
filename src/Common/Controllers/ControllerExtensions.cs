﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VSS.MasterDataProxies.Interfaces;
using VSS.Productivity3D.Common.Contracts;
using VSS.Productivity3D.Common.Models;
using VSS.Productivity3D.Common.ResultHandling;
using VSS.VisionLink.Interfaces.Events.MasterData.Models;

namespace VSS.Productivity3D.Common.Controllers
{
  /// <summary>
  /// Extensions for the Compaction controller.
  /// </summary>
  public static class ControllerExtensions
  {
    /// <summary>
    /// Gets the ids of the surveyed surfaces to exclude from Raptor calculations. 
    /// This is the deactivated ones.
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="fileListProxy">Proxy client to get list of imported files for the project</param>
    /// <param name="projectUid">The UID of the project containing the surveyed surfaces</param>
    /// <param name="customHeaders">Http request custom headers</param>
    /// <returns>The list of file ids for the surveyed surfaces to be excluded</returns>
    public static async Task<List<long>> GetExcludedSurveyedSurfaceIds(this Controller controller, IFileListProxy fileListProxy, Guid projectUid, IDictionary<string, string> customHeaders)
    {
      var fileList = await fileListProxy.GetFiles(projectUid.ToString(), customHeaders);
      if (fileList == null || fileList.Count == 0)
        return null;
      return fileList
        .Where(f => f.ImportedFileType == ImportedFileType.SurveyedSurface && !f.IsActivated)
        .Select(f => f.LegacyFileId).ToList();
    }

    public static async Task<long?> GetLegacyFileId(this Controller controller, IFileListProxy fileListProxy, Guid projectUid, Guid fileUid, IDictionary<string, string> customHeaders)
    {
      var fileList = await fileListProxy.GetFiles(projectUid.ToString(), customHeaders);
      if (fileList == null || fileList.Count == 0)
        return null;

      return fileList.Where(f => f.ImportedFileUid == fileUid.ToString() && f.IsActivated).Select(f => f.LegacyFileId).FirstOrDefault();
    }
    

    /// <summary>
    /// Gets the list of contributing machines from the query parameters
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="assetId">The asset ID</param>
    /// <param name="machineName">The machine name</param>
    /// <param name="isJohnDoe">The john doe flag</param>
    /// <returns>List of machines</returns>
    public static List<MachineDetails> GetMachines(this Controller controller, long? assetId, string machineName, bool? isJohnDoe)
    {
      MachineDetails machine = null;
      if (assetId.HasValue || !string.IsNullOrEmpty(machineName) || isJohnDoe.HasValue)
      {
        if (!assetId.HasValue || string.IsNullOrEmpty(machineName) || !isJohnDoe.HasValue)
        {
          throw new ServiceException(HttpStatusCode.BadRequest,
            new ContractExecutionResult(ContractExecutionStatesEnum.ValidationError,
              "If using a machine, asset ID machine name and john doe flag must be provided"));
        }
        machine = MachineDetails.CreateMachineDetails(assetId.Value, machineName, isJohnDoe.Value);
      }
      return machine == null ? null : new List<MachineDetails> { machine };
    }

    /// <summary>
    /// 
    /// </summary>
    public static void ProcessStatusCode(this Controller controller, ServiceException serviceException)
    {
      if (serviceException.Code == HttpStatusCode.BadRequest &&
          serviceException.GetResult.Code == ContractExecutionStatesEnum.FailedToGetResults)
      {
        serviceException.Code = HttpStatusCode.NoContent;
      }
    }
  }
}