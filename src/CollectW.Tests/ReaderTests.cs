using System;
using System.Threading;
using CollectW.Model;
using Xunit;

namespace CollectW.Tests
{
    public class ReaderTests : BaseSetup
    {
        [Fact]
        public void ThrowsArgumentExceptionIfDefinitionIsNotValid()
        {
            Assert.Throws(typeof (ArgumentException), () => { var reader = new Reader(new CounterDefinition()); });
        }

        [Fact]
        public void ReadACounter()
        {
            var reader =
                new Reader(new CounterDefinition
                {
                    CategorieName = "Processor",
                    CounterName = "% Processor Time",
                    InstanceName = "_Total"
                });
            reader.Read(new[] {Sink}).Wait();
            Assert.Equal(1, SentValues.Count);
            Assert.Equal(0, SentValues[0]);
            Thread.Sleep(200);
            reader.Read(new[] {Sink}).Wait();
            Assert.Equal(2, SentValues.Count);
            Assert.True(SentValues[1] > 0);
        }

        [Fact]
        public void ReadOneCountersUsingAnInterval()
        {
            var interval = new Interval(TimeSpan.FromMilliseconds(50), new[] {Sink});
            interval.AddDefinition(new CounterDefinition
            {
                CategorieName = "Processor",
                CounterName = "% Processor Time",
                InstanceName = "_Total",
                CollectInterval = 50
            });
            interval.Start();
            Thread.Sleep(400);
            interval.Stop();
            Assert.True(SentValues.Count >= 5);
        }

        [Fact]
        public void ReadingMultipleCountersUsingAnInterval()
        {
            using (var interval = new Interval(TimeSpan.FromMilliseconds(50), new[] {Sink}))
            {
                interval.AddDefinition(new CounterDefinition
                {
                    CategorieName = "Processor",
                    CounterName = "% Processor Time",
                    InstanceName = "_Total",
                    CollectInterval = 50
                });
                interval.AddDefinition(new CounterDefinition
                {
                    CategorieName = "Processor",
                    CounterName = "% Processor Time",
                    InstanceName = "0",
                    CollectInterval = 50
                });
                interval.Start();
                Thread.Sleep(400);
                interval.Stop();
                Assert.True(SentValues.Count >= 10);
            }
        }
    }
}