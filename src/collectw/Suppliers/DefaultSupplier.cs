using System;
using System.Collections.Generic;
using CollectW.Model;
using CollectW.Services;

namespace CollectW.Suppliers
{
    public class DefaultSupplier : ISupplyCounterDefinitions
    {
        private readonly List<CounterDefinition> _definitions;

        public DefaultSupplier(params CounterDefinition[] definitions)
        {
            _definitions = new List<CounterDefinition>(definitions);
        }

        public IEnumerable<CounterDefinition> CreateDefinitions()
        {
            return _definitions;
        }

        public void Configure(dynamic configuration)
        {
        }

        public event EventHandler DefinitionsChanged;

        public void Add(CounterDefinition definition)
        {
            if (!_definitions.Contains(definition))
            {
                _definitions.Add(definition);
                if (DefinitionsChanged != null)
                {
                    DefinitionsChanged(this, EventArgs.Empty);
                }
            }
        }
    }
}