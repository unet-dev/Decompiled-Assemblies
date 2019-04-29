using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class StorageBox : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public ItemContainer contents;

		public bool ShouldPool = true;

		private bool _disposed;

		public StorageBox()
		{
		}

		public StorageBox Copy()
		{
			StorageBox storageBox = Pool.Get<StorageBox>();
			this.CopyTo(storageBox);
			return storageBox;
		}

		public void CopyTo(StorageBox instance)
		{
			if (this.contents == null)
			{
				instance.contents = null;
				return;
			}
			if (instance.contents == null)
			{
				instance.contents = this.contents.Copy();
				return;
			}
			this.contents.CopyTo(instance.contents);
		}

		public static StorageBox Deserialize(Stream stream)
		{
			StorageBox storageBox = Pool.Get<StorageBox>();
			StorageBox.Deserialize(stream, storageBox, false);
			return storageBox;
		}

		public static StorageBox Deserialize(byte[] buffer)
		{
			StorageBox storageBox = Pool.Get<StorageBox>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				StorageBox.Deserialize(memoryStream, storageBox, false);
			}
			return storageBox;
		}

		public static StorageBox Deserialize(byte[] buffer, StorageBox instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				StorageBox.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static StorageBox Deserialize(Stream stream, StorageBox instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num != 10)
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
				else if (instance.contents != null)
				{
					ItemContainer.DeserializeLengthDelimited(stream, instance.contents, isDelta);
				}
				else
				{
					instance.contents = ItemContainer.DeserializeLengthDelimited(stream);
				}
			}
			return instance;
		}

		public static StorageBox DeserializeLength(Stream stream, int length)
		{
			StorageBox storageBox = Pool.Get<StorageBox>();
			StorageBox.DeserializeLength(stream, length, storageBox, false);
			return storageBox;
		}

		public static StorageBox DeserializeLength(Stream stream, int length, StorageBox instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num != 10)
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
				else if (instance.contents != null)
				{
					ItemContainer.DeserializeLengthDelimited(stream, instance.contents, isDelta);
				}
				else
				{
					instance.contents = ItemContainer.DeserializeLengthDelimited(stream);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static StorageBox DeserializeLengthDelimited(Stream stream)
		{
			StorageBox storageBox = Pool.Get<StorageBox>();
			StorageBox.DeserializeLengthDelimited(stream, storageBox, false);
			return storageBox;
		}

		public static StorageBox DeserializeLengthDelimited(Stream stream, StorageBox instance, bool isDelta)
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
				if (num != 10)
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
				else if (instance.contents != null)
				{
					ItemContainer.DeserializeLengthDelimited(stream, instance.contents, isDelta);
				}
				else
				{
					instance.contents = ItemContainer.DeserializeLengthDelimited(stream);
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
			StorageBox.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			StorageBox.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(StorageBox instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			if (instance.contents != null)
			{
				instance.contents.ResetToPool();
				instance.contents = null;
			}
			Pool.Free<StorageBox>(ref instance);
		}

		public void ResetToPool()
		{
			StorageBox.ResetToPool(this);
		}

		public static void Serialize(Stream stream, StorageBox instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.contents != null)
			{
				stream.WriteByte(10);
				memoryStream.SetLength((long)0);
				ItemContainer.Serialize(memoryStream, instance.contents);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, StorageBox instance, StorageBox previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.contents != null)
			{
				stream.WriteByte(10);
				memoryStream.SetLength((long)0);
				ItemContainer.SerializeDelta(memoryStream, instance.contents, previous.contents);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, StorageBox instance)
		{
			byte[] bytes = StorageBox.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(StorageBox instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				StorageBox.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			StorageBox.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return StorageBox.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			StorageBox.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, StorageBox previous)
		{
			if (previous == null)
			{
				StorageBox.Serialize(stream, this);
				return;
			}
			StorageBox.SerializeDelta(stream, this, previous);
		}
	}
}