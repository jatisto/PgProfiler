using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MoiLib.ExecuteError;
using MoiLib.ExecuteError.ExecutionResultModelsByType;
using MoiLib.Helpers;
using PgProfiler.Db;
using PgProfiler.Dto;
using PgProfiler.Helpers;
using PgProfiler.Interface;
using FileHelpers = PgProfiler.Helpers.FileHelpers;

namespace PgProfiler.Data
{
	/// <summary>
	/// Класс для работы с настройками
	/// </summary>
	public class WorkingWithSettings : IWorkingWithSettings
	{
		/// <summary>
		/// Возвращает список файлов в папки.
		/// </summary>
		/// <param name="pathToFolder">Путь до папки</param>
		/// <param name="data">Дата</param>
		public IEnumerable<LogFileInfo> GetFileInDirectory (string pathToFolder, DateTime? data = null)
		{
			var logFile = new List<string>();
			logFile.AddRange(Helpers.FileHelpers.GetFileListInFolder(pathToFolder,
				"",
				"*.log*",
				SearchOption.TopDirectoryOnly));

			var fileList = new List<LogFileInfo>();
			if (fileList == null)
				throw new ArgumentNullException(nameof(fileList));

			fileList.AddRange(from file in logFile.Select((value, i) => (value, i))
				let info = new FileInfo(file.value)
				select new LogFileInfo()
				{
					Id = file.i,
					Name = info.Name,
					CreateDateTime = info.CreationTime,
					DirectoryName = info.DirectoryName,
					Directory = info.Directory?.Name
				});
			if (data != null)
			{
				return
					new List<LogFileInfo>(
						fileList.OrderByDescending(x => x.Id)
							.Where(x => x.CreateDateTime.Date == data.GetValueOrDefault().Date));
			}

			return
				new List<LogFileInfo>(
					fileList.OrderByDescending(x => x.Id));
		}

		/// <summary>
		/// Получить настройки
		/// </summary>
		public ValueTask<SettingPg> GetSettings ()
		{
			var appData = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + $"\\{Constants.PostfixFolderName}";
			CheckDirectoryOrCreate(appData);
			try
			{
				var settings = GetSettingFile<SettingPg>(appData + $"\\{Constants.SettingFileName}");
				return ValueTask.FromResult(settings);
			} catch (Exception e)
			{
				return ValueTask.FromResult(DefaultModelHelpers.GetDefaultModel() ?? new SettingPg());
			}
			
		}

		/// <summary>
		/// Получить настройки подключения по id
		/// </summary>
		/// <param name="uId">Id настройки</param>
		public ValueTask<ConnectionModel?> GetСonnectionSettingsById (string? uId)
		{
			var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\{Constants.PostfixFolderName}";
			CheckDirectoryOrCreate(appData);
			var settings = GetSettingFile<SettingPg>(appData + $"\\{Constants.SettingFileName}");
			var result = settings?.Settings!.Select(x => x.SettingPg).ToList();
			return ValueTask.FromResult(result != null ? result.FirstOrDefault(x => x!.Uid == uId) : new ConnectionModel());
		}

		/// <summary>
		/// Сохранение настроек
		/// </summary>
		/// <param name="connectionModel">Модель подключений</param>
		/// <param name="path">Путь к логам</param>
		/// <param name="codeEditor">Редактор (notepad++.exe)</param>
		public async Task<ExecuteResult> SaveSettings (ConnectionModel? connectionModel, string? path, string? codeEditor)
		{
			var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\{Constants.PostfixFolderName}";
			CheckDirectoryOrCreate(appData);
			var settings = GetSettingFile<SettingPg?>(appData + $"\\{Constants.SettingFileName}");

			if (settings != null)
			{
				var check =
					settings!.Settings?.FirstOrDefault(
						x => x.SettingPg!.Uid == connectionModel?.Uid) == null;

				var checkDbName =
					settings.Settings?.FirstOrDefault(
						x => x.SettingPg!.NameDb == connectionModel?.NameDb && 
						     x.SettingPg.ServerDb == connectionModel?.ServerDb &&
						     x.SettingPg.PortDb == connectionModel?.PortDb &&
						     x.SettingPg.Login == connectionModel?.Login &&
						     x.SettingPg.Password == connectionModel?.Password);
				var old = checkDbName?.SettingPg;
				if (check)
				{
					settings.Path = path;
					settings.CodeEditor = codeEditor;
					if (checkDbName == null)
					{
						settings.Settings!.Add(new Setting()
						{
							SettingPg = connectionModel
						});
					} else
					{
						settings.Settings!.Remove(checkDbName);
						settings.Settings!.Add(new Setting()
						{
							SettingPg = old
						});
					}
				}
				var writeToFileNew = await WriteToFile(settings, () => Path.Combine(appData + $"\\{Constants.SettingFileName}"));
				if (!writeToFileNew.Result)
					return new ExecuteResult()
					{
						Message = $@"Ошибка записи в файл.{Environment.NewLine}Причина: {writeToFileNew.Message}",
						State = ExecuteState.Error
					};
				if (checkDbName == null)
				{
					await SetActiveSettings(connectionModel?.Uid);	
				}
			
				return new ExecuteResult()
				{
					Message = @"Данные успешно записаны в файл",
					State = ExecuteState.Ok
				};
			} else
			{
				var model = new SettingPg();
				if (connectionModel == null)
				{
					model = DefaultModelHelpers.GetDefaultModel() ?? new SettingPg();
				}else
				{
					connectionModel.IsActive = true;
					model.Path = path ?? Constants.PathToLog;
					model.CodeEditor = codeEditor ?? Constants.CodeEditor;
					model.Settings = new List<Setting>()
					{
						new()
						{
							SettingPg = connectionModel
						}
					};
				}
				var writeToFileNew = await WriteToFile(model, () => Path.Combine(appData + $"\\{Constants.SettingFileName}"));
				if (!writeToFileNew.Result)
					return new ExecuteResult()
					{
						Message = $@"Ошибка записи в файл.{Environment.NewLine}Причина: {writeToFileNew.Message}",
						State = ExecuteState.Error
					};

				return new ExecuteResult()
				{
					Message = @"Данные успешно записаны в файл",
					State = ExecuteState.Ok
				};
			}
		}

