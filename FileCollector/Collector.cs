using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using FileCollector.Enums;
using FileCollector.Interfaces;
using FileCollector.Results;

namespace FileCollector
{
    /// <summary>
    /// Provides properties and instance methods for collect all files from the given root directory into one specific directroy. This class can not be inherited.
    /// </summary>
    public sealed class Collector : IFileCollector
    {
        #region Constants

        private const bool OverwriteDefault = false;

        #endregion

        #region Fields

        private readonly IFileSystem _fileSystem;
        private Dictionary<string, int> _processedFileNames;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the source path of files and folders.
        /// </summary>
        public string SourceRootPath { get; set; }

        /// <summary>
        /// Gets or sets the path of the folder where the files will be moved.
        /// </summary>
        public string DestinationDirectoryPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether overwrite the already existing files in the destination directory.
        /// </summary>
        public bool Overwrite { get; set; }

        /// <summary>
        /// Gets or sets the type of the collect operation. It's value is Copy by default.
        /// </summary>
        public CollectOperation CollectOperation { get; set; }

        /// <summary>
        /// Gets or sets the list of file extensions to ignore when moving files.
        /// </summary>
        public List<string> ExtensionsToIgnore { get; set; }

        #endregion

        #region CTORs

        /// <summary>
        /// Initializes a new instance of the <see cref="Collector"/> class.
        /// </summary>
        /// <param name="sourceRootPath">The source path of files and folders.</param>
        /// <param name="destinationDirectoryPath">The path of the folder where the files will be moved.</param>
        public Collector(string sourceRootPath, string destinationDirectoryPath)
            : this(sourceRootPath, destinationDirectoryPath, OverwriteDefault, CollectOperation.Copy, new List<string>(), new FileSystem())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Collector"/> class.
        /// </summary>
        /// <param name="sourceRootPath">The source path of files and folders.</param>
        /// <param name="destinationDirectoryPath">The path of the folder where the files will be moved.</param>
        /// <param name="fileSystem">The file system used.</param>
        public Collector(string sourceRootPath, string destinationDirectoryPath, IFileSystem fileSystem)
            : this(sourceRootPath, destinationDirectoryPath, OverwriteDefault, CollectOperation.Copy, new List<string>(), fileSystem)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Collector"/> class.
        /// </summary>
        /// <param name="sourceRootPath">The source path of files and folders.</param>
        /// <param name="destinationDirectoryPath">The path of the folder where the files will be moved.</param>
        /// <param name="overwrite">true to overwrite the destination files if they already exist; false otherwise.</param>
        public Collector(string sourceRootPath, string destinationDirectoryPath, bool overwrite)
            : this(sourceRootPath, destinationDirectoryPath, overwrite, CollectOperation.Copy, new List<string>(), new FileSystem())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Collector"/> class.
        /// </summary>
        /// <param name="sourceRootPath">The source path of files and folders.</param>
        /// <param name="destinationDirectoryPath">The path of the folder where the files will be moved.</param>
        /// <param name="overwrite">true to overwrite the destination files if they already exist; false otherwise.</param>
        /// <param name="fileSystem">The file system used.</param>
        public Collector(string sourceRootPath, string destinationDirectoryPath, bool overwrite, IFileSystem fileSystem)
            : this(sourceRootPath, destinationDirectoryPath, overwrite, CollectOperation.Copy, new List<string>(), fileSystem)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Collector"/> class.
        /// </summary>
        /// <param name="sourceRootPath">The source path of files and folders.</param>
        /// <param name="destinationDirectoryPath">The path of the folder where the files will be moved.</param>
        /// <param name="overwrite">true to overwrite the destination files if they already exist; false otherwise.</param>
        /// <param name="collectOperation">The collect operation.</param>
        public Collector(string sourceRootPath, string destinationDirectoryPath, bool overwrite, CollectOperation collectOperation)
            : this(sourceRootPath, destinationDirectoryPath, overwrite, collectOperation, new List<string>(), new FileSystem())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Collector"/> class.
        /// </summary>
        /// <param name="sourceRootPath">The source path of files and folders.</param>
        /// <param name="destinationDirectoryPath">The path of the folder where the files will be moved.</param>
        /// <param name="overwrite">true to overwrite the destination files if they already exist; false otherwise.</param>
        /// <param name="collectOperation">The collect operation.</param>
        /// <param name="fileSystem">The file system used.</param>
        public Collector(string sourceRootPath, string destinationDirectoryPath, bool overwrite, CollectOperation collectOperation, IFileSystem fileSystem)
            : this(sourceRootPath, destinationDirectoryPath, overwrite, collectOperation, new List<string>(), fileSystem)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Collector"/> class.
        /// </summary>
        /// <param name="sourceRootPath">The source path of files and folders.</param>
        /// <param name="destinationDirectoryPath">The path of the folder where the files will be moved.</param>
        /// <param name="overwrite">true to overwrite the destination files if they already exist; false otherwise.</param>
        /// <param name="collectOperation">The collect operation.</param>
        /// <param name="extensionsToIgnore">The list of file extensions to ignore when moving files.</param>
        public Collector(string sourceRootPath, string destinationDirectoryPath, bool overwrite, CollectOperation collectOperation, List<string> extensionsToIgnore)
            : this(sourceRootPath, destinationDirectoryPath, overwrite, collectOperation, extensionsToIgnore, new FileSystem())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Collector"/> class.
        /// </summary>
        /// <param name="sourceRootPath">The source path of files and folders.</param>
        /// <param name="destinationDirectoryPath">The path of the folder where the files will be moved.</param>
        /// <param name="overwrite">true to overwrite the destination files if they already exist; false otherwise.</param>
        /// <param name="collectOperation">The collect operation.</param>
        /// <param name="extensionsToIgnore">The list of file extensions to ignore when moving files.</param>
        /// <param name="fileSystem">The file system used.</param>
        public Collector(string sourceRootPath, string destinationDirectoryPath, bool overwrite, CollectOperation collectOperation, List<string> extensionsToIgnore,
            IFileSystem fileSystem) => (SourceRootPath, DestinationDirectoryPath, Overwrite, CollectOperation, ExtensionsToIgnore, _fileSystem)
                = (sourceRootPath, destinationDirectoryPath, overwrite, collectOperation, extensionsToIgnore, fileSystem);

