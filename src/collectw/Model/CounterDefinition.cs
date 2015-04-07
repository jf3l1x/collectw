using System;
using System.Runtime.Serialization;

namespace CollectW.Model
{
    [DataContract]
    public class CounterDefinition
    {
        [DataMember]
        public string CategorieName { get; set; }

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

        public string Identifier
        {
            get
            {
                return string.Format("{0}.{1}.{2}.{3}", Environment.MachineName, CategorieName, CounterName,
                    InstanceName);
            }
        }

        public override string ToString()
        {
            return Identifier;
        }

      
    }
}