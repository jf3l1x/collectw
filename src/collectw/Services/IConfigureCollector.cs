using System;
using System.Collections.Generic;

namespace CollectW.Services
{
    public interface IConfigureCollector
    {
        ISupplyCounterDefinitions Supplier { get; }
        IEnumerable<ISendInfo> Sinks { get; }
        event EventHandler Changed;
    }
}