using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ESS.FW.Common.Components;
using ESS.FW.Common.IO;
using ESS.FW.Common.Serializing;
using ESS.FW.Common.ServiceBus;
using ESS.FW.Common.Utilities;
using ESS.FW.Common.Utilities.AssenblyScanner;

namespace ESS.FW.Common.Configurations
{
    public class Configuration
    {
        public string RootPath;
        private Configuration()
        {
            RootPath = AppContext.BaseDirectory;
        }
        

        /// <summary>
        ///     Provides the singleton access instance.
        /// </summary>
        public static Configuration Instance { get; private set; }

        public static Configuration Create()
        {
            if (Instance != null)
            {
                throw new Exception("Could not create configuration instance twice.");
            }
            Instance = new Configuration();
            return Instance;
        }

        public Configuration SetDefault<TService, TImplementer>(LifeStyle life = LifeStyle.Singleton)
            where TService : class
            where TImplementer : class, TService
        {
            ObjectContainer.Register<TService, TImplementer>(life);
            return this;
        }

        public Configuration SetDefault<TService, TImplementer>(TImplementer instance)
            where TService : class
            where TImplementer : class, TService
        {
            ObjectContainer.RegisterInstance<TService, TImplementer>(instance);
            return this;
        }

        public Configuration RegisterCommonComponents()
        {
            SetDefault<IBinarySerializer, DefaultBinarySerializer>();
            //SetDefault<ICacheFactory, MemoryCacheFactory>();
            SetDefault<IBus, EmptyBus>();
            SetDefault<IJsonSerializer, NewtonsoftJsonSerializer>();
            //SetDefault<IIgnite, DefaultIgnite>();
            ObjectContainer.Current.RegisterGeneric(typeof (IRequestClient<,>), typeof (EmptyRequestClient<,>));
            SetDefault<IOHelper, IOHelper>();
            
            return this;
        }

        /// <summary>
        ///     Register all the components from the given assemblies.
        /// </summary>
        public Configuration RegisterComponents(Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                try
                {
                    foreach (var type in assembly.GetTypes().Where(TypeUtils.IsComponent))
                    {
                        ObjectContainer.RegisterComponent(type);
                    }
                }
                catch (ReflectionTypeLoadException loadException)
                {
                    //目前写死判断，少dll报错，多dll跳过
                    var message = string.Join(",", loadException.LoaderExceptions.Select(c => c.Message).Distinct());
                    if (message.Contains("系统找不到指定的文件"))
                    {
                        throw new ApplicationException(message);
                    }
                }
            }
            return this;
        }

        /// <summary>
        ///     Register all the components.
        /// </summary>
        public Configuration RegisterComponents()
        {
            var assemblies = GetAssembliesInDirectory(RootPath);
            RegisterComponents(assemblies.ToArray());
            return this;
        }
        public Configuration RegisterComponents(Func<string,bool> filter)
        {
            var assemblies = GetAssembliesInDirectory(RootPath, filter);
            RegisterComponents(assemblies.ToArray());
            return this;
        }
        public Configuration RegisterComponents(Func<string, bool> filter, params string[] assembliesToSkip)
        {
            var assemblies = GetAssembliesInDirectory(RootPath, filter, assembliesToSkip);
            RegisterComponents(assemblies.ToArray());
            return this;
        }

        private IEnumerable<Assembly> GetAssembliesInDirectory(string path, Func<string, bool> filter = null,params string[] assembliesToSkip)
        {
            var assemblyScanner = new AssemblyScanner(path, filter);
            //assemblyScanner.MustReferenceAtLeastOneAssembly.Add(typeof(IHandleMessages<>).Assembly);
            
            if (assembliesToSkip != null)
            {
                assemblyScanner.AssembliesToSkip = assembliesToSkip.ToList();
            }
            return assemblyScanner
                .GetScannableAssemblies()
                .Assemblies;
        }
    }
}