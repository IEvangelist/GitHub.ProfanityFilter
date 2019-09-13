using System;
using System.Collections.Generic;
using System.Linq;
using IEvangelist.GitHub.Services.Enums;
using IEvangelist.GitHub.Services.Filters;
using IEvangelist.GitHub.Services.Options;
using Microsoft.Extensions.Options;

namespace IEvangelist.GitHub.Services.Providers
{
    public class WordReplacerProvider : IWordReplacerProvider
    {
        readonly IEnumerable<IWordReplacer> _wordReplacers;
        readonly GitHubOptions _config;

        public WordReplacerProvider(
            IEnumerable<IWordReplacer> wordReplacers,
            IOptions<GitHubOptions> options) => 
            (_wordReplacers, _config) = (wordReplacers, options.Value);

        public IWordReplacer GetWordReplacer() =>
            _config.BodyTextReplacerType switch
            {
                BodyTextReplacerType.GitHubEmoji => _wordReplacers.FirstOrDefault(wr => wr is GitHubEmojiWordReplacer),
                BodyTextReplacerType.LintLicker => _wordReplacers.FirstOrDefault(wr => wr is LintLickerWordReplacer),
                _ => throw new Exception("There is no corresponding word replacer available.")
            };
    }
}