using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class Landmine : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public ulong triggeredID;

		public bool ShouldPool = true;

		private bool _disposed;

		public Landmine()
		{
		}

		public Landmine Copy()
		{
			Landmine landmine = Pool.Get<Landmine>();
			this.CopyTo(landmine);
			return landmine;
		}

		public void CopyTo(Landmine instance)
		{
			instance.triggeredID = this.triggeredID;
		}

		public static Landmine Deserialize(Stream stream)
		{
			Landmine landmine = Pool.Get<Landmine>();
			Landmine.Deserialize(stream, landmine, false);
			return landmine;
		}

		public static Landmine Deserialize(byte[] buffer)
		{
			Landmine landmine = Pool.Get<Landmine>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				Landmine.Deserialize(memoryStream, landmine, false);
			}
			return landmine;
		}

		public static Landmine Deserialize(byte[] buffer, Landmine instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				Landmine.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static Landmine Deserialize(Stream stream, Landmine instance, bool isDelta)
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
					instance.triggeredID = ProtocolParser.ReadUInt64(stream);
				}
			}
			return instance;
		}

		public static Landmine DeserializeLength(Stream stream, int length)
		{
			Landmine landmine = Pool.Get<Landmine>();
			Landmine.DeserializeLength(stream, length, landmine, false);
			return landmine;
		}

		public static Landmine DeserializeLength(Stream stream, int length, Landmine instance, bool isDelta)
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
					instance.triggeredID = ProtocolParser.ReadUInt64(stream);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static Landmine DeserializeLengthDelimited(Stream stream)
		{
			Landmine landmine = Pool.Get<Landmine>();
			Landmine.DeserializeLengthDelimited(stream, landmine, false);
			return landmine;
		}

		public static Landmine DeserializeLengthDelimited(Stream stream, Landmine instance, bool isDelta)
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
					instance.triggeredID = ProtocolParser.ReadUInt64(stream);
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
			Landmine.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			Landmine.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(Landmine instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.triggeredID = (ulong)0;
			Pool.Free<Landmine>(ref instance);
		}

		public void ResetToPool()
		{
			Landmine.ResetToPool(this);
		}

		public static void Serialize(Stream stream, Landmine instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt64(stream, instance.triggeredID);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, Landmine instance, Landmine previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.triggeredID != previous.triggeredID)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt64(stream, instance.triggeredID);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, Landmine instance)
		{
			byte[] bytes = Landmine.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(Landmine instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Landmine.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			Landmine.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return Landmine.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			Landmine.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, Landmine previous)
		{
			if (previous == null)
			{
				Landmine.Serialize(stream, this);
				return;
			}
			Landmine.SerializeDelta(stream, this, previous);
		}
	}
}