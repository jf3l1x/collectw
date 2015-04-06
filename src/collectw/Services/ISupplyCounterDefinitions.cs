using System.Collections.Generic;
using CollectW.Model;

namespace CollectW.Services
{
    public interface ISupplyCounterDefinitions
    {
        IEnumerable<CounterDefinition> CreateDefinitions();
    }
}