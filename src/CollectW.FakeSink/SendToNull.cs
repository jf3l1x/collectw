using System.Threading.Tasks;
using CollectW.Services;

namespace CollectW.FakeSink
{
    public class SendToNull : ISendInfo
    {
        public Task Send(string counter, float value)
        {
            return Task.FromResult(0);
        }

        public void Configure(dynamic configuration)
        {
        }
    }
}