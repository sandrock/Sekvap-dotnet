
# Sekvap-dotnet

![Sekvap logo](https://raw.githubusercontent.com/sandrock/Sekvap-dotnet/master/res/logo-64.png)

A simple Key/Value language "Sekvap" implemented for .NET. [Nuget.org/SrkSekvap/](https://www.nuget.org/packages/SrkSekvap/)

See also: [Sekvap-java](https://github.com/sandrock/Sekvap-java/)

This thing converts this kind of strings into a `IList<KeyValuePair<string, string>>`.

```
Main value;Key1=Value 1;Key2=Value 2;Key2=Value 2 again;

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
    string input = string.Join(string.Empty, parts); // "hello world;Name=;Foo=Bar"
    var result = lang.Parse(input);
    Assert.IsNotNull(result);
    Assert.AreEqual(3, result.Count);
    int i = -1;

    Assert.AreEqual("Value", result[++i].Key);      // result[0].Key   = "Value"
    Assert.AreEqual(parts[0], result[i].Value);     // result[0].Value = "hello world"

    Assert.AreEqual(parts[2], result[++i].Key);     // result[1].Key   = "Name"
    Assert.AreEqual(string.Empty, result[i].Value); // result[1].Value = ""

    Assert.AreEqual(parts[5], result[++i].Key);     // result[2].Key   = "Foo"
    Assert.AreEqual(parts[7], result[i].Value);     // result[2].Value = "Bar"
}
```

You may look at the [other tests](src/SrkSekvap.Tests/SekvapLanguageTests.cs).

Why Sekvap?
--------------

Sekvap is easy to read, easy to parse and easy to store.

Sekvap is lighter than JSON or XML. It does not provide any depth though.

You can parse .net connection strings with it.


Install
--------------

[Nuget package: SrkSekvap](https://www.nuget.org/packages/SrkSekvap/)

```
> Install-Package SrkSekvap
```


To Do list
--------------

- [x] Parse (string to IList&lt;KeyValuePair&lt;string, string>>)
- [x] Serialize (IEnumerable&lt;KeyValuePair&lt;string, string>> to string)
- [x] nuget package [Nuget.org/SrkSekvap/](https://www.nuget.org/packages/SrkSekvap/)
- [ ] Deserialize (string to typed object)
- [ ] Serialize (typed object to string)
- [ ] [Language specifications](Sekvap-Language-Specifications.md)


Contribute
--------------

Open to contributions via issues and PRs.


Publishing
--------------

- Commit
- change assenbly version and nuget version
- write release notes
- make a Release build
- `nuget.exe push -src https://api.nuget.org/v3/index.json  -apikey xxx  src/SrkSekvap/bin/Release/SrkSekvap.X.X.X.nupkg`

