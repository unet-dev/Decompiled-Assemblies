using Network;
using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public static class NetworkPacketEx
	{
		public static BasePlayer Player(this Message v)
		{
			if (v.connection == null)
			{
				return null;
			}
			return v.connection.player as BasePlayer;
		}
	}
}