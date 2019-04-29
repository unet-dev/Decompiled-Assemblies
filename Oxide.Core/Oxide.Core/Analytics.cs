using Oxide.Core.Libraries;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Oxide.Core
{
	public static class Analytics
	{
		private readonly static WebRequests Webrequests;

		private readonly static Oxide.Core.Plugins.PluginManager PluginManager;

		private readonly static Oxide.Core.Libraries.Covalence.Covalence Covalence;

		private readonly static Oxide.Core.Libraries.Lang Lang;

		private const string trackingId = "UA-48448359-3";

		private const string url = "https://www.google-analytics.com/collect";

		private readonly static string Identifier;

		static Analytics()
		{
			Analytics.Webrequests = Interface.Oxide.GetLibrary<WebRequests>(null);
			Analytics.PluginManager = Interface.Oxide.RootPluginManager;
			Analytics.Covalence = Interface.Oxide.GetLibrary<Oxide.Core.Libraries.Covalence.Covalence>(null);
			Analytics.Lang = Interface.Oxide.GetLibrary<Oxide.Core.Libraries.Lang>(null);
			Analytics.Identifier = string.Format("{0}:{1}", Analytics.Covalence.Server.Address, Analytics.Covalence.Server.Port);
		}

		public static void Collect()
		{
			Analytics.SendPayload(string.Concat(string.Concat(new string[] { "v=1&tid=UA-48448359-3&cid=", Analytics.Identifier, "&t=screenview&cd=", Analytics.Covalence.Game, "+", Analytics.Covalence.Server.Version }), string.Format("&an=Oxide&av={0}&ul={1}", OxideMod.Version, Analytics.Lang.GetServerLanguage())));
		}

		public static void Event(string category, string action)
		{
			Analytics.SendPayload(string.Concat(new string[] { "v=1&tid=UA-48448359-3&cid=", Analytics.Identifier, "&t=event&ec=", category, "&ea=", action }));
		}

		public static void SendPayload(string payload)
		{
			Dictionary<string, string> strs = new Dictionary<string, string>()
			{
				{ "User-Agent", string.Format("Oxide/{0} ({1}; {2})", OxideMod.Version, Environment.OSVersion, Environment.OSVersion.Platform) }
			};
			Analytics.Webrequests.Enqueue("https://www.google-analytics.com/collect", Uri.EscapeUriString(payload), (int code, string response) => {
			}, null, RequestMethod.POST, strs, 0f);
		}
	}
}