using System;
using System.Collections.Generic;
using CollectW.Model;

namespace CollectW.Services
{
    public interface ISupplyCounterDefinitions:IObservable<IEnumerable<CounterDefinition>>
    {
        IEnumerable<CounterDefinition> CreateDefinitions();
        void Configure(dynamic configuration);
    }
}