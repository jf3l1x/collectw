using System;
using System.Runtime.Serialization;

namespace CollectW.Model
{
    [DataContract]
    public class CounterDefinition
    {
        [DataMember]
        public string CategoryName { get; set; }

        [DataMember]
        public string InstanceName { get; set; }

        [DataMember]
        public string CounterName { get; set; }

        [DataMember]
        public int CollectInterval { get; set; }

        public TimeSpan CollectIntervalSpan
        {
            get { return TimeSpan.FromMilliseconds(CollectInterval); }
        }
    }
}