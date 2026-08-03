using StoreSim;

Store myStore = new Store();

//ai gened constraint test for now
myStore.AddProduct(new Product { Name = "Laptop", Id = 1, Description = "A high-performance laptop", Category = "Electronics", Quantity = 10, Price = 999.99m });
myStore.AddProduct(new Product { Name = "Smartphone", Id = 2, Description = "A latest model smartphone", Category = "Electronics", Quantity = 20, Price = 699.99m });

int customerId = 42;

// 1. Test: Reserve an item successfully
Console.WriteLine($"--- Customer {customerId} trying to reserve 2 Laptops ---");
bool reserve1 = myStore.ReserveItem(customerId, 1, 2);
Console.WriteLine($"Reservation successful? {reserve1}");
Console.WriteLine($"Laptop remaining stock: {myStore.Inventory[0].Quantity}\n");

// 2. Test: Try to over-reserve (Should fail because stock is too low)
Console.WriteLine($"--- Customer {customerId} trying to reserve 5 Smartphones (only 20 left) ---");
bool reserve2 = myStore.ReserveItem(customerId, 2, 5);
Console.WriteLine($"Reservation successful? {reserve2}");
Console.WriteLine($"Smartphone remaining stock: {myStore.Inventory[1].Quantity}\n");

// 3. Test: Return an item from the cart
Console.WriteLine($"--- Customer {customerId} returning 1 Laptop back to inventory ---");
bool returnSuccess = myStore.ReturnItemToInventory(customerId, 1, 1);
Console.WriteLine($"Return successful? {returnSuccess}");
Console.WriteLine($"Laptop stock after return: {myStore.Inventory[0].Quantity}\n");

// 4. Test: Complete checkout
Console.WriteLine($"--- Customer {customerId} completing checkout ---");
bool checkoutSuccess = myStore.CompleteCheckout(customerId);
Console.WriteLine($"Checkout successful? {checkoutSuccess}");

Console.WriteLine("\n=== SIMULATION COMPLETE ===");