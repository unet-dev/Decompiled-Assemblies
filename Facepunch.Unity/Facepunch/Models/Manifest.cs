using Newtonsoft.Json;
using System;

namespace Facepunch.Models
{
	public class Manifest
	{
		public Facepunch.Models.Manifest.NewsInfo News;

		public Facepunch.Models.Manifest.ServersInfo Servers;

		public string ExceptionReportingUrl;

		public string BenchmarkUrl;

		public string AnalyticUrl;

		public string DatabaseUrl;

		public string LeaderboardUrl;

		public string FeedbackUrl;

		public Facepunch.Models.Manifest.Administrator[] Administrators;

		public Manifest()
		{
		}

		internal static Facepunch.Models.Manifest FromJson(string text)
		{
			Facepunch.Models.Manifest manifest = JsonConvert.DeserializeObject<Facepunch.Models.Manifest>(text);
			if (manifest == null)
			{
				return null;
			}
			if (manifest.Servers == null)
			{
				return null;
			}
			return manifest;
		}

		public class Administrator
		{
			public string UserId;

			public string Level;

			public Administrator()
			{
			}
		}

		public class NewsInfo
		{
			public Facepunch.Models.Manifest.NewsInfo.StatusInfo[] Status;

			public NewsInfo()
			{
			}

			public class StatusInfo
			{
				public DateTime DateTime;

				public string Content;

				public string Type;

				public StatusInfo()
				{
				}
			}
		}

		public class ServerDesc
		{
			public string Address;

			public int Port;

			public ServerDesc()
			{
			}
		}

		public class ServersInfo
		{
			public Facepunch.Models.Manifest.ServerDesc[] Official;

			public string[] Banned;

			public ServersInfo()
			{
			}

			public bool IsBannedServer(string ip)
			{
				string[] banned = this.Banned;
				for (int i = 0; i < (int)banned.Length; i++)
				{
					if (ip.StartsWith(banned[i]))
					{
						return true;
					}
				}
				return false;
			}
		}
	}
}