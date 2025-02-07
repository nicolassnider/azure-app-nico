﻿namespace Blazor.AzureCosmosDb.Demo.Data;

public class CosmosDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string AccountPrimaryKey { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
}
