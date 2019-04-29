using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class DudExplosive : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public float fuseTimeLeft;

		public bool ShouldPool = true;

		private bool _disposed;

		public DudExplosive()
		{
		}

		public DudExplosive Copy()
		{
			DudExplosive dudExplosive = Pool.Get<DudExplosive>();
			this.CopyTo(dudExplosive);
			return dudExplosive;
		}

		public void CopyTo(DudExplosive instance)
		{
			instance.fuseTimeLeft = this.fuseTimeLeft;
		}

		public static DudExplosive Deserialize(Stream stream)
		{
			DudExplosive dudExplosive = Pool.Get<DudExplosive>();
			DudExplosive.Deserialize(stream, dudExplosive, false);
			return dudExplosive;
		}

		public static DudExplosive Deserialize(byte[] buffer)
		{
			DudExplosive dudExplosive = Pool.Get<DudExplosive>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				DudExplosive.Deserialize(memoryStream, dudExplosive, false);
			}
			return dudExplosive;
		}

		public static DudExplosive Deserialize(byte[] buffer, DudExplosive instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				DudExplosive.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static DudExplosive Deserialize(Stream stream, DudExplosive instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num != 13)
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
					instance.fuseTimeLeft = ProtocolParser.ReadSingle(stream);
				}
			}
			return instance;
		}

		public static DudExplosive DeserializeLength(Stream stream, int length)
		{
			DudExplosive dudExplosive = Pool.Get<DudExplosive>();
			DudExplosive.DeserializeLength(stream, length, dudExplosive, false);
			return dudExplosive;
		}

		public static DudExplosive DeserializeLength(Stream stream, int length, DudExplosive instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num != 13)
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
					instance.fuseTimeLeft = ProtocolParser.ReadSingle(stream);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static DudExplosive DeserializeLengthDelimited(Stream stream)
		{
			DudExplosive dudExplosive = Pool.Get<DudExplosive>();
			DudExplosive.DeserializeLengthDelimited(stream, dudExplosive, false);
			return dudExplosive;
		}

		public static DudExplosive DeserializeLengthDelimited(Stream stream, DudExplosive instance, bool isDelta)
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
				if (num != 13)
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
					instance.fuseTimeLeft = ProtocolParser.ReadSingle(stream);
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
			DudExplosive.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			DudExplosive.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(DudExplosive instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.fuseTimeLeft = 0f;
			Pool.Free<DudExplosive>(ref instance);
		}

		public void ResetToPool()
		{
			DudExplosive.ResetToPool(this);
		}

		public static void Serialize(Stream stream, DudExplosive instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(13);
			ProtocolParser.WriteSingle(stream, instance.fuseTimeLeft);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, DudExplosive instance, DudExplosive previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.fuseTimeLeft != previous.fuseTimeLeft)
			{
				stream.WriteByte(13);
				ProtocolParser.WriteSingle(stream, instance.fuseTimeLeft);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, DudExplosive instance)
		{
			byte[] bytes = DudExplosive.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(DudExplosive instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				DudExplosive.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			DudExplosive.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return DudExplosive.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			DudExplosive.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, DudExplosive previous)
		{
			if (previous == null)
			{
				DudExplosive.Serialize(stream, this);
				return;
			}
			DudExplosive.SerializeDelta(stream, this, previous);
		}
	}
}