		/// <summary>
		/// Установить активным подключение
		/// </summary>
		/// <param name="uId">Id настройки</param>
		public async Task<ExecuteResult> SetActiveSettings (string? uId)
		{
			var appData = Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + $"\\{Constants.PostfixFolderName}";
			CheckDirectoryOrCreate(appData);
			var settings = GetSettingFile<SettingPg>(appData + $"\\{Constants.SettingFileName}");
			if (settings.Settings != null && !settings.Settings.Any())
				return new ExecuteResult()
				{
					Message = $@"Ошибка: по пути {appData + $"\\{Constants.SettingFileName}"} настройки не найдены.",
					State = ExecuteState.Error
				};


			var checkIsActive =
				settings.Settings?.FirstOrDefault(
					x => x.SettingPg!.IsActive == true);

			if (checkIsActive != null)
				checkIsActive.SettingPg!.IsActive = false;

			var model1 = new SettingPg()
			{
				Path = settings.Path,
				Settings = settings.Settings
			};
			var writeToFileIsActive = await WriteToFile(model1,
				() => Path.Combine(appData + $"\\{Constants.SettingFileName}"));

			if (!writeToFileIsActive.Result)
				return new ExecuteResult()
				{
					Message = $@"Ошибка записи в файл.{Environment.NewLine}Причина: {writeToFileIsActive.Message}",
					State = ExecuteState.Error
				};

			var checkUid =
				settings?.Settings!.FirstOrDefault(
					x => x.SettingPg!.Uid == uId);

			if (checkUid != null)
				checkUid.SettingPg!.IsActive = true;

			var writeToFile = await WriteToFile(settings, () => Path.Combine(appData + $"\\{Constants.SettingFileName}"));

			if (!writeToFile.Result)
				return new ExecuteResult()
				{
					Message = $@"Ошибка записи в файл.{Environment.NewLine}Причина: {writeToFile.Message}",
					State = ExecuteState.Error
				};
			
			// Query.Disconnection();
			return writeToFile;
		}

		/// <summary>
		/// Получить результат десериализации в текущем рабочем каталоге по наименованию файла
		/// </summary>
		/// <param name="path">Наименование файла</param>
		private static T GetSettingFile<T> (string path)
			=>
				FileAssistantJsonHelper.GetDeserializationTGenericFromFileByPath<T>(
					() => path);


		/// <summary>
		/// Запись в файл списка объектов типа <see cref="KeyValuePair"/> с ключём (<see cref="string"/>) и объектной моделью типа <see cref="T"/>"/>
		/// </summary>
		/// <param name="model">Данные для записи</param>
		/// <param name="path"></param>
		private async Task<BoolExecuteResult> WriteToFile<T> (T model, Func<string> path)
			=> await FileAssistantJsonHelper.WriteToFile<T>(model, path: path);

		public  async Task<string?>  ReadAllText (string path)
		{
			var result = await ByteHelper.ReadAllLinesAsync(() => path);
			string? convert = default;
			if (result != null)
			{
				convert = Encoding.Default.GetString(result.Where(x => x != 0).ToArray());
			}
			return convert;
		}

		/// <summary>
		/// Проверить каталог или создать его если не существует
		/// </summary>
		/// <param name="path">Путь</param>
		private static void CheckDirectoryOrCreate (string path)
		{
			DirectoryInfo dirInfo = new(path);
			if (!dirInfo.Exists) dirInfo.Create();
		}
	}
}