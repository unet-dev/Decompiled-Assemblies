using Network;
using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Libraries.Covalence;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Oxide.Game.Rust.Libraries
{
	public class Player : Library
	{
		private static string ipPattern;

		internal readonly Permission permission = Interface.Oxide.GetLibrary<Permission>(null);

		public List<BasePlayer> Players
		{
			get
			{
				return BasePlayer.activePlayerList;
			}
		}

		public List<BasePlayer> Sleepers
		{
			get
			{
				return BasePlayer.sleepingPlayerList;
			}
		}

		static Player()
		{
			Player.ipPattern = ":{1}[0-9]{1}\\d*";
		}

		public Player()
		{
		}

		public string Address(Connection connection)
		{
			return Regex.Replace(connection.ipaddress, Player.ipPattern, "");
		}

		public string Address(BasePlayer player)
		{
			bool flag;
			if (player != null)
			{
				Networkable networkable = player.net;
				if (networkable != null)
				{
					flag = networkable.connection;
				}
				else
				{
					flag = false;
				}
			}
			else
			{
				flag = false;
			}
			if (!flag)
			{
				return null;
			}
			return this.Address(player.net.connection);
		}

		public void Ban(ulong id, string reason = "")
		{
			object obj;
			if (this.IsBanned(id))
			{
				return;
			}
			BasePlayer basePlayer = this.FindById(id);
			ulong num = id;
			if (basePlayer != null)
			{
				obj = basePlayer.displayName;
			}
			else
			{
				obj = null;
			}
			if (obj == null)
			{
				obj = "Unknown";
			}
			ServerUsers.Set(num, ServerUsers.UserGroup.Banned, (string)obj, reason);
			ServerUsers.Save();
			if (basePlayer != null && this.IsConnected(basePlayer))
			{
				this.Kick(basePlayer, reason);
			}
		}

		public void Ban(string id, string reason = "")
		{
			this.Ban(Convert.ToUInt64(id), reason);
		}

		public void Ban(BasePlayer player, string reason = "")
		{
			this.Ban(player.UserIDString, reason);
		}

		public void ClearInventory(BasePlayer player)
		{
			PlayerInventory playerInventory = this.Inventory(player);
			if (playerInventory == null)
			{
				return;
			}
			playerInventory.Strip();
		}

		public void Command(BasePlayer player, string command, params object[] args)
		{
			player.SendConsoleCommand(command, args);
		}

		public void DropItem(BasePlayer player, int itemId)
		{
			Quaternion quaternion;
			Vector3 vector3 = player.transform.position;
			PlayerInventory playerInventory = this.Inventory(player);
			for (int i = 0; i < playerInventory.containerMain.capacity; i++)
			{
				Item slot = playerInventory.containerMain.GetSlot(i);
				if (slot.info.itemid == itemId)
				{
					Vector3 vector31 = (vector3 + new Vector3(0f, 1f, 0f)) + (vector3 / 2f);
					Vector3 vector32 = (vector3 + new Vector3(0f, 0.2f, 0f)) * 8f;
					quaternion = new Quaternion();
					slot.Drop(vector31, vector32, quaternion);
				}
			}
			for (int j = 0; j < playerInventory.containerBelt.capacity; j++)
			{
				Item item = playerInventory.containerBelt.GetSlot(j);
				if (item.info.itemid == itemId)
				{
					Vector3 vector33 = (vector3 + new Vector3(0f, 1f, 0f)) + (vector3 / 2f);
					Vector3 vector34 = (vector3 + new Vector3(0f, 0.2f, 0f)) * 8f;
					quaternion = new Quaternion();
					item.Drop(vector33, vector34, quaternion);
				}
			}
			for (int k = 0; k < playerInventory.containerWear.capacity; k++)
			{
				Item slot1 = playerInventory.containerWear.GetSlot(k);
				if (slot1.info.itemid == itemId)
				{
					Vector3 vector35 = (vector3 + new Vector3(0f, 1f, 0f)) + (vector3 / 2f);
					Vector3 vector36 = (vector3 + new Vector3(0f, 0.2f, 0f)) * 8f;
					quaternion = new Quaternion();
					slot1.Drop(vector35, vector36, quaternion);
				}
			}
		}

		public void DropItem(BasePlayer player, Item item)
		{
			Quaternion quaternion;
			Vector3 vector3 = player.transform.position;
			PlayerInventory playerInventory = this.Inventory(player);
			for (int i = 0; i < playerInventory.containerMain.capacity; i++)
			{
				Item slot = playerInventory.containerMain.GetSlot(i);
				if (slot == item)
				{
					Vector3 vector31 = (vector3 + new Vector3(0f, 1f, 0f)) + (vector3 / 2f);
					Vector3 vector32 = (vector3 + new Vector3(0f, 0.2f, 0f)) * 8f;
					quaternion = new Quaternion();
					slot.Drop(vector31, vector32, quaternion);
				}
			}
			for (int j = 0; j < playerInventory.containerBelt.capacity; j++)
			{
				Item slot1 = playerInventory.containerBelt.GetSlot(j);
				if (slot1 == item)
				{
					Vector3 vector33 = (vector3 + new Vector3(0f, 1f, 0f)) + (vector3 / 2f);
					Vector3 vector34 = (vector3 + new Vector3(0f, 0.2f, 0f)) * 8f;
					quaternion = new Quaternion();
					slot1.Drop(vector33, vector34, quaternion);
				}
			}
			for (int k = 0; k < playerInventory.containerWear.capacity; k++)
			{
				Item item1 = playerInventory.containerWear.GetSlot(k);
				if (item1 == item)
				{
					Vector3 vector35 = (vector3 + new Vector3(0f, 1f, 0f)) + (vector3 / 2f);
					Vector3 vector36 = (vector3 + new Vector3(0f, 0.2f, 0f)) * 8f;
					quaternion = new Quaternion();
					item1.Drop(vector35, vector36, quaternion);
				}
			}
		}

		public BasePlayer Find(string nameOrIdOrIp)
		{
			BasePlayer basePlayer;
			List<BasePlayer>.Enumerator enumerator = this.Players.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BasePlayer current = enumerator.Current;
					if (!nameOrIdOrIp.Equals(current.displayName, StringComparison.OrdinalIgnoreCase) && !nameOrIdOrIp.Equals(current.UserIDString) && !nameOrIdOrIp.Equals(current.net.connection.ipaddress))
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

		public BasePlayer FindById(string id)
		{
			BasePlayer basePlayer;
			List<BasePlayer>.Enumerator enumerator = this.Players.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BasePlayer current = enumerator.Current;
					if (!id.Equals(current.UserIDString))
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

		public BasePlayer FindById(ulong id)
		{
			BasePlayer basePlayer;
			List<BasePlayer>.Enumerator enumerator = this.Players.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BasePlayer current = enumerator.Current;
					if (!id.Equals(current.userID))
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

		public void GiveItem(BasePlayer player, int itemId, int quantity = 1)
		{
			this.GiveItem(player, Item.GetItem(itemId), quantity);
		}

		public void GiveItem(BasePlayer player, Item item, int quantity = 1)
		{
			player.inventory.GiveItem(ItemManager.CreateByItemID(item.info.itemid, quantity, (ulong)0), null);
		}

		public void Heal(BasePlayer player, float amount)
		{
			player.Heal(amount);
		}

		public void Hurt(BasePlayer player, float amount)
		{
			player.Hurt(amount);
		}

		public PlayerInventory Inventory(BasePlayer player)
		{
			return player.inventory;
		}

		public bool IsAdmin(ulong id)
		{
			return ServerUsers.Is(id, ServerUsers.UserGroup.Owner);
		}

		public bool IsAdmin(string id)
		{
			return this.IsAdmin(Convert.ToUInt64(id));
		}

		public bool IsAdmin(BasePlayer player)
		{
			return this.IsAdmin(player.userID);
		}

		public bool IsBanned(ulong id)
		{
			return ServerUsers.Is(id, ServerUsers.UserGroup.Banned);
		}

		public bool IsBanned(string id)
		{
			return this.IsBanned(Convert.ToUInt64(id));
		}

		public bool IsBanned(BasePlayer player)
		{
			return this.IsBanned(player.userID);
		}

		public bool IsConnected(BasePlayer player)
		{
			return BasePlayer.activePlayerList.Contains(player);
		}

		public bool IsSleeping(ulong id)
		{
			return BasePlayer.FindSleeping(id);
		}

		public bool IsSleeping(string id)
		{
			return this.IsSleeping(Convert.ToUInt64(id));
		}

		public bool IsSleeping(BasePlayer player)
		{
			return this.IsSleeping(player.userID);
		}

		public void Kick(BasePlayer player, string reason = "")
		{
			player.Kick(reason);
		}

		public void Kill(BasePlayer player)
		{
			player.Die(null);
		}

		public CultureInfo Language(BasePlayer player)
		{
			return CultureInfo.GetCultureInfo(player.net.connection.info.GetString("global.language", "") ?? "en");
		}

		public void Message(BasePlayer player, string message, string prefix, ulong userId = 0L, params object[] args)
		{
			if (string.IsNullOrEmpty(message))
			{
				return;
			}
			message = (args.Length != 0 ? string.Format(Formatter.ToUnity(message), args) : Formatter.ToUnity(message));
			string str = (prefix != null ? string.Concat(prefix, " ", message) : message);
			player.SendConsoleCommand("chat.add", new object[] { userId, str, 1 });
		}

		public void Message(BasePlayer player, string message, ulong userId = 0L)
		{
			this.Message(player, message, null, userId, Array.Empty<object>());
		}

		public int Ping(Connection connection)
		{
			return Net.sv.GetAveragePing(connection);
		}

		public int Ping(BasePlayer player)
		{
			return this.Ping(player.net.connection);
		}

		public Vector3 Position(BasePlayer player)
		{
			return player.transform.position;
		}

		public void Rename(BasePlayer player, string name)
		{
			name = (string.IsNullOrEmpty(name.Trim()) ? player.displayName : name);
			player.net.connection.username = name;
			player.displayName = name;
			player._name = name;
			player.SendNetworkUpdateImmediate(false);
			player.IPlayer.Name = name;
			this.permission.UpdateNickname(player.UserIDString, name);
		}

		public void Reply(BasePlayer player, string message, string prefix, ulong userId = 0L, params object[] args)
		{
			this.Message(player, message, prefix, userId, args);
		}

		public void Reply(BasePlayer player, string message, ulong userId = 0L)
		{
			this.Message(player, message, null, userId, Array.Empty<object>());
		}

		public void ResetInventory(BasePlayer player)
		{
			PlayerInventory playerInventory = this.Inventory(player);
			if (playerInventory != null)
			{
				playerInventory.DoDestroy();
				playerInventory.ServerInit(player);
			}
		}

		public void Teleport(BasePlayer player, Vector3 destination)
		{
			if (player.IsSpectating())
			{
				return;
			}
			player.MovePosition(destination);
			player.ClientRPCPlayer<Vector3>(null, player, "ForcePositionTo", destination);
		}

		public void Teleport(BasePlayer player, BasePlayer target)
		{
			this.Teleport(player, this.Position(target));
		}

		public void Teleport(BasePlayer player, float x, float y, float z)
		{
			this.Teleport(player, new Vector3(x, y, z));
		}

		public void Unban(ulong id)
		{
			if (!this.IsBanned(id))
			{
				return;
			}
			ServerUsers.Remove(id);
			ServerUsers.Save();
		}

		public void Unban(string id)
		{
			this.Unban(Convert.ToUInt64(id));
		}

		public void Unban(BasePlayer player)
		{
			this.Unban(player.userID);
		}
	}
}