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
    public class IssueHandler : GitHubBaseHandler<IssueHandler>, IIssueHandler
    {
        readonly IProfanityFilter _profanityFilter;

        public IssueHandler(
            IGitHubGraphQLClient client,
            ILogger<IssueHandler> logger,
            IProfanityFilter profanityFilter)
            : base(client, logger) => _profanityFilter = profanityFilter;

        public async ValueTask HandleIssueAsync(string payloadJson)
        {
            try
            {
                var payload = payloadJson.FromJson<IssueEventPayload>();
                if (payload is null)
                {
                    _logger.LogWarning("GitHub issue payload is null.");
                    return;
                }

                _logger.LogInformation($"Handling issue: {payload.Action}, {payload.Issue.NodeId}");

                switch (payload.Action)
                {
                    case "opened":
                        await HandleIssueOpenedAsync(payload);
                        break;

                    case "edited":
                    case "deleted":
                    case "transferred":
                    case "pinned":
                    case "unpinned":
                    case "closed":
                    case "reopened":
                    case "assigned":
                    case "unassigned":
                    case "labeled":
                    case "unlabeled":
                    case "locked":
                    case "unlocked":
                    case "milestoned":
                    case "demilestoned":
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        async ValueTask HandleIssueOpenedAsync(IssueEventPayload payload)
        {
            var issue = payload.Issue;

            try
            {
                var clientId = Guid.NewGuid().ToString();
                //await _client.AddLabelAsync(issue.NodeId, new[] { "MDU6TGFiZWwxNDY5Mjc4NzMx" }, clientId);

                var (replaceTitle, replaceBody) =
                    (_profanityFilter.IsProfane(issue.Title), _profanityFilter.IsProfane(issue.Body));

                if (replaceTitle || replaceBody)
                {
                    var title = replaceTitle ? _profanityFilter.ApplyFilter(issue.Title, '*') : issue.Title;
                    var body = replaceBody ? _profanityFilter.ApplyFilter(issue.Body) : issue.Body;

                    _logger.LogInformation($"Replaced title: {title}");
                    _logger.LogInformation($"Replaced body: {body}");

                    //await _client.AddReactionAsync(issue.NodeId, ReactionContent.Confused, clientId);

                    var input = new UpdateIssueInput
                    {
                        Id = issue.NodeId.ToGitHubId(),
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