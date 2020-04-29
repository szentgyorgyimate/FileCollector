namespace FileCollector.Tests.Helpers
{
    /// <summary>
    /// Provides fake directory and file paths for unit testing.
    /// </summary>
    public static class Paths
    {
        internal const string FakeSourcePath = "c:\\source";
        internal const string FakeDestinationPath = "c:\\destination";
        internal const string DuplicateFileName = "duplicate.txt";

        #region Properties

        /// <summary>
        /// Gets the c:\source\duplicate.txt file path.
        /// </summary>
        internal static string FakeDuplicateFilePath => $"{FakeSourcePath}\\{DuplicateFileName}";

        /// <summary>
        /// Gets c:\source\secondfile.jpg file path.
        /// </summary>
        internal static string FakeSecondFilePath => $"{FakeSourcePath}\\secondfile.jpg";

        /// <summary>
        /// Gets c:\source\firstsubdir directory path.
        /// </summary>
        internal static string FakeFirstSubDirectoryPath => $"{FakeSourcePath}\\firstsubdir";

        /// <summary>
        /// Gets c:\source\firstsubdir\duplicate.txt file path.
        /// </summary>
        internal static string FakeDuplicatedSubFilePath => $"{FakeFirstSubDirectoryPath}\\{DuplicateFileName}";

        /// <summary>
        /// Gets c:\source\secondsubdir directory path.
        /// </summary>
        internal static string FakeSecondSubDirectoryPath => $"{FakeSourcePath}\\secondsubdir";

        /// <summary>
        /// Gets c:\source\firstsubdir\firstsubsubdir directory path.
        /// </summary>
        internal static string FakeFirstSubSubDirectoryPath => $"{FakeFirstSubDirectoryPath}\\firstsubsubdir";

        /// <summary>
        /// Gets c:\source\firstsubdir\firstsubsubdir\duplicate.txt file path.
        /// </summary>
        internal static string FakeFirstSubSubDuplicatedFilePath => $"{FakeFirstSubSubDirectoryPath}\\{DuplicateFileName}";

        #endregion
    }
}
