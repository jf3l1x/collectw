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

        public string Identifier
        {
            get
            {
                if (string.IsNullOrEmpty(InstanceName))
                {
                    return string.Format("{0}.{1}.{2}", Environment.MachineName, CategoryName, CounterName);
                }
                return string.Format("{0}.{1}.{2}.{3}", Environment.MachineName, CategoryName, CounterName,
                    InstanceName);
            }
        }

        public override string ToString()
        {
            return Identifier;
        }

      
    }
}