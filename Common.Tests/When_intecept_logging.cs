using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESS.FW.Common.Components;
using NUnit.Framework;

namespace ESS.FW.Common.Tests.Components
{
    [TestFixture]
    public class When_intecept_logging
    {
        [Test]
        public void Component_should_be_log_by_logInterceptor()
        {
            using (var builder = TestContainerBuilder.ConstructBuilder())
            {
                //builder.RegisterComponent<LogComponent>();
                //var obj = builder.Resolve<IPublicInterface>();
                //obj.PublicMethod();
            }
        }
    }
    
    public interface IPublicInterface
    {
        int PublicMethod();
    }

    [Component(LifeStyle.Transient, ModuleLevel.Normal, Interceptor.Log)]
    public class LogComponent : IPublicInterface
    {
        public int PublicMethod()
        {
            return 10;
        }
    }
}
