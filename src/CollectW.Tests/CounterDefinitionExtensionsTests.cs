using System.Linq;
using CollectW.Extensions;
using CollectW.Model;
using Xunit;

namespace CollectW.Tests
{
    public class CounterDefinitionExtensionsTests
    {
        [Fact]
        public void return_true_for_exists_when_counter_has_no_intance()
        {
            var counter = new CounterDefinition
            {
                CategoryName = "Memory",
                CollectInterval = 1000,
                CounterName = "Available MBytes",
                InstanceName = Constants.SingleInstanceName
            };
            Assert.True(counter.Exists());
        }

        [Fact]
        public void
            when_passing_a_regex_to_return_all_instances_and_the_counter_has_no_instance_should_return_the_default_single_instance_name
            ()
        {
            var counter = new CounterDefinition
            {
                CategoryName = "Memory",
                CollectInterval = 1000,
                CounterName = "Available MBytes",
                InstanceName = "/.*/"
            };
            var counters = counter.Expand().ToList();
            Assert.Equal(1,counters.Count);
            Assert.Equal(Constants.SingleInstanceName,counters.First().InstanceName);
            Assert.Equal(counter.CategoryName, counters.First().CategoryName);
            Assert.Equal(counter.CounterName, counters.First().CounterName);
        }
        [Fact]
        public void
            when_passing_a_counter_name_with_spaces_should_expand_correctly
            ()
        {
            var counter = new CounterDefinition
            {
                CategoryName = "PhysicalDisk",
                CollectInterval = 1000,
                CounterName = "% Idle Time",
                InstanceName = "_Total"
            };
            var counters = counter.Expand().ToList();
            Assert.Equal(1, counters.Count);
            Assert.Equal(counter.InstanceName, counters.First().InstanceName);
            Assert.Equal(counter.CategoryName, counters.First().CategoryName);
            Assert.Equal(counter.CounterName, counters.First().CounterName);
        }
    }
}