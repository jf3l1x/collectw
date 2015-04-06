using System.Threading.Tasks;

namespace CollectW.Services
{
    public interface ISendInfo
    {
        Task Send(string counter, float value);
    }
}