using System.Runtime.Serialization;

namespace service.Infrastructure.Dependency
{
    /// <summary>
    /// Represents errors that occur during application execution.
    /// This exception includes additional properties for error code and transaction ID.
    /// </summary>
    [Serializable]
    public class CustomException : Exception
    {
        /// <summary>
        /// Gets or sets the error code associated with the exception.
        /// </summary>
        public string? ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the transaction ID associated with the exception.
        /// </summary>
        public string? TransactionId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomException"/> class.
        /// </summary>
        public CustomException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CustomException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomException"/> class
        /// with a specified error message and a reference to the inner exception
        /// that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public CustomException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomException"/> class
        /// with a specified error message, a reference to the inner exception,
        /// an error code, and a transaction ID.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        /// <param name="errorCode">The error code associated with the exception.</param>
        /// <param name="transactionId">The transaction ID associated with the exception.</param>
        public CustomException(string message, Exception inner, string? errorCode, string? transactionId)
            : base(message, inner)
        {
            ErrorCode = errorCode;
            TransactionId = transactionId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomException"/> class
        /// with serialized data.
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the serialized object data.</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination.</param>
        protected CustomException(SerializationInfo info, StreamingContext context)
            #pragma warning disable SYSLIB0051
            : base(info, context)
            #pragma warning restore SYSLIB0051
        {
            if (info != null)
            {
                ErrorCode = info.GetString("ErrorCode");
                TransactionId = info.GetString("TransactionId");
            }
        }

        /// <summary>
        /// Sets the SerializationInfo with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the serialized object data.</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            #pragma warning disable SYSLIB0051
            base.GetObjectData(info, context);
            #pragma warning restore SYSLIB0051
            if (info != null)
            {
                info.AddValue("ErrorCode", ErrorCode);
                info.AddValue("TransactionId", TransactionId);
            }
        }
    }
}
