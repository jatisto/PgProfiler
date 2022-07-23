using PgProfile.Service.FactoryMethod.ParsingSingleFile;

namespace PgProfile.Service.FactoryMethod
{
    public class LogProcessor<T> where T : class
    {
        /// <summary>
        /// Метод для распределения парсинга
        /// </summary>
        /// <param name="pathToFolder"></param>
        /// <param name="selectedDate"></param>
        /// <param name="fileName"></param>
        public List<T> Parsing(string? pathToFolder, DateTime? selectedDate, string? fileName = null)
        {
            return Run(new DebugFile<T>(pathToFolder, selectedDate, fileName));
        }
        private static List<T> Run(Creator<T> creator) => creator.Parsing();
    }
}