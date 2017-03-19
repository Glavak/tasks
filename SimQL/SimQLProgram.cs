using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
                if(!String.IsNullOrEmpty(JsonString)) 
                 result.Add((iterator + " = " + GetJSObject(jObject,iterator).Replace(",",".")));
              
		    }
            return result;
		}

        
	    public static string GetJSObject(JObject data, string query)
	    {
	        JObject o = data;
            JToken acme = data.SelectToken("data." + query);

            return acme.ToString();
        }

       
    }
}
