using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Constraints;

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
	}

    public static class ConvertV2ToV3
    {
        public static JsonV3Model ConvertToV3(JsonV2Model v2Model)
        {
            var v3Model = new JsonV3Model();
            var constants = v2Model.Constants;
            foreach (var model in v2Model.Products)
            {
                if (constants == null)
                    v3Model.products.Add(ConvertIfPriceIsANumber(model));
                else
                    v3Model.products.Add(ConvertIfPriceIsAFormula(model, constants));
            }
            return v3Model;
        }

        private static ProductV3Model ConvertIfPriceIsANumber(KeyValuePair<int, ProductV2Model> model)
        {
            return new ProductV3Model()
            {
                id = model.Key,
                count = model.Value.count,
                name = model.Value.name,
                price = Convert.ToDouble(model.Value.price)
            };
        }

        private static ProductV3Model ConvertIfPriceIsAFormula(KeyValuePair<int, ProductV2Model> model, Dictionary<string, double> constants)
        {
            foreach (var constant in constants)
                if (model.Value.price.Contains(constant.Key))
                    model.Value.price = model.Value.price.Replace(constant.Key, constant.Value.ToString());

            var price = EvalTask.EvalProgram.Process(model.Value.price);
            return new ProductV3Model()
            {
                id = model.Key,
                count = model.Value.count,
                name = model.Value.name,
                price = double.Parse(price, CultureInfo.InvariantCulture)
            };
        }
    }

    [TestFixture]
    public class JsonProgram_Should
    {
        [Test]
        public void TestConvertV2ToV3()
        {
            string v2String = "{\"version\": \"2\",\"products\": {\"1\": {\"name\": \"Pen\",\"price\": 12,\"count\": 100}," +
                        "\"2\": {\"name\": \"Pencil\",\"price\": 8,\"count\": 1000}," +
                        "\"3\": {\"name\": \"Box\",\"price\": 12.1,\"count\": 50}}}";
            var v2Json = JObject.Parse(v2String).ToObject<JsonV2Model>();
            string v3String = "{\"version\":\"3\",\"products\":[{\"id\":1,\"name\":\"Pen\",\"price\":12,\"count\":100},{\"id\":2,\"name\":\"Pencil\",\"price\":8,\"count\":1000},{\"id\":3,\"name\":\"Box\",\"price\":12.1,\"count\":50}]}";
            var result = JsonConvert.SerializeObject(ConvertV2ToV3.ConvertToV3(v2Json));

            Assert.AreEqual(v3String, result);
        }

        [Test]
        public void TestFormulaInPrice()
        {
            string v2String = "{\'version\':\'2\',\'constants\':{\'a\':3,\'b\':4,\'c\':4.5},\'products\':{\'1\':{\'name\':\'product-name\',\'price\':\'c+a+b\',\'count\':100}}}";
            var v2Json = JObject.Parse(v2String).ToObject<JsonV2Model>();
            string v3String = "{\"version\":\"3\",\"products\":[{\"id\":1,\"name\":\"product-name\",\"price\":11.5,\"count\":100}]}";
            var result = JsonConvert.SerializeObject(ConvertV2ToV3.ConvertToV3(v2Json));

            Assert.AreEqual(v3String, result);
        }
    }
}
