# RJJSON
this is the changelog for RJJSON (JSONParse) a Json Parser for c# written by RJ_Infinity
## JSONParse-0.5 Feb 2022
### Additions
- added `FillDefault` function which fills a JSON dict with defalut values 
### Fixes
- added missing comment doc for `FormatJson`
### Improvements
- The `GetData` method has been removed (partialy due to the restructure)
- **SIGNIFICANT CHANGE** the classes that inhereted from `JsonTypes` have beed removed and the `JsonTypes` class has been renamed to `JSONType`. `JsonType` has now not got a `Data` field and instead has
    + `DictData`
    + `ListData`
    + `BoolData`
    + `StringData`
    + `FloatData`

these all throw a `InvalidTypeException` if the `Type` of the class dosent match the c# parralell type if they match then it returns the data that the instance holds once initialised the `Type` field cannot me changed this was done to improve typping as there is a strict type now and no `dynamic`s or `object` casting the retrivial of data should now be handled like so
```c#
public void functionName(JsonType json){
    if (json.Type == Json.Types.DICT){
        Dictionary<string, JsonType> rootData = json.DictData;
    }else{
        // throw an error or handle another case
    }
    // or for getting a key
    if(
        json.Type == Json.Types.DICT && //check the root is a dict
        json.DictData.ContainsKey("A Json Key") && //check it contains the key
        json["A Json Key"].Type == Json.Types.STRING //check the key has the correct type
    ){
        string theKey = json["A Json Key"].StringData; //extract the data
    }
}
```
or a functionaly simalar method using asserts ect.
## JSONParse-0.4 Aug 2021
### Additions
- added the indexer wrappers
- added the enumerator wrappers
### Fixes
- made all classes and relavent functions public so they are usefull from external libarys
### Improvments
- small optimisations
- better Exception classes
## JSONParse-0.3 Apr 2021
### Additions
- `FormatJson` function accepts a `string` or `JSONTypes` and formats it
### Fixes
- Fixed string escaping
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
