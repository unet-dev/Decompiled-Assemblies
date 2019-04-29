using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;
using UnityEngine;

public class EffectData : IDisposable, Pool.IPooled, IProto
{
	[NonSerialized]
	public uint type;

	[NonSerialized]
	public uint pooledstringid;

	[NonSerialized]
	public int number;

	[NonSerialized]
	public Vector3 origin;

	[NonSerialized]
	public Vector3 normal;

	[NonSerialized]
	public float scale;

	[NonSerialized]
	public uint entity;

	[NonSerialized]
	public uint bone;

	[NonSerialized]
	public ulong source;

	public bool ShouldPool = true;

	private bool _disposed;

	public EffectData()
	{
	}

	public EffectData Copy()
	{
		EffectData effectDatum = Pool.Get<EffectData>();
		this.CopyTo(effectDatum);
		return effectDatum;
	}

	public void CopyTo(EffectData instance)
	{
		instance.type = this.type;
		instance.pooledstringid = this.pooledstringid;
		instance.number = this.number;
		instance.origin = this.origin;
		instance.normal = this.normal;
		instance.scale = this.scale;
		instance.entity = this.entity;
		instance.bone = this.bone;
		instance.source = this.source;
	}

	public static EffectData Deserialize(Stream stream)
	{
		EffectData effectDatum = Pool.Get<EffectData>();
		EffectData.Deserialize(stream, effectDatum, false);
		return effectDatum;
	}

	public static EffectData Deserialize(byte[] buffer)
	{
		EffectData effectDatum = Pool.Get<EffectData>();
		using (MemoryStream memoryStream = new MemoryStream(buffer))
		{
			EffectData.Deserialize(memoryStream, effectDatum, false);
		}
		return effectDatum;
	}

	public static EffectData Deserialize(byte[] buffer, EffectData instance, bool isDelta = false)
	{
		using (MemoryStream memoryStream = new MemoryStream(buffer))
		{
			EffectData.Deserialize(memoryStream, instance, isDelta);
		}
		return instance;
	}

