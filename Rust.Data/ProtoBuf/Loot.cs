using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class Loot : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public ItemContainer contents;

		public bool ShouldPool = true;

		private bool _disposed;

		public Loot()
		{
		}

		public Loot Copy()
		{
			Loot loot = Pool.Get<Loot>();
			this.CopyTo(loot);
			return loot;
		}

		public void CopyTo(Loot instance)
		{
			if (this.contents == null)
			{
				instance.contents = null;
				return;
			}
			if (instance.contents == null)
			{
				instance.contents = this.contents.Copy();
				return;
			}
			this.contents.CopyTo(instance.contents);
		}

		public static Loot Deserialize(Stream stream)
		{
			Loot loot = Pool.Get<Loot>();
			Loot.Deserialize(stream, loot, false);
			return loot;
		}

		public static Loot Deserialize(byte[] buffer)
		{
			Loot loot = Pool.Get<Loot>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				Loot.Deserialize(memoryStream, loot, false);
			}
			return loot;
		}

		public static Loot Deserialize(byte[] buffer, Loot instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				Loot.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static Loot Deserialize(Stream stream, Loot instance, bool isDelta)
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
				else if (instance.contents != null)
				{
					ItemContainer.DeserializeLengthDelimited(stream, instance.contents, isDelta);
				}
				else
				{
					instance.contents = ItemContainer.DeserializeLengthDelimited(stream);
				}
			}
			return instance;
		}

		public static Loot DeserializeLength(Stream stream, int length)
		{
			Loot loot = Pool.Get<Loot>();
			Loot.DeserializeLength(stream, length, loot, false);
			return loot;
		}

		public static Loot DeserializeLength(Stream stream, int length, Loot instance, bool isDelta)
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
				else if (instance.contents != null)
				{
					ItemContainer.DeserializeLengthDelimited(stream, instance.contents, isDelta);
				}
				else
				{
					instance.contents = ItemContainer.DeserializeLengthDelimited(stream);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static Loot DeserializeLengthDelimited(Stream stream)
		{
			Loot loot = Pool.Get<Loot>();
			Loot.DeserializeLengthDelimited(stream, loot, false);
			return loot;
		}

		public static Loot DeserializeLengthDelimited(Stream stream, Loot instance, bool isDelta)
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
				else if (instance.contents != null)
				{
					ItemContainer.DeserializeLengthDelimited(stream, instance.contents, isDelta);
				}
				else
				{
					instance.contents = ItemContainer.DeserializeLengthDelimited(stream);
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
			Loot.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			Loot.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(Loot instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			if (instance.contents != null)
			{
				instance.contents.ResetToPool();
				instance.contents = null;
			}
			Pool.Free<Loot>(ref instance);
		}

		public void ResetToPool()
		{
			Loot.ResetToPool(this);
		}

		public static void Serialize(Stream stream, Loot instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.contents != null)
			{
				stream.WriteByte(10);
				memoryStream.SetLength((long)0);
				ItemContainer.Serialize(memoryStream, instance.contents);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, Loot instance, Loot previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.contents != null)
			{
				stream.WriteByte(10);
				memoryStream.SetLength((long)0);
				ItemContainer.SerializeDelta(memoryStream, instance.contents, previous.contents);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, Loot instance)
		{
			byte[] bytes = Loot.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(Loot instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Loot.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			Loot.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return Loot.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			Loot.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, Loot previous)
		{
			if (previous == null)
			{
				Loot.Serialize(stream, this);
				return;
			}
			Loot.SerializeDelta(stream, this, previous);
		}
	}
}