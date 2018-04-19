using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Common.Components;
using NUnit.Framework;

namespace ESS.FW.Common.Tests.Components
{

    [TestFixture]
    public class When_registering_components
    {


        [Test]
        public void A_registration_should_be_allowed_to_be_updated()
        {
            using (var builder = TestContainerBuilder.ConstructBuilder())
            {
                builder.Register<ISingletonComponent, SingletonComponent>();
                builder.Register<ISingletonComponent, AnotherSingletonComponent>();

                Assert.IsInstanceOf<AnotherSingletonComponent>(builder.Resolve(typeof(ISingletonComponent)));
            }

            //Not supported by, typeof(SpringObjectBuilder));
        }

        [Test]
        public void A_registration_should_update_default_component_for_interface()
        {
            using (var builder = TestContainerBuilder.ConstructBuilder())
            {
                builder.RegisterType(typeof(SomeClass), LifeStyle.Transient);
                builder.RegisterType(typeof(SomeOtherClass), LifeStyle.Transient);

                Assert.IsNotInstanceOf<SomeClass>(builder.Resolve(typeof(ISomeInterface)));
                Assert.IsInstanceOf<SomeOtherClass>(builder.Resolve(typeof(ISomeInterface)));
            }
        }

        [Test]
        public void Register_singleton_should_be_supported()
        {
            using (var builder = TestContainerBuilder.ConstructBuilder())
            {
                var singleton = new SingletonComponent();
                builder.RegisterInstance<ISingletonComponent, SingletonComponent>(singleton);
                Assert.AreEqual(builder.Resolve<ISingletonComponent>(), singleton);
            }
        }

        [Test]
        public void Registering_the_same_singleton_for_different_interfaces_should_be_supported()
        {
            using (var builder = TestContainerBuilder.ConstructBuilder())
            {
                var singleton = new SingletonThatImplementsToInterfaces();
                builder.RegisterInstance<ISingleton1, SingletonThatImplementsToInterfaces>(singleton);
                builder.RegisterInstance<ISingleton2, SingletonThatImplementsToInterfaces>(singleton);

                builder.RegisterType(typeof(ComponentThatDependsOnMultiSingletons), LifeStyle.Transient);

                var dependency = (ComponentThatDependsOnMultiSingletons)builder.Resolve(typeof(ComponentThatDependsOnMultiSingletons));

                Assert.NotNull(dependency.Singleton1);
                Assert.NotNull(dependency.Singleton2);

                Assert.AreEqual(builder.Resolve(typeof(ISingleton1)), singleton);
                Assert.AreEqual(builder.Resolve(typeof(ISingleton2)), singleton);
            }

            //Not supported by,typeof(SpringObjectBuilder));
        }


        [Test]
        public void Setter_dependencies_should_be_supported_when_resolving_interfaces()
        {
            using (var builder = TestContainerBuilder.ConstructBuilder())
            {
                builder.RegisterType(typeof(SomeClass), LifeStyle.Transient);
                builder.Register<IWithSetterDependencies, ClassWithSetterDependencies>();

                builder.Register<ISomeInterface, SomeClass>();

                var component = (ClassWithSetterDependencies)builder.Resolve(typeof(IWithSetterDependencies));

          
                Assert.NotNull(component.ConcreteDependency, "Concrete classed should be property injected");
                Assert.NotNull(component.InterfaceDependency, "Interfaces should be property injected");
                Assert.NotNull(component.concreteDependencyWithSetOnly, "Set only properties should be supported");
            }
        }



        [Test]
        public void Concrete_classes_should_get_the_same_lifecycle_as_their_interfaces()
        {
            using (var builder = TestContainerBuilder.ConstructBuilder())
            {
                builder.RegisterType(typeof(SingletonComponent), LifeStyle.Singleton);

                var instance = builder.Resolve(typeof(SingletonComponent)) as SingletonComponent;
                builder.RegisterInstance<SingletonComponent, SingletonComponent>(instance);
                builder.RegisterInstance<ISingletonComponent, SingletonComponent>(instance);

                Assert.AreSame(builder.Resolve(typeof(SingletonComponent)), builder.Resolve(typeof(ISingletonComponent)));
            }
        }

