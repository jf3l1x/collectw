using System;
using System.Collections.Generic;
using CollectW.Extensions;
using Collectw.Logging;
using CollectW.Model;
using CollectW.Services;

namespace CollectW
{
    public class Collector : IDisposable
    {
        private static readonly ILog Logger = LogProvider.For<Collector>();
        private readonly Dictionary<TimeSpan, Interval> _counters = new Dictionary<TimeSpan, Interval>();
        public Collector(ISupplyCounterDefinitions definitionSupplier, IEnumerable<ISendInfo> sinks)
        {
            try
            {
                if (definitionSupplier == null)
                {
                    Logger.Error("No definition supplier specified!");
                    throw new ArgumentException("definitionSupplier");
                }
                foreach (CounterDefinition definition in definitionSupplier.CreateDefinitions())
                {
                    AddReader(definition,sinks);
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("error trying to configure the collector {@exception}", ex);
                throw;
            }
        }

        public void Dispose()
        {
            Stop();
            foreach (Interval counter in _counters.Values)
            {
                counter.Dispose();
            }
        }

        private void AddReader(CounterDefinition definition, IEnumerable<ISendInfo> sinks)
        {
            if (definition.Exists())
            {
                Interval interval = null;
                if (!_counters.TryGetValue(definition.CollectIntervalSpan, out interval))
                {
                    interval = new Interval(definition.CollectIntervalSpan, sinks);
                    _counters.Add(definition.CollectIntervalSpan, interval);
                }
                interval.AddDefinition(definition);    
            }
            else
            {
                Logger.ErrorFormat("received a counter definition for a non existente counter/instance: {@definition}. Ignoring it!",definition);
            }
            
        }

        public void Start()
        {
            foreach (Interval interval in _counters.Values)
            {
                interval.Start();
            }
        }


        public void Stop()
        {
            foreach (Interval interval in _counters.Values)
            {
                interval.Stop();
            }
        }
    }
}