using Microsoft.Azure.Cosmos;

namespace IEvangelist.GitHub.Repository
{
    public interface ICosmosContainerProvider
    {
        Container GetContainer();
    }
}