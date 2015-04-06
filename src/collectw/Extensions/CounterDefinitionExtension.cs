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
    }
}