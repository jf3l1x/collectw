using System.ServiceProcess;

namespace CollectW.Service
{
    internal static class Program
    {
        private static void Main()
        {
            var servicesToRun = new ServiceBase[]
            {
                new Daemon()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}