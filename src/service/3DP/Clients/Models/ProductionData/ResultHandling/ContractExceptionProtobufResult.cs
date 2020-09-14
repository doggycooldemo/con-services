﻿using Newtonsoft.Json;
using ProtoBuf;
using VSS.MasterData.Models.ResultHandling.Abstractions;

namespace VSS.Productivity3D.Productivity3D.Models.ProductionData.ResultHandling
{
  /// <summary>
  ///   Represents general (minimal) response generated by a service. All other responses should be derived from this class.
  /// </summary>
  public class ContractExecutionProtobufResult
  {
    public const string DefaultMessage = "success";

    [ProtoMember(5, IsRequired = false)]
    [JsonProperty(PropertyName = "code")]
    public int Code { get; protected set; }

    [ProtoMember(6, IsRequired = false)]
    [JsonProperty(PropertyName = "message")]
    public string Message { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContractExecutionResult" /> class with default
    /// <see cref="ContractExecutionStatesEnum.ExecutedSuccessfully" /> result and "success" message
    /// </summary>
    public ContractExecutionProtobufResult()
      : this(DefaultMessage)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContractExecutionResult" /> class.
    /// </summary>
    /// <param name="code">
    /// The resulting code. Default value is <see cref="ContractExecutionStatesEnum.ExecutedSuccessfully" />
    /// </param>
    /// <param name="message">The verbose user-friendly message. Default value is empty string.</param>
    public ContractExecutionProtobufResult(int code, string message = DefaultMessage)
    {
      Code = code;
      Message = message;
    }

    public static ContractExecutionProtobufResult ErrorResult(string errorMessage = "Unhandled error state") => new ContractExecutionProtobufResult(
      ContractExecutionStatesEnum.InternalProcessingError,
      errorMessage);

    /// <summary>
    /// Initializes a new instance of the <see cref="ContractExecutionResult" /> class with default
    /// <see cref="ContractExecutionStatesEnum.ExecutedSuccessfully" /> result
    /// </summary>
    /// <param name="message">The verbose user-friendly message.</param>
    protected ContractExecutionProtobufResult(string message)
      : this(ContractExecutionStatesEnum.ExecutedSuccessfully, message)
    { }
  }
}
