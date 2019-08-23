using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace XunSoft.Json.Utilities
{
    /// <summary>
    /// 集合包装接口
    /// </summary>
    internal interface IWrappedCollection : IList
    {
        object UnderlyingCollection { get; }
    }

    /// <summary>
    /// 集合包装类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class CollectionWrapper<T> : ICollection<T>, IWrappedCollection
    {
        private readonly ICollection<T> _genericCollection;
        private readonly IList          _list;
        private          object         _syncRoot;

        public CollectionWrapper(IList list)
        {
            ValidationUtils.ArgumentNotNull(list, nameof(list));
            if (list is ICollection<T> collection)
            {
                this._genericCollection = collection;
            }
            else
            {
                this._list = list;
            }
        }

        public CollectionWrapper(ICollection<T> collection)
        {
            ValidationUtils.ArgumentNotNull(collection, nameof(collection));
            this._genericCollection = collection;
        }

        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return (this._genericCollection ?? this._list.Cast<T>()).GetEnumerator();
        }

        /// <summary>
        ///  获取枚举器
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="item"></param>
        public virtual void Add(T item)
        {
            if (this._genericCollection != null)
            {
                this._genericCollection.Add(item);
            }
            else
            {
                this._list.Add(item);
            }
        }

        /// <summary>
        /// 添加 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int Add(object value)
        {
            VerifyValueType(value);
            this.Add((T) value);
            return this.Count - 1;
        }

        /// <summary>
        /// 判断集合是否包含value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(object value)
        {
            return IsCompatibleObject(value) && this.Contains(value);
        }

        void IList.Clear()
        {
            if (this._genericCollection != null)
            {
                this._genericCollection.Clear();
            }
            else
            {
                this._list.Clear();
            }
        }

        public int IndexOf(object value)
        {
            if (this._genericCollection != null)
            {
                throw new InvalidOperationException("Wrapped ICollection<T> 不支持 IndexOf.");
            }

            if (IsCompatibleObject(value))
            {
                return this._list.IndexOf(value);
            }

            return -1;
        }

        public void Insert(int index, object value)
        {
            if (this._genericCollection != null)
            {
                throw new InvalidOperationException("Wrapped ICollection<T> 不支持 Insert.");
            }

            VerifyValueType(value);
            this._list.Insert(index, value);
        }

        public void Remove(object value)
        {
            if (IsCompatibleObject(value))
            {
                this.Remove((T) value);
            }
        }

        public void RemoveAt(int index)
        {
            if (this._genericCollection != null)
            {
                throw new InvalidOperationException("Wrapped ICollection<T> 不支持 RemoveAt.");
            }

            this._list.RemoveAt(index);
        }

        public object this[int index]
        {
            get
            {
                if (this._genericCollection != null)
                {
                    throw new InvalidOperationException("Wrapped ICollection<T> does not support indexer.");
                }

                return this._list[index];
            }
            set
            {
                if (this._genericCollection != null)
                {
                    throw new InvalidOperationException("Wrapped ICollection<T> does not support indexer.");
                }

                VerifyValueType(value);
                this._list[index] = (T) value;
            }
        }


        public bool IsFixedSize => this._genericCollection?.IsReadOnly ?? this._list.IsFixedSize;

        void ICollection<T>.Clear()
        {
            if (this._genericCollection != null)
            {
                this._genericCollection.Clear();
            }
            else
            {
                this._list.Clear();
            }
        }

        public bool Contains(T item)
        {
            return this._genericCollection?.Contains(item) ?? this._list.Contains(item);
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            if (this._genericCollection != null)
            {
                this._genericCollection.CopyTo(array, arrayIndex);
            }
            else
            {
                this._list.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(T item)
        {
            if (this._genericCollection != null)
            {
                return this._genericCollection.Remove(item);
            }

            bool contains = this._list.Contains(item);
            if (contains)
            {
                this._list.Remove(item);
            }

            return contains;
        }

        public void CopyTo(Array array, int index)
        {
            this.CopyTo((T[]) array, index);
        }

        private static void VerifyValueType(object value)
        {
            if (!IsCompatibleObject(value))
            {
                throw new ArgumentException(
                        "The value '{0}' is not of type '{1}' and cannot be used in this generic collection."
                                .FormatWith(
                                        CultureInfo.InvariantCulture, value, typeof(T)), nameof(value));
            }
        }

        /// <summary>
        ///     判断value是否是可兼容的对象
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool IsCompatibleObject(object value)
        {
            return value is T || value == null && (!typeof(T).IsValueType || ReflectionUtils.IsNullableType(typeof(T)));
        }


        public object SyncRoot
        {
            get
            {
                if (this._syncRoot == null)
                {
                    Interlocked.CompareExchange(ref this._syncRoot, new object(), null);
                }

                return this._syncRoot;
            }
        }

        public bool IsSynchronized => false;

        public object UnderlyingCollection => (object) this._genericCollection ?? this._list;

        public virtual int Count => this._genericCollection?.Count ?? this._list.Count;

        public virtual bool IsReadOnly => this._genericCollection?.IsReadOnly ?? this._list.IsReadOnly;
    }
}