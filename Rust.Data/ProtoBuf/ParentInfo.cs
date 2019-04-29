using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class ParentInfo : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public uint uid;

		[NonSerialized]
		public uint bone;

		public bool ShouldPool = true;

		private bool _disposed;

		public ParentInfo()
		{
		}

		public ParentInfo Copy()
		{
			ParentInfo parentInfo = Pool.Get<ParentInfo>();
			this.CopyTo(parentInfo);
			return parentInfo;
		}

		public void CopyTo(ParentInfo instance)
		{
			instance.uid = this.uid;
			instance.bone = this.bone;
		}

		public static ParentInfo Deserialize(Stream stream)
		{
			ParentInfo parentInfo = Pool.Get<ParentInfo>();
			ParentInfo.Deserialize(stream, parentInfo, false);
			return parentInfo;
		}

		public static ParentInfo Deserialize(byte[] buffer)
		{
			ParentInfo parentInfo = Pool.Get<ParentInfo>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				ParentInfo.Deserialize(memoryStream, parentInfo, false);
			}
			return parentInfo;
		}

		public static ParentInfo Deserialize(byte[] buffer, ParentInfo instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				ParentInfo.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static ParentInfo Deserialize(Stream stream, ParentInfo instance, bool isDelta)
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
					instance.bone = ProtocolParser.ReadUInt32(stream);
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

		public static ParentInfo DeserializeLength(Stream stream, int length)
		{
			ParentInfo parentInfo = Pool.Get<ParentInfo>();
			ParentInfo.DeserializeLength(stream, length, parentInfo, false);
			return parentInfo;
		}

		public static ParentInfo DeserializeLength(Stream stream, int length, ParentInfo instance, bool isDelta)
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
					instance.bone = ProtocolParser.ReadUInt32(stream);
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

		public static ParentInfo DeserializeLengthDelimited(Stream stream)
		{
			ParentInfo parentInfo = Pool.Get<ParentInfo>();
			ParentInfo.DeserializeLengthDelimited(stream, parentInfo, false);
			return parentInfo;
		}

		public static ParentInfo DeserializeLengthDelimited(Stream stream, ParentInfo instance, bool isDelta)
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
					instance.bone = ProtocolParser.ReadUInt32(stream);
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
			ParentInfo.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			ParentInfo.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(ParentInfo instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.uid = 0;
			instance.bone = 0;
			Pool.Free<ParentInfo>(ref instance);
		}

		public void ResetToPool()
		{
			ParentInfo.ResetToPool(this);
		}

		public static void Serialize(Stream stream, ParentInfo instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.uid);
			stream.WriteByte(16);
			ProtocolParser.WriteUInt32(stream, instance.bone);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, ParentInfo instance, ParentInfo previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.uid != previous.uid)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt32(stream, instance.uid);
			}
			if (instance.bone != previous.bone)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.bone);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, ParentInfo instance)
		{
			byte[] bytes = ParentInfo.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(ParentInfo instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				ParentInfo.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			ParentInfo.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return ParentInfo.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			ParentInfo.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, ParentInfo previous)
		{
			if (previous == null)
			{
				ParentInfo.Serialize(stream, this);
				return;
			}
			ParentInfo.SerializeDelta(stream, this, previous);
		}
	}
}