using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class PlayerInventory : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public ItemContainer invMain;

		[NonSerialized]
		public ItemContainer invBelt;

		[NonSerialized]
		public ItemContainer invWear;

		public bool ShouldPool = true;

		private bool _disposed;

		public PlayerInventory()
		{
		}

		public PlayerInventory Copy()
		{
			PlayerInventory playerInventory = Pool.Get<PlayerInventory>();
			this.CopyTo(playerInventory);
			return playerInventory;
		}

		public void CopyTo(PlayerInventory instance)
		{
			if (this.invMain == null)
			{
				instance.invMain = null;
			}
			else if (instance.invMain != null)
			{
				this.invMain.CopyTo(instance.invMain);
			}
			else
			{
				instance.invMain = this.invMain.Copy();
			}
			if (this.invBelt == null)
			{
				instance.invBelt = null;
			}
			else if (instance.invBelt != null)
			{
				this.invBelt.CopyTo(instance.invBelt);
			}
			else
			{
				instance.invBelt = this.invBelt.Copy();
			}
			if (this.invWear == null)
			{
				instance.invWear = null;
				return;
			}
			if (instance.invWear == null)
			{
				instance.invWear = this.invWear.Copy();
				return;
			}
			this.invWear.CopyTo(instance.invWear);
		}

		public static PlayerInventory Deserialize(Stream stream)
		{
			PlayerInventory playerInventory = Pool.Get<PlayerInventory>();
			PlayerInventory.Deserialize(stream, playerInventory, false);
			return playerInventory;
		}

		public static PlayerInventory Deserialize(byte[] buffer)
		{
			PlayerInventory playerInventory = Pool.Get<PlayerInventory>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				PlayerInventory.Deserialize(memoryStream, playerInventory, false);
			}
			return playerInventory;
		}

		public static PlayerInventory Deserialize(byte[] buffer, PlayerInventory instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				PlayerInventory.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static PlayerInventory Deserialize(Stream stream, PlayerInventory instance, bool isDelta)
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
					if (instance.invMain != null)
					{
						ItemContainer.DeserializeLengthDelimited(stream, instance.invMain, isDelta);
					}
					else
					{
						instance.invMain = ItemContainer.DeserializeLengthDelimited(stream);
					}
				}
				else if (num == 18)
				{
					if (instance.invBelt != null)
					{
						ItemContainer.DeserializeLengthDelimited(stream, instance.invBelt, isDelta);
					}
					else
					{
						instance.invBelt = ItemContainer.DeserializeLengthDelimited(stream);
					}
				}
				else if (num != 26)
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
				else if (instance.invWear != null)
				{
					ItemContainer.DeserializeLengthDelimited(stream, instance.invWear, isDelta);
				}
				else
				{
					instance.invWear = ItemContainer.DeserializeLengthDelimited(stream);
				}
			}
			return instance;
		}

		public static PlayerInventory DeserializeLength(Stream stream, int length)
		{
			PlayerInventory playerInventory = Pool.Get<PlayerInventory>();
			PlayerInventory.DeserializeLength(stream, length, playerInventory, false);
			return playerInventory;
		}

		public static PlayerInventory DeserializeLength(Stream stream, int length, PlayerInventory instance, bool isDelta)
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
					if (instance.invMain != null)
					{
						ItemContainer.DeserializeLengthDelimited(stream, instance.invMain, isDelta);
					}
					else
					{
						instance.invMain = ItemContainer.DeserializeLengthDelimited(stream);
					}
				}
				else if (num == 18)
				{
					if (instance.invBelt != null)
					{
						ItemContainer.DeserializeLengthDelimited(stream, instance.invBelt, isDelta);
					}
					else
					{
						instance.invBelt = ItemContainer.DeserializeLengthDelimited(stream);
					}
				}
				else if (num != 26)
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
				else if (instance.invWear != null)
				{
					ItemContainer.DeserializeLengthDelimited(stream, instance.invWear, isDelta);
				}
				else
				{
					instance.invWear = ItemContainer.DeserializeLengthDelimited(stream);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static PlayerInventory DeserializeLengthDelimited(Stream stream)
		{
			PlayerInventory playerInventory = Pool.Get<PlayerInventory>();
			PlayerInventory.DeserializeLengthDelimited(stream, playerInventory, false);
			return playerInventory;
		}

		public static PlayerInventory DeserializeLengthDelimited(Stream stream, PlayerInventory instance, bool isDelta)
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
					if (instance.invMain != null)
					{
						ItemContainer.DeserializeLengthDelimited(stream, instance.invMain, isDelta);
					}
					else
					{
						instance.invMain = ItemContainer.DeserializeLengthDelimited(stream);
					}
				}
				else if (num == 18)
				{
					if (instance.invBelt != null)
					{
						ItemContainer.DeserializeLengthDelimited(stream, instance.invBelt, isDelta);
					}
					else
					{
						instance.invBelt = ItemContainer.DeserializeLengthDelimited(stream);
					}
				}
				else if (num != 26)
				{
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
				else if (instance.invWear != null)
				{
					ItemContainer.DeserializeLengthDelimited(stream, instance.invWear, isDelta);
				}
				else
				{
					instance.invWear = ItemContainer.DeserializeLengthDelimited(stream);
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
			PlayerInventory.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			PlayerInventory.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(PlayerInventory instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			if (instance.invMain != null)
			{
				instance.invMain.ResetToPool();
				instance.invMain = null;
			}
			if (instance.invBelt != null)
			{
				instance.invBelt.ResetToPool();
				instance.invBelt = null;
			}
			if (instance.invWear != null)
			{
				instance.invWear.ResetToPool();
				instance.invWear = null;
			}
			Pool.Free<PlayerInventory>(ref instance);
		}

		public void ResetToPool()
		{
			PlayerInventory.ResetToPool(this);
		}

		public static void Serialize(Stream stream, PlayerInventory instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.invMain != null)
			{
				stream.WriteByte(10);
				memoryStream.SetLength((long)0);
				ItemContainer.Serialize(memoryStream, instance.invMain);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			if (instance.invBelt != null)
			{
				stream.WriteByte(18);
				memoryStream.SetLength((long)0);
				ItemContainer.Serialize(memoryStream, instance.invBelt);
				uint num = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			}
			if (instance.invWear != null)
			{
				stream.WriteByte(26);
				memoryStream.SetLength((long)0);
				ItemContainer.Serialize(memoryStream, instance.invWear);
				uint length1 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length1);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length1);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, PlayerInventory instance, PlayerInventory previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.invMain != null)
			{
				stream.WriteByte(10);
				memoryStream.SetLength((long)0);
				ItemContainer.SerializeDelta(memoryStream, instance.invMain, previous.invMain);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			if (instance.invBelt != null)
			{
				stream.WriteByte(18);
				memoryStream.SetLength((long)0);
				ItemContainer.SerializeDelta(memoryStream, instance.invBelt, previous.invBelt);
				uint num = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			}
			if (instance.invWear != null)
			{
				stream.WriteByte(26);
				memoryStream.SetLength((long)0);
				ItemContainer.SerializeDelta(memoryStream, instance.invWear, previous.invWear);
				uint length1 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length1);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length1);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, PlayerInventory instance)
		{
			byte[] bytes = PlayerInventory.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(PlayerInventory instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				PlayerInventory.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			PlayerInventory.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return PlayerInventory.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			PlayerInventory.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, PlayerInventory previous)
		{
			if (previous == null)
			{
				PlayerInventory.Serialize(stream, this);
				return;
			}
			PlayerInventory.SerializeDelta(stream, this, previous);
		}
	}
}