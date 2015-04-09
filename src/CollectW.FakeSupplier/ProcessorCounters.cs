using System;
using System.Collections.Generic;
using CollectW.Model;
using CollectW.Services;

namespace CollectW.FakeSupplier
{
    public class ProcessorCounters : ISupplyCounterDefinitions,IDisposable
    {
       
        public IEnumerable<CounterDefinition> CreateDefinitions()
        {
            yield return
                new CounterDefinition
                {
                    InstanceName = "0",
                    CategoryName = "Processor",
                    CounterName = "% Processor Time",
                    CollectInterval = 100
                };
        }

        public void Configure(dynamic configuration)
        {
            
        }

        public event EventHandler DefinitionsChanged;
        public void Dispose()
        {
            Disposed = true;
        }

        public bool Disposed { get; set; }
    }
}