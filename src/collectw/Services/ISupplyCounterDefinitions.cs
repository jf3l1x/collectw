using System;
using System.Collections.Generic;
using CollectW.Model;

namespace CollectW.Services
{
    public interface ISupplyCounterDefinitions
    {
        IEnumerable<CounterDefinition> CreateDefinitions();
        void Configure(dynamic configuration);
        event EventHandler DefinitionsChanged;
    }
}