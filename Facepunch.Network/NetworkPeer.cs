using Facepunch;
using System;

namespace Network
{
	public abstract class NetworkPeer
	{
		public Write write;

		public Read read;

		public INetworkCryptocraphy cryptography;

		protected NetworkPeer()
		{
		}

		public abstract string GetDebug(Connection connection);

		public abstract ulong GetStat(Connection connection, NetworkPeer.StatTypeLong type);

		protected Message StartMessage(Message.Type type, Connection connection)
		{
			Message message = Pool.Get<Message>();
			message.peer = this;
			message.type = type;
			message.connection = connection;
			return message;
		}

		public enum StatTypeLong
		{
			BytesSent,
			BytesSent_LastSecond,
			BytesReceived,
			BytesReceived_LastSecond,
			MessagesInSendBuffer,
			BytesInSendBuffer,
			MessagesInResendBuffer,
			BytesInResendBuffer,
			PacketLossAverage,
			PacketLossLastSecond,
			ThrottleBytes
		}
	}
}