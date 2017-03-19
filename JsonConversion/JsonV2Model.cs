using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonConversion
{
    public class JsonV2Model
    {
        public JsonV2Model()
        {
            Products = new Dictionary<int, ProductV2Model>();
            version = "2";
        }

        public string version;
        public Dictionary<int, ProductV2Model> Products;
    }

    public class ProductV2Model
    {
        public string name;
        public int price;
        public int count;
    }
}
