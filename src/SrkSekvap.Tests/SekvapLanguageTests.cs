
namespace SrkSekvap.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SrkSekvap;

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
            public void PrefixOk_SimpleValue()
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
            public void PrefixOk_SimpleValueWithEscapedSeparator()
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
            public void PrefixOk_SimpleValue_PlusOneKevap()
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
            public void PrefixOk_SimpleValue_PlusTwoKevap()
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
            public void PrefixOk_SimpleValue_PlusOneEmptyKevap()
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
            public void PrefixOk_SimpleValue_PlusOneEmptyEqualKevap()
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
            public void PrefixOk_SimpleEmptyValue_PlusTwoKevap()
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
            public void PrefixOk_SimpleEmptyEqualValue_PlusTwoKevap()
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
            public void PrefixOk_SimpleValue_PlusTwoEmptyKevap()
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
            public void PrefixOk_SimpleValue_PlusTwoEmptyEqualKevap()
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
            public void PrefixOk_SimpleValue_PlusOneEmptyKevapAndOneEmptyEqualKevap()
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
            public void PrefixOk_SimpleValue_PlusOneEmptyKevap_EndsWithSemicolon()
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
    }
}
