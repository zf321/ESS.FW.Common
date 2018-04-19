using System;
using ESS.FW.Common.Components;
using NUnit.Framework;

namespace ESS.FW.Common.Tests.Components
{
    [TestFixture]
    public class When_ExceptionHandlingInterceptor 
    {
        [Test]
        public void TestExceptionHandlingInterceptor()
        {
            using (var builder = TestContainerBuilder.ConstructBuilder())
            {
                builder.RegisterType<LogComponent>();
                var obj = builder.Resolve<IInterceptor>();
                
                Assert.Throws<Exception>(() => obj.PublicMethod());
            }

        }
        [Test]
        public void TestExceptionHandlingInterceptorHeighCpuUsage()
        {
            using (var builder = TestContainerBuilder.ConstructBuilder())
            {
                builder.RegisterComponent<LogComponent>(LifeStyle.Transient,Interceptor.ExceptionHandle);
                var obj = builder.Resolve<IInterceptor>();

                for (int i = 0; i < 1000; i++)
                {
                    try
                    {

                        obj.PublicMethod();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    
                }
            }

        }

        public interface IInterceptor
        {
            int PublicMethod();
        }

        [Component(LifeStyle.Transient,Interceptor.ExceptionHandle)]
        public class LogComponent : IInterceptor
        {
            public int PublicMethod()
            {
                throw new Exception(" 测试异常发生了 ！ ");
                return 10;
            }
        }


    }
}
