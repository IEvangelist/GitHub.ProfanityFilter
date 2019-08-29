using IEvangelist.GitHub.Services.Extensions;
using IEvangelist.GitHub.Webhooks;
using IEvangelist.GitHub.Webhooks.Validators;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

[assembly: WebJobsStartup(typeof(Startup))]
namespace IEvangelist.GitHub.Webhooks
{
    internal class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var descriptor = builder.Services.FirstOrDefault(d => d.ServiceType == typeof(IConfiguration));
            if (descriptor?.ImplementationInstance is IConfigurationRoot configuration)
            {
                ConfigureServices(builder.Services, configuration).BuildServiceProvider(true);
            }
            else
            {
                throw new ApplicationException("The function requires a valid IConfigurationRoot instance.");
            }
        }

        IServiceCollection ConfigureServices(
            IServiceCollection services, IConfiguration configuration) => 
            services.AddGitHubServices(configuration)
                    .AddSingleton<IGitHubPayloadValidator, GitHubPayloadValidator>();
    }
}