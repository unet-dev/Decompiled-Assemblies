using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;
using UnityEngine;

namespace ProtoBuf
{
	public class BaseEntity : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public Vector3 pos;

		[NonSerialized]
		public Vector3 rot;

		[NonSerialized]
		public int flags;

		[NonSerialized]
		public float time;

		[NonSerialized]
		public ulong skinid;

		public bool ShouldPool = true;

		private bool _disposed;

		public BaseEntity()
		{
		}

		public BaseEntity Copy()
		{
			BaseEntity baseEntity = Pool.Get<BaseEntity>();
			this.CopyTo(baseEntity);
			return baseEntity;
		}

		public void CopyTo(BaseEntity instance)
		{
			instance.pos = this.pos;
			instance.rot = this.rot;
			instance.flags = this.flags;
			instance.time = this.time;
			instance.skinid = this.skinid;
		}

		public static BaseEntity Deserialize(Stream stream)
		{
			BaseEntity baseEntity = Pool.Get<BaseEntity>();
			BaseEntity.Deserialize(stream, baseEntity, false);
			return baseEntity;
		}

		public static BaseEntity Deserialize(byte[] buffer)
		{
			BaseEntity baseEntity = Pool.Get<BaseEntity>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				BaseEntity.Deserialize(memoryStream, baseEntity, false);
			}
			return baseEntity;
		}

		public static BaseEntity Deserialize(byte[] buffer, BaseEntity instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				BaseEntity.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static BaseEntity Deserialize(Stream stream, BaseEntity instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num <= 18)
				{
					if (num == 10)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.pos, isDelta);
						continue;
					}
					else if (num == 18)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.rot, isDelta);
						continue;
					}
				}
				else if (num == 24)
				{
					instance.flags = (int)ProtocolParser.ReadUInt64(stream);
					continue;
				}
				else if (num == 37)
				{
					instance.time = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 40)
				{
					instance.skinid = ProtocolParser.ReadUInt64(stream);
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

		public static BaseEntity DeserializeLength(Stream stream, int length)
		{
			BaseEntity baseEntity = Pool.Get<BaseEntity>();
			BaseEntity.DeserializeLength(stream, length, baseEntity, false);
			return baseEntity;
		}

		public static BaseEntity DeserializeLength(Stream stream, int length, BaseEntity instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num <= 18)
				{
					if (num == 10)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.pos, isDelta);
						continue;
					}
					else if (num == 18)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.rot, isDelta);
						continue;
					}
				}
				else if (num == 24)
				{
					instance.flags = (int)ProtocolParser.ReadUInt64(stream);
					continue;
				}
				else if (num == 37)
				{
					instance.time = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 40)
				{
					instance.skinid = ProtocolParser.ReadUInt64(stream);
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

		public static BaseEntity DeserializeLengthDelimited(Stream stream)
		{
			BaseEntity baseEntity = Pool.Get<BaseEntity>();
			BaseEntity.DeserializeLengthDelimited(stream, baseEntity, false);
			return baseEntity;
		}

		public static BaseEntity DeserializeLengthDelimited(Stream stream, BaseEntity instance, bool isDelta)
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
				if (num <= 18)
				{
					if (num == 10)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.pos, isDelta);
						continue;
					}
					else if (num == 18)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.rot, isDelta);
						continue;
					}
				}
				else if (num == 24)
				{
					instance.flags = (int)ProtocolParser.ReadUInt64(stream);
					continue;
				}
				else if (num == 37)
				{
					instance.time = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 40)
				{
					instance.skinid = ProtocolParser.ReadUInt64(stream);
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
			BaseEntity.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			BaseEntity.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(BaseEntity instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.pos = new Vector3();
			instance.rot = new Vector3();
			instance.flags = 0;
			instance.time = 0f;
			instance.skinid = (ulong)0;
			Pool.Free<BaseEntity>(ref instance);
		}

		public void ResetToPool()
		{
			BaseEntity.ResetToPool(this);
		}

		public static void Serialize(Stream stream, BaseEntity instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(10);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.pos);
			uint length = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			stream.WriteByte(18);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.rot);
			uint num = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, num);
			stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			stream.WriteByte(24);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.flags);
			stream.WriteByte(37);
			ProtocolParser.WriteSingle(stream, instance.time);
			stream.WriteByte(40);
			ProtocolParser.WriteUInt64(stream, instance.skinid);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, BaseEntity instance, BaseEntity previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.pos != previous.pos)
			{
				stream.WriteByte(10);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.pos, previous.pos);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			if (instance.rot != previous.rot)
			{
				stream.WriteByte(18);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.rot, previous.rot);
				uint num = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			}
			if (instance.flags != previous.flags)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.flags);
			}
			if (instance.time != previous.time)
			{
				stream.WriteByte(37);
				ProtocolParser.WriteSingle(stream, instance.time);
			}
			if (instance.skinid != previous.skinid)
			{
				stream.WriteByte(40);
				ProtocolParser.WriteUInt64(stream, instance.skinid);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, BaseEntity instance)
		{
			byte[] bytes = BaseEntity.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(BaseEntity instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BaseEntity.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			BaseEntity.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return BaseEntity.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			BaseEntity.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, BaseEntity previous)
		{
			if (previous == null)
			{
				BaseEntity.Serialize(stream, this);
				return;
			}
			BaseEntity.SerializeDelta(stream, this, previous);
		}
	}
}