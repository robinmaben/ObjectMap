using System;
using System.Collections.Generic;

namespace ObjectMap
{
    public class ObjectMap
    {
        private readonly Dictionary<Type, Func<object>> _objectProviders = new Dictionary<Type, Func<object>>();
        
        static ObjectMap()
        {
            Instance = new ObjectMap();
        }

        private static ObjectMap Instance { get; }

        public static IEnumerable<object> Registry => Instance._objectProviders.Values;

        public static void Register<TRequest, TObj>(Func<TObj> provider = null) where TObj : new()
        {
            Instance._objectProviders[typeof (TRequest)] = () => provider ?? (() => new TObj());
        }

        public static void Register<TObj>(TObj obj)
        {
            Instance._objectProviders[typeof (TObj)] = () => obj;
        }

        public static void Register<TRequest, TObj>(TObj obj) where TObj : TRequest
        {
            Instance._objectProviders[typeof(TRequest)] = () => obj;
        }

        public static TRequest Get<TRequest>()
        {
            Func<object> obj;
            Instance._objectProviders.TryGetValue(typeof (TRequest), out obj);

            return (TRequest) obj?.Invoke();
        }
    }
}
