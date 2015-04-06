using System;
using System.Collections.Generic;
using CollectW.Model;
using CollectW.Services;

namespace CollectW.FakeSupplier
{
    public class ProcessorCounters : ISupplyCounterDefinitions
    {
        public IDisposable Subscribe(IObserver<IEnumerable<CounterDefinition>> observer)
        {
            return null;
        }

        public IEnumerable<CounterDefinition> CreateDefinitions()
        {
            yield return
                new CounterDefinition
                {
                    InstanceName = "0",
                    CategorieName = "Processor",
                    CounterName = "% Processor Time",
                    CollectInterval = 100
                };
        }

        public void Configure(dynamic configuration)
        {
            
        }
    }
}