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
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using SrkSekvap;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Diagnostics;
    using Xunit;

    public class SekvapLanguageTests
    {
        public class Ctor0
        {
            [Fact]
            public void Works()
            {
                new SekvapLanguage();
            }
        }

        public class ParseMethod
        {
            [Fact]
            public void NullInput()
            {
                var lang = new SekvapLanguage();
                string input = null;
                Assert.Throws<ArgumentNullException>(() => lang.Parse(input));
            }

            [Fact]
            public void NoValue()
            {
                var lang = new SekvapLanguage();
                string input = string.Empty;
                var result = lang.Parse(input);
                Assert.NotNull(result);
                Assert.Equal(1, result.Count);
                Assert.Equal("Value", result[0].Key);
                Assert.Equal(string.Empty, result[0].Value);
            }

            [Fact]
            public void SimpleValue()
            {
                var lang = new SekvapLanguage();
                var parts = new string[]
                {
                    "hello world",
                };
                string input = parts[0];
                var result = lang.Parse(input);
                Assert.NotNull(result);
                Assert.Equal(1, result.Count);
                Assert.Equal("Value", result[0].Key);
                Assert.Equal(parts[0], result[0].Value);
            }

            [Fact]
            public void SimpleValue_Escape1()
            {
                var lang = new SekvapLanguage();
                var parts = new string[]
                {
                    "\\;",
                };
                string input = parts[0];
                var result = lang.Parse(input);
                Assert.NotNull(result);
                Assert.Equal(1, result.Count);

                Assert.Equal("Value", result[0].Key);
                Assert.Equal(";", result[0].Value);
            }

            [Fact]
            public void SimpleValue_Escape2()
            {
                var lang = new SekvapLanguage();
                var parts = new string[]
                {
                    "\\;aaa",
                };
                string input = parts[0];
                var result = lang.Parse(input);
                Assert.NotNull(result);
                Assert.Equal(1, result.Count);

                Assert.Equal("Value", result[0].Key);
                Assert.Equal(";aaa", result[0].Value);
            }

            [Fact]
            public void SimpleValueWithEscapedSeparator_Old()
            {
                var lang = new SekvapLanguage();
                lang.AllowSelfEscape = true;
                var parts = new string[]
                {
                    "hello ;; world",
                };
                string input = parts[0];
                var result = lang.Parse(input);
                Assert.NotNull(result);
                Assert.Equal(1, result.Count);

                Assert.Equal("Value", result[0].Key);
                Assert.Equal("hello ; world", result[0].Value);
            }

            [Fact]
            public void SimpleValueWithEscapedSeparator_Old_End()
            {
                var lang = new SekvapLanguage();
                lang.AllowSelfEscape = true;
                var parts = new string[]
                {
                    "hello;;",
                };
                string input = parts[0];
                var result = lang.Parse(input);
                Assert.NotNull(result);
                Assert.Equal(1, result.Count);

                Assert.Equal("Value", result[0].Key);
                Assert.Equal("hello;", result[0].Value);
            }

            [Fact]
            public void SimpleValueWithEscapedSeparator_Old_Begin()
            {
                var lang = new SekvapLanguage();
                lang.AllowSelfEscape = true;
                var parts = new string[]
                {
                    ";;hello",
                };
                string input = parts[0];
                var result = lang.Parse(input);
                Assert.NotNull(result);
                Assert.Equal(1, result.Count);

                Assert.Equal("Value", result[0].Key);
                Assert.Equal(";hello", result[0].Value);
            }

            [Fact]
            public void SimpleValueWithEscapedSeparator()
            {
                var lang = new SekvapLanguage();
                var parts = new string[]
                {
                    "hello ;; world",
                };
                string input = parts[0];
                var result = lang.Parse(input);
                Assert.NotNull(result);
                Assert.Equal(3, result.Count);

                Assert.Equal("Value", result[0].Key);
                Assert.Equal("hello ", result[0].Value);

                Assert.Equal(string.Empty, result[1].Key);
                Assert.Equal(null, result[1].Value);

                Assert.Equal(" world", result[2].Key);
                Assert.Equal(null, result[2].Value);
            }

            [Fact]
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
                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
                int i = -1;
                Assert.Equal("Value", result[++i].Key);
                Assert.Equal(parts[0], result[i].Value);
                Assert.Equal(parts[2], result[++i].Key);
                Assert.Equal(parts[4], result[i].Value);
            }

            [Fact]
            public void SimpleValue_PlusOneKevap_OneEscapedKey1()
            {
                var lang = new SekvapLanguage();
                var parts = new string[]
                {
                    "hello world",
                    ";", "Na\\;me", "=", "John Smith",
                };
                string input = string.Join(string.Empty, parts);
                var result = lang.Parse(input);
                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
                int i = -1;

                Assert.Equal("Value", result[++i].Key);
                Assert.Equal(parts[0], result[i].Value);

                Assert.Equal("Na;me", result[++i].Key);
                Assert.Equal(parts[4], result[i].Value);
            }

            [Fact]
            public void SimpleValue_PlusOneKevap_OneEscapedKey2()
            {
                var lang = new SekvapLanguage();
                var parts = new string[]
                {
                    "hello world",
                    ";", "Name\\;", "=", "John Smith",
                };
                string input = string.Join(string.Empty, parts);
                var result = lang.Parse(input);
                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
                int i = -1;

                Assert.Equal("Value", result[++i].Key);
                Assert.Equal(parts[0], result[i].Value);

                Assert.Equal("Name;", result[++i].Key);
                Assert.Equal(parts[4], result[i].Value);
            }

            [Fact]
            public void SimpleValue_PlusOneKevap_OneEscapedKey3()
            {
                var lang = new SekvapLanguage();
                var parts = new string[]
                {
                    "hello world",
                    ";", "\\;Name", "=", "John Smith",
                };
                string input = string.Join(string.Empty, parts);
                var result = lang.Parse(input);
                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
                int i = -1;

                Assert.Equal("Value", result[++i].Key);
                Assert.Equal(parts[0], result[i].Value);

                Assert.Equal(";Name", result[++i].Key);
                Assert.Equal(parts[4], result[i].Value);
            }

            [Fact]
            public void SimpleValue_PlusOneKevap_OneEscapedValue()
            {
                var lang = new SekvapLanguage();
                var parts = new string[]
                {
                    "hello world",
                    ";", "Name", "=", "John\\;Smith",
                };
                string input = string.Join(string.Empty, parts);
                var result = lang.Parse(input);
                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
                int i = -1;

                Assert.Equal("Value", result[++i].Key);
                Assert.Equal(parts[0], result[i].Value);

                Assert.Equal(parts[2], result[++i].Key);
                Assert.Equal("John;Smith", result[i].Value);
            }

            [Fact]
            public void SimpleValue_PlusTwoKevap()
            {
                var lang = new SekvapLanguage();
                var parts = new string[]
                {
                    "helo",
                    ";", "Name", "=", "John Smith",
                    ";", "Foo", "=", "Bar",
                };
                string input = string.Join(string.Empty, parts);
                var result = lang.Parse(input);
                Assert.NotNull(result);
                Assert.Equal(3, result.Count);
                int i = -1;
                Assert.Equal("Value", result[++i].Key);
                Assert.Equal(parts[0], result[i].Value);
                Assert.Equal(parts[2], result[++i].Key);
                Assert.Equal(parts[4], result[i].Value);
                Assert.Equal(parts[6], result[++i].Key);
                Assert.Equal(parts[8], result[i].Value);
            }

            [Fact]
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
                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
                int i = -1;
                Assert.Equal("Value", result[++i].Key);
                Assert.Equal(parts[0], result[i].Value);
                Assert.Equal(parts[2], result[++i].Key);
                Assert.Equal(null, result[i].Value);
            }

            [Fact]
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
                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
                int i = -1;
                Assert.Equal("Value", result[++i].Key);
                Assert.Equal(parts[0], result[i].Value);
                Assert.Equal(parts[2], result[++i].Key);
                Assert.Equal(string.Empty, result[i].Value);
            }

            [Fact]
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
                Assert.NotNull(result);
                Assert.Equal(3, result.Count);
                int i = -1;
                Assert.Equal("Value", result[++i].Key);
                Assert.Equal(parts[0], result[i].Value);
                Assert.Equal(parts[2], result[++i].Key);
                Assert.Equal(null, result[i].Value);
                Assert.Equal(parts[4], result[++i].Key);
                Assert.Equal(parts[6], result[i].Value);
            }

            [Fact]
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
                Assert.NotNull(result);
                Assert.Equal(3, result.Count);
                int i = -1;
                Assert.Equal("Value", result[++i].Key);
                Assert.Equal(parts[0], result[i].Value);
                Assert.Equal(parts[2], result[++i].Key);
                Assert.Equal(string.Empty, result[i].Value);
                Assert.Equal(parts[5], result[++i].Key);
                Assert.Equal(parts[7], result[i].Value);
            }

            [Fact]
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
                Assert.NotNull(result);
                Assert.Equal(3, result.Count);
                int i = -1;
                Assert.Equal("Value", result[++i].Key);
                Assert.Equal(parts[0], result[i].Value);
                Assert.Equal(parts[2], result[++i].Key);
                Assert.Equal(null, result[i].Value);
                Assert.Equal(parts[4], result[++i].Key);
                Assert.Equal(null, result[i].Value);
            }

            [Fact]
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
                Assert.NotNull(result);
                Assert.Equal(3, result.Count);
                int i = -1;
                Assert.Equal("Value", result[++i].Key);
                Assert.Equal(parts[0], result[i].Value);
                Assert.Equal(parts[2], result[++i].Key);
                Assert.Equal(string.Empty, result[i].Value);
                Assert.Equal(parts[5], result[++i].Key);
                Assert.Equal(string.Empty, result[i].Value);
            }

            [Fact]
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
                Assert.NotNull(result);
                Assert.Equal(3, result.Count);
                int i = -1;
                Assert.Equal("Value", result[++i].Key);
                Assert.Equal(parts[0], result[i].Value);
                Assert.Equal(parts[2], result[++i].Key);
                Assert.Equal(null, result[i].Value);
                Assert.Equal(parts[4], result[++i].Key);
                Assert.Equal(string.Empty, result[i].Value);
            }

            [Fact]
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
                Assert.NotNull(result);
                Assert.Equal(3, result.Count);
                int i = -1;
                Assert.Equal("Value", result[++i].Key);
                Assert.Equal(parts[0], result[i].Value);
                Assert.Equal(parts[2], result[++i].Key);
                Assert.Equal(null, result[i].Value);
            }

            [Fact]
            public void ConnectionString1()
            {
                var lang = new SekvapLanguage();
                var parts = new string[]
                {
                    "Data Source", "=", "myServerAddress", ";",
                    "Initial Catalog", "=", "myDataBase", ";",
                    "Integrated Security", "=", "SSPI", ";",
                    "User ID", "=", "myDomain\\myUsername", ";",
                    "Password", "=", "myPass\\=word", ";",
                };
                string input = string.Concat(parts);
                var result = lang.Parse(input);
                Assert.NotNull(result);
                Assert.Equal(6, result.Count);
                int i = -1;
                ////Assert.Equal("Value", result[++i].Key); // Value = 
                ////Assert.Equal(null, result[i].Value); //       = null
                Assert.Equal(parts[0], result[++i].Key); // Data Source = 
                Assert.Equal(parts[2], result[i].Value); //             = myServerAddress
                Assert.Equal(parts[4], result[++i].Key); // Initial Catalog = 
                Assert.Equal(parts[6], result[i].Value); //                 = myDataBase
                Assert.Equal(parts[8], result[++i].Key); // Integrated Security = 
                Assert.Equal(parts[10], result[i].Value); //                     = SSPI
                Assert.Equal(parts[12], result[++i].Key); // User ID =
                Assert.Equal(parts[14], result[i].Value); //         = myDomain\\myUsername
                Assert.Equal(parts[16], result[++i].Key); // Password = 
                Assert.Equal("myPass=word", result[i].Value); //          = myPass=word
                Assert.Equal("", result[++i].Key); // "" = 
                Assert.Equal(null, result[i].Value); //       = null
            }

            [Fact]
            public void ConsecutiveEmptyKevap1()
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
                Assert.NotNull(result);
                Assert.Equal(3, result.Count);
                int i = -1;

                // "Value" == "hello world"
                Assert.Equal("Value", result[++i].Key);
                Assert.Equal(parts[0], result[i].Value);

                // "Name" == null
                Assert.Equal(parts[2], result[++i].Key);
                Assert.Equal(null, result[i].Value);

                // "" == null
                Assert.Equal(string.Empty, result[++i].Key);
                Assert.Equal(null, result[i].Value);
            }

            [Fact]
            public void ConsecutiveEmptyKevap2()
            {
                var lang = new SekvapLanguage();
                var parts = new string[]
                {
                    // hello world;Name;;
                    "hello world",
                    ";", "Name",
                    ";",
                    ";",
                };
                string input = string.Join(string.Empty, parts);
                var result = lang.Parse(input);
                Assert.NotNull(result);
                Assert.Equal(4, result.Count);
                int i = -1;

                // "Value" == "hello world"
                Assert.Equal("Value", result[++i].Key);
                Assert.Equal(parts[0], result[i].Value);

                // "Name;;" == null
                Assert.Equal("Name", result[++i].Key);
                Assert.Equal(null, result[i].Value);

                // "" == null
                Assert.Equal(string.Empty, result[++i].Key);
                Assert.Equal(null, result[i].Value);

                // "" == null
                Assert.Equal(string.Empty, result[++i].Key);
                Assert.Equal(null, result[i].Value);
            }

            [Fact]
            public void ConsecutiveEmptyKevap3()
            {
                var lang = new SekvapLanguage();
                var parts = new string[]
                {
                    // hello world;Name=;;
                    "hello world",
                    ";", "Name", "=",
                    ";",
                    ";",
                };
                string input = string.Join(string.Empty, parts);
                var result = lang.Parse(input);
                Assert.NotNull(result);
                Assert.Equal(4, result.Count);
                int i = -1;

                // "Value" == "hello world"
                Assert.Equal("Value", result[++i].Key);
                Assert.Equal(parts[0], result[i].Value);

                // "Name" == ";"
                Assert.Equal("Name", result[++i].Key);
                Assert.Equal(string.Empty, result[i].Value);

                // "" == null
                Assert.Equal(string.Empty, result[++i].Key);
                Assert.Equal(null, result[i].Value);

                // "" == null
                Assert.Equal(string.Empty, result[++i].Key);
                Assert.Equal(null, result[i].Value);
            }

            [Fact]
            public void ConsecutiveEmptyKevap4()
            {
                var lang = new SekvapLanguage();
                var parts = new string[]
                {
                    // hello world;Name=;;;
                    "hello world",
                    ";", "Name", "=",
                    ";",
                    ";",
                    ";",
                };
                string input = string.Join(string.Empty, parts);
                var result = lang.Parse(input);
                Assert.NotNull(result);
                Assert.Equal(5, result.Count);
                int i = -1;

                // "Value" == "hello world"
                Assert.Equal("Value", result[++i].Key);
                Assert.Equal(parts[0], result[i].Value);

                // "Name" == ";"
                Assert.Equal("Name", result[++i].Key);
                Assert.Equal(string.Empty, result[i].Value);

                // "" == null
                Assert.Equal(string.Empty, result[++i].Key);
                Assert.Equal(null, result[i].Value);

                // "" == null
                Assert.Equal(string.Empty, result[++i].Key);
                Assert.Equal(null, result[i].Value);

                // "" == null
                Assert.Equal(string.Empty, result[++i].Key);
                Assert.Equal(null, result[i].Value);
            }

            [Fact]
            public void EmptyKey0()
            {
                // a key cannot be empty. so this is a key starting with the equa sign.
                var lang = new SekvapLanguage();
                var input = "helo;=b;x=y";
                var result = lang.Parse(input);
                Assert.NotNull(result);
                Assert.Equal(3, result.Count);
                int i = -1;

                Assert.Equal("Value", result[++i].Key);
                Assert.Equal("helo", result[i].Value);

                Assert.Equal("=b", result[++i].Key);
                Assert.Equal(null, result[i].Value);

                Assert.Equal("x", result[++i].Key);
                Assert.Equal("y", result[i].Value);
            }

            [Fact]
            public void EqualSignInValue0()
            {
                var lang = new SekvapLanguage();
                var input = "=8;x=y";
                var result = lang.Parse(input);
                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
                int i = -1;

                Assert.Equal("Value", result[++i].Key);
                Assert.Equal("=8", result[i].Value);

                Assert.Equal("x", result[++i].Key);
                Assert.Equal("y", result[i].Value);
            }

            [Fact]
            public void EqualSignInValue1()
            {
                var lang = new SekvapLanguage();
                var input = "helo;Comp==8;x=y";
                var result = lang.Parse(input);
                Assert.NotNull(result);
                Assert.Equal(3, result.Count);
                int i = -1;

                Assert.Equal("Value", result[++i].Key);
                Assert.Equal("helo", result[i].Value);

                Assert.Equal("Comp", result[++i].Key);
                Assert.Equal("=8", result[i].Value);

                Assert.Equal("x", result[++i].Key);
                Assert.Equal("y", result[i].Value);
            }

            [Fact]
            public void EqualSignInValue2()
            {
                var lang = new SekvapLanguage();
                var input = "helo;Comp=<=8;x=y";
                var result = lang.Parse(input);
                Assert.NotNull(result);
                Assert.Equal(3, result.Count);
                int i = -1;

                Assert.Equal("Value", result[++i].Key);
                Assert.Equal("helo", result[i].Value);

                Assert.Equal("Comp", result[++i].Key);
                Assert.Equal("<=8", result[i].Value);

                Assert.Equal("x", result[++i].Key);
                Assert.Equal("y", result[i].Value);
            }
        }

        public class SerializeEnumerableMethod
        {
            [Fact]
            public void Arg0IsNull_Throws()
            {
                var target = new SekvapLanguage();
                Assert.Throws<ArgumentNullException>(() => target.Serialize(default(IEnumerable<KeyValuePair<string, string>>)));
            }

            [Fact]
            public void NoValues()
            {
                var source = new List<KeyValuePair<string, string>>();
                var target = new SekvapLanguage();
                var result = target.Serialize(source.AsEnumerable());
                Assert.NotNull(result);
                Assert.Equal(0, result.Length);
            }

            [Fact]
            public void EmptyValues1()
            {
                var source = new List<KeyValuePair<string, string>>();
                source.Add(new KeyValuePair<string, string>(string.Empty, string.Empty));
                var target = new SekvapLanguage();

                var result = target.Serialize(source.AsEnumerable());
                Assert.NotNull(result);
                Assert.Equal(0, result.Length);

                target.SkipSerializeEmpty = false;
                var result1 = target.Serialize(source.AsEnumerable());
                Assert.NotNull(result1);
                Assert.Equal(0, result.Length);
            }

            [Fact]
            public void Value()
            {
                var source = new Dictionary<string, string>();
                source.Add("Value", "Hello");
                var target = new SekvapLanguage();
                var result = target.Serialize(source.AsEnumerable());
                Assert.NotNull(result);
                Assert.Equal("Hello", result);
            }

            [Fact]
            public void Value_Item()
            {
                var source = new Dictionary<string, string>();
                source.Add("Value", "Hello");
                source.Add("Foo", "Bar");
                var target = new SekvapLanguage();
                var result = target.Serialize(source.AsEnumerable());
                Assert.NotNull(result);
                Assert.Equal("Hello;Foo=Bar", result);
            }

            [Fact]
            public void Value_Item_Value()
            {
                var source = new List<KeyValuePair<string, string>>();
                source.Add(new KeyValuePair<string, string>("Value", "Hello"));
                source.Add(new KeyValuePair<string, string>("Foo", "Bar"));
                source.Add(new KeyValuePair<string, string>("Value", "Yop"));
                var target = new SekvapLanguage();
                var result = target.Serialize(source.AsEnumerable());
                Assert.NotNull(result);
                Assert.Equal("Hello;Foo=Bar;Value=Yop", result);
            }
        }

        public class SerializeCollectionMethod
        {
            [Fact]
            public void Arg0IsNull_Throws()
            {
                var target = new SekvapLanguage();
                Assert.Throws<ArgumentNullException>(() => target.Serialize(default(ICollection<KeyValuePair<string, string>>)));
            }

            [Fact]
            public void NoValues()
            {
                var source = new List<KeyValuePair<string, string>>();
                var target = new SekvapLanguage();
                var result = target.Serialize(source);
                Assert.NotNull(result);
                Assert.Equal(0, result.Length);
            }

            [Fact]
            public void Value()
            {
                var source = new Dictionary<string, string>();
                source.Add("Value", "Hello");
                var target = new SekvapLanguage();
                var result = target.Serialize(source);
                Assert.NotNull(result);
                Assert.Equal("Hello", result);
            }

            [Fact]
            public void Value_Item()
            {
                var source = new Dictionary<string, string>();
                source.Add("Value", "Hello");
                source.Add("Foo", "Bar");
                var target = new SekvapLanguage();
                var result = target.Serialize(source);
                Assert.NotNull(result);
                Assert.Equal("Hello;Foo=Bar", result);
            }

            [Fact]
            public void Value_Item_Value()
            {
                var source = new List<KeyValuePair<string, string>>();
                source.Add(new KeyValuePair<string, string>("Value", "Hello"));
                source.Add(new KeyValuePair<string, string>("Foo", "Bar"));
                source.Add(new KeyValuePair<string, string>("Value", "Yop"));
                var target = new SekvapLanguage();
                var result = target.Serialize(source);
                Assert.NotNull(result);
                Assert.Equal("Hello;Foo=Bar;Value=Yop", result);
            }
        }

        [Fact]
        public void ParseTests()
        {
            var assembly = typeof(SekvapLanguageTests).Assembly;
            var stream = assembly.GetManifestResourceStream("SrkSekvap.Tests.SekvapLanguage.ParseTests.txt");

            var lines = new List<Test>();

            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                string line, input = null, json = null;
                int lineNumber = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    if (line.StartsWith("$ "))
                    {
                        input = line.Substring(2);
                    }
                    else if (line.StartsWith("> "))
                    {
                        json = line.Substring(2);
                        try
                        {
                            var data = JsonConvert.DeserializeObject<string[,]>(json);
                            lines.Add(new Test(lineNumber, input, data));
                        }
                        catch (JsonReaderException ex)
                        {
                            throw new InvalidOperationException(ex.Message + " @" + lineNumber + "\r\n" + input + "\r\n" + json);
                        }
                    }
                }
            }

            var lang = new SekvapLanguage();
            lang.SkipSerializeEmpty = false;
            var full = new StringBuilder();
            var lite = new StringBuilder();
            int errors = 0, oks = 0, localErrors = 0;
            var prevLiteLine = -1;
            var ok = new Action<Test, int, string>((test1, col1, message1) =>
            {
                full.AppendLine("#" + col1 + ": " + (message1 ?? "OK"));
                oks++;
            });
            var error = new Action<Test, int, string, string>((test1, col1, input, message1) =>
            {
                if (prevLiteLine != test1.lineNumber)
                {
                    lite.AppendLine();
                    if (test1.input == input)
                    {
                        lite.AppendLine("TEST @" + test1.lineNumber + "    $ " + test1.input);
                    }
                    else
                    {
                        lite.AppendLine("TEST @" + test1.lineNumber + " ReParse!");
                        lite.AppendLine("$0 " + test1.input);
                        lite.AppendLine("$1 " + input);
                    }
                }

                prevLiteLine = test1.lineNumber;
                var message2 = col1 >= 0 ? ("#" + col1 + ": " + message1) : message1;
                lite.AppendLine((string)message2);
                full.AppendLine((string)message2);
                errors++;
                localErrors++;
            });

            var test = new Action<Test, IList<KeyValuePair<string, string>>, string>((test1, result, input) =>
            {
                var line = test1;
                for (int col = 0; col < line.data.GetLength(0); col++)
                {
                    if (result.Count >= (col + 1))
                    {
                        var key = line.data[col, 0];
                        if (key.Equals(result[col].Key))
                        {
                            // key ok
                            var value = line.data[col, 1];
                            if (value == null)
                            {
                                if (result[col].Value == null)
                                {
                                    // value ok (null)
                                    ok(line, col, null);
                                }
                                else
                                {
                                    error(line, col, input, "Value should be NULL; not '" + result[col].Value + "'. ");
                                }
                            }
                            else
                            {
                                if (value.Equals(result[col].Value))
                                {
                                    // value ok
                                    ok(line, col, null);
                                }
                                else
                                {
                                    error(line, col, input, "Value should be '" + result[col].Value + "'; not '" + result[col].Value + "'. ");
                                }
                            }
                        }
                        else
                        {
                            error(line, col, input, "Key should be '" + key + "'; not '" + result[col].Key + "'. ");
                        }
                    }
                    else
                    {
                        error(line, col, input, "Column index " + col + " is missing. ");
                    }
                }
            });

            var testReparse = false;
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                localErrors = 0;
                var result = lang.Parse(line.input);
                full.AppendLine();
                full.AppendLine("TEST @" + line.lineNumber + "    $ " + line.input);
                full.AppendLine();
                if (result.Count != line.data.GetLength(0))
                {
                    error(line, -1, line.input, "Result only has " + result.Count + " columns; " + line.data.GetLength(0) + " are expected. ");
                }

                test(line, result, line.input);

                if (localErrors == 0 && testReparse)
                {
                    var serialized = lang.Serialize(result);
                    ////if (line.input.Equals(serialized))
                    ////{
                    ////    ok(line, -1, "Serialize OK.");
                    ////}
                    ////else
                    ////{
                    ////    error(line, -1, "Serialized differs. \r\n$ " + line.input + "\r\n& " + serialized);
                    ////}

                    var unserialized = lang.Parse(serialized);
                    test(line, unserialized, serialized);
                }
            }

            var message = "Lines:" + lines.Count + " OK:" + oks + " Errors:" + errors + "\r\n";
            lite.Insert(0, message);
            full.Insert(0, message);
            Debug.WriteLine(full.ToString());
            if (errors > 0)
            {
                throw new InvalidOperationException(errors + " errors\r\n\r\n" + lite.ToString());
            }
        }

        class Test
        {
            internal int lineNumber;
            internal string input;
            internal string[,] data;

            public Test(int lineNumber, string input, string[,] data)
            {
                this.input = input;
                this.data = data;
                this.lineNumber = lineNumber;
            }
        }
    }
}
