﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VSS.ConfigurationStore;
using VSS.KafkaConsumer.Kafka;
using VSS.MasterData.Repositories;
using VSS.Productivity3D.TagFileAuth.WebAPI.Models.Executors;
using VSS.Productivity3D.TagFileAuth.WebAPI.Models.Models;
using VSS.Productivity3D.TagFileAuth.WebAPI.Models.ResultHandling;
using VSS.Productivity3D.TagFileAuth.WebAPI.Models.Utilities;
using VSS.VisionLink.Interfaces.Events.MasterData.Interfaces;

namespace VSS.Productivity3D.TagFileAuth.WebAPI.Controllers
{
  /// <summary>
  /// Project controller.
  /// </summary>
  public class ProjectV2Controller : BaseController
  {
    private readonly ILogger _log;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public ProjectV2Controller(ILoggerFactory logger, IConfigurationStore configStore,
      IRepository<IAssetEvent> assetRepository, IRepository<IDeviceEvent> deviceRepository,
      IRepository<ICustomerEvent> customerRepository, IRepository<IProjectEvent> projectRepository,
      IRepository<ISubscriptionEvent> subscriptionsRepository, IKafka producer)
      : base(logger, configStore, assetRepository, deviceRepository,
        customerRepository, projectRepository,
        subscriptionsRepository, producer)
    {
      _log = logger.CreateLogger<ProjectV2Controller>();
    }

    /// <summary>
    /// This endpoint is used by CTCTs Earthworks product,
    ///      the endpoint is known as the 'ProjectDiscovery' endpoint, 
    ///      to allow a user once or twice a day
    ///      to obtain a Cut/fill or other map from 3dpService. 
    ///      This step tries to identify a unique projectUid.
    /// 
    /// Gets the ProjectUid where
    ///     which belongs to the devices Customer and 
    ///     whose boundary the location is inside at the given date time. 
    ///     authority is determined by servicePlans from the provided deviceUid.
    /// </summary>
    /// <param name="request">Details of the asset, location and date time</param>
    /// <returns>
    /// The project Uid if the asset is inside ONE todo: or more> TBD
    ///      project otherwise a returnCode.
    /// </returns>
    /// <executor>ProjectUidExecutor</executor>
    [Route("api/v2/project/getUid")]
    [HttpPost]
    public async Task<GetProjectUidResult> GetProjectUid([FromBody]GetProjectUidRequest request)
    {
      _log.LogDebug("GetProjectUid: request:{0}", JsonConvert.SerializeObject(request));
      request.Validate();

      var executor = RequestExecutorContainer.Build<ProjectUidExecutor>(_log, configStore, assetRepository, deviceRepository, customerRepository, projectRepository, subscriptionsRepository);
      var result = await executor.ProcessAsync(request) as GetProjectUidResult;

      _log.LogResult(ToString(), request, result);
      return result;
    }
  }
}
