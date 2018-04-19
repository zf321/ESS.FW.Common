using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Autofac;
using ESS.FW.Common.Components;
using NUnit.Framework;
using Autofac.Features.AttributeFilters;
using Autofac.Features.Metadata;
using System.Linq;
using System.ComponentModel.Composition;

namespace ESS.FW.Common.Tests.Components
{

    [TestFixture]
    public class When_building_components
    {
        [Test]
        public void Singleton_components_should_get_their_dependencies_autowired()
        {
            using (var builder = TestContainerBuilder.ConstructBuilder())
            {
                builder.Register<ISingletonComponentWithPropertyDependency, SingletonComponentWithPropertyDependency>();
                builder.Register<SingletonComponent, SingletonComponent>();

                var singleton = (SingletonComponentWithPropertyDependency)builder.Resolve<ISingletonComponentWithPropertyDependency>();
                Assert.IsNotNull(singleton.Dependency);
            }
        }

        [Test]
        public void Singleton_components_should_yield_the_same_instance()
        {
            using (var builder = TestContainerBuilder.ConstructBuilder())
            {
                InitializeBuilder(builder);

                Assert.AreEqual(builder.Resolve<IComponent>(), builder.Resolve<IComponent>());
                Assert.AreEqual(builder.Resolve<IComponent>().Now, builder.Resolve<IComponent>().Now);
            }
        }

        [Test]
        public void Singlecall_components_should_yield_unique_instances()
        {
            using (var builder = TestContainerBuilder.ConstructBuilder())
            {
                InitializeBuilder(builder);
                Assert.AreNotEqual(builder.Resolve<SinglecallComponent>(), builder.Resolve<SinglecallComponent>());
            }
        }
        
        

        [Test]
        public void Requesting_an_unregistered_component_should_throw()
        {
            using (var builder = TestContainerBuilder.ConstructBuilder())
            {
                InitializeBuilder(builder);
                Assert.That(() => builder.Resolve<UnregisteredComponent>(), Throws.Exception);
            }
        }

        [Test]
        public void Should_be_able_to_build_components_registered_after_first_build()
        {
            using (var builder = TestContainerBuilder.ConstructBuilder())
            {
                InitializeBuilder(builder);
                builder.Resolve <SingletonComponent>();
                builder.RegisterType(typeof(UnregisteredComponent), LifeStyle.Singleton);

                var unregisteredComponent = builder.Resolve<UnregisteredComponent>();
                Assert.NotNull(unregisteredComponent);
                Assert.NotNull(unregisteredComponent.SingletonComponent);
            }
            //Not supported by,typeof(SpringObjectBuilder));
        }

        [Test]
        public void Should_support_mixed_dependency_styles()
        {
            using (var builder = TestContainerBuilder.ConstructBuilder())
            {
                InitializeBuilder(builder);
                builder.RegisterType(typeof(ComponentWithBothConstructorAndSetterInjection), LifeStyle.Transient);
                builder.RegisterType(typeof(ConstructorDependency), LifeStyle.Transient);
                builder.RegisterType(typeof(SetterDependency), LifeStyle.Transient);

                var component = builder.Resolve<ComponentWithBothConstructorAndSetterInjection>();
                Assert.NotNull(component.ConstructorDependency);
                Assert.NotNull(component.SetterDependency);
            }
            
        }
        [Test]
        public void Should_support_meta()
        {
            using (var builder = TestContainerBuilder.ConstructBuilder())
            {
                InitializeBuilder(builder);
                builder.RegisterType(typeof(BpmMeta), LifeStyle.Transient);
                builder.RegisterType(typeof(ErpMeta), LifeStyle.Transient);
                builder.RegisterType(typeof(MetaClass), LifeStyle.Transient);
                builder.RegisterType(typeof(DefaultMeta), LifeStyle.Transient);
                
                var component = builder.Resolve<IMetaClass>();
                Assert.NotNull(component.Bpm);
                Assert.That(component.Bpm,Is.InstanceOf(typeof(BpmMeta)));
                Assert.NotNull(component.Erp);
                Assert.That(component.Erp, Is.InstanceOf(typeof(ErpMeta)));
                Assert.NotNull(component.Default);
                Assert.That(component.Default, Is.InstanceOf(typeof(DefaultMeta)));

                List<Meta<IMetaInterface>> comps = builder.Resolve<IEnumerable<Meta<IMetaInterface>>>().ToList();
                Assert.AreEqual(3, comps.Count());
                Assert.True(comps.Any(c => c.Metadata.Any(d => d.Key == "Type" && d.Value is DbTypes.Erp)));
            }
            
        }
        [Test]
        public void Resovle_MemoryUsage_test()
        {
            using (var builder = TestContainerBuilder.ConstructBuilder())
            {
                builder.RegisterType(typeof(SinglecallComponent), LifeStyle.Transient);

                var process = Process.GetCurrentProcess();
                var startMemoryUsage = process.WorkingSet64;
                for (int i = 0; i < 10000; i++)
                {
                    var comp = builder.Resolve<SinglecallComponent>();
                }
                Debug.WriteLine((process.WorkingSet64 - startMemoryUsage) / 1024 + " kb");
            }
        }


