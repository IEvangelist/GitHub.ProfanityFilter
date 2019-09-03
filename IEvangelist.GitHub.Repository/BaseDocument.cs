using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace IEvangelist.GitHub.Repository
{
    public class BaseDocument
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        internal PartitionKey PartitionKey => new PartitionKey(Id);
    }
}