using Facepunch.Models;
using Facepunch.Models.Feedback;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Facepunch
{
	public static class Feedback
	{
		public static Action<List<PlayerInfo>> GetPlayersForFeedback;

		public static bool IsOpening
		{
			get;
			private set;
		}

		internal static void Frame()
		{
			if (Input.GetKeyDown(KeyCode.F7))
			{
				Feedback.Open("");
			}
		}

		public static bool Open(string section = "")
		{
			if (Facepunch.Application.Manifest == null)
			{
				Debug.LogWarning("[Feedback] Manifest not loaded");
				return false;
			}
			if (string.IsNullOrEmpty(Facepunch.Application.Manifest.FeedbackUrl))
			{
				Debug.LogWarning("[Feedback] Feedback URL isn't set in manifest");
				return false;
			}
			if (Feedback.IsOpening)
			{
				Debug.LogWarning("[Feedback] Already opening");
				return false;
			}
			GameInfo array = new GameInfo()
			{
				Auth = Facepunch.Application.Integration.Auth
			};
			try
			{
				List<PlayerInfo> playerInfos = new List<PlayerInfo>();
				if (Feedback.GetPlayersForFeedback != null)
				{
					Feedback.GetPlayersForFeedback(playerInfos);
				}
				array.Players = playerInfos.Take<PlayerInfo>(100).ToArray<PlayerInfo>();
			}
			catch (Exception exception)
			{
				Debug.LogWarning("[Feedback] Error getting player list");
				Debug.LogException(exception);
			}
			Dictionary<string, string> strs = new Dictionary<string, string>()
			{
				{ "data", JsonConvert.SerializeObject(array) }
			};
			Debug.Log("[Feedback] Querying Feedback..");
			WebUtil.Post(Facepunch.Application.Manifest.FeedbackUrl, strs, false, (string str) => {
				str = str.Trim(new char[] { '\"' });
				if (!string.IsNullOrEmpty(section))
				{
					str = string.Concat(str, "&type=", section);
				}
				Debug.Log("[Feedback] Got Response");
				Debug.Log(string.Concat("[Feedback] Opening Url: \"", str, "\""));
				UnityEngine.Application.OpenURL(str);
			});
			return true;
		}
	}
}