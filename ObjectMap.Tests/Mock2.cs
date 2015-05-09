namespace ObjectMap.Tests
{
    public class Mock2 : IMock2
    {
        public IMock Mock { get; set; }
        public IDependency Dependency { get; set; }

        public Mock2(IMock mock)
        {
            Mock = mock;
        }
    }
}