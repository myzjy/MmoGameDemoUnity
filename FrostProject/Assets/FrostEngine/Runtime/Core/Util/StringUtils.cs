using System;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting.Antlr3.Runtime.Collections;

namespace FrostEngine
{
    public abstract class StringUtils
    {
        [ThreadStatic] private static StringBuilder _cachedStringBuilder;

        [ThreadStatic] private static object[] _oneArgs;

        [ThreadStatic] private static object[] _twoArgs;

        [ThreadStatic] private static object[] _threeArgs;

        public static readonly string Empty = "";

        private const string NullObjectString = "null";

        public static readonly string[] EmptyStringArray = new string[] { };

        public static readonly object[] OneObjectArray = { NullObjectString };

        public static readonly string Comma = ","; // [com·ma || 'kɒmə] n.  逗点; 逗号
        public static readonly string CommaRegex = ",|，";

        public static readonly string Period = "."; // 句号

        public static readonly string LeftSquareBracket = "["; //左方括号

        public static readonly string RightSquareBracket = "]"; //右方括号

        public static readonly string Colon = ":"; //冒号[co·lon || 'kəʊlən]
        public static readonly string ColonRegex = ":|：";

        public static readonly string Semicolon = ";"; //分号['semi'kәulәn]
        public static readonly string SemicolonRegex = ";|；";

        public static readonly string QuotationMark = "\""; //引号[quo·ta·tion || kwəʊ'teɪʃn]

        public static readonly string Ellipsis = "..."; //省略号

        public static readonly string ExclamationPoint = "!"; //感叹号

        public static readonly string Dash = "-"; //破折号

        public static readonly string QuestionMark = "?"; //问好

        public static readonly string Hyphen = "-"; //连接号，连接号与破折号的区别是，连接号的两头不用空格

        public static readonly string Slash = "/"; //斜线号

        public static readonly string BackSlash = "\\"; //反斜线号

        public static readonly string VerticalBar = "|"; // 竖线
        public static readonly string VerticalBarRegex = "\\|";

        public static readonly string Sharp = "#";
        public static readonly string SharpRegex = "\\#";

        public static readonly string Dollar = "$"; // 美元符号

        private const string EmptyJson = "{}";

        public static readonly string MultipleHyphens =
            "-----------------------------------------------------------------------";


        public static readonly int IndexNotFound = -1; //Represents a failed index search.

        public static readonly string DefaultCharset = "UTF-8";

        /**
         * 用于随机选的数字
         */
        public static readonly string ArabNumber = "0123456789";

        /**
         * 用于随机选的字符
         */
        public static readonly string EnglishChar = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static readonly HashSet<char> EnglishSet = new HashSet<char>()
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u',
            'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U',
            'V', 'W', 'X', 'Y', 'Z'
        };

        /**
     * Checks if a CharSequence is empty ("") or null.
     * <pre>
     * StringUtils.isEmpty(null)      = true
     * StringUtils.isEmpty("")        = true
     * StringUtils.isEmpty(" ")       = false
     * StringUtils.isEmpty("bob")     = false
     * </pre>
     * It no longer trims the CharSequence.
     * That functionality is available in isBlank().
     *
     * @param cs the CharSequence to check, may be null
     * @return {@code true} if the CharSequence is empty or null
     */
        private static bool IsEmpty(string cs)
        {
            return string.IsNullOrEmpty(cs);
        }


        /**
     * StringUtils.isBlank(null)=true
     * StringUtils.isBlank("")=true
     * StringUtils.isBlank("    ")=true
     * StringUtils.isBlank(" b ")=false
     *
     * @param cs 要检查的字符串
     * @return 是否为空的字符串
     */
        public static bool IsBlank(string cs)
        {
            if (IsEmpty(cs))
            {
                return true;
            }

            var length = cs.Length;
            var charArray = cs.ToCharArray();
            for (var i = 0; i < length; i++)
            {
                if (char.IsWhiteSpace(charArray[i]) == false)
                {
                    return false;
                }
            }

            return true;
        }

        public static string Format(string format, object arg0)
        {
            if (format == null)
            {
                throw new Exception("Format is invalid.");
            }

            _oneArgs ??= new object[1];

            _oneArgs[0] = arg0;
            return Format(format, _oneArgs);
        }

        public static string Format(string format, object arg0, object arg1)
        {
            if (format == null)
            {
                throw new Exception("Format is invalid.");
            }

            _twoArgs ??= new object[2];

            _twoArgs[0] = arg0;
            _twoArgs[1] = arg1;
            return Format(format, _twoArgs);
        }

        public static string Format(string format, object arg0, object arg1, object arg2)
        {
            if (format == null)
            {
                throw new Exception("Format is invalid.");
            }

            _threeArgs ??= new object[3];

            _threeArgs[0] = arg0;
            _threeArgs[1] = arg1;
            _threeArgs[2] = arg2;
            return Format(format, _threeArgs);
        }

