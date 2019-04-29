using Network;
using Oxide.Core;
using Oxide.Core.Libraries;
using ProtoBuf;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Oxide.Game.Rust.Libraries
{
	public class Rust : Library
	{
		internal readonly Oxide.Game.Rust.Libraries.Player Player = new Oxide.Game.Rust.Libraries.Player();

		internal readonly Oxide.Game.Rust.Libraries.Server Server = new Oxide.Game.Rust.Libraries.Server();

		public override bool IsGlobal
		{
			get
			{
				return false;
			}
		}

		public Rust()
		{
		}

		[LibraryFunction("BroadcastChat")]
		public void BroadcastChat(string name, string message = null, string userId = "0")
		{
			this.Server.Broadcast(message, name, Convert.ToUInt64(userId), Array.Empty<object>());
		}

		[LibraryFunction("FindPlayer")]
		public BasePlayer FindPlayer(string nameOrIdOrIp)
		{
			return this.Player.Find(nameOrIdOrIp);
		}

		[LibraryFunction("FindPlayerById")]
		public BasePlayer FindPlayerById(ulong id)
		{
			return this.Player.FindById(id);
		}

		[LibraryFunction("FindPlayerByIdString")]
		public BasePlayer FindPlayerByIdString(string id)
		{
			return this.Player.FindById(id);
		}

		[LibraryFunction("FindPlayerByName")]
		public BasePlayer FindPlayerByName(string name)
		{
			return this.Player.Find(name);
		}

		[LibraryFunction("ForcePlayerPosition")]
		public void ForcePlayerPosition(BasePlayer player, float x, float y, float z)
		{
			this.Player.Teleport(player, x, y, z);
		}

		[LibraryFunction("OwnerIDFromEntity")]
		public string OwnerIDFromEntity(BaseEntity entity)
		{
			return entity.OwnerID.ToString();
		}

		[LibraryFunction("PrivateBindingFlag")]
		public BindingFlags PrivateBindingFlag()
		{
			return BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
		}

		[LibraryFunction("QuoteSafe")]
		public string QuoteSafe(string str)
		{
			return str.Quote();
		}

		[LibraryFunction("RunClientCommand")]
		public void RunClientCommand(BasePlayer player, string command, params object[] args)
		{
			this.Player.Command(player, command, args);
		}

		[LibraryFunction("RunServerCommand")]
		public void RunServerCommand(string command, params object[] args)
		{
			this.Server.Command(command, args);
		}

		[LibraryFunction("SendChatMessage")]
		public void SendChatMessage(BasePlayer player, string name, string message = null, string userId = "0")
		{
			this.Player.Message(player, message, name, Convert.ToUInt64(userId), Array.Empty<object>());
		}

		[LibraryFunction("UserIDFromConnection")]
		public string UserIDFromConnection(Connection connection)
		{
			return connection.userid.ToString();
		}

		[LibraryFunction("UserIDFromPlayer")]
		public string UserIDFromPlayer(BasePlayer player)
		{
			return player.UserIDString;
		}

		[LibraryFunction("UserIDsFromBuildingPrivilege")]
		public Array UserIDsFromBuildingPrivlidge(BuildingPrivlidge priv)
		{
			return (
				from eid in priv.authorizedPlayers
				select eid.userid.ToString()).ToArray<string>();
		}
	}
}