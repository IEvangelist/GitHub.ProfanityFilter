using IEvangelist.GitHub.Services.Enums;
using IEvangelist.GitHub.Services.Extensions;
using IEvangelist.GitHub.Services.Filters;
using IEvangelist.GitHub.Services.GraphQL;
using IEvangelist.GitHub.Services.Hanlders;
using IEvangelist.GitHub.Services.Models;
using IEvangelist.GitHub.Services.Options;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Octokit;
using Octokit.GraphQL.Model;
using System;
using System.Threading.Tasks;

namespace IEvangelist.GitHub.Services.Handlers
{
    public class IssueHandler
        : GitHubBaseHandler<IssueHandler>, IIssueHandler
    {
        readonly GitHubOptions _options;
        readonly IRepository<FilterActivity> _repository;

        public IssueHandler(
            IGitHubGraphQLClient client,
            ILogger<IssueHandler> logger,
            IOptions<GitHubOptions> options,
            IProfanityFilter profanityFilter,
            IRepository<FilterActivity> repository)
            : base(client, profanityFilter, logger) =>
            (_options, _repository) = (options.Value, repository);

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

                _logger.LogInformation(
                    $"Handling issue: {payload.Action}, {payload.Issue.NodeId}");

                switch (payload.Action)
                {
                    case "opened":
                        await HandleIssueAsync(payload);
                        break;

                    case "reopened":
                    case "edited":
                        var activity = 
                            await _repository.GetAsync(payload.Issue.NodeId);
                        if (activity?.WorkedOn
                                    .Subtract(DateTime.Now)
                                    .TotalSeconds <= 1)
                        {
                            _logger.LogInformation(
                                $"Just worked on this issue {payload.Issue.NodeId}...");
                        }

                        await HandleIssueAsync(payload, activity);
                        break;

                    case "closed":
                    case "deleted":
                        await _repository.DeleteAsync(payload.Issue.NodeId);
                        break;

                    case "assigned":
                    case "demilestoned":
                    case "labeled":
                    case "locked":
                    case "milestoned":
                    case "pinned":
                    case "transferred":
                    case "unassigned":
                    case "unlabeled":
                    case "unlocked":
                    case "unpinned":
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}\n{ex.StackTrace}", ex);
            }
        }

        async ValueTask HandleIssueAsync(
            IssueEventPayload payload,
            FilterActivity activity = null)
        {
            try
            {
                var issue = payload.Issue;
                var (title, body) = (issue.Title, issue.Body);
                var wasJustOpened = activity is null;
                if (!wasJustOpened)
                {
                    (title, body) = 
                        await _client.GetIssueTitleAndBodyAsync(issue.Number);
                }

                var filterResult = TryApplyProfanityFilter(title, body);
                if (filterResult.IsFiltered)
                {
                    var updateIssue = issue.ToUpdate();
                    updateIssue.Title = filterResult.Title;
                    updateIssue.Body = filterResult.Body;
                    await _client.UpdateIssueAsync(issue.Number, updateIssue);

                    var clientId = Guid.NewGuid().ToString();
                    if (wasJustOpened)
                    {
                        await _repository.CreateAsync(new FilterActivity
                        {
                            Id = issue.NodeId,
                            WasProfane = true,
                            ActivityType = ActivityType.Issue,
                            MutationOrNodeId = clientId,
                            WorkedOn = DateTime.Now,
                            OriginalTitleText = title,
                            OriginalBodyText = body,
                            ModifiedTitleText = filterResult.Title,
                            ModifiedBodyText = filterResult.Body
                        });
                    }
                    else
                    {
                        activity.WasProfane = true;
                        activity.WorkedOn = DateTime.Now;
                        await _repository.UpdateAsync(activity);
                    }

                    await _client.AddReactionAsync(
                        issue.NodeId,
                        ReactionContent.Confused,
                        clientId);
                    await _client.AddLabelAsync(
                        issue.NodeId,
                        new[] { _options.ProfaneLabelId },
                        clientId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"Error while attempting to filter issue: {ex.Message}\n{ex.StackTrace}", ex);
            }
        }
    }
}