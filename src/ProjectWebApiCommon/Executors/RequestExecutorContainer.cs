﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VSS.ConfigurationStore;
using VSS.KafkaConsumer.Kafka;
using VSS.MasterData.Project.WebAPI.Common.Internal;
using VSS.MasterData.Project.WebAPI.Common.Models;
using VSS.MasterData.Project.WebAPI.Common.ResultsHandling;
using VSS.MasterData.Repositories;

namespace VSS.MasterData.Project.WebAPI.Common.Executors
{
  /// <summary>
  ///   Represents abstract container for all request executors. Uses abstract factory pattern to seperate executor logic
  ///   from controller logic for testability and possible executor versioning.
  /// </summary>
  public abstract class RequestExecutorContainer
  {
    /// <summary>
    /// Repository factory used in ProcessEx
    /// </summary>
    protected IProjectRepository projectRepo;

    /// <summary>
    /// Configuration items
    /// </summary>
    protected IConfigurationStore configStore;

    /// <summary>
    /// Logger for logging
    /// </summary>
    protected ILogger log;

    /// <summary>
    /// handle exceptions
    /// </summary>
    protected IServiceExceptionHandler serviceExceptionHandler;

    /// <summary>
    /// Gets or sets the Kafak consumer.
    /// </summary>
    protected IKafka producer;

    /// <summary>
    /// Gets or sets the Kafka topic.
    /// </summary>
    protected string kafkaTopicName;



    /// <summary>
    /// Generates the dynamic errorlist for instanciated executor.
    /// </summary>
    /// <returns>List of errors with corresponding descriptions.</returns>
    public List<Tuple<int, string>> GenerateErrorlist()
    {
      List<Tuple<int, string>> result = new List<Tuple<int, string>>();
      for (int i = 0; i < ContractExecutionStates.Count; i++)
      {
        result.Add(new Tuple<int, string>(ContractExecutionStates.ValueAt(i),
          ContractExecutionStates.NameAt(i)));
      }
      ContractExecutionStates.ClearDynamic();
      return result;
    }

    /// <summary>
    /// Processes the specified item. This is the main method to execute real action.
    /// </summary>
    /// <typeparam name="T">>Generic type which should be</typeparam>
    /// <param name="item">>The item.</param>
    /// <returns></returns>
    protected abstract ContractExecutionResult ProcessEx<T>(T item);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <returns></returns>
    protected virtual Task<ContractExecutionResult> ProcessAsyncEx<T>(T item)
    {
      throw new ServiceException(HttpStatusCode.InternalServerError,
        new ContractExecutionResult(ContractExecutionStatesEnum.InternalProcessingError, "Missing asynchronous executor process method override"));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ServiceException"></exception>
    public ContractExecutionResult Process<T>(T item)
    {
      if (item == null)
        throw new ServiceException(HttpStatusCode.BadRequest,
          new ContractExecutionResult(ContractExecutionStatesEnum.InternalProcessingError, "Serialization error"));
      return ProcessEx(item);
    }

    public async Task<ContractExecutionResult> ProcessAsync<T>(T item)
    {
      if (item == null)
        throw new ServiceException(HttpStatusCode.BadRequest,
          new ContractExecutionResult(ContractExecutionStatesEnum.InternalProcessingError, "Serialization error"));
      return await ProcessAsyncEx(item);
    }

    /// <summary>
    /// Gets the available contract execution error states.
    /// </summary>
    /// <value>
    /// The contract execution states.
    /// </value>
    protected ContractExecutionStatesEnum ContractExecutionStates { get; }

    /// <summary>
    /// Default constructor which creates all structures necessary for error handling.
    /// </summary>
    protected RequestExecutorContainer()
    {
      ContractExecutionStates = new ContractExecutionStatesEnum();
      ProcessErrorCodes();
    }

    /// <summary>
    /// Dynamically defines new error codes for the executor instance. Don't forget to clean them up after exit.
    /// </summary>
    protected virtual void ProcessErrorCodes()
    { }

    /// <summary>
    /// Injected constructor for mocking.
    /// </summary>
    protected RequestExecutorContainer(IProjectRepository projectRepo, IConfigurationStore configStore,  ILoggerFactory logger, IServiceExceptionHandler serviceExceptionHandler, IKafka producer) : this()
    {
      this.projectRepo = projectRepo;
      this.configStore = configStore;
      if (logger != null)
        log = logger.CreateLogger<RequestExecutorContainer>();
      this.serviceExceptionHandler = serviceExceptionHandler;
      this.producer = producer;
      kafkaTopicName = "VSS.Interfaces.Events.MasterData.IProjectEvent" +
                       configStore.GetValueString("KAFKA_TOPIC_NAME_SUFFIX");
    }

    /// <summary>
    /// Default destructor which destroys all structures necessary for error handling.
    /// </summary>
    ~RequestExecutorContainer()
    {
      ContractExecutionStates?.ClearDynamic();
    }

    /// <summary>
    ///   Builds this instance for specified executor type.
    /// </summary>
    /// <typeparam name="TExecutor">The type of the executor.</typeparam>
    /// <returns></returns>
    public static TExecutor Build<TExecutor>(IProjectRepository projectRepo, IConfigurationStore configStore, ILoggerFactory logger, IServiceExceptionHandler serviceExceptionHandler, IKafka producer, string kafkaTopicName = null)
      where TExecutor : RequestExecutorContainer, new()
    {
      var executor = new TExecutor() { projectRepo = projectRepo, configStore = configStore, log = logger.CreateLogger<TExecutor>(), serviceExceptionHandler = serviceExceptionHandler, producer = producer, kafkaTopicName = kafkaTopicName};
      return executor;
    }

  }
}

