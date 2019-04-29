using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class OwnerInfo : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public ulong steamid;

		public bool ShouldPool = true;

		private bool _disposed;

		public OwnerInfo()
		{
		}

		public OwnerInfo Copy()
		{
			OwnerInfo ownerInfo = Pool.Get<OwnerInfo>();
			this.CopyTo(ownerInfo);
			return ownerInfo;
		}

		public void CopyTo(OwnerInfo instance)
		{
			instance.steamid = this.steamid;
		}

		public static OwnerInfo Deserialize(Stream stream)
		{
			OwnerInfo ownerInfo = Pool.Get<OwnerInfo>();
			OwnerInfo.Deserialize(stream, ownerInfo, false);
			return ownerInfo;
		}

		public static OwnerInfo Deserialize(byte[] buffer)
		{
			OwnerInfo ownerInfo = Pool.Get<OwnerInfo>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				OwnerInfo.Deserialize(memoryStream, ownerInfo, false);
			}
			return ownerInfo;
		}

		public static OwnerInfo Deserialize(byte[] buffer, OwnerInfo instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				OwnerInfo.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static OwnerInfo Deserialize(Stream stream, OwnerInfo instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num != 8)
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
				else
				{
					instance.steamid = ProtocolParser.ReadUInt64(stream);
				}
			}
			return instance;
		}

		public static OwnerInfo DeserializeLength(Stream stream, int length)
		{
			OwnerInfo ownerInfo = Pool.Get<OwnerInfo>();
			OwnerInfo.DeserializeLength(stream, length, ownerInfo, false);
			return ownerInfo;
		}

		public static OwnerInfo DeserializeLength(Stream stream, int length, OwnerInfo instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num != 8)
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
				else
				{
					instance.steamid = ProtocolParser.ReadUInt64(stream);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static OwnerInfo DeserializeLengthDelimited(Stream stream)
		{
			OwnerInfo ownerInfo = Pool.Get<OwnerInfo>();
			OwnerInfo.DeserializeLengthDelimited(stream, ownerInfo, false);
			return ownerInfo;
		}

		public static OwnerInfo DeserializeLengthDelimited(Stream stream, OwnerInfo instance, bool isDelta)
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
				if (num != 8)
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
				else
				{
					instance.steamid = ProtocolParser.ReadUInt64(stream);
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
			OwnerInfo.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			OwnerInfo.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(OwnerInfo instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.steamid = (ulong)0;
			Pool.Free<OwnerInfo>(ref instance);
		}

		public void ResetToPool()
		{
			OwnerInfo.ResetToPool(this);
		}

		public static void Serialize(Stream stream, OwnerInfo instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt64(stream, instance.steamid);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, OwnerInfo instance, OwnerInfo previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.steamid != previous.steamid)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt64(stream, instance.steamid);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, OwnerInfo instance)
		{
			byte[] bytes = OwnerInfo.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(OwnerInfo instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				OwnerInfo.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			OwnerInfo.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return OwnerInfo.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			OwnerInfo.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, OwnerInfo previous)
		{
			if (previous == null)
			{
				OwnerInfo.Serialize(stream, this);
				return;
			}
			OwnerInfo.SerializeDelta(stream, this, previous);
		}
	}
}