using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CollectW.Extensions;
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
            if (_definition == null || _definition.CategoryName.IsEmpty() || 
                _definition.CounterName.IsEmpty())
            {
                Logger.Error("Invalid Counter Definition");
                throw new ArgumentException("definition");
            }
            
            _counter = new PerformanceCounter(definition.CategoryName, definition.CounterName, definition.InstanceName,
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
                    Parallel.ForEach(sinks, (sink) => sink.Send(_definition.Identifier, value));
                   
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