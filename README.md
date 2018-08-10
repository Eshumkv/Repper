# Repper 

The point of this program is to use a custom language to set up configuration files. 

This is an example:

```csharp
// First code line should be the file, prefixed with an equal sign
= C:\temp\test.config 

// Now you can start your replacements. 

// This one looks for the node configuration, then node appSettings, ... 
// At the node add it looks for an attribute with the name 'key' and value 'something'
// When it finds this, it looks for another attribute with the name 'value' 
// When it has found this, it replaces the value for this attribute with the value after the equal.
configuration.appSettings.add#key{something}#value = HolaMiFoute

// It's also possible to use functions as a value
// Functions HAVE to be prefixed with an '&', otherwise they will be interpreted as a string
configuration.connectionStrings.add#name{FinancienNet_Financien}#connectionString = &replaceConn($value, "remappfitest\\sql")
```

