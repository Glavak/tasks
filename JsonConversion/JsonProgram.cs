using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace JsonConversion
{
	class JsonProgram
	{
		static void Main()
		{
			string json = Console.In.ReadToEnd();
			
			Console.Write(Convert(json));
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

    [TestFixture]
    public class JsonProgram_Should
    {
        [Test]
        public void Test1()
        {
            var input = 
                "{	\"version\": \"2\",	\"products\": {		\"1\": {			\"name\": \"Pen\",			\"price\": 12,			\"count\": 100		},		\"2\": {			\"name\": \"Pencil\",			\"price\": 8,			\"count\": 1000		},		\"3\": {			\"name\": \"Box\",			\"price\": 12.1,			\"count\": 50		}	}}";
            var expected = "{ \'version\':\'3\', \'products\': \'[{\"id\":1,\"name\":\"Pen\",\"price\":12.0,\"count\":100},{\"id\":2,\"name\":\"Pencil\",\"price\":8.0,\"count\":1000},{\"id\":3,\"name\":\"Box\",\"price\":12.1,\"count\":50}]\' }";

            var actual = JsonProgram.Convert(input);

            Assert.AreEqual(expected, actual);
        }
    }

    class V3Product
    {
        public int id;

        public V3Product(int id, Product oldProduct)
        {
            this.id = id;
            this.name = oldProduct.name;
            this.price = oldProduct.price;
            this.count = oldProduct.count;
        }

        public string name;
        public double price;
        public int count;
    }

    class Product
    {
        public string name;
        public double price;
        public int count;
    }
}
