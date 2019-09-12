using System.Collections.Generic;
using System.Linq;

namespace IEvangelist.GitHub.Services.Filters
{
    public class ProfanityFilter : IProfanityFilter
    {
        readonly IWordReplacer _wordReplacer;

        public ProfanityFilter(IWordReplacer wordReplacer) => 
            _wordReplacer = wordReplacer;

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

            return string.Join(
                " ",
                words.Select(_ => _.isProfane ? GetReplacement(_.word, null, placeHolder) : _.word));
        }

        string GetReplacement(string word, Queue<string> replacements, char? placeHolder = null) =>
            placeHolder.HasValue
                ? new string(placeHolder.Value, word.Length)
                : _wordReplacer.ReplaceWord(word);
    }
}