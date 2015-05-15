using NUnit.Framework;

namespace ObjectMap.Tests
{
    [TestFixture]
    public class BasicTests
    {
        [Test]
        public void WithoutDependency()
        {
            ObjectMap.Register<IMock, Mock>();
            var result = ObjectMap.Get<IMock>();

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Mock>(result);
        }

        [Test]
        public void WithCtorDependencies()
        {
            ObjectMap.Register<IMock, Mock>();
            ObjectMap.Register<IMock2, Mock2>();

            var result = ObjectMap.Get<IMock2>();

            Assert.IsInstanceOf<Mock2>(result);
        }

        [Test]
        public void WithDependentProperties()
        {
            ObjectMap.Register<IMock, Mock>();
            ObjectMap.Register<IMock2, Mock2>();
            ObjectMap.Register<IDependency, Dependency>();
            
            var result = ObjectMap.Get<IMock2>();

            Assert.IsInstanceOf<Mock2>(result);
            Assert.IsNotNull(result.Dependency);
        }
    }
}
