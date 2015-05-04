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
            ObjectMap.Register<IMock, Mock2>();
            ObjectMap.Register<IDependency, Dependency>();

            var result = ObjectMap.Get<IMock>();

            Assert.IsInstanceOfType(result, typeof (Mock2));
        }
    }

    public class Mock2 : IMock
    {
        public IDependency Dependency { get; set; }

        public Mock2(IDependency dependency)
        {
            Dependency = dependency;
        }
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
