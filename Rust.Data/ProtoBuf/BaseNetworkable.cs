using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class BaseNetworkable : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public uint uid;

		[NonSerialized]
		public uint @group;

		[NonSerialized]
		public uint prefabID;

		public bool ShouldPool = true;

		private bool _disposed;

		public BaseNetworkable()
		{
		}

		public BaseNetworkable Copy()
		{
			BaseNetworkable baseNetworkable = Pool.Get<BaseNetworkable>();
			this.CopyTo(baseNetworkable);
			return baseNetworkable;
		}

		public void CopyTo(BaseNetworkable instance)
		{
			instance.uid = this.uid;
			instance.@group = this.@group;
			instance.prefabID = this.prefabID;
		}

		public static BaseNetworkable Deserialize(Stream stream)
		{
			BaseNetworkable baseNetworkable = Pool.Get<BaseNetworkable>();
			BaseNetworkable.Deserialize(stream, baseNetworkable, false);
			return baseNetworkable;
		}

		public static BaseNetworkable Deserialize(byte[] buffer)
		{
			BaseNetworkable baseNetworkable = Pool.Get<BaseNetworkable>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				BaseNetworkable.Deserialize(memoryStream, baseNetworkable, false);
			}
			return baseNetworkable;
		}

		public static BaseNetworkable Deserialize(byte[] buffer, BaseNetworkable instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				BaseNetworkable.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static BaseNetworkable Deserialize(Stream stream, BaseNetworkable instance, bool isDelta)
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
					instance.uid = ProtocolParser.ReadUInt32(stream);
				}
				else if (num == 16)
				{
					instance.@group = ProtocolParser.ReadUInt32(stream);
				}
				else if (num == 24)
				{
					instance.prefabID = ProtocolParser.ReadUInt32(stream);
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

		public static BaseNetworkable DeserializeLength(Stream stream, int length)
		{
			BaseNetworkable baseNetworkable = Pool.Get<BaseNetworkable>();
			BaseNetworkable.DeserializeLength(stream, length, baseNetworkable, false);
			return baseNetworkable;
		}

		public static BaseNetworkable DeserializeLength(Stream stream, int length, BaseNetworkable instance, bool isDelta)
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
					instance.uid = ProtocolParser.ReadUInt32(stream);
				}
				else if (num == 16)
				{
					instance.@group = ProtocolParser.ReadUInt32(stream);
				}
				else if (num == 24)
				{
					instance.prefabID = ProtocolParser.ReadUInt32(stream);
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

		public static BaseNetworkable DeserializeLengthDelimited(Stream stream)
		{
			BaseNetworkable baseNetworkable = Pool.Get<BaseNetworkable>();
			BaseNetworkable.DeserializeLengthDelimited(stream, baseNetworkable, false);
			return baseNetworkable;
		}

		public static BaseNetworkable DeserializeLengthDelimited(Stream stream, BaseNetworkable instance, bool isDelta)
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
					instance.uid = ProtocolParser.ReadUInt32(stream);
				}
				else if (num == 16)
				{
					instance.@group = ProtocolParser.ReadUInt32(stream);
				}
				else if (num == 24)
				{
					instance.prefabID = ProtocolParser.ReadUInt32(stream);
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
			BaseNetworkable.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			BaseNetworkable.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(BaseNetworkable instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.uid = 0;
			instance.@group = 0;
			instance.prefabID = 0;
			Pool.Free<BaseNetworkable>(ref instance);
		}

		public void ResetToPool()
		{
			BaseNetworkable.ResetToPool(this);
		}

		public static void Serialize(Stream stream, BaseNetworkable instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.uid);
			stream.WriteByte(16);
			ProtocolParser.WriteUInt32(stream, instance.@group);
			stream.WriteByte(24);
			ProtocolParser.WriteUInt32(stream, instance.prefabID);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, BaseNetworkable instance, BaseNetworkable previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.uid != previous.uid)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt32(stream, instance.uid);
			}
			if (instance.@group != previous.@group)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.@group);
			}
			if (instance.prefabID != previous.prefabID)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt32(stream, instance.prefabID);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, BaseNetworkable instance)
		{
			byte[] bytes = BaseNetworkable.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(BaseNetworkable instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BaseNetworkable.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			BaseNetworkable.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return BaseNetworkable.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			BaseNetworkable.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, BaseNetworkable previous)
		{
			if (previous == null)
			{
				BaseNetworkable.Serialize(stream, this);
				return;
			}
			BaseNetworkable.SerializeDelta(stream, this, previous);
		}
	}
}