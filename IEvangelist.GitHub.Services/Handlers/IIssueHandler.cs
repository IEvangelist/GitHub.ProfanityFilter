using System.Threading.Tasks;

namespace IEvangelist.GitHub.Services.Handlers
{
    public interface IIssueHandler
    {
        ValueTask HandleIssueAsync(string payloadJson);
    }
}