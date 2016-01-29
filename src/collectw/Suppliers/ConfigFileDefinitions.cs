using System;
using System.Collections.Generic;
using Collectw.Logging;
using CollectW.Model;
using CollectW.Services;
using Newtonsoft.Json.Linq;

namespace CollectW.Config
{
    public class ConfigFileDefinitions : ISupplyCounterDefinitions
    {
        private static readonly ILog Logger = LogProvider.For<ConfigFileDefinitions>();
        private List<CounterDefinition> _definitions;
        
        public IEnumerable<CounterDefinition> CreateDefinitions()
        {
            return _definitions;
        }

        public void Configure(dynamic configuration)
        {
            try
            {
                _definitions = new List<CounterDefinition>();
                foreach (JObject def in configuration)
                {
                    _definitions.Add(def.ToObject<CounterDefinition>());
                }
               
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat(
                    "error trying to read configurations from config file . Exception {@exception}", ex);
                throw;
            }
        }

        public event EventHandler DefinitionsChanged;
    }
}