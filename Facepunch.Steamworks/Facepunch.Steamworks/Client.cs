using Facepunch.Steamworks.Interop;
using SteamNative;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Facepunch.Steamworks
{
	public class Client : BaseSteamworks, IDisposable
	{
		private Facepunch.Steamworks.Auth _auth;

		private Facepunch.Steamworks.Friends _friends;

		private Facepunch.Steamworks.Lobby _lobby;

		private Facepunch.Steamworks.Screenshots _screenshots;

		public Facepunch.Steamworks.Achievements Achievements
		{
			get;
			private set;
		}

		public Facepunch.Steamworks.App App
		{
			get;
			private set;
		}

		public Facepunch.Steamworks.Auth Auth
		{
			get
			{
				if (this._auth == null)
				{
					this._auth = new Facepunch.Steamworks.Auth()
					{
						client = this
					};
				}
				return this._auth;
			}
		}

		public string[] AvailableLanguages
		{
			get;
		}

		public string BetaName
		{
			get;
			private set;
		}

		public int BuildId
		{
			get;
			private set;
		}

		public string CurrentCountry
		{
			get;
		}

		public string CurrentLanguage
		{
			get;
		}

		public Facepunch.Steamworks.Friends Friends
		{
			get
			{
				if (this._friends == null)
				{
					this._friends = new Facepunch.Steamworks.Friends(this);
				}
				return this._friends;
			}
		}

		public DirectoryInfo InstallFolder
		{
			get;
			private set;
		}

		public static Client Instance
		{
			get;
			private set;
		}

		public bool IsCybercafe
		{
			get
			{
				return this.native.apps.BIsCybercafe();
			}
		}

		public bool IsLoggedOn
		{
			get
			{
				return this.native.user.BLoggedOn();
			}
		}

		public bool IsLowViolence
		{
			get
			{
				return this.native.apps.BIsLowViolence();
			}
		}

		public bool IsSubscribed
		{
			get
			{
				return this.native.apps.BIsSubscribed();
			}
		}

		public bool IsSubscribedFromFreeWeekend
		{
			get
			{
				return this.native.apps.BIsSubscribedFromFreeWeekend();
			}
		}

		public Facepunch.Steamworks.Lobby Lobby
		{
			get
			{
				if (this._lobby == null)
				{
					this._lobby = new Facepunch.Steamworks.Lobby(this);
				}
				return this._lobby;
			}
		}

		public Facepunch.Steamworks.LobbyList LobbyList
		{
			get;
			private set;
		}

		public Facepunch.Steamworks.MicroTransactions MicroTransactions
		{
			get;
			private set;
		}

		public Facepunch.Steamworks.Overlay Overlay
		{
			get;
			private set;
		}

		public ulong OwnerSteamId
		{
			get;
			private set;
		}

		public Facepunch.Steamworks.RemoteStorage RemoteStorage
		{
			get;
			private set;
		}

		public Facepunch.Steamworks.Screenshots Screenshots
		{
			get
			{
				if (this._screenshots == null)
				{
					this._screenshots = new Facepunch.Steamworks.Screenshots(this);
				}
				return this._screenshots;
			}
		}

		public Facepunch.Steamworks.ServerList ServerList
		{
			get;
			private set;
		}

		public Facepunch.Steamworks.Stats Stats
		{
			get;
			private set;
		}

		public ulong SteamId
		{
			get;
			private set;
		}

		public Facepunch.Steamworks.User User
		{
			get;
			private set;
		}

		public string Username
		{
			get;
			private set;
		}

		public Facepunch.Steamworks.Voice Voice
		{
			get;
			private set;
		}

		public Client(uint appId) : base(appId)
		{
			if (Client.Instance != null)
			{
				throw new Exception("Only one Facepunch.Steamworks.Client can exist - dispose the old one before trying to create a new one.");
			}
			Client.Instance = this;
			this.native = new NativeInterface();
			if (!this.native.InitClient(this))
			{
				this.native.Dispose();
				this.native = null;
				Client.Instance = null;
				return;
			}
			SteamNative.Callbacks.RegisterCallbacks(this);
			base.SetupCommonInterfaces();
			this.Voice = new Facepunch.Steamworks.Voice(this);
			this.ServerList = new Facepunch.Steamworks.ServerList(this);
			this.LobbyList = new Facepunch.Steamworks.LobbyList(this);
			this.App = new Facepunch.Steamworks.App(this);
			this.Stats = new Facepunch.Steamworks.Stats(this);
			this.Achievements = new Facepunch.Steamworks.Achievements(this);
			this.MicroTransactions = new Facepunch.Steamworks.MicroTransactions(this);
			this.User = new Facepunch.Steamworks.User(this);
			this.RemoteStorage = new Facepunch.Steamworks.RemoteStorage(this);
			this.Overlay = new Facepunch.Steamworks.Overlay(this);
			base.Workshop.friends = this.Friends;
			this.Stats.UpdateStats();
			base.AppId = appId;
			this.Username = this.native.friends.GetPersonaName();
			this.SteamId = this.native.user.GetSteamID();
			this.BetaName = this.native.apps.GetCurrentBetaName();
			this.OwnerSteamId = this.native.apps.GetAppOwner();
			string appInstallDir = this.native.apps.GetAppInstallDir(base.AppId);
			if (!string.IsNullOrEmpty(appInstallDir) && Directory.Exists(appInstallDir))
			{
				this.InstallFolder = new DirectoryInfo(appInstallDir);
			}
			this.BuildId = this.native.apps.GetAppBuildId();
			this.CurrentCountry = this.native.utils.GetIPCountry();
			this.CurrentLanguage = this.native.apps.GetCurrentGameLanguage();
			this.AvailableLanguages = this.native.apps.GetAvailableGameLanguages().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			this.Update();
		}

		public override void Dispose()
		{
			if (this.disposed)
			{
				return;
			}
			if (this.Voice != null)
			{
				this.Voice = null;
			}
			if (this.ServerList != null)
			{
				this.ServerList.Dispose();
				this.ServerList = null;
			}
			if (this.LobbyList != null)
			{
				this.LobbyList.Dispose();
				this.LobbyList = null;
			}
			if (this.App != null)
			{
				this.App.Dispose();
				this.App = null;
			}
			if (this.Stats != null)
			{
				this.Stats.Dispose();
				this.Stats = null;
			}
			if (this.Achievements != null)
			{
				this.Achievements.Dispose();
				this.Achievements = null;
			}
			if (this.MicroTransactions != null)
			{
				this.MicroTransactions.Dispose();
				this.MicroTransactions = null;
			}
			if (this.User != null)
			{
				this.User.Dispose();
				this.User = null;
			}
			if (this.RemoteStorage != null)
			{
				this.RemoteStorage.Dispose();
				this.RemoteStorage = null;
			}
			if (Client.Instance == this)
			{
				Client.Instance = null;
			}
			base.Dispose();
		}

		~Client()
		{
			this.Dispose();
		}

		public Leaderboard GetLeaderboard(string name, Client.LeaderboardSortMethod sortMethod = 0, Client.LeaderboardDisplayType displayType = 0)
		{
			Leaderboard leaderboard = new Leaderboard(this);
			this.native.userstats.FindOrCreateLeaderboard(name, (SteamNative.LeaderboardSortMethod)sortMethod, (SteamNative.LeaderboardDisplayType)displayType, new Action<LeaderboardFindResult_t, bool>(leaderboard.OnBoardCreated));
			return leaderboard;
		}

		public static bool RestartIfNecessary(uint appid)
		{
			bool flag;
			using (SteamApi steamApi = new SteamApi())
			{
				flag = steamApi.SteamAPI_RestartAppIfNecessary(appid);
			}
			return flag;
		}

		public void RunCallbacks()
		{
			this.native.api.SteamAPI_RunCallbacks();
		}

		public override void Update()
		{
			if (!base.IsValid)
			{
				return;
			}
			this.RunCallbacks();
			this.Voice.Update();
			this.Friends.Cycle();
			base.Update();
		}

		public enum LeaderboardDisplayType
		{
			None,
			Numeric,
			TimeSeconds,
			TimeMilliSeconds
		}

		public enum LeaderboardSortMethod
		{
			None,
			Ascending,
			Descending
		}
	}
}