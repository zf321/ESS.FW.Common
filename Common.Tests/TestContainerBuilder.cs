using ESS.FW.Common.Autofac;
using ESS.FW.Common.Components;
using ESS.FW.Common.ServiceBus;

namespace ESS.FW.Common.Tests.Components
{

    public class TestContainerBuilder
    {
        
        public static AutofacObjectContainer ConstructBuilder()
        {
            Configurations.Configuration.Create();
            AutofacObjectContainer container = new AutofacObjectContainer();
            ObjectContainer.SetContainer(container);
            container.Register<IBus, EmptyBus>();
            return container;
        }
        

    }
}