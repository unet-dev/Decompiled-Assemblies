using Network;
using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public static class ArgEx
	{
		public static BasePlayer GetPlayer(this ConsoleSystem.Arg arg, int iArgNum)
		{
			string str = arg.GetString(iArgNum, "");
			if (str == null)
			{
				return null;
			}
			return BasePlayer.Find(str);
		}

		public static BasePlayer GetPlayerOrSleeper(this ConsoleSystem.Arg arg, int iArgNum)
		{
			string str = arg.GetString(iArgNum, "");
			if (str == null)
			{
				return null;
			}
			BasePlayer basePlayer = BasePlayer.Find(str);
			if (basePlayer)
			{
				return basePlayer;
			}
			return BasePlayer.FindSleeping(str);
		}

		public static BasePlayer GetSleeper(this ConsoleSystem.Arg arg, int iArgNum)
		{
			string str = arg.GetString(iArgNum, "");
			if (str == null)
			{
				return null;
			}
			return BasePlayer.FindSleeping(str);
		}

		public static BasePlayer Player(this ConsoleSystem.Arg arg)
		{
			if (arg.Connection == null)
			{
				return null;
			}
			return arg.Connection.player as BasePlayer;
		}
	}
}