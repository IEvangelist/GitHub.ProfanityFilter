namespace IEvangelist.GitHub.Services.Filters
{
    public interface IProfanityFilter
    {
        bool IsProfane(string content);

        string ApplyFilter(string content, char? placeHolder = null);
    }
}