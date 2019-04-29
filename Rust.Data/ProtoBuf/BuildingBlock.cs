using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class BuildingBlock : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public int model;

		[NonSerialized]
		public int grade;

		[NonSerialized]
		public bool beingDemolished;

		public bool ShouldPool = true;

		private bool _disposed;

		public BuildingBlock()
		{
		}

		public BuildingBlock Copy()
		{
			BuildingBlock buildingBlock = Pool.Get<BuildingBlock>();
			this.CopyTo(buildingBlock);
			return buildingBlock;
		}

		public void CopyTo(BuildingBlock instance)
		{
			instance.model = this.model;
			instance.grade = this.grade;
			instance.beingDemolished = this.beingDemolished;
		}

		public static BuildingBlock Deserialize(Stream stream)
		{
			BuildingBlock buildingBlock = Pool.Get<BuildingBlock>();
			BuildingBlock.Deserialize(stream, buildingBlock, false);
			return buildingBlock;
		}

		public static BuildingBlock Deserialize(byte[] buffer)
		{
			BuildingBlock buildingBlock = Pool.Get<BuildingBlock>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				BuildingBlock.Deserialize(memoryStream, buildingBlock, false);
			}
			return buildingBlock;
		}

		public static BuildingBlock Deserialize(byte[] buffer, BuildingBlock instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				BuildingBlock.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static BuildingBlock Deserialize(Stream stream, BuildingBlock instance, bool isDelta)
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
					instance.model = (int)ProtocolParser.ReadUInt64(stream);
				}
				else if (num == 16)
				{
					instance.grade = (int)ProtocolParser.ReadUInt64(stream);
				}
				else if (num == 24)
				{
					instance.beingDemolished = ProtocolParser.ReadBool(stream);
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

		public static BuildingBlock DeserializeLength(Stream stream, int length)
		{
			BuildingBlock buildingBlock = Pool.Get<BuildingBlock>();
			BuildingBlock.DeserializeLength(stream, length, buildingBlock, false);
			return buildingBlock;
		}

		public static BuildingBlock DeserializeLength(Stream stream, int length, BuildingBlock instance, bool isDelta)
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
					instance.model = (int)ProtocolParser.ReadUInt64(stream);
				}
				else if (num == 16)
				{
					instance.grade = (int)ProtocolParser.ReadUInt64(stream);
				}
				else if (num == 24)
				{
					instance.beingDemolished = ProtocolParser.ReadBool(stream);
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

		public static BuildingBlock DeserializeLengthDelimited(Stream stream)
		{
			BuildingBlock buildingBlock = Pool.Get<BuildingBlock>();
			BuildingBlock.DeserializeLengthDelimited(stream, buildingBlock, false);
			return buildingBlock;
		}

		public static BuildingBlock DeserializeLengthDelimited(Stream stream, BuildingBlock instance, bool isDelta)
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
					instance.model = (int)ProtocolParser.ReadUInt64(stream);
				}
				else if (num == 16)
				{
					instance.grade = (int)ProtocolParser.ReadUInt64(stream);
				}
				else if (num == 24)
				{
					instance.beingDemolished = ProtocolParser.ReadBool(stream);
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
			BuildingBlock.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			BuildingBlock.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(BuildingBlock instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.model = 0;
			instance.grade = 0;
			instance.beingDemolished = false;
			Pool.Free<BuildingBlock>(ref instance);
		}

		public void ResetToPool()
		{
			BuildingBlock.ResetToPool(this);
		}

		public static void Serialize(Stream stream, BuildingBlock instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.model);
			stream.WriteByte(16);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.grade);
			stream.WriteByte(24);
			ProtocolParser.WriteBool(stream, instance.beingDemolished);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, BuildingBlock instance, BuildingBlock previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.model != previous.model)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.model);
			}
			if (instance.grade != previous.grade)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.grade);
			}
			stream.WriteByte(24);
			ProtocolParser.WriteBool(stream, instance.beingDemolished);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, BuildingBlock instance)
		{
			byte[] bytes = BuildingBlock.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(BuildingBlock instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BuildingBlock.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			BuildingBlock.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return BuildingBlock.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			BuildingBlock.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, BuildingBlock previous)
		{
			if (previous == null)
			{
				BuildingBlock.Serialize(stream, this);
				return;
			}
			BuildingBlock.SerializeDelta(stream, this, previous);
		}
	}
}