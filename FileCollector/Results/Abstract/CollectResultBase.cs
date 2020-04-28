using FileCollector.Enums;

namespace FileCollector.Results.Abstract
{
    /// <summary>
    /// The collect operation base class.
    /// </summary>
    public abstract class CollectResultBase
    {
        /// <summary>
        /// Gets or sets the original path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the collect operation is succeeded.
        /// </summary>
        public bool IsSucceeded { get; set; }

        /// <summary>
        /// Gets or sets the error type which occurred during the collect operation.
        /// </summary>
        public CollectErrorType ErrorType { get; set; }

        /// <summary>
        /// Gets or sets the message of the error.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