        [Test]
        public void All_implemented_interfaces_should_be_registered()
        {
            using (var builder = TestContainerBuilder.ConstructBuilder())
            {
                builder.RegisterType(typeof(ComponentWithMultipleInterfaces),
                    LifeStyle.Transient);

                Assert.IsTrue(builder.IsRegistered<ISomeInterface>());

                Assert.IsTrue(builder.IsRegistered<ISomeOtherInterface>());

                Assert.IsTrue(builder.IsRegistered<IYetAnotherInterface>());
            }
        }

        [Test]
        public void Multiple_implementations_will_be_override()
        {
            using (var builder = TestContainerBuilder.ConstructBuilder())
            {
                builder.RegisterType(typeof(SomeClass), LifeStyle.Transient);
                builder.RegisterType(typeof(SomeOtherClass), LifeStyle.Transient);
                
                Assert.IsTrue(builder.IsRegistered<ISomeInterface>());
                Assert.AreNotSame(builder.Resolve<SomeClass>().GetType(), builder.Resolve<ISomeInterface>().GetType());
                
            }
            //Not supported by,typeof(WindsorObjectBuilder));
        }
        

        [Test]
        public void Generic_interfaces_should_be_registered()
        {
            using (var builder = TestContainerBuilder.ConstructBuilder())
            {
                builder.Register<ISomeGenericInterface<string>, ComponentWithGenericInterface>();
                Assert.NotNull(builder.Resolve(typeof(ISomeGenericInterface<string>)));
            }
        }
        
    }

    public class ComponentThatDependsOnMultiSingletons
    {
        public ComponentThatDependsOnMultiSingletons(ISingleton1 singleton1,ISingleton2 singleton2)
        {
            Singleton1 = singleton1;
            Singleton2 = singleton2;
        }
        public ISingleton1 Singleton1;
        public ISingleton2 Singleton2;
    }

    public class SingletonThatImplementsToInterfaces : ISingleton2
    {
    }

    public interface ISingleton2 : ISingleton1
    {
    }

    public interface ISingleton1
    {
    }

    public class ComponentWithMultipleInterfaces : ISomeInterface, ISomeOtherInterface
    {
    }

    public class ComponentWithGenericInterface : ISomeGenericInterface<string>
    {
    }

    public class ComponentWithSystemInterface : IGrouping<string, string>, IDisposable
    {
        public IEnumerator<string> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string Key
        {
            get { throw new NotImplementedException(); }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public interface ISomeGenericInterface<T>
    {
    }

    public interface ISomeOtherInterface : IYetAnotherInterface
    {
    }

    public interface IYetAnotherInterface
    {
    }

    public class DuplicateClass
    {
        public bool SomeProperty { get; set; }
        public bool AnotherProperty { get; set; }
    }

    public interface IWithSetterDependencies
    {
    }

    public class ClassWithSetterDependencies : IWithSetterDependencies
    {
        public SomeEnum EnumDependency { get; set; }
        public int SimpleDependency { get; set; }
        public string StringDependency { get; set; }
        public ISomeInterface InterfaceDependency { get; set; }
        public SomeClass ConcreteDependency { get; set; }
        public SomeClass ConcreteDependencyWithSetOnly
        {
            set { concreteDependencyWithSetOnly = value; }
        }

        public SomeClass ConcreteDependencyWithPrivateSet { get; private set; }

        public SomeClass concreteDependencyWithSetOnly;
    }

    public class SomeClass : ISomeInterface
    {
    }

    public class InheritedFromSomeClass : SomeClass
    {
    }

    public class SomeOtherClass : ISomeInterface
    {
    }

    public interface ISomeInterface
    {
    }

    public enum SomeEnum
    {
        X
    }

    public class SingletonComponent : ISingletonComponent
    {
    }

    public class AnotherSingletonComponent : ISingletonComponent
    {
    }

    public interface ISingletonComponent
    {
    }
}