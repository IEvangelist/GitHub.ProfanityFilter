using IEvangelist.GitHub.Services.Filters;

namespace IEvangelist.GitHub.Services.Providers
{
    public interface IWordReplacerProvider
    {
        IWordReplacer GetWordReplacer();
    }
}