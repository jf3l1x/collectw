using System;
using System.Collections.Generic;
using System.Linq;
using CollectW.Extensions;
using Collectw.Logging;
using CollectW.Model;
using CollectW.Services;

namespace CollectW
{
    public class Collector : IDisposable
    {
        private readonly IConfigureCollector _config;
        private static readonly ILog Logger = LogProvider.For<Collector>();
        private readonly Dictionary<TimeSpan, Interval> _counters = new Dictionary<TimeSpan, Interval>();
        private ISupplyCounterDefinitions _definitionSupplier;
        private IEnumerable<ISendInfo> _sinks;
        private bool _running;

        public Collector(ISupplyCounterDefinitions definitionSupplier, IEnumerable<ISendInfo> sinks)
        {
            SetSupplier(definitionSupplier);
            _sinks = sinks.ToList();
            try
            {
                if (_definitionSupplier == null)
                {
                    Logger.Error("No definition supplier specified!");
                    throw new ArgumentException("definitionSupplier");
                }
                ConfigureReaders(_sinks);
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("error trying to configure the collector {@exception}", ex);
                throw;
            }
        }

        private void SetSupplier(ISupplyCounterDefinitions definitionSupplier)
        {
            if (_definitionSupplier != null)
            {
                _definitionSupplier.DefinitionsChanged -= DefinitionsChanged;
            }
            _definitionSupplier = definitionSupplier;
            _definitionSupplier.DefinitionsChanged += DefinitionsChanged;
        }

        public Collector(IConfigureCollector config) : this(config.Supplier, config.Sinks)
        {
            _config = config;
            config.Changed += config_Changed;
        }

        void config_Changed(object sender, EventArgs e)
        {
            _sinks = _config.Sinks.ToList();
            SetSupplier(_config.Supplier);
            ConfigureReaders(_sinks);
        }
      
        private void DefinitionsChanged(object sender, EventArgs e)
        {
            ConfigureReaders(_sinks);
        }

        private void ConfigureReaders(IEnumerable<ISendInfo> sinks)
        {
            DisposeIntervals();
            _counters.Clear();
            foreach (CounterDefinition definition in _definitionSupplier.CreateDefinitions())
            {
                AddReader(definition, sinks);
            }
            if (_running)
            {
                Start();
            }
        }

     
        public void Dispose()
        {
            Stop();
            DisposeIntervals();
            
        }

        private void DisposeIntervals()
        {
            foreach (Interval counter in _counters.Values)
            {
                counter.Dispose();
            }
        }

        private void AddReader(CounterDefinition definition, IEnumerable<ISendInfo> sinks)
        {
            foreach (var counterDefinition in definition.Expand())
            {
                if (counterDefinition.Exists())
                {
                    Interval interval = null;
                    if (!_counters.TryGetValue(counterDefinition.CollectIntervalSpan, out interval))
                    {
                        interval = new Interval(counterDefinition.CollectIntervalSpan, sinks);
                        _counters.Add(counterDefinition.CollectIntervalSpan, interval);
                    }
                    interval.AddDefinition(counterDefinition);
                }
                else
                {
                    Logger.ErrorFormat(
                        "received a counter definition for a non existente counter/instance: {@definition}. Ignoring it!",
                        counterDefinition);
                }
            }
           
        }

        public void Start()
        {
            _running = true;
            foreach (Interval interval in _counters.Values)
            {
                interval.Start();
            }
        }


        public void Stop()
        {
            _running = false;
            foreach (Interval interval in _counters.Values)
            {
                interval.Stop();
            }
        }

      
    }
}