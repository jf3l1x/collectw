using System;
using System.Threading;
using System.Threading.Tasks;
using CollectW.CounterDefinitionSuppliers;
using CollectW.Services;
using CollectW.Sinks;

namespace CollectW.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var service = new Collector(new JsonConfigFile(), new[] {new ConsoleSink()});
            service.Start();
            System.Console.WriteLine("reading values...");
            while (true)
            {
                Thread.Sleep(500);
            }
        }
    }

    
}