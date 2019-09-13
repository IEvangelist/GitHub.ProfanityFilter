using IEvangelist.GitHub.Services.Enums;

namespace IEvangelist.GitHub.Services.Options
{
    public class GitHubOptions
    {
        public string ApiToken { get; set; }

        public string Owner { get; set; } = "IEvangelist";

        public string Repo { get; set; } = "GitHub.ProfanityFilter";

        public string WebhookSecret { get; set; }

        public string ProfaneLabelId { get; set; } = "MDU6TGFiZWwxNTI1MDA4Mzkz";

        public BodyTextReplacerType BodyTextReplacerType { get; set; }
    }
}