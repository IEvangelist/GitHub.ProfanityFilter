using Microsoft.AspNetCore.Http;
using System.Linq;

namespace IEvangelist.GitHub.Webhooks.Extensions
{
    static class HttpHeadersExtensions
    {
        internal static string GetValueOrDefault(this IHeaderDictionary headers, string name) =>
            headers.TryGetValue(name, out var values) ? values.FirstOrDefault() : null;
    }
}