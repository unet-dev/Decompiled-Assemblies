using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class Corpse : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public uint parentID;

		public bool ShouldPool = true;

		private bool _disposed;

		public Corpse()
		{
		}

		public Corpse Copy()
		{
			Corpse corpse = Pool.Get<Corpse>();
			this.CopyTo(corpse);
			return corpse;
		}

		public void CopyTo(Corpse instance)
		{
			instance.parentID = this.parentID;
		}

		public static Corpse Deserialize(Stream stream)
		{
			Corpse corpse = Pool.Get<Corpse>();
			Corpse.Deserialize(stream, corpse, false);
			return corpse;
		}

		public static Corpse Deserialize(byte[] buffer)
		{
			Corpse corpse = Pool.Get<Corpse>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				Corpse.Deserialize(memoryStream, corpse, false);
			}
			return corpse;
		}

		public static Corpse Deserialize(byte[] buffer, Corpse instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				Corpse.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static Corpse Deserialize(Stream stream, Corpse instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num != 8)
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
					instance.parentID = ProtocolParser.ReadUInt32(stream);
				}
			}
			return instance;
		}

		public static Corpse DeserializeLength(Stream stream, int length)
		{
			Corpse corpse = Pool.Get<Corpse>();
			Corpse.DeserializeLength(stream, length, corpse, false);
			return corpse;
		}

		public static Corpse DeserializeLength(Stream stream, int length, Corpse instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num != 8)
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
					instance.parentID = ProtocolParser.ReadUInt32(stream);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static Corpse DeserializeLengthDelimited(Stream stream)
		{
			Corpse corpse = Pool.Get<Corpse>();
			Corpse.DeserializeLengthDelimited(stream, corpse, false);
			return corpse;
		}

		public static Corpse DeserializeLengthDelimited(Stream stream, Corpse instance, bool isDelta)
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
				if (num != 8)
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
					instance.parentID = ProtocolParser.ReadUInt32(stream);
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
			Corpse.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			Corpse.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(Corpse instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.parentID = 0;
			Pool.Free<Corpse>(ref instance);
		}

		public void ResetToPool()
		{
			Corpse.ResetToPool(this);
		}

		public static void Serialize(Stream stream, Corpse instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.parentID);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, Corpse instance, Corpse previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.parentID != previous.parentID)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt32(stream, instance.parentID);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, Corpse instance)
		{
			byte[] bytes = Corpse.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(Corpse instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Corpse.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			Corpse.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return Corpse.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			Corpse.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, Corpse previous)
		{
			if (previous == null)
			{
				Corpse.Serialize(stream, this);
				return;
			}
			Corpse.SerializeDelta(stream, this, previous);
		}
	}
}