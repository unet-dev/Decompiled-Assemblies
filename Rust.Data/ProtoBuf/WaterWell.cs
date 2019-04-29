using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class WaterWell : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public float pressure;

		[NonSerialized]
		public float waterLevel;

		public bool ShouldPool = true;

		private bool _disposed;

		public WaterWell()
		{
		}

		public WaterWell Copy()
		{
			WaterWell waterWell = Pool.Get<WaterWell>();
			this.CopyTo(waterWell);
			return waterWell;
		}

		public void CopyTo(WaterWell instance)
		{
			instance.pressure = this.pressure;
			instance.waterLevel = this.waterLevel;
		}

		public static WaterWell Deserialize(Stream stream)
		{
			WaterWell waterWell = Pool.Get<WaterWell>();
			WaterWell.Deserialize(stream, waterWell, false);
			return waterWell;
		}

		public static WaterWell Deserialize(byte[] buffer)
		{
			WaterWell waterWell = Pool.Get<WaterWell>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				WaterWell.Deserialize(memoryStream, waterWell, false);
			}
			return waterWell;
		}

		public static WaterWell Deserialize(byte[] buffer, WaterWell instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				WaterWell.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static WaterWell Deserialize(Stream stream, WaterWell instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num == 13)
				{
					instance.pressure = ProtocolParser.ReadSingle(stream);
				}
				else if (num == 21)
				{
					instance.waterLevel = ProtocolParser.ReadSingle(stream);
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

		public static WaterWell DeserializeLength(Stream stream, int length)
		{
			WaterWell waterWell = Pool.Get<WaterWell>();
			WaterWell.DeserializeLength(stream, length, waterWell, false);
			return waterWell;
		}

		public static WaterWell DeserializeLength(Stream stream, int length, WaterWell instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num == 13)
				{
					instance.pressure = ProtocolParser.ReadSingle(stream);
				}
				else if (num == 21)
				{
					instance.waterLevel = ProtocolParser.ReadSingle(stream);
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

		public static WaterWell DeserializeLengthDelimited(Stream stream)
		{
			WaterWell waterWell = Pool.Get<WaterWell>();
			WaterWell.DeserializeLengthDelimited(stream, waterWell, false);
			return waterWell;
		}

		public static WaterWell DeserializeLengthDelimited(Stream stream, WaterWell instance, bool isDelta)
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
				if (num == 13)
				{
					instance.pressure = ProtocolParser.ReadSingle(stream);
				}
				else if (num == 21)
				{
					instance.waterLevel = ProtocolParser.ReadSingle(stream);
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
			WaterWell.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			WaterWell.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(WaterWell instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.pressure = 0f;
			instance.waterLevel = 0f;
			Pool.Free<WaterWell>(ref instance);
		}

		public void ResetToPool()
		{
			WaterWell.ResetToPool(this);
		}

		public static void Serialize(Stream stream, WaterWell instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(13);
			ProtocolParser.WriteSingle(stream, instance.pressure);
			stream.WriteByte(21);
			ProtocolParser.WriteSingle(stream, instance.waterLevel);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, WaterWell instance, WaterWell previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.pressure != previous.pressure)
			{
				stream.WriteByte(13);
				ProtocolParser.WriteSingle(stream, instance.pressure);
			}
			if (instance.waterLevel != previous.waterLevel)
			{
				stream.WriteByte(21);
				ProtocolParser.WriteSingle(stream, instance.waterLevel);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, WaterWell instance)
		{
			byte[] bytes = WaterWell.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(WaterWell instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				WaterWell.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			WaterWell.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return WaterWell.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			WaterWell.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, WaterWell previous)
		{
			if (previous == null)
			{
				WaterWell.Serialize(stream, this);
				return;
			}
			WaterWell.SerializeDelta(stream, this, previous);
		}
	}
}