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
    public class PullRequestHandler : GitHubBaseHandler<PullRequestHandler>, IPullRequestHandler
    {
        readonly IProfanityFilter _profanityFilter;
        readonly GitHubOptions _options;
        readonly IRepository<FilterActivity> _repository;

        public PullRequestHandler(
            IGitHubGraphQLClient client,
            ILogger<PullRequestHandler> logger,
            IOptions<GitHubOptions> options,
            IProfanityFilter profanityFilter,
            IRepository<FilterActivity> repository)
            : base(client, logger) =>
            (_profanityFilter, _options, _repository) = (profanityFilter, options.Value, repository);

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
                    case "opened":
                        await HandlePullRequestAsync(payload);
                        break;

                    case "reopened":
                    case "edited":
                        var activity = await _repository.GetAsync(payload.PullRequest.NodeId);
                        if (activity?.WorkedOn.Subtract(DateTime.Now).TotalSeconds <= 1)
                        {
                            _logger.LogInformation($"Just worked on this pull request {payload.PullRequest.NodeId}...");
                        }

                        await HandlePullRequestAsync(payload, activity);
                        break;

                    case "closed":
                        await _repository.DeleteAsync(payload.PullRequest.NodeId);
                        break;

                    case "assigned":
                    case "labeled":
                    case "locked":
                    case "ready_for_review":
                    case "review_request_removed":
                    case "review_requested":
                    case "unassigned":
                    case "unlabeled":
                    case "unlocked":
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        async ValueTask HandlePullRequestAsync(PullRequestEventPayload payload, FilterActivity activity = null)
        {
            try
            {
                var pullRequest = payload.PullRequest;
                var (title, body) = (pullRequest.Title, pullRequest.Body);
                var wasJustOpened = activity is null;
                if (!wasJustOpened)
                {
                    (title, body) = await _client.GetPullRequestTitleAndBodyAsync(pullRequest.Number);
                }

                var filterResult = ApplyProfanityFilter(title, body, _profanityFilter);
                if (filterResult.IsFiltered)
                {
                    await _client.UpdatePullRequestAsync(pullRequest.Number, new PullRequestUpdate
                    {
                        Title = title,
                        Body = body
                    });

                    var clientId = Guid.NewGuid().ToString();
                    if (wasJustOpened)
                    {
                        await _repository.CreateAsync(new FilterActivity
                        {
                            Id = pullRequest.NodeId,
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

                    await _client.AddReactionAsync(pullRequest.NodeId, ReactionContent.Confused, clientId);
                    await _client.AddLabelAsync(pullRequest.NodeId, new[] { _options.ProfaneLabelId }, clientId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while attempting to filter issue: {ex.Message}\n{ex.StackTrace}", ex);
            }
        }
    }
}