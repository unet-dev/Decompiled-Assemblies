using Network;
using System;
using System.IO;

public class NetworkCryptographyServer : NetworkCryptography
{
	public NetworkCryptographyServer()
	{
	}

	protected override void DecryptionHandler(Connection connection, MemoryStream src, int srcOffset, MemoryStream dst, int dstOffset)
	{
		if (connection.encryptionLevel <= 1)
		{
			Craptography.XOR(2164, src, srcOffset, dst, dstOffset);
			return;
		}
		EACServer.Decrypt(connection, src, srcOffset, dst, dstOffset);
	}

	protected override void EncryptionHandler(Connection connection, MemoryStream src, int srcOffset, MemoryStream dst, int dstOffset)
	{
		if (connection.encryptionLevel <= 1)
		{
			Craptography.XOR(2164, src, srcOffset, dst, dstOffset);
			return;
		}
		EACServer.Encrypt(connection, src, srcOffset, dst, dstOffset);
	}
}