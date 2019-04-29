using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;
using UnityEngine;

public class PlayerTick : IDisposable, Pool.IPooled, IProto
{
	[NonSerialized]
	public InputMessage inputState;

	[NonSerialized]
	public Vector3 position;

	[NonSerialized]
	public ModelState modelState;

	[NonSerialized]
	public uint activeItem;

	[NonSerialized]
	public Vector3 eyePos;

	[NonSerialized]
	public uint parentID;

	public bool ShouldPool = true;

	private bool _disposed;

	public PlayerTick()
	{
	}

	public PlayerTick Copy()
	{
		PlayerTick playerTick = Pool.Get<PlayerTick>();
		this.CopyTo(playerTick);
		return playerTick;
	}

	public void CopyTo(PlayerTick instance)
	{
		if (this.inputState == null)
		{
			instance.inputState = null;
		}
		else if (instance.inputState != null)
		{
			this.inputState.CopyTo(instance.inputState);
		}
		else
		{
			instance.inputState = this.inputState.Copy();
		}
		instance.position = this.position;
		if (this.modelState == null)
		{
			instance.modelState = null;
		}
		else if (instance.modelState != null)
		{
			this.modelState.CopyTo(instance.modelState);
		}
		else
		{
			instance.modelState = this.modelState.Copy();
		}
		instance.activeItem = this.activeItem;
		instance.eyePos = this.eyePos;
		instance.parentID = this.parentID;
	}

	public static PlayerTick Deserialize(Stream stream)
	{
		PlayerTick playerTick = Pool.Get<PlayerTick>();
		PlayerTick.Deserialize(stream, playerTick, false);
		return playerTick;
	}

	public static PlayerTick Deserialize(byte[] buffer)
	{
		PlayerTick playerTick = Pool.Get<PlayerTick>();
		using (MemoryStream memoryStream = new MemoryStream(buffer))
		{
			PlayerTick.Deserialize(memoryStream, playerTick, false);
		}
		return playerTick;
	}

	public static PlayerTick Deserialize(byte[] buffer, PlayerTick instance, bool isDelta = false)
	{
		using (MemoryStream memoryStream = new MemoryStream(buffer))
		{
			PlayerTick.Deserialize(memoryStream, instance, isDelta);
		}
		return instance;
	}

