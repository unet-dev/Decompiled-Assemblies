using Facepunch;
using Facepunch.Extend;
using Network;
using Network.Visibility;
using Rust;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Profiling;

namespace ConVar
{
	[Factory("global")]
	public class Global : ConsoleSystem
	{
		private static int _developer;

		[ClientVar]
		[ServerVar]
		public static int maxthreads;

		[ClientVar(Saved=true)]
		[ServerVar(Saved=true)]
		public static int perf;

		[ClientVar(ClientInfo=true, Saved=true, Help="If you're an admin this will enable god mode")]
		public static bool god;

		[ClientVar(ClientInfo=true, Saved=true, Help="If enabled you will be networked when you're spectating. This means that you will hear audio chat, but also means that cheaters will potentially be able to detect you watching them.")]
		public static bool specnet;

		[ClientVar]
		[ServerVar]
		public static int developer
		{
			get
			{
				return ConVar.Global._developer;
			}
			set
			{
				ConVar.Global._developer = value;
			}
		}

		[ClientVar]
		[ServerVar]
		public static bool timewarning
		{
			get
			{
				return TimeWarning.Enabled;
			}
			set
			{
				TimeWarning.Enabled = value;
			}
		}

		static Global()
		{
			ConVar.Global.maxthreads = 8;
			ConVar.Global.perf = 0;
			ConVar.Global.god = false;
			ConVar.Global.specnet = false;
		}

		public Global()
		{
		}

		[ServerVar]
		public static void breakitem(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			Item activeItem = basePlayer.GetActiveItem();
			if (activeItem != null)
			{
				activeItem.LoseCondition(activeItem.condition);
			}
		}

		[ClientVar]
		[ServerVar]
		public static void colliders(ConsoleSystem.Arg args)
		{
			int num = (
				from x in (IEnumerable<Collider>)UnityEngine.Object.FindObjectsOfType<Collider>()
				where x.enabled
				select x).Count<Collider>();
			int num1 = (
				from x in (IEnumerable<Collider>)UnityEngine.Object.FindObjectsOfType<Collider>()
				where !x.enabled
				select x).Count<Collider>();
			args.ReplyWith(string.Concat(new object[] { num, " colliders enabled, ", num1, " disabled" }));
		}

		[ClientVar]
		[ServerVar]
		public static void error(ConsoleSystem.Arg args)
		{
			null.transform.position = Vector3.zero;
		}

		[ClientVar]
		[ServerVar]
		public static void free(ConsoleSystem.Arg args)
		{
			ConVar.Pool.clear_prefabs(args);
			ConVar.Pool.clear_assets(args);
			ConVar.Pool.clear_memory(args);
			ConVar.GC.collect();
			ConVar.GC.unload();
		}

		[ServerVar]
		public static void injure(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (basePlayer.IsDead())
			{
				return;
			}
			basePlayer.StartWounded();
		}

		[ServerUserVar]
		public static void kill(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (basePlayer.IsSpectating())
			{
				return;
			}
			if (basePlayer.IsDead())
			{
				return;
			}
			if (!basePlayer.CanSuicide())
			{
				basePlayer.ConsoleMessage("You can't suicide again so quickly, wait a while");
				return;
			}
			basePlayer.MarkSuicide();
			basePlayer.Hurt(1000f, DamageType.Suicide, basePlayer, false);
		}

