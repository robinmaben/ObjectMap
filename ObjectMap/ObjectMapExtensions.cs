using System;

namespace ObjectMap
{
    public static class ObjectMapExtensions
    {
        public static void TryInjectDependencies(this object @object)
        {
            ObjectMap.TryInjectDependencies(@object);
        }

        public static void Singleton(this Type type)
        {
            ObjectProvider provider;
            if (ObjectMap.Instance.ProviderRegistry.TryGetValue(type, out provider))
            {
                provider.LifecycleOptions = LifecycleOptions.Singleton;
            }
        }

        public static void PerRequest(this Type type)
        {
            ObjectProvider provider;
            if (ObjectMap.Instance.ProviderRegistry.TryGetValue(type, out provider))
            {
                provider.LifecycleOptions = LifecycleOptions.PerRequest;
            }
        }
    }
}