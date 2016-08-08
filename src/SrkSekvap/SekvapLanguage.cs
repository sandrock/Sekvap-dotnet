// Sekvap-dotnet
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

        public SekvapLanguage()
        {
        }

        public static void AddToResult(List<KeyValuePair<string, string>> collection, string key, string value)
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
            bool isStart = true, isKey = false, isValue = false, isEnd = false;
            string capturedKey = null;
            int captureStartIndex = 0, captureEndIndex, captureLength;
            for (int i = captureStartIndex; i <= value.Length; i++)
            {
                char c, cp1;
                if (i == value.Length)
                {
                    c = char.MinValue;
                    cp1 = char.MinValue;
                    isEnd = true;
                    captureEndIndex = i - 1;
                    captureLength = i - captureStartIndex;
                }
                else
                {
                    c = value[i];
                    cp1 = (i + 1) < value.Length ? value[i + 1] : char.MinValue;
                    captureEndIndex = i;
                    captureLength = i - captureStartIndex;
                }

                if (isStart)
                {
                    if (c == ';' && cp1 == ';')
                    {
                        i++;
                    }
                    else if (c == ';' && cp1 != ';' || isEnd)
                    {
                        // end of start part
                        AddToResult(result, "Value", value.Substring(captureStartIndex, captureLength));
                        i++;
                        isStart = false;
                        isKey = true;
                        captureStartIndex = i;
                    }
                }
                else if (isKey)
                {
                    if ((c == '=' && cp1 == '=') || (c == ';' && cp1 == ';'))
                    {
                        i++;
                    }
                    else if (c == ';' && cp1 != ';' || isEnd)
                    {
                        if (isValue)
                        {
                            // end of start part
                            var capturedValue = value.Substring(captureStartIndex, captureLength);
                            AddToResult(result, capturedKey, capturedValue);
                        }
                        else
                        {
                            capturedKey = value.Substring(captureStartIndex, captureLength);
                            AddToResult(result, capturedKey, null);
                        }
                        isValue = false;
                        captureStartIndex = i + 1;
                    }
                    else if (c == '=' && cp1 != '=')
                    {
                        // end of start part
                        capturedKey = value.Substring(captureStartIndex, captureLength);
                        isValue = true;
                        captureStartIndex = i + 1;
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

            if (values is ICollection<KeyValuePair<string, string>>)
            {
                return this.Serialize((ICollection<KeyValuePair<string, string>>)values);
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

                sb.Append(";");
                EscapeKey(item.Key, sb);
                sb.Append("=");
                EscapeValue(item.Value, sb);
            }

            return sb.ToString();
        }

        private static string EscapeKey(string value)
        {
            return EscapeKey(value, null);
        }

        private static string EscapeKey(string value, StringBuilder sb)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("The value cannot be empty", "value");

            var pos = value.IndexOfAny(keyChars);
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
                    sb.Append(c);
                }

                sb.Append(c);
            }

            return sb.ToString();
        }

        private static string EscapeValue(string value)
        {
            if (value == null)
                return null;

            return EscapeValue(value, null);
        }

        private static string EscapeValue(string value, StringBuilder sb)
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
                    sb.Append(c);
                }

                sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
