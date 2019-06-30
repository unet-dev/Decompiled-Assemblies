using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Libraries.Covalence;
using Oxide.Game.Rust.Libraries;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Oxide.Game.Rust.Libraries.Covalence
{
	public class RustPlayer : IPlayer, IEquatable<IPlayer>
	{
		internal readonly Oxide.Game.Rust.Libraries.Player Player = new Oxide.Game.Rust.Libraries.Player();

		private static Permission libPerms;

		private readonly BasePlayer player;

		private readonly ulong steamId;

		public string Address
		{
			get
			{
				return this.Player.Address(this.player);
			}
		}

		public TimeSpan BanTimeRemaining
		{
			get
			{
				if (!this.IsBanned)
				{
					return TimeSpan.Zero;
				}
				return TimeSpan.MaxValue;
			}
		}

		public float Health
		{
			get
			{
				return this.player.health;
			}
			set
			{
				this.player.health = value;
			}
		}

		public string Id
		{
			get;
		}

		public bool IsAdmin
		{
			get
			{
				return this.Player.IsAdmin(this.steamId);
			}
		}

		public bool IsBanned
		{
			get
			{
				return this.Player.IsBanned(this.steamId);
			}
		}

		public bool IsConnected
		{
			get
			{
				return this.Player.IsConnected(this.player);
			}
		}

		public bool IsServer
		{
			get
			{
				return false;
			}
		}

		public bool IsSleeping
		{
			get
			{
				return this.Player.IsSleeping(this.player);
			}
		}

		public CultureInfo Language
		{
			get
			{
				return this.Player.Language(this.player);
			}
		}

		public CommandType LastCommand
		{
			get;
			set;
		}

		public float MaxHealth
		{
			get
			{
				return this.player.MaxHealth();
			}
			set
			{
				this.player._maxHealth = value;
			}
		}

		public string Name
		{
			get;
			set;
		}

		public object Object
		{
			get
			{
				return this.player;
			}
		}

		public int Ping
		{
			get
			{
				return this.Player.Ping(this.player);
			}
		}

		internal RustPlayer(ulong id, string name)
		{
			if (RustPlayer.libPerms == null)
			{
				RustPlayer.libPerms = Interface.Oxide.GetLibrary<Permission>(null);
			}
			this.steamId = id;
			this.Name = name.Sanitize();
			this.Id = id.ToString();
		}

		internal RustPlayer(BasePlayer player) : this(player.userID, player.displayName)
		{
			this.player = player;
		}

		public void AddToGroup(string group)
		{
			RustPlayer.libPerms.AddUserGroup(this.Id, group);
		}

		public void Ban(string reason, TimeSpan duration = null)
		{
			this.Player.Ban(this.steamId, reason);
		}

		public bool BelongsToGroup(string group)
		{
			return RustPlayer.libPerms.UserHasGroup(this.Id, group);
		}

		public void Command(string command, params object[] args)
		{
			this.player.SendConsoleCommand(command, args);
		}

		public bool Equals(IPlayer other)
		{
			string id;
			string str = this.Id;
			if (other != null)
			{
				id = other.Id;
			}
			else
			{
				id = null;
			}
			return str == id;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is IPlayer))
			{
				return false;
			}
			return this.Id == ((IPlayer)obj).Id;
		}

		public override int GetHashCode()
		{
			return this.Id.GetHashCode();
		}

		public void GrantPermission(string perm)
		{
			RustPlayer.libPerms.GrantUserPermission(this.Id, perm, null);
		}

		public bool HasPermission(string perm)
		{
			return RustPlayer.libPerms.UserHasPermission(this.Id, perm);
		}

		public void Heal(float amount)
		{
			this.Player.Heal(this.player, amount);
		}

		public void Hurt(float amount)
		{
			this.Player.Hurt(this.player, amount);
		}

		public void Kick(string reason)
		{
			this.Player.Kick(this.player, reason);
		}

		public void Kill()
		{
			this.Player.Kill(this.player);
		}

		public void Message(string message, string prefix, params object[] args)
		{
			this.Player.Message(this.player, message, prefix, (ulong)0, args);
		}

		public void Message(string message)
		{
			this.Message(message, null, Array.Empty<object>());
		}

		public void Position(out float x, out float y, out float z)
		{
			Vector3 vector3 = this.Player.Position(this.player);
			x = vector3.x;
			y = vector3.y;
			z = vector3.z;
		}

		public GenericPosition Position()
		{
			Vector3 vector3 = this.Player.Position(this.player);
			return new GenericPosition(vector3.x, vector3.y, vector3.z);
		}

		public void RemoveFromGroup(string group)
		{
			RustPlayer.libPerms.RemoveUserGroup(this.Id, group);
		}

		public void Rename(string name)
		{
			this.Player.Rename(this.player, name);
		}

		public void Reply(string message, string prefix, params object[] args)
		{
			CommandType lastCommand = this.LastCommand;
			if (lastCommand == CommandType.Chat)
			{
				this.Message(message, prefix, args);
				return;
			}
			if (lastCommand != CommandType.Console)
			{
				return;
			}
			this.player.ConsoleMessage(String.Format(Formatter.ToPlaintext(message), args));
		}

		public void Reply(string message)
		{
			this.Reply(message, null, Array.Empty<object>());
		}

		public void RevokePermission(string perm)
		{
			RustPlayer.libPerms.RevokeUserPermission(this.Id, perm);
		}

		public void Teleport(float x, float y, float z)
		{
			this.Player.Teleport(this.player, x, y, z);
		}

		public void Teleport(GenericPosition pos)
		{
			this.Teleport(pos.X, pos.Y, pos.Z);
		}

		public override string ToString()
		{
			return String.Concat(new String[] { "Covalence.RustPlayer[", this.Id, ", ", this.Name, "]" });
		}

		public void Unban()
		{
			this.Player.Unban(this.steamId);
		}
	}
}