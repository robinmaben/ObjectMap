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
        internal HashSet<Type> InjectablesRegistry { get; private set; }
        
        static ObjectMap()
        {
            Instance = new ObjectMap();
        }

        private ObjectMap()
        {
            ProviderRegistry = new Dictionary<Type, ObjectProvider>();
            InjectablesRegistry = new HashSet<Type>();
        }

        public static Type Register<TRequest, TObj>() where TObj : class, TRequest where TRequest : class
        {
            return RegisterProvider(typeof (TRequest), () => TryCreateInstance(typeof (TObj)));
        }

        public static Type Register<TRequest>(Func<object> provider)
        {
            return RegisterProvider(typeof (TRequest), provider);
        }

        private static Type RegisterProvider(Type requestedType, Func<object> provider)
        {
            Instance.ProviderRegistry[requestedType] = new ObjectProvider(provider);
            return requestedType;
        }

        public static Type InjectAllPropertiesofType<TInjectable>()
        {
            var injectableType = typeof(TInjectable);
            injectableType.InjectAllPropertiesofType();

            return injectableType;
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

        internal static object TryCreateInstance(Type type)
        {
            var constructor = type.GetConstructors()
                .FirstOrDefault(ctor => ctor.GetParameters()
                    .All(paramInfo => Instance.ProviderRegistry.ContainsKey(paramInfo.ParameterType)));

            var instance = CreateInstance(type, constructor);

            TryInjectDependencies(instance, type);

            return instance;
        }

        internal static void TryInjectDependencies(object instance, IReflect instanceType = null)
        {
            instanceType = instanceType ?? instance.GetType();

            foreach (var propertyInfo in
                    instanceType.GetProperties(BindingFlags.Public | BindingFlags.Instance |
                                               BindingFlags.FlattenHierarchy))
            {
                if (propertyInfo.CanWrite
                    && Instance.InjectablesRegistry.Contains(propertyInfo.PropertyType))
                    //TODO: Handle cyclic dependencies
                    if (propertyInfo.GetValue(instance) == null)
                    {
                        propertyInfo.SetValue(instance, GetInstance(propertyInfo.PropertyType));
                    }
            }
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