        public static string Format(string template, params object[] args)
        {
            if (string.IsNullOrEmpty(template))
            {
                return template;
            }

            // 初始化定义好的长度以获得更好的性能
            CachedStringBuilder();

            // 记录已经处理到的位置
            var readIndex = 0;
            foreach (var t in args)
            {
                // 占位符所在位置
                var placeholderIndex = template.IndexOf(EmptyJson, readIndex, StringComparison.Ordinal);
                // 剩余部分无占位符
                if (placeholderIndex == -1)
                {
                    // 不带占位符的模板直接返回
                    if (readIndex == 0)
                    {
                        return template;
                    }

                    break;
                }

                _cachedStringBuilder.Append(template, readIndex, placeholderIndex - readIndex);
                _cachedStringBuilder.Append(t);
                readIndex = placeholderIndex + 2;
            }

            // 字符串模板剩余部分不再包含占位符，加入剩余部分后返回结果
            _cachedStringBuilder.Append(template, readIndex, template.Length - readIndex);
            return _cachedStringBuilder.ToString();
        }

        public static string SubstringAfterFirst(string str, string separator)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            if (separator == null)
            {
                return Empty;
            }

            var pos = str.IndexOf(separator, StringComparison.Ordinal);
            if (pos < 0)
            {
                return Empty;
            }

            return str.Substring(pos + separator.Length);
        }

        public static string SubstringBeforeFirst(string str, string separator)
        {
            if (string.IsNullOrEmpty(str) || separator == null)
            {
                return str;
            }

            if (string.IsNullOrEmpty(separator))
            {
                return Empty;
            }

            var pos = str.IndexOf(separator, StringComparison.Ordinal);
            if (pos < 0)
            {
                return str;
            }

            return str.Substring(0, pos);
        }

        private static StringBuilder CachedStringBuilder()
        {
            _cachedStringBuilder ??= new StringBuilder(1024);

            _cachedStringBuilder.Clear();
            return _cachedStringBuilder;
        }

        // Joining
        //-----------------------------------------------------------------------

        private static string DefaultString(string str, string defaultStr)
        {
            return str ?? defaultStr;
        }

        public static byte[] Bytes(string str)
        {
            try
            {
                return ConverterUtils.GetBytes(str);
            }
            catch (Exception)
            {
                return CollectionUtils.EmptyByteArray;
            }
        }

        public static string BytesToString(byte[] bytes)
        {
            try
            {
                return ConverterUtils.GetString(bytes);
            }
            catch (Exception e)
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.LogError($"[StringUtils] [方法:BytesToString(byte[] bytes)] [msg:{e}]");
#endif
                return Empty;
            }
        }

        /**
         * Joins the elements of the provided varargs into a single String containing the provided elements.
         * No delimiter is added before or after the list.
         * null elements and separator are treated as empty Strings ("").
         *
         * <pre>
         * StringUtils.joinWith(",", {"a", "b"})        = "a,b"
         * StringUtils.joinWith(",", {"a", "b",""})     = "a,b,"
         * StringUtils.joinWith(",", {"a", null, "b"})  = "a,,b"
         * StringUtils.joinWith(null, {"a", "b"})       = "ab"
         * </pre>
         *
         * @param separator the separator character to use, null treated as ""
         * @param objects   the varargs providing the values to join together. {@code null} elements are treated as ""
         * @return the joined String.
         * @throws java.lang.IllegalArgumentException if a null varargs is provided
         */
        public static string JoinWith(string separator, params object[] objects)
        {
            if (objects == null)
            {
                throw new Exception("Object varargs must not be null");
            }

            var sanitizedSeparator = DefaultString(separator, Empty);

            var builder = CachedStringBuilder();
            for (var i = 0; i < objects.Length; i++)
            {
                var value = objects[i].ToString();
                builder.Append(value);

                if (i < objects.Length - 1)
                {
                    builder.Append(sanitizedSeparator);
                }
            }

            return builder.ToString();
        }

        public static string GetLengthString(long length)
        {
            if (length < 1024)
            {
                return $"{length.ToString()} Bytes";
            }

            if (length < 1024 * 1024)
            {
                return $"{(length / 1024f):F2} KB";
            }

            return length < 1024 * 1024 * 1024
                ? $"{(length / 1024f / 1024f):F2} MB"
                : $"{(length / 1024f / 1024f / 1024f):F2} GB";
        }

        public static string GetByteLengthString(long byteLength)
        {
            if (byteLength < 1024L) // 2 ^ 10
            {
                return Format("{} Bytes", byteLength.ToString());
            }

            if (byteLength < 1048576L) // 2 ^ 20
            {
                return Format("{} KB", (byteLength / 1024f).ToString("F2"));
            }

            if (byteLength < 1073741824L) // 2 ^ 30
            {
                return Format("{} MB", (byteLength / 1048576f).ToString("F2"));
            }

            if (byteLength < 1099511627776L) // 2 ^ 40
            {
                return Format("{} GB", (byteLength / 1073741824f).ToString("F2"));
            }

            if (byteLength < 1125899906842624L) // 2 ^ 50
            {
                return Format("{} TB", (byteLength / 1099511627776f).ToString("F2"));
            }

            if (byteLength < 1152921504606846976L) // 2 ^ 60
            {
                return Format("{} PB", (byteLength / 1125899906842624f).ToString("F2"));
            }

            return Format("{} EB", (byteLength / 1152921504606846976f).ToString("F2"));
        }
    }
}