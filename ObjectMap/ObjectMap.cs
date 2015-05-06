using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

//TODO: Nuget Package
//TODO: Separate classes into individual files and utilities
namespace ObjectMap
{
    public class ObjectMap
    {
        private static readonly ObjectMap Instance;
        private readonly Dictionary<Type, Func<object>> _objectProviders = new Dictionary<Type, Func<object>>();
        private readonly Dictionary<Type, Func<object>> _objectInjectionInstructions = new Dictionary<Type, Func<object>>();
        
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

        public static void InjectAllPropertiesofType<TInjectable>(Func<TInjectable> objectProvider = null) where TInjectable : class
        {
            Instance._objectInjectionInstructions[typeof (TInjectable)] = objectProvider ?? (() => CreateInstance(typeof(TInjectable)) as TInjectable);
        }

        public static void InjectAllPropertiesofType<TInjectable, TInject>() where TInjectable : class where TInject : class , TInjectable
        {
            Instance._objectInjectionInstructions[typeof (TInjectable)] = () => CreateInstance(typeof (TInject));
        }

        public static TRequest Get<TRequest>() where TRequest : class
        {
            return GetInstance(typeof(TRequest)) as TRequest;
        }

        public static object GetInstance(Type requestedType)
        {
            Func<object> objectProvider;

            return Instance._objectProviders.TryGetValue(requestedType, out objectProvider)
                ? objectProvider.Invoke()
                : null;
        }

        private static object CreateInstance(Type type)
        {
            var constructor = FindSuitableConstructor(type);
            var instance = CreateInstance(type, constructor);
            
            TryInjectDependencies(instance, type);

            return instance;
        }

        private static object CreateInstance(Type type, ConstructorInfo constructor)
        {
            var instance = constructor != null
                ? constructor.Invoke(
                    constructor.GetParameters().Select(paramInfo => GetInstance(paramInfo.GetType())).ToArray())
                : Activator.CreateInstance(type);
            
            //TODO: Handle cyclic dependencies
            
            return instance;
        }

        private static ConstructorInfo FindSuitableConstructor(Type type)
        {
            return type.GetConstructors()
                       .FirstOrDefault(ctor => ctor.GetParameters()
                                                   .All(paramInfo => Instance._objectProviders.ContainsKey(paramInfo.ParameterType)));
        }

        private static void TryInjectDependencies(object instance, IReflect instanceType)
        {
            foreach (
                var propertyInfo in
                    instanceType.GetProperties(BindingFlags.Public | BindingFlags.Instance |
                                               BindingFlags.FlattenHierarchy))
            {
                if (propertyInfo.CanWrite
                    &&
                    Instance._objectInjectionInstructions.ContainsKey(propertyInfo.PropertyType))

                    //TODO: Handle cyclic dependencies
                    if (propertyInfo.GetValue(instance) == null)
                    {
                        propertyInfo.SetValue(instance,
                            Instance._objectInjectionInstructions[propertyInfo.PropertyType].Invoke());
                    }
            }
        }
    }
}
