using System;
using IEvangelist.GitHub.Repository.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace IEvangelist.GitHub.Repository
{
    public class CosmosContainerProvider : ICosmosContainerProvider, IDisposable
    {
        readonly RepositoryOptions _options;

        CosmosClient _client;
        Container _container;

        public CosmosContainerProvider(
            IOptions<RepositoryOptions> options) =>
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

        public Container GetContainer()
        {
            if (_container is null)
            {
                _client = new CosmosClient(_options.CosmosConnectionString);
                var database = _client.GetDatabase(_options.DatabaseId);
                _container = database.GetContainer(_options.ContainerId);
            }

            return _container;
        }

        public void Dispose() => _client?.Dispose();
    }
}