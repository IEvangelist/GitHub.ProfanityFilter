using IEvangelist.GitHub.Services.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace IEvangelist.GitHub.Services.Filters
{
    public class ProfanityFilter : IProfanityFilter
    {
        public bool IsProfane(string content) =>
            string.IsNullOrWhiteSpace(content)
                ? false
                : InappropriateLanguage.Expressions
                                       .Value
                                       .Any(exp => exp.IsMatch(content));

        public string ApplyFilter(string content, char? placeHolder = null) =>
            ReplaceProfanity(content, placeHolder);

        string ReplaceProfanity(string content, char? placeHolder = null)
        {
            var words =
                content.Split(new[] { ' ' })
                       .Select(word => (isProfane: IsProfane(word), word))
                       .ToList();

            var profaneCount = words.Count(_ => _.isProfane);
            var replacementQueue = 
                new Queue<string>(
                    GitHubEmoji.ProfaneReplacements
                               .GetRandomElements(profaneCount));

            return string.Join(
                " ",
                words.Select(_ => _.isProfane ? GetReplacement(_.word, replacementQueue, placeHolder) : _.word));
        }

        static string GetReplacement(string word, Queue<string> replacements, char? placeHolder = null) =>
            placeHolder.HasValue
                ? new string(placeHolder.Value, word.Length)
                : replacements.Dequeue();
    }
}