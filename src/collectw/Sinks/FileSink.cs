using System;
using System.IO;
using System.Threading.Tasks;
using CollectW.Services;

namespace CollectW.Sinks
{
    public class FileSink : ISendInfo, IDisposable
    {
        private readonly TextWriter _writer;
        private object _lock = new object();

        public FileSink(string path)
        {
            _writer = TextWriter.Synchronized(new StreamWriter(path, true));
        }

        public void Dispose()
        {
            _writer.Dispose();
        }

        public Task Send(string counter, float value)
        {
            return Task.Run(() => _writer.WriteLine("[{0}] - {1} : {2} ", DateTimeOffset.UtcNow, counter, value));
        }
    }
}