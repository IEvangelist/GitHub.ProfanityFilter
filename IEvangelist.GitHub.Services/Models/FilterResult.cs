namespace IEvangelist.GitHub.Services.Models
{
    internal readonly struct FilterResult
    {
        internal static FilterResult NotFiltered { get; } = new FilterResult();

        internal readonly string Title;
        internal readonly bool IsTitleFiltered;

        internal readonly string Body;
        internal readonly bool IsBodyFiltered;

        internal bool IsFiltered => IsTitleFiltered || IsBodyFiltered;

        internal FilterResult(
            string title,
            bool isTitleFiltered,
            string body,
            bool isBodyFiltered) =>
            (Title, IsTitleFiltered, Body, IsBodyFiltered) =
                (title, isTitleFiltered, body, isBodyFiltered);       
    }
}