		[ClientVar]
		[ServerVar]
		public static void objects(ConsoleSystem.Arg args)
		{
			int i;
			Type type;
			UnityEngine.Object[] objArray = UnityEngine.Object.FindObjectsOfType<UnityEngine.Object>();
			string str = "";
			Dictionary<Type, int> types = new Dictionary<Type, int>();
			Dictionary<Type, long> types1 = new Dictionary<Type, long>();
			UnityEngine.Object[] objArray1 = objArray;
			for (i = 0; i < (int)objArray1.Length; i++)
			{
				UnityEngine.Object obj = objArray1[i];
				int runtimeMemorySize = Profiler.GetRuntimeMemorySize(obj);
				if (!types.ContainsKey(obj.GetType()))
				{
					types.Add(obj.GetType(), 1);
				}
				else
				{
					Dictionary<Type, int> item = types;
					type = obj.GetType();
					item[type] = item[type] + 1;
				}
				if (!types1.ContainsKey(obj.GetType()))
				{
					types1.Add(obj.GetType(), (long)runtimeMemorySize);
				}
				else
				{
					Dictionary<Type, long> types2 = types1;
					type = obj.GetType();
					types2[type] = types2[type] + (long)runtimeMemorySize;
				}
			}
			foreach (KeyValuePair<Type, long> keyValuePair in 
				from x in types1
				orderby x.Value descending
				select x)
			{
				object[] key = new object[] { str, null, null, null, null, null, null };
				i = types[keyValuePair.Key];
				key[1] = i.ToString().PadLeft(10);
				key[2] = " ";
				key[3] = keyValuePair.Value.FormatBytes<long>(false).PadLeft(15);
				key[4] = "\t";
				key[5] = keyValuePair.Key;
				key[6] = "\n";
				str = string.Concat(key);
			}
			args.ReplyWith(str);
		}

		[ClientVar]
		[ServerVar]
		public static void queue(ConsoleSystem.Arg args)
		{
			string str = "";
			str = string.Concat(str, "stabilityCheckQueue:\t\t", StabilityEntity.stabilityCheckQueue.Info(), "\n");
			str = string.Concat(str, "updateSurroundingsQueue:\t", StabilityEntity.updateSurroundingsQueue.Info(), "\n");
			args.ReplyWith(str);
		}

		[ClientVar]
		[ServerVar]
		public static void quit(ConsoleSystem.Arg args)
		{
			SingletonComponent<ServerMgr>.Instance.Shutdown();
			Rust.Application.isQuitting = true;
			Network.Net.sv.Stop("quit");
			Process.GetCurrentProcess().Kill();
			UnityEngine.Debug.Log("Quitting");
			Rust.Application.Quit();
		}

		[ServerVar]
		public static void report(ConsoleSystem.Arg args)
		{
			ServerPerformance.DoReport();
		}

		[ServerUserVar]
		public static void respawn(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (basePlayer.IsDead() || basePlayer.IsSpectating())
			{
				basePlayer.Respawn();
				return;
			}
			if (ConVar.Global.developer > 0)
			{
				UnityEngine.Debug.LogWarning(string.Concat(basePlayer, " wanted to respawn but isn't dead or spectating"));
			}
			basePlayer.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}

		[ServerUserVar]
		public static void respawn_sleepingbag(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (!basePlayer.IsDead())
			{
				return;
			}
			uint num = args.GetUInt(0, 0);
			if (num == 0)
			{
				args.ReplyWith("Missing sleeping bag ID");
				return;
			}
			if (!SleepingBag.SpawnPlayer(basePlayer, num))
			{
				args.ReplyWith("Couldn't spawn in sleeping bag!");
			}
		}

		[ServerUserVar]
		public static void respawn_sleepingbag_remove(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			uint num = args.GetUInt(0, 0);
			if (num == 0)
			{
				args.ReplyWith("Missing sleeping bag ID");
				return;
			}
			SleepingBag.DestroyBag(basePlayer, num);
		}

		[ServerVar]
		public static void restart(ConsoleSystem.Arg args)
		{
			ServerMgr.RestartServer(args.GetString(1, string.Empty), args.GetInt(0, 300));
		}

		[ServerUserVar]
		public static void setinfo(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			string str = args.GetString(0, null);
			string str1 = args.GetString(1, null);
			if (str == null || str1 == null)
			{
				return;
			}
			basePlayer.SetInfo(str, str1);
		}

		[ServerVar]
		public static void sleep(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (basePlayer.IsSleeping())
			{
				return;
			}
			if (basePlayer.IsSpectating())
			{
				return;
			}
			if (basePlayer.IsDead())
			{
				return;
			}
			basePlayer.StartSleeping();
		}

