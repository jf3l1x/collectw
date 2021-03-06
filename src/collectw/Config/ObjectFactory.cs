﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Collectw.Logging;
using CollectW.Services;
using CollectW.Sinks;
using CollectW.Sinks.StatsD;

namespace CollectW.Config
{
    public class ObjectFactory
    {
        private static readonly ILog Logger = LogProvider.For<ObjectFactory>();

        private static readonly List<Type> Sinks = new List<Type>
        {
            typeof (FileSink),
            typeof (ConsoleSink),
            typeof (StatsDSink)
        };

        private static readonly List<Type> Suppliers = new List<Type> {typeof (ConfigFileDefinitions)};

        static ObjectFactory()
        {
            FindImplementations();
        }

        private static void FindImplementations()
        {
            Traverse(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "modules"));
        }

        private static void Traverse(string path)
        {
            if (Directory.Exists(path))
            {
                foreach (string dir in Directory.EnumerateDirectories(path))
                {
                    Traverse(dir);
                }
                foreach (string module in Directory.EnumerateFiles(path, "*.dll"))
                {
                    Assembly assembly = Assembly.LoadFile(Path.GetFullPath(module));
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.IsPublic)
                        {
                            //Do we have a default constructor
                            if (type.GetConstructor(Type.EmptyTypes) != null)
                            {
                                if (type.GetInterface("CollectW.Services.ISendInfo") != null)
                                {
                                    Sinks.Add(type);
                                }
                                if (type.GetInterface("CollectW.Services.ISupplyCounterDefinitions") != null)
                                {
                                    Suppliers.Add(type);
                                }
                            }
                        }
                    }
                }
            }
            
        }

        public static ISendInfo CreateSink(dynamic typeName, dynamic configuration)
        {
            try
            {
                Logger.Debug("CreateSink");
                Logger.DebugFormat("trying to create a sink of type {@typeName} with configuration {@configuration}", (object)typeName.ToString(), (object)configuration.ToString());
                var type = FindType(typeName.ToString(),Sinks);
                if (type == null)
                {
                    Logger.ErrorFormat(
                        "could not find any type with name : {@typeName} implementing ISendInfo interface. Make sure the assembly is under the modules directory and that the class implements the ISendInfo interface!",
                        (object)typeName.ToString());
                    throw new ArgumentException("typeName");
                }
                var retval = (ISendInfo) Activator.CreateInstance(type);
                retval.Configure(configuration);
                Logger.DebugFormat("sink {@typeName} created!", (object)typeName.ToString());
                return retval;
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("error instantiating {@typeName} : {@exception}", (object)typeName.ToString(), ex);
                throw;
            }
        }

        private static Type FindType(string typeName,IEnumerable<Type> source )
        {
            Type type =
                source.FirstOrDefault(
                    s =>
                        string.Equals(typeName, s.FullName, StringComparison.InvariantCultureIgnoreCase)) ??
                source.FirstOrDefault(
                            s =>
                                string.Equals(typeName, s.Name, StringComparison.InvariantCultureIgnoreCase));

            return type;
        }

        public static ISupplyCounterDefinitions CreateDefinitionsSupplier(dynamic typeName, dynamic configuration)
        {
            try
            {
                var type = FindType(typeName.ToString(), Suppliers);
                if (type == null)
                {
                    Logger.ErrorFormat(
                        "could not find any type with name : {@typeName} implementing ISupplyCounterDefinitions interface. Make sure the assembly is under the modules directory and that the class implements the ISendInfo interface!",
                        (object)typeName.ToString());
                    throw new ArgumentException("typeName");
                }
                var retval = (ISupplyCounterDefinitions)Activator.CreateInstance(type);
                retval.Configure(configuration);
                return retval;
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("error instantiating {@typeName} : {@exception}", (object)typeName.ToString(), ex);
                throw;
            }
        }
    }
}