using IEvangelist.GitHub.Services.Extensions;
using IEvangelist.GitHub.Services.Filters;
using IEvangelist.GitHub.Services.GraphQL;
using IEvangelist.GitHub.Services.Hanlders;
using IEvangelist.GitHub.Services.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Octokit;
using Octokit.GraphQL.Model;
using System;
using System.Threading.Tasks;

namespace IEvangelist.GitHub.Services.Handlers
{
    public class PullRequestHandler : GitHubBaseHandler<PullRequestHandler>, IPullRequestHandler
    {
        readonly IProfanityFilter _profanityFilter;
        readonly GitHubOptions _options;

        public PullRequestHandler(
            IGitHubGraphQLClient client,
            ILogger<PullRequestHandler> logger,
            IOptions<GitHubOptions> options,
            IProfanityFilter profanityFilter)
            : base(client, logger) =>
            (_profanityFilter, _options) = (profanityFilter, options.Value);

        public async ValueTask HandlePullRequestAsync(string payloadJson)
        {
            try
            {
                var payload = payloadJson.FromJson<PullRequestEventPayload>();
                if (payload is null)
                {
                    _logger.LogWarning("GitHub pull request payload is null.");
                    return;
                }

                _logger.LogInformation($"Handling pull request: {payload.Action}, {payload.PullRequest.NodeId}");

                switch (payload.Action)
                {
                    case "assigned":
                    case "unassigned":
                    case "review_requested":
                    case "review_request_removed":
                    case "labeled":
                    case "unlabeled":
                        break;

                    case "opened":
                    case "edited":
                        await HandlePullRequestAsync(payload);
                        break;

                    case "closed":
                    case "ready_for_review":
                    case "locked":
                    case "unlocked":
                    case "reopened":
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        async ValueTask HandlePullRequestAsync(PullRequestEventPayload payload)
        {
            try
            {
                var pullRequest = payload.PullRequest;
                var clientId = Guid.NewGuid().ToString();

                var (replaceTitle, replaceBody) =
                    (_profanityFilter.IsProfane(pullRequest.Title), _profanityFilter.IsProfane(pullRequest.Body));

                if (replaceTitle || replaceBody)
                {
                    var title = replaceTitle ? _profanityFilter.ApplyFilter(pullRequest.Title, '*') : pullRequest.Title;
                    var body = replaceBody ? _profanityFilter.ApplyFilter(pullRequest.Body) : pullRequest.Body;

                    if (replaceTitle) _logger.LogInformation($"Replaced title: {title}");
                    if (replaceBody) _logger.LogInformation($"Replaced body: {body}");

                    await _client.UpdatePullRequestAsync(pullRequest.Number, new PullRequestUpdate
                    {
                        Title = title,
                        Body = body
                    });
                    await _client.AddReactionAsync(pullRequest.NodeId, ReactionContent.Confused, clientId);
                    await _client.AddLabelAsync(pullRequest.NodeId, new[] { _options.ProfaneLabelId }, clientId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while attempting to filter pull request: {ex.Message}\n{ex.StackTrace}", ex);
            }
        }
    }
}