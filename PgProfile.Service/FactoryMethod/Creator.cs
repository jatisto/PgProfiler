using PgProfile.Service.Interface;

namespace PgProfile.Service.FactoryMethod
{
	/// <summary>
	/// Класс для создания шаблонного метода
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class Creator<T> where T : class
	{
		protected Creator (string? pathToFolder, DateTime? selectedDate, string? fileName, bool isDateFilter = false)
		{
			PathToFolder = pathToFolder;
			FileName = fileName;
			SelectedDate = selectedDate;
			IsDateFilter = isDateFilter;
		}

		/// <summary>
		/// Путь до папки
		/// </summary>
		private string? PathToFolder { get; set; }

		/// <summary>
		/// Включить фильтор по дате
		/// </summary>
		private bool IsDateFilter { get; set; }

		/// <summary>
		/// Наименование файла
		/// </summary>
		private string? FileName { get; set; }

		/// <summary>
		/// Выбранная дата
		/// </summary>
		private DateTime? SelectedDate { get; set; }

		/// <summary>
		/// Метод создания
		/// </summary>
		protected abstract IParsingFactory<T>? FactoryMethod ();

		/// <summary>
		/// Вызов шаблонного метода
		/// </summary>
		public List<T> Parsing () =>
			FactoryMethod()!.RunParsing(PathToFolder, SelectedDate, FileName, IsDateFilter);
	}
}