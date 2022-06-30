using System.IO;

namespace PgProfiler.Helpers
{
	/// <summary>
	/// Класс помощник для работы с файлами
	/// </summary>
	public static class FileHelpers
	{
		/// <summary>
		/// Собераем путь
		/// </summary>
		/// <param name="pathToFolder">Путь до файла</param>
		/// <param name="postfix">Окончание пути (если требуется)</param>
		/// <param name="fileName">Наименование файла (если требуется)</param>
		private static string PathBuilder(string pathToFolder, string postfix = null!, string fileName = null!) =>
			$"{pathToFolder}\\{(string.IsNullOrEmpty(postfix) ? "" : $"{postfix}")}{(string.IsNullOrEmpty(fileName) ? "" : $"\\{fileName}")}";

		
		/// <summary>
		/// Получить список файлов в папке
		/// </summary>
		/// <param name="pathToFolder">Путь до папки</param>
		/// <param name="postfix">Конец пути</param>
		/// <param name="fileExtension">Расшерение файлов в папке (которые требуются)</param>
		/// <param name="searchOption">Опции поиска</param>
		public static string[] GetFileListInFolder(string pathToFolder, string postfix, string fileExtension,
			SearchOption searchOption) =>
				Directory.GetFiles(PathBuilder(pathToFolder, postfix), fileExtension, searchOption);

	}
}