        void InitializeBuilder(IObjectContainer container)
        {
            container.Register<IComponent, SingletonComponent>(LifeStyle.Singleton);
            container.RegisterType(typeof(SingletonComponent), LifeStyle.Singleton);
            container.RegisterType(typeof(SinglecallComponent), LifeStyle.Transient);
        }
        public interface IComponent
        {
            DateTime Now { get; set; }
        }
        public class SingletonComponent: IComponent
        {
            public DateTime Now { get;  set; }

            public SingletonComponent()
            {
                Now = DateTime.Now;
            }
        }


        public interface ISingletonComponentWithPropertyDependency
        {

        }

        public class SingletonComponentWithPropertyDependency : ISingletonComponentWithPropertyDependency
        {
            public SingletonComponent Dependency { get; set; }

            public SingletonComponentWithPropertyDependency(SingletonComponent dependency)
            {
                Dependency = dependency;
            }
        }

        public class SinglecallComponent
        {
        }

        public class UnregisteredComponent
        {
            public SingletonComponent SingletonComponent { get; set; }
        }
        
    }

    public class StaticFactory
    {
        public ComponentCreatedByFactory Create()
        {
            return new ComponentCreatedByFactory();
        }
    }

    public class ComponentCreatedByFactory
    {
    }

    public class ComponentWithBothConstructorAndSetterInjection
    {
        public ComponentWithBothConstructorAndSetterInjection(ConstructorDependency constructorDependency)
        {
            ConstructorDependency = constructorDependency;
        }

        public ConstructorDependency ConstructorDependency { get; set; }

        public SetterDependency SetterDependency { get; set; }
    }

    public class ConstructorDependency
    {
    }

    public class SetterDependency
    {
    }

    public interface IMetaInterface
    {
        
    }

    [DbTypeMetadata(DbTypes.Bpm)]
    public class BpmMeta:IMetaInterface
    {
    }

    [DbTypeMetadata(DbTypes.Erp)]
    public class ErpMeta : IMetaInterface
    {
    }
    public class DefaultMeta : IMetaInterface
    {
    }
    public interface IMetaClass
    {
        IMetaInterface Default { get; set; }
        IMetaInterface Bpm { get; set; }
         IMetaInterface Erp { get; set; }
    }
    public class MetaClass : IMetaClass
    {
        public IMetaInterface Default { get; set; }
        public IMetaInterface Bpm { get; set; }
        public IMetaInterface Erp { get; set; }
        public MetaClass(IMetaInterface @default,[MetadataFilter("Type", DbTypes.Bpm)]IMetaInterface bpm, [MetadataFilter("Type", DbTypes.Erp)]IMetaInterface erp)
        {
            Default = @default;
            Bpm = bpm;
            Erp = erp;
        }
    }
    
    public enum DbTypes
    {
        /// <summary>
        /// 
        /// </summary>
        Erp,
        /// <summary>
        /// 
        /// </summary>
        Bpm
    }
    
    [MetadataAttribute]
    public class DbTypeMetadataAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public DbTypes @Type { get;  set; } = DbTypes.Erp;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public DbTypeMetadataAttribute(DbTypes type = DbTypes.Erp)
        {
            @Type = type;
        }
    }
}