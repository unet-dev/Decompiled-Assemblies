using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace UnityEngine
{
	public class RaySerialized
	{
		public RaySerialized()
		{
		}

		public static Ray Deserialize(byte[] buffer, ref Ray instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				RaySerialized.Deserialize(memoryStream, ref instance, isDelta);
			}
			return instance;
		}

		public static Ray Deserialize(Stream stream, ref Ray instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num == 10)
				{
					Vector3 vector3 = instance.origin;
					instance.origin = Vector3Serialized.DeserializeLengthDelimited(stream, ref vector3, isDelta);
				}
				else if (num == 18)
				{
					Vector3 vector31 = instance.direction;
					instance.direction = Vector3Serialized.DeserializeLengthDelimited(stream, ref vector31, isDelta);
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

		public static Ray DeserializeLength(Stream stream, int length, ref Ray instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num == 10)
				{
					Vector3 vector3 = instance.origin;
					instance.origin = Vector3Serialized.DeserializeLengthDelimited(stream, ref vector3, isDelta);
				}
				else if (num == 18)
				{
					Vector3 vector31 = instance.direction;
					instance.direction = Vector3Serialized.DeserializeLengthDelimited(stream, ref vector31, isDelta);
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

		public static Ray DeserializeLengthDelimited(Stream stream, ref Ray instance, bool isDelta)
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
				if (num == 10)
				{
					Vector3 vector3 = instance.origin;
					instance.origin = Vector3Serialized.DeserializeLengthDelimited(stream, ref vector3, isDelta);
				}
				else if (num == 18)
				{
					Vector3 vector31 = instance.direction;
					instance.direction = Vector3Serialized.DeserializeLengthDelimited(stream, ref vector31, isDelta);
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

		public static void ResetToPool(Ray instance)
		{
			Vector3 vector3 = new Vector3();
			instance.origin = vector3;
			vector3 = new Vector3();
			instance.direction = vector3;
		}

		public static void Serialize(Stream stream, Ray instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(10);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.origin);
			uint length = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			stream.WriteByte(18);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.direction);
			uint num = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, num);
			stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, Ray instance, Ray previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.origin != previous.origin)
			{
				stream.WriteByte(10);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.origin, previous.origin);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			if (instance.direction != previous.direction)
			{
				stream.WriteByte(18);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.direction, previous.direction);
				uint num = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, Ray instance)
		{
			byte[] bytes = RaySerialized.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(Ray instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				RaySerialized.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}
	}
}