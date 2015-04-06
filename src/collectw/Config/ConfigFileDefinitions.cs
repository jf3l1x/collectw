using System;
using System.Collections.Generic;
using CollectW.Helpers;
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
        private List<IObserver<IEnumerable<CounterDefinition>>> _observers;

        
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
                if (_observers != null)
                {
                    foreach (var observer in _observers)
                    {
                        observer.OnNext(_definitions);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat(
                    "error trying to read configurations from config file . Exception {@exception}", ex);
                throw;
            }
        }


        public IDisposable Subscribe(IObserver<IEnumerable<CounterDefinition>> observer)
        {
            if (_observers == null)
            {
                _observers = new List<IObserver<IEnumerable<CounterDefinition>>>();
            }
            _observers.Add(observer);
            return new Unsubscriber<IEnumerable<CounterDefinition>> (_observers,observer);
        }
    }
}