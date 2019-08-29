using IEvangelist.GitHub.Services.Extensions;
using IEvangelist.GitHub.Services.Filters;
using IEvangelist.GitHub.Services.GraphQL;
using IEvangelist.GitHub.Services.Hanlders;
using Microsoft.Extensions.Logging;
using Octokit;
using Octokit.GraphQL.Model;
using System;
using System.Threading.Tasks;

namespace IEvangelist.GitHub.Services.Handlers
{
    public class PullRequestHandler : GitHubBaseHandler<PullRequestHandler>, IPullRequestHandler
    {
        readonly IProfanityFilter _profanityFilter;

        public PullRequestHandler(
            IGitHubGraphQLClient client,
            ILogger<PullRequestHandler> logger,
            IProfanityFilter profanityFilter)
            : base(client, logger) => _profanityFilter = profanityFilter;

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
                {   case "assigned":
                    case "unassigned":
                    case "review_requested":
                    case "review_request_removed":
                    case "labeled":
                    case "unlabeled":
                    case "opened":
                        await HandlePullRequestOpenedAsync(payload);
                        break;
                    case "edited":
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

        async ValueTask HandlePullRequestOpenedAsync(PullRequestEventPayload payload)
        {
            try
            {
                var pullRequest = payload.PullRequest;
                var clientId = Guid.NewGuid().ToString();

                // TODO: create a "profane label"
                //await _client.AddLabelAsync(issue.NodeId, new[] { "MDU6TGFiZWwxNDY5Mjc4NzMx" }, clientId);

                var (replaceTitle, replaceBody) =
                    (_profanityFilter.IsProfane(pullRequest.Title), _profanityFilter.IsProfane(pullRequest.Body));

                if (replaceTitle || replaceBody)
                {
                    var title = replaceTitle ? _profanityFilter.ApplyFilter(pullRequest.Title, '*') : pullRequest.Title;
                    var body = replaceBody ? _profanityFilter.ApplyFilter(pullRequest.Body) : pullRequest.Body;

                    _logger.LogInformation($"Replaced title: {title}");
                    _logger.LogInformation($"Replaced body: {body}");

                    //await _client.AddReactionAsync(issue.NodeId, ReactionContent.Confused, clientId);

                    var input = new UpdatePullRequestInput
                    {
                        PullRequestId = pullRequest.NodeId.ToGitHubId(),
                        Title = title,
                        Body = body,
                        ClientMutationId = clientId
                    };

                    //await _client.UpdateIssueAsync(input);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while attempting to filter issue: {ex.Message}\n{ex.StackTrace}", ex);
            }
        }
    }
}