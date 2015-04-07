using System;
using System.Threading.Tasks;
using CollectW.Services;

namespace CollectW.FakeSink
{
    public class SendToNull : ISendInfo,IDisposable
    {
        public Task Send(string counter, float value)
        {
            return Task.FromResult(0);
        }

        public void Configure(dynamic configuration)
        {
        }

        public void Dispose()
        {
            Disposed = true;
        }

        public bool Disposed { get; set; }
    }
}