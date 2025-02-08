using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Blazor.AzureCosmosDb.Demo.Data;

public class EngineerService : IEngineerService
{
    private readonly CosmosDbSettings _cosmosDbSettings;

    public EngineerService(IOptions<CosmosDbSettings> cosmosDbSettings)
    {
        _cosmosDbSettings = cosmosDbSettings.Value;
    }

    private Container GetContainerClient()
    {
        var cosmosDbClient = new CosmosClient(_cosmosDbSettings.ConnectionString);
        var container = cosmosDbClient.GetContainer(_cosmosDbSettings.DatabaseName, _cosmosDbSettings.ContainerName);
        return container;
    }

    public async Task UpsertEngineer(Engineer engineer)
    {
        try
        {
            if (engineer.id == null)
            {
                engineer.id = Guid.NewGuid();
            }
            var container = GetContainerClient();
            var updateRes = await container.UpsertItemAsync(engineer, new PartitionKey(engineer.id.ToString()));
            Console.Write(updateRes.StatusCode);
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Console.WriteLine($"Resource not found: {ex.Message}");
            // Handle the case where the resource was not found
        }
        catch (CosmosException ex)
        {
            Console.WriteLine($"CosmosException: {ex.Message}");
            // Handle other Cosmos DB exceptions
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            // Handle general exceptions
        }
    }

    public async Task DeleteEngineer(string? id, string? partitionKey)
    {
        try
        {
            var container = GetContainerClient();
            var deleteRes = await container.DeleteItemAsync<Engineer>(id, new PartitionKey(partitionKey));
            Console.Write(deleteRes.StatusCode);
        }
        catch (Exception ex)
        {
            throw new Exception("Exception", ex);
        }
    }

    public async Task<List<Engineer>> GetEngineerDetails()
    {
        List<Engineer> engineers = new List<Engineer>();
        try
        {
            var container = GetContainerClient();
            var sqlQuery = "SELECT * FROM c";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQuery);
            FeedIterator<Engineer> queryResultSetIterator = container.GetItemQueryIterator<Engineer>(queryDefinition);

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Engineer> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (Engineer engineer in currentResultSet)
                {
                    engineers.Add(engineer);
                }
            }
        }
        catch (Exception ex)
        {
            ex.Message.ToString();
        }
        return engineers;
    }

    public async Task<Engineer> GetEngineerDetailsById(string? id, string? partitionKey)
    {
        try
        {
            var container = GetContainerClient();
            ItemResponse<Engineer> response = await container.ReadItemAsync<Engineer>(id, new PartitionKey(partitionKey));
            return response.Resource;
        }
        catch (Exception ex)
        {
            throw new Exception("Exception", ex);
        }
    }
}