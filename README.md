# collectw [![Build status](https://ci.appveyor.com/api/projects/status/43hsevhbhl029fyv?svg=true)](https://ci.appveyor.com/project/JoseFelix/collectw) [![NuGet Status](http://img.shields.io/nuget/v/collectw.svg?style=flat)](https://www.nuget.org/packages/collectw/)[![Chocolatey Status](https://img.shields.io/chocolatey/v/chocolatey.service.svg)](https://chocolatey.org/packages/collectw.service)

A Windows performance counter collector and forwarder

If you need to collect Windows performance counters this may fit yours needs.

You can use collectw as a lib inside your system or you can use collectw.service as a windows service

##Quick Start using as a lib

Grab the collectw [nuget](https://www.nuget.org/packages/collectw)


###Sending to console

```C#
    var collector =
        new Collector(
            new DefaultSupplier(new[]
            {
                new CounterDefinition()
                {
                    CategoryName = "Processor",
                    InstanceName = "_Total",
                    CounterName = "% Processor Time",
                    CollectInterval = 50
                }
            }), new[] {new ConsoleSink()});
    collector.Start();
    //Do your stuff
    Thread.Sleep(400);
    //at some point, when no more performance counter info is needed
    collector.Stop();
    collector.Dispose();
```

###Sending info to statsD

```C#
    var collector =
        new Collector(
            new DefaultSupplier(new[]
            {
                new CounterDefinition()
                {
                    CategoryName = "Processor",
                    InstanceName = "_Total",
                    CounterName = "% Processor Time",
                    CollectInterval = 5000
                }
            }),
            new[]
            {
                    new StatsDSink(
                        new RegexResolver().Add(".*", StatsDTypes.Gauge),
                        new Uri("UDP://localhost:8125")
                        )
            });
    collector.Start();
    //Do your stuff
    Thread.Sleep(400);
    //at some point, when no more performance counter info is needed
    collector.Stop();
    collector.Dispose();
```
The StatsD Sink receives a RegexResolver that resolves wich statsD type the counter represents. you can use the Add Method of the instance to setup using regular expressions the map between the counter full name (category.countername.instance) and the counter type. In the example above i'm saying that all counters will be of type gauge. The Regex Resolver caches the result of the match to avoid performance degradation, in this way the same counter fullname will be checked against the regexes only once.


##Quick Start using as a service

You can install the service using the [chocolatey package](https://chocolatey.org/packages/collectw.service)
or using the sc command :
sc.exe create collectw binPath= "PATH_TO_COLLECTW_SERVICE\collectw.service.exe" DisplayName= "CollectW"

The service will need a config.json file in the same directory of the exe file.
The configuration file will be monitored and any changes on it will reconfigure the service.

###A configuration that sends info to statsD

```javascript
{
    "CounterDefinition": {
        "Type": "ConfigFileDefinitions",
        "Configuration": [
            {

                "CategoryName": "Processor",
                "CounterName": "\/.*\/",
                "InstanceName": "_Total",
                "CollectInterval": 5000
            }
        ]

    },
    "Sinks": [
        {
            "Type": "StatsDSink",
            "Configuration": {
                "Host": "localhost",
                "Port": 8125,
                "MaxUdpPacket": 512,
                "CounterTypeMaps": [
                    {
                        "Regex": ".*",
                        "Type": "Gauge"
                    }
                ]
            }
        }
    ]
}
```
####Counter Definition
#####Type
  The Type of the supplier that will be used to supply counter definitions to the collector, in this case we are using the internal ConfigFileDefinitions class.
#####Configuration
  The schema of the object supplied in this property is determined by the class being used, in this case we have a list of CounterDefinitions.
  The CategoryName, CounterName and Instance Name can have a RegularExpression that will be used to expand the counters being collected. in the example above i'm reading all counters from category Processor for the Instance _Total. Be aware that the regular expression must be informed in the javascript format /.*/ but you must escape it inside the json so it becomes \/.*\/

####Sinks
  It's a list of sinks to wich the collector will forward the values collected
  For each sink we will have the Type and Configuration as explained above, the type will tell wich class will be used and the configuration will be an object specific to the sink that contains all information it needs to work.
  In the above example we are using the StatsDSink Class that ships with collectw. in the configuration object we have:
  * Host is the ip address or hostname of the statsD server
  * Port the port that the statsD server are listening to
  * MaxUdpPacket is the maximum size of the UDP packet
  * CounterTypemaps is a list of Maps containing a regular expression and a type. in this case we are saying that every counter is of type gauge. in this property the regex don't need to be informed in the javascript format.
