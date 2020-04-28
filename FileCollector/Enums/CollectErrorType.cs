namespace FileCollector.Enums
{
    /// <summary>
    /// Specifies the error type which occurred during the collect operation of the <see cref="Collector"/>. None by default.
    /// </summary>
    public enum CollectErrorType
    {
        /// <summary>
        /// No error occured during the collect operation.
        /// </summary>
        None = 0,

        /// <summary>
        /// UnauthorizedAccessException has been thrown during the collect operation.
        /// </summary>
        Unauthorized,

        /// <summary>
        /// NotSupportedException has been thrown during the collect operation.
        /// </summary>
        NotSupported,

        /// <summary>
        /// DirectoryNotFoundException or FileNotFoundException has been thrown during the collect operation.
        /// </summary>
        NotFound,

        /// <summary>
        /// PathTooLongException has been thrown during the collect operation.
        /// </summary>
        PathTooLong,

        /// <summary>
        /// IOException has been thrown during the collect operation.
        /// </summary>
        IO,

        /// <summary>
        /// Other exception has been thrown during the collect operation.
        /// </summary>
        Unknown
    }
}