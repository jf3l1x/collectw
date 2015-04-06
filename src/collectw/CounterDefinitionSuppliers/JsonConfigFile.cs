using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Collectw.Logging;
using CollectW.Model;
using CollectW.Services;
using Newtonsoft.Json;

namespace CollectW.CounterDefinitionSuppliers
{
    public class JsonConfigFile : ISupplyCounterDefinitions
    {
        private static readonly ILog Logger = LogProvider.For<JsonConfigFile>();
        private readonly string _path;

        public JsonConfigFile(string path = null)
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
        }

        public IEnumerable<CounterDefinition> CreateDefinitions()
        {
            try
            {
                Logger.DebugFormat("trying to read configurations from file {@filePath}", _path);
                using (var stream = new StreamReader(_path))
                {
                    return JsonConvert.DeserializeObject<IEnumerable<CounterDefinition>>(stream.ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat(
                    "error trying to read configurations from config file : {@filePath}. Exception {@exception}", _path,
                    ex);
                throw;
            }
        }
    }
}