using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;
using UnityEngine;

namespace ProtoBuf
{
	public class HotAirBalloon : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public float inflationAmount;

		[NonSerialized]
		public Vector3 velocity;

		public bool ShouldPool = true;

		private bool _disposed;

		public HotAirBalloon()
		{
		}

		public HotAirBalloon Copy()
		{
			HotAirBalloon hotAirBalloon = Pool.Get<HotAirBalloon>();
			this.CopyTo(hotAirBalloon);
			return hotAirBalloon;
		}

		public void CopyTo(HotAirBalloon instance)
		{
			instance.inflationAmount = this.inflationAmount;
			instance.velocity = this.velocity;
		}

		public static HotAirBalloon Deserialize(Stream stream)
		{
			HotAirBalloon hotAirBalloon = Pool.Get<HotAirBalloon>();
			HotAirBalloon.Deserialize(stream, hotAirBalloon, false);
			return hotAirBalloon;
		}

		public static HotAirBalloon Deserialize(byte[] buffer)
		{
			HotAirBalloon hotAirBalloon = Pool.Get<HotAirBalloon>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				HotAirBalloon.Deserialize(memoryStream, hotAirBalloon, false);
			}
			return hotAirBalloon;
		}

		public static HotAirBalloon Deserialize(byte[] buffer, HotAirBalloon instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				HotAirBalloon.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static HotAirBalloon Deserialize(Stream stream, HotAirBalloon instance, bool isDelta)
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
					instance.inflationAmount = ProtocolParser.ReadSingle(stream);
				}
				else if (num == 18)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.velocity, isDelta);
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

		public static HotAirBalloon DeserializeLength(Stream stream, int length)
		{
			HotAirBalloon hotAirBalloon = Pool.Get<HotAirBalloon>();
			HotAirBalloon.DeserializeLength(stream, length, hotAirBalloon, false);
			return hotAirBalloon;
		}

		public static HotAirBalloon DeserializeLength(Stream stream, int length, HotAirBalloon instance, bool isDelta)
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
					instance.inflationAmount = ProtocolParser.ReadSingle(stream);
				}
				else if (num == 18)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.velocity, isDelta);
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

		public static HotAirBalloon DeserializeLengthDelimited(Stream stream)
		{
			HotAirBalloon hotAirBalloon = Pool.Get<HotAirBalloon>();
			HotAirBalloon.DeserializeLengthDelimited(stream, hotAirBalloon, false);
			return hotAirBalloon;
		}

		public static HotAirBalloon DeserializeLengthDelimited(Stream stream, HotAirBalloon instance, bool isDelta)
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
					instance.inflationAmount = ProtocolParser.ReadSingle(stream);
				}
				else if (num == 18)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.velocity, isDelta);
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
			HotAirBalloon.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			HotAirBalloon.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(HotAirBalloon instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.inflationAmount = 0f;
			instance.velocity = new Vector3();
			Pool.Free<HotAirBalloon>(ref instance);
		}

		public void ResetToPool()
		{
			HotAirBalloon.ResetToPool(this);
		}

		public static void Serialize(Stream stream, HotAirBalloon instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(13);
			ProtocolParser.WriteSingle(stream, instance.inflationAmount);
			stream.WriteByte(18);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.velocity);
			uint length = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, HotAirBalloon instance, HotAirBalloon previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.inflationAmount != previous.inflationAmount)
			{
				stream.WriteByte(13);
				ProtocolParser.WriteSingle(stream, instance.inflationAmount);
			}
			if (instance.velocity != previous.velocity)
			{
				stream.WriteByte(18);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.velocity, previous.velocity);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, HotAirBalloon instance)
		{
			byte[] bytes = HotAirBalloon.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(HotAirBalloon instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				HotAirBalloon.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			HotAirBalloon.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return HotAirBalloon.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			HotAirBalloon.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, HotAirBalloon previous)
		{
			if (previous == null)
			{
				HotAirBalloon.Serialize(stream, this);
				return;
			}
			HotAirBalloon.SerializeDelta(stream, this, previous);
		}
	}
}