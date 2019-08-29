using IEvangelist.GitHub.Services.Options;
using IEvangelist.GitHub.Webhooks.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography;
using System.Text;

namespace IEvangelist.GitHub.Webhooks.Validators
{
    public class GitHubPayloadValidator : IGitHubPayloadValidator
    {
        readonly GitHubOptions _options;

        public GitHubPayloadValidator(IOptions<GitHubOptions> options) =>
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

        public bool IsPayloadSignatureValid(byte[] bytes, string receivedSignature)
        {
            if (string.IsNullOrWhiteSpace(receivedSignature))
            {
                return false;
            }

            using var hmac = new HMACSHA1(Encoding.ASCII.GetBytes(_options.WebhookSecret));
            var hash = hmac.ComputeHash(bytes);
            var actualSignature = $"sha1={hash.ToHexString()}";

            return IsSignatureValid(actualSignature, receivedSignature);
        }

        static bool IsSignatureValid(string a, string b)
        {
            var length = Math.Min(a.Length, b.Length);
            var equals = a.Length == b.Length;
            for (var i = 0; i < length; ++i)
            {
                equals &= a[i] == b[i];
            }

            return equals;
        }
    }
}