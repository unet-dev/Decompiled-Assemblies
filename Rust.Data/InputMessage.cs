using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;
using UnityEngine;

public class InputMessage : IDisposable, Pool.IPooled, IProto
{
	[NonSerialized]
	public int buttons;

	[NonSerialized]
	public Vector3 aimAngles;

	[NonSerialized]
	public Vector3 mouseDelta;

	public bool ShouldPool = true;

	private bool _disposed;

	public InputMessage()
	{
	}

	public InputMessage Copy()
	{
		InputMessage inputMessage = Pool.Get<InputMessage>();
		this.CopyTo(inputMessage);
		return inputMessage;
	}

	public void CopyTo(InputMessage instance)
	{
		instance.buttons = this.buttons;
		instance.aimAngles = this.aimAngles;
		instance.mouseDelta = this.mouseDelta;
	}

	public static InputMessage Deserialize(Stream stream)
	{
		InputMessage inputMessage = Pool.Get<InputMessage>();
		InputMessage.Deserialize(stream, inputMessage, false);
		return inputMessage;
	}

	public static InputMessage Deserialize(byte[] buffer)
	{
		InputMessage inputMessage = Pool.Get<InputMessage>();
		using (MemoryStream memoryStream = new MemoryStream(buffer))
		{
			InputMessage.Deserialize(memoryStream, inputMessage, false);
		}
		return inputMessage;
	}

	public static InputMessage Deserialize(byte[] buffer, InputMessage instance, bool isDelta = false)
	{
		using (MemoryStream memoryStream = new MemoryStream(buffer))
		{
			InputMessage.Deserialize(memoryStream, instance, isDelta);
		}
		return instance;
	}

	public static InputMessage Deserialize(Stream stream, InputMessage instance, bool isDelta)
	{
		while (true)
		{
			int num = stream.ReadByte();
			if (num == -1)
			{
				break;
			}
			if (num == 8)
			{
				instance.buttons = (int)ProtocolParser.ReadUInt64(stream);
			}
			else if (num == 18)
			{
				Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.aimAngles, isDelta);
			}
			else if (num == 26)
			{
				Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.mouseDelta, isDelta);
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

	public static InputMessage DeserializeLength(Stream stream, int length)
	{
		InputMessage inputMessage = Pool.Get<InputMessage>();
		InputMessage.DeserializeLength(stream, length, inputMessage, false);
		return inputMessage;
	}

	public static InputMessage DeserializeLength(Stream stream, int length, InputMessage instance, bool isDelta)
	{
		long position = stream.Position + (long)length;
		while (stream.Position < position)
		{
			int num = stream.ReadByte();
			if (num == -1)
			{
				throw new EndOfStreamException();
			}
			if (num == 8)
			{
				instance.buttons = (int)ProtocolParser.ReadUInt64(stream);
			}
			else if (num == 18)
			{
				Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.aimAngles, isDelta);
			}
			else if (num == 26)
			{
				Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.mouseDelta, isDelta);
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

	public static InputMessage DeserializeLengthDelimited(Stream stream)
	{
		InputMessage inputMessage = Pool.Get<InputMessage>();
		InputMessage.DeserializeLengthDelimited(stream, inputMessage, false);
		return inputMessage;
	}

	public static InputMessage DeserializeLengthDelimited(Stream stream, InputMessage instance, bool isDelta)
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
			if (num == 8)
			{
				instance.buttons = (int)ProtocolParser.ReadUInt64(stream);
			}
			else if (num == 18)
			{
				Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.aimAngles, isDelta);
			}
			else if (num == 26)
			{
				Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.mouseDelta, isDelta);
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
		InputMessage.Deserialize(stream, this, isDelta);
	}

	public virtual void LeavePool()
	{
		this._disposed = false;
	}

	public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
	{
		InputMessage.DeserializeLength(stream, size, this, isDelta);
	}

	public static void ResetToPool(InputMessage instance)
	{
		if (!instance.ShouldPool)
		{
			return;
		}
		instance.buttons = 0;
		instance.aimAngles = new Vector3();
		instance.mouseDelta = new Vector3();
		Pool.Free<InputMessage>(ref instance);
	}

	public void ResetToPool()
	{
		InputMessage.ResetToPool(this);
	}

	public static void Serialize(Stream stream, InputMessage instance)
	{
		MemoryStream memoryStream = Pool.Get<MemoryStream>();
		stream.WriteByte(8);
		ProtocolParser.WriteUInt64(stream, (ulong)instance.buttons);
		stream.WriteByte(18);
		memoryStream.SetLength((long)0);
		Vector3Serialized.Serialize(memoryStream, instance.aimAngles);
		uint length = (uint)memoryStream.Length;
		ProtocolParser.WriteUInt32(stream, length);
		stream.Write(memoryStream.GetBuffer(), 0, (int)length);
		stream.WriteByte(26);
		memoryStream.SetLength((long)0);
		Vector3Serialized.Serialize(memoryStream, instance.mouseDelta);
		uint num = (uint)memoryStream.Length;
		ProtocolParser.WriteUInt32(stream, num);
		stream.Write(memoryStream.GetBuffer(), 0, (int)num);
		Pool.FreeMemoryStream(ref memoryStream);
	}

	public static void SerializeDelta(Stream stream, InputMessage instance, InputMessage previous)
	{
		MemoryStream memoryStream = Pool.Get<MemoryStream>();
		if (instance.buttons != previous.buttons)
		{
			stream.WriteByte(8);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.buttons);
		}
		if (instance.aimAngles != previous.aimAngles)
		{
			stream.WriteByte(18);
			memoryStream.SetLength((long)0);
			Vector3Serialized.SerializeDelta(memoryStream, instance.aimAngles, previous.aimAngles);
			uint length = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length);
		}
		if (instance.mouseDelta != previous.mouseDelta)
		{
			stream.WriteByte(26);
			memoryStream.SetLength((long)0);
			Vector3Serialized.SerializeDelta(memoryStream, instance.mouseDelta, previous.mouseDelta);
			uint num = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, num);
			stream.Write(memoryStream.GetBuffer(), 0, (int)num);
		}
		Pool.FreeMemoryStream(ref memoryStream);
	}

	public static void SerializeLengthDelimited(Stream stream, InputMessage instance)
	{
		byte[] bytes = InputMessage.SerializeToBytes(instance);
		ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
		stream.Write(bytes, 0, (int)bytes.Length);
	}

	public static byte[] SerializeToBytes(InputMessage instance)
	{
		byte[] array;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			InputMessage.Serialize(memoryStream, instance);
			array = memoryStream.ToArray();
		}
		return array;
	}

	public void ToProto(Stream stream)
	{
		InputMessage.Serialize(stream, this);
	}

	public byte[] ToProtoBytes()
	{
		return InputMessage.SerializeToBytes(this);
	}

	public virtual void WriteToStream(Stream stream)
	{
		InputMessage.Serialize(stream, this);
	}

	public virtual void WriteToStreamDelta(Stream stream, InputMessage previous)
	{
		if (previous == null)
		{
			InputMessage.Serialize(stream, this);
			return;
		}
		InputMessage.SerializeDelta(stream, this, previous);
	}
}