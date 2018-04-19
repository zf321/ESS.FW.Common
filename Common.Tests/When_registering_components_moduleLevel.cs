using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Common.Components;
using NUnit.Framework;

namespace ESS.FW.Common.Tests.Components
{

    [TestFixture]
    public class When_registering_components_moduleLevel
    {
        [Test]
        public void Component_should_be_load_at_moduleLevel_all()
        {
            using (var builder = TestContainerBuilder.ConstructBuilder())
            {
                var moduleLevel = ModuleLevel.All;
                ObjectContainer.RegisterComponent<CoreComponent>(moduleLevel);
                ObjectContainer.RegisterComponent<NormalComponent>(moduleLevel);
                ObjectContainer.RegisterComponent<ThirdPartComponent>(moduleLevel);

                Assert.IsTrue(ObjectContainer.IsRegistered<CoreComponent>());

                Assert.IsTrue(ObjectContainer.IsRegistered<NormalComponent>());

                Assert.IsTrue(ObjectContainer.IsRegistered<ThirdPartComponent>());
            }

        }
        [Test]
        public void Component_should_be_load_at_moduleLevel_normal()
        {
            using (var builder = TestContainerBuilder.ConstructBuilder())
            {
                var moduleLevel = ModuleLevel.Normal;
                ObjectContainer.RegisterComponent<CoreComponent>(moduleLevel);
                ObjectContainer.RegisterComponent<NormalComponent>(moduleLevel);
                ObjectContainer.RegisterComponent<ThirdPartComponent>(moduleLevel);

                Assert.IsTrue(builder.IsRegistered<CoreComponent>());

                Assert.IsTrue(builder.IsRegistered<NormalComponent>());

                Assert.IsFalse(builder.IsRegistered<ThirdPartComponent>());
            }

        }
        [Test]
        public void Component_should_be_load_at_moduleLevel_core()
        {
            using (var builder = TestContainerBuilder.ConstructBuilder())
            {
                var moduleLevel = ModuleLevel.Core;
                ObjectContainer.RegisterComponent<CoreComponent>(moduleLevel);
                ObjectContainer.RegisterComponent<NormalComponent>(moduleLevel);
                ObjectContainer.RegisterComponent<ThirdPartComponent>(moduleLevel);

                Assert.IsTrue(builder.IsRegistered<CoreComponent>());

                Assert.IsFalse(builder.IsRegistered<NormalComponent>());

                Assert.IsFalse(builder.IsRegistered<ThirdPartComponent>());
            }

        }


        [Component(LifeStyle.Transient,ModuleLevel.Core)]
        public class CoreComponent
        {

        }

        [Component(LifeStyle.Transient, ModuleLevel.Normal)]
        public class NormalComponent

        {

        }

        [Component(LifeStyle.Transient, ModuleLevel.Third)]
        public class ThirdPartComponent
        {

        }

    }

}