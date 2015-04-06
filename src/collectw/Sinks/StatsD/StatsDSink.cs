using System;
using System.Globalization;
using System.Threading.Tasks;
using Collectw.Logging;
using CollectW.Services;
using StatsdClient;

namespace CollectW.Sinks.StatsD
{
    public class StatsDSink : ISendInfo, IDisposable
    {
        private static readonly ILog Logger = LogProvider.For<StatsDSink>();
        private readonly StatsdUDP _connection;
        private readonly IResolveCounterType _resolver;
        private readonly Statsd _sender;

        public StatsDSink(IResolveCounterType resolver, Uri endpoint, int maxUdpPacket = 512)
        {
            if (resolver == null)
            {
                Logger.Error("a null resolver was passed to StatusDSink! throwing argument exception!");
                throw new ArgumentException("resolver cannot be null!");
            }
            _connection = new StatsdUDP(endpoint.Host, endpoint.Port, maxUdpPacket);
            _sender = new Statsd(_connection);
            _resolver = resolver;
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
            }
        }

        public Task Send(string counter, float value)
        {
            return Task.Run(() =>
            {
                StatsDTypes type = _resolver.Resolve(counter);
                switch (type)
                {
                    case StatsDTypes.Counting:
                        _sender.Send<Statsd.Counting>(counter, (int) value);
                        break;
                    case StatsDTypes.Gauge:
                        _sender.Send<Statsd.Gauge>(counter, value);
                        break;
                    case StatsDTypes.Histogram:
                        _sender.Send<Statsd.Histogram>(counter, (int) value);
                        break;
                    case StatsDTypes.Meter:
                        _sender.Send<Statsd.Meter>(counter, (int) value);
                        break;
                    case StatsDTypes.Set:
                        _sender.Send<Statsd.Set>(counter, value.ToString(CultureInfo.InvariantCulture));
                        break;
                    case StatsDTypes.Timing:
                        _sender.Send<Statsd.Timing>(counter, (int) value);
                        break;
                }
            });

        }
    }
}