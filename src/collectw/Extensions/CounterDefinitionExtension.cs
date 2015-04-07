using System.Collections.Generic;
using System.Diagnostics;
using CollectW.Model;

namespace CollectW.Extensions
{
    internal static class CounterDefinitionExtension
    {
        internal static bool Exists(this CounterDefinition definition)
        {
            return PerformanceCounterCategory.Exists(definition.CategorieName) &&
                   PerformanceCounterCategory.CounterExists(definition.CounterName,
                       definition.CategorieName) &&
                   PerformanceCounterCategory.InstanceExists(definition.InstanceName, definition.CategorieName);
        }

        internal static IEnumerable<CounterDefinition> Expand(this CounterDefinition definition)
        {
            foreach (var category in definition.ExpandCategories())
            {
                foreach (var instance in definition.ExpandInstances(category))
                {
                    foreach (var counter in definition.ExpandCounters(category,instance))
                    {
                        yield return new CounterDefinition()
                        {
                            CategorieName = category,
                            InstanceName = instance,
                            CounterName = counter,
                            CollectInterval = definition.CollectInterval
                        };
                    }
                }
            }
            
        }

        private static IEnumerable<string> ExpandInstances(this CounterDefinition definition,
            string category)
        {
            var regex = definition.InstanceName.AsRegex();
            if (regex != null)
            {
                foreach (var instance in new PerformanceCounterCategory(category).GetInstanceNames())
                {
                    if (instance.IsEmpty() || regex.IsMatch(instance)) 
                    {

                        yield return instance;
                    }
                }
            }
            else
            {
                yield return definition.InstanceName;
            }
        }
        private static IEnumerable<string> ExpandCounters(this CounterDefinition definition,string category,string instance)
        {
            var regex = definition.CounterName.AsRegex();
            if (regex != null)
            {
                foreach (var counter in new PerformanceCounterCategory(category).GetCounters(instance))
                {
                    if (regex.IsMatch(counter.CounterName))
                    {
                        
                        yield return counter.CounterName;
                    }
                }
            }
            else
            {
                yield return definition.CounterName;
            }
        }
        internal static IEnumerable<string> ExpandCategories(this CounterDefinition definition)
        {
            var regex = definition.CategorieName.AsRegex();
            if (regex!=null)
            {
                foreach (var category in PerformanceCounterCategory.GetCategories())
                {
                    
                    if (regex.IsMatch(category.CategoryName))
                    {
                        yield return category.CategoryName;
                    }
                }
            }
            else
            {
                yield return definition.CategorieName;
            }

        }
    }
}