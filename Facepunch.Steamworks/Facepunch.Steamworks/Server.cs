using Facepunch.Steamworks.Interop;
using SteamNative;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;

namespace Facepunch.Steamworks
{
	public class Server : BaseSteamworks
	{
		private bool _dedicatedServer;

		private int _maxplayers;

		private int _botcount;

		private string _mapname;

		private string _modDir = "";

		private string _product = "";

		private string _gameDescription = "";

		private string _serverName = "";

		private bool _passworded;

		private string _gametags = "";

		private Dictionary<string, string> KeyValue = new Dictionary<string, string>();

		public ServerAuth Auth
		{
			get;
			internal set;
		}

		public int AutomaticHeartbeatRate
		{
			set
			{
				this.native.gameServer.SetHeartbeatInterval(value);
			}
		}

		public bool AutomaticHeartbeats
		{
			set
			{
				this.native.gameServer.EnableHeartbeats(value);
			}
		}

		public int BotCount
		{
			get
			{
				return this._botcount;
			}
			set
			{
				if (this._botcount == value)
				{
					return;
				}
				this.native.gameServer.SetBotPlayerCount(value);
				this._botcount = value;
			}
		}

		public bool DedicatedServer
		{
			get
			{
				return this._dedicatedServer;
			}
			set
			{
				if (this._dedicatedServer == value)
				{
					return;
				}
				this.native.gameServer.SetDedicatedServer(value);
				this._dedicatedServer = value;
			}
		}

		public string GameDescription
		{
			get
			{
				return this._gameDescription;
			}
			internal set
			{
				if (this._gameDescription == value)
				{
					return;
				}
				this.native.gameServer.SetGameDescription(value);
				this._gameDescription = value;
			}
		}

		public string GameTags
		{
			get
			{
				return this._gametags;
			}
			set
			{
				if (this._gametags == value)
				{
					return;
				}
				this.native.gameServer.SetGameTags(value);
				this._gametags = value;
			}
		}

		public static Server Instance
		{
			get;
			private set;
		}

		internal override bool IsGameServer
		{
			get
			{
				return true;
			}
		}

		public bool LoggedOn
		{
			get
			{
				return this.native.gameServer.BLoggedOn();
			}
		}

		public string MapName
		{
			get
			{
				return this._mapname;
			}
			set
			{
				if (this._mapname == value)
				{
					return;
				}
				this.native.gameServer.SetMapName(value);
				this._mapname = value;
			}
		}

		public int MaxPlayers
		{
			get
			{
				return this._maxplayers;
			}
			set
			{
				if (this._maxplayers == value)
				{
					return;
				}
				this.native.gameServer.SetMaxPlayerCount(value);
				this._maxplayers = value;
			}
		}

		public string ModDir
		{
			get
			{
				return this._modDir;
			}
			internal set
			{
				if (this._modDir == value)
				{
					return;
				}
				this.native.gameServer.SetModDir(value);
				this._modDir = value;
			}
		}

		public bool Passworded
		{
			get
			{
				return this._passworded;
			}
			set
			{
				if (this._passworded == value)
				{
					return;
				}
				this.native.gameServer.SetPasswordProtected(value);
				this._passworded = value;
			}
		}

		public string Product
		{
			get
			{
				return this._product;
			}
			internal set
			{
				if (this._product == value)
				{
					return;
				}
				this.native.gameServer.SetProduct(value);
				this._product = value;
			}
		}

		public IPAddress PublicIp
		{
			get
			{
				uint publicIP = this.native.gameServer.GetPublicIP();
				if (publicIP == 0)
				{
					return null;
				}
				return Utility.Int32ToIp(publicIP);
			}
		}

		public ServerQuery Query
		{
			get;
			internal set;
		}

		public string ServerName
		{
			get
			{
				return this._serverName;
			}
			set
			{
				if (this._serverName == value)
				{
					return;
				}
				this.native.gameServer.SetServerName(value);
				this._serverName = value;
			}
		}

		public ServerStats Stats
		{
			get;
			internal set;
		}

		public Server(uint appId, ServerInit init) : base(appId)
		{
			if (Server.Instance != null)
			{
				throw new Exception("Only one Facepunch.Steamworks.Server can exist - dispose the old one before trying to create a new one.");
			}
			Server.Instance = this;
			this.native = new NativeInterface();
			uint num = 0;
			if (init.SteamPort == 0)
			{
				init.RandomSteamPort();
			}
			if (init.IpAddress != null)
			{
				num = init.IpAddress.IpToInt32();
			}
			if (!this.native.InitServer(this, num, init.SteamPort, init.GamePort, init.QueryPort, (init.Secure ? 3 : 2), init.VersionString))
			{
				this.native.Dispose();
				this.native = null;
				Server.Instance = null;
				return;
			}
			SteamNative.Callbacks.RegisterCallbacks(this);
			base.SetupCommonInterfaces();
			this.native.gameServer.EnableHeartbeats(true);
			this.MaxPlayers = 32;
			this.BotCount = 0;
			this.Product = string.Format("{0}", base.AppId);
			this.ModDir = init.ModDir;
			this.GameDescription = init.GameDescription;
			this.Passworded = false;
			this.DedicatedServer = true;
			this.Query = new ServerQuery(this);
			this.Stats = new ServerStats(this);
			this.Auth = new ServerAuth(this);
			this.Update();
		}

		public override void Dispose()
		{
			if (this.disposed)
			{
				return;
			}
			if (this.Query != null)
			{
				this.Query = null;
			}
			if (this.Stats != null)
			{
				this.Stats = null;
			}
			if (this.Auth != null)
			{
				this.Auth = null;
			}
			if (Server.Instance == this)
			{
				Server.Instance = null;
			}
			base.Dispose();
		}

		~Server()
		{
			this.Dispose();
		}

		public void ForceHeartbeat()
		{
			this.native.gameServer.ForceHeartbeat();
		}

		public void LogOnAnonymous()
		{
			this.native.gameServer.LogOnAnonymous();
			this.ForceHeartbeat();
		}

		public void SetKey(string Key, string Value)
		{
			if (!this.KeyValue.ContainsKey(Key))
			{
				this.KeyValue.Add(Key, Value);
			}
			else
			{
				if (this.KeyValue[Key] == Value)
				{
					return;
				}
				this.KeyValue[Key] = Value;
			}
			this.native.gameServer.SetKeyValue(Key, Value);
		}

		public override void Update()
		{
			if (!base.IsValid)
			{
				return;
			}
			this.native.api.SteamGameServer_RunCallbacks();
			base.Update();
		}

		public void UpdatePlayer(ulong steamid, string name, int score)
		{
			this.native.gameServer.BUpdateUserData(steamid, name, (uint)score);
		}
	}
}