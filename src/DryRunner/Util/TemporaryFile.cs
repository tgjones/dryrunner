using System;
using System.IO;

namespace DryRunner.Util
{
    internal class TemporaryFile : IDisposable
    {
        public string Path { get; private set; }

        public TemporaryFile()
        {
            Path = System.IO.Path.GetTempFileName();
        }

        public string GetContents()
        {
            return File.ReadAllText(Path);
        }

        public void Dispose()
        {
            File.Delete(Path);
        }
    }
}