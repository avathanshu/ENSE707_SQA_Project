using System;
using System.Collections.Generic;
using System.Text;

namespace StoreSim
{
    public class CartDisplayItem
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Quantity * Price;
    }
}
