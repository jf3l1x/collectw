using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Collectw.Logging;
using CollectW.Services;

namespace CollectW.Model
{
    internal class Reader : IDisposable
    {
        private static readonly ILog Logger = LogProvider.For<Reader>();
        private readonly PerformanceCounter _counter;
        private readonly CounterDefinition _definition;

        internal Reader(CounterDefinition definition)
        {
            _definition = definition;
            _counter = new PerformanceCounter(definition.CategorieName, definition.CounterName, definition.InstanceName,
                true);
        }

        public void Dispose()
        {
            if (_counter != null)
            {
                _counter.Dispose();
            }
        }

        public Task Read(IEnumerable<ISendInfo> sinks)
        {
            return Task.Run(() =>
            {
                try
                {
                    float value = _counter.NextValue();
                    foreach (ISendInfo sink in sinks)
                    {
                        sink.Send(_definition.Identifier, value);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                    throw;
                }
            });
        }
    }
}