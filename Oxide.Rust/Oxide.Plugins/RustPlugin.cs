using Network;
using Oxide.Core;
using Oxide.Core.Plugins;
using Oxide.Game.Rust.Libraries;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Oxide.Plugins
{
	public abstract class RustPlugin : CSharpPlugin
	{
		protected Command cmd = Interface.Oxide.GetLibrary<Command>(null);

		protected Rust rust = Interface.Oxide.GetLibrary<Rust>(null);

		protected Oxide.Game.Rust.Libraries.Item Item = Interface.Oxide.GetLibrary<Oxide.Game.Rust.Libraries.Item>(null);

		protected Oxide.Game.Rust.Libraries.Player Player = Interface.Oxide.GetLibrary<Oxide.Game.Rust.Libraries.Player>(null);

		protected Oxide.Game.Rust.Libraries.Server Server = Interface.Oxide.GetLibrary<Oxide.Game.Rust.Libraries.Server>(null);

		protected RustPlugin()
		{
		}

		private void AddOnlinePlayer(BasePlayer player)
		{
			foreach (CSharpPlugin.PluginFieldInfo onlinePlayerField in this.onlinePlayerFields)
			{
				Type genericArguments = onlinePlayerField.GenericArguments[1];
				object obj = (genericArguments.GetConstructor(new Type[] { typeof(BasePlayer) }) == null ? Activator.CreateInstance(genericArguments) : Activator.CreateInstance(genericArguments, new object[] { player }));
				genericArguments.GetField("Player").SetValue(obj, player);
				onlinePlayerField.Call("Add", new object[] { player, obj });
			}
		}

		[HookMethod("OnPlayerDisconnected")]
		private void base_OnPlayerDisconnected(BasePlayer player, string reason)
		{
			base.NextTick(() => {
				foreach (CSharpPlugin.PluginFieldInfo onlinePlayerField in this.onlinePlayerFields)
				{
					onlinePlayerField.Call("Remove", new object[] { player });
				}
			});
		}

		[HookMethod("OnPlayerInit")]
		private void base_OnPlayerInit(BasePlayer player)
		{
			this.AddOnlinePlayer(player);
		}

		protected void ForcePlayerPosition(BasePlayer player, Vector3 destination)
		{
			player.MovePosition(destination);
			if (player.IsSpectating() && (double)Vector3.Distance(player.transform.position, destination) <= 25)
			{
				player.SendNetworkUpdate(BasePlayer.NetworkQueue.UpdateDistance);
				return;
			}
			player.ClientRPCPlayer<Vector3>(null, player, "ForcePositionTo", destination);
		}

		public override void HandleAddedToManager(PluginManager manager)
		{
			int i;
			FieldInfo[] fields = base.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
			for (i = 0; i < (int)fields.Length; i++)
			{
				FieldInfo fieldInfo = fields[i];
				if (fieldInfo.GetCustomAttributes(typeof(OnlinePlayersAttribute), true).Length != 0)
				{
					CSharpPlugin.PluginFieldInfo pluginFieldInfo = new CSharpPlugin.PluginFieldInfo(this, fieldInfo);
					if ((int)pluginFieldInfo.GenericArguments.Length != 2 || pluginFieldInfo.GenericArguments[0] != typeof(BasePlayer))
					{
						base.Puts(string.Concat("The ", fieldInfo.Name, " field is not a Hash with a BasePlayer key! (online players will not be tracked)"), Array.Empty<object>());
					}
					else if (!pluginFieldInfo.LookupMethod("Add", pluginFieldInfo.GenericArguments))
					{
						base.Puts(string.Concat("The ", fieldInfo.Name, " field does not support adding BasePlayer keys! (online players will not be tracked)"), Array.Empty<object>());
					}
					else if (!pluginFieldInfo.LookupMethod("Remove", new Type[] { typeof(BasePlayer) }))
					{
						base.Puts(string.Concat("The ", fieldInfo.Name, " field does not support removing BasePlayer keys! (online players will not be tracked)"), Array.Empty<object>());
					}
					else if (pluginFieldInfo.GenericArguments[1].GetField("Player") == null)
					{
						base.Puts(string.Concat("The ", pluginFieldInfo.GenericArguments[1].Name, " class does not have a public Player field! (online players will not be tracked)"), Array.Empty<object>());
					}
					else if (pluginFieldInfo.HasValidConstructor(new Type[] { typeof(BasePlayer) }))
					{
						this.onlinePlayerFields.Add(pluginFieldInfo);
					}
					else
					{
						base.Puts(string.Concat("The ", fieldInfo.Name, " field is using a class which contains no valid constructor (online players will not be tracked)"), Array.Empty<object>());
					}
				}
			}
			MethodInfo[] methods = base.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
			for (i = 0; i < (int)methods.Length; i++)
			{
				MethodInfo methodInfo = methods[i];
				object[] customAttributes = methodInfo.GetCustomAttributes(typeof(ConsoleCommandAttribute), true);
				if (customAttributes.Length == 0)
				{
					customAttributes = methodInfo.GetCustomAttributes(typeof(ChatCommandAttribute), true);
					if (customAttributes.Length != 0)
					{
						ChatCommandAttribute chatCommandAttribute = customAttributes[0] as ChatCommandAttribute;
						if (chatCommandAttribute != null)
						{
							this.cmd.AddChatCommand(chatCommandAttribute.Command, this, methodInfo.Name);
						}
					}
				}
				else
				{
					ConsoleCommandAttribute consoleCommandAttribute = customAttributes[0] as ConsoleCommandAttribute;
					if (consoleCommandAttribute != null)
					{
						this.cmd.AddConsoleCommand(consoleCommandAttribute.Command, this, methodInfo.Name);
					}
				}
			}
			if (this.onlinePlayerFields.Count > 0)
			{
				foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
				{
					this.AddOnlinePlayer(basePlayer);
				}
			}
			base.HandleAddedToManager(manager);
		}

		protected void PrintToChat(BasePlayer player, string format, params object[] args)
		{
			bool flag;
			if (player != null)
			{
				flag = player.net;
			}
			else
			{
				flag = false;
			}
			if (flag)
			{
				BasePlayer basePlayer = player;
				object[] objArray = new object[] { 0, null, null };
				objArray[1] = (args.Length != 0 ? string.Format(format, args) : format);
				objArray[2] = 1f;
				basePlayer.SendConsoleCommand("chat.add", objArray);
			}
		}

		protected void PrintToChat(string format, params object[] args)
		{
			if (BasePlayer.activePlayerList.Count >= 1)
			{
				object[] objArray = new object[] { 0, null, null };
				objArray[1] = (args.Length != 0 ? string.Format(format, args) : format);
				objArray[2] = 1f;
				ConsoleNetwork.BroadcastToAllClients("chat.add", objArray);
			}
		}

		protected void PrintToConsole(BasePlayer player, string format, params object[] args)
		{
			bool flag;
			if (player != null)
			{
				flag = player.net;
			}
			else
			{
				flag = false;
			}
			if (flag)
			{
				player.SendConsoleCommand(string.Concat("echo ", (args.Length != 0 ? string.Format(format, args) : format)), Array.Empty<object>());
			}
		}

		protected void PrintToConsole(string format, params object[] args)
		{
			if (BasePlayer.activePlayerList.Count >= 1)
			{
				ConsoleNetwork.BroadcastToAllClients(string.Concat("echo ", (args.Length != 0 ? string.Format(format, args) : format)), Array.Empty<object>());
			}
		}

		protected void SendError(ConsoleSystem.Arg arg, string format, params object[] args)
		{
			object obj;
			bool flag;
			Connection connection = arg.Connection;
			if (connection != null)
			{
				obj = connection.player;
			}
			else
			{
				obj = null;
			}
			BasePlayer basePlayer = obj as BasePlayer;
			string str = (args.Length != 0 ? string.Format(format, args) : format);
			if (basePlayer != null)
			{
				flag = basePlayer.net;
			}
			else
			{
				flag = false;
			}
			if (!flag)
			{
				Debug.LogError(str);
				return;
			}
			basePlayer.SendConsoleCommand(string.Concat("echo ", str), Array.Empty<object>());
		}

		protected void SendReply(ConsoleSystem.Arg arg, string format, params object[] args)
		{
			object obj;
			bool flag;
			Connection connection = arg.Connection;
			if (connection != null)
			{
				obj = connection.player;
			}
			else
			{
				obj = null;
			}
			BasePlayer basePlayer = obj as BasePlayer;
			string str = (args.Length != 0 ? string.Format(format, args) : format);
			if (basePlayer != null)
			{
				flag = basePlayer.net;
			}
			else
			{
				flag = false;
			}
			if (!flag)
			{
				base.Puts(str, Array.Empty<object>());
				return;
			}
			basePlayer.SendConsoleCommand(string.Concat("echo ", str), Array.Empty<object>());
		}

		protected void SendReply(BasePlayer player, string format, params object[] args)
		{
			this.PrintToChat(player, format, args);
		}

		protected void SendWarning(ConsoleSystem.Arg arg, string format, params object[] args)
		{
			object obj;
			bool flag;
			Connection connection = arg.Connection;
			if (connection != null)
			{
				obj = connection.player;
			}
			else
			{
				obj = null;
			}
			BasePlayer basePlayer = obj as BasePlayer;
			string str = (args.Length != 0 ? string.Format(format, args) : format);
			if (basePlayer != null)
			{
				flag = basePlayer.net;
			}
			else
			{
				flag = false;
			}
			if (!flag)
			{
				Debug.LogWarning(str);
				return;
			}
			basePlayer.SendConsoleCommand(string.Concat("echo ", str), Array.Empty<object>());
		}
	}
}