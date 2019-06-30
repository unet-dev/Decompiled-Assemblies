using System;
using System.IO;

namespace Network
{
	public interface INetworkCryptocraphy
	{
		void Decrypt(Connection connection, MemoryStream stream, int offset);

		MemoryStream DecryptCopy(Connection connection, MemoryStream stream, int offset);

		void Encrypt(Connection connection, MemoryStream stream, int offset);

		MemoryStream EncryptCopy(Connection connection, MemoryStream stream, int offset);

		bool IsEnabledIncoming(Connection connection);

		bool IsEnabledOutgoing(Connection connection);
	}
}