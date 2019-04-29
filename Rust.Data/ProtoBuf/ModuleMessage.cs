using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class ModuleMessage : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public uint itemid;

		[NonSerialized]
		public int moduleid;

		[NonSerialized]
		public int type;

		[NonSerialized]
		public byte[] data;

		public bool ShouldPool = true;

		private bool _disposed;

		public ModuleMessage()
		{
		}

		public ModuleMessage Copy()
		{
			ModuleMessage moduleMessage = Pool.Get<ModuleMessage>();
			this.CopyTo(moduleMessage);
			return moduleMessage;
		}

		public void CopyTo(ModuleMessage instance)
		{
			instance.itemid = this.itemid;
			instance.moduleid = this.moduleid;
			instance.type = this.type;
			if (this.data == null)
			{
				instance.data = null;
				return;
			}
			instance.data = new byte[(int)this.data.Length];
			Array.Copy(this.data, instance.data, (int)instance.data.Length);
		}

		public static ModuleMessage Deserialize(Stream stream)
		{
			ModuleMessage moduleMessage = Pool.Get<ModuleMessage>();
			ModuleMessage.Deserialize(stream, moduleMessage, false);
			return moduleMessage;
		}

		public static ModuleMessage Deserialize(byte[] buffer)
		{
			ModuleMessage moduleMessage = Pool.Get<ModuleMessage>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				ModuleMessage.Deserialize(memoryStream, moduleMessage, false);
			}
			return moduleMessage;
		}

		public static ModuleMessage Deserialize(byte[] buffer, ModuleMessage instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				ModuleMessage.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static ModuleMessage Deserialize(Stream stream, ModuleMessage instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num <= 16)
				{
					if (num == 8)
					{
						instance.itemid = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 16)
					{
						instance.moduleid = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
				}
				else if (num == 24)
				{
					instance.type = (int)ProtocolParser.ReadUInt64(stream);
					continue;
				}
				else if (num == 34)
				{
					instance.data = ProtocolParser.ReadBytes(stream);
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

		public static ModuleMessage DeserializeLength(Stream stream, int length)
		{
			ModuleMessage moduleMessage = Pool.Get<ModuleMessage>();
			ModuleMessage.DeserializeLength(stream, length, moduleMessage, false);
			return moduleMessage;
		}

		public static ModuleMessage DeserializeLength(Stream stream, int length, ModuleMessage instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num <= 16)
				{
					if (num == 8)
					{
						instance.itemid = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 16)
					{
						instance.moduleid = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
				}
				else if (num == 24)
				{
					instance.type = (int)ProtocolParser.ReadUInt64(stream);
					continue;
				}
				else if (num == 34)
				{
					instance.data = ProtocolParser.ReadBytes(stream);
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

		public static ModuleMessage DeserializeLengthDelimited(Stream stream)
		{
			ModuleMessage moduleMessage = Pool.Get<ModuleMessage>();
			ModuleMessage.DeserializeLengthDelimited(stream, moduleMessage, false);
			return moduleMessage;
		}

		public static ModuleMessage DeserializeLengthDelimited(Stream stream, ModuleMessage instance, bool isDelta)
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
				if (num <= 16)
				{
					if (num == 8)
					{
						instance.itemid = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 16)
					{
						instance.moduleid = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
				}
				else if (num == 24)
				{
					instance.type = (int)ProtocolParser.ReadUInt64(stream);
					continue;
				}
				else if (num == 34)
				{
					instance.data = ProtocolParser.ReadBytes(stream);
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
			ModuleMessage.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			ModuleMessage.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(ModuleMessage instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.itemid = 0;
			instance.moduleid = 0;
			instance.type = 0;
			instance.data = null;
			Pool.Free<ModuleMessage>(ref instance);
		}

		public void ResetToPool()
		{
			ModuleMessage.ResetToPool(this);
		}

		public static void Serialize(Stream stream, ModuleMessage instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.itemid);
			stream.WriteByte(16);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.moduleid);
			stream.WriteByte(24);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.type);
			if (instance.data != null)
			{
				stream.WriteByte(34);
				ProtocolParser.WriteBytes(stream, instance.data);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, ModuleMessage instance, ModuleMessage previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.itemid != previous.itemid)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt32(stream, instance.itemid);
			}
			if (instance.moduleid != previous.moduleid)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.moduleid);
			}
			if (instance.type != previous.type)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.type);
			}
			if (instance.data != null)
			{
				stream.WriteByte(34);
				ProtocolParser.WriteBytes(stream, instance.data);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, ModuleMessage instance)
		{
			byte[] bytes = ModuleMessage.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(ModuleMessage instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				ModuleMessage.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			ModuleMessage.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return ModuleMessage.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			ModuleMessage.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, ModuleMessage previous)
		{
			if (previous == null)
			{
				ModuleMessage.Serialize(stream, this);
				return;
			}
			ModuleMessage.SerializeDelta(stream, this, previous);
		}
	}
}