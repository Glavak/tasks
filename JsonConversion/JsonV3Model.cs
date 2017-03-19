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
        public Dimensions dimensions;
    }

    public class ProductV3Successfull : ProductV3Abstract
    {
        public double price;
    }

    public class ProductV3Error : ProductV3Abstract
    {
        public double price => 0;
    }

    public class Dimensions
    {
        public int l;
        public int w;
        public int h;

        public Dimensions(int[] size)
        {
            this.l = size[2];
            this.w = size[0];
            this.h = size[1];
        }
    }
}
