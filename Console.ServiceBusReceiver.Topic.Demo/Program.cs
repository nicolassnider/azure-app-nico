using Azure.Messaging.ServiceBus;

const string serviceBusConnectionString = "<connection-string>";
const string topicName = "<topic-name>";
const int maxNumberOfMessages = 3;

ServiceBusClient client = new ServiceBusClient(serviceBusConnectionString);
ServiceBusSender sender = client.CreateSender(topicName);

using ServiceBusMessageBatch batch = await sender.CreateMessageBatchAsync();

for (int i = 1; i < maxNumberOfMessages; i++)
{
    if (!batch.TryAddMessage(new ServiceBusMessage($"This is message {i}")))
    {
        Console.WriteLine($"Message {i} not addeed to the list");
    }
}

try
{
    await sender.SendMessagesAsync(batch);
    Console.WriteLine("Messages sent successfully");

}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
    throw;
}
finally
{
    await sender.DisposeAsync();
    await client.DisposeAsync();
}