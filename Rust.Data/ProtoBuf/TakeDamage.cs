using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;
using UnityEngine;

namespace ProtoBuf
{
	public class TakeDamage : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public float amount;

		[NonSerialized]
		public Vector3 direction;

		[NonSerialized]
		public int type;

		public bool ShouldPool = true;

		private bool _disposed;

		public TakeDamage()
		{
		}

		public TakeDamage Copy()
		{
			TakeDamage takeDamage = Pool.Get<TakeDamage>();
			this.CopyTo(takeDamage);
			return takeDamage;
		}

		public void CopyTo(TakeDamage instance)
		{
			instance.amount = this.amount;
			instance.direction = this.direction;
			instance.type = this.type;
		}

		public static TakeDamage Deserialize(Stream stream)
		{
			TakeDamage takeDamage = Pool.Get<TakeDamage>();
			TakeDamage.Deserialize(stream, takeDamage, false);
			return takeDamage;
		}

		public static TakeDamage Deserialize(byte[] buffer)
		{
			TakeDamage takeDamage = Pool.Get<TakeDamage>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				TakeDamage.Deserialize(memoryStream, takeDamage, false);
			}
			return takeDamage;
		}

		public static TakeDamage Deserialize(byte[] buffer, TakeDamage instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				TakeDamage.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static TakeDamage Deserialize(Stream stream, TakeDamage instance, bool isDelta)
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
					instance.amount = ProtocolParser.ReadSingle(stream);
				}
				else if (num == 18)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.direction, isDelta);
				}
				else if (num == 24)
				{
					instance.type = (int)ProtocolParser.ReadUInt64(stream);
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

		public static TakeDamage DeserializeLength(Stream stream, int length)
		{
			TakeDamage takeDamage = Pool.Get<TakeDamage>();
			TakeDamage.DeserializeLength(stream, length, takeDamage, false);
			return takeDamage;
		}

		public static TakeDamage DeserializeLength(Stream stream, int length, TakeDamage instance, bool isDelta)
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
					instance.amount = ProtocolParser.ReadSingle(stream);
				}
				else if (num == 18)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.direction, isDelta);
				}
				else if (num == 24)
				{
					instance.type = (int)ProtocolParser.ReadUInt64(stream);
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

		public static TakeDamage DeserializeLengthDelimited(Stream stream)
		{
			TakeDamage takeDamage = Pool.Get<TakeDamage>();
			TakeDamage.DeserializeLengthDelimited(stream, takeDamage, false);
			return takeDamage;
		}

		public static TakeDamage DeserializeLengthDelimited(Stream stream, TakeDamage instance, bool isDelta)
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
					instance.amount = ProtocolParser.ReadSingle(stream);
				}
				else if (num == 18)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.direction, isDelta);
				}
				else if (num == 24)
				{
					instance.type = (int)ProtocolParser.ReadUInt64(stream);
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
			TakeDamage.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			TakeDamage.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(TakeDamage instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.amount = 0f;
			instance.direction = new Vector3();
			instance.type = 0;
			Pool.Free<TakeDamage>(ref instance);
		}

		public void ResetToPool()
		{
			TakeDamage.ResetToPool(this);
		}

		public static void Serialize(Stream stream, TakeDamage instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(13);
			ProtocolParser.WriteSingle(stream, instance.amount);
			stream.WriteByte(18);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.direction);
			uint length = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			stream.WriteByte(24);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.type);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, TakeDamage instance, TakeDamage previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.amount != previous.amount)
			{
				stream.WriteByte(13);
				ProtocolParser.WriteSingle(stream, instance.amount);
			}
			if (instance.direction != previous.direction)
			{
				stream.WriteByte(18);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.direction, previous.direction);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			if (instance.type != previous.type)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.type);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, TakeDamage instance)
		{
			byte[] bytes = TakeDamage.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(TakeDamage instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				TakeDamage.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			TakeDamage.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return TakeDamage.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			TakeDamage.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, TakeDamage previous)
		{
			if (previous == null)
			{
				TakeDamage.Serialize(stream, this);
				return;
			}
			TakeDamage.SerializeDelta(stream, this, previous);
		}
	}
}