using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Collectw.Logging;
using CollectW.Services;

namespace CollectW.Model
{
    internal class Reader:IDisposable
    {
        private readonly CounterDefinition _definition;
        private readonly PerformanceCounter _counter;
        private static readonly ILog Logger = LogProvider.For<Reader>();
        internal Reader(CounterDefinition definition)
        {
            _definition = definition;
            _counter=new PerformanceCounter(definition.CategorieName,definition.CounterName,definition.InstanceName,true);
        }

        public Task Read(IEnumerable<ISendInfo> sinks)
        {
            return Task.Run(() =>
            {
                try
                {
                  
                    var value = _counter.NextValue();
                    foreach (var sink in sinks)
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

        public void Dispose()
        {
            if (_counter != null)
            {
                _counter.Dispose();
            }
        }
    }
}