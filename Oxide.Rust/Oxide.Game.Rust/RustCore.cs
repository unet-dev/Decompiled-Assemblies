using Facepunch;
using Network;
using Oxide.Core;
using Oxide.Core.Configuration;
using Oxide.Core.Libraries;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Plugins;
using Oxide.Core.RemoteConsole;
using Oxide.Game.Rust.Libraries;
using Oxide.Game.Rust.Libraries.Covalence;
using Rust.Ai;
using Rust.Ai.HTN;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Oxide.Game.Rust
{
	public class RustCore : CSPlugin
	{
		internal readonly Command cmdlib = Interface.Oxide.GetLibrary<Command>(null);

		internal readonly Lang lang = Interface.Oxide.GetLibrary<Lang>(null);

		internal readonly Permission permission = Interface.Oxide.GetLibrary<Permission>(null);

		internal readonly Oxide.Game.Rust.Libraries.Player Player = Interface.Oxide.GetLibrary<Oxide.Game.Rust.Libraries.Player>(null);

		internal readonly static RustCovalenceProvider Covalence;

		internal readonly PluginManager pluginManager = Interface.Oxide.RootPluginManager;

		internal readonly IServer Server = RustCore.Covalence.CreateServer();

		internal bool serverInitialized;

		internal bool isPlayerTakingDamage;

		internal static string ipPattern;

		internal static IEnumerable<string> RestrictedCommands
		{
			get
			{
				return new String[] { "ownerid", "moderatorid", "removeowner", "removemoderator" };
			}
		}

		static RustCore()
		{
			RustCore.Covalence = RustCovalenceProvider.Instance;
			RustCore.ipPattern = ":{1}[0-9]{1}\\d*";
		}

		public RustCore()
		{
			base.Title = "Rust";
			base.Author = RustExtension.AssemblyAuthors;
			base.Version = RustExtension.AssemblyVersion;
		}

		public static BasePlayer FindPlayer(string nameOrIdOrIp)
		{
			BasePlayer basePlayer;
			bool flag;
			BasePlayer basePlayer1 = null;
			List<BasePlayer>.Enumerator enumerator = BasePlayer.activePlayerList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BasePlayer current = enumerator.Current;
					if (String.IsNullOrEmpty(current.UserIDString))
					{
						continue;
					}
					if (!current.UserIDString.Equals(nameOrIdOrIp))
					{
						if (String.IsNullOrEmpty(current.displayName))
						{
							continue;
						}
						if (!current.displayName.Equals(nameOrIdOrIp, StringComparison.OrdinalIgnoreCase))
						{
							if (current.displayName.Contains(nameOrIdOrIp, CompareOptions.OrdinalIgnoreCase))
							{
								basePlayer1 = current;
							}
							Networkable networkable = current.net;
							if (networkable != null)
							{
								flag = networkable.connection;
							}
							else
							{
								flag = false;
							}
							if (!flag || !current.net.connection.ipaddress.Equals(nameOrIdOrIp))
							{
								continue;
							}
							basePlayer = current;
							return basePlayer;
						}
						else
						{
							basePlayer = current;
							return basePlayer;
						}
					}
					else
					{
						basePlayer = current;
						return basePlayer;
					}
				}
				goto Label0;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return basePlayer;
			foreach (BasePlayer basePlayer2 in BasePlayer.sleepingPlayerList)
			{
				if (String.IsNullOrEmpty(basePlayer2.UserIDString))
				{
					continue;
				}
				if (!basePlayer2.UserIDString.Equals(nameOrIdOrIp))
				{
					if (String.IsNullOrEmpty(basePlayer2.displayName))
					{
						continue;
					}
					if (!basePlayer2.displayName.Equals(nameOrIdOrIp, StringComparison.OrdinalIgnoreCase))
					{
						if (!basePlayer2.displayName.Contains(nameOrIdOrIp, CompareOptions.OrdinalIgnoreCase))
						{
							continue;
						}
						basePlayer1 = basePlayer2;
					}
					else
					{
						basePlayer = basePlayer2;
						return basePlayer;
					}
				}
				else
				{
					basePlayer = basePlayer2;
					return basePlayer;
				}
			}
			return basePlayer1;
		}

		public static BasePlayer FindPlayerById(ulong id)
		{
			BasePlayer basePlayer;
			foreach (BasePlayer basePlayer1 in BasePlayer.activePlayerList)
			{
				if (basePlayer1.userID != id)
				{
					continue;
				}
				basePlayer = basePlayer1;
				return basePlayer;
			}
			List<BasePlayer>.Enumerator enumerator = BasePlayer.sleepingPlayerList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BasePlayer current = enumerator.Current;
					if (current.userID != id)
					{
						continue;
					}
					basePlayer = current;
					return basePlayer;
				}
				return null;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return basePlayer;
		}

		public static BasePlayer FindPlayerByIdString(string id)
		{
			BasePlayer basePlayer;
			foreach (BasePlayer basePlayer1 in BasePlayer.activePlayerList)
			{
				if (String.IsNullOrEmpty(basePlayer1.UserIDString) || !basePlayer1.UserIDString.Equals(id))
				{
					continue;
				}
				basePlayer = basePlayer1;
				return basePlayer;
			}
			List<BasePlayer>.Enumerator enumerator = BasePlayer.sleepingPlayerList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BasePlayer current = enumerator.Current;
					if (String.IsNullOrEmpty(current.UserIDString) || !current.UserIDString.Equals(id))
					{
						continue;
					}
					basePlayer = current;
					return basePlayer;
				}
				return null;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return basePlayer;
		}

		public static BasePlayer FindPlayerByName(string name)
		{
			BasePlayer basePlayer;
			BasePlayer basePlayer1 = null;
			foreach (BasePlayer basePlayer2 in BasePlayer.activePlayerList)
			{
				if (String.IsNullOrEmpty(basePlayer2.displayName))
				{
					continue;
				}
				if (!basePlayer2.displayName.Equals(name, StringComparison.OrdinalIgnoreCase))
				{
					if (!basePlayer2.displayName.Contains(name, CompareOptions.OrdinalIgnoreCase))
					{
						continue;
					}
					basePlayer1 = basePlayer2;
				}
				else
				{
					basePlayer = basePlayer2;
					return basePlayer;
				}
			}
			List<BasePlayer>.Enumerator enumerator = BasePlayer.sleepingPlayerList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BasePlayer current = enumerator.Current;
					if (String.IsNullOrEmpty(current.displayName))
					{
						continue;
					}
					if (!current.displayName.Equals(name, StringComparison.OrdinalIgnoreCase))
					{
						if (!current.displayName.Contains(name, CompareOptions.OrdinalIgnoreCase))
						{
							continue;
						}
						basePlayer1 = current;
					}
					else
					{
						basePlayer = current;
						return basePlayer;
					}
				}
				return basePlayer1;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return basePlayer;
		}

		private int GetPlayersSensed(NPCPlayerApex npc, Vector3 position, float distance, BaseEntity[] targetList)
		{
			return BaseEntity.Query.Server.GetInSphere(position, distance, targetList, (BaseEntity entity) => {
				BasePlayer basePlayer = entity as BasePlayer;
				object obj = (!(basePlayer != null) || !(npc != null) || !(basePlayer != npc) ? null : Interface.CallHook("OnNpcPlayerTarget", npc, basePlayer));
				if (obj != null)
				{
					foreach (Memory.SeenInfo all in npc.AiContext.Memory.All)
					{
						if (all.Entity != basePlayer)
						{
							continue;
						}
						npc.AiContext.Memory.All.Remove(all);
						goto Label0;
					}
					foreach (Memory.ExtendedInfo allExtended in npc.AiContext.Memory.AllExtended)
					{
						if (allExtended.Entity != basePlayer)
						{
							continue;
						}
						npc.AiContext.Memory.AllExtended.Remove(allExtended);
						if (!(basePlayer != null) || obj != null || !basePlayer.isServer || basePlayer.IsSleeping() || basePlayer.IsDead())
						{
							return false;
						}
						return basePlayer.Family != npc.Family;
					}
				}
				if (!(basePlayer != null) || obj != null || !basePlayer.isServer || basePlayer.IsSleeping() || basePlayer.IsDead())
				{
					return false;
				}
				return basePlayer.Family != npc.Family;
			});
		}

		[HookMethod("GrantCommand")]
		private void GrantCommand(IPlayer player, string command, string[] args)
		{
			IPlayer player1;
			if (!this.PermissionsLoaded(player))
			{
				return;
			}
			if ((int)args.Length < 3)
			{
				player.Reply(this.lang.GetMessage("CommandUsageGrant", this, player.Id));
				return;
			}
			string str = args[0];
			string name = args[1].Sanitize();
			string str1 = args[2];
			if (!this.permission.PermissionExists(str1, null))
			{
				player.Reply(String.Format(this.lang.GetMessage("PermissionNotFound", this, player.Id), str1));
				return;
			}
			if (str.Equals("group"))
			{
				if (!this.permission.GroupExists(name))
				{
					player.Reply(String.Format(this.lang.GetMessage("GroupNotFound", this, player.Id), name));
					return;
				}
				if (this.permission.GroupHasPermission(name, str1))
				{
					player.Reply(String.Format(this.lang.GetMessage("GroupAlreadyHasPermission", this, player.Id), (object)name, str1));
					return;
				}
				this.permission.GrantGroupPermission(name, str1, null);
				player.Reply(String.Format(this.lang.GetMessage("GroupPermissionGranted", this, player.Id), (object)name, str1));
				return;
			}
			if (!str.Equals("user"))
			{
				player.Reply(this.lang.GetMessage("CommandUsageGrant", this, player.Id));
				return;
			}
			IPlayer[] array = RustCore.Covalence.PlayerManager.FindPlayers(name).ToArray<IPlayer>();
			if ((int)array.Length > 1)
			{
				player.Reply(String.Format(this.lang.GetMessage("PlayersFound", this, player.Id), String.Join(", ", (
					from p in (IEnumerable<IPlayer>)array
					select p.Name).ToArray<string>())));
				return;
			}
			if ((int)array.Length == 1)
			{
				player1 = array[0];
			}
			else
			{
				player1 = null;
			}
			IPlayer player2 = player1;
			if (player2 == null && !this.permission.UserIdValid(name))
			{
				player.Reply(String.Format(this.lang.GetMessage("PlayerNotFound", this, player.Id), name));
				return;
			}
			string id = name;
			if (player2 != null)
			{
				id = player2.Id;
				name = player2.Name;
				this.permission.UpdateNickname(id, name);
			}
			if (this.permission.UserHasPermission(name, str1))
			{
				player.Reply(String.Format(this.lang.GetMessage("PlayerAlreadyHasPermission", this, player.Id), (object)id, str1));
				return;
			}
			this.permission.GrantUserPermission(id, str1, null);
			player.Reply(String.Format(this.lang.GetMessage("PlayerPermissionGranted", this, player.Id), (object)String.Concat(name, " (", id, ")"), str1));
		}

		[HookMethod("GroupCommand")]
		private void GroupCommand(IPlayer player, string command, string[] args)
		{
			if (!this.PermissionsLoaded(player))
			{
				return;
			}
			if ((int)args.Length < 2)
			{
				player.Reply(this.lang.GetMessage("CommandUsageGroup", this, player.Id));
				player.Reply(this.lang.GetMessage("CommandUsageGroupParent", this, player.Id));
				player.Reply(this.lang.GetMessage("CommandUsageGroupRemove", this, player.Id));
				return;
			}
			string str = args[0];
			string str1 = args[1];
			string str2 = ((int)args.Length >= 3 ? args[2] : "");
			int num = ((int)args.Length == 4 ? Int32.Parse(args[3]) : 0);
			if (str.Equals("add"))
			{
				if (this.permission.GroupExists(str1))
				{
					player.Reply(String.Format(this.lang.GetMessage("GroupAlreadyExists", this, player.Id), str1));
					return;
				}
				this.permission.CreateGroup(str1, str2, num);
				player.Reply(String.Format(this.lang.GetMessage("GroupCreated", this, player.Id), str1));
				return;
			}
			if (str.Equals("remove"))
			{
				if (!this.permission.GroupExists(str1))
				{
					player.Reply(String.Format(this.lang.GetMessage("GroupNotFound", this, player.Id), str1));
					return;
				}
				this.permission.RemoveGroup(str1);
				player.Reply(String.Format(this.lang.GetMessage("GroupDeleted", this, player.Id), str1));
				return;
			}
			if (str.Equals("set"))
			{
				if (!this.permission.GroupExists(str1))
				{
					player.Reply(String.Format(this.lang.GetMessage("GroupNotFound", this, player.Id), str1));
					return;
				}
				this.permission.SetGroupTitle(str1, str2);
				this.permission.SetGroupRank(str1, num);
				player.Reply(String.Format(this.lang.GetMessage("GroupChanged", this, player.Id), str1));
				return;
			}
			if (!str.Equals("parent"))
			{
				player.Reply(this.lang.GetMessage("CommandUsageGroup", this, player.Id));
				player.Reply(this.lang.GetMessage("CommandUsageGroupParent", this, player.Id));
				player.Reply(this.lang.GetMessage("CommandUsageGroupRemove", this, player.Id));
				return;
			}
			if ((int)args.Length <= 2)
			{
				player.Reply(this.lang.GetMessage("CommandUsageGroupParent", this, player.Id));
				return;
			}
			if (!this.permission.GroupExists(str1))
			{
				player.Reply(String.Format(this.lang.GetMessage("GroupNotFound", this, player.Id), str1));
				return;
			}
			string str3 = args[2];
			if (!String.IsNullOrEmpty(str3) && !this.permission.GroupExists(str3))
			{
				player.Reply(String.Format(this.lang.GetMessage("GroupParentNotFound", this, player.Id), str3));
				return;
			}
			if (!this.permission.SetGroupParent(str1, str3))
			{
				player.Reply(String.Format(this.lang.GetMessage("GroupParentNotChanged", this, player.Id), str1));
				return;
			}
			player.Reply(String.Format(this.lang.GetMessage("GroupParentChanged", this, player.Id), (object)str1, str3));
		}

		[HookMethod("ICanPickupEntity")]
		private object ICanPickupEntity(BasePlayer player, DoorCloser entity)
		{
			bool flag = false;
			bool flag1;
			object obj = Interface.CallHook("CanPickupEntity", player, entity);
			object obj1 = obj;
			if (!(obj is Boolean))
			{
				flag1 = false;
			}
			else
			{
				flag = (Boolean)obj1;
				flag1 = true;
			}
			if (!flag1 | !flag)
			{
				return null;
			}
			return true;
		}

		[HookMethod("Init")]
		private void Init()
		{
			RemoteLogger.SetTag("game", base.Title.ToLower());
			RemoteLogger.SetTag("game version", this.Server.Version);
			base.AddCovalenceCommand(new String[] { "oxide.plugins", "o.plugins", "plugins" }, "PluginsCommand", "oxide.plugins");
			base.AddCovalenceCommand(new String[] { "oxide.load", "o.load", "plugin.load" }, "LoadCommand", "oxide.load");
			base.AddCovalenceCommand(new String[] { "oxide.reload", "o.reload", "plugin.reload" }, "ReloadCommand", "oxide.reload");
			base.AddCovalenceCommand(new String[] { "oxide.unload", "o.unload", "plugin.unload" }, "UnloadCommand", "oxide.unload");
			base.AddCovalenceCommand(new String[] { "oxide.grant", "o.grant", "perm.grant" }, "GrantCommand", "oxide.grant");
			base.AddCovalenceCommand(new String[] { "oxide.group", "o.group", "perm.group" }, "GroupCommand", "oxide.group");
			base.AddCovalenceCommand(new String[] { "oxide.revoke", "o.revoke", "perm.revoke" }, "RevokeCommand", "oxide.revoke");
			base.AddCovalenceCommand(new String[] { "oxide.show", "o.show", "perm.show" }, "ShowCommand", "oxide.show");
			base.AddCovalenceCommand(new String[] { "oxide.usergroup", "o.usergroup", "perm.usergroup" }, "UserGroupCommand", "oxide.usergroup");
			base.AddCovalenceCommand(new String[] { "oxide.lang", "o.lang", "lang" }, "LangCommand", (string[])null);
			base.AddCovalenceCommand(new String[] { "oxide.save", "o.save" }, "SaveCommand", (string[])null);
			base.AddCovalenceCommand(new String[] { "oxide.version", "o.version" }, "VersionCommand", (string[])null);
			foreach (KeyValuePair<string, Dictionary<string, string>> language in Localization.languages)
			{
				this.lang.RegisterMessages(language.Value, this, language.Key);
			}
			if (this.permission.IsLoaded)
			{
				int num1 = 0;
				foreach (string defaultGroup in Interface.Oxide.Config.Options.DefaultGroups)
				{
					if (this.permission.GroupExists(defaultGroup))
					{
						continue;
					}
					int num2 = num1;
					num1 = num2 + 1;
					this.permission.CreateGroup(defaultGroup, defaultGroup, num2);
				}
				this.permission.RegisterValidate((string s) => {
					ulong num;
					if (!UInt64.TryParse(s, out num))
					{
						return false;
					}
					return (num == 0 ? 1 : (int)Math.Floor(Math.Log10((double)((float)num)) + 1)) >= 17;
				});
				this.permission.CleanUp();
			}
		}

		[HookMethod("IOnBaseCombatEntityHurt")]
		private object IOnBaseCombatEntityHurt(BaseCombatEntity entity, HitInfo info)
		{
			if (entity is BasePlayer)
			{
				return null;
			}
			return Interface.CallHook("OnEntityTakeDamage", entity, info);
		}

		[HookMethod("IOnBasePlayerAttacked")]
		private object IOnBasePlayerAttacked(BasePlayer player, HitInfo info)
		{
			if (!this.serverInitialized || player == null || info == null || player.IsDead() || this.isPlayerTakingDamage || player is NPCPlayer)
			{
				return null;
			}
			if (Interface.CallHook("OnEntityTakeDamage", player, info) != null)
			{
				return true;
			}
			this.isPlayerTakingDamage = true;
			try
			{
				player.OnAttacked(info);
			}
			finally
			{
				this.isPlayerTakingDamage = false;
			}
			return true;
		}

		[HookMethod("IOnBasePlayerHurt")]
		private object IOnBasePlayerHurt(BasePlayer player, HitInfo info)
		{
			if (this.isPlayerTakingDamage)
			{
				return null;
			}
			return Interface.CallHook("OnEntityTakeDamage", player, info);
		}

		[HookMethod("IOnHtnNpcPlayerTarget")]
		private object IOnHtnNpcPlayerTarget(IHTNAgent npc, BasePlayer target)
		{
			if (npc == null || Interface.CallHook("OnNpcPlayerTarget", npc.Body, target) == null)
			{
				return null;
			}
			npc.AiDomain.NpcContext.BaseMemory.Forget(0f);
			npc.AiDomain.NpcContext.BaseMemory.PrimaryKnownEnemyPlayer.PlayerInfo.Player = null;
			return true;
		}

		[HookMethod("IOnLoseCondition")]
		private object IOnLoseCondition(Item item, float amount)
		{
			object[] objArray = new Object[] { item, amount };
			Interface.CallHook("OnLoseCondition", objArray);
			amount = (Single)objArray[1];
			float single = item.condition;
			Item item1 = item;
			item1.condition = item1.condition - amount;
			if (item.condition <= 0f && item.condition < single)
			{
				item.OnBroken();
			}
			return true;
		}

		[HookMethod("IOnNpcPlayerSenseClose")]
		private object IOnNpcPlayerSenseClose(NPCPlayerApex npc)
		{
			NPCPlayerApex.EntityQueryResultCount = this.GetPlayersSensed(npc, npc.ServerPosition, npc.Stats.CloseRange, NPCPlayerApex.EntityQueryResults);
			return true;
		}

		[HookMethod("IOnNpcPlayerSenseVision")]
		private object IOnNpcPlayerSenseVision(NPCPlayerApex npc)
		{
			Vector3 serverPosition = npc.ServerPosition;
			float visionRange = npc.Stats.VisionRange;
			BaseEntity[] playerQueryResults = NPCPlayerApex.PlayerQueryResults;
			NPCPlayerApex.PlayerQueryResultCount = this.GetPlayersSensed(npc, serverPosition, visionRange, playerQueryResults);
			return true;
		}

		[HookMethod("IOnNpcPlayerTarget")]
		private object IOnNpcPlayerTarget(NPCPlayerApex npc, BaseEntity target)
		{
			if (Interface.CallHook("OnNpcPlayerTarget", npc, target) == null)
			{
				return null;
			}
			return 0f;
		}

		[HookMethod("IOnNpcTarget")]
		private object IOnNpcTarget(BaseNpc npc, BaseEntity target)
		{
			if (Interface.CallHook("OnNpcTarget", npc, target) == null)
			{
				return null;
			}
			npc.SetFact(BaseNpc.Facts.HasEnemy, 0, true, true);
			npc.SetFact(BaseNpc.Facts.EnemyRange, 3, true, true);
			npc.SetFact(BaseNpc.Facts.AfraidRange, 1, true, true);
			return 0f;
		}

		[HookMethod("IOnPlayerBanned")]
		private void IOnPlayerBanned(Network.Connection connection)
		{
			string str = Regex.Replace(connection.ipaddress, RustCore.ipPattern, "") ?? "0";
			string str1 = connection.authStatus ?? "Unknown";
			Interface.CallHook("OnPlayerBanned", connection.username, connection.userid, str, str1);
			Interface.CallHook("OnUserBanned", connection.username, connection.userid.ToString(), str, str1);
		}

		[HookMethod("IOnPlayerChat")]
		private object IOnPlayerChat(ConsoleSystem.Arg arg, string message)
		{
			IPlayer player;
			arg.Args[0] = message.EscapeRichText();
			BasePlayer connection = arg.Connection.player as BasePlayer;
			if (connection != null)
			{
				player = connection.IPlayer;
			}
			else
			{
				player = null;
			}
			IPlayer player1 = player;
			if (player1 == null)
			{
				return null;
			}
			object obj = Interface.CallHook("OnPlayerChat", arg);
			object obj1 = Interface.CallHook("OnUserChat", player1, message);
			if (obj == null)
			{
				obj = obj1;
			}
			return obj;
		}

		[HookMethod("IOnPlayerCommand")]
		private void IOnPlayerCommand(ConsoleSystem.Arg arg)
		{
			string str;
			string[] strArray;
			IPlayer player;
			string str1 = arg.GetString(0, "").Trim();
			if (String.IsNullOrEmpty(str1) || str1[0] != '/' || str1.Length <= 1)
			{
				return;
			}
			this.ParseCommand(str1.TrimStart(new Char[] { '/' }), out str, out strArray);
			if (str == null)
			{
				return;
			}
			BasePlayer connection = arg.Connection.player as BasePlayer;
			if (connection != null)
			{
				player = connection.IPlayer;
			}
			else
			{
				player = null;
			}
			IPlayer player1 = player;
			if (player1 == null)
			{
				return;
			}
			object obj = Interface.CallHook("OnPlayerCommand", arg);
			object obj1 = Interface.CallHook("OnUserCommand", player1, str, strArray);
			if (obj != null || obj1 != null)
			{
				return;
			}
			if (!RustCore.Covalence.CommandSystem.HandleChatMessage(player1, str1) && !this.cmdlib.HandleChatCommand(connection, str, strArray) && Interface.Oxide.Config.Options.Modded)
			{
				player1.Reply(String.Format(this.lang.GetMessage("UnknownCommand", this, player1.Id), str));
			}
		}

		[HookMethod("IOnRconCommand")]
		private object IOnRconCommand(IPEndPoint sender, string command)
		{
			string message;
			if (sender != null && !String.IsNullOrEmpty(command))
			{
				RemoteMessage remoteMessage = RemoteMessage.GetMessage(command);
				if (remoteMessage != null)
				{
					message = remoteMessage.Message;
				}
				else
				{
					message = null;
				}
				if (!String.IsNullOrEmpty(message))
				{
					string[] strArray = Oxide.Core.CommandLine.Split(remoteMessage.Message);
					if ((int)strArray.Length >= 1 && Interface.CallHook("OnRconCommand", sender, strArray[0].ToLower(), strArray.Skip<string>(1).ToArray<string>()) != null)
					{
						return true;
					}
				}
			}
			return null;
		}

		[HookMethod("IOnRconInitialize")]
		private object IOnRconInitialize()
		{
			if (!Interface.Oxide.Config.Rcon.Enabled)
			{
				return null;
			}
			return true;
		}

		[HookMethod("IOnRunCommandLine")]
		private object IOnRunCommandLine()
		{
			foreach (KeyValuePair<string, string> @switch in Facepunch.CommandLine.GetSwitches())
			{
				string value = @switch.Value;
				if (value == "")
				{
					value = "1";
				}
				string str = @switch.Key.Substring(1);
				ConsoleSystem.Option unrestricted = ConsoleSystem.Option.Unrestricted;
				unrestricted.PrintOutput = false;
				ConsoleSystem.Run(unrestricted, str, new Object[] { value });
			}
			return false;
		}

		[HookMethod("IOnServerCommand")]
		private object IOnServerCommand(ConsoleSystem.Arg arg)
		{
			if (arg == null || arg.Connection != null && arg.Player() == null)
			{
				return true;
			}
			if (arg.cmd.FullName == "chat.say")
			{
				return null;
			}
			return Interface.CallHook("OnServerCommand", arg);
		}

		[HookMethod("IOnServerInitialized")]
		private void IOnServerInitialized()
		{
			if (!this.serverInitialized)
			{
				Oxide.Core.Analytics.Collect();
				if (!Interface.Oxide.Config.Options.Modded)
				{
					Interface.Oxide.LogWarning("The server is currently listed under Community. Please be aware that Facepunch only allows admin tools(that do not affect gameplay) under the Community section", Array.Empty<object>());
				}
				this.serverInitialized = true;
				Interface.CallHook("OnServerInitialized", this.serverInitialized);
			}
		}

		[HookMethod("IOnServerUsersRemove")]
		private void IOnServerUsersRemove(ulong steamId)
		{
			object name;
			object address;
			object obj;
			object address1;
			if (this.serverInitialized)
			{
				string str = steamId.ToString();
				IPlayer player = RustCore.Covalence.PlayerManager.FindPlayerById(str);
				if (ServerUsers.Is(steamId, ServerUsers.UserGroup.Banned))
				{
					if (player != null)
					{
						name = player.Name;
					}
					else
					{
						name = null;
					}
					if (name == null)
					{
						name = "Unnamed";
					}
					object obj1 = steamId;
					if (player != null)
					{
						address = player.Address;
					}
					else
					{
						address = null;
					}
					if (address == null)
					{
						address = "0";
					}
					Interface.CallHook("OnPlayerUnbanned", name, obj1, address);
					if (player != null)
					{
						obj = player.Name;
					}
					else
					{
						obj = null;
					}
					if (obj == null)
					{
						obj = "Unnamed";
					}
					string str1 = str;
					if (player != null)
					{
						address1 = player.Address;
					}
					else
					{
						address1 = null;
					}
					if (address1 == null)
					{
						address1 = "0";
					}
					Interface.CallHook("OnUserUnbanned", obj, str1, address1);
				}
			}
		}

		[HookMethod("IOnServerUsersSet")]
		private void IOnServerUsersSet(ulong steamId, ServerUsers.UserGroup group, string name, string reason)
		{
			object address;
			object obj;
			if (this.serverInitialized)
			{
				string str = steamId.ToString();
				IPlayer player = RustCore.Covalence.PlayerManager.FindPlayerById(str);
				if (group == ServerUsers.UserGroup.Banned)
				{
					string str1 = name;
					object obj1 = steamId;
					if (player != null)
					{
						address = player.Address;
					}
					else
					{
						address = null;
					}
					if (address == null)
					{
						address = "0";
					}
					Interface.CallHook("OnPlayerBanned", str1, obj1, address, reason);
					string str2 = name;
					string str3 = str;
					if (player != null)
					{
						obj = player.Address;
					}
					else
					{
						obj = null;
					}
					if (obj == null)
					{
						obj = "0";
					}
					Interface.CallHook("OnUserBanned", str2, str3, obj, reason);
				}
			}
		}

		[HookMethod("IOnUserApprove")]
		private object IOnUserApprove(Network.Connection connection)
		{
			string str = connection.username;
			string str1 = connection.userid.ToString();
			string str2 = Regex.Replace(connection.ipaddress, RustCore.ipPattern, "");
			uint num = connection.authLevel;
			if (this.permission.IsLoaded)
			{
				this.permission.UpdateNickname(str1, str);
				OxideConfig.DefaultGroups defaultGroups = Interface.Oxide.Config.Options.DefaultGroups;
				if (!this.permission.UserHasGroup(str1, defaultGroups.Players))
				{
					this.permission.AddUserGroup(str1, defaultGroups.Players);
				}
				if (num == 2 && !this.permission.UserHasGroup(str1, defaultGroups.Administrators))
				{
					this.permission.AddUserGroup(str1, defaultGroups.Administrators);
				}
			}
			RustCore.Covalence.PlayerManager.PlayerJoin(connection.userid, str);
			object obj = Interface.CallHook("CanClientLogin", connection);
			object obj1 = Interface.CallHook("CanUserLogin", str, str1, str2);
			if (obj == null)
			{
				obj = obj1;
			}
			object obj2 = obj;
			if (obj2 is String || obj2 is Boolean && !(Boolean)obj2)
			{
				ConnectionAuth.Reject(connection, (obj2 is String ? obj2.ToString() : this.lang.GetMessage("ConnectionRejected", this, str1)));
				return true;
			}
			object obj3 = Interface.CallHook("OnUserApprove", connection);
			object obj4 = Interface.CallHook("OnUserApproved", str, str1, str2);
			if (obj3 == null)
			{
				obj3 = obj4;
			}
			return obj3;
		}

		[HookMethod("LangCommand")]
		private void LangCommand(IPlayer player, string command, string[] args)
		{
			if ((int)args.Length < 1)
			{
				player.Reply(this.lang.GetMessage("CommandUsageLang", this, player.Id));
				return;
			}
			if (player.IsServer)
			{
				this.lang.SetServerLanguage(args[0]);
				player.Reply(String.Format(this.lang.GetMessage("ServerLanguage", this, player.Id), this.lang.GetServerLanguage()));
				return;
			}
			if (this.lang.GetLanguages(null).Contains<string>(args[0]))
			{
				this.lang.SetLanguage(args[0], player.Id);
			}
			player.Reply(String.Format(this.lang.GetMessage("PlayerLanguage", this, player.Id), args[0]));
		}

		[HookMethod("LoadCommand")]
		private void LoadCommand(IPlayer player, string command, string[] args)
		{
			if ((int)args.Length < 1)
			{
				player.Reply(this.lang.GetMessage("CommandUsageLoad", this, player.Id));
				return;
			}
			if (args[0].Equals("*") || args[0].Equals("all"))
			{
				Interface.Oxide.LoadAllPlugins(false);
				return;
			}
			string[] strArray = args;
			for (int i = 0; i < (int)strArray.Length; i++)
			{
				string str = strArray[i];
				if (!String.IsNullOrEmpty(str))
				{
					Interface.Oxide.LoadPlugin(str);
					this.pluginManager.GetPlugin(str);
				}
			}
		}

		private void OnClientAuth(Network.Connection connection)
		{
			connection.username = Regex.Replace(connection.username, "<[^>]*>", String.Empty);
		}

		[HookMethod("OnPlayerDisconnected")]
		private void OnPlayerDisconnected(BasePlayer player, string reason)
		{
			IPlayer player1 = player.IPlayer;
			if (player1 != null)
			{
				Interface.CallHook("OnUserDisconnected", player1, reason);
			}
			RustCore.Covalence.PlayerManager.PlayerDisconnected(player);
		}

		[HookMethod("OnPlayerInit")]
		private void OnPlayerInit(BasePlayer player)
		{
			this.lang.SetLanguage(player.net.connection.info.GetString("global.language", "en"), player.UserIDString);
			RustCore.Covalence.PlayerManager.PlayerConnected(player);
			IPlayer player1 = RustCore.Covalence.PlayerManager.FindPlayerById(player.UserIDString);
			if (player1 != null)
			{
				player.IPlayer = player1;
				Interface.CallHook("OnUserConnected", player1);
			}
		}

		[HookMethod("OnPlayerKicked")]
		private void OnPlayerKicked(BasePlayer player, string reason)
		{
			if (player.IPlayer != null)
			{
				Interface.CallHook("OnUserKicked", player.IPlayer, reason);
			}
		}

		[HookMethod("OnPlayerRespawn")]
		private object OnPlayerRespawn(BasePlayer player)
		{
			IPlayer player1 = player.IPlayer;
			if (player1 == null)
			{
				return null;
			}
			return Interface.CallHook("OnUserRespawn", player1);
		}

		[HookMethod("OnPlayerRespawned")]
		private void OnPlayerRespawned(BasePlayer player)
		{
			IPlayer player1 = player.IPlayer;
			if (player1 != null)
			{
				Interface.CallHook("OnUserRespawned", player1);
			}
		}

		[HookMethod("OnPluginLoaded")]
		private void OnPluginLoaded(Plugin plugin)
		{
			if (this.serverInitialized)
			{
				plugin.CallHook("OnServerInitialized", new Object[] { false });
			}
		}

		[HookMethod("OnServerSave")]
		private void OnServerSave()
		{
			Interface.Oxide.OnSave();
			RustCore.Covalence.PlayerManager.SavePlayerData();
		}

		[HookMethod("OnServerShutdown")]
		private void OnServerShutdown()
		{
			Interface.Oxide.OnShutdown();
			RustCore.Covalence.PlayerManager.SavePlayerData();
		}

		private void ParseCommand(string argstr, out string command, out string[] args)
		{
			List<string> strs = new List<string>();
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			string str = argstr;
			for (int i = 0; i < str.Length; i++)
			{
				char chr = str[i];
				if (chr == '\"')
				{
					if (!flag)
					{
						flag = true;
					}
					else
					{
						string str1 = stringBuilder.ToString().Trim();
						if (!String.IsNullOrEmpty(str1))
						{
							strs.Add(str1);
						}
						stringBuilder.Clear();
						flag = false;
					}
				}
				else if (!Char.IsWhiteSpace(chr) || flag)
				{
					stringBuilder.Append(chr);
				}
				else
				{
					string str2 = stringBuilder.ToString().Trim();
					if (!String.IsNullOrEmpty(str2))
					{
						strs.Add(str2);
					}
					stringBuilder.Clear();
				}
			}
			if (stringBuilder.Length > 0)
			{
				string str3 = stringBuilder.ToString().Trim();
				if (!String.IsNullOrEmpty(str3))
				{
					strs.Add(str3);
				}
			}
			if (strs.Count == 0)
			{
				command = null;
				args = null;
				return;
			}
			command = strs[0];
			strs.RemoveAt(0);
			args = strs.ToArray();
		}

		private bool PermissionsLoaded(IPlayer player)
		{
			if (this.permission.IsLoaded)
			{
				return true;
			}
			player.Reply(String.Format(this.lang.GetMessage("PermissionsNotLoaded", this, player.Id), this.permission.LastException.Message));
			return false;
		}

		[HookMethod("PluginsCommand")]
		private void PluginsCommand(IPlayer player, string command, string[] args)
		{
			string str;
			Plugin[] array = (
				from pl in this.pluginManager.GetPlugins()
				where !pl.IsCorePlugin
				select pl).ToArray<Plugin>();
			HashSet<string> strs = new HashSet<string>(
				from pl in (IEnumerable<Plugin>)array
				select pl.Name);
			Dictionary<string, string> strs1 = new Dictionary<string, string>();
			foreach (PluginLoader pluginLoader in Interface.Oxide.GetPluginLoaders())
			{
				foreach (string str1 in pluginLoader.ScanDirectory(Interface.Oxide.PluginDirectory).Except<string>(strs))
				{
					strs1[str1] = (pluginLoader.PluginErrors.TryGetValue(str1, out str) ? str : "Unloaded");
				}
			}
			if ((int)array.Length + strs1.Count < 1)
			{
				player.Reply(this.lang.GetMessage("NoPluginsFound", this, player.Id));
				return;
			}
			string str2 = String.Format("Listing {0} plugins:", (int)array.Length + strs1.Count);
			int num = 1;
			foreach (Plugin plugin in 
				from p in (IEnumerable<Plugin>)array
				where (object)p.Filename != (object)null
				select p)
			{
				System.Object[] title = new Object[6];
				int num1 = num;
				num = num1 + 1;
				title[0] = num1;
				title[1] = plugin.Title;
				title[2] = plugin.Version;
				title[3] = plugin.Author;
				title[4] = plugin.TotalHookTime;
				title[5] = plugin.Filename.Basename(null);
				str2 = String.Concat(str2, String.Format("\n  {0:00} \"{1}\" ({2}) by {3} ({4:0.00}s) - {5}", title));
			}
			foreach (string key in strs1.Keys)
			{
				int num2 = num;
				num = num2 + 1;
				str2 = String.Concat(str2, String.Format("\n  {0:00} {1} - {2}", num2, key, strs1[key]));
			}
			player.Reply(str2);
		}

		[HookMethod("ReloadCommand")]
		private void ReloadCommand(IPlayer player, string command, string[] args)
		{
			if ((int)args.Length < 1)
			{
				player.Reply(this.lang.GetMessage("CommandUsageReload", this, player.Id));
				return;
			}
			if (args[0].Equals("*") || args[0].Equals("all"))
			{
				Interface.Oxide.ReloadAllPlugins(null);
				return;
			}
			string[] strArray = args;
			for (int i = 0; i < (int)strArray.Length; i++)
			{
				string str = strArray[i];
				if (!String.IsNullOrEmpty(str))
				{
					Interface.Oxide.ReloadPlugin(str);
				}
			}
		}

		[HookMethod("RevokeCommand")]
		private void RevokeCommand(IPlayer player, string command, string[] args)
		{
			IPlayer player1;
			if (!this.PermissionsLoaded(player))
			{
				return;
			}
			if ((int)args.Length < 3)
			{
				player.Reply(this.lang.GetMessage("CommandUsageRevoke", this, player.Id));
				return;
			}
			string str = args[0];
			string name = args[1].Sanitize();
			string str1 = args[2];
			if (str.Equals("group"))
			{
				if (!this.permission.GroupExists(name))
				{
					player.Reply(String.Format(this.lang.GetMessage("GroupNotFound", this, player.Id), name));
					return;
				}
				if (!this.permission.GroupHasPermission(name, str1))
				{
					player.Reply(String.Format(this.lang.GetMessage("GroupDoesNotHavePermission", this, player.Id), (object)name, str1));
					return;
				}
				this.permission.RevokeGroupPermission(name, str1);
				player.Reply(String.Format(this.lang.GetMessage("GroupPermissionRevoked", this, player.Id), (object)name, str1));
				return;
			}
			if (!str.Equals("user"))
			{
				player.Reply(this.lang.GetMessage("CommandUsageRevoke", this, player.Id));
				return;
			}
			IPlayer[] array = RustCore.Covalence.PlayerManager.FindPlayers(name).ToArray<IPlayer>();
			if ((int)array.Length > 1)
			{
				player.Reply(String.Format(this.lang.GetMessage("PlayersFound", this, player.Id), String.Join(", ", (
					from p in (IEnumerable<IPlayer>)array
					select p.Name).ToArray<string>())));
				return;
			}
			if ((int)array.Length == 1)
			{
				player1 = array[0];
			}
			else
			{
				player1 = null;
			}
			IPlayer player2 = player1;
			if (player2 == null && !this.permission.UserIdValid(name))
			{
				player.Reply(String.Format(this.lang.GetMessage("PlayerNotFound", this, player.Id), name));
				return;
			}
			string id = name;
			if (player2 != null)
			{
				id = player2.Id;
				name = player2.Name;
				this.permission.UpdateNickname(id, name);
			}
			if (!this.permission.UserHasPermission(id, str1))
			{
				player.Reply(String.Format(this.lang.GetMessage("PlayerDoesNotHavePermission", this, player.Id), (object)name, str1));
				return;
			}
			this.permission.RevokeUserPermission(id, str1);
			player.Reply(String.Format(this.lang.GetMessage("PlayerPermissionRevoked", this, player.Id), (object)String.Concat(name, " (", id, ")"), str1));
		}

		[HookMethod("SaveCommand")]
		private void SaveCommand(IPlayer player, string command, string[] args)
		{
			if (this.PermissionsLoaded(player) && player.IsAdmin)
			{
				Interface.Oxide.OnSave();
				RustCore.Covalence.PlayerManager.SavePlayerData();
				player.Reply(this.lang.GetMessage("DataSaved", this, player.Id));
			}
		}

		[HookMethod("ShowCommand")]
		private void ShowCommand(IPlayer player, string command, string[] args)
		{
			IPlayer player1;
			if (!this.PermissionsLoaded(player))
			{
				return;
			}
			if ((int)args.Length < 1)
			{
				player.Reply(this.lang.GetMessage("CommandUsageShow", this, player.Id));
				player.Reply(this.lang.GetMessage("CommandUsageShowName", this, player.Id));
				return;
			}
			string str = args[0];
			string name = ((int)args.Length == 2 ? args[1].Sanitize() : String.Empty);
			if (str.Equals("perms"))
			{
				player.Reply(String.Format(String.Concat(this.lang.GetMessage("Permissions", this, player.Id), ":\n", String.Join(", ", this.permission.GetPermissions())), Array.Empty<object>()));
				return;
			}
			if (str.Equals("perm"))
			{
				if ((int)args.Length < 2 || String.IsNullOrEmpty(name))
				{
					player.Reply(this.lang.GetMessage("CommandUsageShow", this, player.Id));
					player.Reply(this.lang.GetMessage("CommandUsageShowName", this, player.Id));
					return;
				}
				string[] permissionUsers = this.permission.GetPermissionUsers(name);
				string[] permissionGroups = this.permission.GetPermissionGroups(name);
				string str1 = String.Concat(String.Format(this.lang.GetMessage("PermissionPlayers", this, player.Id), name), ":\n");
				str1 = String.Concat(str1, (permissionUsers.Length != 0 ? String.Join(", ", permissionUsers) : this.lang.GetMessage("NoPermissionPlayers", this, player.Id)));
				str1 = String.Concat(str1, "\n\n", String.Format(this.lang.GetMessage("PermissionGroups", this, player.Id), name), ":\n");
				player.Reply(String.Concat(str1, (permissionGroups.Length != 0 ? String.Join(", ", permissionGroups) : this.lang.GetMessage("NoPermissionGroups", this, player.Id))));
				return;
			}
			if (!str.Equals("user"))
			{
				if (!str.Equals("group"))
				{
					if (str.Equals("groups"))
					{
						player.Reply(String.Format(String.Concat(this.lang.GetMessage("Groups", this, player.Id), ":\n", String.Join(", ", this.permission.GetGroups())), Array.Empty<object>()));
						return;
					}
					player.Reply(this.lang.GetMessage("CommandUsageShow", this, player.Id));
					player.Reply(this.lang.GetMessage("CommandUsageShowName", this, player.Id));
					return;
				}
				if ((int)args.Length < 2 || String.IsNullOrEmpty(name))
				{
					player.Reply(this.lang.GetMessage("CommandUsageShow", this, player.Id));
					player.Reply(this.lang.GetMessage("CommandUsageShowName", this, player.Id));
					return;
				}
				if (!this.permission.GroupExists(name))
				{
					player.Reply(String.Format(this.lang.GetMessage("GroupNotFound", this, player.Id), name));
					return;
				}
				string[] usersInGroup = this.permission.GetUsersInGroup(name);
				string[] groupPermissions = this.permission.GetGroupPermissions(name, false);
				string str2 = String.Concat(String.Format(this.lang.GetMessage("GroupPlayers", this, player.Id), name), ":\n");
				str2 = String.Concat(str2, (usersInGroup.Length != 0 ? String.Join(", ", usersInGroup) : this.lang.GetMessage("NoPlayersInGroup", this, player.Id)));
				str2 = String.Concat(str2, "\n\n", String.Format(this.lang.GetMessage("GroupPermissions", this, player.Id), name), ":\n");
				str2 = String.Concat(str2, (groupPermissions.Length != 0 ? String.Join(", ", groupPermissions) : this.lang.GetMessage("NoGroupPermissions", this, player.Id)));
				for (string i = this.permission.GetGroupParent(name); this.permission.GroupExists(i); i = this.permission.GetGroupParent(i))
				{
					str2 = String.Concat(str2, "\n", String.Format(this.lang.GetMessage("ParentGroupPermissions", this, player.Id), i), ":\n");
					str2 = String.Concat(str2, String.Join(", ", this.permission.GetGroupPermissions(i, false)));
				}
				player.Reply(str2);
				return;
			}
			if ((int)args.Length < 2 || String.IsNullOrEmpty(name))
			{
				player.Reply(this.lang.GetMessage("CommandUsageShow", this, player.Id));
				player.Reply(this.lang.GetMessage("CommandUsageShowName", this, player.Id));
				return;
			}
			IPlayer[] array = RustCore.Covalence.PlayerManager.FindPlayers(name).ToArray<IPlayer>();
			if ((int)array.Length > 1)
			{
				player.Reply(String.Format(this.lang.GetMessage("PlayersFound", this, player.Id), String.Join(", ", (
					from p in (IEnumerable<IPlayer>)array
					select p.Name).ToArray<string>())));
				return;
			}
			if ((int)array.Length == 1)
			{
				player1 = array[0];
			}
			else
			{
				player1 = null;
			}
			IPlayer player2 = player1;
			if (player2 == null && !this.permission.UserIdValid(name))
			{
				player.Reply(String.Format(this.lang.GetMessage("PlayerNotFound", this, player.Id), name));
				return;
			}
			string id = name;
			if (player2 != null)
			{
				id = player2.Id;
				name = player2.Name;
				this.permission.UpdateNickname(id, name);
				name = String.Concat(name, " (", id, ")");
			}
			string[] userPermissions = this.permission.GetUserPermissions(id);
			string[] userGroups = this.permission.GetUserGroups(id);
			string str3 = String.Concat(String.Format(this.lang.GetMessage("PlayerPermissions", this, player.Id), name), ":\n");
			str3 = String.Concat(str3, (userPermissions.Length != 0 ? String.Join(", ", userPermissions) : this.lang.GetMessage("NoPlayerPermissions", this, player.Id)));
			str3 = String.Concat(str3, "\n\n", String.Format(this.lang.GetMessage("PlayerGroups", this, player.Id), name), ":\n");
			player.Reply(String.Concat(str3, (userGroups.Length != 0 ? String.Join(", ", userGroups) : this.lang.GetMessage("NoPlayerGroups", this, player.Id))));
		}

		[HookMethod("UnloadCommand")]
		private void UnloadCommand(IPlayer player, string command, string[] args)
		{
			if ((int)args.Length < 1)
			{
				player.Reply(this.lang.GetMessage("CommandUsageUnload", this, player.Id));
				return;
			}
			if (args[0].Equals("*") || args[0].Equals("all"))
			{
				Interface.Oxide.UnloadAllPlugins(null);
				return;
			}
			string[] strArray = args;
			for (int i = 0; i < (int)strArray.Length; i++)
			{
				string str = strArray[i];
				if (!String.IsNullOrEmpty(str))
				{
					Interface.Oxide.UnloadPlugin(str);
				}
			}
		}

		[HookMethod("UserGroupCommand")]
		private void UserGroupCommand(IPlayer player, string command, string[] args)
		{
			IPlayer player1;
			if (!this.PermissionsLoaded(player))
			{
				return;
			}
			if ((int)args.Length < 3)
			{
				player.Reply(this.lang.GetMessage("CommandUsageUserGroup", this, player.Id));
				return;
			}
			string str = args[0];
			string name = args[1].Sanitize();
			string str1 = args[2];
			IPlayer[] array = RustCore.Covalence.PlayerManager.FindPlayers(name).ToArray<IPlayer>();
			if ((int)array.Length > 1)
			{
				player.Reply(String.Format(this.lang.GetMessage("PlayersFound", this, player.Id), String.Join(", ", (
					from p in (IEnumerable<IPlayer>)array
					select p.Name).ToArray<string>())));
				return;
			}
			if ((int)array.Length == 1)
			{
				player1 = array[0];
			}
			else
			{
				player1 = null;
			}
			IPlayer player2 = player1;
			if (player2 == null && !this.permission.UserIdValid(name))
			{
				player.Reply(String.Format(this.lang.GetMessage("PlayerNotFound", this, player.Id), name));
				return;
			}
			string id = name;
			if (player2 != null)
			{
				id = player2.Id;
				name = player2.Name;
				this.permission.UpdateNickname(id, name);
				name = String.Concat(name, "(", id, ")");
			}
			if (!this.permission.GroupExists(str1))
			{
				player.Reply(String.Format(this.lang.GetMessage("GroupNotFound", this, player.Id), str1));
				return;
			}
			if (str.Equals("add"))
			{
				this.permission.AddUserGroup(id, str1);
				player.Reply(String.Format(this.lang.GetMessage("PlayerAddedToGroup", this, player.Id), (object)name, str1));
				return;
			}
			if (!str.Equals("remove"))
			{
				player.Reply(this.lang.GetMessage("CommandUsageUserGroup", this, player.Id));
				return;
			}
			this.permission.RemoveUserGroup(id, str1);
			player.Reply(String.Format(this.lang.GetMessage("PlayerRemovedFromGroup", this, player.Id), (object)name, str1));
		}

		[HookMethod("VersionCommand")]
		private void VersionCommand(IPlayer player, string command, string[] args)
		{
			if (player.IsServer)
			{
				player.Reply(String.Concat((object)"Oxide.Rust Version: ", RustExtension.AssemblyVersion));
				return;
			}
			string str = RustCore.Covalence.FormatText(this.lang.GetMessage("Version", this, player.Id));
			player.Reply(String.Format(str, new Object[] { RustExtension.AssemblyVersion, RustCore.Covalence.GameName, this.Server.Version, this.Server.Protocol }));
		}
	}
}