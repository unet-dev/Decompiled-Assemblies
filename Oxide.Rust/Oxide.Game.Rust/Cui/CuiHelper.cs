using Network;
using Newtonsoft.Json;
using Oxide.Core;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Oxide.Game.Rust.Cui
{
	public static class CuiHelper
	{
		public static bool AddUi(BasePlayer player, List<CuiElement> elements)
		{
			return CuiHelper.AddUi(player, CuiHelper.ToJson(elements, false));
		}

		public static bool AddUi(BasePlayer player, string json)
		{
			bool flag;
			if (player != null)
			{
				flag = player.net;
			}
			else
			{
				flag = false;
			}
			if (!flag || Interface.CallHook("CanUseUI", player, json) != null)
			{
				return false;
			}
			CommunityEntity serverInstance = CommunityEntity.ServerInstance;
			SendInfo sendInfo = new SendInfo()
			{
				connection = player.net.connection
			};
			serverInstance.ClientRPCEx<string>(sendInfo, null, "AddUI", json);
			return true;
		}

		public static bool DestroyUi(BasePlayer player, string elem)
		{
			bool flag;
			if (player != null)
			{
				flag = player.net;
			}
			else
			{
				flag = false;
			}
			if (!flag)
			{
				return false;
			}
			Interface.CallHook("OnDestroyUI", player, elem);
			CommunityEntity serverInstance = CommunityEntity.ServerInstance;
			SendInfo sendInfo = new SendInfo()
			{
				connection = player.net.connection
			};
			serverInstance.ClientRPCEx<string>(sendInfo, null, "DestroyUI", elem);
			return true;
		}

		public static List<CuiElement> FromJson(string json)
		{
			return JsonConvert.DeserializeObject<List<CuiElement>>(json);
		}

		public static Color GetColor(this ICuiColor elem)
		{
			return ColorEx.Parse(elem.Color);
		}

		public static string GetGuid()
		{
			Guid guid = Guid.NewGuid();
			return guid.ToString().Replace("-", String.Empty);
		}

		public static void SetColor(this ICuiColor elem, Color color)
		{
			elem.Color = String.Format("{0} {1} {2} {3}", new Object[] { color.r, color.g, color.b, color.a });
		}

		public static string ToJson(List<CuiElement> elements, bool format = false)
		{
			return JsonConvert.SerializeObject(elements, (format ? Formatting.Indented : Formatting.None), new JsonSerializerSettings()
			{
				DefaultValueHandling = DefaultValueHandling.Ignore
			}).Replace("\\n", "\n");
		}
	}
}