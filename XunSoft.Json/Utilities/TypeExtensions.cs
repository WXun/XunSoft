using System;

namespace XunSoft.Json.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsInterface(this Type type)
        {
            return type.IsInterface;
        }

        /// <summary>
        /// 判断当前类型是否表示可以用来构造其他泛型类型的泛型类型定义
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsGenericTypeDefinition(this Type type)
        {
            return type.IsGenericTypeDefinition;
        }

        /// <summary>
        /// 判断当前类型是否是泛型类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsGenericType(this Type type)
        {
            return type.IsGenericType;
        }
    }
}