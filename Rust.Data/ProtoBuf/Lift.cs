using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class Lift : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public int floor;

		public bool ShouldPool = true;

		private bool _disposed;

		public Lift()
		{
		}

		public Lift Copy()
		{
			Lift lift = Pool.Get<Lift>();
			this.CopyTo(lift);
			return lift;
		}

		public void CopyTo(Lift instance)
		{
			instance.floor = this.floor;
		}

		public static Lift Deserialize(Stream stream)
		{
			Lift lift = Pool.Get<Lift>();
			Lift.Deserialize(stream, lift, false);
			return lift;
		}

		public static Lift Deserialize(byte[] buffer)
		{
			Lift lift = Pool.Get<Lift>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				Lift.Deserialize(memoryStream, lift, false);
			}
			return lift;
		}

		public static Lift Deserialize(byte[] buffer, Lift instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				Lift.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static Lift Deserialize(Stream stream, Lift instance, bool isDelta)
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
					instance.floor = (int)ProtocolParser.ReadUInt64(stream);
				}
			}
			return instance;
		}

		public static Lift DeserializeLength(Stream stream, int length)
		{
			Lift lift = Pool.Get<Lift>();
			Lift.DeserializeLength(stream, length, lift, false);
			return lift;
		}

		public static Lift DeserializeLength(Stream stream, int length, Lift instance, bool isDelta)
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
					instance.floor = (int)ProtocolParser.ReadUInt64(stream);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static Lift DeserializeLengthDelimited(Stream stream)
		{
			Lift lift = Pool.Get<Lift>();
			Lift.DeserializeLengthDelimited(stream, lift, false);
			return lift;
		}

		public static Lift DeserializeLengthDelimited(Stream stream, Lift instance, bool isDelta)
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
					instance.floor = (int)ProtocolParser.ReadUInt64(stream);
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
			Lift.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			Lift.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(Lift instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.floor = 0;
			Pool.Free<Lift>(ref instance);
		}

		public void ResetToPool()
		{
			Lift.ResetToPool(this);
		}

		public static void Serialize(Stream stream, Lift instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.floor);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, Lift instance, Lift previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.floor != previous.floor)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.floor);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, Lift instance)
		{
			byte[] bytes = Lift.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(Lift instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Lift.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			Lift.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return Lift.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			Lift.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, Lift previous)
		{
			if (previous == null)
			{
				Lift.Serialize(stream, this);
				return;
			}
			Lift.SerializeDelta(stream, this, previous);
		}
	}
}