using System;
using Serilog;
using Xunit;

namespace CollectW.Tests.Configuration
{
    public class ConfigurationTests
    {
        public ConfigurationTests()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console().MinimumLevel.Debug()
                .CreateLogger();
        }
        [Fact]
        public void ConfigurationFailsIfEmpty()
        {
            var configuration = new Config.Configuration(@"Configuration\ConfigFiles\Empty.json");
            Assert.Throws(typeof (InvalidOperationException), () =>
            {
                var collector = configuration.CreateCollector();
            });

        }

        [Fact]
        public void ConfigurationFailsIfMissingCounterDefinition()
        {
            var configuration = new Config.Configuration(@"Configuration\ConfigFiles\MissingCounterDefinition.json");
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                var collector = configuration.CreateCollector();
            });
        }

        [Fact]
        public void ConfigurationFailsIfMissingSinks()
        {
            var configuration = new Config.Configuration(@"Configuration\ConfigFiles\MissingSinks.json");
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                var collector = configuration.CreateCollector();
            });
        }
        [Fact]
        public void ConfigurationShouldWorkIfIthasAllSections()
        {
            var configuration = new Config.Configuration(@"Configuration\ConfigFiles\Basic.json");
            var collector = configuration.CreateCollector();
            Assert.NotNull(collector);
        }

        [Fact]
        public void ConfigurationShouldBeAbleToLoadExternalAssembliesForSinks()
        {
            var configuration = new Config.Configuration(@"Configuration\ConfigFiles\FakeSink.json");
            var collector = configuration.CreateCollector();
            Assert.NotNull(collector);
        }
        [Fact]
        public void ConfigurationShouldBeAbleToLoadExternalAssembliesForSuppliers()
        {
            var configuration = new Config.Configuration(@"Configuration\ConfigFiles\ProcessorCounters.json");
            var collector = configuration.CreateCollector();
            Assert.NotNull(collector);
        }
       
    }
}
