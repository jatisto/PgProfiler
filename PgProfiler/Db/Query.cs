using System.Data;
using MoiLib.ExecuteError;
using MoiLib.ExecuteError.ExecutionResultModelsByType;
using MoiLib.Extension;
using Npgsql;
using PgProfiler.Dto;
using IsolationLevel = System.Data.IsolationLevel;

namespace PgProfiler.Db ;

	/// <summary>
	/// Класс для работы с запросами к БД
	/// </summary>
	public class Query : ExecuteErrorHandling
	{
		/// <summary>
		/// Настройки подключения к БД
		/// </summary>
		private ISettings Settings { get; set; }
		
		/// <summary>
		/// Подключение к БД
		/// </summary>
		private readonly ConnectionNpgSql? _connection;

		/// <summary>
		/// Конструктор класса
		/// </summary>
		public Query (ISettings settings)
		{
			Settings = settings;
			_connection = new ConnectionNpgSql(Settings);
		}

		/// <summary>
		/// Получить данные из БД
		/// </summary>
		/// <param name="query">Запрос</param>
		/// <param name="connection">Подключение</param>
		private  ResultExecuteResult<DataTable> GetDataOnQuery (string? query, NpgsqlConnection? connection)
		{
			return Execute(() =>
			{
				var table = new DataTable();
				using var con = connection;
				using var cmd = new NpgsqlCommand(@query, con);
				var adapter = new NpgsqlDataAdapter();
				adapter.SelectCommand = cmd;
				adapter.Fill(table);
				return new ResultExecuteResult<DataTable>(table);
			});
		}

		/// <summary>
		/// Получить данные из view pg_stat_statements
		/// </summary>
		/// <param name="query"></param>
		public  ResultExecuteResult<List<StatementV1>?> GetDataFromPgStatStatements (BuilderQuery query)
		{
			return Execute(() =>
			{
				var convertToQuery = QueryHelper.ConvertToQuery(query);

				var con = _connection!.Connection();
				var result = GetDataOnQuery(convertToQuery, con);

				var str = new List<StatementV1>();
				var i = 0;
				foreach (DataRow row in result.Result.Rows)
				{
					var cells = row.ItemArray;
					str.Add(new StatementV1()
					{
						Index = i,
						DatName = (string) cells[0]!,
						DbId = (uint) cells[1]!,
						UserId = (uint) cells[2]!,
						QueryId = (long) cells[3]!,
						Query = (string) cells[4]!,
						QueryStart = (DateTime) cells[5]!,
						Calls = (long) cells[6]!,
						TimeSecExec = (double) cells[7]!,
						TimeSecPlan = (double) cells[8]!,
						Plans = (long) cells[9]!,
						Rows = (long) cells[10]!,
						SheredBalkHitReadWritten = (long) cells[11]!,
					});
					i++;
				}

				return new ResultExecuteResult<List<StatementV1>?>(str);
			});
		}

		/// <summary>
		/// Получить данные из view pg_stat_statements
		/// </summary>
		/// <param name="func">Обработка запроса</param>
		/// <param name="query">Запрос</param>
		public  ValueTask<ResultExecuteResult<T>> GetDataFromPgStatStatementsFunk<T> (Func<DataRowCollection, T> func, BuilderQuery query)
		where T: ExecuteResult, new()
		{
			var existsPg = IsExistsView("pg_stat_statements");
			var existsV1 = IsExistsView("statement_v1");
			
			if (!existsPg.IsOk || !existsV1.IsOk)
				return ValueTask.FromResult(new ResultExecuteResult<T>()
				{
					Result = new T(),
					Message = $"{(existsPg.IsOk ? "" : existsPg.Message + " ")}{(existsV1.IsOk ? "" : existsV1.Message)}"
				});
			
			return ValueTask.FromResult(new ResultExecuteResult<T>(Execute(() =>
			{
				var convertToQuery = QueryHelper.ConvertToQuery(query);

				var con = _connection!.Connection();
				var result = GetDataOnQuery(convertToQuery, con);

				return func(result.Result.Rows);
			})));
		}
		
		/// <summary>
		/// Установка view pg_stat_statements и statements_v1
		/// </summary>
		public  void InstallPgStatStatements ()
		{
			Execute(() =>
			{
				var queries = new []
				{
					"CREATE EXTENSION pg_stat_statements;",
					@"ALTER SYSTEM SET SHARED_PRELOAD_LIBRARIES = 'pg_stat_statements';",
					"SET pg_stat_statements.track = 'all';",
					"SET log_statement = 'all';",
					"SET compute_query_id = 'on';",
					@"CREATE VIEW statement_v1
								  (datname, dbid, userid, queryid, query, query_start, calls, time_sec_exec, time_sec_plan, plans, rows,
								   shered_balk_hit_read_written)
								  AS
								  SELECT db.datname,
									     pg.dbid,
									     pg.userid,
									     pg.queryid,
									     pg.query,
									     pa.query_start,
									     pg.calls,
									     round(pg.total_exec_time) / 1000::double precision                AS time_sec_exec,
									     round(pg.total_plan_time) / 1000::double precision                AS time_sec_plan,
									     pg.plans,
									     pg.rows,
									     pg.shared_blks_hit + pg.shared_blks_read + pg.shared_blks_written AS shered_balk_hit_read_written
								    FROM pg_stat_statements pg
										     JOIN pg_database db ON pg.dbid = db.oid
										     JOIN pg_authid auth ON pg.userid = auth.oid
										     JOIN pg_stat_activity pa ON db.oid = pa.datid AND auth.oid = pa.usesysid
								  ;
								  ALTER TABLE statement_v1
									  OWNER TO postgres;"
				};
				var con = _connection!.Connection();
				if (con == null)
					return;

				foreach (var builderQuery in queries)
				{
					using var command = new NpgsqlCommand(builderQuery, con);
					command.CommandTimeout = 120;
					command.ExecuteNonQuery();
				}
			});
		}
		/// <summary>
		/// Установка view pg_stat_statements и statements_v1
		/// </summary>
		public  void DeletePgStatStatements ()
		{
			Execute(() =>
			{
				const string query = "DROP EXTENSION pg_stat_statements CASCADE;";
				var con = _connection!.Connection();
				if (con == null)
					return;
				using var transaction = con.BeginTransaction(IsolationLevel.Serializable);
				try
				{
					using var command = new NpgsqlCommand(query, con);
					command.CommandTimeout = 120;
					command.ExecuteNonQuery();
					transaction.Commit();
				} catch (Exception e)
				{
					transaction.Rollback();
				}
			});
		}
		
		/// <summary>
		/// Отчистка логов из базы данных
		/// </summary>
		/// <param name="dbId">Id базы данных для которой требуеться отчистка</param>
		public  BoolExecuteResult PgStatStatementsReset (uint dbId)
		{
			var exists = IsExistsView("pg_stat_statements");
			try
			{
				if (!exists.Result) return exists;
				
				var query = $@"SELECT pg_stat_statements_reset(dbid := {dbId});";
				var con = _connection!.Connection();
				using var cmd = new NpgsqlCommand(query, con);
				cmd.ExecuteNonQuery();
				return exists;
			} catch (Exception e)
			{
				 return new BoolExecuteResult(false, e.CreateErrorMessage());
			}
		}

		/// <summary>
		/// Проверка на существование view
		/// </summary>
		/// <param name="nameView">Наименования View</param>
		public  BoolExecuteResult IsExistsView (string nameView)
		{
			return Execute(() =>
			{
				var connection = _connection!.Connection();
				var query = @$"SELECT 1 WHERE exists(SELECT viewname FROM pg_views WHERE viewname = '{nameView}');";
				var result = 0;
				try
				{
					var resultTb = GetDataOnQuery(query, connection);
					foreach (DataRow row in resultTb.Result.Rows)
					{
						var cells = row.ItemArray;
						result = (int) cells[0]!;
					}

					return result == 1 ? new BoolExecuteResult(true, $"Представлние [{nameView}] установлен.") : new BoolExecuteResult(false, $"Представлние [{nameView}] не установлен.");
				}
				catch(NpgsqlException ex)
				{
					return new BoolExecuteResult(false, ex.CreateErrorMessage());
				}
				finally
				{
					connection?.Close();
				}
			});
		}
	}