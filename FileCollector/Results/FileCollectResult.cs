using FileCollector.Results.Abstract;

namespace FileCollector.Results
{
    /// <summary>
    /// The result of the file collect operation. This class can not be inherited.
    /// </summary>
    public sealed class FileCollectResult : CollectResultBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="FileCollectResult"/> class.
        /// </summary>
        public FileCollectResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FileCollectResult"/> class.
        /// </summary>
        /// <param name="filePath">The original path of the file.</param>
        public FileCollectResult(string filePath)
        {
            Path = filePath;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the new path of the file.
        /// </summary>
        public string NewFilePath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the file has been renamed.
        /// </summary>
        public bool IsRenamed { get; set; }

        #endregion
    }
}