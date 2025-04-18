using Microsoft.EntityFrameworkCore;
using WebStore.Data;
using WebStore.Entities;

namespace WebStore.Assignments
{
    /// Additional tutorial materials https://dotnettutorials.net/lesson/linq-to-entities-in-entity-framework-core/

    /// <summary>
    /// This class demonstrates various LINQ query tasks 
    /// to practice querying an EF Core database.
    /// 
    /// ASSIGNMENT INSTRUCTIONS:
    ///   1. For each method labeled "TODO", write the necessary
    ///      LINQ query to return or display the required data.
    ///      
    ///   2. Print meaningful output to the console (or return
    ///      collections, as needed).
    ///      
    ///   3. Test each method by calling it from your Program.cs
    ///      or test harness.
    /// </summary>
    public class LinqQueriesAssignment
    {

        private readonly AssignmentOrm3Context _dbContext;

        public LinqQueriesAssignment(AssignmentOrm3Context context)
        {
            _dbContext = context;
        }
        /// <summary>
        /// 1. List all customers in the database:
        ///    - Print each customer's full name (First + Last) and Email.
        /// </summary>
        public async Task Task01_ListAllCustomers()
        {
            // TODO: Write a LINQ query that fetches all customers
            //       and prints their names + emails to the console.
            // HINT: context.Customers

           //  Uncomment this code after generating the entity models
            
            var customers = await _dbContext.Customers
               // .AsNoTracking() // optional for read-only
               .ToListAsync();

            Console.WriteLine("=== TASK 01: List All Customers ===");

            foreach (var c in customers)
            {
                Console.WriteLine($"{c.FirstName} {c.LastName} - {c.Email}");
            }

            
        }

        /// <summary>
        /// 2. Fetch all orders along with:
        ///    - Customer Name
        ///    - Order ID
        ///    - Order Status
        ///    - Number of items in each order (the sum of OrderItems.Quantity)
        /// </summary>
        public async Task Task02_ListOrdersWithItemCount()
        {
            // TODO: Write a query to return all orders,
            //       along with the associated customer name, order status,
            //       and the total quantity of items in that order.

            // HINT: Use Include/ThenInclude or projection with .Select(...).
            //       Summing the quantities: order.OrderItems.Sum(oi => oi.Quantity).
            Console.WriteLine(" ");
            var orders = await _dbContext.Orders
                .Include(o => o.Customer)  // Join s tabulkou Customers
                .Include(o => o.OrderItems) // Join s tabulkou OrderItems
                .Select(o => new
                {
                    o.OrderId,
                    CustomerName = o.Customer.FirstName + " " + o.Customer.LastName,
                    o.OrderStatus,
                    ItemCount = o.OrderItems.Sum(oi => oi.Quantity)
                })
                .ToListAsync();

                Console.WriteLine("\n=== TASK 02: List Orders With Item Count ===");
                foreach (var o in orders)
                {
                    Console.WriteLine($"Order {o.OrderId} - {o.CustomerName} - {o.OrderStatus} - Items: {o.ItemCount}");
                }
            
        }

        /// <summary>
        /// 3. List all products (ProductName, Price),
        ///    sorted by price descending (highest first).
        /// </summary>
        public async Task Task03_ListProductsByDescendingPrice()
        {
            // TODO: Write a query to fetch all products and sort them
            //       by descending price.
            // HINT: context.Products.OrderByDescending(p => p.Price)
            Console.WriteLine(" ");
            var products = await _dbContext.Products
               .OrderByDescending(p => p.Price)
               .ToListAsync();

                Console.WriteLine("\n=== Task 03: List Products By Descending Price ===");
                foreach (var p in products)
                {
                    Console.WriteLine($"{p.ProductName} - ${p.Price}");
                }
        }

