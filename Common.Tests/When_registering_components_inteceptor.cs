using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ESS.FW.Common.Autofac;
using ESS.FW.Common.Components;
using NUnit.Framework;

namespace ESS.FW.Common.Tests.Components
{
    [TestFixture]
    public class When_registering_components_inteceptor
    {
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

        [Component(LifeStyle.Transient, ModuleLevel.Normal, Interceptor.Measure)]
        public class MeasureComponent : IPublicInterface
        {
            public int PublicMethod()
            {
                Thread.Sleep(100000);
                return 10;
               
            }
        }

        [Component(LifeStyle.Transient, ModuleLevel.Normal, Interceptor.Measure)]
        public class MeasureComponent2 : IPublicInterface
        {
            public int PublicMethod()
            {
                return 10;
            }
        }

        [Component(LifeStyle.Transient, ModuleLevel.Normal, Interceptor.Measure | Interceptor.Log)]
        public class MeasureAndLogComponent : IPublicInterface
        {
            public int PublicMethod()
            {
                return 10;
            }
        }

        public class NoInterceptorComponent : IPublicInterface
        {
            public int PublicMethod()
            {
                return 10;
            }
        }

        [Test]
        [Explicit]
        public void Component_interceptor_performence_test()
        {
            Stopwatch st = new Stopwatch();
            using (var builder = TestContainerBuilder.ConstructBuilder())
            {
                //no interceptor performence test
                ObjectContainer.RegisterComponent<NoInterceptorComponent>();
                int k = 0;
                st.Start();
                Parallel.For(0, 100000, i =>
                {
                    try
                    {
                        IPublicInterface obj = builder.Resolve<IPublicInterface>();
                        int ret = obj.PublicMethod();
                        k++;
                        if (k % 10000 == 0)
                        {
                            Console.WriteLine(k + " -- " + ret);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                });
                Console.WriteLine("no interceptor ={0}", st.Elapsed);

                //MeasureAndLog performence test
                ObjectContainer.RegisterComponent<MeasureAndLogComponent>();
                k = 0;
                st.Restart();
                Parallel.For(0, 100000, i =>
                {
                    try
                    {
                        IPublicInterface obj = builder.Resolve<IPublicInterface>();
                        int ret = obj.PublicMethod();
                        k++;
                        if (k % 10000 == 0)
                        {
                            Console.WriteLine(k + " -- " + ret);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                });
                Console.WriteLine("MeasureAndLog ={0}", st.Elapsed);

                //no ioc performence test 
                k = 0;
                st.Restart();
                Parallel.For(0, 100000, i =>
                {
                    try
                    {
                        int ret = new NoInterceptorComponent().PublicMethod();
                        k++;
                        if (k % 10000 == 0)
                        {
                            Console.WriteLine(k + " -- " + ret);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                });
                Console.WriteLine("no ioc ={0}", st.Elapsed);
            }
        }

        [Test]
        public void Component_should_be_interceptor_by_logInterceptor()
        {
            using (var builder = TestContainerBuilder.ConstructBuilder())
            {
                ObjectContainer.RegisterComponent<LogComponent>();
                IPublicInterface obj = builder.Resolve<IPublicInterface>();
                Assert.AreEqual(11, obj.PublicMethod());
            }
        }

        [Test]
        public void Component_should_be_interceptor_by_measureInterceptor()
        {
            Configurations.Configuration.Create().UseAutofac().RegisterCommonComponents();
            using (var scope = ObjectContainer.BeginLifetimeScope())
            {
                Stopwatch st = new Stopwatch();
                //ObjectContainer.Register<ILoggerFactory, EntLibLoggerFactory>();
                st.Start();
                ObjectContainer.RegisterComponent<MeasureComponent>();
                IPublicInterface obj = scope.Resolve<IPublicInterface>();
                Assert.AreEqual(12, obj.PublicMethod());


                ObjectContainer.RegisterComponent<MeasureComponent2>();
                obj = scope.Resolve<IPublicInterface>();
                Assert.AreEqual(12, obj.PublicMethod());
                st.Stop();
                Debug.WriteLine(st.Elapsed);
            }
        }

        [Test]
        public void Component_should_be_interceptor_by_measureInterceptor_and_logInterceptor()
        {
            if(ObjectContainer.Current==null)
                Configurations.Configuration.Create().UseAutofac().RegisterCommonComponents();
            using (var scope = ObjectContainer.BeginLifetimeScope())
            {
                ObjectContainer.RegisterComponent<MeasureAndLogComponent>();
                IPublicInterface obj = scope.Resolve<IPublicInterface>();
                Assert.AreEqual(13, obj.PublicMethod());
            }
        }
    }
}