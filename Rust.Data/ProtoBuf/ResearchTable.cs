using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class ResearchTable : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public float researchTimeLeft;

		public bool ShouldPool = true;

		private bool _disposed;

		public ResearchTable()
		{
		}

		public ResearchTable Copy()
		{
			ResearchTable researchTable = Pool.Get<ResearchTable>();
			this.CopyTo(researchTable);
			return researchTable;
		}

		public void CopyTo(ResearchTable instance)
		{
			instance.researchTimeLeft = this.researchTimeLeft;
		}

		public static ResearchTable Deserialize(Stream stream)
		{
			ResearchTable researchTable = Pool.Get<ResearchTable>();
			ResearchTable.Deserialize(stream, researchTable, false);
			return researchTable;
		}

		public static ResearchTable Deserialize(byte[] buffer)
		{
			ResearchTable researchTable = Pool.Get<ResearchTable>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				ResearchTable.Deserialize(memoryStream, researchTable, false);
			}
			return researchTable;
		}

		public static ResearchTable Deserialize(byte[] buffer, ResearchTable instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				ResearchTable.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static ResearchTable Deserialize(Stream stream, ResearchTable instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num != 13)
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
					instance.researchTimeLeft = ProtocolParser.ReadSingle(stream);
				}
			}
			return instance;
		}

		public static ResearchTable DeserializeLength(Stream stream, int length)
		{
			ResearchTable researchTable = Pool.Get<ResearchTable>();
			ResearchTable.DeserializeLength(stream, length, researchTable, false);
			return researchTable;
		}

		public static ResearchTable DeserializeLength(Stream stream, int length, ResearchTable instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num != 13)
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
					instance.researchTimeLeft = ProtocolParser.ReadSingle(stream);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static ResearchTable DeserializeLengthDelimited(Stream stream)
		{
			ResearchTable researchTable = Pool.Get<ResearchTable>();
			ResearchTable.DeserializeLengthDelimited(stream, researchTable, false);
			return researchTable;
		}

		public static ResearchTable DeserializeLengthDelimited(Stream stream, ResearchTable instance, bool isDelta)
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
				if (num != 13)
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
					instance.researchTimeLeft = ProtocolParser.ReadSingle(stream);
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
			ResearchTable.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			ResearchTable.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(ResearchTable instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.researchTimeLeft = 0f;
			Pool.Free<ResearchTable>(ref instance);
		}

		public void ResetToPool()
		{
			ResearchTable.ResetToPool(this);
		}

		public static void Serialize(Stream stream, ResearchTable instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(13);
			ProtocolParser.WriteSingle(stream, instance.researchTimeLeft);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, ResearchTable instance, ResearchTable previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.researchTimeLeft != previous.researchTimeLeft)
			{
				stream.WriteByte(13);
				ProtocolParser.WriteSingle(stream, instance.researchTimeLeft);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, ResearchTable instance)
		{
			byte[] bytes = ResearchTable.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(ResearchTable instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				ResearchTable.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			ResearchTable.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return ResearchTable.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			ResearchTable.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, ResearchTable previous)
		{
			if (previous == null)
			{
				ResearchTable.Serialize(stream, this);
				return;
			}
			ResearchTable.SerializeDelta(stream, this, previous);
		}
	}
}