﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WebApiModels.ResultHandling
{
    /// <summary>
    ///   Represents general (minimal) reponse generated by a sevice. All other responses should be derived from this class.
    /// </summary>
    public class ContractExecutionResult
    {
        /// <summary>
        /// The result of the request. True for success and false for failure.
        /// </summary>
        [JsonProperty(PropertyName = "Result", Required = Required.Always)]
        public bool Result { get; set; }

        public const string DefaultMessage = "success";

        /// <summary>
        ///   Initializes a new instance of the <see cref="ContractExecutionResult" /> class.
        /// </summary>
        /// <param name="code">
        ///   The resulting code. Default value is <see cref="ContractExecutionStatesEnum.ExecutedSuccessfully" />
        /// </param>
        /// <param name="message">The verbose user-friendly message. Default value is empty string.</param>
        public ContractExecutionResult(ContractExecutionStatesEnum code, string message = DefaultMessage,
            bool result = true)
        {
            Code = code;
            Message = message;
            Result = result;
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
        public ContractExecutionStatesEnum Code { get; set; }

        /// <summary>
        ///   Defines user-friendly message.
        /// </summary>
        /// <value>
        ///   The message string.
        /// </value>
        [JsonProperty(PropertyName = "Message", Required = Required.Always)]
        public string Message { get; set; }
    }

    /// <summary>
    ///   Defines standard return codes for a contract.
    /// </summary>
    public enum ContractExecutionStatesEnum
    {
        /// <summary>
        ///   Service request executed successfully
        /// </summary>
        ExecutedSuccessfully = 0,

        /// <summary>
        ///   Requested data was invalid or POSTed JSON was invalid
        /// </summary>
        IncorrectRequestedData = -1,

        /// <summary>
        ///   Supplied data didn't pass validation
        /// </summary>
        ValidationError = -2,

        /// <summary>
        ///   Internal processing error
        /// </summary>
        InternalProcessingError = -3,

        /// <summary>
        ///   Failed to get results
        /// </summary>
        FailedToGetResults = -4,

        /// <summary>
        ///   Failed to authorize for the project
        /// </summary>
        AuthError = -5,

        /// <summary>
        ///   Failed to authorize for the project
        /// </summary>
        PartialData = -6,

        /// <summary>
        /// Asset does not have a valid subscription for specified date
        /// </summary>
        NoSubscription = -7
    }
}