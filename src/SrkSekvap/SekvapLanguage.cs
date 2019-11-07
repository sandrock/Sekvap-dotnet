// Sekvap-dotnet
//
// Copyright (C) 2015 SandRock
// Copyright (C) 2015 HiinoFW
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace SrkSekvap
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Sekvap serializer.
    /// See https://github.com/sandrock/Sekvap-dotnet for more information.
    /// </summary>
    public class SekvapLanguage
    {
        /// <summary>
        /// chars that need escaping when serializing a key.
        /// </summary>
        private static readonly char[] keyChars = new char[] { '=', ';', };

        /// <summary>
        /// chars that need escaping when serializing a value.
        /// </summary>
        private static readonly char[] valueChars = new char[] { ';', };

        private const char escape = '\\';

        private bool allowSelfEscape = false;

        public SekvapLanguage()
        {
        }

        /// <summary>
        /// EXPERIMENTAL. Enabling this will use the old self-escape rule. Not recommanded because not tested.
        /// </summary>
        public bool AllowSelfEscape
        {
            get { return this.allowSelfEscape; }
            set { this.allowSelfEscape = value; }
        }

        /// <summary>
        /// For unit tests. When false, empty kevaps will be serialized too.
        /// </summary>
        public bool SkipSerializeEmpty { get; set; } = true;

        public static void AddToResult(IList<KeyValuePair<string, string>> collection, string key, string value)
        {
            collection.Add(new KeyValuePair<string, string>(key, value));
        }

        /// <summary>
        /// Parse a string.
        /// </summary>
        /// <param name="value">the value to parse</param>
        /// <returns>a collection of key-value pairs</returns>
        public IList<KeyValuePair<string, string>> Parse(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            var result = new List<KeyValuePair<string, string>>();
            bool isStart = true, isKey = true, isValue = false, isEnd = false;
            string capturedKey = null;
            var sb = new StringBuilder();
            for (int i = 0; i <= value.Length; i++)
            {
                // prepare current char
                char c, cp1;
                if (i == value.Length)
                {
                    // prepare for last char
                    c = char.MinValue;
                    cp1 = char.MinValue;
                    isEnd = true;
                }
                else
                {
                    // prepare for any other char
                    c = value[i];
                    cp1 = (i + 1) < value.Length ? value[i + 1] : char.MinValue;
                }

                if (isStart)
                {
                    if (allowSelfEscape && c == ';' && cp1 == ';')
                    {
                        // reached a self-escaped ;
                        ////i++;
                    }
                    else if (c == '\\' && cp1 == ';')
                    {
                        // reached a slash-escaped ;
                        ////sb.Append(cp1);
                        ////i++;
                    }
                    else if (allowSelfEscape && (c == ';' && cp1 != ';' || isEnd))
                    {
                        // end of start value
                        var capturedValue = sb.ToString();
                        sb.Clear();
                        AddToResult(result, "Value", capturedValue);
                        i++;
                        isStart = false;
                        isKey = true;
                        sb.Append(cp1);
                        continue;
                    }
                    else if (!allowSelfEscape && (c == ';' || isEnd))
                    {
                        // end of start value
                        var capturedValue = sb.ToString();
                        sb.Clear();
                        AddToResult(result, "Value", capturedValue);
                        isStart = false;
                        isKey = true;
                        continue;
                    }
                }
                
                if (isKey)
                {
                    if (allowSelfEscape && ((c == '=' && cp1 == '=') || (c == ';' && cp1 == ';')))
                    {
                        sb.Append(cp1);
                        i++;
                    }
                    else if (!allowSelfEscape && ((c == '\\' && cp1 == '=') || (c == '\\' && cp1 == ';')))
                    {
                        sb.Append(cp1);
                        i++;
                    }
                    else if ((c == ';' && cp1 != ';') || (c == ';' && cp1 == ';' && !allowSelfEscape) || isEnd)
                    {
                        if (isValue)
                        {
                            // end of start part
                            var capturedValue = sb.ToString();
                            sb.Clear();
                            AddToResult(result, capturedKey, capturedValue);
                        }
                        else
                        {
                            capturedKey = sb.ToString();
                            sb.Clear();
                            AddToResult(result, capturedKey, null);
                        }

                        isValue = false;
                        continue;
                    }
                    else if (c == '=' && cp1 != '=')
                    {
                        // end of start part
                        capturedKey = sb.ToString();
                        sb.Clear();
                        isValue = true;
                        isStart = false;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Serializes a set of key-value pairs in Sekvap.
        /// </summary>
        /// <param name="values"></param>
        /// <returns>a Sekvap value</returns>
        public string Serialize(IEnumerable<KeyValuePair<string, string>> values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            var collection = values as ICollection<KeyValuePair<string, string>>;
            if (collection != null)
            {
                return this.Serialize(collection);
            }

            var sb = new StringBuilder();
            int i = 0;
            foreach (var item in values)
            {
                if (!(i++ == 0 && "Value".Equals(item.Key)))
                {
                    sb.Append(";");
                }

                EscapeKey(item.Key, sb);
                sb.Append("=");
                EscapeValue(item.Value, sb);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Serializes a collection of key-value pairs in Sekvap.
        /// </summary>
        /// <param name="values"></param>
        /// <returns>a Sekvap value</returns>
        public string Serialize(ICollection<KeyValuePair<string, string>> values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            var sb = new StringBuilder();
            int i = -1, skip = -1;
            foreach (var item in values)
            {
                i++;
                if ("Value".Equals(item.Key))
                {
                    skip = i;
                    EscapeValue(item.Value, sb);
                    break;
                }
            }

            i = -1;
            foreach (var item in values)
            {
                i++;
                if (skip == i)
                    continue;

                if (sb.Length > 0)
                {
                    sb.Append(";"); 
                }

                if (string.IsNullOrEmpty(item.Key) && string.IsNullOrEmpty(item.Value) && this.SkipSerializeEmpty)
                {
                }
                else
                {
                    EscapeKey(item.Key, sb);
                    if (!string.IsNullOrEmpty(item.Value))
                    {
                        sb.Append("=");
                        EscapeValue(item.Value, sb);
                    }
                }
            }

            return sb.ToString();
        }

        private string EscapeKey(string key, StringBuilder sb)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            var pos = key.IndexOfAny(keyChars);
            if (pos < 0)
            {
                if (sb != null)
                {
                    sb.Append(key);
                }

                return key;
            }

            sb = sb ?? new StringBuilder(key.Length + 2);
            for (int i = 0; i < key.Length; i++)
            {
                var c = key[i];
                if (keyChars.Contains(c))
                {
                    sb.Append(allowSelfEscape ? c : '\\');
                }

                sb.Append(c);
            }

            return sb.ToString();
        }

        private string EscapeValue(string value, StringBuilder sb)
        {
            if (value == null)
                return null;

            var pos = value.IndexOfAny(valueChars);
            if (pos < 0)
            {
                if (sb != null)
                {
                    sb.Append(value);
                }

                return value;
            }

            sb = sb ?? new StringBuilder(value.Length + 2);
            for (int i = 0; i < value.Length; i++)
            {
                var c = value[i];
                if (keyChars.Contains(c))
                {
                    sb.Append(allowSelfEscape ? c : '\\');
                }

                sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
