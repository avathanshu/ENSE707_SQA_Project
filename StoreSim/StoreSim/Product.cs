using System;
using System.Collections.Generic;
using System.Text;

namespace StoreSim
{
    public class Product
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; } = 0;
        public decimal Price { get; set; } = 0.0m;    
    }
}
