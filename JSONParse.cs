using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// <version>0.5</version>
/// Adds A new Type <c>JSONType</c> which is a wrapper for the c# representations of json types
/// Also Adds JSON class which is "A mostly static class which helps yopu minippulate <c>JSONTypes</c>"
/// 
/// © RJ_Infinity 2021
/// </summary>
namespace RJJSON
{
    /// <summary>
    /// Class <c>JSON</c> A mostly static class which helps yopu minippulate <c>JSONTypes</c>
    /// </summary>
    public class JSON
    {
        /// <summary>
        /// Method <c>StringToObject</c> converts a <c>string</c> representation of JSON to an object representation namely <c>JSONTypes</c> the sort used in C# code the oposit method is <code>JSON.ObjectToString()</code>
        /// </summary>
        /// <param name="Json">A string representation of JSON</param>
        /// <returns>A JSONTypes object with the same structure and values as the string representation</returns>
        public static JSONType StringToObject(string Json)
        {
            Json = Json.Replace("\r\n", "\n").Replace("\n", "");//remove all types of line ending
            bool InString = false;
            bool InEsc = false;
            for (int ichar = 0; ichar < Json.Length; ichar++)//this loop removes whitespace except in strings
            {
                if (!InString)
                {
                    while (Json.Substring(ichar, 1) == " ")//while the current character is a whitespace remove it
                    {
                        Json = Json.Remove(ichar, 1);//while is neededd not if as otherise it fails to remove two whitespace in a row
                    }
                    while (Json.Substring(ichar, 1) == "\t")
                    {
                        Json = Json.Remove(ichar, 1);
                    }
                }
                if (!InEsc)//stops ending strings if the " was preceded with a \
                {
                    if (Json.Substring(ichar, 1) == "\"")
                    {
                        InString = !InString;
                    }
                }
                if (InEsc) { InEsc = false; }
                if (Json.Substring(ichar, 1) == "\\")
                {
                    InEsc = true;
                }
            }
            if (Json == "true")
            {
                JSONType rv = new JSONType(Types.BOOL);
                rv.BoolData = true;
                return rv;
            }
            if (Json == "false")
            {
                JSONType rv = new JSONType(Types.BOOL);
                rv.BoolData = false;
                return rv;
            }
            //if (double.TryParse(Json, out double JsonAsDBL))
            if (JsonParseNumber(Json, out double JsonAsDBL))
            {
                JSONType rv = new JSONType(Types.FLOAT);
                rv.FloatData = JsonAsDBL;
                return rv;
            }
            switch (Json.Substring(0, 1))//get type of fist thing
            {
                case "\""://string
                    if (Json.Substring(Json.Length - 1, 1) == "\"")
                    {
                        JSONType rv = new JSONType(Types.STRING);
                        rv.StringData = RemoveEscChars(Json.Substring(1, Json.Length - 2));//remove the quotes
                        return rv;
                    }
                    else//it is incorectly formated
                    {
                        throw new FormatException("The JSON string is incorectly formated");
                    }
                //break;
                case "{":
                    if (Json.Substring(Json.Length - 1, 1) == "}")
                    {
                        JSONType returnv = new JSONType(Types.DICT);
                        if (Json.Substring(1, Json.Length - 2).Length > 0){
                            foreach (string str in SplitToJsonObj(Json.Substring(1, Json.Length - 2)))
                            {
                                KeyValuePair<string, JSONType> KeyValue = GetKeyValuePair(str);
                                returnv.DictData.Add(KeyValue.Key, KeyValue.Value);
                            }
                        }
                        return returnv;
                    }
                    else//it is incorectly formated
                    {
                        throw new FormatException("The JSON string is incorectly formated");
                    }
                //break;
                case "[":
                    if (Json.Substring(Json.Length - 1, 1) == "]")
                    {
                        JSONType returnv = new JSONType(Types.LIST);
                        if (Json.Substring(1, Json.Length - 2).Length > 0)
                        {
                            foreach (string str in SplitToJsonObj(Json.Substring(1, Json.Length - 2)))
                            {
                                returnv.ListData.Add(StringToObject(str));
                            }
                        }
                        return returnv;
                    }
                    else//it is incorectly formated
                    {
                        throw new FormatException("The JSON string is incorectly formated");
                    }
                //break;
            }
            throw new Exception("Json is invalid");//just so the compiler dosnt shout at me (this should probs be an exception)
        }
        private static char[] digits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private static string ConsumeDigits(ref string Json)
        {
            string rv = "";
            while (Json.Length > 0 && digits.Contains(Json[0]))
            {
                rv += Json[0];
                Json = Json.Substring(1);
            }
            return rv;
        }
        private static bool JsonParseNumber(string Json, out double result)
        {
            if (!double.TryParse(Json, out double JsonAsDBL))
            {
                result = 0;
                return false;
            }
            result = JsonAsDBL;

            if (Json[0] == '-')
            { Json = Json.Substring(1); }

            if (Json.Length == 0 || !digits.Contains(Json[0]))
            {
                result = 0;
                return false;
            }
            if (Json[0] != '0')
            { ConsumeDigits(ref Json); }
            else
            { Json = Json.Substring(1); }
            if (Json.Length == 0) { return true; }

            if (Json.Length > 1 && Json[0] == '.' && digits.Contains(Json[1]))
            {
                Json = Json.Substring(1);
                ConsumeDigits(ref Json);
            }
            if (Json.Length == 0) { return true; }

            if (Json[0] == 'e' || Json[0] == 'E')
            { Json = Json.Substring(1); }
            else
            {
                result = 0;
                return false;
            }
            if (Json.Length > 0 && (Json[0] == '-' || Json[0] == '+'))
            { Json = Json.Substring(1); }
            if (Json.Length > 0 && digits.Contains(Json[0]))
            { ConsumeDigits(ref Json); }
            if (Json.Length == 0) { return true; }

            result = 0;
            return false;
        }
        private static KeyValuePair<string, JSONType> GetKeyValuePair(string Json)
        {
            string Key = "";
            string Value = "";
            bool InString = false;
            bool InEsc = false;
            for (int ichar = 0; ichar < Json.Length; ichar++)
            {
                if (!InString)
                {
                    if (Json.Substring(ichar,1) == ":")
                    {
                        Key = Json.Substring(1, ichar - 2);
                        Value = Json.Substring(ichar+1, Json.Length - ichar - 1);
                        return new KeyValuePair<string, JSONType>(Key, StringToObject(Value));
                    }
                }
                if (!InEsc)//stops ending strings if the " was preceded with a \
                {

                    if (Json.Substring(ichar, 1) == "\"")
                    {
                        InString = !InString;
                    }
                }
                if (InEsc) { InEsc = false; }
                if (Json.Substring(ichar, 1) == "\\")
                {
                    InEsc = true;
                }
            }
            return new KeyValuePair<string, JSONType>(Key, StringToObject(Value));
        }
        private static string RemoveEscChars(string str)
        {
            bool InEsc = false;
            string newStr = "";
            for (int ichar = 0; ichar < str.Length; ichar++)
            {
                if (str.Substring(ichar, 1) == "\\" && !InEsc)
                {
                    InEsc = true;
                }
                else if (InEsc)
                {
                    InEsc = false;
                    switch (str.Substring(ichar, 1))
                    {
                        case "b":
                            newStr += "\b";
                            break;
                        case "f":
                            newStr += "\f";
                            break;
                        case "n":
                            newStr += "\n";
                            break;
                        case "r":
                            newStr += "\r";
                            break;
                        case "t":
                            newStr += "\t";
                            break;
                        case "\"":
                            newStr += "\"";
                            break;
                        case "\\":
                            newStr += "\\";
                            break;
                        default:
                            newStr += "\\" + str.Substring(ichar, 1);
                            break;
                    }
                }
                else
                {
                    newStr += str.Substring(ichar, 1);
                }
            }
            return newStr;
        }
        private static List<string> SplitToJsonObj(string Json)
        {
            Json += ",//"; // so the last element trigers the comma detection code and gets put in the list the "//" are so it dosnt run out of string
            List<char> close = new List<char> { };
            List<string> returnv = new List<string> { };
            bool InString = false;
            bool InEsc = false;
            for (int ichar = 0; ichar < Json.Length; ichar++)
            {
                if (!InString && close.Count == 0)
                {
                    if (Json.Substring(ichar, 1) == ",")
                    {
                        returnv.Add(Json.Substring(0, ichar));
                        Json = Json.Substring(ichar+1, Json.Length - ichar -1);
                        ichar = 0;
                    }
                }
                if (!InEsc)//stops ending strings if the " was preceded with a \
                {
                    
                    if (Json.Substring(ichar, 1) == "\"")
                    {
                        InString = !InString;
                    }
                }
                if (Json.Substring(ichar, 1) == "{") { close.Add('}'); }
                if (Json.Substring(ichar, 1) == "[") { close.Add(']'); }
                if (close.Count > 0)
                {
                    if (Json.Substring(ichar, 1) == close[close.Count - 1].ToString())
                    {
                        close.RemoveAt(close.Count - 1);
                    }
                }
                if (InEsc) { InEsc = false; }
                if (Json.Substring(ichar, 1) == "\\")
                {
                    InEsc = true;
                }
            }
            return returnv;
        }
        /// <summary>
        /// Method <c>StringToObject</c> converts a <c>JSONTypes</c> representation of JSON to a <c>string</c> representation the type you would see in a file the oposit method is <code>JSON.StringToObject()</code>
        /// </summary>
        /// <param name="Json"></param>
        /// <returns>String Representation Of JSONTypes Object</returns>
        public static string ObjectToString(JSONType Json)
        {
            string returnv = "";
            if (Json.Type == JSON.Types.NULL)
            {
                returnv = "null";
            }
            else if (Json.Type == JSON.Types.DICT)
            {
                returnv += "{";
                bool firstloop = true;
                foreach(KeyValuePair<string, JSONType> El in Json)
                {
                    if (!firstloop)
                    {
                        returnv += ",";
                    }
                    firstloop = false;
                    returnv += "\"" + El.Key.Replace("\"", "\\\"") + "\":" + ObjectToString(El.Value);
                }
                returnv += "}";
            }
            else if(Json.Type == JSON.Types.LIST)
            {
                returnv += "[";
                bool firstloop = true;
                foreach (JSONType El in Json)
                {
                    if (!firstloop)
                    {
                        returnv += ",";
                    }
                    firstloop = false;
                    returnv += ObjectToString(El);
                }
                returnv += "]";
            }
            else if(Json.Type == JSON.Types.BOOL)
            {
                returnv = Json.BoolData.ToString().ToLower();
            }
            else if (Json.Type == JSON.Types.STRING)
            {
                returnv = "\"" + Json.StringData.Replace("\"", "\\\"").Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\b", "\\b").Replace("\f", "\\f").Replace("\r", "\\r") + "\"";
            }
            else if (Json.Type == JSON.Types.FLOAT)
            {
                returnv = ((decimal)Json.FloatData).ToString();
            }
            else
            {
                throw new Exception("Unknown Data Type\"" + Json.Type + "\"");
            }
            return returnv;
        }
        /// <summary>
        /// formats a Json string
        /// </summary>
        /// <param name="JSONStr">the json to be formated</param>
        /// <returns>a formated Json string</returns>
        public static string FormatJson(string JSONStr)
        {
            char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            List<char> closes = new List<char>();
            int tabs = 0;
            string FormatedJSONStr = "";
            bool inString = false;
            bool esc = false;
            bool isnum = false;
            for (int ichar = 0; ichar < JSONStr.Length; ichar++)
            {
                if (esc){
                    esc = false;
                    switch (JSONStr[ichar])
                    {
                        case 'b':
                            FormatedJSONStr += "\\b";
                        break;
                        case 'f':
                            FormatedJSONStr += "\\f";
                        break;
                        case 'n':
                            FormatedJSONStr += "\\n";
                        break;
                        case 'r':
                            FormatedJSONStr += "\\r";
                        break;
                        case 't':
                            FormatedJSONStr += "\\t";
                        break;
                        case '"':
                            FormatedJSONStr += "\\\"";
                        break;
                        case '\\':
                            FormatedJSONStr += "\\\\";
                        break;
                        default:
                            throw new Exception("error Invalid Data To Be Formated");
                        //break;
                    }
                }
                else if (JSONStr[ichar] == '\\') {
                    esc = true;
                }
                else if (JSONStr[ichar] == '"' && !esc){
                    inString = !inString;
                    FormatedJSONStr += "\"";
                    if (!inString)
                    {
                        try
                        {
                            if(!((JSONStr[ichar+1] == ',') || (JSONStr[ichar + 1] == ':')))
                            {
                                FormatedJSONStr += "\n";
                                for (int i = 0; i < tabs; i++)
                                {
                                    FormatedJSONStr += "\t";
                                }
                            }
                        }
                        catch (ArgumentOutOfRangeException){}
                    }
                }

                else if ((JSONStr[ichar] == '[' || JSONStr[ichar] == '{') && !inString)
                {
                    closes.Add( (JSONStr[ichar]=='[') ? ']' : '}');
                    FormatedJSONStr += JSONStr[ichar]+"\n";
                    tabs++;
                    for (int i = 0; i < tabs; i++)
                    {
                        FormatedJSONStr += "\t";
                    }
                }
                else if(JSONStr[ichar] == ',' && !inString)
                {
                    FormatedJSONStr += ",\n";
                    for (int i = 0; i < tabs; i++)
                    {
                        FormatedJSONStr += "\t";
                    }
                }
                else
                {
                    foreach (char num in numbers)
                    {
                        if (JSONStr[ichar] == num){
                            try
                            {
                                if (JSONStr[ichar+1] == '}' || JSONStr[ichar + 1] == ']')
                                {
                                    isnum = true;
                                    FormatedJSONStr += JSONStr[ichar]+"\n";
                                    for (int i = 0; i < tabs; i++)
                                    {
                                        FormatedJSONStr += "\t";
                                    }
                                }
                            }
                            catch (ArgumentOutOfRangeException)
                            {}
                            break;
                        }
                    }
                    if (!isnum)
                    {
                        try
                        {
                            if (JSONStr[ichar] == closes[closes.Count - 1] && !inString)
                            {
                                if (FormatedJSONStr[FormatedJSONStr.Length -1] == '}' || FormatedJSONStr[FormatedJSONStr.Length - 1] == ']')
                                {
                                    FormatedJSONStr += "\n";
                                }
                                if (FormatedJSONStr[FormatedJSONStr.Length-1] == '\t')
                                {
                                    FormatedJSONStr = FormatedJSONStr.Substring(0, FormatedJSONStr.Length - 1);
                                }
                                FormatedJSONStr += JSONStr[ichar];
                                tabs--;
                                closes.RemoveAt(closes.Count - 1);
                            }
                            else
                            {
                                FormatedJSONStr += JSONStr[ichar];
                            }
                        }
                        catch (ArgumentOutOfRangeException){
                            FormatedJSONStr += JSONStr[ichar];
                        }
                    }
                    else
                    {
                        isnum = false;
                    }
                }
            }
            return FormatedJSONStr;
        }
        /// <summary>
        /// formats a Json string
        /// </summary>
        /// <param name="JSONStr">the json to be formated</param>
        /// <returns>a formated Json string</returns>
        public static string FormatJson(JSONType JSONStr){
            return FormatJson(JSONStr.ToString());
        }
        /// <summary>
        /// fills the missing values from a Json object with the default values provided
        /// </summary>
        /// <param name="Default">your default values</param>
        /// <param name="Custom">the incomplete values</param>
        /// <returns></returns>
        public static JSONType FillDefault(JSONType Default, JSONType Custom)
        {
            switch (Default.Type)
            {
                case Types.NULL:
                case Types.BOOL:
                case Types.STRING:
                case Types.FLOAT:
                case Types.LIST:
                {
                    if (Custom.Type == Types.NULL)
                    {
                        return Default;
                    }
                    return Custom;
                }
                case Types.DICT: return ReplaceDict(Default, Custom);
            }
            throw new Exception("This Should Be An Imposible State");
        }
        private static JSONType ReplaceDict(JSONType Default, JSONType Custom)
        {
            if (Default.Type != Types.DICT || Custom.Type != Types.DICT)
            {
                throw new InvalidTypeException("the passed type wasnt a JSON dictionary");
            }
            JSONType NewJson = new JSONType(Types.DICT);
            foreach (KeyValuePair<string, JSONType> i in Default)
            {
                if (Custom.DictData.ContainsKey(i.Key))
                {
                    if (Custom[i.Key].Type == Types.DICT && i.Value.Type == Types.DICT)
                    {
                        NewJson[i.Key] = ReplaceDict(Custom[i.Key], i.Value);
                    }
                    else
                    {
                        NewJson[i.Key] = Custom[i.Key];
                    }
                }
                else
                {
                    NewJson[i.Key] = i.Value;
                }
            }
            return NewJson;
        }

