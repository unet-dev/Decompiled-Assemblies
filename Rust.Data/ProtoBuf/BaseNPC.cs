using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class BaseNPC : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public int flags;

		public bool ShouldPool = true;

		private bool _disposed;

		public BaseNPC()
		{
		}

		public BaseNPC Copy()
		{
			BaseNPC baseNPC = Pool.Get<BaseNPC>();
			this.CopyTo(baseNPC);
			return baseNPC;
		}

		public void CopyTo(BaseNPC instance)
		{
			instance.flags = this.flags;
		}

		public static BaseNPC Deserialize(Stream stream)
		{
			BaseNPC baseNPC = Pool.Get<BaseNPC>();
			BaseNPC.Deserialize(stream, baseNPC, false);
			return baseNPC;
		}

		public static BaseNPC Deserialize(byte[] buffer)
		{
			BaseNPC baseNPC = Pool.Get<BaseNPC>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				BaseNPC.Deserialize(memoryStream, baseNPC, false);
			}
			return baseNPC;
		}

		public static BaseNPC Deserialize(byte[] buffer, BaseNPC instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				BaseNPC.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static BaseNPC Deserialize(Stream stream, BaseNPC instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num != 16)
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
					instance.flags = (int)ProtocolParser.ReadUInt64(stream);
				}
			}
			return instance;
		}

		public static BaseNPC DeserializeLength(Stream stream, int length)
		{
			BaseNPC baseNPC = Pool.Get<BaseNPC>();
			BaseNPC.DeserializeLength(stream, length, baseNPC, false);
			return baseNPC;
		}

		public static BaseNPC DeserializeLength(Stream stream, int length, BaseNPC instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num != 16)
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
					instance.flags = (int)ProtocolParser.ReadUInt64(stream);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static BaseNPC DeserializeLengthDelimited(Stream stream)
		{
			BaseNPC baseNPC = Pool.Get<BaseNPC>();
			BaseNPC.DeserializeLengthDelimited(stream, baseNPC, false);
			return baseNPC;
		}

		public static BaseNPC DeserializeLengthDelimited(Stream stream, BaseNPC instance, bool isDelta)
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
				if (num != 16)
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
					instance.flags = (int)ProtocolParser.ReadUInt64(stream);
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
			BaseNPC.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			BaseNPC.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(BaseNPC instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.flags = 0;
			Pool.Free<BaseNPC>(ref instance);
		}

		public void ResetToPool()
		{
			BaseNPC.ResetToPool(this);
		}

		public static void Serialize(Stream stream, BaseNPC instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(16);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.flags);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, BaseNPC instance, BaseNPC previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.flags != previous.flags)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.flags);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, BaseNPC instance)
		{
			byte[] bytes = BaseNPC.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(BaseNPC instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BaseNPC.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			BaseNPC.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return BaseNPC.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			BaseNPC.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, BaseNPC previous)
		{
			if (previous == null)
			{
				BaseNPC.Serialize(stream, this);
				return;
			}
			BaseNPC.SerializeDelta(stream, this, previous);
		}
	}
}