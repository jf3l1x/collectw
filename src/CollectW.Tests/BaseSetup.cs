using System.Collections.Generic;
using System.Threading.Tasks;
using CollectW.Model;
using CollectW.Services;
using Moq;

namespace CollectW.Tests
{
    public class BaseSetup
    {
        protected readonly List<float> SentValues = new List<float>();
        protected readonly HashSet<string> Counters=new HashSet<string>();
        protected readonly ISendInfo Sink;
        protected readonly ISupplyCounterDefinitions Supplier;

        public BaseSetup()
        {
            var sinkMock = new Mock<ISendInfo>();
            sinkMock.Setup(s => s.Send(It.IsAny<string>(), It.IsAny<float>())).Returns<string, float>(RecordSend);
            Sink = sinkMock.Object;
            var supplierMock = new Mock<ISupplyCounterDefinitions>();
            supplierMock.Setup(s => s.CreateDefinitions()).Returns(() => new[]
            {
                new CounterDefinition
                {
                    CategoryName = "Processor",
                    CounterName = "% Processor Time",
                    InstanceName = "_Total",
                    CollectInterval = 50
                }
            });
            Supplier = supplierMock.Object;
        }

        private Task RecordSend(string counter, float value)
        {
            SentValues.Add(value);
            Counters.Add(counter);
            return Task.FromResult(0);
        }
    }
}