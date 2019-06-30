using Facepunch;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Network
{
	public abstract class NetworkPeer
	{
		public Write write;

		public INetworkCryptocraphy cryptography;

		public NetRead read { get; } = new NetRead();

		protected NetworkPeer()
		{
		}

		public void Decrypt(Connection connection, NetRead read)
		{
			if (read.Length <= (long)1)
			{
				return;
			}
			if (this.cryptography == null)
			{
				return;
			}
			if (!this.cryptography.IsEnabledIncoming(connection))
			{
				return;
			}
			MemoryStream streamForDecryption = read.GetStreamForDecryption();
			this.cryptography.Decrypt(connection, streamForDecryption, 1);
			read.SetLength(streamForDecryption.Length);
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