		[ServerVar]
		public static void spectate(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (!basePlayer.IsDead())
			{
				basePlayer.DieInstantly();
			}
			string str = args.GetString(0, "");
			if (basePlayer.IsDead())
			{
				basePlayer.StartSpectating();
				basePlayer.UpdateSpectateTarget(str);
			}
		}

		[ClientVar]
		public static void status_cl(ConsoleSystem.Arg args)
		{
		}

		[ServerUserVar]
		public static void status_sv(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			args.ReplyWith(basePlayer.GetDebugStatus());
		}

		[ClientVar]
		[ServerVar]
		public static void subscriptions(ConsoleSystem.Arg arg)
		{
			TextTable textTable = new TextTable();
			textTable.AddColumn("realm");
			textTable.AddColumn("group");
			BasePlayer basePlayer = arg.Player();
			if (basePlayer)
			{
				foreach (Group group in basePlayer.net.subscriber.subscribed)
				{
					textTable.AddRow(new string[] { "sv", group.ID.ToString() });
				}
			}
			arg.ReplyWith(textTable.ToString());
		}

		[ClientVar]
		[ServerVar]
		public static void sysinfo(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith(SystemInfoGeneralText.currentInfo);
		}

		[ClientVar]
		[ServerVar]
		public static void sysuid(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith(SystemInfo.deviceUniqueIdentifier);
		}

		[ServerVar]
		public static void teleport(ConsoleSystem.Arg args)
		{
			if (!args.HasArgs(2))
			{
				BasePlayer basePlayer = args.Player();
				if (!basePlayer)
				{
					return;
				}
				if (!basePlayer.IsAlive())
				{
					return;
				}
				BasePlayer playerOrSleeper = args.GetPlayerOrSleeper(0);
				if (!playerOrSleeper)
				{
					return;
				}
				if (!playerOrSleeper.IsAlive())
				{
					return;
				}
				basePlayer.Teleport(playerOrSleeper);
				return;
			}
			BasePlayer player = args.GetPlayer(0);
			if (!player)
			{
				return;
			}
			if (!player.IsAlive())
			{
				return;
			}
			BasePlayer playerOrSleeper1 = args.GetPlayerOrSleeper(1);
			if (!playerOrSleeper1)
			{
				return;
			}
			if (!playerOrSleeper1.IsAlive())
			{
				return;
			}
			player.Teleport(playerOrSleeper1);
		}

		[ServerVar]
		public static void teleport2me(ConsoleSystem.Arg args)
		{
			BasePlayer player = args.GetPlayer(0);
			if (!player)
			{
				return;
			}
			if (!player.IsAlive())
			{
				return;
			}
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (!basePlayer.IsAlive())
			{
				return;
			}
			player.Teleport(basePlayer);
		}

		[ServerVar]
		public static void teleportany(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (!basePlayer.IsAlive())
			{
				return;
			}
			basePlayer.Teleport(args.GetString(0, ""), false);
		}

		[ServerVar]
		public static void teleportpos(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (!basePlayer.IsAlive())
			{
				return;
			}
			basePlayer.Teleport(args.GetVector3(0, Vector3.zero));
		}

		[ClientVar]
		[ServerVar]
		public static void textures(ConsoleSystem.Arg args)
		{
			string str = "";
			Texture[] textureArray = UnityEngine.Object.FindObjectsOfType<Texture>();
			for (int i = 0; i < (int)textureArray.Length; i++)
			{
				Texture texture = textureArray[i];
				string str1 = Profiler.GetRuntimeMemorySize(texture).FormatBytes<int>(false);
				str = string.Concat(new string[] { str, texture.ToString().PadRight(30), texture.name.PadRight(30), str1, "\n" });
			}
			args.ReplyWith(str);
		}

		[ClientVar]
		[ServerVar(ServerUser=true)]
		public static void version(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith(string.Format("Protocol: {0}\nBuild Date: {1}\nUnity Version: {2}\nChangeset: {3}\nBranch: {4}", new object[] { Protocol.printable, BuildInfo.Current.BuildDate, UnityEngine.Application.unityVersion, BuildInfo.Current.Scm.ChangeId, BuildInfo.Current.Scm.Branch }));
		}
	}
}