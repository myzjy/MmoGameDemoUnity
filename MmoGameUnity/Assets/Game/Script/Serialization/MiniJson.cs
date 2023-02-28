/*
 * Copyright (c) 2013 Calvin Rien
 *
 * Based on the JSON parser by Patrick van Bergen
 * http://techblog.procurios.nl/k/618/news/view/14605/14863/How-do-I-write-my-own-parser-for-JSON.html
 *
 * Simplified it so that it doesn't throw exceptions
 * and can be used in Unity iPhone with maximum code stripping.
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
 * CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

// ReSharper disable once CheckNamespace
namespace MiniJSON
{
    /// <summary>
    /// This class encodes and decodes JSON strings.
    /// Spec. details, see http://www.json.org/
    ///
    /// JSON uses Arrays and Objects. These correspond here to the datatypes IList and IDictionary.
    /// All numbers are parsed to doubles.
    /// </summary>
    public static class Json
    {
        /// <summary>
        /// Parses the string json into a value
        /// </summary>
        /// <param name="json">A JSON string.</param>
        /// <returns>An List&lt;object&gt;, a Dictionary&lt;string, object&gt;, a double, an integer,a string, null, true, or false</returns>
        public static object Deserialize(string json)
        {
            // save the string for debug information
            return json == null ? null : Parser.Parse(json);
        }

        private sealed class Parser : IDisposable
        {
            private const string WordBreak = "{}[],:\"";

            private static bool IsWordBreak(char c)
            {
                return char.IsWhiteSpace(c) || WordBreak.IndexOf(c) != -1;
            }

            // ReSharper disable once InconsistentNaming
            private enum TOKEN
            {
                None,
                CurlyOpen,
                CurlyClose,
                SquaredOpen,
                SquaredClose,
                Colon,
                Comma,
                String,
                Number,
                True,
                False,
                Null
            };

            private StringReader _json;

            private Parser(string jsonString)
            {
                _json = new StringReader(jsonString);
            }

            public static object Parse(string jsonString)
            {
                using (var instance = new Parser(jsonString))
                {
                    return instance.ParseValue();
                }
            }

            public void Dispose()
            {
                _json.Dispose();
                _json = null;
            }

            Dictionary<string, object> ParseObject()
            {
                Dictionary<string, object> table = new Dictionary<string, object>();

                // ditch opening brace
                _json.Read();

                // {
                while (true)
                {
                    switch (NextToken)
                    {
                        case TOKEN.None:
                            return null;
                        case TOKEN.Comma:
                            continue;
                        case TOKEN.CurlyClose:
                            return table;
                        default:
                            // name
                            string name = ParseString();
                            if (name == null)
                            {
                                return null;
                            }

                            // :
                            if (NextToken != TOKEN.Colon)
                            {
                                return null;
                            }

                            // ditch the colon
                            _json.Read();

                            // value
                            table[name] = ParseValue();
                            break;
                    }
                }
            }

            List<object> ParseArray()
            {
                List<object> array = new List<object>();

                // ditch opening bracket
                _json.Read();

                // [
                var parsing = true;
                while (parsing)
                {
                    TOKEN nextToken = NextToken;

                    switch (nextToken)
                    {
                        case TOKEN.None:
                            return null;
                        case TOKEN.Comma:
                            continue;
                        case TOKEN.SquaredClose:
                            parsing = false;
                            break;
                        default:
                            object value = ParseByToken(nextToken);

                            array.Add(value);
                            break;
                    }
                }

                return array;
            }

            object ParseValue()
            {
                TOKEN nextToken = NextToken;
                return ParseByToken(nextToken);
            }

            object ParseByToken(TOKEN token)
            {
                switch (token)
                {
                    case TOKEN.String:
                        return ParseString();
                    case TOKEN.Number:
                        return ParseNumber();
                    case TOKEN.CurlyOpen:
                        return ParseObject();
                    case TOKEN.SquaredOpen:
                        return ParseArray();
                    case TOKEN.True:
                        return true;
                    case TOKEN.False:
                        return false;
                    case TOKEN.Null:
                        return null;
                    default:
                        return null;
                }
            }

            string ParseString()
            {
                StringBuilder s = new StringBuilder();
                char c;

                // ditch opening quote
                _json.Read();

                bool parsing = true;
                while (parsing)
                {
                    if (_json.Peek() == -1)
                    {
                        parsing = false;
                        break;
                    }

                    c = NextChar;
                    switch (c)
                    {
                        case '"':
                            parsing = false;
                            break;
                        case '\\':
                            if (_json.Peek() == -1)
                            {
                                parsing = false;
                                break;
                            }

                            c = NextChar;
                            switch (c)
                            {
                                case '"':
                                case '\\':
                                case '/':
                                    s.Append(c);
                                    break;
                                case 'b':
                                    s.Append('\b');
                                    break;
                                case 'f':
                                    s.Append('\f');
                                    break;
                                case 'n':
                                    s.Append('\n');
                                    break;
                                case 'r':
                                    s.Append('\r');
                                    break;
                                case 't':
                                    s.Append('\t');
                                    break;
                                case 'u':
                                    var hex = new char[4];

                                    for (int i = 0; i < 4; i++)
                                    {
                                        hex[i] = NextChar;
                                    }

                                    s.Append((char)Convert.ToInt32(new string(hex), 16));
                                    break;
                            }

                            break;
                        default:
                            s.Append(c);
                            break;
                    }
                }

                return s.ToString();
            }

            object ParseNumber()
            {
                string number = NextWord;

                if (number.IndexOf('.') == -1)
                {
                    long parsedInt;
                    Int64.TryParse(number, out parsedInt);
                    return parsedInt;
                }

                double parsedDouble;
                Double.TryParse(number, out parsedDouble);
                return parsedDouble;
            }

            void EatWhitespace()
            {
                while (Char.IsWhiteSpace(PeekChar))
                {
                    _json.Read();

                    if (_json.Peek() == -1)
                    {
                        break;
                    }
                }
            }

            char PeekChar
            {
                get { return Convert.ToChar(_json.Peek()); }
            }

            char NextChar
            {
                get { return Convert.ToChar(_json.Read()); }
            }

            string NextWord
            {
                get
                {
                    StringBuilder word = new StringBuilder();

                    while (!IsWordBreak(PeekChar))
                    {
                        word.Append(NextChar);

                        if (_json.Peek() == -1)
                        {
                            break;
                        }
                    }

                    return word.ToString();
                }
            }

            TOKEN NextToken
            {
                get
                {
                    EatWhitespace();

                    if (_json.Peek() == -1)
                    {
                        return TOKEN.None;
                    }

                    switch (PeekChar)
                    {
                        case '{':
                            return TOKEN.CurlyOpen;
                        case '}':
                            _json.Read();
                            return TOKEN.CurlyClose;
                        case '[':
                            return TOKEN.SquaredOpen;
                        case ']':
                            _json.Read();
                            return TOKEN.SquaredClose;
                        case ',':
                            _json.Read();
                            return TOKEN.Comma;
                        case '"':
                            return TOKEN.String;
                        case ':':
                            return TOKEN.Colon;
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                        case '-':
                            return TOKEN.Number;
                    }

                    switch (NextWord)
                    {
                        case "false":
                            return TOKEN.False;
                        case "true":
                            return TOKEN.True;
                        case "null":
                            return TOKEN.Null;
                    }

                    return TOKEN.None;
                }
            }
        }

        /// <summary>
        /// Converts a IDictionary / IList object or a simple type (string, int, etc.) into a JSON string
        /// </summary>
        /// <param name="json">A Dictionary&lt;string, object&gt; / List&lt;object&gt;</param>
        /// <returns>A JSON encoded string, or null if object 'json' is not serializable</returns>
        public static string Serialize(object obj)
        {
            return Serializer.Serialize(obj);
        }

        sealed class Serializer
        {
            StringBuilder builder;

            Serializer()
            {
                builder = new StringBuilder();
            }

            public static string Serialize(object obj)
            {
                var instance = new Serializer();

                instance.SerializeValue(obj);

                return instance.builder.ToString();
            }

            void SerializeValue(object value)
            {
                IList asList;
                IDictionary asDict;
                string asStr;

                if (value == null)
                {
                    builder.Append("null");
                }
                else if ((asStr = value as string) != null)
                {
                    SerializeString(asStr);
                }
                else if (value is bool)
                {
                    builder.Append((bool)value ? "true" : "false");
                }
                else if ((asList = value as IList) != null)
                {
                    SerializeArray(asList);
                }
                else if ((asDict = value as IDictionary) != null)
                {
                    SerializeObject(asDict);
                }
                else if (value is char)
                {
                    SerializeString(new string((char)value, 1));
                }
                else
                {
                    SerializeOther(value);
                }
            }

            void SerializeObject(IDictionary obj)
            {
                bool first = true;

                builder.Append('{');

                foreach (object e in obj.Keys)
                {
                    if (!first)
                    {
                        builder.Append(',');
                    }

                    SerializeString(e.ToString());
                    builder.Append(':');

                    SerializeValue(obj[e]);

                    first = false;
                }

                builder.Append('}');
            }

            void SerializeArray(IList anArray)
            {
                builder.Append('[');

                bool first = true;

                foreach (object obj in anArray)
                {
                    if (!first)
                    {
                        builder.Append(',');
                    }

                    SerializeValue(obj);

                    first = false;
                }

                builder.Append(']');
            }

            void SerializeString(string str)
            {
                builder.Append('\"');

                char[] charArray = str.ToCharArray();
                foreach (var c in charArray)
                {
                    switch (c)
                    {
                        case '"':
                            builder.Append("\\\"");
                            break;
                        case '\\':
                            builder.Append("\\\\");
                            break;
                        case '\b':
                            builder.Append("\\b");
                            break;
                        case '\f':
                            builder.Append("\\f");
                            break;
                        case '\n':
                            builder.Append("\\n");
                            break;
                        case '\r':
                            builder.Append("\\r");
                            break;
                        case '\t':
                            builder.Append("\\t");
                            break;
                        default:
                            int codepoint = Convert.ToInt32(c);
                            if ((codepoint >= 32) && (codepoint <= 126))
                            {
                                builder.Append(c);
                            }
                            else
                            {
                                builder.Append("\\u");
                                builder.Append(codepoint.ToString("x4"));
                            }

                            break;
                    }
                }

                builder.Append('\"');
            }

            private void SerializeOther(object value)
            {
                switch (value)
                {
                    // NOTE: decimals lose precision during serialization.
                    // They always have, I'm just letting you know.
                    // Previously floats and doubles lost precision too.
                    case float f:
                        builder.Append(f.ToString("R"));
                        break;
                    case int _:
                    case uint _:
                    case long _:
                    case sbyte _:
                    case byte _:
                    case short _:
                    case ushort _:
                    case ulong _:
                        builder.Append(value);
                        break;
                    case double _:
                    case decimal _:
                        builder.Append(Convert.ToDouble(value).ToString("R"));
                        break;
                    default:
                        SerializeString(value.ToString());
                        break;
                }
            }
        }
    }
}