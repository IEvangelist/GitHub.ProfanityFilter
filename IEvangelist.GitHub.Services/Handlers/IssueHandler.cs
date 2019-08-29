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
    public class IssueHandler : GitHubBaseHandler<IssueHandler>, IIssueHandler
    {
        readonly IProfanityFilter _profanityFilter;
        readonly GitHubOptions _options;

        public IssueHandler(
            IGitHubGraphQLClient client,
            ILogger<IssueHandler> logger,
            IOptions<GitHubOptions> options,
            IProfanityFilter profanityFilter)
            : base(client, logger) =>
            (_profanityFilter, _options) = (profanityFilter, options.Value);

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
                    case "edited":
                        await HandleIssueAsync(payload);
                        break;

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

        async ValueTask HandleIssueAsync(IssueEventPayload payload)
        {
            try
            {
                var issue = payload.Issue;
                var clientId = Guid.NewGuid().ToString();

                var (replaceTitle, replaceBody) =
                    (_profanityFilter.IsProfane(issue.Title), _profanityFilter.IsProfane(issue.Body));

                if (replaceTitle || replaceBody)
                {
                    var title = replaceTitle ? _profanityFilter.ApplyFilter(issue.Title, '*') : issue.Title;
                    var body = replaceBody ? _profanityFilter.ApplyFilter(issue.Body) : issue.Body;

                    _logger.LogInformation($"Replaced title: {title}");
                    _logger.LogInformation($"Replaced body: {body}");

                    await _client.AddReactionAsync(issue.NodeId, ReactionContent.Confused, clientId);
                    await _client.AddLabelAsync(issue.NodeId, new[] { _options.ProfaneLabelId }, clientId);
                    await _client.UpdateIssueAsync(new UpdateIssueInput
                    {
                        Id = issue.NodeId.ToGitHubId(),
                        Title = title,
                        Body = body,
                        ClientMutationId = clientId
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while attempting to filter issue: {ex.Message}\n{ex.StackTrace}", ex);
            }
        }
    }
}