﻿using System;
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
        private readonly IMachineNameProvider _machineNameProvider;
        private readonly ICounterIdentifierGenerator _counterIdentifierGenerator;

        internal Reader(CounterDefinition definition, IMachineNameProvider machineNameProvider, ICounterIdentifierGenerator counterIdentifierGenerator)
        {
            _definition = definition;
            _machineNameProvider = machineNameProvider;
            _counterIdentifierGenerator = counterIdentifierGenerator;
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
                    //StatsD does not accept : in the identifier name
                    Parallel.ForEach(sinks, (sink) => sink.Send(_counterIdentifierGenerator.Generate(_machineNameProvider, _definition).Replace(":",string.Empty), value));
                   
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