using System;
using System.Threading;
using System.Threading.Tasks;
using CollectW.Model;
using CollectW.Providers;
using CollectW.Services;
using CollectW.Sinks;
using CollectW.Sinks.StatsD;
using CollectW.Suppliers;
using CollectW.Tests.Impl;
using Moq;
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
                    CategoryName = "Processor",
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
                    CategoryName = "Processor",
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

        [Fact]
        public void SimpleSetupAsALib()
        {
            var collector =
                new Collector(
                    new DefaultSupplier(new[]
                    {
                        new CounterDefinition()
                        {
                            CategoryName = "Processor",
                            InstanceName = "_Total",
                            CounterName = "% Processor Time",
                            CollectInterval = 5000
                        }
                    }), new[] {new ConsoleSink()});
            collector.Start();
            //Do your stuff
            Thread.Sleep(400);
            //at some point, when no more performance counter info is needed
            collector.Stop();
            collector.Dispose();
        }
        [Fact]
        public void SendingToStatsD()
        {
            var collector =
                new Collector(
                    new DefaultSupplier(new[]
                    {
                        new CounterDefinition()
                        {
                            CategoryName = "Processor",
                            InstanceName = "_Total",
                            CounterName = "% Processor Time",
                            CollectInterval = 5000
                        }
                    }),
                    new[]
                    {
                            new StatsDSink(
                                new RegexResolver().Add(".*", StatsDTypes.Gauge), 
                                new Uri("UDP://localhost:8125")
                                )
                    });
            collector.Start();
            //Do your stuff
            Thread.Sleep(400);
            //at some point, when no more performance counter info is needed
            collector.Stop();
            collector.Dispose();
        }

        [Fact]
        public void NonDefaultMachineNameProvider()
        {
            var machineName = Guid.NewGuid().ToString();
            var machineNameProviderMock = new Mock<IMachineNameProvider>();
            machineNameProviderMock.Setup(x => x.GetMachineName()).Returns(machineName);

            var sinkMock = new Mock<ISendInfo>();
            sinkMock.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<float>())).Returns(Task.FromResult(true));

            var collector =
               new Collector(
                   new DefaultSupplier(new[]
                    {
                        new CounterDefinition()
                        {
                            CategoryName = "Processor",
                            InstanceName = "_Total",
                            CounterName = "% Processor Time",
                            CollectInterval = 5000
                        }
                    }), new[]  { sinkMock.Object }, machineNameProviderMock.Object );
            collector.Start();
            //Do your stuff
            Thread.Sleep(400);
            //at some point, when no more performance counter info is needed
            collector.Stop();
            collector.Dispose();
            
            sinkMock.Verify(x => x.Send(It.Is<string>(c => c.StartsWith(machineName)), It.IsAny<float>()), Times.AtLeastOnce);
        }

        [Fact]
        public void NonDefaultCounterIdentifierGenerator()
        {
            var identifier = Guid.NewGuid().ToString();
            var counterIdentifierGeneratorMock = new Mock<ICounterIdentifierGenerator>();
            counterIdentifierGeneratorMock.Setup(x => x.Generate(It.IsAny<IMachineNameProvider>(), It.IsAny<CounterDefinition>())).Returns(identifier);

            var sinkMock = new Mock<ISendInfo>();
            sinkMock.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<float>())).Returns(Task.FromResult(true));

            var collector =
               new Collector(
                   new DefaultSupplier(new[]
                    {
                        new CounterDefinition()
                        {
                            CategoryName = "Processor",
                            InstanceName = "_Total",
                            CounterName = "% Processor Time",
                            CollectInterval = 5000
                        }
                    }), new[] { sinkMock.Object },new DefaultMachineNameProvider(), counterIdentifierGeneratorMock.Object);
            collector.Start();
            //Do your stuff
            Thread.Sleep(400);
            //at some point, when no more performance counter info is needed
            collector.Stop();
            collector.Dispose();

            sinkMock.Verify(x => x.Send(identifier, It.IsAny<float>()), Times.AtLeastOnce);
        }
    }
}