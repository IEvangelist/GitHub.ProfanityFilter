using IEvangelist.GitHub.Services.Extensions;
using System.Collections.Generic;

namespace IEvangelist.GitHub.Services.Filters
{
    public class LintLickerWordReplacer : IWordReplacer
    {
        // In homage to https://www.youtube.com/watch?v=5ssytWYn6nY
        internal static ISet<string> Words { get; } = new HashSet<string>
        {
            "*biscuit-eating-bulldog*",
            "*french-toast*",
            "*doodoo-head*",
            "*cootie-queen*",
            "*lint-licker*",
            "*pickle*",
            "*kumquat*",
            "*stinky-mc-stinkface*"
        };

        public string ReplaceWord(string word)
        {
            return Words.GetRandomElement();
        }
    }
}