        #endregion

        #region Methods

        /// <summary>
        /// Collect files from folders and subfolders of <see cref="SourceRootPath"/> to <see cref="DestinationDirectoryPath"/>
        /// </summary>
        /// <exception cref="IOException">Thrown when the destination directory specified by the <see cref="DestinationDirectoryPath"/> is a file or the network name is not known.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the caller of create <see cref="DestinationDirectoryPath"/> does not have the required permission</exception>
        /// <exception cref="ArgumentException">Thrown when the <see cref="DestinationDirectoryPath"/> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for invalid characters by using the GetInvalidPathChars() method or path is prefixed with, or contains, only a colon character(:).</exception>
        /// <returns>The list of results of the dictionary collect operations.</returns>
        public List<CollectResult> CollectFiles()
        {
            CheckSourceRootPath();
            CheckPath(DestinationDirectoryPath, nameof(DestinationDirectoryPath));

            _processedFileNames = new Dictionary<string, int>();
            var resultList = new List<CollectResult>();

            if (!_fileSystem.Directory.Exists(DestinationDirectoryPath))
            {
                _fileSystem.Directory.CreateDirectory(DestinationDirectoryPath);
            }

            CollectFiles(SourceRootPath, resultList);

            return resultList.OrderBy(r => r.Path).ToList();
        }

        private void CheckSourceRootPath()
        {
            CheckPath(SourceRootPath, nameof(SourceRootPath));

            if (!_fileSystem.Directory.Exists(SourceRootPath))
            {
                throw new DirectoryNotFoundException($"The directory {SourceRootPath} is not found.");
            }
        }

        private void CheckPath(string path, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                if (path == null)
                {
                    throw new ArgumentNullException($"{propertyName} is null.");
                }
                else
                {
                    throw new ArgumentException($"{propertyName} is empty or consists only of white-space characters.");
                }
            }

            foreach (char invalidPathChar in Path.GetInvalidPathChars())
            {
                if (path.Contains(invalidPathChar))
                {
                    throw new ArgumentException($"{propertyName} contains invalid character(s).");
                }
            }
        }

        private void CollectFiles(string directoryPath, List<CollectResult> resultList)
        {
            var collectResult = new CollectResult(directoryPath);

            MoveFiles(directoryPath, collectResult);

            if (collectResult.IsSucceeded)
            {
                string[] subdirectoryPaths = _fileSystem.Directory.GetDirectories(directoryPath);

                collectResult.HasSubDirectories = subdirectoryPaths.Any();

                foreach (string subdirectoryPath in subdirectoryPaths)
                {
                    CollectFiles(subdirectoryPath, resultList);
                }
            }

            resultList.Add(collectResult);
        }

