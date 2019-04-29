using System;
using System.Globalization;
using System.Net;

namespace Oxide.Core.Libraries.Covalence
{
	public interface IServer
	{
		IPAddress Address
		{
			get;
		}

		CultureInfo Language
		{
			get;
		}

		IPAddress LocalAddress
		{
			get;
		}

		int MaxPlayers
		{
			get;
			set;
		}

		string Name
		{
			get;
			set;
		}

		int Players
		{
			get;
		}

		ushort Port
		{
			get;
		}

		string Protocol
		{
			get;
		}

		Oxide.Core.Libraries.Covalence.SaveInfo SaveInfo
		{
			get;
		}

		DateTime Time
		{
			get;
			set;
		}

		string Version
		{
			get;
		}

		void Ban(string id, string reason, TimeSpan duration = null);

		TimeSpan BanTimeRemaining(string id);

		void Broadcast(string message, string prefix, params object[] args);

		void Broadcast(string message);

		void Command(string command, params object[] args);

		bool IsBanned(string id);

		void Save();

		void Unban(string id);
	}
}