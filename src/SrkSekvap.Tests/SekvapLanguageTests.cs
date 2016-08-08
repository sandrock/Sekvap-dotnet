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

namespace SrkSekvap.Tests
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SrkSekvap;
    using System.Collections.Generic;

    [TestClass]
    public class SekvapLanguageTests
    {
        [TestClass]
        public class Ctor0
        {
            [TestMethod]
            public void Works()
            {
                new SekvapLanguage();
            }
        }

        [TestClass]
        public class ParseMethod
        {
            [TestMethod, ExpectedException(typeof(ArgumentNullException))]
            public void NullInput()
            {
                var lang = new SekvapLanguage();
                string input = null;
                lang.Parse(input);
            }

            [TestMethod]
            public void NoValue()
            {
                var lang = new SekvapLanguage();
                string input = string.Empty;
                var result = lang.Parse(input);
                Assert.IsNotNull(result);
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual("Value", result[0].Key);
                Assert.AreEqual(string.Empty, result[0].Value);
            }

            [TestMethod]
            public void SimpleValue()
            {
                var lang = new SekvapLanguage();
                var parts = new string[]
                {
                    "hello world",
                };
                string input = parts[0];
                var result = lang.Parse(input);
                Assert.IsNotNull(result);
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual("Value", result[0].Key);
                Assert.AreEqual(parts[0], result[0].Value);
            }

            [TestMethod]
            public void SimpleValueWithEscapedSeparator()
            {
                var lang = new SekvapLanguage();
                var parts = new string[]
                {
                    "hello ;; world",
                };
                string input = parts[0];
                var result = lang.Parse(input);
                Assert.IsNotNull(result);
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual("Value", result[0].Key);
                Assert.AreEqual(parts[0], result[0].Value);
            }

            [TestMethod]
            public void SimpleValue_PlusOneKevap()
            {
                var lang = new SekvapLanguage();
                var parts = new string[]
                {
                    "hello world",
                    ";", "Name", "=", "John Smith",
                };
                string input = string.Join(string.Empty, parts);
                var result = lang.Parse(input);
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Count);
                int i = -1;
                Assert.AreEqual("Value", result[++i].Key);
                Assert.AreEqual(parts[0], result[i].Value);
                Assert.AreEqual(parts[2], result[++i].Key);
                Assert.AreEqual(parts[4], result[i].Value);
            }

            [TestMethod]
            public void SimpleValue_PlusTwoKevap()
            {
                var lang = new SekvapLanguage();
                var parts = new string[]
                {
                    "hello world",
                    ";", "Name", "=", "John Smith",
                    ";", "Foo", "=", "Bar",
                };
                string input = string.Join(string.Empty, parts);
                var result = lang.Parse(input);
                Assert.IsNotNull(result);
                Assert.AreEqual(3, result.Count);
                int i = -1;
                Assert.AreEqual("Value", result[++i].Key);
                Assert.AreEqual(parts[0], result[i].Value);
                Assert.AreEqual(parts[2], result[++i].Key);
                Assert.AreEqual(parts[4], result[i].Value);
                Assert.AreEqual(parts[6], result[++i].Key);
                Assert.AreEqual(parts[8], result[i].Value);
            }

            [TestMethod]
            public void SimpleValue_PlusOneEmptyKevap()
            {
                var lang = new SekvapLanguage();
                var parts = new string[]
                {
                    "hello world",
                    ";", "Name",
                };
                string input = string.Join(string.Empty, parts);
                var result = lang.Parse(input);
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Count);
                int i = -1;
                Assert.AreEqual("Value", result[++i].Key);
                Assert.AreEqual(parts[0], result[i].Value);
                Assert.AreEqual(parts[2], result[++i].Key);
                Assert.AreEqual(null, result[i].Value);
            }

            [TestMethod]
            public void SimpleValue_PlusOneEmptyEqualKevap()
            {
                var lang = new SekvapLanguage();
                var parts = new string[]
                {
                    "hello world",
                    ";", "Name", "=",
                };
                string input = string.Join(string.Empty, parts);
                var result = lang.Parse(input);
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Count);
                int i = -1;
                Assert.AreEqual("Value", result[++i].Key);
                Assert.AreEqual(parts[0], result[i].Value);
                Assert.AreEqual(parts[2], result[++i].Key);
                Assert.AreEqual(string.Empty, result[i].Value);
            }

            [TestMethod]
            public void SimpleEmptyValue_PlusTwoKevap()
            {
                var lang = new SekvapLanguage();
                var parts = new string[]
                {
                    "hello world",
                    ";", "Name", 
                    ";", "Foo", "=", "Bar",
                };
                string input = string.Join(string.Empty, parts);
                var result = lang.Parse(input);
                Assert.IsNotNull(result);
                Assert.AreEqual(3, result.Count);
                int i = -1;
                Assert.AreEqual("Value", result[++i].Key);
                Assert.AreEqual(parts[0], result[i].Value);
                Assert.AreEqual(parts[2], result[++i].Key);
                Assert.AreEqual(null, result[i].Value);
                Assert.AreEqual(parts[4], result[++i].Key);
                Assert.AreEqual(parts[6], result[i].Value);
            }

            [TestMethod]
            public void SimpleEmptyEqualValue_PlusTwoKevap()
            {
                var lang = new SekvapLanguage();
                var parts = new string[]
                {
                    "hello world",
                    ";", "Name", "=", 
                    ";", "Foo", "=", "Bar",
                };
                string input = string.Join(string.Empty, parts);
                var result = lang.Parse(input);
                Assert.IsNotNull(result);
                Assert.AreEqual(3, result.Count);
                int i = -1;
                Assert.AreEqual("Value", result[++i].Key);
                Assert.AreEqual(parts[0], result[i].Value);
                Assert.AreEqual(parts[2], result[++i].Key);
                Assert.AreEqual(string.Empty, result[i].Value);
                Assert.AreEqual(parts[5], result[++i].Key);
                Assert.AreEqual(parts[7], result[i].Value);
            }

            [TestMethod]
            public void SimpleValue_PlusTwoEmptyKevap()
            {
                var lang = new SekvapLanguage();
                var parts = new string[]
                {
                    "hello world",
                    ";", "Name", 
                    ";", "Foo",
                };
                string input = string.Join(string.Empty, parts);
                var result = lang.Parse(input);
                Assert.IsNotNull(result);
                Assert.AreEqual(3, result.Count);
                int i = -1;
                Assert.AreEqual("Value", result[++i].Key);
                Assert.AreEqual(parts[0], result[i].Value);
                Assert.AreEqual(parts[2], result[++i].Key);
                Assert.AreEqual(null, result[i].Value);
                Assert.AreEqual(parts[4], result[++i].Key);
                Assert.AreEqual(null, result[i].Value);
            }

            [TestMethod]
            public void SimpleValue_PlusTwoEmptyEqualKevap()
            {
                var lang = new SekvapLanguage();
                var parts = new string[]
                {
                    "hello world",
                    ";", "Name", "=",
                    ";", "Foo", "=",
                };
                string input = string.Join(string.Empty, parts);
                var result = lang.Parse(input);
                Assert.IsNotNull(result);
                Assert.AreEqual(3, result.Count);
                int i = -1;
                Assert.AreEqual("Value", result[++i].Key);
                Assert.AreEqual(parts[0], result[i].Value);
                Assert.AreEqual(parts[2], result[++i].Key);
                Assert.AreEqual(string.Empty, result[i].Value);
                Assert.AreEqual(parts[5], result[++i].Key);
                Assert.AreEqual(string.Empty, result[i].Value);
            }

            [TestMethod]
            public void SimpleValue_PlusOneEmptyKevapAndOneEmptyEqualKevap()
            {
                var lang = new SekvapLanguage();
                var parts = new string[]
                {
                    "hello world",
                    ";", "Name",
                    ";", "Foo", "=",
                };
                string input = string.Join(string.Empty, parts);
                var result = lang.Parse(input);
                Assert.IsNotNull(result);
                Assert.AreEqual(3, result.Count);
                int i = -1;
                Assert.AreEqual("Value", result[++i].Key);
                Assert.AreEqual(parts[0], result[i].Value);
                Assert.AreEqual(parts[2], result[++i].Key);
                Assert.AreEqual(null, result[i].Value);
                Assert.AreEqual(parts[4], result[++i].Key);
                Assert.AreEqual(string.Empty, result[i].Value);
            }

            [TestMethod]
            public void SimpleValue_PlusOneEmptyKevap_EndsWithSemicolon()
            {
                var lang = new SekvapLanguage();
                var parts = new string[]
                {
                    "hello world",
                    ";", "Name",
                    ";",
                };
                string input = string.Join(string.Empty, parts);
                var result = lang.Parse(input);
                Assert.IsNotNull(result);
                Assert.AreEqual(3, result.Count);
                int i = -1;
                Assert.AreEqual("Value", result[++i].Key);
                Assert.AreEqual(parts[0], result[i].Value);
                Assert.AreEqual(parts[2], result[++i].Key);
                Assert.AreEqual(null, result[i].Value);
            }
        }

        [TestClass]
        public class SerializeEnumerableMethod
        {
            [TestMethod, ExpectedException(typeof(ArgumentNullException))]
            public void Arg0IsNull_Throws()
            {
                var target = new SekvapLanguage();
                target.Serialize(default(IEnumerable<KeyValuePair<string, string>>));
            }

            [TestMethod]
            public void NoValues()
            {
                var source = new List<KeyValuePair<string, string>>();
                var target = new SekvapLanguage();
                var result = target.Serialize(source.AsEnumerable());
                Assert.IsNotNull(result);
                Assert.AreEqual(0, result.Length);
            }

            [TestMethod]
            public void Value()
            {
                var source = new Dictionary<string, string>();
                source.Add("Value", "Hello");
                var target = new SekvapLanguage();
                var result = target.Serialize(source.AsEnumerable());
                Assert.IsNotNull(result);
                Assert.AreEqual("Hello", result);
            }

            [TestMethod]
            public void Value_Item()
            {
                var source = new Dictionary<string, string>();
                source.Add("Value", "Hello");
                source.Add("Foo", "Bar");
                var target = new SekvapLanguage();
                var result = target.Serialize(source.AsEnumerable());
                Assert.IsNotNull(result);
                Assert.AreEqual("Hello;Foo=Bar", result);
            }

            [TestMethod]
            public void Value_Item_Value()
            {
                var source = new List<KeyValuePair<string, string>>();
                source.Add(new KeyValuePair<string, string>("Value", "Hello"));
                source.Add(new KeyValuePair<string, string>("Foo", "Bar"));
                source.Add(new KeyValuePair<string, string>("Value", "Yop"));
                var target = new SekvapLanguage();
                var result = target.Serialize(source.AsEnumerable());
                Assert.IsNotNull(result);
                Assert.AreEqual("Hello;Foo=Bar;Value=Yop", result);
            }
        }

        [TestClass]
        public class SerializeCollectionMethod
        {
            [TestMethod, ExpectedException(typeof(ArgumentNullException))]
            public void Arg0IsNull_Throws()
            {
                var target = new SekvapLanguage();
                target.Serialize(default(ICollection<KeyValuePair<string, string>>));
            }

            [TestMethod]
            public void NoValues()
            {
                var source = new List<KeyValuePair<string, string>>();
                var target = new SekvapLanguage();
                var result = target.Serialize(source);
                Assert.IsNotNull(result);
                Assert.AreEqual(0, result.Length);
            }

            [TestMethod]
            public void Value()
            {
                var source = new Dictionary<string, string>();
                source.Add("Value", "Hello");
                var target = new SekvapLanguage();
                var result = target.Serialize(source);
                Assert.IsNotNull(result);
                Assert.AreEqual("Hello", result);
            }

            [TestMethod]
            public void Value_Item()
            {
                var source = new Dictionary<string, string>();
                source.Add("Value", "Hello");
                source.Add("Foo", "Bar");
                var target = new SekvapLanguage();
                var result = target.Serialize(source);
                Assert.IsNotNull(result);
                Assert.AreEqual("Hello;Foo=Bar", result);
            }

            [TestMethod]
            public void Value_Item_Value()
            {
                var source = new List<KeyValuePair<string, string>>();
                source.Add(new KeyValuePair<string, string>("Value", "Hello"));
                source.Add(new KeyValuePair<string, string>("Foo", "Bar"));
                source.Add(new KeyValuePair<string, string>("Value", "Yop"));
                var target = new SekvapLanguage();
                var result = target.Serialize(source);
                Assert.IsNotNull(result);
                Assert.AreEqual("Hello;Foo=Bar;Value=Yop", result);
            }
        }
    }
}
