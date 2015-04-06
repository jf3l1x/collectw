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
        private StatsdUDP _connection;
        private RegexResolver _resolver;
        private Statsd _sender;

        public StatsDSink(RegexResolver resolver, Uri uri,int maxUdpPacketSize=512)
        {
            _resolver = resolver;
            _connection = new StatsdUDP(uri.Host, uri.Port, maxUdpPacketSize);
        }

        public StatsDSink()
        {
            
        }
        public void Dispose()
        {
            DisposeDependencies();
        }

        private void DisposeDependencies()
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

        public void Configure(dynamic configuration)
        {
            try
            {
                DisposeDependencies();
                //IResolveCounterType resolver, Uri endpoint, int maxUdpPacket = 512
                _connection = new StatsdUDP(configuration.Host.ToString(), (int)configuration.Port, (int)configuration.MaxUdpPacket);
                _sender = new Statsd(_connection);
                _resolver = new RegexResolver();
                foreach (var map in configuration.CounterTypeMaps)
                {
                    _resolver.Add(map.Regex.ToString(), Enum.Parse(typeof(StatsDTypes),map.Type.ToString()));
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("Error configuring StatsDSink, check your configuration file! Exception:{@exception}",ex);
                throw;
            }
         
        }
    }
}