using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class ServerGib : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public string gibName;

		public bool ShouldPool = true;

		private bool _disposed;

		public ServerGib()
		{
		}

		public ServerGib Copy()
		{
			ServerGib serverGib = Pool.Get<ServerGib>();
			this.CopyTo(serverGib);
			return serverGib;
		}

		public void CopyTo(ServerGib instance)
		{
			instance.gibName = this.gibName;
		}

		public static ServerGib Deserialize(Stream stream)
		{
			ServerGib serverGib = Pool.Get<ServerGib>();
			ServerGib.Deserialize(stream, serverGib, false);
			return serverGib;
		}

		public static ServerGib Deserialize(byte[] buffer)
		{
			ServerGib serverGib = Pool.Get<ServerGib>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				ServerGib.Deserialize(memoryStream, serverGib, false);
			}
			return serverGib;
		}

		public static ServerGib Deserialize(byte[] buffer, ServerGib instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				ServerGib.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static ServerGib Deserialize(Stream stream, ServerGib instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num != 10)
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
				else
				{
					instance.gibName = ProtocolParser.ReadString(stream);
				}
			}
			return instance;
		}

		public static ServerGib DeserializeLength(Stream stream, int length)
		{
			ServerGib serverGib = Pool.Get<ServerGib>();
			ServerGib.DeserializeLength(stream, length, serverGib, false);
			return serverGib;
		}

		public static ServerGib DeserializeLength(Stream stream, int length, ServerGib instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num != 10)
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
				else
				{
					instance.gibName = ProtocolParser.ReadString(stream);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static ServerGib DeserializeLengthDelimited(Stream stream)
		{
			ServerGib serverGib = Pool.Get<ServerGib>();
			ServerGib.DeserializeLengthDelimited(stream, serverGib, false);
			return serverGib;
		}

		public static ServerGib DeserializeLengthDelimited(Stream stream, ServerGib instance, bool isDelta)
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
				if (num != 10)
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
				else
				{
					instance.gibName = ProtocolParser.ReadString(stream);
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
			ServerGib.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			ServerGib.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(ServerGib instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.gibName = string.Empty;
			Pool.Free<ServerGib>(ref instance);
		}

		public void ResetToPool()
		{
			ServerGib.ResetToPool(this);
		}

		public static void Serialize(Stream stream, ServerGib instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.gibName != null)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteString(stream, instance.gibName);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, ServerGib instance, ServerGib previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.gibName != null && instance.gibName != previous.gibName)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteString(stream, instance.gibName);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, ServerGib instance)
		{
			byte[] bytes = ServerGib.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(ServerGib instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				ServerGib.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			ServerGib.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return ServerGib.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			ServerGib.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, ServerGib previous)
		{
			if (previous == null)
			{
				ServerGib.Serialize(stream, this);
				return;
			}
			ServerGib.SerializeDelta(stream, this, previous);
		}
	}
}