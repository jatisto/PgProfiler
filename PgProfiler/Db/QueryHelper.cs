using System.Text;

namespace PgProfiler.Db ;

	/// <summary>
	/// Хелпер для работы с построением запросов 
	/// </summary>
	public static class QueryHelper
	{
		/// <summary>
		/// Построение строки комплексного фильтра
		/// </summary>
		/// <param name="complexFilter">Комплексный фильтр</param>
		private static string GetComplexFilter (IEnumerable<ComplexFilterItems>? complexFilter)
		{
			var whereFilter =
				complexFilter!.Select(
					filter =>
						$"{filter.ColumnName} {SqlDictionary.GetComparisonOperators(filter.ComparisonOperator)} '{filter.Value}' {(filter.ConditionalStatements == ConditionalStatements.None ? "" : SqlDictionary.GetConditionalStatements(filter.ConditionalStatements))}")
					.ToList();
			return string.Join($" ", whereFilter);
		}

		/// <summary>
		/// Построение фильтра
		/// </summary>
		/// <param name="filterQuery">Фильтр</param>
		private static IEnumerable<string> GetSelectFilter (Dictionary<string, object>? filterQuery)
		{
			return filterQuery!.Select(item => $"{item.Key} = '{item.Value}'").ToList();
		}

		/// <summary>
		/// Получить список полей для возврата
		/// </summary>
		/// <param name="filterQuery"></param>
		/// <param name="tableName"></param>
		/// <returns></returns>
		private static string GetSelectListParameters (Dictionary<string, string>? filterQuery, string? tableName)
		{
			var tableAlias = CreateDefaultAlias(tableName);
			var result =
				filterQuery!.Select(
					item => $"{(string.IsNullOrEmpty(tableAlias) ? "" : tableAlias + ".")}{item.Key} AS \"{item.Value}\"").ToList();
			return string.Join($",{Environment.NewLine}\t", result);
		}

		/// <summary>
		/// Создать alias по умолчанию
		/// </summary>
		/// <param name="tableName">Наименование таблицы</param>
		private static string? CreateDefaultAlias (string? tableName = null)
		{
			var str = new StringBuilder();
			if (string.IsNullOrEmpty(tableName))
				return null;

			var firstLetterOfEachWord =
				string.Join(" ",
					tableName.Split('_').ToList()
						.ConvertAll(word => string.Concat(word[..1].ToUpper(), word.AsSpan(1))));

			foreach (var c in firstLetterOfEachWord.Trim().Where(c => c == char.ToUpper(c)))
				str.Append(char.ToLower(c));

			return str.Replace(" ", "").ToString();
		}

		/// <summary>
		/// Построить фильтр сортировки
		/// </summary>
		/// <param name="orderBy">Список полей для сортировки</param>
		/// <param name="tableName">Наименование таблицы</param>
		/// <param name="isDesc">Параметр сортировки</param>
		private static string GetOrderByFilter (IReadOnlyList<string>? orderBy, string tableName, bool isDesc = true)
		{
			var alias = CreateDefaultAlias(tableName);
			var strResult = new StringBuilder();
			if (orderBy == null) return strResult.ToString();
			for (var i = 0; i < orderBy.Count; i++)
			{
				if (i != orderBy.Count - 1)
				{
					strResult.Append(alias + "." + orderBy[i] + ", ");
				} else
				{
					strResult.Append(alias + "." + orderBy[i]);
				}
			}
			strResult.Append(!isDesc ? " DESC" : "");
			return strResult.ToString();
		}

		/// <summary>
		/// Конвертирует запрос в строку запроса
		/// </summary>
		/// <param name="filterQuery">Запрос</param>
		public static string? ConvertToQuery (BuilderQuery filterQuery)
		{
			var stringBuilder = new StringBuilder();
			stringBuilder.Append($"{SqlDictionary.GetSqlOperators(SqlOperators.Select)}");
			stringBuilder.AppendLine();
			stringBuilder.Append('\t');
			stringBuilder.Append($"{GetSelectListParameters(filterQuery.Columns, filterQuery.TableName)}");
			stringBuilder.AppendLine();
			stringBuilder.Append("  ");
			stringBuilder.Append($"{SqlDictionary.GetSqlOperators(SqlOperators.From)}");
			stringBuilder.Append(' ');
			stringBuilder.Append(filterQuery.TableName);
			stringBuilder.Append(' ');
			stringBuilder.Append(CreateDefaultAlias(filterQuery.TableName));
			stringBuilder.AppendLine();
			if (filterQuery.ComplexFilter != null && filterQuery.ComplexFilter?.Count() != 0)
			{
				stringBuilder.Append($"{SqlDictionary.GetSqlOperators(SqlOperators.Where)}");
				stringBuilder.Append(' ');
				stringBuilder.Append($"{GetComplexFilter(filterQuery.ComplexFilter)}");
				stringBuilder.AppendLine();
			}
			if (filterQuery.OrderBy != null)
			{
				if (filterQuery.TableName != null)
				{
					stringBuilder.Append($"{SqlDictionary.GetSqlOperators(SqlOperators.OrderBy)}");
					stringBuilder.Append(' ');
					stringBuilder.Append(GetOrderByFilter(filterQuery.OrderBy, filterQuery.TableName, filterQuery.IsDesc));
					stringBuilder.AppendLine();
				}
			}
			if (filterQuery.GroupBy != null)
			{
				stringBuilder.AppendLine();
				stringBuilder.Append($"{SqlDictionary.GetSqlOperators(SqlOperators.OrderBy)}");
				stringBuilder.Append(' ');
				if (filterQuery.TableName != null) stringBuilder.Append(GetOrderByFilter(filterQuery.GroupBy, filterQuery.TableName, filterQuery.IsDesc));
				stringBuilder.AppendLine();
			}
			if (filterQuery.Limit != default)
			{
				stringBuilder.Append(SqlDictionary.GetSqlOperators(SqlOperators.Limit));
				stringBuilder.Append(' ');
				stringBuilder.Append(filterQuery.Limit);
			}
			stringBuilder.AppendLine();
			stringBuilder.Append(';');
			return stringBuilder.ToString();
		}

		/// <summary>
		/// Получить список щапросов
		/// </summary>
		/// <param name="filterQuerys">Запросы</param>
		public static string?[] GetListQuery(IEnumerable<BuilderQuery> filterQuerys) => 
			filterQuerys.Select(ConvertToQuery).ToArray();

		/// <summary>
		/// Получить INSERT запрос
		/// </summary>
		/// <param name="tableName">Наименование таблицы</param>
		/// <param name="columns">Список полей</param>
		/// <param name="values">Список значений</param>
		public static string GetInsertQuery (string tableName, string[] columns, string[] values)
		{
			var query = $"INSERT INTO {tableName} (";
			for (var i = 0; i < columns.Length; i++)
			{
				query += columns[i];
				if (i < columns.Length - 1)
				{
					query += ", ";
				}
			}
			query += ") VALUES (";
			for (var i = 0; i < values.Length; i++)
			{
				query += "'" + values[i] + "'";
				if (i < values.Length - 1)
				{
					query += ", ";
				}
			}
			query += ")";
			return query;
		}

		/// <summary>
		/// Получить UPDATE запрос
		/// </summary>
		/// <param name="tableName">Наименование таблицы</param>
		/// <param name="columns">Список полей</param>
		/// <param name="values">Список значений</param>
		/// <param name="whereColumns">Список полей для Where</param>
		/// <param name="whereValues">Список значение для Where</param>
		public static string GetUpdateQuery (string tableName, string[] columns, string[] values, string[] whereColumns,
			string[] whereValues)
		{
			var query = $"UPDATE {tableName} SET ";
			for (var i = 0; i < columns.Length; i++)
			{
				query += columns[i] + " = '" + values[i] + "'";
				if (i < columns.Length - 1)
				{
					query += ", ";
				}
			}
			query += " WHERE ";
			for (var i = 0; i < whereColumns.Length; i++)
			{
				query += whereColumns[i] + " = '" + whereValues[i] + "'";
				if (i < whereColumns.Length - 1)
				{
					query += " AND ";
				}
			}
			return query;
		}

		/// <summary>
		/// Получить DELETE запрос
		/// </summary>
		/// <param name="tableName">Наименование таблицы</param>
		/// <param name="whereColumns">Список полей для Where</param>
		/// <param name="whereValues">Список значение для Where</param>
		public static string GetDeleteQuery (string tableName, string[] whereColumns, string[] whereValues)
		{
			var query = $"DELETE FROM {tableName} WHERE ";
			for (var i = 0; i < whereColumns.Length; i++)
			{
				query += whereColumns[i] + " = '" + whereValues[i] + "'";
				if (i < whereColumns.Length - 1)
				{
					query += " AND ";
				}
			}
			return query;
		}
	}