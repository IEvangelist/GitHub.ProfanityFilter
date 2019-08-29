using System.Threading.Tasks;

namespace IEvangelist.GitHub.Services.Handlers
{
    public interface IPullRequestHandler
    {
        ValueTask HandlePullRequestAsync(string payloadJson);
    }
}