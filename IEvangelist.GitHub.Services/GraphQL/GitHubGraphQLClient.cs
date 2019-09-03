using IEvangelist.GitHub.Services.Extensions;
using IEvangelist.GitHub.Services.Options;
using Microsoft.Extensions.Options;
using Octokit;
using Octokit.GraphQL;
using Octokit.GraphQL.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

using IGraphQLConnection = Octokit.GraphQL.IConnection;
using GraphQLConnection = Octokit.GraphQL.Connection;
using GraphQLProductHeaderValue = Octokit.GraphQL.ProductHeaderValue;

using Connection = Octokit.Connection;
using ProductHeaderValue = Octokit.ProductHeaderValue;

namespace IEvangelist.GitHub.Services.GraphQL
{
    public class GitHubGraphQLClient : IGitHubGraphQLClient
    {
        const string ProductID = "GitHub.ProfanityFilter";
        const string ProductVersion = "1.0";

        readonly IGraphQLConnection _connection;
        readonly IGitHubClient _client;
        readonly GitHubOptions _config;

        public GitHubGraphQLClient(IOptions<GitHubOptions> config)
        {
            _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
            _connection = new GraphQLConnection(new GraphQLProductHeaderValue(ProductID, ProductVersion), _config.ApiToken);
            _client = new GitHubClient(new Connection(new ProductHeaderValue(ProductID, ProductVersion))
            {
                Credentials = new Credentials(_config.ApiToken)
            });
        }

        public async ValueTask<string> AddLabelAsync(string issueOrPullRequestId, string[] labelIds, string clientId)
        {
            var mutation =
                new Mutation()
                    .AddLabelsToLabelable(new AddLabelsToLabelableInput
                    {
                        ClientMutationId = clientId,
                        LabelableId = issueOrPullRequestId.ToGitHubId(),
                        LabelIds = labelIds.Select(id => id.ToGitHubId()).ToArray()
                    })
                    .Select(payload => new
                    {
                        payload.ClientMutationId
                    })
                    .Compile();

            var result = await _connection.Run(mutation);
            return result.ClientMutationId;
        }

        public async ValueTask<string> AddReactionAsync(string issueOrPullRequestId, ReactionContent reaction, string clientId)
        {
            var mutation =
                new Mutation()
                    .AddReaction(new AddReactionInput
                    {
                        ClientMutationId = clientId,
                        SubjectId = issueOrPullRequestId.ToGitHubId(),
                        Content = reaction
                    })
                    .Select(payload => new
                    {
                        payload.ClientMutationId
                    })
                    .Compile();

            var result = await _connection.Run(mutation);
            return result.ClientMutationId;
        }

        public async ValueTask<string> RemoveLabelAsync(string issueOrPullRequestId, string clientId)
        {
            var mutation =
                new Mutation()
                    .ClearLabelsFromLabelable(new ClearLabelsFromLabelableInput
                    {
                        ClientMutationId = clientId,
                        LabelableId = issueOrPullRequestId.ToGitHubId()
                    })
                    .Select(payload => new
                    {
                        payload.ClientMutationId
                    })
                    .Compile();

            var result = await _connection.Run(mutation);
            return result.ClientMutationId;
        }

        public async ValueTask<string> RemoveReactionAsync(string issueOrPullRequestId, ReactionContent reaction, string clientId)
        {
            var mutation =
               new Mutation()
                   .RemoveReaction(new RemoveReactionInput
                   {
                       ClientMutationId = clientId,
                       SubjectId = issueOrPullRequestId.ToGitHubId(),
                       Content = reaction
                   })
                   .Select(payload => new
                   {
                       payload.ClientMutationId
                   })
                   .Compile();

            var result = await _connection.Run(mutation);
            return result.ClientMutationId;
        }

        public async ValueTask<string> UpdateIssueAsync(UpdateIssueInput input)
        {
            var mutation =
                new Mutation()
                    .UpdateIssue(input)
                    .Select(payload => new
                    {
                        payload.ClientMutationId
                    })
                    .Compile();

            var result = await _connection.Run(mutation);
            return result.ClientMutationId;
        }

        public async ValueTask<string> UpdateIssueAsync(int number, IssueUpdate input)
        {
            var result = await _client.Issue.Update(_config.Owner, _config.Repo, number, input);
            return result.NodeId;
        }

        public async ValueTask<string> UpdatePullRequestAsync(UpdatePullRequestInput input)
        {
            var mutation =
                new Mutation()
                    .UpdatePullRequest(input)
                    .Select(payload => new
                    {
                        payload.ClientMutationId
                    })
                    .Compile();

            var result = await _connection.Run(mutation);
            return result.ClientMutationId;
        }

        public async ValueTask<string> UpdatePullRequestAsync(int number, PullRequestUpdate input)
        {
            var result = await _client.PullRequest.Update(_config.Owner, _config.Repo, number, input);
            return result.NodeId;
        }

        public async ValueTask<(string, string)> GetIssueTitleAndBodyAsync(int issueNumber)
        {
            var query =
                new Query()
                    .Repository(_config.Repo, _config.Owner)
                    .Issue(issueNumber)
                    .Select(issue => new
                    {
                        issue.Title,
                        issue.Body
                    })
                    .Compile();

            var result = await _connection.Run(query);
            return (result.Title, result.Body);
        }

        public async ValueTask<(string, string)> GetPullRequestTitleAndBodyAsync(int pullRequestNumber)
        {
            var query =
                new Query()
                    .Repository(_config.Repo, _config.Owner)
                    .PullRequest(pullRequestNumber)
                    .Select(pullRequest => new
                    {
                        pullRequest.Title,
                        pullRequest.Body
                    })
                    .Compile();

            var result = await _connection.Run(query);
            return (result.Title, result.Body);
        }
    }
}