namespace PgProfiler.Db ;

	/// <summary>
	/// Класс для построения запросов к БД
	/// </summary>
	public class BuilderQuery
	{
		/// <summary>
		/// Фильтр полей
		/// </summary>
		public Dictionary<string, object>? FieldsFilter { get; set; }
		
		/// <summary>
		/// Колонки для выборки
		/// </summary>
		public Dictionary<string, string>? Columns { get; set; }
		
		/// <summary>
		/// Сортировка
		/// </summary>
		public string[]? OrderBy { get; set; }
		
		/// <summary>
		/// Групировка
		/// </summary>
		public string[]? GroupBy { get; set; }
		
		/// <summary>
		/// Наименование таблицы
		/// </summary>
		public string? TableName { get; set; }
		
		/// <summary>
		/// Комплексный фильтр
		/// </summary>
		public IEnumerable<ComplexFilterItems>? ComplexFilter { get; set; }
		
		/// <summary>
		/// Лимит выборки
		/// </summary>
		public int? Limit { get; set; }
		
		/// <summary>
		/// Параметры сортировки
		/// </summary>
		public bool IsDesc { get; set; }
	}
	
	/// <summary>
	/// Класс для построения комплексных запросов
	/// </summary>
	public class ComplexFilterItems
	{
		/// <summary>
		/// Наименование поля
		/// </summary>
		public string? ColumnName { get; set; }
		
		/// <summary>
		/// Оператор сравнения
		/// </summary>
		public ComparisonOperators ComparisonOperator { get; set; } = ComparisonOperators.None;
		
		/// <summary>
		/// Значение поля
		/// </summary>
		public object? Value { get; set; }
		
		/// <summary>
		/// Условный оператор
		/// </summary>
		public ConditionalStatements ConditionalStatements { get; set; } = ConditionalStatements.None;
	}