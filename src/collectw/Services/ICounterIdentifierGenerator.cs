using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollectW.Model;

namespace CollectW.Services
{
    public interface ICounterIdentifierGenerator
    {
        string Generate(IMachineNameProvider machineNameProvider, CounterDefinition counterDefinition);
    }
}
