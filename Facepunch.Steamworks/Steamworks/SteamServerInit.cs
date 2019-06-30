using System;
using System.Net;

namespace Steamworks
{
	public struct SteamServerInit
	{
		public IPAddress IpAddress;

		public ushort SteamPort;

		public ushort GamePort;

		public ushort QueryPort;

		public bool Secure;

		public string VersionString;

		public string ModDir;

		public string GameDescription;

		public bool DedicatedServer;

		public SteamServerInit(string modDir, string gameDesc)
		{
			this.DedicatedServer = true;
			this.ModDir = modDir;
			this.GameDescription = gameDesc;
			this.GamePort = 27015;
			this.QueryPort = 27016;
			this.Secure = true;
			this.VersionString = "1.0.0.0";
			this.IpAddress = null;
			this.SteamPort = 0;
		}

		public SteamServerInit WithQueryShareGamePort()
		{
			this.QueryPort = 65535;
			return this;
		}

		public SteamServerInit WithRandomSteamPort()
		{
			this.SteamPort = (ushort)(new Random()).Next(10000, 60000);
			return this;
		}
	}
}