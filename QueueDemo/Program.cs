using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Threading.Tasks;

namespace QueueDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var keyVaultName = Environment.GetEnvironmentVariable("KEY_VAULT_NAME");
            var kvUri = $"https://{keyVaultName}.vault.azure.net";

            var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());

            var queueNameSecret = await client.GetSecretAsync("QueueName");
            var queueName = queueNameSecret.Value.Value;

            var storageConnectionStringSecret = await client.GetSecretAsync("StorageConnectionString");
            var storageConnectionString = storageConnectionStringSecret.Value.Value;

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(queueName);

            Console.WriteLine("Checking queue...");
            await queue.CreateIfNotExistsAsync();

            Console.WriteLine("Sending message...");
            CloudQueueMessage message = new("Hello world!");
            await queue.AddMessageAsync(message);

            Console.WriteLine("Reading Message...");
            CloudQueueMessage myMessage = await queue.GetMessageAsync();
            Console.WriteLine("Message: {0}", myMessage.AsString);
        }
    }
}
