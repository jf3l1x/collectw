using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Collectw.Logging;
using CollectW.Services;
using Newtonsoft.Json;

namespace CollectW.Config
{
    public class Configuration:IConfigureCollector,IDisposable
    {
        private static readonly ILog Logger = LogProvider.For<Configuration>();
        private readonly string _path;
        private dynamic _configuration;
        private FileSystemWatcher _watcher;
        private List<ISendInfo> _sinks;
        private string _configurationHash = string.Empty;
        private Task _updateConfig;

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
                _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            }
            ReadConfig();
        }

        private void ReadConfig()
        {
            try
            {

                using (var stream = File.Open(_path,FileMode.Open,FileAccess.Read,FileShare.Read))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var configString = reader.ReadToEnd();
                        _configurationHash = Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(configString)));
                        _configuration = JsonConvert.DeserializeObject(configString);    
                    }
                    
                }
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
                DisposeSinks();
                _sinks = new List<ISendInfo>();
                foreach (dynamic sink in _configuration.Sinks)
                {
                    _sinks.Add(ObjectFactory.CreateSink(sink.Type, sink.Configuration));
                }
                DisposeSupplier();
                Supplier = ObjectFactory.CreateDefinitionsSupplier(_configuration.CounterDefinition.Type,
                    _configuration.CounterDefinition.Configuration);
                StartWatch();
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("error reading configuration file: {@exception}", ex);
                throw;
            }
        }

        private Task InvokeWhenFileIsReady()
        {
            return Task.Run(() =>
            {
                bool done = false;
                while (!done)
                {
                    try
                    {
                        using (var stream = File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            done = true;
                            using (var reader = new StreamReader(stream))
                            {
                                if (
                                    Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(reader.ReadToEnd()))) !=
                                    _configurationHash)
                                {
                                    ReadConfig();
                                    if (Changed != null)
                                    {
                                        Changed(this, EventArgs.Empty);
                                    }   
                                    Logger.Debug("Config File Has Changed");
                                }
                                else
                                {
                                    Logger.Debug("Config File has not changed!");
                                }

                            }
                        }
                    }
                    catch (IOException)
                    {

                    }
                }
            });
            
            
            
            
        }
        private void DisposeSupplier()
        {
            if (Supplier != null)
            {
                var disposable = Supplier as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        private void StartWatch()
        {
            if (_watcher == null)
            {
                _watcher = new FileSystemWatcher(Path.GetDirectoryName(Path.GetFullPath(_path)), Path.GetFileName(_path));
                _watcher.NotifyFilter = NotifyFilters.LastWrite;
                _watcher.Changed += OnChange;
                _watcher.EnableRaisingEvents = true;    
            }
            
        }

        void OnChange(object sender, FileSystemEventArgs e)
        {
            Logger.DebugFormat("Configuration file changed! status {@args}",e);
            if (_updateConfig == null || _updateConfig.IsCompleted)
            {
                _updateConfig = InvokeWhenFileIsReady();
            }


           
            
        }

        public Collector CreateCollector()
        {
            try
            {
                return new Collector(this);
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("Error creating Collector : {@exception}", ex);
                throw;
            }
        }

        public void Dispose()
        {
            if (_watcher != null)
            {
                _watcher.Dispose();
            }
            if (_updateConfig != null && !_updateConfig.IsCompleted && !_updateConfig.IsFaulted)
            {
                _updateConfig.Dispose();
            }
            DisposeSinks();
        }

        private void DisposeSinks()
        {
            if (_sinks != null)
            {
                foreach (var sendInfo in _sinks)
                {
                    var disposable = sendInfo as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
            }
        }

        public ISupplyCounterDefinitions Supplier { get; private set; }

        public IEnumerable<ISendInfo> Sinks
        {
            get { return _sinks; }
        }
        public event EventHandler Changed;
    }
}