using System;

namespace ObjectMap
{
    [Flags]
    public enum LifecycleOptions
    {
        LastCreated,
        PerRequest,
        Singleton
    }
}