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

        private static ProductV3Abstract ConvertIfPriceIsANumber(KeyValuePair<int, ProductV2Model> model)
        {
            return new ProductV3Successfull()
            {
                id = model.Key,
                count = model.Value.count,
                name = model.Value.name,
                price = Convert.ToDouble(model.Value.price)
            };
        }

        private static ProductV3Abstract ConvertIfPriceIsAFormula(KeyValuePair<int, ProductV2Model> model, Dictionary<string, double> constants)
        {
            var price = EvalTask.EvalProgram.Process(model.Value.price, constants);
            if(price != "error")
                if(model.Value.size != null)
                    return new ProductV3Sized()
                    {
                        id = model.Key,
                        count = model.Value.count,
                        name = model.Value.name,
                        dimensions = new Dimensions(model.Value.size),
                        price = double.Parse(price, CultureInfo.InvariantCulture)
                    };
                else
                    return new ProductV3Successfull()
                    {
                        id = model.Key,
                        count = model.Value.count,
                        name = model.Value.name,
                        price = double.Parse(price, CultureInfo.InvariantCulture)
                    };
            else
                return new ProductV3Error()
                {
                    id = model.Key,
                    count = model.Value.count,
                    name = model.Value.name
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
            string v3String = "{\"version\":\"3\",\"products\":[{\"id\":1,\"name\":\"Pen\",\"price\":12,\"count\":100}," +
                "{\"id\":2,\"name\":\"Pencil\",\"price\":8,\"count\":1000}," +
                "{\"id\":3,\"name\":\"Box\",\"price\":12.1,\"count\":50}]}";
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

        [Test]
        public void TestFormulaInPriceAnotherCase()
        {
            string v2String = "{\"version\":\"2\"," +
                "\"constants\":{\"p\":375570429.0,\"BDT5b\":1895861230.0,\"pk6rq0teL\":2147483647.0,\"d801p5J6\":1994942430.0,\"GuuguI\":0.0}," +
                "\"products\":{\"0\":{\"name\":\"RQbl0WfVY\",\"price\":\"(-84.06633771214)*(p)\",\"count\":881891749}," +
                "\"1\":{\"name\":\"kDaf0Z\",\"price\":\"(12.1639563293028)+(p)\",\"count\":2147483647}," +
                "\"304322859\":{\"name\":\"KcOi9dvy\",\"price\":\"(pk6rq0teL)/(-37.0159578216336)\",\"count\":1}," +
                "\"1408114756\":{\"name\":\"v4N\",\"price\":\"(-85.7739047081088)/(37.9361148634628)\",\"count\":1}," +
                "\"1889271802\":{\"name\":\"wtXpL\",\"price\":\"(13.8310322136763)*(58.0251485845191)\",\"count\":0}," +
                "\"687275581\":{\"name\":\"bp\",\"price\":\"(-92.3887056263111)-(d801p5J6)\",\"count\":0}," +
                "\"1732466776\":{\"name\":\"IohrwvzQ\",\"price\":\"(d801p5J6)+(41.3040054688715)\",\"count\":2147483647}," +
                "\"2147483647\":{\"name\":\"Abap\",\"price\":\"(-59.916888624391)-(-66.5829328664499)\",\"count\":2147483647}," +
                "\"517841970\":{\"name\":\"KD9S4\",\"price\":\"(58.4912672445603)/(p)\",\"count\":2147483647}," +
                "\"1362982700\":{\"name\":\"9pH\",\"price\":\"(-54.4264007147524)/(-45.6487527795363)\",\"count\":1}}}";
            var v2Test = JObject.Parse(v2String).ToObject<JsonV2Model>();
            var result = JsonConvert.SerializeObject(ConvertV2ToV3.ConvertToV3(v2Test));


        }
    }
}
