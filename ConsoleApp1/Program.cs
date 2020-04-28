using System;
using FileCollector;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileCollector = new FileCollector.Collector("d:\\test_source",
                                        "d:\\test_destination",
                                        false,
                                        FileCollector.Enums.CollectOperation.Move);

            var result = fileCollector.CollectFiles();
        }
    }
}
