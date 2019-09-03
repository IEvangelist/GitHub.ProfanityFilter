using IEvangelist.GitHub.Repository;
using IEvangelist.GitHub.Services.Enums;
using IEvangelist.GitHub.Services.Extensions;
using IEvangelist.GitHub.Services.Filters;
using IEvangelist.GitHub.Services.GraphQL;
using IEvangelist.GitHub.Services.Hanlders;
using IEvangelist.GitHub.Services.Models;
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
        readonly GitHubOptions _options;
        readonly IProfanityFilter _profanityFilter;
        readonly IRepository<FilterActivity> _repository;

        public IssueHandler(
            IGitHubGraphQLClient client,
            ILogger<IssueHandler> logger,
            IOptions<GitHubOptions> options,
            IProfanityFilter profanityFilter,
            IRepository<FilterActivity> repository)
            : base(client, logger) =>
            (_profanityFilter, _options, _repository) = (profanityFilter, options.Value, repository);

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
                        await HandleIssueAsync(payload, null);
                        break;

                    case "edited":
                        var activity = await _repository.GetAsync(payload.Issue.NodeId);
                        if (activity?.WorkedOn.Subtract(DateTime.Now).TotalSeconds <= 1)
                        {
                            _logger.LogInformation("We just worked on this item...");
                        }

                        await HandleIssueAsync(payload, activity);
                        break;

                    case "deleted":
                        await _repository.DeleteAsync(payload.Issue.NodeId);
                        break;

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
                _logger.LogError($"{ex.Message}\n{ex.StackTrace}", ex);
            }
        }

        async ValueTask HandleIssueAsync(IssueEventPayload payload, FilterActivity activity = null)
        {
            try
            {
                var issue = payload.Issue;
                var filterRresult = HandleFiltering(issue.Title, issue.Body, _profanityFilter);
                if (filterRresult.IsFiltered)
                {
                    var updateIssue = issue.ToUpdate();
                    updateIssue.Title = filterRresult.Title;
                    updateIssue.Body = filterRresult.Body;
                    await _client.UpdateIssueAsync(issue.Number, updateIssue);

                    var clientId = Guid.NewGuid().ToString();
                    if (activity is null)
                    {
                        await _repository.CreateAsync(new FilterActivity
                        {
                            Id = issue.NodeId,
                            WasProfane = true,
                            Type = ActivityType.Issue,
                            MutationOrNodeId = clientId,
                            WorkedOn = DateTime.Now
                        });
                    }
                    else
                    {
                        activity.WasProfane = true;
                        activity.WorkedOn = DateTime.Now;
                        await _repository.UpdateAsync(activity);
                    }

                    await _client.AddReactionAsync(issue.NodeId, ReactionContent.Confused, clientId);
                    await _client.AddLabelAsync(issue.NodeId, new[] { _options.ProfaneLabelId }, clientId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while attempting to filter issue: {ex.Message}\n{ex.StackTrace}", ex);
            }
        }
    }
}