using System;
using System.Collections.Generic;
using CollectW.Model;
using CollectW.Services;

namespace CollectW.Tests.Impl
{
    public class UpdateableSupplier : ISupplyCounterDefinitions
    {
        private readonly List<CounterDefinition> _counters = new List<CounterDefinition>();

        public IEnumerable<CounterDefinition> CreateDefinitions()
        {
            return _counters;
        }

        public void Configure(dynamic configuration)
        {
        }

        public event EventHandler DefinitionsChanged;

        public void AddCounter(CounterDefinition counter)
        {
            _counters.Add(counter);
            if (DefinitionsChanged != null)
            {
                DefinitionsChanged(this, EventArgs.Empty);
            }
        }
    }
}