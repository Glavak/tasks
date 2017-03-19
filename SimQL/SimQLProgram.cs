using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SimQLTask;

namespace SimQLTask
{
    class SimQLProgram
    {
        static void Main(string[] args)
        {
            string json = Console.In.ReadToEnd();
            foreach (var result in ExecuteQueries(json))
                Console.WriteLine(result);
        }

        public static IEnumerable<string> ExecuteQueries(string json)
        {
            List<string> result = new List<string>();

            var jObject = JObject.Parse(json);
            var data = (JObject) jObject["data"];
            var queries = jObject["queries"].ToObject<string[]>();

            foreach (var iterator in queries)
            {
                string JsonString = GetJSObject(jObject, iterator);

                if (!String.IsNullOrEmpty(JsonString) && !JsonString.Equals("{}"))

                    result.Add((iterator + " = " + JsonString.Replace(",", ".")));
                else
                {
                    result.Add(iterator);
                }
            }
            return result;
        }

        public static string GetJSObject(JObject data, string query)
        {


            JObject o = data;
            JToken acme;
            List<JToken> tokensToAggregate = new List<JToken>();
            if (!query.Contains("min") && !query.Contains("max") && !query.Contains("sum"))
                tokensToAggregate = data.SelectTokens("data." + query).ToList<JToken>();

            JToken resultToken = null;

            var escapedQuery = getPath(query);
            if (query.Contains("min("))
            {
                var selectedTokens = data.SelectTokens("data." + escapedQuery).ToList();
                if (selectedTokens.Count == 0)
                {

                    escapedQuery = escapedQuery.Substring(0, escapedQuery.LastIndexOf("."));

                    var neededElement = query.Substring(query.LastIndexOf("."),
                        query.Length - query.LastIndexOf(".") - 1);
                    selectedTokens = data.SelectTokens("data." + escapedQuery + "[*]" + neededElement).ToList();
                    resultToken = selectedTokens.Min(s => s.Value<double>());
                }
                else if (selectedTokens.First().Type == JTokenType.Array)
                    resultToken =
                        selectedTokens.Select(s => s.Value<JArray>()).Children().Min(s => s.Value<double>());
                else if (selectedTokens.First().Type == JTokenType.Float ||
                         selectedTokens.First().Type == JTokenType.Integer)
                    resultToken = selectedTokens.Min(s => s.Value<double>());
            }
            else if (query.Contains("sum("))
            {
                var selectedTokens = data.SelectTokens("data." + escapedQuery).ToList();
                if (selectedTokens.Count == 0)
                {

                    escapedQuery = escapedQuery.Substring(0, escapedQuery.LastIndexOf("."));

                    var neededElement = query.Substring(query.LastIndexOf("."),
                        query.Length - query.LastIndexOf(".") - 1);

                    selectedTokens = data.SelectTokens("data." + escapedQuery + "[*]" + neededElement).ToList();
                    if (selectedTokens[0].Type == JTokenType.Array)
                        selectedTokens = data.SelectTokens("data." + escapedQuery + "[*]" + neededElement + "[*]").ToList();
                    else if (selectedTokens[0].Type == JTokenType.Integer)
                        selectedTokens = data.SelectTokens("data." + escapedQuery + "[*]" + neededElement).ToList();

                    resultToken = selectedTokens.Sum(s => s.Value<double>());
                }
                else if (selectedTokens.First().Type == JTokenType.Array)
                    resultToken =
                        selectedTokens.Select(s => s.Value<JArray>()).Children().Sum(s => s.Value<double>());
                else if (selectedTokens.First().Type == JTokenType.Float ||
                         selectedTokens.First().Type == JTokenType.Integer)
                    resultToken = selectedTokens.Sum(s => s.Value<double>());

            }
            else if (query.Contains("max("))
            {
                var selectedTokens = data.SelectTokens("data." + escapedQuery).ToList();
                if (selectedTokens.Count == 0)
                {

                    escapedQuery = escapedQuery.Substring(0, escapedQuery.LastIndexOf("."));

                    var neededElement = query.Substring(query.LastIndexOf("."),
                        query.Length - query.LastIndexOf(".") - 1);
                    selectedTokens = data.SelectTokens("data." + escapedQuery + "[*]" + neededElement).ToList();
                    resultToken = selectedTokens.Max(s => s.Value<double>());
                }
                else if (selectedTokens.First().Type == JTokenType.Array)
                    resultToken =
                        selectedTokens.Select(s => s.Value<JArray>()).Children().Max(s => s.Value<double>());
                else if (selectedTokens.First().Type == JTokenType.Float ||
                         selectedTokens.First().Type == JTokenType.Integer)
                    resultToken = selectedTokens.Max(s => s.Value<double>());

            }

            if (resultToken != null)
                return resultToken.ToString();


            try
            {
                return data.SelectToken("data." + query).ToString();
            }
            catch (Exception e)
            {
                return "";
            }

        }

        public static string getPath(string query)
        {
            var resultStr = query.Replace("min(", string.Empty)
                .Replace("sum(", string.Empty)
                .Replace("max(", string.Empty)
                .Replace(")", string.Empty);
            return resultStr;
        }

        [TestFixture]
        public class SimQLProgram_Should
        {
            [Test]
            public void Pass_WhenNull()
            {
                var input =
                    "{    \"data\": {        \"a\": {            \"x\":3.14,             \"b\": {\"c\":15},             \"c\": {\"c\":9}        },         \"z\":42    },    \"queries\": [        \"a.b.c\",        \"z\",        \"a.x\"    ]}";
                var output = "a.b.c = 15\r\nz = 42\r\na.x = 3.14";
                var result = ExecuteQueries(input);
                Assert.AreEqual(output, String.Join("\r\n", result));
            }

            [Test]
            public void Pass_EmptyJson()
            {
                var input =
                    "{\"data\":{\"empty\":{},\"ab\":0,\"x1\":1,\"x2\":2,\"y1\":{\"y2\":{\"y3\":3}}},\"queries\":[\"empty\",\"xyz\",\"x1.x2\",\"y1.y2.z\",\"empty.foobar\"]}";
                var output = "empty\r\nxyz\r\nx1.x2\r\ny1.y2.z\r\nempty.foobar";
                var result = ExecuteQueries(input);
                Assert.AreEqual(output, String.Join("\r\n", result));
            }

            [Test]
            public void math_funct()
            {
                var input =
                    "{\"data\":{\"a\":{\"x\":3.14,\"b\":[{\"c\":15},{\"c\":9}]},\"z\":[2.65,35]},\"queries\":[\"sum(a.b.c)\",\"min(z)\",\"max(a.x)\"]}";
                var output = "sum(a.b.c) = 24\r\nmin(z) = 2.65\r\nmax(a.x) = 3.14";
                var result = ExecuteQueries(input);
                Assert.AreEqual(output, String.Join("\r\n", result));
            }
            [Test]
            public void PassAgregate_WhenNull()
            {
                var input =
                    "{\"data\":{\"empty\":[],\"x\":[0.1,0.2,0.3],\"a\":[{\"b\":10,\"c\":[1,2,3]},{\"b\":30,\"c\":[4]},{\"d\":500}]},\"queries\":[\"sum(empty)\",\"sum(a.b)\",\"sum(a.c)\",\"sum(a.d)\",\"sum(x)\"]}";
                var output = "sum(empty) = 0\r\nsum(a.b) = 40\r\nsum(a.c) = 10\r\nsum(a.d) = 500\r\nsum(x) = 0.6";
                var result = ExecuteQueries(input);
                Assert.AreEqual(output, String.Join("\r\n", result));
            }
        }
    }
}