using PgProfile.Service.Interface;

namespace PgProfile.Service.FactoryMethod.ParsingSingleFile
{
    public class DebugFile<T> : Creator<T> where T : class
    {
        public DebugFile(string? pathToFolder, DateTime? selectedDate, string? fileName) : base(pathToFolder, selectedDate, fileName)
        {
        }

        protected override IParsingFactory<T>? FactoryMethod() => new ParsingDebugFile() as IParsingFactory<T>;
    }
}