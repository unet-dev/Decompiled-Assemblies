using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;
using UnityEngine;

public class ModelState : IDisposable, Pool.IPooled, IProto
{
	[NonSerialized]
	public float waterLevel;

	[NonSerialized]
	public Vector3 lookDir;

	[NonSerialized]
	public int flags;

	[NonSerialized]
	public int poseType;

	public bool ShouldPool = true;

	private bool _disposed;

	public bool aiming
	{
		get
		{
			return this.HasFlag(ModelState.Flag.Aiming);
		}
		set
		{
			this.SetFlag(ModelState.Flag.Aiming, value);
		}
	}

	public bool ducked
	{
		get
		{
			return this.HasFlag(ModelState.Flag.Ducked);
		}
		set
		{
			this.SetFlag(ModelState.Flag.Ducked, value);
		}
	}

	public bool flying
	{
		get
		{
			return this.HasFlag(ModelState.Flag.Flying);
		}
		set
		{
			this.SetFlag(ModelState.Flag.Flying, value);
		}
	}

	public bool jumped
	{
		get
		{
			return this.HasFlag(ModelState.Flag.Jumped);
		}
		set
		{
			this.SetFlag(ModelState.Flag.Jumped, value);
		}
	}

	public bool mounted
	{
		get
		{
			return this.HasFlag(ModelState.Flag.Mounted);
		}
		set
		{
			this.SetFlag(ModelState.Flag.Mounted, value);
		}
	}

	public bool onground
	{
		get
		{
			return this.HasFlag(ModelState.Flag.OnGround);
		}
		set
		{
			this.SetFlag(ModelState.Flag.OnGround, value);
		}
	}

	public bool onLadder
	{
		get
		{
			return this.HasFlag(ModelState.Flag.OnLadder);
		}
		set
		{
			this.SetFlag(ModelState.Flag.OnLadder, value);
		}
	}

	public bool prone
	{
		get
		{
			return this.HasFlag(ModelState.Flag.Prone);
		}
		set
		{
			this.SetFlag(ModelState.Flag.Prone, value);
		}
	}

	public bool relaxed
	{
		get
		{
			return this.HasFlag(ModelState.Flag.Relaxed);
		}
		set
		{
			this.SetFlag(ModelState.Flag.Relaxed, value);
		}
	}

	public bool sleeping
	{
		get
		{
			return this.HasFlag(ModelState.Flag.Sleeping);
		}
		set
		{
			this.SetFlag(ModelState.Flag.Sleeping, value);
		}
	}

	public bool sprinting
	{
		get
		{
			return this.HasFlag(ModelState.Flag.Sprinting);
		}
		set
		{
			this.SetFlag(ModelState.Flag.Sprinting, value);
		}
	}

	public ModelState()
	{
	}

	public ModelState Copy()
	{
		ModelState modelState = Pool.Get<ModelState>();
		this.CopyTo(modelState);
		return modelState;
	}

	public void CopyTo(ModelState instance)
	{
		instance.waterLevel = this.waterLevel;
		instance.lookDir = this.lookDir;
		instance.flags = this.flags;
		instance.poseType = this.poseType;
	}

	public static ModelState Deserialize(Stream stream)
	{
		ModelState modelState = Pool.Get<ModelState>();
		ModelState.Deserialize(stream, modelState, false);
		return modelState;
	}

	public static ModelState Deserialize(byte[] buffer)
	{
		ModelState modelState = Pool.Get<ModelState>();
		using (MemoryStream memoryStream = new MemoryStream(buffer))
		{
			ModelState.Deserialize(memoryStream, modelState, false);
		}
		return modelState;
	}

	public static ModelState Deserialize(byte[] buffer, ModelState instance, bool isDelta = false)
	{
		using (MemoryStream memoryStream = new MemoryStream(buffer))
		{
			ModelState.Deserialize(memoryStream, instance, isDelta);
		}
		return instance;
	}

