using System.Text.Json;
using MoiLib.ExecuteError;
using PgProfiler.Data;
using PgProfiler.Helpers;
using static System.Text.Json.JsonSerializer;

namespace PgProfiler ;


	/// <inheritdoc cref="MoiLib.ExecuteError.ExecuteErrorHandling" />
	public class ContextSettings : ExecuteErrorHandling, ISettings
	{
		private SettingPg? ContextSetting { get; set; } = new();
		private ConnectionModel? ConnectionModel { get; set; } = new();

		public ContextSettings () { GetSettingsFromFile(); }

		private void GetSettingsFromFile ()
		{
			Execute(() =>
			{
				var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
				              + $"\\{Constants.PostfixFolderName}";

				if (!File.Exists(appData + $"\\{Constants.SettingFileName}"))
				{
					ContextSetting = new SettingPg();
				} else
				{
					using FileStream fs = new(appData + $"\\{Constants.SettingFileName}", FileMode.OpenOrCreate);

					var options = new JsonSerializerOptions
					{
						WriteIndented = true
					};

					try
					{
						var settings = Deserialize<SettingPg>(fs, options);
						ContextSetting = settings;
					} catch (Exception e)
					{
						ContextSetting = DefaultModelHelpers.GetDefaultModel();
					}
				}

				if (ContextSetting == null) return;
				var setting = ContextSetting.Settings;
				ConnectionModel? result;
				if (setting == null)
					return;
				{
					result = setting.Select(x => x.SettingPg).FirstOrDefault(x => x is { IsActive: true });
				}
				ConnectionModel = result;
			});
		}

		public SettingPg GetSettings () => ContextSetting ?? new SettingPg();

		public static SettingPg GetSettingsStatic () => new ContextSettings().GetSettings();
		
		public static ConnectionModel GetConnectionModelStatic () => new ContextSettings().GetConnectionModel();
		
		public ConnectionModel GetConnectionModel () => ConnectionModel ?? new ConnectionModel();

		public string GetConnectionString ()
		{
			var connectionString =
				"PORT=5432;DATABASE=postgres;HOST=localhost;PASSWORD=123456;USER ID=postgres";

			var connectionModel = ConnectionModel;

			if (connectionModel != null)
			{
				connectionString = "PORT=" + connectionModel.PortDb + ";DATABASE=" + connectionModel.NameDb + ";HOST="
				                   + connectionModel.ServerDb + ";PASSWORD="
				                   + connectionModel.Password + ";USER ID="
				                   + connectionModel.Login + "";
			}
			return connectionString;
		}
	}

	public interface ISettings
	{
		SettingPg? GetSettings ();
		string? GetConnectionString ();
		ConnectionModel? GetConnectionModel ();
	}