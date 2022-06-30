using MoiLib.ExecuteError;
using MoiLib.Extension;
using Npgsql;
using PgProfiler.Data;

namespace PgProfiler ;

	/// <summary>
	/// Класс для работы с подключением к базе данных PostgreSQL
	/// </summary>
	public class ConnectionNpgSql : ExecuteErrorHandling
	{
		private readonly ISettings? _settings;
		
		/// <summary>
		/// Наименование баызы данных
		/// </summary>
		private  string? ConnectionDb { get; set; }

		/// <summary>
		/// Конструктор класса
		/// </summary>
		public ConnectionNpgSql (ISettings? settings)
		{
			_settings = settings;
		}

		/// <summary>
		/// Подключиться к базе данных
		/// </summary>
		public NpgsqlConnection? Connection () { return GetConnection(_settings!.GetConnectionModel()); }

		/// <summary>
		/// Получить наименование базы данных
		/// </summary>
		public string? GetConnectionInfo () => ConnectionDb;
		
		/// <summary>
		/// Получить соединение
		/// </summary>
		/// <param name="connectionModel">Модель подключения</param>
		private  NpgsqlConnection? GetConnection (ConnectionModel? connectionModel)
		{
			return Execute(() =>
			{
				if (connectionModel != null && connectionModel.Control_())
				{
					var con = IsConnection(connectionModel);
					con.Open();
					ConnectionDb = connectionModel.NameDb;
					return con;
				}
				ThrowException("Подключение невозможно! Введите новые параметры подключения!");
				return null;
			});
		}

		/// <summary>
		/// Проверка подключения
		/// </summary>
		/// <param name="connectionModel">Модельь подключений</param>
		private static NpgsqlConnection IsConnection (ConnectionModel? connectionModel)
		{
			return Execute(() =>
			{
				var connectionString =
					"PORT=5432;DATABASE=postgres;HOST=localhost;PASSWORD=123456;USER ID=postgres";

				if (connectionModel != null && connectionModel.Control_())
				{
					connectionString = "PORT=" + connectionModel.PortDb + ";DATABASE=" + connectionModel.NameDb + ";HOST=" + connectionModel.ServerDb + ";PASSWORD="
					                   + connectionModel.Password + ";USER ID="
					                   + connectionModel.Login + "";
				}

				return new NpgsqlConnection(connectionString);
			});
		}

		/// <summary>
		/// Проверка подключения
		/// </summary>
		public bool Connection_Test (ConnectionModel? connectionModel) { return Test_Connection(connectionModel); }

		/// <summary>
		/// Проверка подключения
		/// </summary>
		/// <param name="connectionModel">Модельь подключений</param>
		private bool Test_Connection (ConnectionModel? connectionModel)
		{
			if (connectionModel == null || !connectionModel.Control_())
				return false;
			var con = new NpgsqlConnection();
			try
			{
				var connectionString = "PORT=" + connectionModel.PortDb
				                       + ";TIMEOUT=15;POOLING=True;MINPOOLSIZE=1;MAXPOOLSIZE=20;COMMANDTIMEOUT=20;COMPATIBLE= 2.0.14.3;DATABASE="
				                       + connectionModel.NameDb + ";HOST=" + connectionModel.ServerDb + ";PASSWORD=" + connectionModel.Password + ";USER ID="
				                       + connectionModel.Login + "";
				con = new NpgsqlConnection(connectionString);
				con.Open();
				return true;
			} catch (NpgsqlException ex)
			{
				ThrowException(ex.CreateErrorMessage());
				return false;
			} finally
			{
				con.Close();
				con.Dispose();
			}
		}

		/// <summary>
		/// Отключение от базы данных
		/// </summary>
		/// <param name="connection">Подключение</param>
		public void Disconnection (NpgsqlConnection? connection)
		{
			if (connection == null)
				return;
			try
			{
				connection.Close();
				connection.Dispose();
			} catch (NpgsqlException ex)
			{
				ThrowException(ex.CreateErrorMessage());
			}
		}

		/// <summary>
		/// Обёртка над throw new Exception
		/// </summary>
		/// <param name="message">Сообщение об ошибки</param>
		private  void ThrowException (string message) { throw new Exception(message); }
	}