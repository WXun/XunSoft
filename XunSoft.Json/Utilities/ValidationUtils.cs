using System;

namespace XunSoft.Json.Utilities
{
    /// <summary>
    /// 校验工具类
    /// </summary>
    internal static class ValidationUtils
    {
        /// <summary>
        /// 参数不为null
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void ArgumentNotNull(object value, string parameterName)
        {
            //++判断value对象是否为空
            if (value == null)
            {
                throw new ArgumentException(parameterName);
            }
        }
        
    }
}