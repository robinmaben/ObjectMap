using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ObjectMap.Tests
{
    [TestClass]
    public class UnitTests
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

            ObjectMap.InjectAllPropertiesofType<IDependency, Dependency>();

            var result = ObjectMap.Get<IMock2>();

            Assert.IsInstanceOfType(result, typeof(Mock2));
            Assert.IsNotNull(result.Dependency);
        }
    }

    public class Mock2 : IMock2
    {
        public IMock Mock { get; set; }
        public IDependency Dependency { get; set; }

        public Mock2(IMock mock)
        {
            Mock = mock;
        }
    }

    public interface IMock2
    {
        IDependency Dependency { get; set; }
    }

    public class Mock : IMock
    {
    
    }

    public class Dependency : IDependency
    {
        
    }

    public interface IMock
    {
    }

    public interface IDependency
    {
    }

    
}
