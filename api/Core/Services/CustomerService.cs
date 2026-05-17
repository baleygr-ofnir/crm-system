using System.Globalization;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using Microsoft.Azure.Cosmos;
using api.Data.Entities;
using azfunc.Helpers;
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
        var customerNormalizedName = StringHelper.TextNormalizer(newCustomer.Name);
        var salesRepNormalizedName = StringHelper.TextNormalizer(newCustomer.SalesRep.Name);
        var salesRepToCreate = newCustomer.SalesRep with
        {
            NormalizedName = salesRepNormalizedName
        };
        var customerToCreate = newCustomer with
        {
            NormalizedName = customerNormalizedName,
            SalesRep = salesRepToCreate
        };
        var response = await _container.CreateItemAsync(
            customerToCreate,
            new PartitionKey(customerToCreate.Id));
        return response.Resource;
    }

    public async Task<Customer?> GetCustomerAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<Customer>(
                id,
                new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IEnumerable<Customer>?> GetCustomersAsync()
    {
        try
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
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IEnumerable<Customer>?> SearchCustomerAsync(string query)
    {
        var queryable = _container.GetItemLinqQueryable<Customer>();
        var normalizedQuery = StringHelper.TextNormalizer(query);
        var matches = queryable
            .Where(c =>
                c.NormalizedName.ToLower().Contains(normalizedQuery)
                || c.SalesRep.NormalizedName.ToLower().Contains(normalizedQuery));

        using var iterator = matches.ToFeedIterator();
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
        var salesRepNormalizedName = StringHelper.TextNormalizer(updatedCustomer.SalesRep.Name);
        var salesRepToUpdate = updatedCustomer.SalesRep with
        {
            NormalizedName = salesRepNormalizedName
        };
        var customerNormalizedName = StringHelper.TextNormalizer(updatedCustomer.Name);
        var customerToUpdate = updatedCustomer with
        {
            Id = id,
            NormalizedName = customerNormalizedName,
            SalesRep = salesRepToUpdate
        };
        try
        {
            var response = await _container.ReplaceItemAsync(
                customerToUpdate,
                id,
                new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<Customer?> PatchCustomerAsync(CustomerPatchRequest patchedCustomer, string id)
    {
        List<PatchOperation> operations = new();

        if (patchedCustomer.Name is not null)
        {
            operations.Add(PatchOperation.Replace("/name", patchedCustomer.Name));

            var normalizedName = StringHelper.TextNormalizer(patchedCustomer.Name);
            operations.Add(PatchOperation.Replace("/normalizedName", normalizedName));
        }

        if (patchedCustomer.Title is not null)
            operations.Add(PatchOperation.Replace("/title", patchedCustomer.Title));

        if (patchedCustomer.Email is not null)
            operations.Add(PatchOperation.Replace("/email", patchedCustomer.Email));

        if (patchedCustomer.Phone is not null)
            operations.Add(PatchOperation.Replace("/phone", patchedCustomer.Phone));

        if (patchedCustomer.Address is not null)
            operations.Add(PatchOperation.Replace("/address", patchedCustomer.Address));

        if (patchedCustomer.SalesRep is not null)
        {
            var normalizedName = StringHelper.TextNormalizer(patchedCustomer.SalesRep.Name);
            var salesRepToPatch = patchedCustomer.SalesRep with { NormalizedName = normalizedName };
            operations.Add(PatchOperation.Replace("/salesRep", salesRepToPatch));
        }

        try
        {
            var response = await _container.PatchItemAsync<Customer>(
                id,
                new PartitionKey(id),
                operations
            );

            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
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
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}