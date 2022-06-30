namespace PgProfiler.Dto ;


	/// <summary>
	/// Класс днных запроса для получения данных из лога PostgreSQL
	/// </summary>
	public class QueryList
	{
		/// <summary>
		/// Индекс записи
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// Наименование базы данных
		/// </summary>
		public string? DatName { get; set; }

		/// <summary>
		/// OID базы данных, в которой выполнялся оператор
		/// </summary>
		public uint? DbId { get; set; }

		/// <summary>
		/// OID пользователя, выполнявшего оператор
		/// </summary>
		public uint? UserId { get; set; }

		/// <summary>
		/// Внутренний хеш-код, вычисленный по дереву разбора оператора
		/// </summary>
		public long QueryId { get; set; }

		/// <summary>
		/// Текст, представляющий оператор (Запрос)
		/// </summary>
		public string? Query { get; set; }

		/// <summary>
		/// Время начала выполнения активного в данный момент запроса, или, если state не active, то время начала выполнения последнего запроса
		/// </summary>
		public DateTime QueryStart { get; set; }

		/// <summary>
		/// Число выполнений
		/// </summary>
		public long Calls { get; set; }

		/// <summary>
		/// Общее время, затраченное на выполнение оператора, в миллисекундах (round(pg.total_exec_time) / 1000::double precision)
		/// </summary>
		public double? TimeSecExec { get; set; }

		/// <summary>
		/// Общее время, затраченное на планирование этого оператора в миллисекундах (если включён параметр pg_stat_statements.track_planning, иначе 0)
		/// round(pg.total_plan_time) / 1000::double precision
		/// </summary>
		public double? TimeSecPlan { get; set; }

		/// <summary>
		/// Число операций планирования этого оператора (если включён параметр pg_stat_statements.track_planning, иначе 0)
		/// </summary>
		public long? Plans { get; set; }

		/// <summary>
		/// Общее число строк, полученных или затронутых оператором
		/// </summary>
		public long? Rows { get; set; }
		
		/// <summary>
		/// Общее число попаданий в разделяемый кеш блоков для данного оператора +
		/// Общее число чтений разделяемых блоков для данного оператора +
		/// Общее число разделяемых блоков, записанных данным оператором
		/// </summary>
		public long? SheredBalkHitReadWritten { get; set; }
	}