	public static EffectData Deserialize(Stream stream, EffectData instance, bool isDelta)
	{
		while (true)
		{
			int num = stream.ReadByte();
			if (num == -1)
			{
				break;
			}
			if (num <= 34)
			{
				if (num <= 16)
				{
					if (num == 8)
					{
						instance.type = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 16)
					{
						instance.pooledstringid = ProtocolParser.ReadUInt32(stream);
						continue;
					}
				}
				else if (num == 24)
				{
					instance.number = (int)ProtocolParser.ReadUInt64(stream);
					continue;
				}
				else if (num == 34)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.origin, isDelta);
					continue;
				}
			}
			else if (num <= 53)
			{
				if (num == 42)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.normal, isDelta);
					continue;
				}
				else if (num == 53)
				{
					instance.scale = ProtocolParser.ReadSingle(stream);
					continue;
				}
			}
			else if (num == 56)
			{
				instance.entity = ProtocolParser.ReadUInt32(stream);
				continue;
			}
			else if (num == 64)
			{
				instance.bone = ProtocolParser.ReadUInt32(stream);
				continue;
			}
			else if (num == 72)
			{
				instance.source = ProtocolParser.ReadUInt64(stream);
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

	public static EffectData DeserializeLength(Stream stream, int length)
	{
		EffectData effectDatum = Pool.Get<EffectData>();
		EffectData.DeserializeLength(stream, length, effectDatum, false);
		return effectDatum;
	}

	public static EffectData DeserializeLength(Stream stream, int length, EffectData instance, bool isDelta)
	{
		long position = stream.Position + (long)length;
		while (stream.Position < position)
		{
			int num = stream.ReadByte();
			if (num == -1)
			{
				throw new EndOfStreamException();
			}
			if (num <= 34)
			{
				if (num <= 16)
				{
					if (num == 8)
					{
						instance.type = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 16)
					{
						instance.pooledstringid = ProtocolParser.ReadUInt32(stream);
						continue;
					}
				}
				else if (num == 24)
				{
					instance.number = (int)ProtocolParser.ReadUInt64(stream);
					continue;
				}
				else if (num == 34)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.origin, isDelta);
					continue;
				}
			}
			else if (num <= 53)
			{
				if (num == 42)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.normal, isDelta);
					continue;
				}
				else if (num == 53)
				{
					instance.scale = ProtocolParser.ReadSingle(stream);
					continue;
				}
			}
			else if (num == 56)
			{
				instance.entity = ProtocolParser.ReadUInt32(stream);
				continue;
			}
			else if (num == 64)
			{
				instance.bone = ProtocolParser.ReadUInt32(stream);
				continue;
			}
			else if (num == 72)
			{
				instance.source = ProtocolParser.ReadUInt64(stream);
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

	public static EffectData DeserializeLengthDelimited(Stream stream)
	{
		EffectData effectDatum = Pool.Get<EffectData>();
		EffectData.DeserializeLengthDelimited(stream, effectDatum, false);
		return effectDatum;
	}

	public static EffectData DeserializeLengthDelimited(Stream stream, EffectData instance, bool isDelta)
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
			if (num <= 34)
			{
				if (num <= 16)
				{
					if (num == 8)
					{
						instance.type = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 16)
					{
						instance.pooledstringid = ProtocolParser.ReadUInt32(stream);
						continue;
					}
				}
				else if (num == 24)
				{
					instance.number = (int)ProtocolParser.ReadUInt64(stream);
					continue;
				}
				else if (num == 34)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.origin, isDelta);
					continue;
				}
			}
			else if (num <= 53)
			{
				if (num == 42)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.normal, isDelta);
					continue;
				}
				else if (num == 53)
				{
					instance.scale = ProtocolParser.ReadSingle(stream);
					continue;
				}
			}
			else if (num == 56)
			{
				instance.entity = ProtocolParser.ReadUInt32(stream);
				continue;
			}
			else if (num == 64)
			{
				instance.bone = ProtocolParser.ReadUInt32(stream);
				continue;
			}
			else if (num == 72)
			{
				instance.source = ProtocolParser.ReadUInt64(stream);
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
		EffectData.Deserialize(stream, this, isDelta);
	}

	public virtual void LeavePool()
	{
		this._disposed = false;
	}

	public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
	{
		EffectData.DeserializeLength(stream, size, this, isDelta);
	}

	public static void ResetToPool(EffectData instance)
	{
		if (!instance.ShouldPool)
		{
			return;
		}
		instance.type = 0;
		instance.pooledstringid = 0;
		instance.number = 0;
		instance.origin = new Vector3();
		instance.normal = new Vector3();
		instance.scale = 0f;
		instance.entity = 0;
		instance.bone = 0;
		instance.source = (ulong)0;
		Pool.Free<EffectData>(ref instance);
	}

	public void ResetToPool()
	{
		EffectData.ResetToPool(this);
	}

	public static void Serialize(Stream stream, EffectData instance)
	{
		MemoryStream memoryStream = Pool.Get<MemoryStream>();
		stream.WriteByte(8);
		ProtocolParser.WriteUInt32(stream, instance.type);
		stream.WriteByte(16);
		ProtocolParser.WriteUInt32(stream, instance.pooledstringid);
		stream.WriteByte(24);
		ProtocolParser.WriteUInt64(stream, (ulong)instance.number);
		stream.WriteByte(34);
		memoryStream.SetLength((long)0);
		Vector3Serialized.Serialize(memoryStream, instance.origin);
		uint length = (uint)memoryStream.Length;
		ProtocolParser.WriteUInt32(stream, length);
		stream.Write(memoryStream.GetBuffer(), 0, (int)length);
		stream.WriteByte(42);
		memoryStream.SetLength((long)0);
		Vector3Serialized.Serialize(memoryStream, instance.normal);
		uint num = (uint)memoryStream.Length;
		ProtocolParser.WriteUInt32(stream, num);
		stream.Write(memoryStream.GetBuffer(), 0, (int)num);
		stream.WriteByte(53);
		ProtocolParser.WriteSingle(stream, instance.scale);
		stream.WriteByte(56);
		ProtocolParser.WriteUInt32(stream, instance.entity);
		stream.WriteByte(64);
		ProtocolParser.WriteUInt32(stream, instance.bone);
		stream.WriteByte(72);
		ProtocolParser.WriteUInt64(stream, instance.source);
		Pool.FreeMemoryStream(ref memoryStream);
	}

	public static void SerializeDelta(Stream stream, EffectData instance, EffectData previous)
	{
		MemoryStream memoryStream = Pool.Get<MemoryStream>();
		if (instance.type != previous.type)
		{
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.type);
		}
		if (instance.pooledstringid != previous.pooledstringid)
		{
			stream.WriteByte(16);
			ProtocolParser.WriteUInt32(stream, instance.pooledstringid);
		}
		if (instance.number != previous.number)
		{
			stream.WriteByte(24);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.number);
		}
		if (instance.origin != previous.origin)
		{
			stream.WriteByte(34);
			memoryStream.SetLength((long)0);
			Vector3Serialized.SerializeDelta(memoryStream, instance.origin, previous.origin);
			uint length = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length);
		}
		if (instance.normal != previous.normal)
		{
			stream.WriteByte(42);
			memoryStream.SetLength((long)0);
			Vector3Serialized.SerializeDelta(memoryStream, instance.normal, previous.normal);
			uint num = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, num);
			stream.Write(memoryStream.GetBuffer(), 0, (int)num);
		}
		if (instance.scale != previous.scale)
		{
			stream.WriteByte(53);
			ProtocolParser.WriteSingle(stream, instance.scale);
		}
		if (instance.entity != previous.entity)
		{
			stream.WriteByte(56);
			ProtocolParser.WriteUInt32(stream, instance.entity);
		}
		if (instance.bone != previous.bone)
		{
			stream.WriteByte(64);
			ProtocolParser.WriteUInt32(stream, instance.bone);
		}
		if (instance.source != previous.source)
		{
			stream.WriteByte(72);
			ProtocolParser.WriteUInt64(stream, instance.source);
		}
		Pool.FreeMemoryStream(ref memoryStream);
	}

	public static void SerializeLengthDelimited(Stream stream, EffectData instance)
	{
		byte[] bytes = EffectData.SerializeToBytes(instance);
		ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
		stream.Write(bytes, 0, (int)bytes.Length);
	}

	public static byte[] SerializeToBytes(EffectData instance)
	{
		byte[] array;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			EffectData.Serialize(memoryStream, instance);
			array = memoryStream.ToArray();
		}
		return array;
	}

	public void ToProto(Stream stream)
	{
		EffectData.Serialize(stream, this);
	}

	public byte[] ToProtoBytes()
	{
		return EffectData.SerializeToBytes(this);
	}

	public virtual void WriteToStream(Stream stream)
	{
		EffectData.Serialize(stream, this);
	}

	public virtual void WriteToStreamDelta(Stream stream, EffectData previous)
	{
		if (previous == null)
		{
			EffectData.Serialize(stream, this);
			return;
		}
		EffectData.SerializeDelta(stream, this, previous);
	}
}