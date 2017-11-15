namespace Shuvaev.IDP
{
	public class AppOptions
	{
		public ConnectionStringsCfg ConnectionStrings { get; set; }
		public class ConnectionStringsCfg
		{
			public string IdpUserDbConnection { get; set; }
		}
	}
}