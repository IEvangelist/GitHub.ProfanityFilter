using IEvangelist.GitHub.Services.Filters;
using IEvangelist.GitHub.Services.GraphQL;
using IEvangelist.GitHub.Services.Models;
using Microsoft.Extensions.Logging;

namespace IEvangelist.GitHub.Services.Hanlders
{
    public class GitHubBaseHandler<T>
    {
        protected readonly IGitHubGraphQLClient _client;
        protected readonly IProfanityFilter _profanityFilter;
        protected readonly ILogger<T> _logger;

        public GitHubBaseHandler(
            IGitHubGraphQLClient client,
            IProfanityFilter profanityFilter,
            ILogger<T> logger) =>
            (_client, _profanityFilter, _logger) = (client, profanityFilter, logger);

        internal FilterResult TryApplyProfanityFilter(
            string title,
            string body)
        {
            if (string.IsNullOrWhiteSpace(title) &&
                string.IsNullOrWhiteSpace(body))
            {
                return FilterResult.NotFiltered;
            }

            var (resultingTitle, isTitleFiltered) = TryApplyFilter(title, '*');
            var (resultingBody, isBodyFiltered) = TryApplyFilter(body);

            return new FilterResult(
                resultingTitle,
                isTitleFiltered,
                resultingBody,
                isBodyFiltered);
        }

        (string text, bool isFiltered) TryApplyFilter(
            string text,
            char? placeHolder = null)
        {
            var filterText = _profanityFilter?.IsProfane(text) ?? false;
            var resultingText =
                filterText
                    ? _profanityFilter?.ApplyFilter(text, placeHolder)
                    : text;

            if (filterText)
            {
                _logger.LogInformation($"Replaced text: {resultingText}");
            }

            return (resultingText, filterText);
        }
    }
}