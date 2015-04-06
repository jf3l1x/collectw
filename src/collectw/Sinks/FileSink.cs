using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CollectW.Services;

namespace CollectW.Sinks
{
    public class FileSink : ISendInfo, IDisposable
    {
        private readonly TextWriter _writer;
        private readonly Timer _timer;
        
        public FileSink(string path, int flushInterval=1000 )
        {
            _timer = new Timer(Flush, null, flushInterval, flushInterval);
            _writer = TextWriter.Synchronized(new StreamWriter(path, true));
        }

        private void Flush(object state)
        {
            _writer.FlushAsync();
        }

        public void Dispose()
        {
            _writer.Dispose();
            _timer.Dispose();
        }

        public Task Send(string counter, float value)
        {
            return Task.Run(() => _writer.WriteLine("[{0}] - {1} : {2} ", DateTimeOffset.UtcNow, counter, value));
        }
    }
}