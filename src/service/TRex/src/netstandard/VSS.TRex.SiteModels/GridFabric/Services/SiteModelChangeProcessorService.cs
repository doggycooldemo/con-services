﻿using System.Linq;
using System.Threading;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Binary;
using Apache.Ignite.Core.Cache.Query;
using Apache.Ignite.Core.Cache.Query.Continuous;
using Apache.Ignite.Core.Services;
using Microsoft.Extensions.Logging;
using VSS.TRex.Common;
using VSS.TRex.Common.Extensions;
using VSS.TRex.Common.Interfaces;
using VSS.TRex.DI;
using VSS.TRex.GridFabric.Grids;
using VSS.TRex.SiteModels.GridFabric.Queues;
using VSS.TRex.SiteModels.Interfaces.GridFabric.Queues;
using VSS.TRex.Storage.Caches;
using VSS.TRex.Storage.Models;

namespace VSS.TRex.SiteModels.GridFabric.Services
{
  /// <summary>
  /// The Ignite deployed service that processes site model change notifications (representing the set of sub grid spatial data
  /// that changed as a result of processing a set of TAG file data) and representing those changes in the per-machine tracking
  /// to enable machine to request data with 'only data that changed since last visit' semantics
  /// </summary>
  public class SiteModelChangeProcessorService : IService, ISiteModelChangeProcessorService, IBinarizable, IFromToBinary
  {
    private static readonly ILogger Log = Logging.Logger.CreateLogger<SiteModelChangeProcessorService>();

    private const byte VERSION_NUMBER = 1;

    /// <summary>
    /// The interval between epochs where the service checks to see if there is anything to do
    /// </summary>
    private int serviceCheckIntervalMS = 1000;

    /// <summary>
    /// Flag set then Cancel() is called to instruct the service to finish operations
    /// </summary>
    private bool aborted;

    /// <summary>
    /// The event wait handle used to mediate sleep periods between operation epochs of the service
    /// </summary>
    private EventWaitHandle waitHandle;

    /// <summary>
    /// Default no-args constructor
    /// </summary>
    public SiteModelChangeProcessorService()
    {
    }

    /// <summary>
    /// Initializes the service ready for accessing buffered site model spatial changer sets and providing them to processing contexts
    /// </summary>
    /// <param name="context"></param>
    public void Init(IServiceContext context)
    {
      Log.LogInformation($"{nameof(SiteModelChangeProcessorService)} {context.Name} initializing");
    }

    /// <summary>
    /// Executes the life cycle of the service until it is aborted
    /// </summary>
    /// <param name="context"></param>
    public void Execute(IServiceContext context)
    {
      try
      {
        Log.LogInformation($"{nameof(SiteModelChangeProcessorService)} {context.Name} starting executing");

        aborted = false;
        waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

        // Get the ignite grid and cache references

        var _ignite = DIContext.Obtain<ITRexGridFactory>()?.Grid(StorageMutability.Mutable) ??
                      Ignition.GetIgnite(TRexGrids.MutableGridName());

        if (_ignite == null)
        {
          Log.LogError("Ignite reference in service is null - aborting service execution");
          return;
        }

        var queueCache = _ignite.GetCache<ISiteModelChangeBufferQueueKey, SiteModelChangeBufferQueueItem> (TRexCaches.SiteModelChangeBufferQueueCacheName());

        var handler = new SiteModelChangeProcessorItemHandler();

        // Construct the continuous query machinery
        // Set the initial query to return all elements in the cache
        // Instantiate the queryHandle and start the continuous query on the remote nodes
        // Note: Only cache items held on this local node will be handled here
        using (var queryHandle = queueCache.QueryContinuous
        (qry: new ContinuousQuery<ISiteModelChangeBufferQueueKey, SiteModelChangeBufferQueueItem>(new LocalSiteModelChangeListener(handler)) {Local = true},
          initialQry: new ScanQuery<ISiteModelChangeBufferQueueKey, SiteModelChangeBufferQueueItem> {Local = true}))
        {
          Log.LogInformation("Performing initial continuous query cursor scan of items to process");

          // Perform the initial query to grab all existing elements and process them
          queryHandle.GetInitialQueryCursor().ForEach(x =>
          {
            if (handler.Process(x.Value))
            {
              queueCache.Remove(x.Key);
            }
          });

          // Cycle looking for new work to do as items arrive until aborted...
          Log.LogInformation("Entering steady state continuous query scan of items to process");

          do
          {
            waitHandle.WaitOne(serviceCheckIntervalMS);
          } while (!aborted);
        }
      }
      finally
      {
        Log.LogInformation($"{nameof(SiteModelChangeProcessorService)} {context.Name} completed executing");
      }
    }

    /// <summary>
    /// Cancels the current operation context of the service
    /// </summary>
    /// <param name="context"></param>
    public void Cancel(IServiceContext context)
    {
      Log.LogInformation($"{nameof(SiteModelChangeProcessorService)} {context.Name} cancelling");

      aborted = true;
      waitHandle?.Set();
    }

    public void WriteBinary(IBinaryWriter writer) => ToBinary(writer.GetRawWriter());

    public void ReadBinary(IBinaryReader reader) => FromBinary(reader.GetRawReader());

    /// <summary>
    /// The service has no serialization requirements
    /// </summary>
    /// <param name="writer"></param>
    public void ToBinary(IBinaryRawWriter writer)
    {
      VersionSerializationHelper.EmitVersionByte(writer, VERSION_NUMBER);

      writer.WriteInt(serviceCheckIntervalMS);
    }

    /// <summary>
    /// The service has no serialization requirements
    /// </summary>
    /// <param name="reader"></param>
    public void FromBinary(IBinaryRawReader reader)
    {
      VersionSerializationHelper.CheckVersionByte(reader, VERSION_NUMBER);

      serviceCheckIntervalMS = reader.ReadInt();
    }
  }
}
