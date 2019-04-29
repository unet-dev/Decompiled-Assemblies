using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class ResourceExtractor : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public ItemContainer fuelContents;

		[NonSerialized]
		public ItemContainer outputContents;

		public bool ShouldPool = true;

		private bool _disposed;

		public ResourceExtractor()
		{
		}

		public ResourceExtractor Copy()
		{
			ResourceExtractor resourceExtractor = Pool.Get<ResourceExtractor>();
			this.CopyTo(resourceExtractor);
			return resourceExtractor;
		}

		public void CopyTo(ResourceExtractor instance)
		{
			if (this.fuelContents == null)
			{
				instance.fuelContents = null;
			}
			else if (instance.fuelContents != null)
			{
				this.fuelContents.CopyTo(instance.fuelContents);
			}
			else
			{
				instance.fuelContents = this.fuelContents.Copy();
			}
			if (this.outputContents == null)
			{
				instance.outputContents = null;
				return;
			}
			if (instance.outputContents == null)
			{
				instance.outputContents = this.outputContents.Copy();
				return;
			}
			this.outputContents.CopyTo(instance.outputContents);
		}

		public static ResourceExtractor Deserialize(Stream stream)
		{
			ResourceExtractor resourceExtractor = Pool.Get<ResourceExtractor>();
			ResourceExtractor.Deserialize(stream, resourceExtractor, false);
			return resourceExtractor;
		}

		public static ResourceExtractor Deserialize(byte[] buffer)
		{
			ResourceExtractor resourceExtractor = Pool.Get<ResourceExtractor>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				ResourceExtractor.Deserialize(memoryStream, resourceExtractor, false);
			}
			return resourceExtractor;
		}

		public static ResourceExtractor Deserialize(byte[] buffer, ResourceExtractor instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				ResourceExtractor.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static ResourceExtractor Deserialize(Stream stream, ResourceExtractor instance, bool isDelta)
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
					if (instance.fuelContents != null)
					{
						ItemContainer.DeserializeLengthDelimited(stream, instance.fuelContents, isDelta);
					}
					else
					{
						instance.fuelContents = ItemContainer.DeserializeLengthDelimited(stream);
					}
				}
				else if (num != 18)
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
				else if (instance.outputContents != null)
				{
					ItemContainer.DeserializeLengthDelimited(stream, instance.outputContents, isDelta);
				}
				else
				{
					instance.outputContents = ItemContainer.DeserializeLengthDelimited(stream);
				}
			}
			return instance;
		}

		public static ResourceExtractor DeserializeLength(Stream stream, int length)
		{
			ResourceExtractor resourceExtractor = Pool.Get<ResourceExtractor>();
			ResourceExtractor.DeserializeLength(stream, length, resourceExtractor, false);
			return resourceExtractor;
		}

		public static ResourceExtractor DeserializeLength(Stream stream, int length, ResourceExtractor instance, bool isDelta)
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
					if (instance.fuelContents != null)
					{
						ItemContainer.DeserializeLengthDelimited(stream, instance.fuelContents, isDelta);
					}
					else
					{
						instance.fuelContents = ItemContainer.DeserializeLengthDelimited(stream);
					}
				}
				else if (num != 18)
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
				else if (instance.outputContents != null)
				{
					ItemContainer.DeserializeLengthDelimited(stream, instance.outputContents, isDelta);
				}
				else
				{
					instance.outputContents = ItemContainer.DeserializeLengthDelimited(stream);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static ResourceExtractor DeserializeLengthDelimited(Stream stream)
		{
			ResourceExtractor resourceExtractor = Pool.Get<ResourceExtractor>();
			ResourceExtractor.DeserializeLengthDelimited(stream, resourceExtractor, false);
			return resourceExtractor;
		}

		public static ResourceExtractor DeserializeLengthDelimited(Stream stream, ResourceExtractor instance, bool isDelta)
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
					if (instance.fuelContents != null)
					{
						ItemContainer.DeserializeLengthDelimited(stream, instance.fuelContents, isDelta);
					}
					else
					{
						instance.fuelContents = ItemContainer.DeserializeLengthDelimited(stream);
					}
				}
				else if (num != 18)
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
				else if (instance.outputContents != null)
				{
					ItemContainer.DeserializeLengthDelimited(stream, instance.outputContents, isDelta);
				}
				else
				{
					instance.outputContents = ItemContainer.DeserializeLengthDelimited(stream);
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
			ResourceExtractor.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			ResourceExtractor.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(ResourceExtractor instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			if (instance.fuelContents != null)
			{
				instance.fuelContents.ResetToPool();
				instance.fuelContents = null;
			}
			if (instance.outputContents != null)
			{
				instance.outputContents.ResetToPool();
				instance.outputContents = null;
			}
			Pool.Free<ResourceExtractor>(ref instance);
		}

		public void ResetToPool()
		{
			ResourceExtractor.ResetToPool(this);
		}

		public static void Serialize(Stream stream, ResourceExtractor instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.fuelContents != null)
			{
				stream.WriteByte(10);
				memoryStream.SetLength((long)0);
				ItemContainer.Serialize(memoryStream, instance.fuelContents);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			if (instance.outputContents != null)
			{
				stream.WriteByte(18);
				memoryStream.SetLength((long)0);
				ItemContainer.Serialize(memoryStream, instance.outputContents);
				uint num = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, ResourceExtractor instance, ResourceExtractor previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.fuelContents != null)
			{
				stream.WriteByte(10);
				memoryStream.SetLength((long)0);
				ItemContainer.SerializeDelta(memoryStream, instance.fuelContents, previous.fuelContents);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			if (instance.outputContents != null)
			{
				stream.WriteByte(18);
				memoryStream.SetLength((long)0);
				ItemContainer.SerializeDelta(memoryStream, instance.outputContents, previous.outputContents);
				uint num = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, ResourceExtractor instance)
		{
			byte[] bytes = ResourceExtractor.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(ResourceExtractor instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				ResourceExtractor.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			ResourceExtractor.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return ResourceExtractor.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			ResourceExtractor.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, ResourceExtractor previous)
		{
			if (previous == null)
			{
				ResourceExtractor.Serialize(stream, this);
				return;
			}
			ResourceExtractor.SerializeDelta(stream, this, previous);
		}
	}
}