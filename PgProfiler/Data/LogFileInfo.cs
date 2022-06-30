using System;

namespace PgProfiler.Data
{
	/// <summary>
	/// Описание лог файла 
	/// </summary>
	public class LogFileInfo
	{
		/// <summary>
		/// Id лог файла
		/// </summary>
		public int Id { get; set; }
		
		/// <summary>
		/// Наименование лог файла
		/// </summary>
		public string? Name { get; set; }

		/// <summary>
		/// Дата и время создания
		/// </summary>
		public DateTime CreateDateTime { get; set; }
		
		/// <summary>
		/// Путь до лог файла
		/// </summary>
		public string? DirectoryName { get; set; }

		/// <summary>
		/// Дириктория лог файла
		/// </summary>
		public string? Directory { get; set; }
	}
}