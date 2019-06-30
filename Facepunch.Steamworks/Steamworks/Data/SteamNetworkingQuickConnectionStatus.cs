using System;

namespace Steamworks.Data
{
	public struct SteamNetworkingQuickConnectionStatus
	{
		public ConnectionState state;

		public int ping;

		public float connectionQualityLocal;

		public float connectionQualityRemote;

		public float outPacketsPerSecond;

		public float outBytesPerSecond;

		public float inPacketsPerSecond;

		public float inBytesPerSecond;

		public int sendRateBytesPerSecond;

		public int pendingUnreliable;

		public int pendingReliable;

		public int sentUnackedReliable;

		public long queueTime;

		private uint[] reserved;
	}
}