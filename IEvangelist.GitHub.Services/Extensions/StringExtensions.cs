using Octokit.GraphQL;
using Octokit.Internal;

namespace IEvangelist.GitHub.Services.Extensions
{
    static class StringExtensions
    {
        static readonly SimpleJsonSerializer Serializer = new SimpleJsonSerializer();

        internal static ID ToGitHubId(this string value) => new ID(value);

        internal static string ToJson<T>(this T value) => Serializer.Serialize(value);

        internal static T FromJson<T>(this string json) => Serializer.Deserialize<T>(json);
    }
}