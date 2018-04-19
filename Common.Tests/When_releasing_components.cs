using System;
using ESS.FW.Common.Components;
using NUnit.Framework;

namespace ESS.FW.Common.Tests.Components
{

    [TestFixture]
    public class When_releasing_components
    {
        [Test]
        public void Transient_component_should_be_destructed_called()
        {
            using (var builder = TestContainerBuilder.ConstructBuilder())
            {
                builder.RegisterType(typeof(TransientClass), LifeStyle.Transient);
                
                using (var comp = builder.Resolve<TransientClass>())
                {

                    comp.Name = "Jon";
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();

                Assert.IsTrue(TransientClass.Destructed);
            }

        }

        public class TransientClass : IDisposable
        {
            public static bool Destructed;

            public string Name { get; set; }


            public void Dispose()
            {
                Destructed = true;
            }
        }
    }
}