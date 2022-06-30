using PgProfiler.Data;

namespace PgProfiler.Helpers ;

	/// <summary>
	/// Класс помошник для работы с дефолтными настройками
	/// </summary>
	public static class DefaultModelHelpers
	{
		/// <summary>
		/// Формирование дефолтной модели для пользователя
		/// </summary>
		public static SettingPg? GetDefaultModel ()
		{
			return new SettingPg()
			{
				Path = Constants.PathToLog,
				CodeEditor = Constants.CodeEditor,
				Settings = new List<Setting>()
				{
					new()
					{
						SettingPg = new ConnectionModel()
						{
							ServerDb = Constants.ServerDb,
							PortDb = Constants.PortDb,
							Login = Constants.Login,
							Password = Constants.Password,
							NameDb = Constants.NameDb,
							IsActive = Constants.IsActive,
							Uid = Constants.Uid,
						},
					}
				}
			};
		}
	}