﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VSS.Common.Abstractions.Clients.CWS.Interfaces;
using VSS.MasterData.Project.WebAPI.Common.Models;
using VSS.MasterData.Project.WebAPI.Common.Utilities;
using VSS.Productivity3D.Project.Abstractions.Models.ResultsHandling;
using VSS.Visionlink.Interfaces.Events.MasterData.Models;

namespace VSS.MasterData.Project.WebAPI.Controllers
{
  /// <summary>
  /// Customer controller v1
  ///     for the UI to get customer list etc as we have no CustomerSvc yet
  /// </summary>
  public class CustomerV1Controller : ProjectBaseController
  {
    private readonly ICwsAccountClient _cwsAccountClient;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public CustomerV1Controller(ICwsAccountClient cwsAccountClient)
    {
      this._cwsAccountClient = cwsAccountClient;
    }

    /// <summary>
    /// Gets a list of customers for the user in user token. 
    /// </summary>
    [Route("api/v1/Customers/me")]
    [HttpGet]
    public async Task<CustomerV1ListResult> GetCustomersForMe()
    {
      Logger.LogInformation(nameof(GetCustomersForMe));
      var customers = await _cwsAccountClient.GetMyAccounts(new Guid(userId));
      return new CustomerV1ListResult
      {
        Customers = customers.Accounts.Select(c =>
            AutoMapperUtility.Automapper.Map<CustomerData>(c))
            .ToList()
      };
    }

    /// <summary>
    /// Gets the total devices licensed for this customer. 
    ///   Also triggers a lazy load of devices from cws, so that shortRaptorAssetId is generated.
    /// </summary>
    [Route("api/v1/customer/license/{customerUid}")]
    [HttpGet]
    public async Task<CustomerV1DeviceLicenseResult> GetCustomerDeviceLicense(string customerUid)
    {
      Logger.LogInformation(nameof(GetCustomerDeviceLicense));
      var deviceLicenses = await _cwsAccountClient.GetDeviceLicenses(new Guid(customerUid));

      // CCSSSCON-207 may want to move this, and into executor
      //  Which endpoint does the UI use to actually select the project. 
      //     That is the endpoint which should load any devices for the account.
      //     These need to be loaded into the localDB device table so that shortRaptorAssetIds can be generated.
      //     The user, after adding devices must login to WorksOS to trigger this process,
      //        so that when tag files are loaded, the new deviceTRN+shortRaptorAssetId will be available
      var deviceList = await CwsDeviceClient.GetDevicesForAccount(new Guid(customerUid));
      foreach (var device in deviceList.Devices)
      {
        // if it exists, does nothing but return a count of 0
        //  don't store customerUid, so that we don't need to move the device if ownership changes.
        //      may ned to change this is we ever need a time-component to ownership
        // CCSSSCON-207 do we care what the status is?
        await DeviceRepo.StoreEvent(AutoMapperUtility.Automapper.Map<CreateDeviceEvent>(device));
      }

      return new CustomerV1DeviceLicenseResult(deviceLicenses.Total);
    }
  }
}

