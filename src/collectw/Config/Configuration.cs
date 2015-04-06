using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Collectw.Logging;
using CollectW.Services;
using Newtonsoft.Json;

namespace CollectW.Config
{
    public class Configuration
    {
        private static readonly ILog Logger = LogProvider.For<Configuration>();
        private readonly string _path;
        private dynamic _configuration;

        public Configuration(string path = null)
        {
            if (!string.IsNullOrEmpty(path))
            {
                //Supplied path
                _path = path;
            }
            else
            {
                //Defaults to the directory of the exe
                _path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "config.json");
            }
            ReadConfig();
        }

        private void ReadConfig()
        {
            try
            {
                using (var stream = new StreamReader(_path))
                {
                    _configuration = JsonConvert.DeserializeObject(stream.ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("error reading configuration file: {@exception}", ex);
                throw;
            }
        }


        public Collector CreateCollector()
        {
            try
            {
                if (_configuration.CounterDefinition == null)
                {
                    Logger.Error("invalid configuration file! Missing CounterDefinition Property!");
                    throw new InvalidOperationException("invalid configuration file");
                }
                if (_configuration.Sinks == null)
                {
                    Logger.Error("invalid configuration file! Missing Sinks Property!");
                    throw new InvalidOperationException("invalid configuration file");
                }
                dynamic supplier = ObjectFactory.CreateDefinitionsSupplier(_configuration.CounterDefinition.Type,
                    _configuration.CounterDefinition.Configuration);
                var sinks = new List<ISendInfo>();
                foreach (dynamic sink in _configuration.Sinks)
                {
                    sinks.Add(ObjectFactory.CreateSink(sink.Type, sink.Configuration));
                }
                return new Collector(supplier, sinks);
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("Error creating Collector : {@exception}", ex);
                throw;
            }
        }
    }
}