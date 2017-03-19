using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public int price;
        public int count;
    }
}
