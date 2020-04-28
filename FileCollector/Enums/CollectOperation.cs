namespace FileCollector.Enums
{
    /// <summary>
    /// Specifies the collect operation type of the <see cref="Collector">. Copy by default.
    /// </summary>
    public enum CollectOperation
    {
        /// <summary>
        /// Copy files.
        /// </summary>
        Copy = 0,

        /// <summary>
        /// Move files.
        /// </summary>
        Move
    }
}
