using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace ServiceBusFunctionDemo
{
    class StoreDataInCosmosDB
    {
        private static readonly string EndpointUri = "https://familyaccount.documents.azure.com:443/";

        private static readonly string PrimaryKey = "ZsNqUMiWyS0aLdjxYE4DCWaAmR2lwyXvQGQD9cGlgLNdbOHJaNiImpyrxOkCeBm0WYYkoFKhJVUwWwZP915iQA==";

        private CosmosClient cosmosClient;

        private Database database;
        private Container container;
        private string databaseId = "FamilyDatabase";
        private string containerId = "FamilyContainer";
        public async Task Main(Family familyResponse)
        {
            try
            {
                StoreDataInCosmosDB p = new StoreDataInCosmosDB();
                await p.GetStartedDemoAsync(familyResponse);

            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}", de.StatusCode, de);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();
            }
        }
        public async Task GetStartedDemoAsync(Family familyResponse)
        {
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
            await this.CreateDatabaseAsync();
            await this.CreateContainerAsync();
            await this.AddItemsToContainerAsync(familyResponse);
        }

        private async Task CreateDatabaseAsync()
        {

            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            Console.WriteLine("Created Database: {0}\n", this.database.Id);
        }
        private async Task CreateContainerAsync()
        {
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/Job");
            Console.WriteLine("Created Container: {0}\n", this.container.Id);
        }

        private async Task AddItemsToContainerAsync(Family familyResponse)
        {
            try
            {
                ItemResponse<Family> itemResponse = await this.container.ReadItemAsync<Family>(familyResponse.Id, new PartitionKey(familyResponse.Job));

            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                ItemResponse<Family> itemResponse = await this.container.CreateItemAsync<Family>(familyResponse, new PartitionKey(familyResponse.Job));


            }
        }

    }
}
