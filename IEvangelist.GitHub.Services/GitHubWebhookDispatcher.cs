using IEvangelist.GitHub.Services.Handlers;
using System.Threading.Tasks;

namespace IEvangelist.GitHub.Services
{
    public class GitHubWebhookDispatcher : IGitHubWebhookDispatcher
    {
        readonly IIssueHandler _issueHandler;
        readonly IPullRequestHandler _pullRequestHandler;

        public GitHubWebhookDispatcher(
            IIssueHandler issueHandler,
            IPullRequestHandler pullRequestHandler)
        {
            _issueHandler = issueHandler;
            _pullRequestHandler = pullRequestHandler;
        }

        public ValueTask DispatchAsync(string eventName, string payloadJson)
        {
            switch (eventName)
            {
                case "issues": return _issueHandler.HandleIssueAsync(payloadJson);
                case "pull_request": return _pullRequestHandler.HandlePullRequestAsync(payloadJson);

                default: return new ValueTask();
            }
        }
    }
}