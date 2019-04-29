using System;
using System.Globalization;

namespace Oxide.Core.Libraries.Covalence
{
	public interface IPlayer
	{
		string Address
		{
			get;
		}

		TimeSpan BanTimeRemaining
		{
			get;
		}

		float Health
		{
			get;
			set;
		}

		string Id
		{
			get;
		}

		bool IsAdmin
		{
			get;
		}

		bool IsBanned
		{
			get;
		}

		bool IsConnected
		{
			get;
		}

		bool IsServer
		{
			get;
		}

		bool IsSleeping
		{
			get;
		}

		CultureInfo Language
		{
			get;
		}

		CommandType LastCommand
		{
			get;
			set;
		}

		float MaxHealth
		{
			get;
			set;
		}

		string Name
		{
			get;
			set;
		}

		object Object
		{
			get;
		}

		int Ping
		{
			get;
		}

		void AddToGroup(string group);

		void Ban(string reason, TimeSpan duration = null);

		bool BelongsToGroup(string group);

		void Command(string command, params object[] args);

		void GrantPermission(string perm);

		bool HasPermission(string perm);

		void Heal(float amount);

		void Hurt(float amount);

		void Kick(string reason);

		void Kill();

		void Message(string message, string prefix, params object[] args);

		void Message(string message);

		void Position(out float x, out float y, out float z);

		GenericPosition Position();

		void RemoveFromGroup(string group);

		void Rename(string name);

		void Reply(string message, string prefix, params object[] args);

		void Reply(string message);

		void RevokePermission(string perm);

		void Teleport(float x, float y, float z);

		void Teleport(GenericPosition pos);

		void Unban();
	}
}