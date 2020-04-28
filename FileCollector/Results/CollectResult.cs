using System.Collections.Generic;
using System.Linq;
using FileCollector.Results.Abstract;

namespace FileCollector.Results
{
    /// <summary>
    /// The result of the directory collect operation. This class can not be inherited.
    /// </summary>
    public sealed class CollectResult : CollectResultBase
    {
        #region Constructors

        /// <summary>
        /// Initialize a new instance of the <see cref="CollectResult"/> class.
        /// </summary>
        public CollectResult()
        {
            FileCollectResults = new List<FileCollectResult>();
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="CollectResult"/> class.
        /// </summary>
        /// <param name="directoryPath">The path of the directory.</param>
        public CollectResult(string directoryPath)
        {
            Path = directoryPath;
            FileCollectResults = new List<FileCollectResult>();
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets or sets the list of the file collect results.
        /// </summary>
        public List<FileCollectResult> FileCollectResults { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the directory has subdirectories.
        /// </summary>
        public bool HasSubDirectories { get; set; }

        /// <summary>
        /// Gets the processed file count of the directory.
        /// </summary>
        public int ProcessedFileCount => FileCollectResults.Count;

        /// <summary>
        /// Gets the succeeded file operation count of the directory.
        /// </summary>
        public int SucceededCount => FileCollectResults.Count(fcr => fcr.IsSucceeded);
        
        #endregion
    }
}
