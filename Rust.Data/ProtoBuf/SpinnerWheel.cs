using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;
using UnityEngine;

namespace ProtoBuf
{
	public class SpinnerWheel : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public Vector3 spin;

		public bool ShouldPool = true;

		private bool _disposed;

		public SpinnerWheel()
		{
		}

		public SpinnerWheel Copy()
		{
			SpinnerWheel spinnerWheel = Pool.Get<SpinnerWheel>();
			this.CopyTo(spinnerWheel);
			return spinnerWheel;
		}

		public void CopyTo(SpinnerWheel instance)
		{
			instance.spin = this.spin;
		}

		public static SpinnerWheel Deserialize(Stream stream)
		{
			SpinnerWheel spinnerWheel = Pool.Get<SpinnerWheel>();
			SpinnerWheel.Deserialize(stream, spinnerWheel, false);
			return spinnerWheel;
		}

		public static SpinnerWheel Deserialize(byte[] buffer)
		{
			SpinnerWheel spinnerWheel = Pool.Get<SpinnerWheel>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				SpinnerWheel.Deserialize(memoryStream, spinnerWheel, false);
			}
			return spinnerWheel;
		}

		public static SpinnerWheel Deserialize(byte[] buffer, SpinnerWheel instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				SpinnerWheel.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static SpinnerWheel Deserialize(Stream stream, SpinnerWheel instance, bool isDelta)
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
				else
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.spin, isDelta);
				}
			}
			return instance;
		}

		public static SpinnerWheel DeserializeLength(Stream stream, int length)
		{
			SpinnerWheel spinnerWheel = Pool.Get<SpinnerWheel>();
			SpinnerWheel.DeserializeLength(stream, length, spinnerWheel, false);
			return spinnerWheel;
		}

		public static SpinnerWheel DeserializeLength(Stream stream, int length, SpinnerWheel instance, bool isDelta)
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
				else
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.spin, isDelta);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static SpinnerWheel DeserializeLengthDelimited(Stream stream)
		{
			SpinnerWheel spinnerWheel = Pool.Get<SpinnerWheel>();
			SpinnerWheel.DeserializeLengthDelimited(stream, spinnerWheel, false);
			return spinnerWheel;
		}

		public static SpinnerWheel DeserializeLengthDelimited(Stream stream, SpinnerWheel instance, bool isDelta)
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
				else
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.spin, isDelta);
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
			SpinnerWheel.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			SpinnerWheel.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(SpinnerWheel instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.spin = new Vector3();
			Pool.Free<SpinnerWheel>(ref instance);
		}

		public void ResetToPool()
		{
			SpinnerWheel.ResetToPool(this);
		}

		public static void Serialize(Stream stream, SpinnerWheel instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(10);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.spin);
			uint length = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, SpinnerWheel instance, SpinnerWheel previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.spin != previous.spin)
			{
				stream.WriteByte(10);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.spin, previous.spin);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, SpinnerWheel instance)
		{
			byte[] bytes = SpinnerWheel.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(SpinnerWheel instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				SpinnerWheel.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			SpinnerWheel.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return SpinnerWheel.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			SpinnerWheel.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, SpinnerWheel previous)
		{
			if (previous == null)
			{
				SpinnerWheel.Serialize(stream, this);
				return;
			}
			SpinnerWheel.SerializeDelta(stream, this, previous);
		}
	}
}