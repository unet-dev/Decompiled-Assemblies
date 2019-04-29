using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;
using UnityEngine;

namespace ProtoBuf
{
	public class CreateBuilding : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public uint entity;

		[NonSerialized]
		public uint socket;

		[NonSerialized]
		public bool onterrain;

		[NonSerialized]
		public Vector3 position;

		[NonSerialized]
		public Vector3 normal;

		[NonSerialized]
		public Ray ray;

		[NonSerialized]
		public uint blockID;

		[NonSerialized]
		public Vector3 rotation;

		public bool ShouldPool = true;

		private bool _disposed;

		public CreateBuilding()
		{
		}

		public CreateBuilding Copy()
		{
			CreateBuilding createBuilding = Pool.Get<CreateBuilding>();
			this.CopyTo(createBuilding);
			return createBuilding;
		}

		public void CopyTo(CreateBuilding instance)
		{
			instance.entity = this.entity;
			instance.socket = this.socket;
			instance.onterrain = this.onterrain;
			instance.position = this.position;
			instance.normal = this.normal;
			instance.ray = this.ray;
			instance.blockID = this.blockID;
			instance.rotation = this.rotation;
		}

		public static CreateBuilding Deserialize(Stream stream)
		{
			CreateBuilding createBuilding = Pool.Get<CreateBuilding>();
			CreateBuilding.Deserialize(stream, createBuilding, false);
			return createBuilding;
		}

		public static CreateBuilding Deserialize(byte[] buffer)
		{
			CreateBuilding createBuilding = Pool.Get<CreateBuilding>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				CreateBuilding.Deserialize(memoryStream, createBuilding, false);
			}
			return createBuilding;
		}

		public static CreateBuilding Deserialize(byte[] buffer, CreateBuilding instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				CreateBuilding.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static CreateBuilding Deserialize(Stream stream, CreateBuilding instance, bool isDelta)
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
							instance.entity = ProtocolParser.ReadUInt32(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.socket = ProtocolParser.ReadUInt32(stream);
							continue;
						}
					}
					else if (num == 24)
					{
						instance.onterrain = ProtocolParser.ReadBool(stream);
						continue;
					}
					else if (num == 34)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.position, isDelta);
						continue;
					}
				}
				else if (num <= 50)
				{
					if (num == 42)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.normal, isDelta);
						continue;
					}
					else if (num == 50)
					{
						RaySerialized.DeserializeLengthDelimited(stream, ref instance.ray, isDelta);
						continue;
					}
				}
				else if (num == 56)
				{
					instance.blockID = ProtocolParser.ReadUInt32(stream);
					continue;
				}
				else if (num == 66)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.rotation, isDelta);
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

		public static CreateBuilding DeserializeLength(Stream stream, int length)
		{
			CreateBuilding createBuilding = Pool.Get<CreateBuilding>();
			CreateBuilding.DeserializeLength(stream, length, createBuilding, false);
			return createBuilding;
		}

		public static CreateBuilding DeserializeLength(Stream stream, int length, CreateBuilding instance, bool isDelta)
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
							instance.entity = ProtocolParser.ReadUInt32(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.socket = ProtocolParser.ReadUInt32(stream);
							continue;
						}
					}
					else if (num == 24)
					{
						instance.onterrain = ProtocolParser.ReadBool(stream);
						continue;
					}
					else if (num == 34)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.position, isDelta);
						continue;
					}
				}
				else if (num <= 50)
				{
					if (num == 42)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.normal, isDelta);
						continue;
					}
					else if (num == 50)
					{
						RaySerialized.DeserializeLengthDelimited(stream, ref instance.ray, isDelta);
						continue;
					}
				}
				else if (num == 56)
				{
					instance.blockID = ProtocolParser.ReadUInt32(stream);
					continue;
				}
				else if (num == 66)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.rotation, isDelta);
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

		public static CreateBuilding DeserializeLengthDelimited(Stream stream)
		{
			CreateBuilding createBuilding = Pool.Get<CreateBuilding>();
			CreateBuilding.DeserializeLengthDelimited(stream, createBuilding, false);
			return createBuilding;
		}

		public static CreateBuilding DeserializeLengthDelimited(Stream stream, CreateBuilding instance, bool isDelta)
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
							instance.entity = ProtocolParser.ReadUInt32(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.socket = ProtocolParser.ReadUInt32(stream);
							continue;
						}
					}
					else if (num == 24)
					{
						instance.onterrain = ProtocolParser.ReadBool(stream);
						continue;
					}
					else if (num == 34)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.position, isDelta);
						continue;
					}
				}
				else if (num <= 50)
				{
					if (num == 42)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.normal, isDelta);
						continue;
					}
					else if (num == 50)
					{
						RaySerialized.DeserializeLengthDelimited(stream, ref instance.ray, isDelta);
						continue;
					}
				}
				else if (num == 56)
				{
					instance.blockID = ProtocolParser.ReadUInt32(stream);
					continue;
				}
				else if (num == 66)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.rotation, isDelta);
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
			CreateBuilding.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			CreateBuilding.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(CreateBuilding instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.entity = 0;
			instance.socket = 0;
			instance.onterrain = false;
			instance.position = new Vector3();
			instance.normal = new Vector3();
			instance.ray = new Ray();
			instance.blockID = 0;
			instance.rotation = new Vector3();
			Pool.Free<CreateBuilding>(ref instance);
		}

		public void ResetToPool()
		{
			CreateBuilding.ResetToPool(this);
		}

		public static void Serialize(Stream stream, CreateBuilding instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.entity);
			stream.WriteByte(16);
			ProtocolParser.WriteUInt32(stream, instance.socket);
			stream.WriteByte(24);
			ProtocolParser.WriteBool(stream, instance.onterrain);
			stream.WriteByte(34);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.position);
			uint length = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			stream.WriteByte(42);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.normal);
			uint num = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, num);
			stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			stream.WriteByte(50);
			memoryStream.SetLength((long)0);
			RaySerialized.Serialize(memoryStream, instance.ray);
			uint length1 = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length1);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length1);
			stream.WriteByte(56);
			ProtocolParser.WriteUInt32(stream, instance.blockID);
			stream.WriteByte(66);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.rotation);
			uint num1 = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, num1);
			stream.Write(memoryStream.GetBuffer(), 0, (int)num1);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, CreateBuilding instance, CreateBuilding previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.entity != previous.entity)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt32(stream, instance.entity);
			}
			if (instance.socket != previous.socket)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.socket);
			}
			stream.WriteByte(24);
			ProtocolParser.WriteBool(stream, instance.onterrain);
			if (instance.position != previous.position)
			{
				stream.WriteByte(34);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.position, previous.position);
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
			stream.WriteByte(50);
			memoryStream.SetLength((long)0);
			RaySerialized.SerializeDelta(memoryStream, instance.ray, previous.ray);
			uint length1 = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length1);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length1);
			if (instance.blockID != previous.blockID)
			{
				stream.WriteByte(56);
				ProtocolParser.WriteUInt32(stream, instance.blockID);
			}
			if (instance.rotation != previous.rotation)
			{
				stream.WriteByte(66);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.rotation, previous.rotation);
				uint num1 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num1);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num1);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, CreateBuilding instance)
		{
			byte[] bytes = CreateBuilding.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(CreateBuilding instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				CreateBuilding.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			CreateBuilding.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return CreateBuilding.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			CreateBuilding.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, CreateBuilding previous)
		{
			if (previous == null)
			{
				CreateBuilding.Serialize(stream, this);
				return;
			}
			CreateBuilding.SerializeDelta(stream, this, previous);
		}
	}
}