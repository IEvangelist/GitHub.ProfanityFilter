using System.Threading.Tasks;

namespace IEvangelist.GitHub.Services
{
    public interface IGitHubWebhookDispatcher
    {
        ValueTask DispatchAsync(string eventName, string payloadJson);
    }
}