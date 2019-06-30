using System;

namespace Network
{
	public interface IClientCallback
	{
		void OnClientDisconnected(string reason);

		void OnNetworkMessage(Message message);
	}
}