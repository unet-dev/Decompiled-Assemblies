using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class Motorboat : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public uint storageid;

		[NonSerialized]
		public uint fuelStorageID;

		public bool ShouldPool = true;

		private bool _disposed;

		public Motorboat()
		{
		}

		public Motorboat Copy()
		{
			Motorboat motorboat = Pool.Get<Motorboat>();
			this.CopyTo(motorboat);
			return motorboat;
		}

		public void CopyTo(Motorboat instance)
		{
			instance.storageid = this.storageid;
			instance.fuelStorageID = this.fuelStorageID;
		}

		public static Motorboat Deserialize(Stream stream)
		{
			Motorboat motorboat = Pool.Get<Motorboat>();
			Motorboat.Deserialize(stream, motorboat, false);
			return motorboat;
		}

		public static Motorboat Deserialize(byte[] buffer)
		{
			Motorboat motorboat = Pool.Get<Motorboat>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				Motorboat.Deserialize(memoryStream, motorboat, false);
			}
			return motorboat;
		}

		public static Motorboat Deserialize(byte[] buffer, Motorboat instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				Motorboat.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static Motorboat Deserialize(Stream stream, Motorboat instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num == 8)
				{
					instance.storageid = ProtocolParser.ReadUInt32(stream);
				}
				else if (num == 16)
				{
					instance.fuelStorageID = ProtocolParser.ReadUInt32(stream);
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

		public static Motorboat DeserializeLength(Stream stream, int length)
		{
			Motorboat motorboat = Pool.Get<Motorboat>();
			Motorboat.DeserializeLength(stream, length, motorboat, false);
			return motorboat;
		}

		public static Motorboat DeserializeLength(Stream stream, int length, Motorboat instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num == 8)
				{
					instance.storageid = ProtocolParser.ReadUInt32(stream);
				}
				else if (num == 16)
				{
					instance.fuelStorageID = ProtocolParser.ReadUInt32(stream);
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

		public static Motorboat DeserializeLengthDelimited(Stream stream)
		{
			Motorboat motorboat = Pool.Get<Motorboat>();
			Motorboat.DeserializeLengthDelimited(stream, motorboat, false);
			return motorboat;
		}

		public static Motorboat DeserializeLengthDelimited(Stream stream, Motorboat instance, bool isDelta)
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
				if (num == 8)
				{
					instance.storageid = ProtocolParser.ReadUInt32(stream);
				}
				else if (num == 16)
				{
					instance.fuelStorageID = ProtocolParser.ReadUInt32(stream);
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
			Motorboat.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			Motorboat.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(Motorboat instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.storageid = 0;
			instance.fuelStorageID = 0;
			Pool.Free<Motorboat>(ref instance);
		}

		public void ResetToPool()
		{
			Motorboat.ResetToPool(this);
		}

		public static void Serialize(Stream stream, Motorboat instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.storageid);
			stream.WriteByte(16);
			ProtocolParser.WriteUInt32(stream, instance.fuelStorageID);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, Motorboat instance, Motorboat previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.storageid != previous.storageid)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt32(stream, instance.storageid);
			}
			if (instance.fuelStorageID != previous.fuelStorageID)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.fuelStorageID);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, Motorboat instance)
		{
			byte[] bytes = Motorboat.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(Motorboat instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Motorboat.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			Motorboat.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return Motorboat.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			Motorboat.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, Motorboat previous)
		{
			if (previous == null)
			{
				Motorboat.Serialize(stream, this);
				return;
			}
			Motorboat.SerializeDelta(stream, this, previous);
		}
	}
}