namespace IEvangelist.GitHub.Services.Options
{
    public class GitHubOptions
    {
        public string ApiToken { get; set; }

        public string Owner { get; set; } = "IEvangelist";

        public string Repo { get; set; } = "GitHub.ProfanityFilter";

        public string WebhookSecret { get; set; }
    }
}