using IEvangelist.GitHub.Services.Extensions;
using IEvangelist.GitHub.Services.Options;
using Microsoft.Extensions.Options;
using Octokit.GraphQL;
using Octokit.GraphQL.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IEvangelist.GitHub.Services.GraphQL
{
    public class GitHubGraphQLClient : IGitHubGraphQLClient
    {
        const string ProductID = "GitHub.ProfanityFilter";
        const string ProductVersion = "1.0";

        readonly IConnection _connection;
        readonly GitHubOptions _config;

        public GitHubGraphQLClient(IOptions<GitHubOptions> config)
        {
            _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
            _connection = new Connection(new ProductHeaderValue(ProductID, ProductVersion), _config.ApiToken);
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
    }
}