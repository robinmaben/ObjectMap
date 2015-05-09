using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ObjectMap.Tests
{
    [TestClass]
    public class BasicTests
    {
        [TestMethod]
        public void WithoutDependency()
        {
            ObjectMap.Register<IMock, Mock>();
            var result = ObjectMap.Get<IMock>();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof (Mock));
        }

        [TestMethod]
        public void WithCtorDependencies()
        {
            ObjectMap.Register<IMock, Mock>();
            ObjectMap.Register<IMock2, Mock2>();

            var result = ObjectMap.Get<IMock2>();

            Assert.IsInstanceOfType(result, typeof (Mock2));
        }

        [TestMethod]
        public void WithDependentProperties()
        {
            ObjectMap.Register<IMock, Mock>();
            ObjectMap.Register<IMock2, Mock2>();

            ObjectMap.Register<IDependency, Dependency>().InjectAllPropertiesofType();
            ObjectMap.InjectAllPropertiesofType<IDependency>();

            var result = ObjectMap.Get<IMock2>();

            Assert.IsInstanceOfType(result, typeof(Mock2));
            Assert.IsNotNull(result.Dependency);
        }
    }
}
