using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;
using UnityEngine;

namespace ProtoBuf
{
	public class DemoHeader : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public uint version;

		[NonSerialized]
		public string level;

		[NonSerialized]
		public uint levelSeed;

		[NonSerialized]
		public uint levelSize;

		[NonSerialized]
		public string checksum;

		[NonSerialized]
		public ulong localclient;

		[NonSerialized]
		public Vector3 position;

		[NonSerialized]
		public Vector3 rotation;

		[NonSerialized]
		public string levelUrl;

		public bool ShouldPool = true;

		private bool _disposed;

		public DemoHeader()
		{
		}

		public DemoHeader Copy()
		{
			DemoHeader demoHeader = Pool.Get<DemoHeader>();
			this.CopyTo(demoHeader);
			return demoHeader;
		}

		public void CopyTo(DemoHeader instance)
		{
			instance.version = this.version;
			instance.level = this.level;
			instance.levelSeed = this.levelSeed;
			instance.levelSize = this.levelSize;
			instance.checksum = this.checksum;
			instance.localclient = this.localclient;
			instance.position = this.position;
			instance.rotation = this.rotation;
			instance.levelUrl = this.levelUrl;
		}

		public static DemoHeader Deserialize(Stream stream)
		{
			DemoHeader demoHeader = Pool.Get<DemoHeader>();
			DemoHeader.Deserialize(stream, demoHeader, false);
			return demoHeader;
		}

