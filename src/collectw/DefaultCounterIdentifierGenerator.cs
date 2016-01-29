using CollectW.Model;
using CollectW.Services;

namespace CollectW
{
    public class DefaultCounterIdentifierGenerator : ICounterIdentifierGenerator
    {
        public string Generate(IMachineNameProvider machineNameProvider, CounterDefinition counterDefinition)
        {
            var machineName = machineNameProvider.GetMachineName();

            if (string.IsNullOrEmpty(counterDefinition.InstanceName))
            {
                return string.Format("{0}.{1}.{2}", machineName, counterDefinition.CategoryName, counterDefinition.CounterName);
            }

            return string.Format("{0}.{1}.{2}.{3}", machineName, counterDefinition.CategoryName, counterDefinition.CounterName, counterDefinition.InstanceName);
        }

    }
}