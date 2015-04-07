using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CollectW.Services;

namespace CollectW.Sinks
{
    public class FileSink : ISendInfo, IDisposable
    {
        private Timer _timer;
        private TextWriter _writer;

        public FileSink()
        {
            
        }
        public FileSink(string path,int flushInterval=1000)
        {
            _timer = new Timer(Flush, null, flushInterval, flushInterval);
            _writer = TextWriter.Synchronized(new StreamWriter(path, true));
        }

        public void Dispose()
        {
            DisposeDependencies();
        }

        private void DisposeDependencies()
        {
            if (_writer != null)
            {
                _writer.Dispose();
            }
            if (_timer != null)
            {
                _timer.Dispose();
            }
        }

        public Task Send(string counter, float value)
        {
            if (_writer != null)
            {
                return _writer.WriteLineAsync(string.Format("[{0:O}] - {1} : {2} ", DateTimeOffset.UtcNow, counter, value));
            }
            return Task.FromResult(0);
        }

        public void Configure(dynamic configuration)
        {
            Configure(configuration.Path.ToString(), (int)configuration.FlushInterval);
        }

        private void Flush(object state)
        {
            if (_writer != null)
            {
                _writer.FlushAsync();
            }
        }

        private void Configure(string path, int flushInterval)
        {
            DisposeDependencies();
            _timer = new Timer(Flush, null, flushInterval, flushInterval);
            _writer = TextWriter.Synchronized(new StreamWriter(path, true));
        }
    }
}