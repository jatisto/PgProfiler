namespace PgProfile.Service.Interface
{
    public interface IParsingFactory<T> where T : class
    {
        List<T> RunParsing(string? pathToFolder, DateTime? selectedDate = null, string? fileName = null, bool isDateFilter = false);
    }
}