using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class Spawnable : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public uint population;

		public bool ShouldPool = true;

		private bool _disposed;

		public Spawnable()
		{
		}

		public Spawnable Copy()
		{
			Spawnable spawnable = Pool.Get<Spawnable>();
			this.CopyTo(spawnable);
			return spawnable;
		}

		public void CopyTo(Spawnable instance)
		{
			instance.population = this.population;
		}

		public static Spawnable Deserialize(Stream stream)
		{
			Spawnable spawnable = Pool.Get<Spawnable>();
			Spawnable.Deserialize(stream, spawnable, false);
			return spawnable;
		}

		public static Spawnable Deserialize(byte[] buffer)
		{
			Spawnable spawnable = Pool.Get<Spawnable>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				Spawnable.Deserialize(memoryStream, spawnable, false);
			}
			return spawnable;
		}

		public static Spawnable Deserialize(byte[] buffer, Spawnable instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				Spawnable.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static Spawnable Deserialize(Stream stream, Spawnable instance, bool isDelta)
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
					instance.population = ProtocolParser.ReadUInt32(stream);
				}
			}
			return instance;
		}

		public static Spawnable DeserializeLength(Stream stream, int length)
		{
			Spawnable spawnable = Pool.Get<Spawnable>();
			Spawnable.DeserializeLength(stream, length, spawnable, false);
			return spawnable;
		}

		public static Spawnable DeserializeLength(Stream stream, int length, Spawnable instance, bool isDelta)
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
					instance.population = ProtocolParser.ReadUInt32(stream);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static Spawnable DeserializeLengthDelimited(Stream stream)
		{
			Spawnable spawnable = Pool.Get<Spawnable>();
			Spawnable.DeserializeLengthDelimited(stream, spawnable, false);
			return spawnable;
		}

		public static Spawnable DeserializeLengthDelimited(Stream stream, Spawnable instance, bool isDelta)
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
					instance.population = ProtocolParser.ReadUInt32(stream);
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
			Spawnable.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			Spawnable.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(Spawnable instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.population = 0;
			Pool.Free<Spawnable>(ref instance);
		}

		public void ResetToPool()
		{
			Spawnable.ResetToPool(this);
		}

		public static void Serialize(Stream stream, Spawnable instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.population);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, Spawnable instance, Spawnable previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.population != previous.population)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt32(stream, instance.population);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, Spawnable instance)
		{
			byte[] bytes = Spawnable.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(Spawnable instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Spawnable.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			Spawnable.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return Spawnable.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			Spawnable.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, Spawnable previous)
		{
			if (previous == null)
			{
				Spawnable.Serialize(stream, this);
				return;
			}
			Spawnable.SerializeDelta(stream, this, previous);
		}
	}
}