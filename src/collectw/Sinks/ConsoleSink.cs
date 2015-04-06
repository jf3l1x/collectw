using System;
using System.Threading.Tasks;
using CollectW.Services;

namespace CollectW.Sinks
{
    public class ConsoleSink : ISendInfo
    {
        public Task Send(string counter, float value)
        {
            Console.WriteLine("[{0:O}] - {1} : {2} ", DateTimeOffset.UtcNow, counter, value);
            return Task.FromResult(0);
        }

        public void Configure(dynamic configuration)
        {
            
        }
    }
}