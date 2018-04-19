using System;
using System.Diagnostics;
using ESS.FW.Common.Components;
using NUnit.Framework;

namespace ESS.FW.Common.Tests.Components
{

    [TestFixture]
    public class When_disposing_the_builder
    {
        [Test]
        public void Should_dispose_all_IDisposable_components()
        {
            var builder = TestContainerBuilder.ConstructBuilder();
            DisposableComponent.DisposeCalled = false;
            AnotherSingletonComponent.DisposeCalled = false;

            builder.RegisterType(typeof(DisposableComponent), LifeStyle.Singleton);
            builder.Register<AnotherSingletonComponent, AnotherSingletonComponent>();

            builder.Resolve(typeof(DisposableComponent));
            builder.Resolve(typeof(AnotherSingletonComponent));
            builder.Dispose();

            Assert.True(DisposableComponent.DisposeCalled, "Dispose should be called on DisposableComponent");
            Assert.True(AnotherSingletonComponent.DisposeCalled, "Dispose should be called on AnotherSingletonComponent");
        }
        
        

        public class DisposableComponent : IDisposable
        {
            public static bool DisposeCalled;

            public void Dispose()
            {
                DisposeCalled = true;
            }
        }

        public class AnotherSingletonComponent : IDisposable
        {
            public static bool DisposeCalled;

            public void Dispose()
            {
                DisposeCalled = true;
            }
        }
    }
}