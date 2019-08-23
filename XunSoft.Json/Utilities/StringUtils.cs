using System;
using System.Globalization;
using System.IO;
using System.Text;
using XunSoft.Json.Enum;

namespace XunSoft.Json.Utilities
{
    internal static class StringUtils
    {
//        /// <summary>
//        ///     回车换行
//        /// </summary>
//        public const string CarriageReturnLineFeed = "\r\n";
//
//        /// <summary>
//        ///     空
//        /// </summary>
//        public const string Empty = "";
//
//        /// <summary>
//        ///     回车
//        /// </summary>
//        public const char CarriageReturn = '\r';
//
//        /// <summary>
//        ///     换行
//        /// </summary>
//        public const char LineFeed = '\n';
//
//        /// <summary>
//        ///     制表符
//        /// </summary>
//        public const char Tab = '\t';

        public static string FormatWith(this string format, IFormatProvider provider, object arg0)
        {
            return format.FormatWith(provider, new[] {arg0});
        }

        public static string FormatWith(this string format, IFormatProvider provider, object arg0, object arg1)
        {
            return format.FormatWith(provider, new[] {arg0, arg1});
        }

        public static string FormatWith(this string format, IFormatProvider provider, object arg0, object arg1,
            object arg2)
        {
            return format.FormatWith(provider, new[] {arg0, arg1, arg2});
        }

        public static string FormatWith(this string format, IFormatProvider provider, object arg0, object arg1,
            object arg2, object arg3)
        {
            return format.FormatWith(provider, new[] {arg0, arg1, arg2, arg3});
        }

        private static string FormatWith(this string format, IFormatProvider provider, params object[] args)
        {
            // leave this a private to force code to use an explicit overload
            // avoids stack memory being reserved for the object array
            ValidationUtils.ArgumentNotNull(format, nameof(format));

            return string.Format(provider, format, args);
        }

        /// <summary>
        ///     判断字符串是否是空格
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static bool IsWhiteSpace(string s)
        {
            if (s == null)
            {
                throw new ArgumentException(nameof(s));
            }

            if (s.Length == 0)
            {
                return false;
            }

            foreach (char t in s)
            {
                if (!char.IsWhiteSpace(t))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="capacity">容量</param>
        /// <returns></returns>
        public static StringWriter CreateStringWriter(int capacity)
        {
            var stringBuilder = new StringBuilder(capacity);
            return new StringWriter(stringBuilder, CultureInfo.InvariantCulture);
        }

        public static string ToCamelCase(string s)
        {
            if (string.IsNullOrEmpty(s) || !char.IsUpper(s[0]))
            {
                return s;
            }

            var chars = s.ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                if (i == 1 && !char.IsUpper(chars[i]))
                {
                    break;
                }

                var hasNext = i + 1 < chars.Length;
                if (i > 0 &&hasNext&& !char.IsUpper(chars[i + 1]))
                {
                    if (char.IsSeparator(chars[i + 1]))
                    {
                        chars[i] = ToLower(chars[i]);
                    }

                    break;
                }

                chars[i] = ToLower(chars[i]);
            }

            return new string(chars);
        }

        private static char ToLower(char c)
        {
            c = char.ToLower(c, CultureInfo.InvariantCulture);
            return c;
        }


        public static string ToSnakeCase(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            var sb = new StringBuilder();
            var state = SnakeCaseState.Start;
            for (var i = 0; i < s.Length; i++)
            {
                if (s[i] == ' ')
                {
                    if (state != SnakeCaseState.Start)
                    {
                        state = SnakeCaseState.NewWord;
                    }
                }
                else if (char.IsUpper(s[i]))
                {
                    switch (state)
                    {
                        case SnakeCaseState.Upper:
                            var hasNext = i + 1 < s.Length;
                            if (i > 0 && hasNext)
                            {
                                var nextChar = s[i + 1];
                                if (!char.IsUpper(nextChar) && nextChar != '_')
                                {
                                    sb.Append('_');
                                }
                            }

                            break;
                        case SnakeCaseState.Lower:
                        case SnakeCaseState.NewWord:
                            sb.Append('_');
                            break;
                        case SnakeCaseState.Start:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    char c;
                    c = char.ToLower(s[i], CultureInfo.InvariantCulture);
                    sb.Append(c);
                    state = SnakeCaseState.Upper;
                }
                else if (s[i] == '_')
                {
                    sb.Append('_');
                    state = SnakeCaseState.Start;
                }
                else
                {
                    if (state == SnakeCaseState.NewWord)
                    {
                        sb.Append('_');
                    }

                    sb.Append(s[i]);
                    state = SnakeCaseState.Lower;
                }
            }

            return sb.ToString();
        }
        /// <summary>
        /// 判断字符串是否以value开始
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool StartsWith(this string source, char value)
        {
            return source.Length > 0 && source[0] == value;
        }
        /// <summary>
        /// 判断字符串是否以value结尾
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool EndsWith(this string source, char value)
        {
            return source.Length > 0 && source[source.Length-1] == value;
        }

        public static string Trim(this string s, int start, int length)
        {
            if (s == null)
            {
                throw new ArgumentException(nameof(s));
            }

            if (start < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(start));
            }

            if (length<0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            int end = start + length - 1;
            if (end >= s.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            for (; start < end; start++)
            {
                if (!char.IsWhiteSpace(s[start]))
                {
                    break;
                }
            }

            for (; end>= start; end--)
            {
                if (!char.IsWhiteSpace(s[end]))
                {
                    break;
                }
            }

            return s.Substring(start, end - start + 1);
        }
    }
}