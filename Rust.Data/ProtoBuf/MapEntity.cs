using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProtoBuf
{
	public class MapEntity : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public List<uint> fogImages;

		[NonSerialized]
		public List<uint> paintImages;

		public bool ShouldPool = true;

		private bool _disposed;

		public MapEntity()
		{
		}

		public MapEntity Copy()
		{
			MapEntity mapEntity = Pool.Get<MapEntity>();
			this.CopyTo(mapEntity);
			return mapEntity;
		}

		public void CopyTo(MapEntity instance)
		{
			throw new NotImplementedException();
		}

		public static MapEntity Deserialize(Stream stream)
		{
			MapEntity mapEntity = Pool.Get<MapEntity>();
			MapEntity.Deserialize(stream, mapEntity, false);
			return mapEntity;
		}

		public static MapEntity Deserialize(byte[] buffer)
		{
			MapEntity mapEntity = Pool.Get<MapEntity>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				MapEntity.Deserialize(memoryStream, mapEntity, false);
			}
			return mapEntity;
		}

		public static MapEntity Deserialize(byte[] buffer, MapEntity instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				MapEntity.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static MapEntity Deserialize(Stream stream, MapEntity instance, bool isDelta)
		{
			if (!isDelta)
			{
				if (instance.fogImages == null)
				{
					instance.fogImages = Pool.Get<List<uint>>();
				}
				if (instance.paintImages == null)
				{
					instance.paintImages = Pool.Get<List<uint>>();
				}
			}
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num == 8)
				{
					instance.fogImages.Add(ProtocolParser.ReadUInt32(stream));
				}
				else if (num == 16)
				{
					instance.paintImages.Add(ProtocolParser.ReadUInt32(stream));
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

		public static MapEntity DeserializeLength(Stream stream, int length)
		{
			MapEntity mapEntity = Pool.Get<MapEntity>();
			MapEntity.DeserializeLength(stream, length, mapEntity, false);
			return mapEntity;
		}

		public static MapEntity DeserializeLength(Stream stream, int length, MapEntity instance, bool isDelta)
		{
			if (!isDelta)
			{
				if (instance.fogImages == null)
				{
					instance.fogImages = Pool.Get<List<uint>>();
				}
				if (instance.paintImages == null)
				{
					instance.paintImages = Pool.Get<List<uint>>();
				}
			}
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
					instance.fogImages.Add(ProtocolParser.ReadUInt32(stream));
				}
				else if (num == 16)
				{
					instance.paintImages.Add(ProtocolParser.ReadUInt32(stream));
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

		public static MapEntity DeserializeLengthDelimited(Stream stream)
		{
			MapEntity mapEntity = Pool.Get<MapEntity>();
			MapEntity.DeserializeLengthDelimited(stream, mapEntity, false);
			return mapEntity;
		}

		public static MapEntity DeserializeLengthDelimited(Stream stream, MapEntity instance, bool isDelta)
		{
			if (!isDelta)
			{
				if (instance.fogImages == null)
				{
					instance.fogImages = Pool.Get<List<uint>>();
				}
				if (instance.paintImages == null)
				{
					instance.paintImages = Pool.Get<List<uint>>();
				}
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
				if (num == 8)
				{
					instance.fogImages.Add(ProtocolParser.ReadUInt32(stream));
				}
				else if (num == 16)
				{
					instance.paintImages.Add(ProtocolParser.ReadUInt32(stream));
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
			MapEntity.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			MapEntity.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(MapEntity instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			if (instance.fogImages != null)
			{
				List<uint> nums = instance.fogImages;
				Pool.FreeList<uint>(ref nums);
				instance.fogImages = nums;
			}
			if (instance.paintImages != null)
			{
				List<uint> nums1 = instance.paintImages;
				Pool.FreeList<uint>(ref nums1);
				instance.paintImages = nums1;
			}
			Pool.Free<MapEntity>(ref instance);
		}

		public void ResetToPool()
		{
			MapEntity.ResetToPool(this);
		}

		public static void Serialize(Stream stream, MapEntity instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.fogImages != null)
			{
				for (int i = 0; i < instance.fogImages.Count; i++)
				{
					uint item = instance.fogImages[i];
					stream.WriteByte(8);
					ProtocolParser.WriteUInt32(stream, item);
				}
			}
			if (instance.paintImages != null)
			{
				for (int j = 0; j < instance.paintImages.Count; j++)
				{
					uint num = instance.paintImages[j];
					stream.WriteByte(16);
					ProtocolParser.WriteUInt32(stream, num);
				}
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, MapEntity instance, MapEntity previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.fogImages != null)
			{
				for (int i = 0; i < instance.fogImages.Count; i++)
				{
					uint item = instance.fogImages[i];
					stream.WriteByte(8);
					ProtocolParser.WriteUInt32(stream, item);
				}
			}
			if (instance.paintImages != null)
			{
				for (int j = 0; j < instance.paintImages.Count; j++)
				{
					uint num = instance.paintImages[j];
					stream.WriteByte(16);
					ProtocolParser.WriteUInt32(stream, num);
				}
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, MapEntity instance)
		{
			byte[] bytes = MapEntity.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(MapEntity instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				MapEntity.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			MapEntity.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return MapEntity.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			MapEntity.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, MapEntity previous)
		{
			if (previous == null)
			{
				MapEntity.Serialize(stream, this);
				return;
			}
			MapEntity.SerializeDelta(stream, this, previous);
		}
	}
}