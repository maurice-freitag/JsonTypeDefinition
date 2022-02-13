# JsonTypeDefinition

An RFC 8927 compliant parser for .NET.

Json Type Definitions (JTDs) provide a human-readable, portable type description that can be consumed by any programming language.

A common use-case would be a REST API describing the form in which payloads are expected. It could publicize this form as a Json Type Definition which could subsequently be interpreted by any client independent of their technology stack.

</br>

## Installation 

`dotnet add package JsonTypeDefinition`

</br>

## Usage

The `JsonTypeDefinitionParser` provides two methods:
```cs
var x = JsonTypeDefinitionParser.Parse(typeof(User));
var y = JsonTypeDefinitionParser.Parse<User>();
```

Both methods will produce the same output. Given the following types:

```cs
class User
{
    [Required]
    public string Name { get; set; }
    public Address Address { get; set; }
    public DateTimeOffset JoinedAt { get; set; }
    public double GPA { get; set; }
}

class Address 
{
    public string StreetName { get; set; }
    public int HouseNumber { get; set; }
}
```

The parser would produce the following Json Type Definition:

```json
{
    "properties": {
        "Name": { "type": "string" }
    },
    "optionalProperties": {
        "Address": { "ref": "Address" },
        "JoinedAt": { "type": "timestamp" },
        "GPA": { "type": "float64" }
    },
    "definitions": {
        "Adress": {
            "optionalProperties": {
                "StreetName": { "type": "string" },
                "HouseNumber": { "type": "int32" }
            }
        }
    }
}
```

Do note that `JsonTypeDefinitionParser.Parse` returns a record of type `RootSchema` and has no reference to `Newtonsoft.Json` or `System.Text.Json`. Feel free to use your favorite JSON serializer to get the appropriate JSON representation! Recommended serialization settings: 

```cs
// Newtonsoft.Json:
var settings = new Newtonsoft.Json.JsonSerializerSettings()
{
    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
    Converters = new Newtonsoft.Json.JsonConverter[] { new Newtonsoft.Json.Converters.StringEnumConverter() }
};

// System.Text.Json:
var options = new System.Text.Json.JsonSerializerOptions { IgnoreNullValues = true };
options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
```

</br>

## Limitations

When defining the models it is important to design them around the limitations that come with a _portable_, _consumer agnostic_ type definition. 

Some commonly used .NET types are currently not supported, including `System.TimeSpan`. Imagine what would happen if you were to create a JTD from a time span object. RFC 8927 does not define a fitting primitive type that could be used (`timestamp` is more akin to `System.DateTimeOffset`) as there is no universally agreed upon way to describe durations. 10 minutes could be described as `00:10:00`, `PT10M` (ISO 8601), `3600` (in seconds), etc. There would be no way for consumers of the JTD to know which notation to use. Instead you're encouraged to work around these limitations for example by replacing 
```cs
public TimeSpan TimeToLive { get; set; }
```
with
```cs
public double TimeToLiveInSeconds { get; set; }
``` 
so that any consumer nows exactly what format you expect. 

</br>

## Future improvements

Contributions are welcome :)
- [ ] custom handlers for handling otherwise unsupported types
- [ ] generate .NET types from JTDs