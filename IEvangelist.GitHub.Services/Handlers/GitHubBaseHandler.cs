using IEvangelist.GitHub.Services.Filters;
using IEvangelist.GitHub.Services.GraphQL;
using IEvangelist.GitHub.Services.Models;
using Microsoft.Extensions.Logging;

namespace IEvangelist.GitHub.Services.Hanlders
{
    public class GitHubBaseHandler<T>
    {
        protected readonly IGitHubGraphQLClient _client;
        protected readonly ILogger<T> _logger;

        public GitHubBaseHandler(IGitHubGraphQLClient client, ILogger<T> logger) =>
            (_client, _logger) = (client, logger);

        internal FilterResult HandleFiltering(string title, string body, IProfanityFilter filter)
        {
            if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(body))
            {
                return FilterResult.NotFiltered;
            }

            var filterTitle = filter?.IsProfane(title) ?? false;
            var filterBody = filter?.IsProfane(body) ?? false;

            var resultingTitle = filterTitle ? filter?.ApplyFilter(title, '*') : title;
            var resultingBody = filterBody ? filter?.ApplyFilter(body) : body;

            if (filterTitle)
            {
                _logger.LogInformation($"Replaced title: {resultingTitle}");
            }
            if (filterBody)
            {
                _logger.LogInformation($"Replaced body: {resultingBody}");
            }

            return new FilterResult
            {
                Title = resultingTitle,
                IsTitleFiltered = filterTitle,
                Body = resultingBody,
                IsBodyFiltered = filterBody
            };
        }
    }
}