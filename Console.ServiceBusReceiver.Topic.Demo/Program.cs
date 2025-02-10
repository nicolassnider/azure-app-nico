using Azure.Messaging.ServiceBus;

const string serviceBusConnectionString = "<connection-string>";
const string topicName = "<topic-name>";
const string sub1Name = "<sb1-name>";

ServiceBusClient client = new ServiceBusClient(serviceBusConnectionString);
ServiceBusProcessor processor = default;

async Task MessageHandler(ProcessMessageEventArgs processMessageEventArgs)
{
    string body = processMessageEventArgs.Message.Body.ToString();
    Console.WriteLine($"Received: {body} - subscription: {sub1Name}");
    await processMessageEventArgs.CompleteMessageAsync(processMessageEventArgs.Message);
}

Task ErrorHandler(ProcessErrorEventArgs processErrorEventArgs)
{
    Console.WriteLine($"An error occurred. Error: {processErrorEventArgs.Exception.Message}");
    return Task.CompletedTask;
}

client = new ServiceBusClient(serviceBusConnectionString);
processor = client.CreateProcessor(topicName, sub1Name, new ServiceBusProcessorOptions());

try
{
    processor.ProcessMessageAsync += MessageHandler;
    processor.ProcessErrorAsync += ErrorHandler;

    await processor.StartProcessingAsync();
    Console.WriteLine("Press any key to end processing");
    Console.ReadKey();

    await processor.StopProcessingAsync();
}
catch (Exception ex)
{

    throw;
}
finally
{
    await processor.DisposeAsync();
    await client.DisposeAsync();
}