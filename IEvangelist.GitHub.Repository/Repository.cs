using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IEvangelist.GitHub.Repository
{
    public class Repository<T> : IRepository<T>
    {
        readonly ICosmosContainerProvider _containerProvider;

        public Repository(
            ICosmosContainerProvider containerProvider) =>
            _containerProvider = containerProvider ?? throw new ArgumentNullException(nameof(containerProvider));

        public async ValueTask<T> GetAsync(string id)
        {
            var container = _containerProvider.GetContainer();
            var response = await container.ReadItemAsync<T>(id, PartitionKey.None);

            return response.Resource;
        }

        public async ValueTask<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            var iterator =
                _containerProvider.GetContainer()
                                  .GetItemLinqQueryable<T>()
                                  .Where(predicate)
                                  .ToFeedIterator();

            IList<T> results = new List<T>();
            while (iterator.HasMoreResults)
            {
                foreach (var result in await iterator.ReadNextAsync())
                {
                    results.Add(result);
                }
            }

            return results;
        }

        public async ValueTask<T> CreateAsync(T value)
        {
            var contaier = _containerProvider.GetContainer();
            var response = await contaier.CreateItemAsync(value);

            return response.Resource;
        }

        public Task<T[]> CreateAsync(IEnumerable<T> values) =>
            Task.WhenAll(values.Select(v => CreateAsync(v).AsTask()));

        public async ValueTask<T> UpdateAsync(T value)
        {
            var container = _containerProvider.GetContainer();
            var response = await container.UpsertItemAsync<T>(value);

            return response.Resource;
        }

        public async ValueTask<T> DeleteAsync(string id)
        {
            var container = _containerProvider.GetContainer();
            var response = await container.DeleteItemAsync<T>(id, PartitionKey.None);

            return response.Resource;
        }
    }
}