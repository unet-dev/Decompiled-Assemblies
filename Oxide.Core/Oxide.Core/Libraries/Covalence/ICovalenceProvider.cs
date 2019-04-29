using System;

namespace Oxide.Core.Libraries.Covalence
{
	public interface ICovalenceProvider
	{
		uint ClientAppId
		{
			get;
		}

		string GameName
		{
			get;
		}

		uint ServerAppId
		{
			get;
		}

		ICommandSystem CreateCommandSystemProvider();

		IPlayerManager CreatePlayerManager();

		IServer CreateServer();

		string FormatText(string text);
	}
}