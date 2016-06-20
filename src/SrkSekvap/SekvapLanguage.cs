
namespace SrkSekvap
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SekvapLanguage
    {
        public SekvapLanguage()
        {
        }

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

        public static void AddToResult(List<KeyValuePair<string, string>> collection, string key, string value)
        {
            collection.Add(new KeyValuePair<string, string>(key, value));
        }
    }
}
