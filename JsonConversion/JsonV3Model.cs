using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JsonConversion
{
    public class JsonV3Model
    {
        public JsonV3Model()
        {
            products = new List<ProductV3Model>();
            version = "3";
        }
        public string version;
        public List<ProductV3Model> products;
    }

    public class ProductV3Model
    {
        public int id;
        public string name;

        //[JsonConverter(typeof(DecimalFormatConverter))]
        public double price;
        public int count;
    }

    /*public class DecimalFormatConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(decimal));
        }

        public override void WriteJson(JsonWriter writer, object value,
                                       JsonSerializer serializer)
        {
            writer.WriteValue(((double)value).ToString("0.#"));
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override object ReadJson(JsonReader reader, Type objectType,
                                     object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }*/
}
