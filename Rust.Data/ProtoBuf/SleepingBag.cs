using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class SleepingBag : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public string name;

		[NonSerialized]
		public ulong deployerID;

		public bool ShouldPool = true;

		private bool _disposed;

		public SleepingBag()
		{
		}

		public SleepingBag Copy()
		{
			SleepingBag sleepingBag = Pool.Get<SleepingBag>();
			this.CopyTo(sleepingBag);
			return sleepingBag;
		}

		public void CopyTo(SleepingBag instance)
		{
			instance.name = this.name;
			instance.deployerID = this.deployerID;
		}

		public static SleepingBag Deserialize(Stream stream)
		{
			SleepingBag sleepingBag = Pool.Get<SleepingBag>();
			SleepingBag.Deserialize(stream, sleepingBag, false);
			return sleepingBag;
		}

		public static SleepingBag Deserialize(byte[] buffer)
		{
			SleepingBag sleepingBag = Pool.Get<SleepingBag>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				SleepingBag.Deserialize(memoryStream, sleepingBag, false);
			}
			return sleepingBag;
		}

		public static SleepingBag Deserialize(byte[] buffer, SleepingBag instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				SleepingBag.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static SleepingBag Deserialize(Stream stream, SleepingBag instance, bool isDelta)
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
					instance.name = ProtocolParser.ReadString(stream);
				}
				else if (num == 24)
				{
					instance.deployerID = ProtocolParser.ReadUInt64(stream);
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

		public static SleepingBag DeserializeLength(Stream stream, int length)
		{
			SleepingBag sleepingBag = Pool.Get<SleepingBag>();
			SleepingBag.DeserializeLength(stream, length, sleepingBag, false);
			return sleepingBag;
		}

		public static SleepingBag DeserializeLength(Stream stream, int length, SleepingBag instance, bool isDelta)
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
					instance.name = ProtocolParser.ReadString(stream);
				}
				else if (num == 24)
				{
					instance.deployerID = ProtocolParser.ReadUInt64(stream);
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

		public static SleepingBag DeserializeLengthDelimited(Stream stream)
		{
			SleepingBag sleepingBag = Pool.Get<SleepingBag>();
			SleepingBag.DeserializeLengthDelimited(stream, sleepingBag, false);
			return sleepingBag;
		}

		public static SleepingBag DeserializeLengthDelimited(Stream stream, SleepingBag instance, bool isDelta)
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
					instance.name = ProtocolParser.ReadString(stream);
				}
				else if (num == 24)
				{
					instance.deployerID = ProtocolParser.ReadUInt64(stream);
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
			SleepingBag.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			SleepingBag.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(SleepingBag instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.name = string.Empty;
			instance.deployerID = (ulong)0;
			Pool.Free<SleepingBag>(ref instance);
		}

		public void ResetToPool()
		{
			SleepingBag.ResetToPool(this);
		}

		public static void Serialize(Stream stream, SleepingBag instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.name != null)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteString(stream, instance.name);
			}
			stream.WriteByte(24);
			ProtocolParser.WriteUInt64(stream, instance.deployerID);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, SleepingBag instance, SleepingBag previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.name != null && instance.name != previous.name)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteString(stream, instance.name);
			}
			if (instance.deployerID != previous.deployerID)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt64(stream, instance.deployerID);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, SleepingBag instance)
		{
			byte[] bytes = SleepingBag.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(SleepingBag instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				SleepingBag.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			SleepingBag.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return SleepingBag.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			SleepingBag.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, SleepingBag previous)
		{
			if (previous == null)
			{
				SleepingBag.Serialize(stream, this);
				return;
			}
			SleepingBag.SerializeDelta(stream, this, previous);
		}
	}
}