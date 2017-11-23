namespace Shuvaev.IDP
{
	public class AppOptions
	{
		public ConnectionStringsCfg ConnectionStrings { get; set; }
		public FacebookConfiguration Facebook { get; set; }

		public class FacebookConfiguration
		{
			public string AppId { get; set; }
			public string AppSecret { get; set; }
		}

		public class ConnectionStringsCfg
		{
			public string IdpUserDbConnection { get; set; }
		}
	}
}