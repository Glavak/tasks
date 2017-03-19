﻿using System;
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
			var data = (JObject)jObject["data"];
			var queries = jObject["queries"].ToObject<string[]>();
          
            foreach (var iterator in queries)
            {
                string JsonString = GetJSObject(jObject, iterator);
                
                if(!String.IsNullOrEmpty(JsonString) && !JsonString.Equals("{}"))
                  
                 result.Add((iterator + " = " + JsonString.Replace(",",".")));
                else
                {
                     result.Add(iterator);
                }
              
		    }
            return result;
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
                Assert.AreEqual(output,String.Join("\r\n",result));
	        }

            [Test]
            public void Pass_EmptyJson()
            {
                var input =
                    "{\"data\":{\"empty\":{},\"ab\":0,\"x1\":1,\"x2\":2,\"y1\":{\"y2\":{\"y3\":3}}},\"queries\":[\"empty\",\"xyz\",\"x1.x2\",\"y1.y2.z\",\"empty.foobar\"]}";
                var output = "empty = \r\nxyz = \r\nx1.x2 = \r\ny1.y2.z = \r\nempty.foobar = ";
                var result = ExecuteQueries(input);
                Assert.AreEqual(output, String.Join("\r\n", result));
            }

	        [Test]
	        public void math_funct()
	        {
                var input =
                   "{    \"data\": {\"a\":{\"x\":3.14, \"b\":[{\"c\":15}, {\"c\":9}]}, \"z\":[2.65, 35]},    \"queries\": [        \"sum(a.b.c)\",        \"min(z)\",        \"max(a.x)\"    ]}";
                var output = "sum(a.b.c) = 24\r\nmin(z) = 2.65\r\nmax(a.x) = 3.14";
                var result = ExecuteQueries(input);
                Assert.AreEqual(output, String.Join("\r\n", result));

            }
	    }
        
	    public static string GetJSObject(JObject data, string query)
	    {
           

	        JObject o = data;
	        JToken acme;
            
            try
	        {
                return data.SelectToken("data." + query).ToString();
            }
	        catch (Exception e)
	        {
	            return "";
	        }


            //TO DO NEXT2 TASK

            

               

        }

	    public static string replaceMathFunc(string query)
	    {
            query.Replace("min(", string.Empty);
            query.Replace(")", string.Empty);
            query.Replace("sum(", string.Empty);
            query.Replace(")", string.Empty);
            query.Replace("max(", string.Empty);
            query.Replace(")", string.Empty);

	        return query;

	    }

       
    }
}