        private void MoveFiles(string path, CollectResult collectResult)
        {
            string[] filePaths = GetFilePaths(path, collectResult);

            foreach (string filePath in filePaths)
            {
                var fileIndex = 0;
                string fileName = Path.GetFileName(filePath);
                string lowerCaseFileName = fileName.ToLower();

                var result = new FileCollectResult(filePath);

                if (_processedFileNames.ContainsKey(lowerCaseFileName.ToLower()))
                {
                    _processedFileNames[lowerCaseFileName]++;

                    fileName = $"{Path.GetFileNameWithoutExtension(fileName)}_({_processedFileNames[lowerCaseFileName]}){Path.GetExtension(fileName)}";

                    result.IsRenamed = true;
                }
                else
                {
                    _processedFileNames.Add(lowerCaseFileName, fileIndex);
                }

                string destinationFilePath = Path.Combine(DestinationDirectoryPath, fileName);
                result.NewFilePath = destinationFilePath;

                try
                {
                    switch (CollectOperation)
                    {
                        case CollectOperation.Copy:
                            _fileSystem.File.Copy(filePath, destinationFilePath, Overwrite);
                            break;
                        case CollectOperation.Move:
                            if (Overwrite && _fileSystem.File.Exists(destinationFilePath))
                            {
                                _fileSystem.File.Delete(destinationFilePath);
                            }

                            _fileSystem.File.Move(filePath, destinationFilePath);
                            break;
                    }

                    result.IsSucceeded = true;
                }
                catch (UnauthorizedAccessException exception)
                {
                    result.ErrorType = CollectErrorType.Unauthorized;
                    result.ErrorMessage = exception.Message;
                }
                catch (PathTooLongException exception)
                {
                    result.ErrorType = CollectErrorType.PathTooLong;
                    result.ErrorMessage = exception.Message;
                }
                catch (FileNotFoundException exception)
                {
                    result.ErrorType = CollectErrorType.NotFound;
                    result.ErrorMessage = exception.Message;
                }
                catch (NotSupportedException exception)
                {
                    result.ErrorType = CollectErrorType.NotSupported;
                    result.ErrorMessage = exception.Message;
                }
                catch (IOException exception)
                {
                    result.ErrorType = CollectErrorType.IO;
                    result.ErrorMessage = exception.Message;
                }
                catch (Exception exception)
                {
                    result.ErrorType = CollectErrorType.Unknown;
                    result.ErrorMessage = exception.Message;
                }
                finally
                {
                    collectResult.FileCollectResults.Add(result);
                }
            }
        }

        private string[] GetFilePaths(string directoryPath, CollectResult collectResult)
        {
            var filePaths = new string[0];

            try
            {
                filePaths = _fileSystem.Directory.GetFiles(directoryPath);

                if (ExtensionsToIgnore.Count > 0)
                {
                    filePaths = filePaths.Where(fp => !ExtensionsToIgnore.Any(eti => fp.EndsWith(eti, StringComparison.OrdinalIgnoreCase))).ToArray();
                }
            }
            catch (UnauthorizedAccessException exception)
            {
                collectResult.ErrorType = CollectErrorType.Unauthorized;
                collectResult.ErrorMessage = exception.Message;
            }
            catch (PathTooLongException exception)
            {
                collectResult.ErrorType = CollectErrorType.PathTooLong;
                collectResult.ErrorMessage = exception.Message;
            }
            catch (DirectoryNotFoundException exception)
            {
                collectResult.ErrorType = CollectErrorType.NotFound;
                collectResult.ErrorMessage = exception.Message;
            }
            catch (IOException exception)
            {
                collectResult.ErrorType = CollectErrorType.IO;
                collectResult.ErrorMessage = exception.Message;
            }
            catch (Exception exception)
            {
                collectResult.ErrorType = CollectErrorType.Unknown;
                collectResult.ErrorMessage = exception.Message;
            }
            finally
            {
                if (collectResult.ErrorType == CollectErrorType.None)
                {
                    collectResult.IsSucceeded = true;
                }
            }

            return filePaths;
        }

        #endregion
    }
}