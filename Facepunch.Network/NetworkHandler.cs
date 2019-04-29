using Network.Visibility;
using System;
using System.Collections.Generic;

namespace Network
{
	public interface NetworkHandler
	{
		void OnNetworkGroupChange();

		void OnNetworkGroupEnter(Group group);

		void OnNetworkGroupLeave(Group group);

		void OnNetworkSubscribersEnter(List<Connection> connections);

		void OnNetworkSubscribersLeave(List<Connection> connections);
	}
}