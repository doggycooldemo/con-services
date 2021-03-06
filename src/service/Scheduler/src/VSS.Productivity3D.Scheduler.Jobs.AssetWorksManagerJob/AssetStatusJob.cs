﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VSS.MasterData.Models.Models;
using VSS.MasterData.Models.ResultHandling.Abstractions;
using VSS.MasterData.Proxies.Interfaces;
using VSS.Productivity.Push.Models;
using VSS.Productivity3D.AssetMgmt3D.Abstractions.Models;
using VSS.Productivity3D.Productivity3D.Abstractions.Interfaces;
using VSS.Productivity3D.Productivity3D.Models.ProductionData;
using VSS.Productivity3D.Project.Abstractions.Interfaces;
using VSS.Productivity3D.Push.Abstractions.AssetLocations;
using VSS.Productivity3D.Scheduler.Abstractions;

namespace VSS.Productivity3D.Scheduler.Jobs.AssetStatusJob
{
  public class AssetStatusJob : IJob
  {
    public static Guid VSSJOB_UID = Guid.Parse("8b179467-2b36-49fc-aee0-ee5d4f4e8efa");

    public Guid VSSJobUid => VSSJOB_UID;

    private readonly IAssetStatusServerHubClient _assetStatusServerHubClient;
    private readonly IFleetAssetDetailsProxy _assetDetailsProxy;
    private readonly IFleetAssetSummaryProxy _assetSummaryProxy;
    private readonly IProductivity3dV2ProxyNotification _productivity3dV2ProxyNotification;
    private readonly ILogger _log;

    private List<AssetUpdateSubscriptionModel> _subscriptions;

    public AssetStatusJob(IAssetStatusServerHubClient assetStatusServerHubClient,
      IFleetAssetDetailsProxy assetDetailsProxy, IFleetAssetSummaryProxy assetSummaryProxy, 
      IProductivity3dV2ProxyNotification productivity3dV2ProxyNotification, ILoggerFactory logger)
    {
      this._assetStatusServerHubClient = assetStatusServerHubClient;
      _log = logger.CreateLogger<AssetStatusJob>();
      this._assetDetailsProxy = assetDetailsProxy;
      this._productivity3dV2ProxyNotification = productivity3dV2ProxyNotification;
      this._assetSummaryProxy = assetSummaryProxy;
    }

    public Task Setup(object o, object context)
    {
      if (_assetStatusServerHubClient.IsConnecting || _assetStatusServerHubClient.Connected)
        return Task.CompletedTask;
      _log.LogInformation("Asset Status Hub Client not connected, starting a connection.");
      return _assetStatusServerHubClient.Connect();
    }

    public async Task Run(object o, object context)
    {
      _subscriptions = await _assetStatusServerHubClient.GetSubscriptions();
      _log.LogInformation($"Found {_subscriptions.Count} subscriptions to process");
      var tasks = _subscriptions.Select(ProcessSubscription).ToList();
      await Task.WhenAll(tasks);
    }

    public Task TearDown(object o, object context)
    {
      return Task.CompletedTask;
    }

    private async Task ProcessSubscription(AssetUpdateSubscriptionModel subscriptionModel)
    {
      try
      {
        //Get machines
        //https://3dproductivity.myvisionlink.com/t/trimble.com/vss-3dproductivity/2.0/projects/a530371d-20a1-40cf-99ce-e11c54140be4/machines
        var route = $"/projects/{subscriptionModel.ProjectUid}/machines";
        var machines = await _productivity3dV2ProxyNotification.ExecuteGenericV2Request<Machine3DStatuses>(route,
          HttpMethod.Get,
          null,
          subscriptionModel.Headers());

        if (machines.Code != ContractExecutionStatesEnum.ExecutedSuccessfully || !machines.MachineStatuses.Any())
          //Nothing to do here. Breaking.
          return;

        var processingAssets = new List<Task>();

        //Now for each machine try to identify a matching asset 
        foreach (var machine in machines.MachineStatuses)
        {
          var task = ProcessAssetEvents(machine, subscriptionModel.ProjectUid, subscriptionModel.CustomerUid, subscriptionModel.Headers());
          processingAssets.Add(task);
        }

        await Task.WhenAll(processingAssets);
      }
      catch (Exception e)
      {
        _log.LogError(e, $"Exception when running subscription {JsonConvert.SerializeObject(subscriptionModel)}");
      }
    }

