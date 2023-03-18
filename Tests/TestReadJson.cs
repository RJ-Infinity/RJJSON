using Microsoft.VisualStudio.TestTools.UnitTesting;
using RJJSON;
using System;
using System.Collections;
using System.Threading;

namespace RJJSONTests
{
    [TestClass]
    public class TestReadJson
    {
        [TestMethod]
        public void TestString()
        {
            string json = @"""this is a test string""";

            JSONType Json = JSON.StringToObject(json);
            
            Assert.AreEqual("this is a test string",Json.StringData);
            Assert.AreEqual(JSON.Types.STRING,Json.Type);
            Assert.AreEqual(json,Json.ToString());
            Assert.ThrowsException<InvalidTypeException>(delegate () { TestHelper.Void(Json.DictData); });
            Assert.ThrowsException<InvalidTypeException>(delegate () { TestHelper.Void(Json.ListData); });
            Assert.ThrowsException<InvalidTypeException>(delegate () { TestHelper.Void(Json.FloatData); });
            Assert.ThrowsException<InvalidTypeException>(delegate () { TestHelper.Void(Json.BoolData); });
            Assert.ThrowsException<InvalidTypeException>(delegate () { TestHelper.Void(Json[0]); });
            Assert.ThrowsException<InvalidTypeException>(delegate () { TestHelper.Void(Json["test"]); });
            Assert.ThrowsException<InvalidTypeException>(delegate () { Json.GetEnumerableList(); });
        }
        [TestMethod]
        public void TestBool()
        {
            string truejson = @"true";
            string falsejson = @"false";

            JSONType TrueJson = JSON.StringToObject(truejson);
            JSONType FalseJson = JSON.StringToObject(falsejson);

            Assert.AreEqual(true, TrueJson.BoolData);
            Assert.AreEqual(false, FalseJson.BoolData);
            Assert.AreEqual(JSON.Types.BOOL, TrueJson.Type);
            Assert.AreEqual(JSON.Types.BOOL, FalseJson.Type);
            Assert.AreEqual(truejson, TrueJson.ToString());
            Assert.AreEqual(falsejson, FalseJson.ToString());

            Assert.ThrowsException<InvalidTypeException>(delegate () { TestHelper.Void(TrueJson.DictData); });
            Assert.ThrowsException<InvalidTypeException>(delegate () { TestHelper.Void(TrueJson.ListData); });
            Assert.ThrowsException<InvalidTypeException>(delegate () { TestHelper.Void(TrueJson.FloatData); });
            Assert.ThrowsException<InvalidTypeException>(delegate () { TestHelper.Void(TrueJson.StringData); });
            Assert.ThrowsException<InvalidTypeException>(delegate () { TestHelper.Void(TrueJson[0]); });
            Assert.ThrowsException<InvalidTypeException>(delegate () { TestHelper.Void(TrueJson["test"]); });
            Assert.ThrowsException<InvalidTypeException>(delegate () { TrueJson.GetEnumerableList(); });

            Assert.ThrowsException<InvalidTypeException>(delegate () { TestHelper.Void(FalseJson.DictData); });
            Assert.ThrowsException<InvalidTypeException>(delegate () { TestHelper.Void(FalseJson.ListData); });
            Assert.ThrowsException<InvalidTypeException>(delegate () { TestHelper.Void(FalseJson.FloatData); });
            Assert.ThrowsException<InvalidTypeException>(delegate () { TestHelper.Void(FalseJson.StringData); });
            Assert.ThrowsException<InvalidTypeException>(delegate () { TestHelper.Void(FalseJson[0]); });
            Assert.ThrowsException<InvalidTypeException>(delegate () { TestHelper.Void(FalseJson["test"]); });
            Assert.ThrowsException<InvalidTypeException>(delegate () { FalseJson.GetEnumerableList(); });
        }
        [TestMethod]
        public void TestFloat()
        {
            JSONType Json = JSON.StringToObject("345.678");
            Assert.AreEqual((double)345.678, Json.FloatData);
            Assert.AreEqual(JSON.Types.FLOAT, Json.Type);

            Assert.ThrowsException<InvalidTypeException>(delegate () { TestHelper.Void(Json.DictData); });
            Assert.ThrowsException<InvalidTypeException>(delegate () { TestHelper.Void(Json.ListData); });
            Assert.ThrowsException<InvalidTypeException>(delegate () { TestHelper.Void(Json.BoolData); });
            Assert.ThrowsException<InvalidTypeException>(delegate () { TestHelper.Void(Json.StringData); });
            Assert.ThrowsException<InvalidTypeException>(delegate () { TestHelper.Void(Json[0]); });
            Assert.ThrowsException<InvalidTypeException>(delegate () { TestHelper.Void(Json["test"]); });
            Assert.ThrowsException<InvalidTypeException>(delegate () { Json.GetEnumerableList(); });

            Assert.AreEqual(Json.FloatData, JSON.StringToObject(Json.ToString()).FloatData);
        }
        //[TestMethod]
        public void TestLargeData()
        {
            string json = @"
[{
  ""id"": 1,
  ""first_name"": ""Jeanette"",
  ""last_name"": ""Penddreth"",
  ""email"": ""jpenddreth0@census.gov"",
  ""gender"": ""Female"",
  ""ip_address"": ""26.58.193.2""
}, {
  ""id"": 2,
  ""first_name"": ""Giavani"",
  ""last_name"": ""Frediani"",
  ""email"": ""gfrediani1@senate.gov"",
  ""gender"": ""Male"",
  ""ip_address"": ""229.179.4.212""
}, {
  ""id"": 3,
  ""first_name"": ""Noell"",
  ""last_name"": ""Bea"",
  ""email"": ""nbea2@imageshack.us"",
  ""gender"": ""Female"",
  ""ip_address"": ""180.66.162.255""
}, {
  ""id"": 4,
  ""first_name"": ""Willard"",
  ""last_name"": ""Valek"",
  ""email"": ""wvalek3@vk.com"",
  ""gender"": ""Male"",
  ""ip_address"": ""67.76.188.26""
}]";
            JSONType type = JSON.StringToObject(json);
        }
    }
}