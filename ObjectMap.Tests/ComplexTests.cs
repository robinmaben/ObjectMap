using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ObjectMap.Tests
{
    [TestClass]
    public class ComplexTests
    {
        [TestMethod]
        public void WithLateRegsiteredDependencies()
        {
            ObjectMap.Register<IMock, Mock>();
            ObjectMap.Register<IMock2, Mock2>();

            var result = ObjectMap.Get<IMock2>();
            Assert.IsInstanceOfType(result, typeof(Mock2));
            Assert.IsNull(result.Dependency);

            ObjectMap.Register<IDependency, Dependency>();
            result.TryInjectDependencies();
            
            Assert.IsNotNull(result.Dependency);
        }

        [TestMethod]
        public void TestLifecycleOptionsPerRequest()
        {
            ObjectMap.Register<IMock, Mock>().PerRequest();

            var result = ObjectMap.Get<IMock>();
            var result2 = ObjectMap.Get<IMock>();

            Assert.AreNotSame(result2, result);
        }

        [TestMethod]
        public void TestLifecycleOptionsSingleton()
        {
            ObjectMap.Register<IMock, Mock>().Singleton();

            var result = ObjectMap.Get<IMock>();
            var result2 = ObjectMap.Get<IMock>();

            Assert.AreSame(result2, result);
        }

        [TestMethod]
        public void TestLifecycleOptionsPerRequestDependency()
        {
            ObjectMap.Register<IMock, Mock>();
            ObjectMap.Register<IMock2, Mock2>().PerRequest(); //The owner objects itself must be different
            

            ObjectMap.Register<IDependency, Dependency>().PerRequest();

            var result = ObjectMap.Get<IMock2>();
            var result2 = ObjectMap.Get<IMock2>();

            Assert.AreNotSame(result2.Dependency, result.Dependency);
        }

        [TestMethod]
        public void TestLifecycleOptionsSingletonDependency()
        {
            ObjectMap.Register<IMock, Mock>();
            ObjectMap.Register<IMock2, Mock2>().PerRequest();//The owner objects itself must be different

            ObjectMap.Register<IDependency, Dependency>().Singleton();

            var result = ObjectMap.Get<IMock2>();
            var result2 = ObjectMap.Get<IMock2>();

            Assert.AreSame(result2.Dependency, result.Dependency);
        }
    }
}