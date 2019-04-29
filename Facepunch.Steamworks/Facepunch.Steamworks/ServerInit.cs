using System;
using System.Net;

namespace Facepunch.Steamworks
{
	public class ServerInit
	{
		public IPAddress IpAddress;

		public ushort SteamPort;

		public ushort GamePort = 27015;

		public ushort QueryPort = 27016;

		public bool Secure = true;

		public string VersionString = "2.0.0.0";

		public string ModDir = "unset";

		public string GameDescription = "unset";

		public ServerInit(string modDir, string gameDesc)
		{
			this.ModDir = modDir;
			this.GameDescription = gameDesc;
		}

		public ServerInit QueryShareGamePort()
		{
			this.QueryPort = 65535;
			return this;
		}

		public ServerInit RandomSteamPort()
		{
			this.SteamPort = (ushort)(new Random()).Next(10000, 60000);
			return this;
		}
	}
}