		public static DemoHeader Deserialize(byte[] buffer)
		{
			DemoHeader demoHeader = Pool.Get<DemoHeader>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				DemoHeader.Deserialize(memoryStream, demoHeader, false);
			}
			return demoHeader;
		}

		public static DemoHeader Deserialize(byte[] buffer, DemoHeader instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				DemoHeader.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static DemoHeader Deserialize(Stream stream, DemoHeader instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num <= 32)
				{
					if (num <= 18)
					{
						if (num == 8)
						{
							instance.version = ProtocolParser.ReadUInt32(stream);
							continue;
						}
						else if (num == 18)
						{
							instance.level = ProtocolParser.ReadString(stream);
							continue;
						}
					}
					else if (num == 24)
					{
						instance.levelSeed = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 32)
					{
						instance.levelSize = ProtocolParser.ReadUInt32(stream);
						continue;
					}
				}
				else if (num <= 48)
				{
					if (num == 42)
					{
						instance.checksum = ProtocolParser.ReadString(stream);
						continue;
					}
					else if (num == 48)
					{
						instance.localclient = ProtocolParser.ReadUInt64(stream);
						continue;
					}
				}
				else if (num == 58)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.position, isDelta);
					continue;
				}
				else if (num == 66)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.rotation, isDelta);
					continue;
				}
				else if (num == 74)
				{
					instance.levelUrl = ProtocolParser.ReadString(stream);
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

		public static DemoHeader DeserializeLength(Stream stream, int length)
		{
			DemoHeader demoHeader = Pool.Get<DemoHeader>();
			DemoHeader.DeserializeLength(stream, length, demoHeader, false);
			return demoHeader;
		}

		public static DemoHeader DeserializeLength(Stream stream, int length, DemoHeader instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num <= 32)
				{
					if (num <= 18)
					{
						if (num == 8)
						{
							instance.version = ProtocolParser.ReadUInt32(stream);
							continue;
						}
						else if (num == 18)
						{
							instance.level = ProtocolParser.ReadString(stream);
							continue;
						}
					}
					else if (num == 24)
					{
						instance.levelSeed = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 32)
					{
						instance.levelSize = ProtocolParser.ReadUInt32(stream);
						continue;
					}
				}
				else if (num <= 48)
				{
					if (num == 42)
					{
						instance.checksum = ProtocolParser.ReadString(stream);
						continue;
					}
					else if (num == 48)
					{
						instance.localclient = ProtocolParser.ReadUInt64(stream);
						continue;
					}
				}
				else if (num == 58)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.position, isDelta);
					continue;
				}
				else if (num == 66)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.rotation, isDelta);
					continue;
				}
				else if (num == 74)
				{
					instance.levelUrl = ProtocolParser.ReadString(stream);
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

		public static DemoHeader DeserializeLengthDelimited(Stream stream)
		{
			DemoHeader demoHeader = Pool.Get<DemoHeader>();
			DemoHeader.DeserializeLengthDelimited(stream, demoHeader, false);
			return demoHeader;
		}

		public static DemoHeader DeserializeLengthDelimited(Stream stream, DemoHeader instance, bool isDelta)
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
				if (num <= 32)
				{
					if (num <= 18)
					{
						if (num == 8)
						{
							instance.version = ProtocolParser.ReadUInt32(stream);
							continue;
						}
						else if (num == 18)
						{
							instance.level = ProtocolParser.ReadString(stream);
							continue;
						}
					}
					else if (num == 24)
					{
						instance.levelSeed = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 32)
					{
						instance.levelSize = ProtocolParser.ReadUInt32(stream);
						continue;
					}
				}
				else if (num <= 48)
				{
					if (num == 42)
					{
						instance.checksum = ProtocolParser.ReadString(stream);
						continue;
					}
					else if (num == 48)
					{
						instance.localclient = ProtocolParser.ReadUInt64(stream);
						continue;
					}
				}
				else if (num == 58)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.position, isDelta);
					continue;
				}
				else if (num == 66)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.rotation, isDelta);
					continue;
				}
				else if (num == 74)
				{
					instance.levelUrl = ProtocolParser.ReadString(stream);
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
			DemoHeader.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			DemoHeader.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(DemoHeader instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.version = 0;
			instance.level = string.Empty;
			instance.levelSeed = 0;
			instance.levelSize = 0;
			instance.checksum = string.Empty;
			instance.localclient = (ulong)0;
			instance.position = new Vector3();
			instance.rotation = new Vector3();
			instance.levelUrl = string.Empty;
			Pool.Free<DemoHeader>(ref instance);
		}

		public void ResetToPool()
		{
			DemoHeader.ResetToPool(this);
		}

		public static void Serialize(Stream stream, DemoHeader instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.version);
			if (instance.level == null)
			{
				throw new ArgumentNullException("level", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteString(stream, instance.level);
			stream.WriteByte(24);
			ProtocolParser.WriteUInt32(stream, instance.levelSeed);
			stream.WriteByte(32);
			ProtocolParser.WriteUInt32(stream, instance.levelSize);
			if (instance.checksum != null)
			{
				stream.WriteByte(42);
				ProtocolParser.WriteString(stream, instance.checksum);
			}
			stream.WriteByte(48);
			ProtocolParser.WriteUInt64(stream, instance.localclient);
			stream.WriteByte(58);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.position);
			uint length = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			stream.WriteByte(66);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.rotation);
			uint num = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, num);
			stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			if (instance.levelUrl == null)
			{
				throw new ArgumentNullException("levelUrl", "Required by proto specification.");
			}
			stream.WriteByte(74);
			ProtocolParser.WriteString(stream, instance.levelUrl);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, DemoHeader instance, DemoHeader previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.version != previous.version)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt32(stream, instance.version);
			}
			if (instance.level != previous.level)
			{
				if (instance.level == null)
				{
					throw new ArgumentNullException("level", "Required by proto specification.");
				}
				stream.WriteByte(18);
				ProtocolParser.WriteString(stream, instance.level);
			}
			if (instance.levelSeed != previous.levelSeed)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt32(stream, instance.levelSeed);
			}
			if (instance.levelSize != previous.levelSize)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteUInt32(stream, instance.levelSize);
			}
			if (instance.checksum != null && instance.checksum != previous.checksum)
			{
				stream.WriteByte(42);
				ProtocolParser.WriteString(stream, instance.checksum);
			}
			if (instance.localclient != previous.localclient)
			{
				stream.WriteByte(48);
				ProtocolParser.WriteUInt64(stream, instance.localclient);
			}
			if (instance.position != previous.position)
			{
				stream.WriteByte(58);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.position, previous.position);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			if (instance.rotation != previous.rotation)
			{
				stream.WriteByte(66);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.rotation, previous.rotation);
				uint num = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			}
			if (instance.levelUrl != previous.levelUrl)
			{
				if (instance.levelUrl == null)
				{
					throw new ArgumentNullException("levelUrl", "Required by proto specification.");
				}
				stream.WriteByte(74);
				ProtocolParser.WriteString(stream, instance.levelUrl);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, DemoHeader instance)
		{
			byte[] bytes = DemoHeader.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(DemoHeader instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				DemoHeader.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			DemoHeader.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return DemoHeader.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			DemoHeader.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, DemoHeader previous)
		{
			if (previous == null)
			{
				DemoHeader.Serialize(stream, this);
				return;
			}
			DemoHeader.SerializeDelta(stream, this, previous);
		}
	}
}