        /// <summary>
        /// 4. Find all "Pending" orders (order status = "Pending")
        ///    and display:
        ///      - Customer Name
        ///      - Order ID
        ///      - Order Date
        ///      - Total price (sum of unit_price * quantity - discount) for each order
        /// </summary>
        public async Task Task04_ListPendingOrdersWithTotalPrice()
        {
            // TODO: Write a query to fetch only PENDING orders,
            //       and calculate their total price.
            // HINT: The total can be computed from each OrderItem:
            //       (oi.UnitPrice * oi.Quantity) - oi.Discount
            Console.WriteLine(" ");
            var pendingOrders = await _dbContext.Orders
                .Where(o => o.OrderStatus == "Pending") //equivalent to SQL WHERE order_status = "Pending"
                .Include(o => o.Customer) //equivalent to SQL join with Customer (JOIN customers ON orders.customer_id = customers.customer_id) 
                .Include(o => o.OrderItems) //equivalent to SQL join with order_items
                .ThenInclude(oi => oi.Product) //on every founded Order will try to found related product
                .Select(o => new //select data to new object with selected fields from the query
                {
                    o.OrderId, //retrun OrderId
                    CustomerName = o.Customer.FirstName + " " + o.Customer.LastName, // retrun o.Customer.FirstName + " " + o.Customer.LastName
                    o.OrderDate, //retrun date
                    TotalPrice = o.OrderItems.Sum(oi => (oi.UnitPrice * oi.Quantity) - oi.Discount) //calculate total price
                })
                .ToListAsync(); //we need async becouse its not instant to fetch data, and wee need to wait for results

            Console.WriteLine("\n=== Task 04: List Pending Orders With Total Price ===");
            foreach (var o in pendingOrders) // Print every Order.
            {
                Console.WriteLine($"Order {o.OrderId} - {o.CustomerName} - Date: {o.OrderDate} - Total: ${o.TotalPrice}");
            }
        }

        /// <summary>
        /// 5. List the total number of orders each customer has placed.
        ///    Output should show:
        ///      - Customer Full Name
        ///      - Number of Orders
        /// </summary>
        public async Task Task05_OrderCountPerCustomer()
        {
            // TODO: Write a query that groups by Customer,
            //       counting the number of orders each has.

            // HINT: 
            //  1) Join Orders and Customers, or
            //  2) Use the navigation (context.Orders or context.Customers),
            //     then group by customer ID or by the customer entity.
            Console.WriteLine(" ");
            var orderCounts = await _dbContext.Orders
                .GroupBy(o => o.Customer)
                .Select(g => new
                {
                    CustomerName = g.Key.FirstName + " " + g.Key.LastName,
                    OrderCount = g.Count()
                })
                .ToListAsync();

                Console.WriteLine("\n=== Task 05: Order Count Per Customer ===");
                foreach (var oc in orderCounts)
                {
                    Console.WriteLine($"{oc.CustomerName} - {oc.OrderCount} orders");
                }
        }

        /// <summary>
        /// 6. Show the top 3 customers who have placed the highest total order value overall.
        ///    - For each customer, calculate SUM of (OrderItems * Price).
        ///      Then pick the top 3.
        /// </summary>
        public async Task Task06_Top3CustomersByOrderValue()
        {
            // TODO: Calculate each customer's total order value 
            //       using their Orders -> OrderItems -> (UnitPrice * Quantity - Discount).
            //       Sort descending and take top 3.

            // HINT: You can do this in a single query or multiple steps.
            //       One approach:
            //         1) Summarize each Order's total
            //         2) Summarize for each Customer
            //         3) Order by descending total
            //         4) Take(3)
            Console.WriteLine(" ");
            var topCustomers = await _dbContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .GroupBy(o => o.Customer)
                .Select(g => new
                {
                    CustomerName = g.Key.FirstName + " " + g.Key.LastName,
                    TotalSpent = g.Sum(o => o.OrderItems.Sum(oi => (oi.UnitPrice * oi.Quantity) - oi.Discount))
                })
                .OrderByDescending(c => c.TotalSpent)
                .Take(3)
                .ToListAsync();

                Console.WriteLine("\n=== Task 06: Top 3 Customers By Order Value ===");
                foreach (var c in topCustomers)
                {
                    Console.WriteLine($"{c.CustomerName} - Total Spent: ${c.TotalSpent}");
                }
        }

        /// <summary>
        /// 7. Show all orders placed in the last 30 days (relative to now).
        ///    - Display order ID, date, and customer name.
        /// </summary>
        public async Task Task07_RecentOrders()
        {
            // TODO: Filter orders to only those with OrderDate >= (DateTime.Now - 30 days).
            //       Output ID, date, and the customer's name.
            Console.WriteLine(" ");
            var recentOrders = await _dbContext.Orders
                .Where(o => o.OrderDate >= DateTime.Now.AddDays(-90))
                .Include(o => o.Customer)
                .Select(o => new
                {
                    o.OrderId,
                    o.OrderDate,
                    CustomerName = o.Customer.FirstName + " " + o.Customer.LastName,
                    o.OrderStatus,
                    o.DeliveredDate
                })
                .ToListAsync();

                Console.WriteLine("\n=== Task 07: Recent Orders ===");
                foreach (var o in recentOrders)
                {
                    Console.WriteLine($"Order {o.OrderId} - {o.CustomerName} - Date: {o.OrderDate} - Status: {o.OrderStatus} - Delibery Date: {o.DeliveredDate}");
                }
        }

