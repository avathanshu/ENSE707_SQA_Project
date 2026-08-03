using System;
using System.Collections.Generic;
using System.Text;

namespace StoreSim
{
    public class Customer
    {
        private static int _idCounter = 1;
        public int Id { get; set; }
        public required string Name { get; set; }
        
        public Customer(String name)
        {
            Id = _idCounter++;
            Name = name;
        }
    }
}
