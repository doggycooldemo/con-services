﻿using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Communication.Tcp;
using Apache.Ignite.Core.Configuration;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Static;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Apache.Ignite.Core.Deployment;
using VSS.TRex.GridFabric.Affinity;
using VSS.TRex.GridFabric.Grids;
using VSS.TRex.Logging;
using VSS.TRex.Storage;

namespace VSS.TRex.Servers.Client
{
    /// <summary>
    /// Defines a representation of a client able to request TRex related compute operations using
    /// the Ignite In Memory Data Grid. All client type server classes should descend from this class.
    /// </summary>
    public class MutableClientServer : IgniteServer
    {
        private static readonly ILogger Log = Logger.CreateLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);

        /// <summary>
        /// Constructor that creates a new server instance with a single role
        /// </summary>
        /// <param name="role"></param>
        public MutableClientServer(string role) : this(new [] { role })
        {
        }

        /// <summary>
        /// Constructor that creates a new server instance with a set of roles
        /// </summary>
        /// <param name="roles"></param>
        public MutableClientServer(string [] roles)
        {
            if (mutableTRexGrid == null)
            {
                // Attempt to attach to an already existing Ignite instance
                mutableTRexGrid = TRexGridFactory.Grid(TRexGrids.MutableGridName());

                // If there was no connection obtained, attempt to create a new instance
                if (mutableTRexGrid == null)
                {
                    string roleNames = roles.Aggregate("|", (s1, s2) => s1 + s2 + "|");

                    TRexNodeID = Guid.NewGuid().ToString();

                    Log.LogInformation($"Creating new Ignite node with Roles = {roleNames} & TRexNodeId = {TRexNodeID}");

                    IgniteConfiguration cfg = new IgniteConfiguration()
                    {
                        // SpringConfigUrl = @".\TRexIgniteConfig.xml",

                        IgniteInstanceName = TRexGrids.MutableGridName(),
                        ClientMode = true,

                        JvmInitialMemoryMb = 512, // Set to minimum advised memory for Ignite grid JVM of 512Mb
                        JvmMaxMemoryMb = 1 * 1024, // Set max to 1Gb

                        UserAttributes = new Dictionary<string, object>()
                        {
                            { "TRexNodeId", TRexNodeID }
                        },

                        // Enforce using only the LocalHost interface
                        DiscoverySpi = new TcpDiscoverySpi()
                        {
                            LocalAddress = "127.0.0.1",
                            LocalPort = 48500,

                            IpFinder = new TcpDiscoveryStaticIpFinder()
                            {
                                Endpoints = new [] { "127.0.0.1:48500..48509" }
                            }
                        },

                        CommunicationSpi = new TcpCommunicationSpi()
                        {
                            LocalAddress = "127.0.0.1",
                            LocalPort = 48100,
                        },

                        Logger = new TRexIgniteLogger(Logger.CreateLogger("MutableClientServer")),
                        
                        // Don't permit the Ignite node to use more than 1Gb RAM (handy when running locally...)
                        DataStorageConfiguration = new DataStorageConfiguration()
                        {
                            PageSize = DataRegions.DEFAULT_MUTABLE_DATA_REGION_PAGE_SIZE,

                            DefaultDataRegionConfiguration = new DataRegionConfiguration
                            {
                                Name = DataRegions.DEFAULT_MUTABLE_DATA_REGION_NAME,
                                InitialSize = 128 * 1024 * 1024,  // 128 MB
                                MaxSize = 256 * 1024 * 1024,  // 128 MB
                                PersistenceEnabled = false
                            },

                            // Establish a separate data region for the TAG file buffer queue
                            DataRegionConfigurations = new List<DataRegionConfiguration>
                            {
                                new DataRegionConfiguration
                                {
                                    Name = DataRegions.TAG_FILE_BUFFER_QUEUE_DATA_REGION,
                                    InitialSize = 128 * 1024 * 1024,  // 128 MB
                                    MaxSize = 256 * 1024 * 1024,  // 128 MB

                                    PersistenceEnabled = false
                                }
                            }
                        },

                        // Set an Ignite metrics heartbeat of 10 seconds 
                        MetricsLogFrequency = new TimeSpan(0, 0, 0, 10),

                        PublicThreadPoolSize = 50,

                        PeerAssemblyLoadingMode = PeerAssemblyLoadingMode.Disabled

                      //BinaryConfiguration = new BinaryConfiguration(typeof(TestQueueItem))
                    };

                    foreach (string roleName in roles)
                    {
                        cfg.UserAttributes.Add($"{ServerRoles.ROLE_ATTRIBUTE_NAME}-{roleName}", "True");
                    }

                    try
                    {
                        mutableTRexGrid = Ignition.Start(cfg);
                    }
                    catch (Exception e)
                    {
                        Log.LogInformation($"Creation of new Ignite node with Role = {roleNames} & TRexNodeId = {TRexNodeID} failed with exception {e}");
                    }
                    finally
                    {
                        Log.LogInformation($"Completed creation of new Ignite node with Role = {roleNames} & TRexNodeId = {TRexNodeID}");
                    }
                }
            }
        }

        public override ICache<NonSpatialAffinityKey, byte[]> InstantiateTRexCacheReference(CacheConfiguration CacheCfg)
        {
            return mutableTRexGrid.GetCache<NonSpatialAffinityKey, byte[]>(CacheCfg.Name);
        }

        public override ICache<SubGridSpatialAffinityKey, byte[]> InstantiateSpatialCacheReference(CacheConfiguration CacheCfg)
        {
            return mutableTRexGrid.GetCache<SubGridSpatialAffinityKey, byte[]>(CacheCfg.Name);
        }       
    }
}
