using IEvangelist.GitHub.Services.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace IEvangelist.GitHub.Services.Filters
{
    public class LintLickerWordReplacer : IWordReplacer
    {
        // In homage to https://www.youtube.com/watch?v=5ssytWYn6nY
        internal static ISet<string> Words { get; } = new HashSet<string>
        {
            "biscuit-eating-bulldog",
            "french-toast",
            "doodoo-head",
            "cootie-queen",
            "lint-licker",
            "pickle",
            "kumquat",
            "stinky-mc-stinkface"
        };

        // float words starting with the same letter to the top of the list
        public string ReplaceWord(string word) => Words.Shuffle()
            .OrderByDescending(x => x.StartsWith(word[0].ToString()) ? 1 : 0)
            .First();

    }
}
