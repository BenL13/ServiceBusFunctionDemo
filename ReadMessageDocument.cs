using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
namespace ServiceBusFunctionDemo
{
    public static class ReadMessageDocument
    {
        [FunctionName("ReadMessageDocument")]
        public static async Task Run([ServiceBusTrigger("myqueue")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
            // parse the message and assign to class objects

            var response = JsonConvert.DeserializeObject<Family>(myQueueItem);
            StoreDataInCosmosDB sd = new StoreDataInCosmosDB();
            await sd.Main(response);
        }
    }
}
