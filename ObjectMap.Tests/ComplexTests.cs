using NUnit.Framework;

namespace ObjectMap.Tests
{
    [TestFixture]
    public class ComplexTests
    {
        [Test]
        public void WithLateRegsiteredDependencies()
        {
            ObjectMap.Register<IMock, Mock>();
            ObjectMap.Register<IMock2, Mock2>();

            var result = ObjectMap.Get<IMock2>();
            Assert.IsInstanceOf<Mock2>(result);
            Assert.IsNull(result.Dependency);

            ObjectMap.Register<IDependency, Dependency>();
            result.TryInjectDependencies();
            
            Assert.IsNotNull(result.Dependency);
        }

        [Test]
        public void TestLifecycleOptionsPerRequest()
        {
            ObjectMap.Register<IMock, Mock>().PerRequest();

            var result = ObjectMap.Get<IMock>();
            var result2 = ObjectMap.Get<IMock>();

            Assert.AreNotSame(result2, result);
        }

        [Test]
        public void TestLifecycleOptionsSingleton()
        {
            ObjectMap.Register<IMock, Mock>().Singleton();

            var result = ObjectMap.Get<IMock>();
            var result2 = ObjectMap.Get<IMock>();

            Assert.AreSame(result2, result);
        }

        [Test]
        public void TestLifecycleOptionsPerRequestDependency()
        {
            ObjectMap.Register<IMock, Mock>();
            ObjectMap.Register<IMock2, Mock2>().PerRequest(); //The owner objects itself must be different
            

            ObjectMap.Register<IDependency, Dependency>().PerRequest();

            var result = ObjectMap.Get<IMock2>();
            var result2 = ObjectMap.Get<IMock2>();

            Assert.AreNotSame(result2.Dependency, result.Dependency);
        }

        [Test]
        public void TestLifecycleOptionsSingletonDependency()
        {
            ObjectMap.Register<IMock, Mock>();
            ObjectMap.Register<IMock2, Mock2>().PerRequest();//The owner objects itself must be different

            ObjectMap.Register<IDependency, Dependency>().Singleton();

            var result = ObjectMap.Get<IMock2>();
            var result2 = ObjectMap.Get<IMock2>();

            Assert.AreSame(result2.Dependency, result.Dependency);
        }

        [Test]
        public void TestCyclicDependecyAvertedSimple()
        {
            ObjectMap.Register<CyclicDependency1, CyclicDependency1>().PerRequest();

            var instance1 = ObjectMap.Get<CyclicDependency1>();

            Assert.IsNotNull(instance1);
            Assert.IsNull(instance1.SelfDependency);
        }

        [Test]
        public void TestCyclicDependecyAverted()
        {
            ObjectMap.Register<CyclicDependency1, CyclicDependency1>().PerRequest();
            ObjectMap.Register<CyclicDependency2, CyclicDependency2>().PerRequest();

            var instance1 = ObjectMap.Get<CyclicDependency1>();

            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance1.Dependency);

            var instance2 = ObjectMap.Get<CyclicDependency1>();

            Assert.IsNotNull(instance2);
            Assert.IsNotNull(instance2.Dependency);
        }
    }
}