        /// <summary>
        /// All The Types JSON supports
        /// (it may appear that JSON supports Int as it has this option
        /// </summary>
        public enum Types
        {
            /// <summary>
            /// represents <c>null</c>
            /// </summary>
            NULL,
            /// <summary>
            /// represents <c>Dictionary&lt;string, JSONTypes&gt;</c>
            /// </summary>
            DICT,
            /// <summary>
            /// represents <c>List&lt;JSONTypes&gt;</c>
            /// </summary>
            LIST,
            /// <summary>
            /// represents <c>bool</c>
            /// </summary>
            BOOL,
            /// <summary>
            /// represents <c>string</c>
            /// </summary>
            STRING,
            /// <summary>
            /// represents <c>double</c>
            /// </summary>
            FLOAT
        }
        /// <summary>
        /// converts <c>JSON.Types</c> to <c>Type</c>
        /// </summary>
        /// <param name="type">the type to convert to a <c>Type</c></param>
        /// <returns>the <c>Type</c> equivelent of <c>Json.Types</c> value passed in</returns>
        public static Type JSONTypesToTypes(JSON.Types type)
        {
            switch (type)
            {
                case JSON.Types.NULL:
                    return null;
                case JSON.Types.DICT:
                    return typeof(Dictionary<string, JSONType>);
                case JSON.Types.LIST:
                    return typeof(List<JSONType>);
                case JSON.Types.BOOL:
                    return typeof(bool);
                case JSON.Types.STRING:
                    return typeof(string);
                case JSON.Types.FLOAT:
                    return typeof(double);
            }
            return null;
        }

    }
    /// <summary>
    /// Class <c>JSONTypes</c> wrapper for all JSON types.
    /// </summary>
    public class JSONType
    {
        /// <summary>
        /// constructor <c>JSONType</c>
        /// <param name="type">the type the <c>JSONType</c> instance wraps</param>
        /// </summary>
        public JSONType(JSON.Types type)
        {
            Type = type;
        }
        /// <summary>
        /// field <c>Type</c>
        /// <returns>The JSON type as a <code>JSON.Types</code></returns>
        /// </summary>
        public JSON.Types Type { get; }
        /// <summary>
        /// field <c>DictData</c> Get the Data that the instance holds if it has a type of <code>JSON.Types.DICT</code>
        /// </summary>
        public Dictionary<string, JSONType> DictData {
            get
            {
                if (Type == JSON.Types.DICT)
                {
                    return dictData;
                }
                throw new InvalidTypeException("The Type is " + Type + " not "+JSON.Types.DICT);
            }
        }
        private Dictionary<string, JSONType> dictData = new Dictionary<string, JSONType> { };
        /// <summary>
        /// field <c>ListData</c> Get the Data that the instance holds if it has a type of <code>JSON.Types.LIST</code>
        /// </summary>
        public List<JSONType> ListData
        {
            get
            {
                if (Type == JSON.Types.LIST)
                {
                    return listData;
                }
                throw new InvalidTypeException("The Type is " + Type + " not " + JSON.Types.LIST);
            }
        }
        private List<JSONType> listData = new List<JSONType> { };
        /// <summary>
        /// field <c>BoolData</c> Get the Data that the instance holds if it has a type of <code>JSON.Types.BOOL</code>
        /// </summary>
        public bool BoolData
        {
            get
            {
                if (Type == JSON.Types.BOOL)
                {
                    return boolData;
                }
                throw new InvalidTypeException("The Type is " + Type + " not " + JSON.Types.BOOL);
            }
            set
            {
                if (Type == JSON.Types.BOOL)
                {
                    boolData = value;
                    return;
                }
                throw new InvalidTypeException("The Type is " + Type + " not " + JSON.Types.BOOL);
            }
        }
        private bool boolData;
        /// <summary>
        /// field <c>StringData</c> Get the Data that the instance holds if it has a type of <code>JSON.Types.STRING</code>
        /// </summary>
        public string StringData
        {
            get
            {
                if (Type == JSON.Types.STRING)
                {
                    return stringData;
                }
                throw new InvalidTypeException("The Type is " + Type + " not " + JSON.Types.STRING);
            }
            set
            {
                if (Type == JSON.Types.STRING)
                {
                    stringData = value;
                    return;
                }
                throw new InvalidTypeException("The Type is " + Type + " not " + JSON.Types.STRING);
            }
        }
        private string stringData;
        /// <summary>
        /// field <c>FloatData</c> Get the Data that the instance holds if it has a type of <code>JSON.Types.FLOAT</code>
        /// </summary>
        public double FloatData
        {
            get
            {
                if (Type == JSON.Types.FLOAT)
                {
                    return floatData;
                }
                throw new InvalidTypeException("The Type is " + Type + " not " + JSON.Types.FLOAT);
            }
            set
            {
                if (Type == JSON.Types.FLOAT)
                {
                    floatData = value;
                    return;
                }
                throw new InvalidTypeException("The Type is " + Type + " not " + JSON.Types.FLOAT);
            }
        }
        private double floatData;
        /// <summary>
        /// method <c>ToString</c> alias for <code>JSON.ObjectToString(this)</code> or in other words the <code>JSON.ObjectToString</code> of itsself
        /// </summary>
        /// <returns>JSON.ObjectToString(this)</returns>
        public override string ToString()
        {
            return JSON.ObjectToString(this);
        }
        /// <summary>
        /// Indexer Root Only works if <c>type</c> is <code>JSON.Types.DICT</code>
        /// </summary>
        /// <param name="index">the value to index to</param>
        /// <returns></returns>
        public virtual JSONType this[int index] {
            get
            {
                if (Type != JSON.Types.LIST)
                {
                    throw new InvalidTypeException("the type isnt a JSON list");
                }
                try
                {
                    return listData[index];
                }
                catch (KeyNotFoundException)
                {
                    throw;
                }
            }
            set
            {
                if (Type != JSON.Types.LIST)
                {
                    throw new InvalidTypeException("the type isnt a JSON list");
                }
                try
                {
                    listData[index] = value;
                }
                catch (KeyNotFoundException)
                {
                    throw;
                }
            }
        }
        /// <summary>
        /// Indexer Root Only works if <c>type</c> is <code>JSON.Types.LIST</code>
        /// </summary>
        /// <param name="index">the value to index to</param>
        /// <returns></returns>
        public virtual JSONType this[string index] {
            get
            {
                if (Type != JSON.Types.DICT)
                {
                    throw new InvalidTypeException("the type isnt a JSON dict");
                }
                try
                {
                    return dictData[index];
                }
                catch (KeyNotFoundException)
                {
                    throw;
                }
            }
            set
            {
                if (Type != JSON.Types.DICT)
                {
                    throw new InvalidTypeException("the type isnt a JSON dict");
                }
                try
                {
                    dictData[index] = value;
                }
                catch (KeyNotFoundException)
                {
                    throw;
                }
            }
        }
        /// <summary>
        /// GetEnumerator Root. Only works if <c>type</c> is <code>JSON.Types.DICT</code> and <code>JSON.Types.LIST</code>
        /// </summary>
        /// <returns>IEnumerator object</returns>
        public virtual IEnumerator GetEnumerator()
        {
            if (Type == JSON.Types.DICT)
            {
                foreach (KeyValuePair<string, JSONType> entry in dictData)
                {
                    yield return entry;
                }
                yield break;
            }
            else if (Type == JSON.Types.LIST)
            {
                foreach (JSONType entry in listData)
                {
                    yield return entry;
                }
                yield break;
            }
            else
            {
                throw new InvalidTypeException("the type isnt a JSON dict or JSON list");
            }
        }
    }
    /// <summary>
    /// thrown when there is an invalid type passed
    /// </summary>
    [System.Serializable]
    public class InvalidTypeException : Exception
    {
        /// <summary>
        /// initiliser for <c>InvalidTypeException</c>
        /// </summary>
        public InvalidTypeException() { }
        /// <summary>
        /// initiliser for <c>InvalidTypeException</c>
        /// </summary>
        public InvalidTypeException(string message) : base(message) { }
        /// <summary>
        /// initiliser for <c>InvalidTypeException</c>
        /// </summary>
        public InvalidTypeException(string message, Exception inner) : base(message, inner) { }
        /// <summary>
        /// initiliser for <c>InvalidTypeException</c>
        /// </summary>
        protected InvalidTypeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
