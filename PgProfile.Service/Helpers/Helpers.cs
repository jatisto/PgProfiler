using System;
using System.IO;
using static System.Text.RegularExpressions.Regex;

namespace PgProfile.Service.Helpers
{
    /// <summary>
    /// Класс хелпер для работы с файлами
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Собераем путь
        /// </summary>
        /// <param name="pathToFolder">Путь до файла</param>
        /// <param name="postfix">Окончание пути (если требуется)</param>
        /// <param name="fileName">Наименование файла (если требуется)</param>
        public static string PathBuilder(string? pathToFolder, string postfix = null, string? fileName = null) =>
            $"{pathToFolder}\\{(string.IsNullOrEmpty(postfix) ? "" : $"{postfix}")}{(string.IsNullOrEmpty(fileName) ? "" : $"\\{fileName}")}";

        /// <summary>
        /// Получить список файлов в папке
        /// </summary>
        /// <param name="pathToFolder">Путь до папки</param>
        /// <param name="postfix">Конец пути</param>
        /// <param name="fileExtension">Расшерение файлов в папке (которые требуются)</param>
        /// <param name="searchOption">Опции поиска</param>
        public static string[] GetFileListInFolder(string? pathToFolder, string postfix, string fileExtension,
            SearchOption searchOption) =>
            Directory.GetFiles(PathBuilder(pathToFolder, postfix), fileExtension, searchOption);

        /// <summary>
        /// Разобрать наименование файла на части
        /// </summary>
        /// <param name="fileName">Наименование файла</param>
        public static (string Module, DateTime Data) ParseTheFileNameIntoParts(string? fileName)
        {
            string?[] result =
                SplitFilenameIntoElementsUsingRegex(fileName, @"([^A-Za-z.]$|.[0-9]{4}.[0-9]{2}.[0-9]{2}.log)");
            string?[] date = SplitFilenameIntoElementsUsingRegex(result[1], @"([^A-Za-z.]$|[0-9]{4}.[0-9]{2}.[0-9]{2})");
            return (result[0], DateTime.Parse(date[1]));
        }

        
        /// <summary>
        /// Разделить имя файла на элементы с помощью регулярного выражения
        /// </summary>
        /// <param name="fileName">Наименование файла</param>
        /// <param name="pattern">Паттерн (regex)</param>
        private static string?[] SplitFilenameIntoElementsUsingRegex(string? fileName, string pattern) =>
            Split(fileName, pattern);

        /// <summary>
        /// Проверка наименования файла на совподение
        /// </summary>
        /// <param name="fileName">Нименование файла</param>
        /// <param name="checkString">Строка для проверки</param>
        public static bool CheckingTheFileNameForAMatch(string fileName, string checkString) => Match(fileName, checkString).Success;

        /// <summary>
        /// Получить наименование сервиса и дату из наименования файла
        /// </summary>
        /// <param name="fileName">Наименование файла</param>
        public static (string Module, DateTime Data) GetServiceNameAndDateFromFileName(string? fileName)
        {
            string[] result = Split(fileName, @"([^A-Za-z.]$|.[0-9]{4}.[0-9]{2}.[0-9]{2}.log)");
            string[] date = Split(result[1], @"([^A-Za-z.]$|[0-9]{4}.[0-9]{2}.[0-9]{2})");
            return (result[0], DateTime.Parse(date[1]));
        }

        /// <summary>
        /// Разделить строку на два элемента массива
        /// </summary>
        /// <param name="content">Строка</param>
        /// <param name="pattern">Регулярное выражение</param>
        /// <param name="item"></param>
        public static (DateTime Data, string Content) SplitStringIntoTwoArrayElements(string content, string pattern,
            string item)
        {
            string[] result = Split(content, pattern);
            string[] date = Split(result[1], @"([0-9]{4}-(0[1-9]|1[0-2])-(0[1-9]|[1-2][0-9]|3[0-1]) [0-9]{2}:[0-9]{2}:[0-9]{2}?)");
            return (DateTime.Parse(date[1]), result[4]);
        }

        // public static (long FileSize, long FileSizeOnDisk, string FormatBytes) GetLenghtFile(string pathToFiles)
        // {
        //     var info = new FileInfo(pathToFiles);
        //     return (info.GetFileSizeOnDisk(), info.GetFileSizeOnDisk(), info.FormatBytes());
        // }

        public static DateTime GetFileCreationTime(string? pathToFolder, string postfix, string? fileName) =>
            File.GetCreationTime($"{PathBuilder(pathToFolder, postfix, fileName)}");
        
        public static DateTime GetFileCreationTime(string? pathToFolder,string? fileName) =>
            File.GetCreationTime($"{PathBuilder(pathToFolder, null, fileName)}");
    }
}