using System;
using System.Collections.Generic;
using System.Threading;
using CollectW.Model;
using CollectW.Services;

namespace CollectW
{
    internal class Interval : IDisposable
    {
        private readonly IList<Reader> _readers;
        private readonly IEnumerable<ISendInfo> _sinks;
        private readonly TimeSpan _period;
        private readonly Timer _timer;

        public Interval(TimeSpan period, IEnumerable<ISendInfo> sinks)
        {
            _period = period;
            _sinks = sinks;
            _timer = new Timer(Tick, null, Timeout.InfiniteTimeSpan, period);
            _readers = new List<Reader>();
        }

        public void Start()
        {
            _timer.Change(TimeSpan.Zero, _period);
        }

        public void Stop()
        {
            _timer.Change(Timeout.InfiniteTimeSpan, _period);
        }
        public void Dispose()
        {
            foreach (var reader in _readers)
            {
                reader.Dispose();

            }
            _timer.Dispose();
        }

        private void Tick(object state)
        {
            foreach (Reader reader in _readers)
            {
                reader.Read(_sinks);
            }
        }

        public void AddDefinition(CounterDefinition definition)
        {
            if (definition.CollectIntervalSpan != _period)
            {
                throw new ArgumentException("definition.CollectionInterval is not equals to this interval period!");
            }
            _readers.Add(new Reader(definition));
        }
    }
}