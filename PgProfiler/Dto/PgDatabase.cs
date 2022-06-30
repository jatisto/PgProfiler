namespace PgProfiler.Dto ;

	/// <summary>
	/// Класс БД
	/// </summary>
	public class PgDatabase
	{
		/// <summary>
		/// Id БД
		/// </summary>
		public uint Oid { get; set; }
		
		/// <summary>
		/// Наименование БД
		/// </summary>
		public string? DatName { get; set; }
	}