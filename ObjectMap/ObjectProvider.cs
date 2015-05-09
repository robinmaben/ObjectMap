using System;

namespace ObjectMap
{
    public class ObjectProvider
    {
        private readonly Func<object> _instancePath;
        private object _instance;

        public LifecycleOptions LifecycleOptions { get; internal set; }

        public ObjectProvider(Func<object> instancePath)
        {
            _instancePath = instancePath;
            LifecycleOptions = LifecycleOptions.LastCreated;
        }

        public object GetInstance()
        {
            switch (LifecycleOptions)
            {
                case LifecycleOptions.PerRequest:
                    return _instancePath();

                case LifecycleOptions.LastCreated:
                case LifecycleOptions.Singleton:
                    return _instance ?? (_instance = _instancePath());

                default:
                    return _instance;
            }
        }
    }
}