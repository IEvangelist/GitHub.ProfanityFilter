using IEvangelist.GitHub.Services.Extensions;
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

        string ReplaceProfanity(string content, char? placeHolder = null) =>
            string.Join(
                " ",
                content.Split(new[] { ' ' })
                       .Select(word => IsProfane(word) ? GetReplacement(word, placeHolder) : word));

        static string GetReplacement(string word, char? placeHolder = null) =>
            placeHolder.HasValue
                ? new string(placeHolder.Value, word.Length)
                : GitHubEmoji.ProfaneReplacements.GetRandomElement();
    }
}