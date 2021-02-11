namespace IEvangelist.GitHub.Services.Models
{
    internal readonly struct FilterResult
    {
        internal static FilterResult NotFiltered { get; } = new FilterResult();

        internal string Title { get; }
        internal bool IsTitleFiltered { get; }

        internal string Body { get; }
        internal bool IsBodyFiltered { get; }

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