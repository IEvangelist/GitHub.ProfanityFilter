using Octokit;
using Octokit.GraphQL.Model;
using System.Threading.Tasks;

namespace IEvangelist.GitHub.Services.GraphQL
{
    public interface IGitHubGraphQLClient
    {
        ValueTask<string> AddLabelAsync(string issueOrPullRequestId, string[] labelIds, string clientId);

        ValueTask<string> RemoveLabelAsync(string issueOrPullRequestId, string clientId);

        ValueTask<string> AddReactionAsync(string issueOrPullRequestId, ReactionContent reaction, string clientId);

        ValueTask<string> RemoveReactionAsync(string issueOrPullRequestId, ReactionContent reaction, string clientId);

        ValueTask<string> UpdateIssueAsync(UpdateIssueInput input);

        ValueTask<string> UpdateIssueAsync(int number, IssueUpdate input);

        ValueTask<string> UpdatePullRequestAsync(UpdatePullRequestInput input);

        ValueTask<string> UpdatePullRequestAsync(int number, PullRequestUpdate input);
    }
}