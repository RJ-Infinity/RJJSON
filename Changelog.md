# RJJSON
this is the changelog for RJJSON (JSONParse) a Json Parser for c# written by RJ_Infinity
## JSONParse-0.2 Feb 2021
### Improvments
- empty dictionarys and list are better parsed 
## JSONParse-0.1 Jan 2021
### Additions
everything this is the first version but as you ask (if you do) here they are
- The Class `JSON` this is a static class that handles conversions to and from native types and the conversion typeofs and an internal type storage system these can be converted to typeofs by `JSON.JSONTypesToTypes(Types type)`
- The Class `JSONTypes` this is a class which all types inherit from it provides the methods `GetData<T>()` and `ToString()` which is just a shortcut to `JSON.ObjectToString(this);`
- The classes
    + `JSONDictionary` wraps `Dictionary<string,JSONTypes>`
    + `JSONList` wraps `List<JSONTypes>`
    + `JSONString` wraps `string`
    + `JSONBool` wraps `bool`
    + `JSONFloat` wraps `double`