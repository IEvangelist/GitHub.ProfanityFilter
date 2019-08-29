using IEvangelist.GitHub.Services.GraphQL;
using Microsoft.Extensions.Logging;

namespace IEvangelist.GitHub.Services.Hanlders
{
    public class GitHubBaseHandler<T>
    {
        protected readonly IGitHubGraphQLClient _client;
        protected readonly ILogger<T> _logger;

        public GitHubBaseHandler(IGitHubGraphQLClient client, ILogger<T> logger) =>
            (_client, _logger) = (client, logger);
    }
}