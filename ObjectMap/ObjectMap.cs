using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

//TODO: Nuget Package

namespace ObjectMap
{
    public class ObjectMap
    {
        internal static readonly ObjectMap Instance;
        internal Dictionary<Type, ObjectProvider> ProviderRegistry { get; private set; }
        
        static ObjectMap()
        {
            Instance = new ObjectMap();
        }

        private ObjectMap()
        {
            ProviderRegistry = new Dictionary<Type, ObjectProvider>();
        }

        public static Type Register<TRequest, TObj>() where TObj : class, TRequest where TRequest : class
        {
            return RegisterProvider(typeof (TRequest), () => TryCreateInstance(typeof (TObj)));
        }

        public static Type Register<TRequest>(Func<object> provider)
        {
            return RegisterProvider(typeof (TRequest), provider);
        }

        public static TRequest Get<TRequest>() where TRequest : class
        {
            return GetInstance(typeof(TRequest)) as TRequest;
        }

        public static object GetInstance(Type requestedType)
        {
            ObjectProvider objectProvider;

            return Instance.ProviderRegistry.TryGetValue(requestedType, out objectProvider)
                ? objectProvider.GetInstance()
                : null;
        }

        //TODO: Handle cyclic dependencies
        public static void TryInjectDependencies(object instance, IReflect instanceType = null)
        {
            instanceType = instanceType ?? instance.GetType();

            var writableNonInitializedDependencies =
                from property in instanceType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                where
                    property.CanWrite
                    && property.PropertyType.IsInterface
                    && Instance.ProviderRegistry.ContainsKey(property.PropertyType)
                    && property.GetValue(instance) == null
                select property;

            foreach (var writableProperty in writableNonInitializedDependencies)
            {
                writableProperty.SetValue(instance, GetInstance(writableProperty.PropertyType));
            }
        }

        private static Type RegisterProvider(Type requestedType, Func<object> provider)
        {
            ObjectProvider objectProvider;
            if (Instance.ProviderRegistry.TryGetValue(requestedType, out objectProvider))
            {
                if (objectProvider.LifecycleOptions == LifecycleOptions.Singleton)
                {
                    return requestedType; //If Singleton do not register a new provider
                }
            }

            Instance.ProviderRegistry[requestedType] = new ObjectProvider(provider);
            return requestedType;
        }

        private static object TryCreateInstance(Type type)
        {
            var constructor = type.GetConstructors()
                .FirstOrDefault(ctor => ctor.GetParameters()
                    .All(paramInfo => Instance.ProviderRegistry.ContainsKey(paramInfo.ParameterType)));

            var instance = CreateInstance(type, constructor);

            TryInjectDependencies(instance, type);

            return instance;
        }

        private static object CreateInstance(Type type, ConstructorInfo constructor)
        {
            if (constructor == null) return Activator.CreateInstance(type);

            var ctorParams = constructor.GetParameters().Select(paramInfo => GetInstance(paramInfo.GetType())).ToArray();
            return constructor.Invoke(ctorParams);

            //TODO: Handle cyclic dependencies
        }
    }
}
