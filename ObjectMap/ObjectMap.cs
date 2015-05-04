using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectMap
{
    public class ObjectMap
    {
        private static readonly ObjectMap Instance;
        private readonly Dictionary<Type, Func<object>> _objectProviders = new Dictionary<Type, Func<object>>();
        
        static ObjectMap()
        {
            Instance = new ObjectMap();
        }

        public static void Register<TRequest, TObj>() where TObj : class, TRequest where TRequest : class
        {
            Instance._objectProviders[typeof (TRequest)] = () => CreateInstance(typeof (TObj));
        }

        public static void Register<TRequest>(Func<TRequest> provider)
        {
            Instance._objectProviders[typeof(TRequest)] = () => provider;
        }

        public static TRequest Get<TRequest>() where TRequest : class
        {
            return GetInstanceProvider(typeof(TRequest)) as TRequest;
        }

        private static object GetInstanceProvider(Type requestedType)
        {
            Func<object> objectProvider;

            return Instance._objectProviders.TryGetValue(requestedType, out objectProvider)
                ? objectProvider.Invoke()
                : null;
        }

        private static object CreateInstance(Type type)
        {
            var constructor =
                type.GetConstructors()
                    .FirstOrDefault(ctor => ctor
                                .GetParameters()
                                .All(paramInfo => Instance._objectProviders.ContainsKey(paramInfo.ParameterType)));

            return constructor != null
                ? constructor.Invoke(constructor.GetParameters().Select(paramInfo => GetInstanceProvider(paramInfo.GetType())).ToArray())
                : null;

            //TODO: Use public parameterless ctor?
            //TODO: Fill dependent Properties on requested type
            //TODO: Prevent circular dependencies
        }
    }
}
