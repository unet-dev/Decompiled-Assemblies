using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class Approval : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public string level;

		[NonSerialized]
		public string hostname;

		[NonSerialized]
		public bool modded;

		[NonSerialized]
		public bool official;

		[NonSerialized]
		public ulong steamid;

		[NonSerialized]
		public uint ipaddress;

		[NonSerialized]
		public int port;

		[NonSerialized]
		public uint levelSeed;

		[NonSerialized]
		public uint levelSize;

		[NonSerialized]
		public string checksum;

		[NonSerialized]
		public uint encryption;

		[NonSerialized]
		public string levelUrl;

		public bool ShouldPool = true;

		private bool _disposed;

		public Approval()
		{
		}

		public Approval Copy()
		{
			Approval approval = Pool.Get<Approval>();
			this.CopyTo(approval);
			return approval;
		}

		public void CopyTo(Approval instance)
		{
			instance.level = this.level;
			instance.hostname = this.hostname;
			instance.modded = this.modded;
			instance.official = this.official;
			instance.steamid = this.steamid;
			instance.ipaddress = this.ipaddress;
			instance.port = this.port;
			instance.levelSeed = this.levelSeed;
			instance.levelSize = this.levelSize;
			instance.checksum = this.checksum;
			instance.encryption = this.encryption;
			instance.levelUrl = this.levelUrl;
		}

		public static Approval Deserialize(Stream stream)
		{
			Approval approval = Pool.Get<Approval>();
			Approval.Deserialize(stream, approval, false);
			return approval;
		}

		public static Approval Deserialize(byte[] buffer)
		{
			Approval approval = Pool.Get<Approval>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				Approval.Deserialize(memoryStream, approval, false);
			}
			return approval;
		}

		public static Approval Deserialize(byte[] buffer, Approval instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				Approval.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static Approval Deserialize(Stream stream, Approval instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num <= 56)
				{
					if (num <= 32)
					{
						if (num == 18)
						{
							instance.level = ProtocolParser.ReadString(stream);
							continue;
						}
						else if (num == 26)
						{
							instance.hostname = ProtocolParser.ReadString(stream);
							continue;
						}
						else if (num == 32)
						{
							instance.modded = ProtocolParser.ReadBool(stream);
							continue;
						}
					}
					else if (num == 40)
					{
						instance.official = ProtocolParser.ReadBool(stream);
						continue;
					}
					else if (num == 48)
					{
						instance.steamid = ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 56)
					{
						instance.ipaddress = ProtocolParser.ReadUInt32(stream);
						continue;
					}
				}
				else if (num <= 80)
				{
					if (num == 64)
					{
						instance.port = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 72)
					{
						instance.levelSeed = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 80)
					{
						instance.levelSize = ProtocolParser.ReadUInt32(stream);
						continue;
					}
				}
				else if (num == 90)
				{
					instance.checksum = ProtocolParser.ReadString(stream);
					continue;
				}
				else if (num == 96)
				{
					instance.encryption = ProtocolParser.ReadUInt32(stream);
					continue;
				}
				else if (num == 106)
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

		public static Approval DeserializeLength(Stream stream, int length)
		{
			Approval approval = Pool.Get<Approval>();
			Approval.DeserializeLength(stream, length, approval, false);
			return approval;
		}

		public static Approval DeserializeLength(Stream stream, int length, Approval instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num <= 56)
				{
					if (num <= 32)
					{
						if (num == 18)
						{
							instance.level = ProtocolParser.ReadString(stream);
							continue;
						}
						else if (num == 26)
						{
							instance.hostname = ProtocolParser.ReadString(stream);
							continue;
						}
						else if (num == 32)
						{
							instance.modded = ProtocolParser.ReadBool(stream);
							continue;
						}
					}
					else if (num == 40)
					{
						instance.official = ProtocolParser.ReadBool(stream);
						continue;
					}
					else if (num == 48)
					{
						instance.steamid = ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 56)
					{
						instance.ipaddress = ProtocolParser.ReadUInt32(stream);
						continue;
					}
				}
				else if (num <= 80)
				{
					if (num == 64)
					{
						instance.port = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 72)
					{
						instance.levelSeed = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 80)
					{
						instance.levelSize = ProtocolParser.ReadUInt32(stream);
						continue;
					}
				}
				else if (num == 90)
				{
					instance.checksum = ProtocolParser.ReadString(stream);
					continue;
				}
				else if (num == 96)
				{
					instance.encryption = ProtocolParser.ReadUInt32(stream);
					continue;
				}
				else if (num == 106)
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

		public static Approval DeserializeLengthDelimited(Stream stream)
		{
			Approval approval = Pool.Get<Approval>();
			Approval.DeserializeLengthDelimited(stream, approval, false);
			return approval;
		}

		public static Approval DeserializeLengthDelimited(Stream stream, Approval instance, bool isDelta)
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
				if (num <= 56)
				{
					if (num <= 32)
					{
						if (num == 18)
						{
							instance.level = ProtocolParser.ReadString(stream);
							continue;
						}
						else if (num == 26)
						{
							instance.hostname = ProtocolParser.ReadString(stream);
							continue;
						}
						else if (num == 32)
						{
							instance.modded = ProtocolParser.ReadBool(stream);
							continue;
						}
					}
					else if (num == 40)
					{
						instance.official = ProtocolParser.ReadBool(stream);
						continue;
					}
					else if (num == 48)
					{
						instance.steamid = ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 56)
					{
						instance.ipaddress = ProtocolParser.ReadUInt32(stream);
						continue;
					}
				}
				else if (num <= 80)
				{
					if (num == 64)
					{
						instance.port = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 72)
					{
						instance.levelSeed = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 80)
					{
						instance.levelSize = ProtocolParser.ReadUInt32(stream);
						continue;
					}
				}
				else if (num == 90)
				{
					instance.checksum = ProtocolParser.ReadString(stream);
					continue;
				}
				else if (num == 96)
				{
					instance.encryption = ProtocolParser.ReadUInt32(stream);
					continue;
				}
				else if (num == 106)
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
			Approval.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			Approval.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(Approval instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.level = string.Empty;
			instance.hostname = string.Empty;
			instance.modded = false;
			instance.official = false;
			instance.steamid = (ulong)0;
			instance.ipaddress = 0;
			instance.port = 0;
			instance.levelSeed = 0;
			instance.levelSize = 0;
			instance.checksum = string.Empty;
			instance.encryption = 0;
			instance.levelUrl = string.Empty;
			Pool.Free<Approval>(ref instance);
		}

		public void ResetToPool()
		{
			Approval.ResetToPool(this);
		}

		public static void Serialize(Stream stream, Approval instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.level == null)
			{
				throw new ArgumentNullException("level", "Required by proto specification.");
			}
			stream.WriteByte(18);
			ProtocolParser.WriteString(stream, instance.level);
			if (instance.hostname != null)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteString(stream, instance.hostname);
			}
			stream.WriteByte(32);
			ProtocolParser.WriteBool(stream, instance.modded);
			stream.WriteByte(40);
			ProtocolParser.WriteBool(stream, instance.official);
			stream.WriteByte(48);
			ProtocolParser.WriteUInt64(stream, instance.steamid);
			stream.WriteByte(56);
			ProtocolParser.WriteUInt32(stream, instance.ipaddress);
			stream.WriteByte(64);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.port);
			stream.WriteByte(72);
			ProtocolParser.WriteUInt32(stream, instance.levelSeed);
			stream.WriteByte(80);
			ProtocolParser.WriteUInt32(stream, instance.levelSize);
			if (instance.checksum != null)
			{
				stream.WriteByte(90);
				ProtocolParser.WriteString(stream, instance.checksum);
			}
			stream.WriteByte(96);
			ProtocolParser.WriteUInt32(stream, instance.encryption);
			if (instance.levelUrl != null)
			{
				stream.WriteByte(106);
				ProtocolParser.WriteString(stream, instance.levelUrl);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, Approval instance, Approval previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.level != previous.level)
			{
				if (instance.level == null)
				{
					throw new ArgumentNullException("level", "Required by proto specification.");
				}
				stream.WriteByte(18);
				ProtocolParser.WriteString(stream, instance.level);
			}
			if (instance.hostname != null && instance.hostname != previous.hostname)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteString(stream, instance.hostname);
			}
			stream.WriteByte(32);
			ProtocolParser.WriteBool(stream, instance.modded);
			stream.WriteByte(40);
			ProtocolParser.WriteBool(stream, instance.official);
			if (instance.steamid != previous.steamid)
			{
				stream.WriteByte(48);
				ProtocolParser.WriteUInt64(stream, instance.steamid);
			}
			if (instance.ipaddress != previous.ipaddress)
			{
				stream.WriteByte(56);
				ProtocolParser.WriteUInt32(stream, instance.ipaddress);
			}
			if (instance.port != previous.port)
			{
				stream.WriteByte(64);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.port);
			}
			if (instance.levelSeed != previous.levelSeed)
			{
				stream.WriteByte(72);
				ProtocolParser.WriteUInt32(stream, instance.levelSeed);
			}
			if (instance.levelSize != previous.levelSize)
			{
				stream.WriteByte(80);
				ProtocolParser.WriteUInt32(stream, instance.levelSize);
			}
			if (instance.checksum != null && instance.checksum != previous.checksum)
			{
				stream.WriteByte(90);
				ProtocolParser.WriteString(stream, instance.checksum);
			}
			if (instance.encryption != previous.encryption)
			{
				stream.WriteByte(96);
				ProtocolParser.WriteUInt32(stream, instance.encryption);
			}
			if (instance.levelUrl != null && instance.levelUrl != previous.levelUrl)
			{
				stream.WriteByte(106);
				ProtocolParser.WriteString(stream, instance.levelUrl);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, Approval instance)
		{
			byte[] bytes = Approval.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(Approval instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Approval.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			Approval.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return Approval.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			Approval.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, Approval previous)
		{
			if (previous == null)
			{
				Approval.Serialize(stream, this);
				return;
			}
			Approval.SerializeDelta(stream, this, previous);
		}
	}
}