namespace PgProfiler ;

	public static class Constants
	{
		public const string SettingFileName = "settings.json";
		public const string PostfixFolderName = "pg_profiler_settings";
		public const string StatementV1 = "statement_v1";
		public const string PgDatabase = "pg_database";
		public const string PathToLog = "C:\\Program Files\\PostgreSQL\\14\\data\\log";
		public const string CodeEditor = "notepad++.exe";
		
		public const string ServerDb = "localhost";
		public const string PortDb = "5432";
		public const string Login = "postgres";
		public const string Password = "123456";
		public const string NameDb = "postgres";
		public const bool IsActive = true;
		public static readonly string Uid = Guid.NewGuid().ToString();
	}