using System.Linq.Expressions;
using Microsoft.Azure.Cosmos;
using api.Data.Entities;
using Microsoft.Azure.Cosmos.Linq;

namespace api.Core.Services;

public class CustomerService
{
    private readonly Container _container;
    
    public CustomerService(Container container)
    {
        _container = container;
    }
    
    public async Task<Customer> CreateCustomerAsync(Customer newCustomer)
    {
        var response = await _container.CreateItemAsync(
            newCustomer,
            new PartitionKey(newCustomer.Id));
        return response.Resource;
    }

    public async Task<Customer> GetCustomerAsync(string id)
    {
        var response = await _container.ReadItemAsync<Customer>(
            id,
            new PartitionKey(id));
        return response.Resource;
    }

    public async Task<IEnumerable<Customer>> GetCustomersAsync()
    {
        var queryable = _container.GetItemLinqQueryable<Customer>();

        using var iterator = queryable.ToFeedIterator();

        var results = new List<Customer>();

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task<IEnumerable<Customer>> SearchCustomerAsync(string query)
    {
        var queryable = _container.GetItemLinqQueryable<Customer>()
            .Where(c => c.Name.Contains(query)
             || c.SalesRep.Name.Contains(query));

        using var iterator = queryable.ToFeedIterator();
        var results = new List<Customer>();

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task<Customer?> UpdateCustomerAsync(Customer updatedCustomer, string id)
    {
        try
        {
            var response = await _container.ReplaceItemAsync(
                updatedCustomer,
                id,
                new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<bool> DeleteCustomerAsync(string id)
    {
        try
        {
            await _container.DeleteItemAsync<Customer>(
                id,
                new PartitionKey(id));
            return true;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}