using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// <version>0.3</version>
/// Adds A new Type <c>JSONTypes</c> and its dirivitives
/// <list type="bullet">
///     <item>
///         <term><c>JSONDictionary</c></term>
///         <description>A wrapper for a Dictionary with the format of JS</description>
///     </item>
///     <item>
///         <term><c>JSONList</c></term>
///         <description>A wrapper for a List with the format of JS</description>
///     </item>
///     <item>
///         <term><c>JSONBool</c></term>
///         <description>A wrapper for a bool</description>
///     </item>
///     <item>
///         <term><c>JSONString</c></term>
///         <description> A wrapper for a string <!--with the format of JS--></description>
///     </item>
///     <item>
///         <term><c>JSONFloat</c></term>
///         <description>A wrapper for a doublewhich is the presision JSON uses</description>
///     </item>
/// </list>
/// Also Adds JSON class which is "A mostly static class which helps yopu minippulate <c>JSONTypes</c>"
/// 
/// © RJ_Infinity 2021
/// </summary>
namespace RJJSON
{
    /// <summary>
    /// Class <c>JSON</c> A mostly static class which helps yopu minippulate <c>JSONTypes</c>
    /// </summary>
    class JSON
    {
        /// <summary>
        /// Method <c>StringToObject</c> converts a <c>string</c> representation of JSON to an object representation namely <c>JSONTypes</c> the sort used in C# code the oposit method is <code>JSON.ObjectToString()</code>
        /// </summary>
        /// <param name="Json">A string representation of JSON</param>
        /// <returns>A JSONTypes object with the same structure and values as the string representation</returns>
        public static JSONTypes StringToObject(string Json)
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
                return new JSONBool(true);
            }
            if (Json == "false")
            {
                return new JSONBool(false);
            }
            double JsonAsDBL = 0;
            if (double.TryParse(Json, out JsonAsDBL))
            {
                return new JSONFloat(JsonAsDBL);
            }
            switch (Json.Substring(0, 1))//get type of fist thing
            {
                case "\""://string
                    if (Json.Substring(Json.Length - 1, 1) == "\"")
                    {
                        return new JSONString(RemoveEscChars(Json.Substring(1, Json.Length - 2)));//it will just be a string
                    }
                    else//it is incorectly formated
                    {
                        throw new FormatException("The JSON string is incorectly formated");
                    }
                break;
                case "{":
                    if (Json.Substring(Json.Length - 1, 1) == "}")
                    {
                        JSONDictionary returnv = new JSONDictionary();
                        if (Json.Substring(1, Json.Length - 2).Length > 0){
                            foreach (string str in SplitToJsonObj(Json.Substring(1, Json.Length - 2)))
                            {
                                KeyValuePair<string,JSONTypes> KeyValue = GetKeyValuePair(str);
                                returnv.Data.Add(KeyValue.Key, KeyValue.Value);
                            }
                        }
                        return returnv;
                    }
                    else//it is incorectly formated
                    {
                        throw new FormatException("The JSON string is incorectly formated");
                    }
                break;
                case "[":
                    if (Json.Substring(Json.Length - 1, 1) == "]")
                    {
                        JSONList returnv = new JSONList();
                        if (Json.Substring(1, Json.Length - 2).Length > 0)
                        {
                            foreach (string str in SplitToJsonObj(Json.Substring(1, Json.Length - 2)))
                            {
                                returnv.Data.Add(StringToObject(str));
                            }
                        }
                        return returnv;
                    }
                    else//it is incorectly formated
                    {
                        throw new FormatException("The JSON string is incorectly formated");
                    }
                break;
            }
            return new JSONString("");//just so the compiler dosnt shout at me (this should probs be an exception)
        }
        private static KeyValuePair<string, JSONTypes> GetKeyValuePair(string Json)
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
                        return new KeyValuePair<string, JSONTypes>(Key, StringToObject(Value));
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
            return new KeyValuePair<string, JSONTypes>(Key, StringToObject(Value));
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
        public static string ObjectToString(JSONTypes Json)
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
                foreach(KeyValuePair<string, JSONTypes> El in Json.Data)
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
                foreach (JSONTypes El in Json.Data)
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
                returnv = Json.Data.ToString().ToLower();
            }
            else if (Json.Type == JSON.Types.STRING)
            {
                returnv = "\"" + Json.Data.Replace("\"", "\\\"").Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\b", "\\b").Replace("\f", "\\f").Replace("\r", "\\r") + "\"";
            }
            else if (Json.Type == JSON.Types.FLOAT)
            {
                returnv = ((decimal)Json.Data).ToString();
            }
            else
            {
                throw new Exception("Unknown Data Type\"" + Json.Type + "\"");
            }
            return returnv;
        }
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
                        break;
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
        public static string FormatJson(JSONTypes JSONStr){
            return FormatJson(JSONStr.ToString());
        }
        /// <summary>
        /// All The Types JSON supports
        /// (it may appear that JSON supports Int as it has this option
        /// </summary>
        public enum Types
        {
            NULL,
            DICT,
            LIST,
            BOOL,
            STRING,
            FLOAT
        }
        public static Type JSONTypesToTypes(JSON.Types type)
        {
            switch (type)
            {
                case JSON.Types.NULL:
                    return null;
                case JSON.Types.DICT:
                    return typeof(Dictionary<string, JSONTypes>);
                case JSON.Types.LIST:
                    return typeof(List<JSONTypes>);
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
    /// Class <c>JSONTypes</c> base type for all JSON types.
    /// </summary>
    abstract class JSONTypes//all types must inhehrit of one class so that they can go togeter in a list or dict
    {
        /// <summary>
        /// field <c>Type</c>
        /// <returns>The JSON type as a <code>JSON.Types</code> int the case of the parent <c>JSONTypes</c> returns <code>JSON.Types.NULL</code></returns>
        /// </summary>
        public abstract JSON.Types Type { get; }
        /// <summary>
        /// field <c>Data</c> Get or Set the acctual data the instance holds
        /// </summary>
        public abstract dynamic Data { get; set; }
        /// <summary>
        /// method <c>GetData</c> alaias for <code>this.Data{get}</code>
        /// </summary>
        /// <typeparam name="T">The return type. Should be the representation of <code>this.Type</code></typeparam>
        /// <returns>The acctual data the instancve holds</returns>
        public abstract dynamic GetData<T>();
        /// <summary>
        /// method <c>ToString</c> alias for <code>JSON.ObjectToString(this)</code> or in other words the <code>JSON.ObjectToString</code> of itsself
        /// </summary>
        /// <returns>JSON.ObjectToString(this)</returns>
        public override string ToString()
        {
            return JSON.ObjectToString(this);
        }
    }
    class JSONDictionary : JSONTypes
    {
        public override JSON.Types Type { get { return JSON.Types.DICT; } }
        private Dictionary<string, JSONTypes> data = new Dictionary<string, JSONTypes> { };
        public override dynamic Data
        {
            get
            {
                return data;
            }
            set
            {
                throw new Exception("Data cannot be assigned to-- it is read only");
            }
        }
        public override dynamic GetData<T>()
        {
            if (typeof(T) == typeof(Dictionary<string, JSONTypes>))
            {
                return data;
            }
            else
            {
                return null;
            }
        }

    }
    class JSONList : JSONTypes
    {
        public override JSON.Types Type { get { return JSON.Types.LIST; } }
        private List<JSONTypes> data = new List<JSONTypes> { };
        public override dynamic Data
        {
            get
            {
                return data;
            }
            set
            {
                throw new Exception("Data cannot be assigned to-- it is read only");
            }
        }
        public override dynamic GetData<T>()
        {
            if (typeof(T) == typeof(List<JSONTypes>))
            {
                return data;
            }
            else
            {
                return null;
            }
        }
    }
    class JSONString : JSONTypes
    {
        public JSONString(string json)
        {
            data = json;
        }
        public override JSON.Types Type { get { return JSON.Types.STRING; } }
        private string data;
        public override dynamic Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }
        public override dynamic GetData<T>()
        {
            if (typeof(T) == typeof(string))
            {
                return data;
            }
            else
            {
                return null;
            }
        }
        public void SetData(string NewData)
        {
            data = NewData;
        }
    }
    class JSONBool : JSONTypes
    {
        public JSONBool(bool json)
        {
            data = json;
        }
        public override JSON.Types Type { get { return JSON.Types.BOOL; } }
        private bool data;
        public override dynamic Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }
        public override dynamic GetData<T>()
        {
            if (typeof(T) == typeof(bool))
            {
                return data;
            }
            else
            {
                return null;
            }
        }
        public void SetData(bool NewData)
        {
            data = NewData;
        }
    }
    class JSONFloat : JSONTypes
    {
        public JSONFloat(double json)
        {
            data = json;
        }
        public override JSON.Types Type { get { return JSON.Types.FLOAT; } }
        public double data;
        public override dynamic Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }
        public override dynamic GetData<T>()
        {
            if (typeof(T) == typeof(double))
            {
                return data;
            }
            else
            {
                return null;
            }
        }
        public void SetData(double NewData)
        {
            data = NewData;
        }
    }
}
