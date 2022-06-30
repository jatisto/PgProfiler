using System.Data;
using System.Diagnostics;
using MoiLib.ExecuteError;
using MoiLib.ExecuteError.ExecutionResultModelsByType;
using PgProfiler.Db;
using PgProfiler.Dto;
using PgProfiler.Interface;

namespace PgProfiler.Data
{
	/// <summary>
	/// Класс для работы с логами PostgreSQL
	/// </summary>
	public class PgProfilerService
	{
		private readonly IWorkingWithSettings _workingWithSettings;
		private static Query? _query;

		public PgProfilerService (ISettings settings)
		{
			_workingWithSettings = new WorkingWithSettings();
			_query = new Query (settings);
		}

		/// <summary>
		/// Возвращает список файлов в папки.
		/// </summary>
		/// <param name="pathToFolder">Путь до папки</param>
		/// <param name="data">Дата</param>
		public Task<IEnumerable<LogFileInfo>> GetFileInDirectory (string pathToFolder, DateTime? data = null)
		{
			var dictionary = _workingWithSettings.GetFileInDirectory(pathToFolder, data);
			return Task.FromResult(dictionary);
		}

		/// <summary>
		/// Получить настройки
		/// </summary>
		public ValueTask<SettingPg> GetSettings () { return _workingWithSettings.GetSettings(); }

		/// <summary>
		/// Получить данные из view pg_stat_statements
		/// </summary>
		public ResultExecuteResult<List<StatementV1>?> GetDataFromPgStatStatements (BuilderQuery query)
		{
			return _query!.GetDataFromPgStatStatements(query);
		}

		/// <summary>
		/// Получить данные из view pg_stat_statements
		/// </summary>
		public BoolExecuteResult IsExistsView (string nameView)
		{
			return _query!.IsExistsView(nameView);
		}

		/// <summary>
		/// Установка view pg_stat_statements и statements_v1
		/// </summary>
		public static void InstallPgStatStatements () => _query!.InstallPgStatStatements();

		/// <summary>
		/// Удалить view pg_stat_statements и statements_v1
		/// </summary>
		public static void DeletePgStatStatements () => _query!.DeletePgStatStatements();

		/// <summary>
		/// Получить данные из view pg_stat_statements
		/// </summary>
		public ValueTask<ResultExecuteResult<T>> GetDataFromPgStatStatementsFunk<T> (Func<DataRowCollection, T> func, BuilderQuery query)
			where T: ExecuteResult, new()
		{
			return _query!.GetDataFromPgStatStatementsFunk(func, query);
		}

		/// <summary>
		/// Отчистка логов из базы данных
		/// </summary>
		/// <param name="dbId">Id базы данных для которой требуеться отчистка</param>
		public static BoolExecuteResult PgStatStatementsReset (uint dbId) => _query!.PgStatStatementsReset(dbId);
		
		/// <summary>
		/// Получить настройки подключения по id
		/// </summary>
		/// <param name="uId">Id настройки</param>
		public ValueTask<ConnectionModel?> GetСonnectionSettingsById (string? uId) { return _workingWithSettings.GetСonnectionSettingsById(uId); }

		/// <summary>
		/// Сохранение настроек
		/// </summary>
		/// <param name="connectionModel">Модель подключений</param>
		/// <param name="path">Путь к логам</param>
		/// <param name="codeEditor">Редактор (notepad++.exe)</param>
		public async Task<ExecuteResult> SaveSettings (ConnectionModel? connectionModel, string? path, string? codeEditor)
		{
			var res = await _workingWithSettings.SaveSettings(connectionModel, path, codeEditor);
			if (!res.IsOk)
				return new ExecuteResult()
				{
					Message = $@"Ошибка записи в файл.{Environment.NewLine}Причина: {res.Message}",
					State = ExecuteState.Error
				};

			
			return new ExecuteResult
			{
				Message = res.Message,
				State = ExecuteState.Ok
			};
		}
		
		/// <summary>
		/// Усанавливаем активные настройки
		/// </summary>
		/// <param name="uId">Id настройки</param>
		public async Task SetActiveSettings (string? uId)
		{
			await _workingWithSettings.SetActiveSettings(uId);
		}
		
		/// <summary>
		/// Открыть файл в редакторе
		/// </summary>
		/// <param name="directoryName">Путь до файла</param>
		/// <param name="fileName">Наименование файла</param>
		public static void ProcessStart (string? directoryName, string? fileName)
		{
			var codeEditor = ContextPg.GetCodeEditor();
			var path = Path.Combine(directoryName + "\\" + fileName);
			if (string.IsNullOrEmpty(codeEditor))
			{
				new Process
				{
					StartInfo = new ProcessStartInfo(path)
					{
						UseShellExecute = true
					}
				}.Start();
			}else
			{
				new Process
				{
					StartInfo = new ProcessStartInfo
					{
						UseShellExecute = true,
						FileName = codeEditor,
						WorkingDirectory = directoryName,
						Arguments = $"{fileName}"
					}
				}.Start();
			}
		}
	}
}