	public static PlayerTick Deserialize(Stream stream, PlayerTick instance, bool isDelta)
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
				if (num == 10)
				{
					if (instance.inputState != null)
					{
						InputMessage.DeserializeLengthDelimited(stream, instance.inputState, isDelta);
						continue;
					}
					else
					{
						instance.inputState = InputMessage.DeserializeLengthDelimited(stream);
						continue;
					}
				}
				else if (num == 18)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.position, isDelta);
					continue;
				}
				else if (num == 34)
				{
					if (instance.modelState != null)
					{
						ModelState.DeserializeLengthDelimited(stream, instance.modelState, isDelta);
						continue;
					}
					else
					{
						instance.modelState = ModelState.DeserializeLengthDelimited(stream);
						continue;
					}
				}
			}
			else if (num == 40)
			{
				instance.activeItem = ProtocolParser.ReadUInt32(stream);
				continue;
			}
			else if (num == 50)
			{
				Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.eyePos, isDelta);
				continue;
			}
			else if (num == 56)
			{
				instance.parentID = ProtocolParser.ReadUInt32(stream);
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

	public static PlayerTick DeserializeLength(Stream stream, int length)
	{
		PlayerTick playerTick = Pool.Get<PlayerTick>();
		PlayerTick.DeserializeLength(stream, length, playerTick, false);
		return playerTick;
	}

	public static PlayerTick DeserializeLength(Stream stream, int length, PlayerTick instance, bool isDelta)
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
				if (num == 10)
				{
					if (instance.inputState != null)
					{
						InputMessage.DeserializeLengthDelimited(stream, instance.inputState, isDelta);
						continue;
					}
					else
					{
						instance.inputState = InputMessage.DeserializeLengthDelimited(stream);
						continue;
					}
				}
				else if (num == 18)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.position, isDelta);
					continue;
				}
				else if (num == 34)
				{
					if (instance.modelState != null)
					{
						ModelState.DeserializeLengthDelimited(stream, instance.modelState, isDelta);
						continue;
					}
					else
					{
						instance.modelState = ModelState.DeserializeLengthDelimited(stream);
						continue;
					}
				}
			}
			else if (num == 40)
			{
				instance.activeItem = ProtocolParser.ReadUInt32(stream);
				continue;
			}
			else if (num == 50)
			{
				Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.eyePos, isDelta);
				continue;
			}
			else if (num == 56)
			{
				instance.parentID = ProtocolParser.ReadUInt32(stream);
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

	public static PlayerTick DeserializeLengthDelimited(Stream stream)
	{
		PlayerTick playerTick = Pool.Get<PlayerTick>();
		PlayerTick.DeserializeLengthDelimited(stream, playerTick, false);
		return playerTick;
	}

	public static PlayerTick DeserializeLengthDelimited(Stream stream, PlayerTick instance, bool isDelta)
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
				if (num == 10)
				{
					if (instance.inputState != null)
					{
						InputMessage.DeserializeLengthDelimited(stream, instance.inputState, isDelta);
						continue;
					}
					else
					{
						instance.inputState = InputMessage.DeserializeLengthDelimited(stream);
						continue;
					}
				}
				else if (num == 18)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.position, isDelta);
					continue;
				}
				else if (num == 34)
				{
					if (instance.modelState != null)
					{
						ModelState.DeserializeLengthDelimited(stream, instance.modelState, isDelta);
						continue;
					}
					else
					{
						instance.modelState = ModelState.DeserializeLengthDelimited(stream);
						continue;
					}
				}
			}
			else if (num == 40)
			{
				instance.activeItem = ProtocolParser.ReadUInt32(stream);
				continue;
			}
			else if (num == 50)
			{
				Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.eyePos, isDelta);
				continue;
			}
			else if (num == 56)
			{
				instance.parentID = ProtocolParser.ReadUInt32(stream);
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
		PlayerTick.Deserialize(stream, this, isDelta);
	}

	public virtual void LeavePool()
	{
		this._disposed = false;
	}

	public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
	{
		PlayerTick.DeserializeLength(stream, size, this, isDelta);
	}

	public static void ResetToPool(PlayerTick instance)
	{
		if (!instance.ShouldPool)
		{
			return;
		}
		if (instance.inputState != null)
		{
			instance.inputState.ResetToPool();
			instance.inputState = null;
		}
		instance.position = new Vector3();
		if (instance.modelState != null)
		{
			instance.modelState.ResetToPool();
			instance.modelState = null;
		}
		instance.activeItem = 0;
		instance.eyePos = new Vector3();
		instance.parentID = 0;
		Pool.Free<PlayerTick>(ref instance);
	}

	public void ResetToPool()
	{
		PlayerTick.ResetToPool(this);
	}

	public static void Serialize(Stream stream, PlayerTick instance)
	{
		MemoryStream memoryStream = Pool.Get<MemoryStream>();
		if (instance.inputState == null)
		{
			throw new ArgumentNullException("inputState", "Required by proto specification.");
		}
		stream.WriteByte(10);
		memoryStream.SetLength((long)0);
		InputMessage.Serialize(memoryStream, instance.inputState);
		uint length = (uint)memoryStream.Length;
		ProtocolParser.WriteUInt32(stream, length);
		stream.Write(memoryStream.GetBuffer(), 0, (int)length);
		stream.WriteByte(18);
		memoryStream.SetLength((long)0);
		Vector3Serialized.Serialize(memoryStream, instance.position);
		uint num = (uint)memoryStream.Length;
		ProtocolParser.WriteUInt32(stream, num);
		stream.Write(memoryStream.GetBuffer(), 0, (int)num);
		if (instance.modelState == null)
		{
			throw new ArgumentNullException("modelState", "Required by proto specification.");
		}
		stream.WriteByte(34);
		memoryStream.SetLength((long)0);
		ModelState.Serialize(memoryStream, instance.modelState);
		uint length1 = (uint)memoryStream.Length;
		ProtocolParser.WriteUInt32(stream, length1);
		stream.Write(memoryStream.GetBuffer(), 0, (int)length1);
		stream.WriteByte(40);
		ProtocolParser.WriteUInt32(stream, instance.activeItem);
		stream.WriteByte(50);
		memoryStream.SetLength((long)0);
		Vector3Serialized.Serialize(memoryStream, instance.eyePos);
		uint num1 = (uint)memoryStream.Length;
		ProtocolParser.WriteUInt32(stream, num1);
		stream.Write(memoryStream.GetBuffer(), 0, (int)num1);
		stream.WriteByte(56);
		ProtocolParser.WriteUInt32(stream, instance.parentID);
		Pool.FreeMemoryStream(ref memoryStream);
	}

	public static void SerializeDelta(Stream stream, PlayerTick instance, PlayerTick previous)
	{
		MemoryStream memoryStream = Pool.Get<MemoryStream>();
		if (instance.inputState == null)
		{
			throw new ArgumentNullException("inputState", "Required by proto specification.");
		}
		stream.WriteByte(10);
		memoryStream.SetLength((long)0);
		InputMessage.SerializeDelta(memoryStream, instance.inputState, previous.inputState);
		uint length = (uint)memoryStream.Length;
		ProtocolParser.WriteUInt32(stream, length);
		stream.Write(memoryStream.GetBuffer(), 0, (int)length);
		if (instance.position != previous.position)
		{
			stream.WriteByte(18);
			memoryStream.SetLength((long)0);
			Vector3Serialized.SerializeDelta(memoryStream, instance.position, previous.position);
			uint num = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, num);
			stream.Write(memoryStream.GetBuffer(), 0, (int)num);
		}
		if (instance.modelState == null)
		{
			throw new ArgumentNullException("modelState", "Required by proto specification.");
		}
		stream.WriteByte(34);
		memoryStream.SetLength((long)0);
		ModelState.SerializeDelta(memoryStream, instance.modelState, previous.modelState);
		uint length1 = (uint)memoryStream.Length;
		ProtocolParser.WriteUInt32(stream, length1);
		stream.Write(memoryStream.GetBuffer(), 0, (int)length1);
		if (instance.activeItem != previous.activeItem)
		{
			stream.WriteByte(40);
			ProtocolParser.WriteUInt32(stream, instance.activeItem);
		}
		if (instance.eyePos != previous.eyePos)
		{
			stream.WriteByte(50);
			memoryStream.SetLength((long)0);
			Vector3Serialized.SerializeDelta(memoryStream, instance.eyePos, previous.eyePos);
			uint num1 = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, num1);
			stream.Write(memoryStream.GetBuffer(), 0, (int)num1);
		}
		if (instance.parentID != previous.parentID)
		{
			stream.WriteByte(56);
			ProtocolParser.WriteUInt32(stream, instance.parentID);
		}
		Pool.FreeMemoryStream(ref memoryStream);
	}

	public static void SerializeLengthDelimited(Stream stream, PlayerTick instance)
	{
		byte[] bytes = PlayerTick.SerializeToBytes(instance);
		ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
		stream.Write(bytes, 0, (int)bytes.Length);
	}

	public static byte[] SerializeToBytes(PlayerTick instance)
	{
		byte[] array;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			PlayerTick.Serialize(memoryStream, instance);
			array = memoryStream.ToArray();
		}
		return array;
	}

	public void ToProto(Stream stream)
	{
		PlayerTick.Serialize(stream, this);
	}

	public byte[] ToProtoBytes()
	{
		return PlayerTick.SerializeToBytes(this);
	}

	public virtual void WriteToStream(Stream stream)
	{
		PlayerTick.Serialize(stream, this);
	}

	public virtual void WriteToStreamDelta(Stream stream, PlayerTick previous)
	{
		if (previous == null)
		{
			PlayerTick.Serialize(stream, this);
			return;
		}
		PlayerTick.SerializeDelta(stream, this, previous);
	}
}