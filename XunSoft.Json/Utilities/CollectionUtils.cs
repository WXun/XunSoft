using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace XunSoft.Json.Utilities
{
    /// <summary>
    /// 集合工具
    /// </summary>
    internal static class CollectionUtils
    {
        /// <summary>
        /// 判断集合是否为null或内容为空
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(ICollection<T> collection)
        {
            if (collection != null)
            {
                return collection.Count == 0;
            }

            return true;
        }

        /// <summary>
        /// 将指定泛型集合的元素添加到指定的泛型集合
        /// </summary>
        /// <param name="initial"></param>
        /// <param name="collection"></param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="ArgumentException"></exception>
        public static void AddRange<T>(this IList<T> initial, IEnumerable<T> collection)
        {
            if (initial == null)
            {
                throw new ArgumentException(nameof(initial));
            }

            if (collection == null)
            {
                return;
            }

            foreach (var value in collection)
            {
                initial.Add(value);
            }
        }

        /// <summary>
        /// 判断当前类型是否键/值集合
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsDictionaryType(Type type)
        {
            ValidationUtils.ArgumentNotNull(type, nameof(type));
            return typeof(IDictionary).IsAssignableFrom(type) || ReflectionUtils.ImplementsGenericDefinition(type, typeof(IReadOnlyDictionary<,>));
        }

        /// <summary>
        /// 解析可枚举集合构造函数
        /// </summary>
        /// <param name="collectionType">集合类型</param>
        /// <param name="collectionItemType">集合成员类型</param>
        /// <returns></returns>
        public static ConstructorInfo ResolveEnumerableCollectionConstructor(Type collectionType,
            Type collectionItemType)
        {
            Type genericConstructorArgument = typeof(IList<>).MakeGenericType(collectionItemType);
            return ResolveEnumerableCollectionConstructor(collectionType, collectionItemType,
                genericConstructorArgument);
        }

        /// <summary>
        /// 解析可枚举集合构造函数
        /// </summary>
        /// <param name="collectionType">集合类型</param>
        /// <param name="collectionItemType">集合成员类型</param>
        /// <param name="constructorArgumentType">构造函数参数类型</param>
        /// <returns></returns>
        public static ConstructorInfo ResolveEnumerableCollectionConstructor(Type collectionType,
            Type collectionItemType, Type constructorArgumentType)
        {
            Type genericEnumerable = typeof(IEnumerable<>).MakeGenericType(collectionItemType);
            ConstructorInfo match = null;
            foreach (ConstructorInfo constructor in collectionType.GetConstructors(
                BindingFlags.Public | BindingFlags.Instance))
            {
                IList<ParameterInfo> parameters = constructor.GetParameters();
                if (parameters.Count == 1)
                {
                    Type parameterType = parameters[0].ParameterType;
                    if (genericEnumerable == parameterType)
                    {
                        match = constructor;
                        break;
                    }

                    if (match != null)
                    {
                        continue;
                    }

                    if (parameterType.IsAssignableFrom(constructorArgumentType))
                    {
                        match = constructor;
                    }
                }
            }

            return match;
        }

        /// <summary>
        /// 给泛型集合添加不重复成员
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool AddDistinct<T>(this IList<T> list, T value)
        {
            return list.AddDistinct(value, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// 给集合添加不重复成员
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool AddDistinct<T>(this IList<T> list, T value, IEqualityComparer<T> comparer)
        {
            if (list.ContainsValue(value, comparer))
            {
                return false;
            }
            list.Add(value);
            return true;
        }
        
        /// <summary>
        /// 给泛型集合添加不重复的成员集合
        /// </summary>
        /// <param name="list"></param>
        /// <param name="values"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool AddRangeDistinct<T>(this IList<T> list, IEnumerable<T> values, IEqualityComparer<T> comparer)
        {
            var allAdded = true;
            foreach (T value in values)
            {
                if (!list.AddDistinct(value, comparer))
                {
                    allAdded = false;
                }
            }
            return allAdded;
        }

        /// <summary>
        /// 判断可枚举泛型集合中是否包含某个元素
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static bool ContainsValue<TSource>(this IEnumerable<TSource> sources, TSource value,
            IEqualityComparer<TSource> comparer)
        {
            if (sources == null)
            {
                throw new ArgumentException(nameof(sources));
            }
            if (comparer == null)
            {
                comparer = EqualityComparer<TSource>.Default;
            }
            return sources.Any(source => comparer.Equals(source, value));
        }
        
        /// <summary>
        /// 返回成员在集合中的索引
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int IndexOf<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            var index = 0;
            foreach (T value in collection)
            {
                if (predicate(value))
                {
                    return index;
                }

                index++;
            }

            return index;
        }
        /// <summary>
        /// 判断泛型集合是否包含value成员
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool Contains<T>(this IEnumerable<T> list, T value, IEqualityComparer comparer)
        {
            return list.Any(t => comparer.Equals(value, t));
        }

        public static int IndexOfReference<T>(this List<T> list, T item)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (ReferenceEquals(item, list[i]))
                {
                    return i;
                }
            }
            return -1;
        }
        /// <summary>
        /// 快速翻转
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        public static void FastReverse<T>(this List<T> list)
        {
            var i = 0;
            int j = list.Count - 1;
            while (i<j)
            {
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
                i++;
                j--;
            }
        }
        
    }
}