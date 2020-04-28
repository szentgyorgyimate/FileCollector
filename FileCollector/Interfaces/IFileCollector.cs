using System.Collections.Generic;
using FileCollector.Results;

namespace FileCollector.Interfaces
{
    public interface IFileCollector
    {
        List<CollectResult> CollectFiles();
    }
}
