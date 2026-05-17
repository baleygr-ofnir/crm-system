using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;

namespace azfunc.Functions;


public class CustomerChangeNotifier
{
    private readonly IConfiguration _configuration;
    private readonly string _databaseName;
    private readonly string _containerName;

    public CustomerChangeNotifier(IConfiguration configuration)
    {
        _configuration = configuration;
        _databaseName = configuration["CosmosDb:DatabaseName"]
    }

    [Function($"CustomerChangeNotifier")]
    public async Task Run(
        [CosmosDBTrigger(
            dat
            )])
}