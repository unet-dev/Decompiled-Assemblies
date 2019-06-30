using Oxide.Core;
using Oxide.Core.Libraries.Covalence;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Oxide.Game.Rust.Libraries.Covalence
{
	public class RustConsolePlayer : IPlayer
	{
		public string Address
		{
			get
			{
				return "127.0.0.1";
			}
		}

		public TimeSpan BanTimeRemaining
		{
			get
			{
				return TimeSpan.Zero;
			}
		}

		public float Health
		{
			get;
			set;
		}

		public string Id
		{
			get
			{
				return "server_console";
			}
		}

		public bool IsAdmin
		{
			get
			{
				return true;
			}
		}

		public bool IsBanned
		{
			get
			{
				return false;
			}
		}

		public bool IsConnected
		{
			get
			{
				return true;
			}
		}

		public bool IsServer
		{
			get
			{
				return true;
			}
		}

		public bool IsSleeping
		{
			get
			{
				return false;
			}
		}

		public CultureInfo Language
		{
			get
			{
				return CultureInfo.InstalledUICulture;
			}
		}

		public CommandType LastCommand
		{
			get
			{
				return CommandType.Console;
			}
			set
			{
			}
		}

		public float MaxHealth
		{
			get;
			set;
		}

		public string Name
		{
			get
			{
				return "Server Console";
			}
			set
			{
			}
		}

		public object Object
		{
			get
			{
				return null;
			}
		}

		public int Ping
		{
			get
			{
				return 0;
			}
		}

		public RustConsolePlayer()
		{
		}

		public void AddToGroup(string group)
		{
		}

		public void Ban(string reason, TimeSpan duration)
		{
		}

		public bool BelongsToGroup(string group)
		{
			return false;
		}

		public void Command(string command, params object[] args)
		{
			ConsoleSystem.Run(ConsoleSystem.Option.Server, command, args);
		}

		public void GrantPermission(string perm)
		{
		}

		public bool HasPermission(string perm)
		{
			return true;
		}

		public void Heal(float amount)
		{
		}

		public void Hurt(float amount)
		{
		}

		public void Kick(string reason)
		{
		}

		public void Kill()
		{
		}

		public void Message(string message, string prefix, params object[] args)
		{
			message = (args.Length != 0 ? String.Format(Formatter.ToPlaintext(message), args) : Formatter.ToPlaintext(message));
			Interface.Oxide.LogInfo((prefix != null ? String.Concat(prefix, " ", message) : message), Array.Empty<object>());
		}

		public void Message(string message)
		{
			this.Message(message, null, Array.Empty<object>());
		}

		public void Position(out float x, out float y, out float z)
		{
			x = 0f;
			y = 0f;
			z = 0f;
		}

		public GenericPosition Position()
		{
			return new GenericPosition(0f, 0f, 0f);
		}

		public void RemoveFromGroup(string group)
		{
		}

		public void Rename(string name)
		{
		}

		public void Reply(string message, string prefix, params object[] args)
		{
			this.Message(message, prefix, args);
		}

		public void Reply(string message)
		{
			this.Message(message, null, Array.Empty<object>());
		}

		public void RevokePermission(string perm)
		{
		}

		public void Teleport(float x, float y, float z)
		{
		}

		public void Teleport(GenericPosition pos)
		{
			this.Teleport(pos.X, pos.Y, pos.Z);
		}

		public void Unban()
		{
		}
	}
}