using api.Core.Services;
using api.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace api.Endpoints;

public static class CustomerEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/customers");

        // CREATE
        group.MapPost("/", async (Customer newCustomer, CustomerService dbService) =>
        {
            var response = await dbService.CreateCustomerAsync(newCustomer);

            return Results.Created($"/api/customers/{response.Id}", response);
        });

        // GET CUSTOMER
        group.MapGet("/{id}", async (string id, CustomerService dbService) =>
        {
            var response = await dbService.GetCustomerAsync(id);

            return (response is { } customer)
                ? Results.Ok(customer)
                : Results.NotFound();
        });

        // GET CUSTOMERS
        group.MapGet("/", async ([FromQuery(Name = "search")] string? query, CustomerService dbService) =>
        {
            IEnumerable<Customer>? response;
            if (query is not null)
            {
                response = await dbService.SearchCustomerAsync(query);
            }
            else
            {
                response = await dbService.GetCustomersAsync();
            }

            return (response is { } customers)
                ? Results.Ok(customers)
                : Results.NotFound();
        });

        // UPDATE CUSTOMER
        group.MapPut("/{id}", async (Customer updatedCustomer, string id, CustomerService dbService) =>
        {
            var response = await dbService.UpdateCustomerAsync(updatedCustomer, id);

            return (response is { } updated)
                ? Results.Ok(updated)
                : Results.NotFound();
        });

        group.MapPatch("/{id}", async (CustomerPatchRequest patchedCustomer, string id, CustomerService dbService) =>
        {
            var response = await dbService.PatchCustomerAsync(patchedCustomer, id);

            return (response is { } patched)
                ? Results.Ok(patched)
                : Results.NotFound();
        });

        group.MapDelete("/{id}", async (string id, CustomerService dbService) =>
        {
            var response = await dbService.DeleteCustomerAsync(id);

            return response
                ? Results.NoContent()
                : Results.NotFound();
        });

        return endpoints;
    }
}