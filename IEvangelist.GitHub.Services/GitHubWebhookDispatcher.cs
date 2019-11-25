using IEvangelist.GitHub.Services.Handlers;
using System.Threading.Tasks;

namespace IEvangelist.GitHub.Services
{
    public class GitHubWebhookDispatcher : IGitHubWebhookDispatcher
    {
        static readonly ValueTask NoopTask = new ValueTask();

        readonly IIssueHandler _issueHandler;
        readonly IPullRequestHandler _pullRequestHandler;

        public GitHubWebhookDispatcher(
            IIssueHandler issueHandler,
            IPullRequestHandler pullRequestHandler) =>
            (_issueHandler, _pullRequestHandler) = (issueHandler, pullRequestHandler);

        public ValueTask DispatchAsync(string eventName, string payloadJson)
            => eventName switch
            {
                "issues" => _issueHandler.HandleIssueAsync(payloadJson),
                "pull_request" => _pullRequestHandler.HandlePullRequestAsync(payloadJson),

                _ => NoopTask,
            };
    }
}