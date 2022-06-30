namespace PgProfiler.Db ;

	/// <summary>
	/// Клас для работы с преобразование Enum в строку
	/// </summary>
	public static class SqlDictionary
	{
		/// <summary>
		/// Сопостовление SQL-операторов
		/// </summary>
		private static readonly Dictionary<SqlOperators, string> SqlOperatorsDic = new()
		{
			{SqlOperators.Where, "WHERE"},
			{SqlOperators.From, "FROM"},
			{SqlOperators.OrderBy, "ORDER BY"},
			{SqlOperators.Limit, "LIMIT"},
			{SqlOperators.Join, "JOIN"},
			{SqlOperators.LeftJoin, "LEFT JOIN"},
			{SqlOperators.OtherJoin, "OTHER JOIN"},
			{SqlOperators.CrossJoin, "CROSS JOIN"},
			{SqlOperators.CrossApplyJoin, "CROSS APPLY JOIN"},
			{SqlOperators.RightJoin, "RIGHT JOIN"},
			{SqlOperators.Select, "SELECT"},
			{SqlOperators.GroupBy, "GROUP BY"},
			{SqlOperators.Having, "HAVING"},
		};

		/// <summary>
		/// Сопостовление Операторов сравнения
		/// </summary>
		private static readonly Dictionary<ComparisonOperators, string> ComparisonOperatorsDic = new()
		{
			{ComparisonOperators.In, "IN"},
			{ComparisonOperators.NotIn, "NOT IN"},
			{ComparisonOperators.Like, "LIKE"},
			{ComparisonOperators.NotLike, "NOT LIKE"},
			{ComparisonOperators.Equal, "="},
			{ComparisonOperators.NotEqual, "!="}
		};
		
		/// <summary>
		/// Сопостовление Условных операторов
		/// </summary>
		private static readonly Dictionary<ConditionalStatements, string> ConditionalStatementsDic = new()
		{
			{ConditionalStatements.And, "AND"},
			{ConditionalStatements.Or, "OR"}
		};

		/// <summary>
		/// Сопостовление Операторов выборки
		/// </summary>
		private static readonly Dictionary<SortingOperators, string> SortingOperatorsDic = new()
		{
			{SortingOperators.Ask, "ASC"},
			{SortingOperators.Desc, "DESC"}
		};

		/// <summary>
		/// Получить значение по ключу (SQL-операторы)
		/// </summary>
		public static string GetSqlOperators (SqlOperators sqlOperators) => SqlOperatorsDic[sqlOperators];

		/// <summary>
		/// Получить значение по ключу (Операторы сравнения)
		/// </summary>
		public static string GetComparisonOperators (ComparisonOperators comparisonOperators)
			=> ComparisonOperatorsDic[comparisonOperators];

		/// <summary>
		/// Получить значение по ключу (Условные операторы)
		/// </summary>
		public static string GetConditionalStatements (ConditionalStatements conditionalStatements)
			=> ConditionalStatementsDic[conditionalStatements];
		
		/// <summary>
		/// Получить значение по ключу (Операторы выборки)
		/// </summary>
		public static string GetSortingOperators (SortingOperators sortingOperators)
			=> SortingOperatorsDic[sortingOperators];
	}