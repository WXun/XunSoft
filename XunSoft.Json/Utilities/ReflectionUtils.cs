using System;
using System.Globalization;

namespace XunSoft.Json.Utilities
{
    /// <summary>
    /// 反射工具类
    /// </summary>
    internal static class ReflectionUtils
    {
        public static readonly Type[] EmptyTypes;

        static ReflectionUtils()
        {
            EmptyTypes = Type.EmptyTypes;
        }

        /// <summary>
        /// 实现了泛型定义
        /// </summary>
        /// <param name="type">判断的类型</param>
        /// <param name="genericInterfaceDefinition">泛型接口定义</param>
        /// <returns></returns>
        public static bool ImplementsGenericDefinition(Type type, Type genericInterfaceDefinition)
        {
            return ImplementsGenericDefinition(type, genericInterfaceDefinition, out _);
        }

        /// <summary>
        /// 实现泛型定义
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genericInterfaceDefinition"></param>
        /// <param name="implementingType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static bool ImplementsGenericDefinition(Type type, Type genericInterfaceDefinition,
                                                       out Type implementingType)
        {
            ValidationUtils.ArgumentNotNull(type, nameof(type));
            ValidationUtils.ArgumentNotNull(genericInterfaceDefinition, nameof(genericInterfaceDefinition));
            if (!genericInterfaceDefinition.IsInterface() || !genericInterfaceDefinition.IsGenericTypeDefinition())
            {
                string message =
                        "'{0}' is not a generic interface definition.".FormatWith(CultureInfo.InvariantCulture,
                                genericInterfaceDefinition);
                throw new ArgumentException(message);
            }

            if (type.IsInterface())
            {
                if (type.IsGenericType())
                {
                    Type interfaceDefinition = type.GetGenericTypeDefinition();
                    if (genericInterfaceDefinition == interfaceDefinition)
                    {
                        implementingType = type;
                        return true;
                    }
                }
            }

            foreach (Type interfaceType in type.GetInterfaces())
            {
                if (!interfaceType.IsGenericType())
                {
                    continue;
                }

                var genericTypeDefinition = interfaceType.GetGenericTypeDefinition();
                if (genericInterfaceDefinition != genericTypeDefinition)
                {
                    continue;
                }

                implementingType = interfaceType;
                return true;
            }

            implementingType = null;
            return false;
        }

        public static bool IsNullableType(Type t)
        {
            ValidationUtils.ArgumentNotNull(t, nameof(t));
            return t.IsGenericType() && t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}