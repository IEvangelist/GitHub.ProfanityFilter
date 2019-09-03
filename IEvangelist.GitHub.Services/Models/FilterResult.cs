namespace IEvangelist.GitHub.Services.Models
{
    internal struct FilterResult
    {
        internal static FilterResult NotFiltered { get; } = new FilterResult();

        internal string Title;
        internal bool IsTitleFiltered;

        internal string Body;
        internal bool IsBodyFiltered;

        internal bool IsFiltered => IsTitleFiltered || IsBodyFiltered;
    }
}