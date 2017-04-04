﻿
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using VSS.Raptor.Service.Common.ResultHandling;

namespace VSS.Raptor.Service.Common.Contracts
{
  /// <summary>
  ///   Represents general (minimal) reponse generated by a sevice. All other responses should be derived from this class.
  /// </summary>
  public class ContractExecutionResult
  {
    /// <summary>
    /// The result of the request. True for success and false for failure.
    /// </summary>
    public bool result { get; set; }

    public const string DefaultMessage = "success";

    /// <summary>
    ///   Initializes a new instance of the <see cref="ContractExecutionResult" /> class.
    /// </summary>
    /// <param name="code">
    ///   The resulting code. Default value is <see cref="ContractExecutionStatesEnum.ExecutedSuccessfully" />
    /// </param>
    /// <param name="message">The verbose user-friendly message. Default value is empty string.</param>
    public ContractExecutionResult(int code, string message = DefaultMessage)
    {
      Code = code;
      Message = message;
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="ContractExecutionResult" /> class with default
    ///   <see cref="ContractExecutionStatesEnum.ExecutedSuccessfully" /> result
    /// </summary>
    /// <param name="message">The verbose user-friendly message.</param>
    protected ContractExecutionResult(string message)
      : this(ContractExecutionStatesEnum.ExecutedSuccessfully, message)
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="ContractExecutionResult" /> class with default
    ///   <see cref="ContractExecutionStatesEnum.ExecutedSuccessfully" /> result and "success" message
    /// </summary>
    public ContractExecutionResult()
      : this(DefaultMessage)
    {
    }


    /// <summary>
    ///   Defines machine-readable code.
    /// </summary>
    /// <value>
    ///   Result code.
    /// </value>
    [JsonProperty(PropertyName = "Code", Required = Required.Always)]
    [Required]
    public int Code { get; protected set; }

    /// <summary>
    ///   Defines user-friendly message.
    /// </summary>
    /// <value>
    ///   The message string.
    /// </value>
    [JsonProperty(PropertyName = "Message", Required = Required.Always)]
    [Required]
    public string Message { get; protected set; }
  }

  /// <summary>
  ///   Defines standard return codes for a contract.
  /// </summary>
  public class ContractExecutionStatesEnum : GenericEnum<ContractExecutionStatesEnum, int>
  {
    /// <summary>
    /// The execution result offset to create dynamically add custom errors
    /// </summary>
    private const int executionResultOffset = 100;

    /// <summary>
    ///   Service request executed successfully
    /// </summary>
    public static readonly int ExecutedSuccessfully = 0;

    /// <summary>
    ///   Requested data was invalid or POSTed JSON was invalid
    /// </summary>
    public static readonly int IncorrectRequestedData = -1;

    /// <summary>
    ///   Supplied data didn't pass validation
    /// </summary>
    public static readonly int ValidationError = -2;

    /// <summary>
    ///   Internal processing error
    /// </summary>
    public static readonly int InternalProcessingError = -3;

    /// <summary>
    ///   Failed to get results
    /// </summary>
    public static readonly int FailedToGetResults = -4;

    /// <summary>
    ///   Failed to authorize for the project
    /// </summary>
    public static readonly int AuthError = -5;

    /// <summary>
    ///   Failed to authorize for the project
    /// </summary>
    public static readonly int PartialData = -6;

    /// <summary>
    /// Asset does not have a valid subscription for specified date
    /// </summary>
    public static readonly int NoSubscription = -7;

    /// <summary>
    /// Dynamically adds new error messages addwith offset.
    /// </summary>
    /// <param name="name">The name of error.</param>
    /// <param name="value">The value of code.</param>
    public void DynamicAddwithOffset(string name, int value)
    {
      DynamicAdd(name, value + executionResultOffset);
    }

    /// <summary>
    /// Gets the error numberwith offset.
    /// </summary>
    /// <param name="errorNum">The error number.</param>
    /// <returns></returns>
    public int GetErrorNumberwithOffset(int errorNum)
    {
      return errorNum + executionResultOffset;
    }

    /// <summary>
    /// Gets the frist available name of a error code taking into account 
    /// </summary>
    /// <param name="value">The code vale to get the name against.</param>
    /// <returns></returns>
    public string FirstNameWithOffset(int value)
    {
      return FirstNameWith(value + executionResultOffset);
    }
  }
}
