# Sekvap-dotnet

A simple Key/Value language "Sekvap" implemented for .NET.

This thing converts this kind of strings into a `IList<KeyValuePair<string, string>>`.

```
Main value;Key1=Value 1;Key2=Value 2;Key2=Value 2 again;
```

```
Data Source=myServerAddress;Initial Catalog=myDataBase;Integrated Security=SSPI;User ID=myDomain\myUsername;Password=myPassword;
```

This unit test will help you understand how it works:

<!-- language: csharp -->
```
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
```

Why Sekvap?
--------------

Sekvap is easy to read, easy to parse and easy to store.

Sekvap is lighter than JSON or XML. It does not provide any depth though.

You can parse .net connection strings with it.



