using IEvangelist.GitHub.Repository.Extensions;
using IEvangelist.GitHub.Services.Filters;
using IEvangelist.GitHub.Services.GraphQL;
using IEvangelist.GitHub.Services.Handlers;
using IEvangelist.GitHub.Services.Options;
using IEvangelist.GitHub.Services.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IEvangelist.GitHub.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGitHubServices(
            this IServiceCollection services,
            IConfiguration configuration) =>
            services.AddLogging(logging => logging.AddFilter(level => true))
                    .AddOptions()
                    .Configure<GitHubOptions>(configuration.GetSection(nameof(GitHubOptions)))
                    .AddSingleton<IGitHubGraphQLClient, GitHubGraphQLClient>()
                    .AddSingleton<IIssueHandler, IssueHandler>()
                    .AddSingleton<IPullRequestHandler, PullRequestHandler>()
                    .AddSingleton<IGitHubWebhookDispatcher, GitHubWebhookDispatcher>()
                    .AddSingleton<IProfanityFilter, ProfanityFilter>()
                    .AddSingleton<IWordReplacerProvider, WordReplacerProvider>()
                    .AddSingleton<IWordReplacer, GitHubEmojiWordReplacer>()
                    .AddSingleton<IWordReplacer, LintLickerWordReplacer>()
                    .AddGitHubRepository(configuration);
    }
}