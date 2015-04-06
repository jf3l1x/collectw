using System.Threading;
using CollectW.Config;

namespace CollectW.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //var service = new Collector(new DemoSupplier(), 
            //    new ISendInfo[]
            //    {
            //        new ConsoleSink(),
            //        new FileSink(@"c:\temp\perfcount.txt"),
            //        new StatsDSink(new RegexResolver().Add(".*", StatsDTypes.Gauge), new Uri("udp://elk.aidev.biz:8125"))
            //    });
            Collector service = new Configuration().CreateCollector();
            service.Start();
            System.Console.WriteLine("reading values...");
            while (true)
            {
                Thread.Sleep(500);
            }
        }
    }
}