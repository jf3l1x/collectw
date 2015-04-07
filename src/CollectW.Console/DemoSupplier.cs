using System;
using System.Collections.Generic;
using CollectW.Model;
using CollectW.Services;

namespace CollectW.Console
{
    internal class DemoSupplier : ISupplyCounterDefinitions
    {
        public IEnumerable<CounterDefinition> CreateDefinitions()
        {
            yield return
                new CounterDefinition
                {
                    CategorieName = "Processor",
                    CounterName = "% Processor Time",
                    InstanceName = "2",
                    CollectInterval = 5000
                };
            yield return
              new CounterDefinition
              {
                  CategorieName = "Processor",
                  CounterName = "% Processor Time",
                  InstanceName = "0",
                  CollectInterval = 5000
              };
            yield return
            new CounterDefinition
            {
                CategorieName = "Processor",
                CounterName = "% Processor Time",
                InstanceName = "_Total",
                CollectInterval = 5000
            };
            yield return
           new CounterDefinition
           {
               CategorieName = "NonExistent",
               CounterName = "% Processor Time",
               InstanceName = "_Total",
               CollectInterval = 700
           };
        }

        public void Configure(dynamic configuration)
        {
            
        }

        public event EventHandler DefinitionsChanged;
    }
}