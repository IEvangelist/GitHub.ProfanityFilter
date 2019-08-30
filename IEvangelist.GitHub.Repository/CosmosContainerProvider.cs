using System;
using IEvangelist.GitHub.Repository.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace IEvangelist.GitHub.Repository
{
    public class CosmosContainerProvider : ICosmosContainerProvider
    {
        readonly RepositoryOptions _options;

        public CosmosContainerProvider(
            IOptions<RepositoryOptions> options) =>
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

        public Container GetContainer()
        {
            using (var client = new CosmosClient(_options.CosmosConnectionString))
            {
                var database = client.GetDatabase(_options.DatabaseId);
                return database.GetContainer(_options.ContainerId);
            }
        }
    }
}