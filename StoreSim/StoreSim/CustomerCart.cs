using System;
using System.Collections.Generic;
using System.Text;

namespace StoreSim
{
    public class CustomerCart
    {
        public int CustomerId { get; set; }
        public List<Cart> Items { get; set; } = new();
    }
}
