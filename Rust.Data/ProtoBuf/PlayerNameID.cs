using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class PlayerNameID : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public string username;

		[NonSerialized]
		public ulong userid;

		public bool ShouldPool = true;

		private bool _disposed;

		public PlayerNameID()
		{
		}

		public PlayerNameID Copy()
		{
			PlayerNameID playerNameID = Pool.Get<PlayerNameID>();
			this.CopyTo(playerNameID);
			return playerNameID;
		}

		public void CopyTo(PlayerNameID instance)
		{
			instance.username = this.username;
			instance.userid = this.userid;
		}

		public static PlayerNameID Deserialize(Stream stream)
		{
			PlayerNameID playerNameID = Pool.Get<PlayerNameID>();
			PlayerNameID.Deserialize(stream, playerNameID, false);
			return playerNameID;
		}

		public static PlayerNameID Deserialize(byte[] buffer)
		{
			PlayerNameID playerNameID = Pool.Get<PlayerNameID>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				PlayerNameID.Deserialize(memoryStream, playerNameID, false);
			}
			return playerNameID;
		}

		public static PlayerNameID Deserialize(byte[] buffer, PlayerNameID instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				PlayerNameID.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static PlayerNameID Deserialize(Stream stream, PlayerNameID instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num == 10)
				{
					instance.username = ProtocolParser.ReadString(stream);
				}
				else if (num == 16)
				{
					instance.userid = ProtocolParser.ReadUInt64(stream);
				}
				else
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
			}
			return instance;
		}

		public static PlayerNameID DeserializeLength(Stream stream, int length)
		{
			PlayerNameID playerNameID = Pool.Get<PlayerNameID>();
			PlayerNameID.DeserializeLength(stream, length, playerNameID, false);
			return playerNameID;
		}

		public static PlayerNameID DeserializeLength(Stream stream, int length, PlayerNameID instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num == 10)
				{
					instance.username = ProtocolParser.ReadString(stream);
				}
				else if (num == 16)
				{
					instance.userid = ProtocolParser.ReadUInt64(stream);
				}
				else
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static PlayerNameID DeserializeLengthDelimited(Stream stream)
		{
			PlayerNameID playerNameID = Pool.Get<PlayerNameID>();
			PlayerNameID.DeserializeLengthDelimited(stream, playerNameID, false);
			return playerNameID;
		}

		public static PlayerNameID DeserializeLengthDelimited(Stream stream, PlayerNameID instance, bool isDelta)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num == 10)
				{
					instance.username = ProtocolParser.ReadString(stream);
				}
				else if (num == 16)
				{
					instance.userid = ProtocolParser.ReadUInt64(stream);
				}
				else
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public virtual void Dispose()
		{
			if (this._disposed)
			{
				return;
			}
			this.ResetToPool();
			this._disposed = true;
		}

		public virtual void EnterPool()
		{
			this._disposed = true;
		}

		public void FromProto(Stream stream, bool isDelta = false)
		{
			PlayerNameID.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			PlayerNameID.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(PlayerNameID instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.username = string.Empty;
			instance.userid = (ulong)0;
			Pool.Free<PlayerNameID>(ref instance);
		}

		public void ResetToPool()
		{
			PlayerNameID.ResetToPool(this);
		}

		public static void Serialize(Stream stream, PlayerNameID instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.username == null)
			{
				throw new ArgumentNullException("username", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteString(stream, instance.username);
			stream.WriteByte(16);
			ProtocolParser.WriteUInt64(stream, instance.userid);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, PlayerNameID instance, PlayerNameID previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.username != previous.username)
			{
				if (instance.username == null)
				{
					throw new ArgumentNullException("username", "Required by proto specification.");
				}
				stream.WriteByte(10);
				ProtocolParser.WriteString(stream, instance.username);
			}
			if (instance.userid != previous.userid)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt64(stream, instance.userid);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, PlayerNameID instance)
		{
			byte[] bytes = PlayerNameID.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(PlayerNameID instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				PlayerNameID.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			PlayerNameID.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return PlayerNameID.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			PlayerNameID.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, PlayerNameID previous)
		{
			if (previous == null)
			{
				PlayerNameID.Serialize(stream, this);
				return;
			}
			PlayerNameID.SerializeDelta(stream, this, previous);
		}
	}
}