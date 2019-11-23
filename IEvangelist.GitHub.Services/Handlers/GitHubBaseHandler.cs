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

        internal FilterResult ApplyProfanityFilter(
            string title,
            string body,
            IProfanityFilter filter)
        {
            if (string.IsNullOrWhiteSpace(title) &&
                string.IsNullOrWhiteSpace(body))
            {
                return FilterResult.NotFiltered;
            }

            var (resultingTitle, isTitleFiltered) = ApplyFilter(title, filter, _logger, '*');
            var (resultingBody, isBodyFiltered) = ApplyFilter(body, filter, _logger);

            return new FilterResult(
                resultingTitle,
                isTitleFiltered,
                resultingBody,
                isBodyFiltered);
        }

        static (string text, bool isFiltered) ApplyFilter(
            string text, 
            IProfanityFilter filter, 
            ILogger logger, 
            char? placeHolder = null)
        {
            var filterText = filter?.IsProfane(text) ?? false;
            var resultingText = filterText ? filter?.ApplyFilter(text, placeHolder) : text;

            if (filterText)
            {
                logger.LogInformation($"Replaced text: {resultingText}");
            }

            return (resultingText, filterText);
        }
    }
}