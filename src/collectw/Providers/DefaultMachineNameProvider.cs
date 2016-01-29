using System;
using CollectW.Services;

namespace CollectW.Providers
{
    public class DefaultMachineNameProvider : IMachineNameProvider
    {
        public string GetMachineName()
        {
            return Environment.MachineName;
        }
    }
}