    /// <summary>
    /// Fetch asset data, the proxy will cache multiple request to the same asset
    /// </summary>
    private async Task<(AssetDetails details, AssetSummary summary)> GetAssetData(string assetUid, IHeaderDictionary headers)
    {
      var assetDetailsTask = _assetDetailsProxy.GetAssetDetails(assetUid, headers);
      var assetSummaryTask = _assetSummaryProxy.GetAssetSummary(assetUid, headers);

      await Task.WhenAll(assetDetailsTask, assetSummaryTask);

      return (assetDetailsTask.Result, assetSummaryTask.Result);
    }

    private async Task ProcessAssetEvents(MachineStatus machine, Guid projectUid, Guid customerUid, IHeaderDictionary headers)
    {
      AssetAggregateStatus statusEvent = null;
      
      if (machine.AssetUid.HasValue)
      {
        // CCSSCOON-85 it's not possible to match radio types in WM cws/ProfileX
        // var matchingAsset = await _deviceProxy.GetMatching3D2DAssets(new MatchingAssetsDisplayModel() {AssetUID3D = assetList.First().Key.ToString()}, headers);
        //Change that for the actual matched asset. Since we supplied 3d asset get data for the matching 2d asset.
        //if there is no 2d asset we should try using SNM asset

        var (details, summary) = await GetAssetData(machine.AssetUid.ToString(), headers);

        statusEvent = GenerateEvent(customerUid, projectUid, machine.AssetUid, machine, details, summary);
      }
      else
      {
        // Cant find some information, build a cut down event
        statusEvent = GenerateEvent(customerUid, projectUid, null, machine, null, null);
      }

      if (statusEvent != null)
        await _assetStatusServerHubClient.UpdateAssetLocationsForClient(statusEvent);
    }

    /// <summary>
    /// Generate a UI Event based on the information passed in
    /// Add any new fields that need populating to this method
    /// </summary>
    private AssetAggregateStatus GenerateEvent(Guid customerUid, Guid projectUid, Guid? assetUid, MachineStatus machineStatus, AssetDetails details, AssetSummary summary)
    {
      var result = new AssetAggregateStatus
      {
        ProjectUid = projectUid,
        CustomerUid = customerUid,
        AssetUid = assetUid,
        UtilizationSummary = summary
      };

      // This is where all the magic happens, in terms of mapping data we have from 3d / 2d endpoints into an event for the UI
      // details / summary can be null, machineStatus won't be.
      var lastLocationTimeUtc = machineStatus.lastKnownTimeStamp;
      // These values are in radians, where the AssetDetails values are in degrees
      result.Latitude = machineStatus.lastKnownLatitude?.LatRadiansToDegrees();
      result.Longitude = machineStatus.lastKnownLongitude?.LonRadiansToDegrees();
      result.Design = machineStatus.lastKnownDesignName;
      result.LiftNumber = machineStatus.lastKnownLayerId;
      result.MachineName = machineStatus.MachineName;

      // If we have a Asset ID (which matches Asset ID in Fleet management) from UF, use that, otherwise machine name
      result.AssetIdentifier = !string.IsNullOrEmpty(details?.AssetId)
        ? details.AssetId
        : machineStatus.MachineName;

      // Extract data from Asset Details
      if (details != null)
      {
        // Do we have a newer location?
        if (lastLocationTimeUtc == null || details.LastLocationUpdateUtc > lastLocationTimeUtc)
        {
          result.Latitude = details.LastReportedLocationLatitude;
          result.Longitude = details.LastReportedLocationLongitude;
          lastLocationTimeUtc = details.LastLocationUpdateUtc;
        }

        result.FuelLevel = details.FuelLevelLastReported;
        result.FuelLevelLastUpdatedUtc = details.FuelReportedTimeUtc;
        result.AssetIcon = details.AssetIcon;
        result.AssetSerialNumber = details.AssetSerialNumber;
        if (details.Devices?.Count > 0)
        {
          result.DeviceName = string.Join(", ", details.Devices.Select(d => d.DeviceType));
        }
      }

      // Clear the values if we don't have everything
      if (lastLocationTimeUtc == null || result.Latitude == null || result.Longitude == null)
      {
        _log.LogWarning($"Clearing event information due to missing data. Lat: {result.Latitude}, Lon: {result.Longitude}, Time: {result.LocationLastUpdatedUtc}");
        result.LocationLastUpdatedUtc = null;
        result.Latitude = null;
        result.Longitude = null;
      }
      else
      {
        result.LocationLastUpdatedUtc = lastLocationTimeUtc;
      }

      return result;
    }
  }
}
