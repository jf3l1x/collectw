using System;
using System.IO;
using System.Linq;
using System.Threading;
using Serilog;
using Xunit;

namespace CollectW.Tests.Configuration
{
    public class ConfigurationTests
    {
        static ConfigurationTests()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Path.Combine(Path.GetTempPath(), "log.txt")).MinimumLevel.Debug()
                .CreateLogger();
        }

        [Fact]
        public void ConfigurationFailsIfEmpty()
        {
            Assert.Throws(typeof (InvalidOperationException),
                () =>
                {
                    using (var configuration = new Config.Configuration(@"Configuration\ConfigFiles\Empty.json"))
                    {
                        Collector collector = configuration.CreateCollector();
                    }
                });
        }

        [Fact]
        public void ConfigurationFailsIfMissingCounterDefinition()
        {
            Assert.Throws(typeof (InvalidOperationException),
                () =>
                {
                    using (
                        var configuration =
                            new Config.Configuration(@"Configuration\ConfigFiles\MissingCounterDefinition.json")
                        )
                    {
                        Collector collector = configuration.CreateCollector();
                    }
                });
        }

        [Fact]
        public void ConfigurationFailsIfMissingSinks()
        {
            Assert.Throws(typeof (InvalidOperationException), () =>
            {
                using (var configuration = new Config.Configuration(@"Configuration\ConfigFiles\MissingSinks.json"))
                {
                    Collector collector = configuration.CreateCollector();
                }
            });
        }

        [Fact]
        public void ConfigurationShouldWorkIfIthasAllSections()
        {
            using (var configuration = new Config.Configuration(@"Configuration\ConfigFiles\Basic.json"))
            {
                Collector collector = configuration.CreateCollector();
                Assert.NotNull(collector);
            }
        }

        [Fact]
        public void ConfigurationShouldBeAbleToLoadExternalAssembliesForSinks()
        {
            using (var configuration = new Config.Configuration(@"Configuration\ConfigFiles\FakeSink.json"))
            {
                Collector collector = configuration.CreateCollector();
                Assert.NotNull(collector);
            }
        }

        [Fact]
        public void ConfigurationShouldBeAbleToLoadExternalAssembliesForSuppliers()
        {
            using (var configuration = new Config.Configuration(@"Configuration\ConfigFiles\ProcessorCounters.json"))
            {
                Collector collector = configuration.CreateCollector();
                Assert.NotNull(collector);
            }
        }

        [Fact]
        public void ConfigurationShouldUseConfigJsonAsDefault()
        {
            using (var configuration = new Config.Configuration())
            {
                Collector collector = configuration.CreateCollector();
                Assert.NotNull(collector);
            }
        }

        [Fact]
        public void IfAnyChangesHappensToTheConfigFileAnEventShouldBeRaised()
        {
            try
            {
                Log.Logger.Debug("Starting test IfAnyChangesHappensToTheConfigFileAnEventShouldBeRaised");
                File.Copy(@"Configuration\ConfigFiles\Basic.json",
                    @"Configuration\ConfigFiles\IfAnyChangesHappensToTheConfigFileAnEventShouldBeRaised.json", true);
                using (
                    var configuration =
                        new Config.Configuration(
                            @"Configuration\ConfigFiles\IfAnyChangesHappensToTheConfigFileAnEventShouldBeRaised.json"))
                {
                    bool triggered = false;
                    configuration.Changed += (s, e) => { triggered = true; };
                    File.Copy(@"Configuration\ConfigFiles\FakeSink.json",
                        @"Configuration\ConfigFiles\IfAnyChangesHappensToTheConfigFileAnEventShouldBeRaised.json", true);
                    Thread.Sleep(200);
                    Assert.True(triggered);
                }
            }
            finally
            {
                File.Delete(@"Configuration\ConfigFiles\IfAnyChangesHappensToTheConfigFileAnEventShouldBeRaised.json");
            }
        }

        [Fact]
        public void WhenConfigurationChangesAllDisposableSinksAreDisposed()
        {
            try
            {
                Log.Logger.Debug("Starting test WhenConfigurationChangesAllDisposableSinksAreDisposed");
                File.Copy(@"Configuration\ConfigFiles\FakeSink.json",
                    @"Configuration\ConfigFiles\WhenConfigurationChangesAllDisposableSinksAreDisposed.json", true);
                using (
                    var configuration =
                        new Config.Configuration(
                            @"Configuration\ConfigFiles\WhenConfigurationChangesAllDisposableSinksAreDisposed.json"))
                {
                    Assert.Equal(1, configuration.Sinks.Count());
                    var sink = (dynamic) configuration.Sinks.First();
                    Assert.Equal("SendToNull", sink.GetType().Name);
                    Assert.Equal(false, sink.Disposed);
                    File.Copy(@"Configuration\ConfigFiles\Basic.json",
                        @"Configuration\ConfigFiles\WhenConfigurationChangesAllDisposableSinksAreDisposed.json", true);
                    Thread.Sleep(200);
                    Assert.Equal(true, sink.Disposed);
                }
            }
            finally
            {
                File.Delete(@"Configuration\ConfigFiles\WhenConfigurationChangesAllDisposableSinksAreDisposed.json");
            }
        }

        [Fact]
        public void WhenConfigurationChangesTheSinkCollectionChanges()
        {
            try
            {
                Log.Logger.Debug("Starting test WhenConfigurationChangesTheSinkCollectionChanges");
                File.Copy(@"Configuration\ConfigFiles\FakeSink.json",
                    @"Configuration\ConfigFiles\WhenConfigurationChangesTheSinkCollectionChanges.json", true);
                using (
                    var configuration =
                        new Config.Configuration(
                            @"Configuration\ConfigFiles\WhenConfigurationChangesTheSinkCollectionChanges.json"))
                {
                    Assert.Equal(1, configuration.Sinks.Count());
                    Assert.Equal("SendToNull", configuration.Sinks.First().GetType().Name);
                    File.Copy(@"Configuration\ConfigFiles\Basic.json",
                        @"Configuration\ConfigFiles\WhenConfigurationChangesTheSinkCollectionChanges.json", true);
                    Thread.Sleep(200);
                    Assert.Equal("ConsoleSink", configuration.Sinks.First().GetType().Name);
                }
            }
            finally
            {
                File.Delete(@"Configuration\ConfigFiles\WhenConfigurationChangesTheSinkCollectionChanges.json");
            }
        }

        [Fact]
        public void WhenConfigurationChangesTheSupplierIsDisposed()
        {
            try
            {
                Log.Logger.Debug("Starting test WhenConfigurationChangesTheSupplierIsDisposed");
                File.Copy(@"Configuration\ConfigFiles\ProcessorCounters.json",
                    @"Configuration\ConfigFiles\WhenConfigurationChangesTheSupplierIsDisposed.json", true);
                using (
                    var configuration =
                        new Config.Configuration(
                            @"Configuration\ConfigFiles\WhenConfigurationChangesTheSupplierIsDisposed.json"))
                {
                    Assert.NotNull(configuration.Supplier);
                    var supplier = (dynamic) configuration.Supplier;
                    Assert.Equal("ProcessorCounters", supplier.GetType().Name);
                    Assert.Equal(false, supplier.Disposed);
                    File.Copy(@"Configuration\ConfigFiles\Basic.json",
                        @"Configuration\ConfigFiles\WhenConfigurationChangesTheSupplierIsDisposed.json", true);
                    Thread.Sleep(200);
                    Assert.Equal(true, supplier.Disposed);
                }
            }
            finally
            {
                File.Delete(@"Configuration\ConfigFiles\WhenConfigurationChangesTheSupplierIsDisposed.json");
            }
        }
        [Fact]
        public void WhenConfigurationChangesTheSupplierChanges()
        {
            try
            {
                File.Copy(@"Configuration\ConfigFiles\ProcessorCounters.json",
                    @"Configuration\ConfigFiles\WhenConfigurationChangesTheSupplierChanges.json", true);
                using (
                    var configuration =
                        new Config.Configuration(
                            @"Configuration\ConfigFiles\WhenConfigurationChangesTheSupplierChanges.json"))
                {
                    Assert.NotNull(configuration.Supplier);
                    Assert.Equal("ProcessorCounters", configuration.Supplier.GetType().Name);
                    File.Copy(@"Configuration\ConfigFiles\Basic.json",
                        @"Configuration\ConfigFiles\WhenConfigurationChangesTheSupplierChanges.json", true);
                    Thread.Sleep(200);
                    Assert.Equal("ConfigFileDefinitions", configuration.Supplier.GetType().Name);
                }
            }
            finally
            {
                File.Delete(@"Configuration\ConfigFiles\WhenConfigurationChangesTheSupplierChanges.json");
            }
        }
    }
}