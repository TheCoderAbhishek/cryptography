namespace service.Core.Entities.Utility
{
    /// <summary>
    /// The utility class for getting Base Response
    /// </summary>
    public class BaseResponse
    {
        /// <summary>
        /// Transaction identifier
        /// </summary>
        public string? Txn { get; set; }

        /// <summary>
        /// Status of the response
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Success message
        /// </summary>
        public string? SuccessMessage { get; set; }

        /// <summary>
        /// Error code
        /// </summary>
        public string? ErrorCode { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}
