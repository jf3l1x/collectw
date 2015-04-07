using System.Threading;
using CollectW.Model;
using CollectW.Tests.Impl;
using Xunit;

namespace CollectW.Tests
{
    public class CollectorTests : BaseSetup
    {
        [Fact]
        public void CreateACollectorManually()
        {
            using (var collector = new Collector(Supplier, new[] {Sink}))
            {
            }
        }

        [Fact]
        public void ReadValuesUsingTheCollector()
        {
            using (var collector = new Collector(Supplier, new[] { Sink }))
            {
                collector.Start();
                Thread.Sleep(400);
                collector.Stop();
                Assert.True(SentValues.Count>5);
            }
        }

        [Fact]
        public void IfTheSupplierChangesTheCollectorReconfigure()
        {
            var supplier = new UpdateableSupplier();
            using (var collector = new Collector(supplier, new[] { Sink }))
            {
                collector.Start();
                Thread.Sleep(400);
                Assert.Equal(0,SentValues.Count);
                supplier.AddCounter(new CounterDefinition()
                {
                    CategorieName = "Processor",
                    CounterName = "% Processor Time",
                    InstanceName = "_Total",
                    CollectInterval = 50
                });
                Thread.Sleep(400);
                collector.Stop();
                Assert.True(SentValues.Count > 5);
            }
        }

        [Fact]
        public void ExpandDefinitions()
        {
            var supplier = new UpdateableSupplier();
            using (var collector = new Collector(supplier, new[] { Sink }))
            {
                supplier.AddCounter(new CounterDefinition()
                {
                    CategorieName = "Processor",
                    CounterName = "/.*/",
                    InstanceName = "/.*/",
                    CollectInterval = 50
                });

                collector.Start();
                Thread.Sleep(1000);
                collector.Stop();
                Assert.True(Counters.Count > 10,string.Format("expected more than {0} but was {1}",10,Counters.Count));
            }
        }
        
    }
}