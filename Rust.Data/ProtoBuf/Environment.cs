using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class Environment : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public long dateTime;

		[NonSerialized]
		public float clouds;

		[NonSerialized]
		public float fog;

		[NonSerialized]
		public float wind;

		[NonSerialized]
		public float rain;

		[NonSerialized]
		public float engineTime;

		public bool ShouldPool = true;

		private bool _disposed;

		public Environment()
		{
		}

		public ProtoBuf.Environment Copy()
		{
			ProtoBuf.Environment environment = Pool.Get<ProtoBuf.Environment>();
			this.CopyTo(environment);
			return environment;
		}

		public void CopyTo(ProtoBuf.Environment instance)
		{
			instance.dateTime = this.dateTime;
			instance.clouds = this.clouds;
			instance.fog = this.fog;
			instance.wind = this.wind;
			instance.rain = this.rain;
			instance.engineTime = this.engineTime;
		}

		public static ProtoBuf.Environment Deserialize(Stream stream)
		{
			ProtoBuf.Environment environment = Pool.Get<ProtoBuf.Environment>();
			ProtoBuf.Environment.Deserialize(stream, environment, false);
			return environment;
		}

		public static ProtoBuf.Environment Deserialize(byte[] buffer)
		{
			ProtoBuf.Environment environment = Pool.Get<ProtoBuf.Environment>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				ProtoBuf.Environment.Deserialize(memoryStream, environment, false);
			}
			return environment;
		}

		public static ProtoBuf.Environment Deserialize(byte[] buffer, ProtoBuf.Environment instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				ProtoBuf.Environment.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static ProtoBuf.Environment Deserialize(Stream stream, ProtoBuf.Environment instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num <= 29)
				{
					if (num == 8)
					{
						instance.dateTime = (long)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 21)
					{
						instance.clouds = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 29)
					{
						instance.fog = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (num == 37)
				{
					instance.wind = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 45)
				{
					instance.rain = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 53)
				{
					instance.engineTime = ProtocolParser.ReadSingle(stream);
					continue;
				}
				Key key = ProtocolParser.ReadKey((byte)num, stream);
				if (key.Field == 0)
				{
					throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
				}
				ProtocolParser.SkipKey(stream, key);
			}
			return instance;
		}

		public static ProtoBuf.Environment DeserializeLength(Stream stream, int length)
		{
			ProtoBuf.Environment environment = Pool.Get<ProtoBuf.Environment>();
			ProtoBuf.Environment.DeserializeLength(stream, length, environment, false);
			return environment;
		}

		public static ProtoBuf.Environment DeserializeLength(Stream stream, int length, ProtoBuf.Environment instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num <= 29)
				{
					if (num == 8)
					{
						instance.dateTime = (long)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 21)
					{
						instance.clouds = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 29)
					{
						instance.fog = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (num == 37)
				{
					instance.wind = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 45)
				{
					instance.rain = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 53)
				{
					instance.engineTime = ProtocolParser.ReadSingle(stream);
					continue;
				}
				Key key = ProtocolParser.ReadKey((byte)num, stream);
				if (key.Field == 0)
				{
					throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
				}
				ProtocolParser.SkipKey(stream, key);
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static ProtoBuf.Environment DeserializeLengthDelimited(Stream stream)
		{
			ProtoBuf.Environment environment = Pool.Get<ProtoBuf.Environment>();
			ProtoBuf.Environment.DeserializeLengthDelimited(stream, environment, false);
			return environment;
		}

		public static ProtoBuf.Environment DeserializeLengthDelimited(Stream stream, ProtoBuf.Environment instance, bool isDelta)
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
				if (num <= 29)
				{
					if (num == 8)
					{
						instance.dateTime = (long)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 21)
					{
						instance.clouds = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 29)
					{
						instance.fog = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (num == 37)
				{
					instance.wind = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 45)
				{
					instance.rain = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 53)
				{
					instance.engineTime = ProtocolParser.ReadSingle(stream);
					continue;
				}
				Key key = ProtocolParser.ReadKey((byte)num, stream);
				if (key.Field == 0)
				{
					throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
				}
				ProtocolParser.SkipKey(stream, key);
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
			ProtoBuf.Environment.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			ProtoBuf.Environment.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(ProtoBuf.Environment instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.dateTime = (long)0;
			instance.clouds = 0f;
			instance.fog = 0f;
			instance.wind = 0f;
			instance.rain = 0f;
			instance.engineTime = 0f;
			Pool.Free<ProtoBuf.Environment>(ref instance);
		}

		public void ResetToPool()
		{
			ProtoBuf.Environment.ResetToPool(this);
		}

		public static void Serialize(Stream stream, ProtoBuf.Environment instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.dateTime);
			stream.WriteByte(21);
			ProtocolParser.WriteSingle(stream, instance.clouds);
			stream.WriteByte(29);
			ProtocolParser.WriteSingle(stream, instance.fog);
			stream.WriteByte(37);
			ProtocolParser.WriteSingle(stream, instance.wind);
			stream.WriteByte(45);
			ProtocolParser.WriteSingle(stream, instance.rain);
			stream.WriteByte(53);
			ProtocolParser.WriteSingle(stream, instance.engineTime);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, ProtoBuf.Environment instance, ProtoBuf.Environment previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.dateTime);
			if (instance.clouds != previous.clouds)
			{
				stream.WriteByte(21);
				ProtocolParser.WriteSingle(stream, instance.clouds);
			}
			if (instance.fog != previous.fog)
			{
				stream.WriteByte(29);
				ProtocolParser.WriteSingle(stream, instance.fog);
			}
			if (instance.wind != previous.wind)
			{
				stream.WriteByte(37);
				ProtocolParser.WriteSingle(stream, instance.wind);
			}
			if (instance.rain != previous.rain)
			{
				stream.WriteByte(45);
				ProtocolParser.WriteSingle(stream, instance.rain);
			}
			if (instance.engineTime != previous.engineTime)
			{
				stream.WriteByte(53);
				ProtocolParser.WriteSingle(stream, instance.engineTime);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, ProtoBuf.Environment instance)
		{
			byte[] bytes = ProtoBuf.Environment.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(ProtoBuf.Environment instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				ProtoBuf.Environment.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			ProtoBuf.Environment.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return ProtoBuf.Environment.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			ProtoBuf.Environment.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, ProtoBuf.Environment previous)
		{
			if (previous == null)
			{
				ProtoBuf.Environment.Serialize(stream, this);
				return;
			}
			ProtoBuf.Environment.SerializeDelta(stream, this, previous);
		}
	}
}