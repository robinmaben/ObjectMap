namespace ObjectMap.Tests
{
    public class CyclicDependency1
    {
        public CyclicDependency1 SelfDependency { get; set; }
        public CyclicDependency2 Dependency { get; set; }
    }

    public class CyclicDependency2
    {
        public CyclicDependency1 CyclicDependency1 { get; set; }
    }
}
