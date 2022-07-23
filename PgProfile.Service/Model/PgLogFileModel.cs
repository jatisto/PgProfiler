namespace PgProfile.Service.Model ;

	public class PgLogFileModel
	{
		public DateTime DateTimeLog { get; set; }
		public string? Zone { get; set; }
		public string? CodeLog { get; set; }
		public string? TypeOperator { get; set; }
		public string? Content { get; set; }
	}