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
        /// EXPERIMENTAL. Enabling this will use the old self-escape rule. Not recommended because not tested.
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
            bool isStart = true, isKey = false, isEnd = false;
            string capturedKey = "Value", capturedValue = null;
            var sb = new StringBuilder();
            char c1, c2, c3;
            bool hasEqual;   // this helps set an empty string or null in a value
            hasEqual = true; // this allows the start Value to be an empty string
            int escapeCounter = 0;
            for (int i = -1; i <= value.Length; i++)
            {
                // prepare current char
                c1 = (i < value.Length && i >= 0) ? value[i] : char.MinValue; // previous (for escape char)
                c2 = ((i + 1) < value.Length) ? value[i + 1] : char.MinValue; // current  (for decision)
                c3 = ((i + 2) < value.Length) ? value[i + 2] : char.MinValue; // next     (for self escape char)
                isEnd = (i + 1) == value.Length;
                escapeCounter--;

                bool isEqual = c2 == '=';
                bool isSemi  = c2 == ';';
                bool isAnti  = !allowSelfEscape && c2 == '\\'   // indicates the char is the escape char
                            ||  allowSelfEscape && c2 == c3 && (c3 == '=' || c3 == ';');
                bool isEscaped = !allowSelfEscape && c1 == '\\' // indicates the char is escaped
                              ||  allowSelfEscape && c1 == c2;

                if (isEqual && !isEscaped && (isStart || isKey) && sb.Length > 0)
                {
                    // on equal sign, accept a key
                    capturedKey = sb.ToString();
                    sb.Clear();
                    isStart = false;
                    isKey = false;
                    hasEqual = true;
                }
                else if (isSemi && !isAnti && !isEscaped || isEnd)
                {
                    // on semicolon/end sign, capture a key+value
                    if (capturedKey != null)
                    {
                        capturedValue = sb.ToString();
                    }
                    else
                    {
                        capturedKey = sb.ToString();
                    }

                    sb.Clear();
                    AddToResult(result, capturedKey, capturedValue ?? (hasEqual ? string.Empty : null));
                    capturedKey = capturedValue = null;
                    isStart = false;
                    isKey = true;
                    hasEqual = false;
                }
                else
                {
                    if (escapeCounter == 1 && !isEqual && !isSemi)
                    {
                        // escaped escaping char
                        sb.Append(c1);
                    }

                    if (isAnti)
                    {
                        // found escaping char, decide what to do at next iteration
                        escapeCounter = 2;
                    }
                    else
                    {
                        // found a common char
                        sb.Append(c2);
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
