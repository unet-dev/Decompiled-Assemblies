using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace UnityEngine
{
	public class Vector3Serialized
	{
		public Vector3Serialized()
		{
		}

		public static Vector3 Deserialize(byte[] buffer, ref Vector3 instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				Vector3Serialized.Deserialize(memoryStream, ref instance, isDelta);
			}
			return instance;
		}

		public static Vector3 Deserialize(Stream stream, ref Vector3 instance, bool isDelta)
		{
			if (!isDelta)
			{
				instance.x = 0f;
				instance.y = 0f;
				instance.z = 0f;
			}
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num == 13)
				{
					instance.x = ProtocolParser.ReadSingle(stream);
				}
				else if (num == 21)
				{
					instance.y = ProtocolParser.ReadSingle(stream);
				}
				else if (num == 29)
				{
					instance.z = ProtocolParser.ReadSingle(stream);
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

		public static Vector3 DeserializeLength(Stream stream, int length, ref Vector3 instance, bool isDelta)
		{
			if (!isDelta)
			{
				instance.x = 0f;
				instance.y = 0f;
				instance.z = 0f;
			}
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
					instance.x = ProtocolParser.ReadSingle(stream);
				}
				else if (num == 21)
				{
					instance.y = ProtocolParser.ReadSingle(stream);
				}
				else if (num == 29)
				{
					instance.z = ProtocolParser.ReadSingle(stream);
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

		public static Vector3 DeserializeLengthDelimited(Stream stream, ref Vector3 instance, bool isDelta)
		{
			if (!isDelta)
			{
				instance.x = 0f;
				instance.y = 0f;
				instance.z = 0f;
			}
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
					instance.x = ProtocolParser.ReadSingle(stream);
				}
				else if (num == 21)
				{
					instance.y = ProtocolParser.ReadSingle(stream);
				}
				else if (num == 29)
				{
					instance.z = ProtocolParser.ReadSingle(stream);
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

		public static void ResetToPool(Vector3 instance)
		{
			instance.x = 0f;
			instance.y = 0f;
			instance.z = 0f;
		}

		public static void Serialize(Stream stream, Vector3 instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(13);
			ProtocolParser.WriteSingle(stream, instance.x);
			stream.WriteByte(21);
			ProtocolParser.WriteSingle(stream, instance.y);
			stream.WriteByte(29);
			ProtocolParser.WriteSingle(stream, instance.z);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, Vector3 instance, Vector3 previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.x != previous.x)
			{
				stream.WriteByte(13);
				ProtocolParser.WriteSingle(stream, instance.x);
			}
			if (instance.y != previous.y)
			{
				stream.WriteByte(21);
				ProtocolParser.WriteSingle(stream, instance.y);
			}
			if (instance.z != previous.z)
			{
				stream.WriteByte(29);
				ProtocolParser.WriteSingle(stream, instance.z);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, Vector3 instance)
		{
			byte[] bytes = Vector3Serialized.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(Vector3 instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Vector3Serialized.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}
	}
}