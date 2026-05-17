using api.Core.Services;
using api.Data.Entities;
using Microsoft.Azure.Cosmos;

namespace api.Data;

public static class DatabaseSeeder
{
    public static async Task SeedCustomersAsync(CustomerService customerService)
    {
        var seedCustomers = new List<Customer>
        {
            new Customer
            {
                Name = "Joakim Larsson",
                Title = "Biodlare",
                Email = "joakim.larsson@biodling.se",
                Phone = "076-2153522",
                Address = "Testvägen 245",
                SalesRep = new SalesRep
                {
                    Name = "Báleygr Järngrimr",
                    Email = "baleygr@example.com",
                    Phone = "073-2455067"
                }
            },
            new Customer
            {
                Name = "Anna Svensson",
                Title = "Verkställande Direktör",
                Email = "anna.svensson@foretag.se",
                Phone = "070-1234567",
                Address = "Storgatan 10",
                SalesRep = new SalesRep
                {
                    Name = "Báleygr Järngrimr",
                    Email = "baleygr@example.com",
                    Phone = "073-2455067"
                }
            },
            new Customer
            {
                Name = "Erik Lund",
                Title = "IT-Konsult",
                Email = "erik.lund@techsol.se",
                Phone = "072-9876543",
                Address = "Teknikgatan 42",
                SalesRep = new SalesRep
                {
                    Name = "Báleygr Järngrimr",
                    Email = "baleygr@example.com",
                    Phone = "073-2455067"
                }
            },
            new Customer
            {
                Name = "Maria Nilsson",
                Title = "Marknadschef",
                Email = "maria.nilsson@reklam.se",
                Phone = "073-5554433",
                Address = "Sveavägen 8",
                SalesRep = new SalesRep
                {
                    Name = "Báleygr Järngrimr",
                    Email = "baleygr@example.com",
                    Phone = "073-2455067"
                }
            },
            new Customer
            {
                Name = "Lars Olofsson",
                Title = "Lagerarbetare",
                Email = "lars.o@logistik.se",
                Phone = "076-1122334",
                Address = "Industrivägen 1",
                SalesRep = new SalesRep
                {
                    Name = "Báleygr Järngrimr",
                    Email = "baleygr@example.com",
                    Phone = "073-2455067"
                }
            }
        };

        foreach (var seedCustomer in seedCustomers)
        {
            await customerService.CreateCustomerAsync(seedCustomer);
        }
    }
}