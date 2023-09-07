# RJJSON
A JSON parser for c#

See the [changelog](https://github.com/RJ-Infinity/RJJSON/blob/main/Changelog.md) for more info


usage
```c#
public void functionName(string FileName){
	//should check that the file is a valid file path and exists
	string jsonContent = System.IO.File.ReadAllText(FileName);
	JSONType json = JSON.StringToObject(jsonContent);
	if(//here it is probably better to use asserts so that you can tell where the exception occured
	// however for the purposes of this demonstration that isnt necessary
		json.Type == Json.Types.DICT && //check the root is a dict
		json.DictData.ContainsKey("A Json Key") && //check it contains the key
		json["A Json Key"].Type == Json.Types.STRING //check the key has the correct type
	){
		string theKey = json["A Json Key"].StringData; //extract the data
	}
	else
	{
		//throw an exception
	}
}
```
