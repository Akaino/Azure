using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TeamsWebhook.Models;

namespace TeamsWebhook {

    class CosmosHelper {
        private readonly IConfiguration Configuration;
        private CosmosClient cosmosClient;
        private Database database;
        private Container container;
        public CosmosHelper(IConfiguration config) {
            this.Configuration = config;
            var options = new CosmosClientOptions() { ConnectionMode = ConnectionMode.Gateway };
            this.cosmosClient = new CosmosClient(this.Configuration["cosmosUri"], this.Configuration["cosmosKey"], options);
            this.database = cosmosClient.GetDatabase(Configuration["cosmosDatabase"]);
            this.container = cosmosClient.GetContainer(database.Id, Configuration["cosmosContainer"]);
        }

        // Cosmos
    public async Task AddItemToContainerAsync(string httpResult)
        {
            var CallRecordModel = JsonConvert.DeserializeObject<CallRecordModel>(httpResult);
            //var requestoptions = new ItemRequestOptions();
            //var callRecordModelResponse = await this.container.ReadItemAsync<ResponseMessage>(CallRecordModel.Id, new PartitionKey(CallRecordModel.Id));
            try
            {
                var item = await this.container.UpsertItemAsync(CallRecordModel, new PartitionKey(CallRecordModel.Id));
                //Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", item.Resource.Id, item.RequestCharge);
            }
            catch (System.Exception)
            {
                Console.WriteLine("Something went wrong");
            }
            /*
            if (callRecordModelResponse.StatusCode == HttpStatusCode.NotFound)
            {
                // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
                try
                {
                    var item = await this.container.CreateItemAsync(CallRecordModel, new PartitionKey(CallRecordModel.Id));
                    Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", item.Resource.Id, item.RequestCharge);
                
                }
                catch (System.Exception)
                {
                    Console.WriteLine("Item already exists. Probably...");
                    ItemResponse<CallRecordModel> item = await this.container.ReplaceItemAsync<CallRecordModel>(CallRecordModel, CallRecordModel.Id);
                }
                // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                
            }
            else
            {
                ItemResponse<CallRecordModel> item = await this.container.ReplaceItemAsync<CallRecordModel>(CallRecordModel, CallRecordModel.Id);
                //ItemResponse<CallRecordModel> item = await this.container.ReadItemAsync<CallRecordModel>(CallRecordModel.Id, new PartitionKey(CallRecordModel.Id));
                Console.WriteLine("Item in database with id: {0} already exists\n", item.Resource.Id, ". Upserted item.");
            }
            */
        } // AddItemToContainerAsync(string httpResult)
    } // class
} // namespace