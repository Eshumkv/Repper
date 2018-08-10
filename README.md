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

Functions will probably come in version 2.0. 

# Wants

## Replacing with Node
For now it's only possible to replace something with a string. So for many replacings in a single node, you'd have a lot of work. Allow it to be replaced with a node. 

## Adding 
I really want a way to say to add a node (or attribute) if it doesn't exist with this value. Default it wouldn't do this, because that just seems silly. 

The use case might be to add debugging info to a web.config. 

# TODO