        /// <summary>
        /// 8. For each product, display how many total items have been sold
        ///    across all orders.
        ///    - Product name, total sold quantity.
        ///    - Sort by total sold descending.
        /// </summary>
        public async Task Task08_TotalSoldPerProduct()
        {
            // TODO: Group or join OrdersItems by Product.
            //       Summation of quantity.
            Console.WriteLine(" ");
            var soldProducts = await _dbContext.OrderItems
                .GroupBy(oi => oi.Product)
                .Select(g => new
                {
                    ProductName = g.Key.ProductName,
                    TotalSold = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(p => p.TotalSold)
                .ToListAsync();

                Console.WriteLine("\n=== Task 08: Total Sold Per Product ===");
                foreach (var p in soldProducts)
                {
                    Console.WriteLine($"{p.ProductName} - {p.TotalSold} items sold");
                }
        }

        /// <summary>
        /// 9. List any orders that have at least one OrderItem with a Discount > 0.
        ///    - Show Order ID, Customer name, and which products were discounted.
        /// </summary>
        public async Task Task09_DiscountedOrders()
        {
            // TODO: Identify orders with any OrderItem having (Discount > 0).
            //       Display order details, plus the discounted products.
            Console.WriteLine(" ");
            var discountedOrders = await _dbContext.Orders
                .Where(o => o.OrderItems.Any(oi => oi.Discount > 0))
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Select(o => new
                {
                    o.OrderId,
                    CustomerName = o.Customer.FirstName + " " + o.Customer.LastName,
                    DiscountedProducts = o.OrderItems
                        .Where(oi => oi.Discount > 0)
                        .Select(oi => oi.Product.ProductName)
                        .ToList()
                })
                .ToListAsync();

                Console.WriteLine("\n=== Task 09: Discounted Orders ===");
                foreach (var o in discountedOrders)
                {
                    Console.WriteLine($"Order {o.OrderId} - {o.CustomerName}");
                    Console.WriteLine("  Discounted Products: " + string.Join(", ", o.DiscountedProducts));
                }
        }

        /// <summary>
        /// 10. (Open-ended) Combine multiple joins or navigation properties
        ///     to retrieve a more complex set of data. For example:
        ///     - All orders that contain products in a certain category
        ///       (e.g., "Electronics"), including the store where each product
        ///       is stocked most. (Requires `Stocks`, `Store`, `ProductCategory`, etc.)
        ///     - Or any custom scenario that spans multiple tables.
        /// </summary>
        public async Task Task10_AdvancedQueryExample()
        {
            // TODO: Design your own complex query that demonstrates
            //       multiple joins or navigation paths. For example:
            //       - Orders that contain any product from "Electronics" category.
            //       - Then, find which store has the highest stock of that product.
            //       - Print results.

            // Here's an outline you could explore:
            // 1) Filter products by category name "Electronics"
            // 2) Find any orders that contain these products
            // 3) For each of those products, find the store with the max stock
            //    (requires .MaxBy(...) in .NET 6+ or custom code in older versions)
            // 4) Print a combined result

            // (Implementation is left as an exercise.)
            Console.WriteLine(" ");
            var electronicsOrders = await _dbContext.Orders
             .Include(o => o.OrderItems)
             .ThenInclude(oi => oi.Product)
             .ThenInclude(p => p.Categories) // Použití kolekce Categories pøímo
             .Where(o => o.OrderItems.Any(oi => oi.Product.Categories.Any(c => c.CategoryName == "Electronics")))
             .Select(o => new
             {
                 o.OrderId,
                 CustomerName = o.Customer.FirstName + " " + o.Customer.LastName,
                 ElectronicsProducts = o.OrderItems
                     .Where(oi => oi.Product.Categories.Any(c => c.CategoryName == "Electronics"))
                     .Select(oi => oi.Product.ProductName)
                     .ToList()
             })
             .ToListAsync();

            Console.WriteLine("\n=== Task 10: Advanced Query Example ===");
            foreach (var o in electronicsOrders)
            {
                Console.WriteLine($"Order {o.OrderId} - {o.CustomerName}");
                Console.WriteLine("  Electronics Products: " + string.Join(", ", o.ElectronicsProducts));
            }
        }
    }
}
