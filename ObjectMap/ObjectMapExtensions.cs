using System;

namespace ObjectMap
{
    public static class ObjectMapExtensions
    {
        public static Type InjectAllPropertiesofType(this Type injectableType)
        {
            ObjectMap.Instance.InjectablesRegistry.Add(injectableType);
            return injectableType;
        }

        public static void EnsureDependenciesInjected(this object @object)
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