
using Microsoft.EntityFrameworkCore;
using WebStore.Assignments;
using WebStore.Data;
using WebStore.Entities;

namespace WebStore
{
    class Program
    {
        static async Task Main(string[] args)
        {

            /*string connectionString = "Host=localhost;Port=5432;Database=assignment_orm_3;Username=postgres;Password=admin1234567810";

            // Vytvoření DbContextOptions
            var optionsBuilder = new DbContextOptionsBuilder<AssignmentOrm3Context>();
            optionsBuilder.UseNpgsql(connectionString);*/

            // Vytvoření instance WebStoreContext s DbContextOptions
            using var context = new AssignmentOrm3Context();

            // Vytvoření instance LinqQueriesAssignment
            var assignments = new LinqQueriesAssignment(context);


            var Assigments = new LinqQueriesAssignment(context);

            await Assigments.Task01_ListAllCustomers();

            await Assigments.Task02_ListOrdersWithItemCount();

            await Assigments.Task03_ListProductsByDescendingPrice();

            await Assigments.Task04_ListPendingOrdersWithTotalPrice();

            await Assigments.Task05_OrderCountPerCustomer();

            await Assigments.Task06_Top3CustomersByOrderValue();

            await Assigments.Task07_RecentOrders();

            await Assigments.Task08_TotalSoldPerProduct();

            await Assigments.Task09_DiscountedOrders();

            await Assigments.Task10_AdvancedQueryExample();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
