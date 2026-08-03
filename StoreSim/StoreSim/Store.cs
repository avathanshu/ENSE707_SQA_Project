using System;
using System.Collections.Generic;
using System.Text;

namespace StoreSim
{
    public class Store
    {
        public List<Product> Inventory { get; set; } = new();

        // Dictionary to keep track of active carts for each customer
        private Dictionary<int, List<Cart>> _activeCarts = new();

        public void AddProduct(Product product)
        {
            Inventory.Add(product);
        }

        public bool ReserveItem(int customerId, int productId, int quantityForCart)
        {
            var product = Inventory.Find(p => p.Id == productId);
            if (product == null || product.Quantity < quantityForCart)
            {
                return false; // Product not found or insufficient stock
            }

            product.Quantity -= quantityForCart; // Reserve the items by reducing stock

            if (!_activeCarts.ContainsKey(customerId))
            {
                _activeCarts[customerId] = new List<Cart>();
            }

            var cart = _activeCarts[customerId].Find(c => c.ProductId == productId);
            if (cart != null)
            {
                cart.Quantity += quantityForCart;
            }
            else
            {
                _activeCarts[customerId].Add(new Cart { ProductId = productId, Quantity = quantityForCart });
            }
            return true;
        }

        public bool ReturnItemToInventory(int customerId, int productId, int quantityToReturn)
        {
            if (!_activeCarts.ContainsKey(customerId))
            {
                return false; // No active cart for this customer
            }

            var customerCart = _activeCarts[customerId];
            var cart = customerCart.Find(c => c.ProductId == productId);
            if (cart == null || cart.Quantity < quantityToReturn)
            {
                return false; // Product not in cart or insufficient quantity to return
            }

            var product = Inventory.Find(p => p.Id == productId);
            if (product != null)
            {
                product.Quantity += quantityToReturn; // Return the items to inventory
            }

            cart.Quantity -= quantityToReturn;

            if (cart.Quantity == 0)
            {
                customerCart.Remove(cart); // Remove the cart item if quantity is zero
            }

            return true;
        }

        public void ClearCustomerCart(int customerId)
        {
            if (! _activeCarts.ContainsKey(customerId)) { return; }

            foreach(var cart in _activeCarts[customerId].ToList())
            {
                ReturnItemToInventory(customerId, cart.ProductId, cart.Quantity); // Return all items to inventory
            }
            _activeCarts.Remove(customerId); // Clear the cart
        }

        public bool CompleteCheckout(int customerId)
        {
            if (!_activeCarts.ContainsKey(customerId))
            {
                return false; // No active cart for this customer
            }
            // Here you would typically process payment and finalize the order
            _activeCarts.Remove(customerId); // Clear the cart after checkout
            return true;
        }
    }
}
