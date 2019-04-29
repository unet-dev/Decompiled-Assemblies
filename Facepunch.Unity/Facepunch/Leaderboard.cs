using Facepunch.Models;
using Facepunch.Models.Leaderboard;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Facepunch
{
	public static class Leaderboard
	{
		public static void GetPage(string board, string country = null, string city = null, int skip = 0, int take = 20, bool desc = true, string[] friends = null, Action<Entry[]> result = null)
		{
			if (Facepunch.Application.Manifest == null || string.IsNullOrEmpty(Facepunch.Application.Manifest.LeaderboardUrl))
			{
				return;
			}
			if (Facepunch.Application.Integration.DebugOutput)
			{
				Debug.Log(string.Concat("[Leaderboard] GetPage ", board));
			}
			Auth auth = Facepunch.Application.Integration.Auth;
			if (auth == null)
			{
				return;
			}
			string str = Facepunch.Application.Manifest.LeaderboardUrl.Replace("{action}", "page");
			str = string.Concat(str, "&type=", WebUtil.Escape(auth.Type));
			str = string.Concat(str, "&board=", WebUtil.Escape(board));
			str = string.Concat(str, string.Format("&desc={0}", desc));
			str = string.Concat(str, string.Format("&skip={0}", skip));
			str = string.Concat(str, string.Format("&take={0}", take));
			if (!string.IsNullOrEmpty(country))
			{
				str = string.Concat(str, "&country=", WebUtil.Escape(country));
			}
			if (!string.IsNullOrEmpty(city))
			{
				str = string.Concat(str, "&city=", WebUtil.Escape(city));
			}
			if (friends != null && friends.Length != 0)
			{
				str = string.Concat(str, "&friends=", WebUtil.Escape(string.Join(";", friends)));
			}
			WebUtil.Get(str, (string txt) => {
				if (result != null)
				{
					Entry[] entryArray = JsonConvert.DeserializeObject<Entry[]>(txt);
					result(entryArray);
				}
			});
		}

		public static void GetRank(string board, string userid, bool desc = true, string[] friends = null, Action<Rank> result = null)
		{
			if (Facepunch.Application.Manifest == null || string.IsNullOrEmpty(Facepunch.Application.Manifest.LeaderboardUrl))
			{
				return;
			}
			if (Facepunch.Application.Integration.DebugOutput)
			{
				Debug.Log(string.Concat("[Leaderboard] Place ", board, " ", userid));
			}
			Auth auth = Facepunch.Application.Integration.Auth;
			if (auth == null)
			{
				return;
			}
			string str = Facepunch.Application.Manifest.LeaderboardUrl.Replace("{action}", "place");
			str = string.Concat(str, "&type=", WebUtil.Escape(auth.Type));
			str = string.Concat(str, "&userid=", WebUtil.Escape(userid));
			str = string.Concat(str, "&board=", WebUtil.Escape(board));
			str = string.Concat(str, string.Format("&desc={0}", desc));
			if (friends != null && friends.Length != 0)
			{
				str = string.Concat(str, "&friends=", WebUtil.Escape(string.Join(";", friends)));
			}
			WebUtil.Get(str, (string txt) => {
				if (result != null)
				{
					Rank rank = JsonConvert.DeserializeObject<Rank>(txt);
					result(rank);
				}
			});
		}

		public static void Insert(string board, float score, bool OnlyIfLower = false, bool OnlyIfHigher = false, string extra = "", Action<string> result = null)
		{
			if (Facepunch.Application.Manifest == null || string.IsNullOrEmpty(Facepunch.Application.Manifest.LeaderboardUrl))
			{
				return;
			}
			if (Facepunch.Application.Integration.DebugOutput)
			{
				Debug.Log(string.Format("[Leaderboard] Insert {0} {1}", board, score));
			}
			Add add = new Add()
			{
				Parent = board,
				Auth = Facepunch.Application.Integration.Auth,
				Score = score,
				ReplaceIfHigher = OnlyIfHigher,
				ReplaceIfLower = OnlyIfLower,
				Extra = extra
			};
			Dictionary<string, string> strs = new Dictionary<string, string>()
			{
				{ "data", JsonConvert.SerializeObject(add) }
			};
			WebUtil.Post(Facepunch.Application.Manifest.LeaderboardUrl.Replace("{action}", "add"), strs, false, result);
		}
	}
}