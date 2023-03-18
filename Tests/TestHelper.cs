using RJJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJJSONTests
{
    internal static class TestHelper
    {
        public static void Void(object v) { }
        public static List<JSONType> GetEnumerableList(this JSONType source)
        {
            List<JSONType> rv = new List<JSONType>();
            foreach (JSONType jt in source)
            {
                rv.Append(jt);
            }
            return rv;
        }
    }
}
