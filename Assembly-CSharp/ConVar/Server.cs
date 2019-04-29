using EasyAntiCheat.Server.Scout;
using Facepunch.Network.Raknet;
using Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace ConVar
{
	[Factory("server")]
	public class Server : ConsoleSystem
	{
		[ServerVar]
		public static string ip;

		[ServerVar]
		public static int port;

		[ServerVar]
		public static int queryport;

		[ServerVar]
		public static int maxplayers;

		[ServerVar]
		public static string hostname;

		[ServerVar]
		public static string identity;

		[ServerVar]
		public static string level;

		[ServerVar]
		public static string levelurl;

		[ServerVar]
		public static int seed;

		[ServerVar]
		public static int salt;

		[ServerVar]
		public static int worldsize;

		[ServerVar]
		public static int saveinterval;

		[ServerVar]
		public static bool secure;

		[ServerVar]
		public static int encryption;

		[ServerVar]
		public static int tickrate;

		[ServerVar]
		public static int entityrate;

		[ServerVar]
		public static float schematime;

		[ServerVar]
		public static float cycletime;

		[ServerVar]
		public static bool official;

		[ServerVar]
		public static bool stats;

		[ServerVar]
		public static bool globalchat;

		[ServerVar]
		public static bool stability;

		[ServerVar]
		public static bool radiation;

		[ServerVar]
		public static float itemdespawn;

		[ServerVar]
		public static float corpsedespawn;

		[ServerVar]
		public static float debrisdespawn;

		[ServerVar]
		public static bool pve;

		[ServerVar]
		public static string description;

		[ServerVar]
		public static string headerimage;

		[ServerVar]
		public static string url;

		[ServerVar]
		public static string branch;

		[ServerVar]
		public static int queriesPerSecond;

		[ServerVar]
		public static int ipQueriesPerMin;

		[ServerVar(Saved=true)]
		public static float meleedamage;

		[ServerVar(Saved=true)]
		public static float arrowdamage;

		[ServerVar(Saved=true)]
		public static float bulletdamage;

		[ServerVar(Saved=true)]
		public static float bleedingdamage;

		[ServerVar(Saved=true)]
		public static float meleearmor;

		[ServerVar(Saved=true)]
		public static float arrowarmor;

		[ServerVar(Saved=true)]
		public static float bulletarmor;

		[ServerVar(Saved=true)]
		public static float bleedingarmor;

		[ServerVar]
		public static int updatebatch;

		[ServerVar]
		public static int updatebatchspawn;

		[ServerVar]
		public static int entitybatchsize;

		[ServerVar]
		public static float entitybatchtime;

		[ServerVar]
		public static float planttick;

		[ServerVar]
		public static float planttickscale;

		[ServerVar]
		public static float metabolismtick;

		[ServerVar(Saved=true)]
		public static bool woundingenabled;

		[ServerVar(Saved=true)]
		public static bool playerserverfall;

		[ServerVar]
		public static bool plantlightdetection;

		[ServerVar]
		public static float respawnresetrange;

		[ServerVar]
		public static int maxunack;

		[ServerVar]
		public static bool netcache;

		[ServerVar]
		public static bool corpses;

		[ServerVar]
		public static bool events;

		[ServerVar]
		public static bool dropitems;

		[ServerVar]
		public static int netcachesize;

		[ServerVar]
		public static int savecachesize;

		[ServerVar]
		public static int combatlogsize;

		[ServerVar]
		public static int combatlogdelay;

		[ServerVar]
		public static int authtimeout;

		[ServerVar]
		public static int playertimeout;

		[ServerVar]
		public static int idlekick;

		[ServerVar]
		public static int idlekickmode;

		[ServerVar]
		public static int idlekickadmins;

		[ServerVar(Saved=true)]
		public static bool showHolsteredItems;

		[ServerVar]
		public static int maxrpcspersecond;

		[ServerVar]
		public static int maxcommandspersecond;

		[ServerVar]
		public static int maxcommandpacketsize;

		[ServerVar]
		public static int maxtickspersecond;

		public static string backupFolder
		{
			get
			{
				return string.Concat("backup/0/", ConVar.Server.identity);
			}
		}

		public static string backupFolder1
		{
			get
			{
				return string.Concat("backup/1/", ConVar.Server.identity);
			}
		}

		public static string backupFolder2
		{
			get
			{
				return string.Concat("backup/2/", ConVar.Server.identity);
			}
		}

		public static string backupFolder3
		{
			get
			{
				return string.Concat("backup/3/", ConVar.Server.identity);
			}
		}

		[ServerVar]
		public static bool compression
		{
			get
			{
				if (Network.Net.sv == null)
				{
					return false;
				}
				return Network.Net.sv.compressionEnabled;
			}
			set
			{
				Network.Net.sv.compressionEnabled = value;
			}
		}

		[ServerVar]
		public static int maxpacketsize
		{
			get
			{
				return Facepunch.Network.Raknet.Server.MaxPacketSize;
			}
			set
			{
				Facepunch.Network.Raknet.Server.MaxPacketSize = Mathf.Clamp(value, 1, 1000000000);
			}
		}

		[ServerVar]
		public static int maxpacketspersecond
		{
			get
			{
				return (int)Facepunch.Network.Raknet.Server.MaxPacketsPerSecond;
			}
			set
			{
				Facepunch.Network.Raknet.Server.MaxPacketsPerSecond = (ulong)Mathf.Clamp(value, 1, 1000000);
			}
		}

		[ServerVar]
		public static float maxreceivetime
		{
			get
			{
				return Facepunch.Network.Raknet.Server.MaxReceiveTime;
			}
			set
			{
				Facepunch.Network.Raknet.Server.MaxReceiveTime = Mathf.Clamp(value, 1f, 1000f);
			}
		}

		[ServerVar]
		public static bool netlog
		{
			get
			{
				if (Network.Net.sv == null)
				{
					return false;
				}
				return Network.Net.sv.logging;
			}
			set
			{
				Network.Net.sv.logging = value;
			}
		}

		public static string rootFolder
		{
			get
			{
				return string.Concat("server/", ConVar.Server.identity);
			}
		}

		static Server()
		{
			ConVar.Server.ip = "";
			ConVar.Server.port = 28015;
			ConVar.Server.queryport = 0;
			ConVar.Server.maxplayers = 500;
			ConVar.Server.hostname = "My Untitled Rust Server";
			ConVar.Server.identity = "my_server_identity";
			ConVar.Server.level = "Procedural Map";
			ConVar.Server.levelurl = "";
			ConVar.Server.seed = 0;
			ConVar.Server.salt = 0;
			ConVar.Server.worldsize = 0;
			ConVar.Server.saveinterval = 600;
			ConVar.Server.secure = true;
			ConVar.Server.encryption = 2;
			ConVar.Server.tickrate = 10;
			ConVar.Server.entityrate = 16;
			ConVar.Server.schematime = 1800f;
			ConVar.Server.cycletime = 500f;
			ConVar.Server.official = false;
			ConVar.Server.stats = false;
			ConVar.Server.globalchat = true;
			ConVar.Server.stability = true;
			ConVar.Server.radiation = true;
			ConVar.Server.itemdespawn = 300f;
			ConVar.Server.corpsedespawn = 300f;
			ConVar.Server.debrisdespawn = 30f;
			ConVar.Server.pve = false;
			ConVar.Server.description = "No server description has been provided.";
			ConVar.Server.headerimage = "";
			ConVar.Server.url = "";
			ConVar.Server.branch = "";
			ConVar.Server.queriesPerSecond = 2000;
			ConVar.Server.ipQueriesPerMin = 30;
			ConVar.Server.meleedamage = 1f;
			ConVar.Server.arrowdamage = 1f;
			ConVar.Server.bulletdamage = 1f;
			ConVar.Server.bleedingdamage = 1f;
			ConVar.Server.meleearmor = 1f;
			ConVar.Server.arrowarmor = 1f;
			ConVar.Server.bulletarmor = 1f;
			ConVar.Server.bleedingarmor = 1f;
			ConVar.Server.updatebatch = 512;
			ConVar.Server.updatebatchspawn = 1024;
			ConVar.Server.entitybatchsize = 100;
			ConVar.Server.entitybatchtime = 1f;
			ConVar.Server.planttick = 60f;
			ConVar.Server.planttickscale = 1f;
			ConVar.Server.metabolismtick = 1f;
			ConVar.Server.woundingenabled = true;
			ConVar.Server.playerserverfall = true;
			ConVar.Server.plantlightdetection = true;
			ConVar.Server.respawnresetrange = 50f;
			ConVar.Server.maxunack = 4;
			ConVar.Server.netcache = true;
			ConVar.Server.corpses = true;
			ConVar.Server.events = true;
			ConVar.Server.dropitems = true;
			ConVar.Server.netcachesize = 0;
			ConVar.Server.savecachesize = 0;
			ConVar.Server.combatlogsize = 30;
			ConVar.Server.combatlogdelay = 10;
			ConVar.Server.authtimeout = 60;
			ConVar.Server.playertimeout = 60;
			ConVar.Server.idlekick = 30;
			ConVar.Server.idlekickmode = 1;
			ConVar.Server.idlekickadmins = 0;
			ConVar.Server.showHolsteredItems = true;
			ConVar.Server.maxrpcspersecond = 200;
			ConVar.Server.maxcommandspersecond = 100;
			ConVar.Server.maxcommandpacketsize = 1000000;
			ConVar.Server.maxtickspersecond = 300;
		}

		public Server()
		{
		}

		[ServerVar(Help="Backup server folder")]
		public static void backup()
		{
			DirectoryEx.Backup(new string[] { ConVar.Server.backupFolder, ConVar.Server.backupFolder1, ConVar.Server.backupFolder2, ConVar.Server.backupFolder3 });
			DirectoryEx.CopyAll(ConVar.Server.rootFolder, ConVar.Server.backupFolder);
		}

		[ServerUserVar]
		public static void cheatreport(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			ulong num = arg.GetUInt64(0, (ulong)0);
			string str = arg.GetString(1, "");
			UnityEngine.Debug.LogWarning(string.Concat(new object[] { basePlayer, " reported ", num, ": ", str.ToPrintable(140) }));
			EACServer.eacScout.SendPlayerReport(num.ToString(), basePlayer.net.connection.userid.ToString(), PlayerReportCategory.PlayerReportCheating, str);
		}

		[ServerUserVar(Help="Get the player combat log")]
		public static string combatlog(ConsoleSystem.Arg arg)
		{
			BasePlayer playerOrSleeper = arg.Player();
			if (arg.HasArgs(1) && playerOrSleeper != null && playerOrSleeper.IsAdmin)
			{
				playerOrSleeper = arg.GetPlayerOrSleeper(0);
			}
			if (playerOrSleeper == null)
			{
				return "invalid player";
			}
			return playerOrSleeper.stats.combat.Get(ConVar.Server.combatlogsize);
		}

		[ServerVar]
		public static void fps(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith(string.Concat(Performance.report.frameRate.ToString(), " FPS"));
		}

		public static string GetServerFolder(string folder)
		{
			string str = string.Concat(ConVar.Server.rootFolder, "/", folder);
			if (Directory.Exists(str))
			{
				return str;
			}
			Directory.CreateDirectory(str);
			return str;
		}

		[ServerVar(Help="Print the current player eyes.")]
		public static string printeyes(ConsoleSystem.Arg arg)
		{
			BasePlayer playerOrSleeper = arg.Player();
			if (arg.HasArgs(1))
			{
				playerOrSleeper = arg.GetPlayerOrSleeper(0);
			}
			if (playerOrSleeper == null)
			{
				return "invalid player";
			}
			return playerOrSleeper.eyes.rotation.eulerAngles.ToString();
		}

		[ServerVar(Help="Print the current player position.")]
		public static string printpos(ConsoleSystem.Arg arg)
		{
			BasePlayer playerOrSleeper = arg.Player();
			if (arg.HasArgs(1))
			{
				playerOrSleeper = arg.GetPlayerOrSleeper(0);
			}
			if (playerOrSleeper == null)
			{
				return "invalid player";
			}
			return playerOrSleeper.transform.position.ToString();
		}

		[ServerVar(Help="Print the current player rotation.")]
		public static string printrot(ConsoleSystem.Arg arg)
		{
			BasePlayer playerOrSleeper = arg.Player();
			if (arg.HasArgs(1))
			{
				playerOrSleeper = arg.GetPlayerOrSleeper(0);
			}
			if (playerOrSleeper == null)
			{
				return "invalid player";
			}
			return playerOrSleeper.transform.rotation.eulerAngles.ToString();
		}

		[ServerVar]
		public static string readcfg(ConsoleSystem.Arg arg)
		{
			ConsoleSystem.Option server;
			string serverFolder = ConVar.Server.GetServerFolder("cfg");
			if (File.Exists(string.Concat(serverFolder, "/serverauto.cfg")))
			{
				string str = File.ReadAllText(string.Concat(serverFolder, "/serverauto.cfg"));
				server = ConsoleSystem.Option.Server;
				ConsoleSystem.RunFile(server.Quiet(), str);
			}
			if (File.Exists(string.Concat(serverFolder, "/server.cfg")))
			{
				string str1 = File.ReadAllText(string.Concat(serverFolder, "/server.cfg"));
				server = ConsoleSystem.Option.Server;
				ConsoleSystem.RunFile(server.Quiet(), str1);
			}
			return "Server Config Loaded";
		}

		[ServerVar(Help="Force save the current game")]
		public static void save(ConsoleSystem.Arg arg)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			foreach (BaseEntity baseEntity in BaseEntity.saveList)
			{
				baseEntity.InvalidateNetworkCache();
			}
			double totalSeconds = stopwatch.Elapsed.TotalSeconds;
			UnityEngine.Debug.Log(string.Concat("Invalidate Network Cache took ", totalSeconds.ToString("0.00"), " seconds"));
			SaveRestore.Save(true);
		}

		[ServerVar(Help="Send network update for all players")]
		public static void sendnetworkupdate(ConsoleSystem.Arg arg)
		{
			foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
			{
				basePlayer.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			}
		}

		[ServerVar(Help="Show holstered items on player bodies")]
		public static void setshowholstereditems(ConsoleSystem.Arg arg)
		{
			ConVar.Server.showHolsteredItems = arg.GetBool(0, ConVar.Server.showHolsteredItems);
			foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
			{
				basePlayer.inventory.UpdatedVisibleHolsteredItems();
			}
			foreach (BasePlayer basePlayer1 in BasePlayer.sleepingPlayerList)
			{
				basePlayer1.inventory.UpdatedVisibleHolsteredItems();
			}
		}

		[ServerVar(ServerAdmin=false, Help="This sends a snapshot of all the entities in the client's pvs. This is mostly redundant, but we request this when the client starts recording a demo.. so they get all the information.")]
		public static void snapshot(ConsoleSystem.Arg arg)
		{
			if (arg.Player() == null)
			{
				return;
			}
			UnityEngine.Debug.Log(string.Concat("Sending full snapshot to ", arg.Player()));
			arg.Player().SendNetworkUpdateImmediate(false);
			arg.Player().SendGlobalSnapshot();
			arg.Player().SendFullSnapshot();
		}

		[ServerVar(Help="Starts a server")]
		public static void start(ConsoleSystem.Arg arg)
		{
			if (Network.Net.sv.IsConnected())
			{
				arg.ReplyWith("There is already a server running!");
				return;
			}
			string str = arg.GetString(0, ConVar.Server.level);
			if (!LevelManager.IsValid(str))
			{
				arg.ReplyWith(string.Concat("Level '", str, "' isn't valid!"));
				return;
			}
			if (UnityEngine.Object.FindObjectOfType<ServerMgr>())
			{
				arg.ReplyWith("There is already a server running!");
				return;
			}
			UnityEngine.Object.DontDestroyOnLoad(GameManager.server.CreatePrefab("assets/bundled/prefabs/system/server.prefab", true));
			LevelManager.LoadLevel(str, true);
		}

		[ServerVar(Help="Stops a server")]
		public static void stop(ConsoleSystem.Arg arg)
		{
			if (!Network.Net.sv.IsConnected())
			{
				arg.ReplyWith("There isn't a server running!");
				return;
			}
			Network.Net.sv.Stop(arg.GetString(0, "Stopping Server"));
		}

		public static float TickDelta()
		{
			return 1f / (float)ConVar.Server.tickrate;
		}

		public static float TickTime(uint tick)
		{
			return (float)((double)ConVar.Server.TickDelta() * (double)((float)tick));
		}

		[ServerVar(Help="Writes config files")]
		public static void writecfg(ConsoleSystem.Arg arg)
		{
			string configString = ConsoleSystem.SaveToConfigString(true);
			File.WriteAllText(string.Concat(ConVar.Server.GetServerFolder("cfg"), "/serverauto.cfg"), configString);
			ServerUsers.Save();
			arg.ReplyWith("Config Saved");
		}
	}
}