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
            ObjectMap.InjectAllPropertiesofType<IDependency>();

            var result = ObjectMap.Get<IMock2>();
            Assert.IsInstanceOfType(result, typeof(Mock2));
            Assert.IsNull(result.Dependency);

            ObjectMap.Register<IDependency, Dependency>();
            result.EnsureDependenciesInjected();
            
            Assert.IsNotNull(result.Dependency);
        }

        [TestMethod]
        public void TestLifecycleOptionsLastCreated()
        {
            
        }

        [TestMethod]
        public void TestLifecycleOptionsPerRequest()
        {

        }

        [TestMethod]
        public void TestLifecycleOptionsSingleton()
        {

        }
    }
}