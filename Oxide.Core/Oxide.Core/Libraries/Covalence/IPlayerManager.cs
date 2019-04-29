using System;
using System.Collections.Generic;

namespace Oxide.Core.Libraries.Covalence
{
	public interface IPlayerManager
	{
		IEnumerable<IPlayer> All
		{
			get;
		}

		IEnumerable<IPlayer> Connected
		{
			get;
		}

		IPlayer FindPlayer(string partialNameOrId);

		IPlayer FindPlayerById(string id);

		IPlayer FindPlayerByObj(object obj);

		IEnumerable<IPlayer> FindPlayers(string partialNameOrId);
	}
}