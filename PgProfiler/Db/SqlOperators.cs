namespace PgProfiler.Db ;

	/// <summary>
	/// SQL-операторы
	/// </summary>
	public enum SqlOperators
	{
		Select,
		From,
		Where,
		Join,
		LeftJoin,
		RightJoin,
		OtherJoin,
		CrossJoin,
		CrossApplyJoin,
		OrderBy,
		Limit,
		GroupBy,
		Having,
		None
	}
	
	
	/// <summary>
	/// Операторы сравнения
	/// </summary>
	public enum ComparisonOperators
	{
		Equal,
		NotEqual,
		Greater,
		GreaterOrEqual,
		Less,
		LessOrEqual,
		Like,
		NotLike,
		In,
		NotIn,
		Between,
		NotBetween,
		IsNull,
		IsNotNull,
		None
	}

	/// <summary>
	/// Условные операторы
	/// </summary>
	public enum ConditionalStatements
	{
		And,
		Or,
		Between,
		None
	}

	/// <summary>
	/// Операторы выборки
	/// </summary>
	public enum SortingOperators
	{
		Ask,
		Desc
	}