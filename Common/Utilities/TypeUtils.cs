using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ESS.FW.Common.Components;

namespace ESS.FW.Common.Utilities
{
    /// <summary>
    ///     A class provides utility methods.
    /// </summary>
    public static class TypeUtils
    {
        /// <summary>
        ///     Check whether a type is a component type.
        /// </summary>
        public static bool IsComponent(Type type)
        {
            return type.IsClass && !type.IsAbstract && type.GetCustomAttributes(typeof (ComponentAttribute), true).Any();
        }

        public static Type GetPureType(Type type)
        {
            if (type.HasElementType || type.IsArray)
            {
                return type.GetElementType();
            }
            if (type.IsGenericType)
            {
                var itemType = type.GetGenericArguments().FirstOrDefault();

                return itemType;
            }

            return type;
        }

        public static Type GetPureType<T>()
        {
            var type = typeof (T);

            if (type.HasElementType || type.IsArray)
            {
                return type.GetElementType();
            }
            if (type.IsGenericType)
            {
                var itemType = type.GetGenericArguments().FirstOrDefault();

                return itemType;
            }

            return type;
        }

        public static string GetTypeFullName(Type type)
        {
            if (type.HasElementType || type.IsArray)
            {
                return GetFormattedName(type.GetElementType().FullName);
            }
            if (type.IsGenericType)
            {
                var itemType = type.GetGenericArguments().FirstOrDefault();

                return GetFormattedName(itemType.FullName);
            }

            return GetFormattedName(type.FullName);
        }


        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetFormattedName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Unformatted  Name");
            }

            return name.Split(',')[0].Trim().Replace('.', '_').Replace('+', '_');
        }


        public static IEnumerable<ConstructorInfo> GetDeclaredConstructors(this Type type)
        {
#if NET40
            const BindingFlags bindingFlags
                = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
            return type.GetConstructors(bindingFlags);
#else
            return type.GetTypeInfo().DeclaredConstructors;
#endif
        }

        public static ConstructorInfo GetDeclaredConstructor(this Type type, params Type[] parameterTypes)
        {
            return type.GetDeclaredConstructors().SingleOrDefault(
                c => !c.IsStatic && c.GetParameters().Select(p => p.ParameterType).SequenceEqual(parameterTypes));
        }
    }
}