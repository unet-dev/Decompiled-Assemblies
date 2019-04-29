using Network;
using System;
using System.IO;

public abstract class NetworkCryptography : INetworkCryptocraphy
{
	private MemoryStream buffer = new MemoryStream();

	protected NetworkCryptography()
	{
	}

	public void Decrypt(Connection connection, MemoryStream stream, int offset)
	{
		this.DecryptionHandler(connection, stream, offset, stream, offset);
	}

	public MemoryStream DecryptCopy(Connection connection, MemoryStream stream, int offset)
	{
		this.buffer.Position = (long)0;
		this.buffer.SetLength((long)0);
		this.buffer.Write(stream.GetBuffer(), 0, offset);
		this.DecryptionHandler(connection, stream, offset, this.buffer, offset);
		return this.buffer;
	}

	protected abstract void DecryptionHandler(Connection connection, MemoryStream src, int srcOffset, MemoryStream dst, int dstOffset);

	public void Encrypt(Connection connection, MemoryStream stream, int offset)
	{
		this.EncryptionHandler(connection, stream, offset, stream, offset);
	}

	public MemoryStream EncryptCopy(Connection connection, MemoryStream stream, int offset)
	{
		this.buffer.Position = (long)0;
		this.buffer.SetLength((long)0);
		this.buffer.Write(stream.GetBuffer(), 0, offset);
		this.EncryptionHandler(connection, stream, offset, this.buffer, offset);
		return this.buffer;
	}

	protected abstract void EncryptionHandler(Connection connection, MemoryStream src, int srcOffset, MemoryStream dst, int dstOffset);

	public bool IsEnabledIncoming(Connection connection)
	{
		if (connection == null || connection.encryptionLevel <= 0)
		{
			return false;
		}
		return connection.decryptIncoming;
	}

	public bool IsEnabledOutgoing(Connection connection)
	{
		if (connection == null || connection.encryptionLevel <= 0)
		{
			return false;
		}
		return connection.encryptOutgoing;
	}
}