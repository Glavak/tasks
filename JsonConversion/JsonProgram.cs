﻿using Newtonsoft.Json.Linq;
using System;
using Newtonsoft.Json;
using NUnit.Framework;

namespace JsonConversion
{
	class JsonProgram
	{
		static void Main()
		{
			string json = Console.In.ReadToEnd();
            var v2 = JObject.Parse(json).ToObject<JsonV2Model>();

            Console.WriteLine(JsonConvert.SerializeObject(ConvertV2ToV3.ConvertToV3(v2)));
		}

	    public static string Convert(string input)
	    {
            JObject v2 = JObject.Parse(input);

            List<V3Product> products = new List<V3Product>();

            foreach (var token in v2["products"])
            {
                Product p = token.First.ToObject<Product>();
                int id = System.Convert.ToInt32(token.Path.Split('.').Last());
                products.Add(new V3Product(id, p));
            }
            
            return "{ 'version':'3', 'products': '" + JsonConvert.SerializeObject(products) + "' }";
        }
	}

    public static class ConvertV2ToV3
    {
        public static JsonV3Model ConvertToV3(JsonV2Model v2Model)
        {
            var v3Model = new JsonV3Model();
            foreach (var model in v2Model.Products)
            {
                v3Model.products.Add(new ProductV3Model()
                {
                    id = model.Key,
                    count = model.Value.count,
                    name = model.Value.name,
                    price = model.Value.price
                });
            }
            return v3Model;
        }
    }

    [TestFixture]
    public class JsonProgram_Should
    {
        [Test]
        public void test_something()
        {
            string v2 = "{\"version\": \"2\",\"products\": {\"1\": {\"name\": \"Pen\",\"price\": 12,\"count\": 100}," +
                        "\"2\": {\"name\": \"Pencil\",\"price\": 8,\"count\": 1000}," +
                        "\"3\": {\"name\": \"Box\",\"price\": 12.1,\"count\": 50}}}";

            var s = JObject.Parse(v2).ToObject<JsonV2Model>();
            //Console.Out.WriteLine("s = {0}", s);

        }
    }
}
