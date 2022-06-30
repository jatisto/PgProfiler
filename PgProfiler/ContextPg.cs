using System.Text.Json;
using MoiLib.ExecuteError;
using PgProfiler.Helpers;
using PgProfiler.Data;
using static System.Text.Json.JsonSerializer;

namespace PgProfiler ;

	/// <summary>
	/// Контекст
	/// </summary>
	public class ContextPg : ExecuteErrorHandling
	{
		/// <summary>
		/// Настройки
		/// </summary>
		private static SettingPg? SettingPg { get; set; }
		
		/// <summary>
		///  Модель данных
		/// </summary>
		private static ConnectionModel? ConnectionModel { get; }

		/// <summary>
		/// Конструктор
		/// </summary>
		static ContextPg ()
		{
			SettingPg = ContextSettings.GetSettingsStatic();
			ConnectionModel = ContextSettings.GetConnectionModelStatic();
		}

		/// <summary>
		/// Получает наимнование БД
		/// </summary>
		public static string? GetSettingDbname ()
		{
			var bean = ConnectionModel;
			if (bean != null && bean.Control_())
			{
				return bean.NameDb;
			}
			return default;
		}

		/// <summary>
		/// Получить путь до логов
		/// </summary>
		public static string? GetPath ()
		{
			var setting = SettingPg;
			return setting?.Path;
		}

		/// <summary>
		/// Получить редактор
		/// </summary>
		public static string? GetCodeEditor()
		{
			var setting = SettingPg;
			return setting?.CodeEditor;
		}

		/// <summary>
		/// Обёртка над Exception
		/// </summary>
		/// <param name="message">Сообщение</param>
		private static void ThrowException (string message) { throw new Exception(message); }
	}