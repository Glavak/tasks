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
            products = new List<ProductV3Abstract>();
            version = "3";
        }
        public string version;
        public List<ProductV3Abstract> products;
    }

    public abstract class ProductV3Abstract
    {
        public int id;
        public string name;

        public int count;
    }

    public class ProductV3Successfull : ProductV3Abstract
    {
        public double price;
    }

    public class ProductV3Error : ProductV3Abstract
    {
        public string price => "error";
    }
}
