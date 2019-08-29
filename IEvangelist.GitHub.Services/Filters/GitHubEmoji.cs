using System.Collections.Generic;

namespace IEvangelist.GitHub.Services.Filters
{
    static class GitHubEmoji
    {
        // Borrowed from: https://www.webfx.com/tools/emoji-cheat-sheet/
        internal static ISet<string> ProfaneReplacements { get; } = new HashSet<string>
        {
            ":rage:",
            ":boom:",
            ":hanky:",
            ":fire:",
            ":facepunch:",
            ":weary:",
            ":laughing:",
            ":hurtrealbad:",
            ":baby_bottle:",
            ":warning:",
            ":snowflake:",
            ":zap:",
            ":cry:",
            ":crying_cat_face:",
            ":rage2:",
            ":beer:",
            ":cocktail:",
            ":do_not_litter:",
            ":no_pedestrians:",
            ":scream:",
            ":smirk:",
            ":flushed:",
            ":anguished:",
            ":expressionless:",
            ":cold_sweat:",
            ":triumph:",
            ":disappointed:",
            ":confounded:"
        };
    }
}