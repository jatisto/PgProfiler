using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MoiLib.ExecuteError;
using MoiLib.ExecuteError.ExecutionResultModelsByType;
using MoiLib.Helpers;
using PgProfiler.Data;

namespace PgProfiler.Interface
{
	/// <summary>
	/// Интерфейс для работы с данными
	/// </summary>
	public interface IWorkingWithSettings
	{
		/// <summary>
		/// Возвращает список файлов в папки.
		/// </summary>
		/// <param name="pathToFolder">Путь до папки</param>
		/// <param name="data">Дата</param>
		IEnumerable<LogFileInfo> GetFileInDirectory (string pathToFolder, DateTime? data);

		/// <summary>
		/// Получить настройки
		/// </summary>
		ValueTask<SettingPg> GetSettings ();
		
		/// <summary>
		/// Получить настройки подключения по id
		/// </summary>
		/// <param name="uId">Id настройки</param>
		ValueTask<ConnectionModel?> GetСonnectionSettingsById (string? uId);
		
		/// <summary>
		/// Сохранение настроек
		/// </summary>
		/// <param name="connectionModel">Модель подключений</param>
		/// <param name="path">Путь к логам</param>
		/// <param name="codeEditor">Редактор (notepad++.exe)</param>
		Task<ExecuteResult> SaveSettings (ConnectionModel? connectionModel, string? path, string? codeEditor);
		
		/// <summary>
		/// Усанавливаем активные настройки
		/// </summary>
		/// <param name="uId">Id настройки</param>
		Task<ExecuteResult> SetActiveSettings (string? uId);
	}
}