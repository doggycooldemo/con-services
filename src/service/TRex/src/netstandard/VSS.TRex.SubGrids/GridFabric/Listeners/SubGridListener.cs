﻿using Apache.Ignite.Core.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Apache.Ignite.Core.Binary;
using VSS.TRex.Common.Exceptions;
using VSS.TRex.Common.Interfaces;
using VSS.TRex.DI;
using VSS.TRex.Pipelines.Interfaces.Tasks;
using VSS.TRex.SubGridTrees.Client.Interfaces;

namespace VSS.TRex.SubGrids.GridFabric.Listeners
{
  /// <summary>
  /// SubGridListener implements a listening post for subgrid results being sent by processing nodes back
  /// to the local context for further processing when using a progressive style of subgrid requesting. 
  /// Subgrids are sent in groups as serialized streams held in memory streams to minimize serialization/deserialization overhead
  /// </summary>
  public class SubGridListener : IMessageListener<byte[]>, IBinarizable, IFromToBinary
  {
    private static readonly ILogger Log = Logging.Logger.CreateLogger<SubGridListener>();

    private const byte VERSION_NUMBER = 1;

    /// <summary>
    /// Count of the number of responses received by this listener
    /// </summary>
    private int responseCounter;

    /// <summary>
    /// Local reference to the client subgrid factory
    /// </summary>
    private static IClientLeafSubgridFactory clientLeafSubGridFactory;

    private IClientLeafSubgridFactory ClientLeafSubGridFactory
      => clientLeafSubGridFactory ?? (clientLeafSubGridFactory = DIContext.Obtain<IClientLeafSubgridFactory>());

    /// <summary>
    /// The reference to the TRexTask responsible for handling the returned subgrid information from the processing cluster
    /// </summary>
    private readonly ITRexTask TRexTask;

    /// <summary>
    /// Processes a response containing a set of subgrids from the subgrid processor for a request
    /// </summary>
    /// <param name="message"></param>
    private void ProcessResponse(byte[] message)
    {
      using (var MS = new MemoryStream(message))
      {
        using (BinaryReader reader = new BinaryReader(MS, Encoding.UTF8, true))
        {
          // Read the number of subgrid present in the stream
          int responseCount = reader.ReadInt32();

          // Create a single instance of the client grid. The approach here is that TransferResponse does not move ownership 
          // to the called context (it may clone the passed in client grid if desired)
          IClientLeafSubGrid[][] clientGrids = new IClientLeafSubGrid[responseCount][];

          try
          {
            byte[] buffer = new byte[10000];

            for (int i = 0; i < responseCount; i++)
            {
              int subgridCount = reader.ReadInt32();
              clientGrids[i] = new IClientLeafSubGrid[subgridCount];

              for (int j = 0; j < subgridCount; j++)
              {
                clientGrids[i][j] = ClientLeafSubGridFactory.GetSubGrid(TRexTask.GridDataType);

                // Check if the returned subgrid is null
                if (reader.ReadBoolean())
                {
                  clientGrids[i][j].Read(reader, buffer);
                }
                else
                {
                  Log.LogWarning($"Subgrid at position [{i},{j}] in subgrid response array is null");
                }
              }
            }

            // Log.InfoFormat("Transferring response#{0} to processor (from thread {1})", thisResponseCount, System.Threading.Thread.CurrentThread.ManagedThreadId);

            // Send the decoded grid to the PipelinedTask, but ensure subgrids are serialized into the TRexTask
            // (no assumption of thread safety within the TRexTask itself)
            try
            {
              lock (TRexTask)
              {
                for (int i = 0; i < responseCount; i++)
                {
                  int thisResponseCount = ++responseCounter;

                  if (TRexTask.TransferResponse(clientGrids[i]))
                  {
                    // Log.DebugFormat("Processed response#{0} (from thread {1})", thisResponseCount, System.Threading.Thread.CurrentThread.ManagedThreadId);
                  }
                  else
                  {
                    Log.LogInformation(
                      $"Processing response#{thisResponseCount} FAILED (from thread {Thread.CurrentThread.ManagedThreadId})");
                  }
                }
              }
            }
            finally
            {
              // Tell the pipeline that a set of subgrid have been completely processed
              TRexTask.PipeLine.SubgridsProcessed(responseCount);
            }

          }
          finally
          {
            // Return the client grid to the factory for recycling now its role is complete here... when using ConcurrentBag
            // ClientLeafSubGridFactory.ReturnClientSubGrid(ref clientGrid);

            // Return the client grid to the factory for recycling now its role is complete here... when using SimpleConcurrentBag
            ClientLeafSubGridFactory.ReturnClientSubGrids(clientGrids, responseCount);
          }
        }
      }
    }

    /// <summary>
    /// The method called to announce the arrival of a message from a remote context in the cluster
    /// </summary>
    /// <param name="nodeId"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public bool Invoke(Guid nodeId, byte[] message)
    {
      Task.Run(() => ProcessResponse(message));

      return true;
    }

    /// <summary>
    /// Constructor accepting a rexTask to pass subgrids into
    /// </summary>
    /// <param name="tRexTask"></param>
    public SubGridListener(ITRexTask tRexTask)
    {
      TRexTask = tRexTask;
    }

    /// <summary>
    /// The subgrid response listener has no serializable state
    /// </summary>
    /// <param name="writer"></param>
    public void WriteBinary(IBinaryWriter writer) => ToBinary(writer.GetRawWriter());

    /// <summary>
    /// The subgrid response listener has no serializable state
    /// </summary>
    /// <param name="reader"></param>
    public void ReadBinary(IBinaryReader reader) => FromBinary(reader.GetRawReader());

    public void ToBinary(IBinaryRawWriter writer)
    {
      writer.WriteByte(VERSION_NUMBER);
    }

    public void FromBinary(IBinaryRawReader reader)
    {
      var version = reader.ReadByte();

      if (version != VERSION_NUMBER)
        throw new TRexSerializationVersionException(VERSION_NUMBER, version);
    }
  }
}