	public static ModelState Deserialize(Stream stream, ModelState instance, bool isDelta)
	{
		while (true)
		{
			int num = stream.ReadByte();
			if (num == -1)
			{
				break;
			}
			if (num <= 82)
			{
				if (num == 37)
				{
					instance.waterLevel = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 82)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.lookDir, isDelta);
					continue;
				}
			}
			else if (num == 88)
			{
				instance.flags = (int)ProtocolParser.ReadUInt64(stream);
				continue;
			}
			else if (num == 96)
			{
				instance.poseType = (int)ProtocolParser.ReadUInt64(stream);
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

	public static ModelState DeserializeLength(Stream stream, int length)
	{
		ModelState modelState = Pool.Get<ModelState>();
		ModelState.DeserializeLength(stream, length, modelState, false);
		return modelState;
	}

	public static ModelState DeserializeLength(Stream stream, int length, ModelState instance, bool isDelta)
	{
		long position = stream.Position + (long)length;
		while (stream.Position < position)
		{
			int num = stream.ReadByte();
			if (num == -1)
			{
				throw new EndOfStreamException();
			}
			if (num <= 82)
			{
				if (num == 37)
				{
					instance.waterLevel = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 82)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.lookDir, isDelta);
					continue;
				}
			}
			else if (num == 88)
			{
				instance.flags = (int)ProtocolParser.ReadUInt64(stream);
				continue;
			}
			else if (num == 96)
			{
				instance.poseType = (int)ProtocolParser.ReadUInt64(stream);
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

	public static ModelState DeserializeLengthDelimited(Stream stream)
	{
		ModelState modelState = Pool.Get<ModelState>();
		ModelState.DeserializeLengthDelimited(stream, modelState, false);
		return modelState;
	}

	public static ModelState DeserializeLengthDelimited(Stream stream, ModelState instance, bool isDelta)
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
			if (num <= 82)
			{
				if (num == 37)
				{
					instance.waterLevel = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 82)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.lookDir, isDelta);
					continue;
				}
			}
			else if (num == 88)
			{
				instance.flags = (int)ProtocolParser.ReadUInt64(stream);
				continue;
			}
			else if (num == 96)
			{
				instance.poseType = (int)ProtocolParser.ReadUInt64(stream);
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

	public static bool Equal(ModelState a, ModelState b)
	{
		if (a == b)
		{
			return true;
		}
		if (a == null || b == null)
		{
			return false;
		}
		if (a.flags != b.flags)
		{
			return false;
		}
		if (a.waterLevel != b.waterLevel)
		{
			return false;
		}
		if (a.lookDir != b.lookDir)
		{
			return false;
		}
		if (a.poseType != b.poseType)
		{
			return false;
		}
		return true;
	}

	public void FromProto(Stream stream, bool isDelta = false)
	{
		ModelState.Deserialize(stream, this, isDelta);
	}

	public bool HasFlag(ModelState.Flag f)
	{
		return (this.flags & (int)f) == (int)f;
	}

	public virtual void LeavePool()
	{
		this._disposed = false;
	}

	public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
	{
		ModelState.DeserializeLength(stream, size, this, isDelta);
	}

	public static void ResetToPool(ModelState instance)
	{
		if (!instance.ShouldPool)
		{
			return;
		}
		instance.waterLevel = 0f;
		instance.lookDir = new Vector3();
		instance.flags = 0;
		instance.poseType = 0;
		Pool.Free<ModelState>(ref instance);
	}

	public void ResetToPool()
	{
		ModelState.ResetToPool(this);
	}

	public static void Serialize(Stream stream, ModelState instance)
	{
		MemoryStream memoryStream = Pool.Get<MemoryStream>();
		stream.WriteByte(37);
		ProtocolParser.WriteSingle(stream, instance.waterLevel);
		stream.WriteByte(82);
		memoryStream.SetLength((long)0);
		Vector3Serialized.Serialize(memoryStream, instance.lookDir);
		uint length = (uint)memoryStream.Length;
		ProtocolParser.WriteUInt32(stream, length);
		stream.Write(memoryStream.GetBuffer(), 0, (int)length);
		stream.WriteByte(88);
		ProtocolParser.WriteUInt64(stream, (ulong)instance.flags);
		stream.WriteByte(96);
		ProtocolParser.WriteUInt64(stream, (ulong)instance.poseType);
		Pool.FreeMemoryStream(ref memoryStream);
	}

	public static void SerializeDelta(Stream stream, ModelState instance, ModelState previous)
	{
		MemoryStream memoryStream = Pool.Get<MemoryStream>();
		if (instance.waterLevel != previous.waterLevel)
		{
			stream.WriteByte(37);
			ProtocolParser.WriteSingle(stream, instance.waterLevel);
		}
		if (instance.lookDir != previous.lookDir)
		{
			stream.WriteByte(82);
			memoryStream.SetLength((long)0);
			Vector3Serialized.SerializeDelta(memoryStream, instance.lookDir, previous.lookDir);
			uint length = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length);
		}
		if (instance.flags != previous.flags)
		{
			stream.WriteByte(88);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.flags);
		}
		if (instance.poseType != previous.poseType)
		{
			stream.WriteByte(96);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.poseType);
		}
		Pool.FreeMemoryStream(ref memoryStream);
	}

	public static void SerializeLengthDelimited(Stream stream, ModelState instance)
	{
		byte[] bytes = ModelState.SerializeToBytes(instance);
		ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
		stream.Write(bytes, 0, (int)bytes.Length);
	}

	public static byte[] SerializeToBytes(ModelState instance)
	{
		byte[] array;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			ModelState.Serialize(memoryStream, instance);
			array = memoryStream.ToArray();
		}
		return array;
	}

	public void SetFlag(ModelState.Flag f, bool b)
	{
		if (b)
		{
			this.flags |= (int)f;
			return;
		}
		this.flags &= (int)(~f);
	}

	public void ToProto(Stream stream)
	{
		ModelState.Serialize(stream, this);
	}

	public byte[] ToProtoBytes()
	{
		return ModelState.SerializeToBytes(this);
	}

	public virtual void WriteToStream(Stream stream)
	{
		ModelState.Serialize(stream, this);
	}

	public virtual void WriteToStreamDelta(Stream stream, ModelState previous)
	{
		if (previous == null)
		{
			ModelState.Serialize(stream, this);
			return;
		}
		ModelState.SerializeDelta(stream, this, previous);
	}

	public enum Flag
	{
		Ducked = 1,
		Jumped = 2,
		OnGround = 4,
		Sleeping = 8,
		Sprinting = 16,
		OnLadder = 32,
		Flying = 64,
		Aiming = 128,
		Prone = 256,
		Mounted = 512,
		Relaxed = 1024
	}
}