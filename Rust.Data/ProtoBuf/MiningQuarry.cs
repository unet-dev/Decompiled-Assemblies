using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class MiningQuarry : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public ResourceExtractor extractor;

		[NonSerialized]
		public int staticType;

		public bool ShouldPool = true;

		private bool _disposed;

		public MiningQuarry()
		{
		}

		public MiningQuarry Copy()
		{
			MiningQuarry miningQuarry = Pool.Get<MiningQuarry>();
			this.CopyTo(miningQuarry);
			return miningQuarry;
		}

		public void CopyTo(MiningQuarry instance)
		{
			if (this.extractor == null)
			{
				instance.extractor = null;
			}
			else if (instance.extractor != null)
			{
				this.extractor.CopyTo(instance.extractor);
			}
			else
			{
				instance.extractor = this.extractor.Copy();
			}
			instance.staticType = this.staticType;
		}

		public static MiningQuarry Deserialize(Stream stream)
		{
			MiningQuarry miningQuarry = Pool.Get<MiningQuarry>();
			MiningQuarry.Deserialize(stream, miningQuarry, false);
			return miningQuarry;
		}

		public static MiningQuarry Deserialize(byte[] buffer)
		{
			MiningQuarry miningQuarry = Pool.Get<MiningQuarry>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				MiningQuarry.Deserialize(memoryStream, miningQuarry, false);
			}
			return miningQuarry;
		}

		public static MiningQuarry Deserialize(byte[] buffer, MiningQuarry instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				MiningQuarry.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static MiningQuarry Deserialize(Stream stream, MiningQuarry instance, bool isDelta)
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
					if (num == 16)
					{
						instance.staticType = (int)ProtocolParser.ReadUInt64(stream);
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
				else if (instance.extractor != null)
				{
					ResourceExtractor.DeserializeLengthDelimited(stream, instance.extractor, isDelta);
				}
				else
				{
					instance.extractor = ResourceExtractor.DeserializeLengthDelimited(stream);
				}
			}
			return instance;
		}

		public static MiningQuarry DeserializeLength(Stream stream, int length)
		{
			MiningQuarry miningQuarry = Pool.Get<MiningQuarry>();
			MiningQuarry.DeserializeLength(stream, length, miningQuarry, false);
			return miningQuarry;
		}

		public static MiningQuarry DeserializeLength(Stream stream, int length, MiningQuarry instance, bool isDelta)
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
					if (num == 16)
					{
						instance.staticType = (int)ProtocolParser.ReadUInt64(stream);
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
				else if (instance.extractor != null)
				{
					ResourceExtractor.DeserializeLengthDelimited(stream, instance.extractor, isDelta);
				}
				else
				{
					instance.extractor = ResourceExtractor.DeserializeLengthDelimited(stream);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static MiningQuarry DeserializeLengthDelimited(Stream stream)
		{
			MiningQuarry miningQuarry = Pool.Get<MiningQuarry>();
			MiningQuarry.DeserializeLengthDelimited(stream, miningQuarry, false);
			return miningQuarry;
		}

		public static MiningQuarry DeserializeLengthDelimited(Stream stream, MiningQuarry instance, bool isDelta)
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
					if (num == 16)
					{
						instance.staticType = (int)ProtocolParser.ReadUInt64(stream);
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
				else if (instance.extractor != null)
				{
					ResourceExtractor.DeserializeLengthDelimited(stream, instance.extractor, isDelta);
				}
				else
				{
					instance.extractor = ResourceExtractor.DeserializeLengthDelimited(stream);
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
			MiningQuarry.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			MiningQuarry.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(MiningQuarry instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			if (instance.extractor != null)
			{
				instance.extractor.ResetToPool();
				instance.extractor = null;
			}
			instance.staticType = 0;
			Pool.Free<MiningQuarry>(ref instance);
		}

		public void ResetToPool()
		{
			MiningQuarry.ResetToPool(this);
		}

		public static void Serialize(Stream stream, MiningQuarry instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.extractor != null)
			{
				stream.WriteByte(10);
				memoryStream.SetLength((long)0);
				ResourceExtractor.Serialize(memoryStream, instance.extractor);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			stream.WriteByte(16);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.staticType);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, MiningQuarry instance, MiningQuarry previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.extractor != null)
			{
				stream.WriteByte(10);
				memoryStream.SetLength((long)0);
				ResourceExtractor.SerializeDelta(memoryStream, instance.extractor, previous.extractor);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			if (instance.staticType != previous.staticType)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.staticType);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, MiningQuarry instance)
		{
			byte[] bytes = MiningQuarry.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(MiningQuarry instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				MiningQuarry.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			MiningQuarry.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return MiningQuarry.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			MiningQuarry.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, MiningQuarry previous)
		{
			if (previous == null)
			{
				MiningQuarry.Serialize(stream, this);
				return;
			}
			MiningQuarry.SerializeDelta(stream, this, previous);
		}
	}
}