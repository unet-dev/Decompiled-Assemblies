using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class BasePlayer : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public string name;

		[NonSerialized]
		public ulong userid;

		[NonSerialized]
		public PlayerInventory inventory;

		[NonSerialized]
		public PlayerMetabolism metabolism;

		[NonSerialized]
		public ModelState modelState;

		[NonSerialized]
		public int playerFlags;

		[NonSerialized]
		public uint heldEntity;

		[NonSerialized]
		public float health;

		[NonSerialized]
		public PersistantPlayer persistantData;

		[NonSerialized]
		public float skinCol;

		[NonSerialized]
		public float skinTex;

		[NonSerialized]
		public float skinMesh;

		[NonSerialized]
		public PlayerLifeStory currentLife;

		[NonSerialized]
		public PlayerLifeStory previousLife;

		[NonSerialized]
		public uint mounted;

		[NonSerialized]
		public ulong currentTeam;

		public bool ShouldPool = true;

		private bool _disposed;

		public BasePlayer()
		{
		}

		public BasePlayer Copy()
		{
			BasePlayer basePlayer = Pool.Get<BasePlayer>();
			this.CopyTo(basePlayer);
			return basePlayer;
		}

		public void CopyTo(BasePlayer instance)
		{
			instance.name = this.name;
			instance.userid = this.userid;
			if (this.inventory == null)
			{
				instance.inventory = null;
			}
			else if (instance.inventory != null)
			{
				this.inventory.CopyTo(instance.inventory);
			}
			else
			{
				instance.inventory = this.inventory.Copy();
			}
			if (this.metabolism == null)
			{
				instance.metabolism = null;
			}
			else if (instance.metabolism != null)
			{
				this.metabolism.CopyTo(instance.metabolism);
			}
			else
			{
				instance.metabolism = this.metabolism.Copy();
			}
			if (this.modelState == null)
			{
				instance.modelState = null;
			}
			else if (instance.modelState != null)
			{
				this.modelState.CopyTo(instance.modelState);
			}
			else
			{
				instance.modelState = this.modelState.Copy();
			}
			instance.playerFlags = this.playerFlags;
			instance.heldEntity = this.heldEntity;
			instance.health = this.health;
			if (this.persistantData == null)
			{
				instance.persistantData = null;
			}
			else if (instance.persistantData != null)
			{
				this.persistantData.CopyTo(instance.persistantData);
			}
			else
			{
				instance.persistantData = this.persistantData.Copy();
			}
			instance.skinCol = this.skinCol;
			instance.skinTex = this.skinTex;
			instance.skinMesh = this.skinMesh;
			if (this.currentLife == null)
			{
				instance.currentLife = null;
			}
			else if (instance.currentLife != null)
			{
				this.currentLife.CopyTo(instance.currentLife);
			}
			else
			{
				instance.currentLife = this.currentLife.Copy();
			}
			if (this.previousLife == null)
			{
				instance.previousLife = null;
			}
			else if (instance.previousLife != null)
			{
				this.previousLife.CopyTo(instance.previousLife);
			}
			else
			{
				instance.previousLife = this.previousLife.Copy();
			}
			instance.mounted = this.mounted;
			instance.currentTeam = this.currentTeam;
		}

		public static BasePlayer Deserialize(Stream stream)
		{
			BasePlayer basePlayer = Pool.Get<BasePlayer>();
			BasePlayer.Deserialize(stream, basePlayer, false);
			return basePlayer;
		}

		public static BasePlayer Deserialize(byte[] buffer)
		{
			BasePlayer basePlayer = Pool.Get<BasePlayer>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				BasePlayer.Deserialize(memoryStream, basePlayer, false);
			}
			return basePlayer;
		}

		public static BasePlayer Deserialize(byte[] buffer, BasePlayer instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				BasePlayer.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static BasePlayer Deserialize(Stream stream, BasePlayer instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num <= 50)
				{
					if (num <= 16)
					{
						if (num == 10)
						{
							instance.name = ProtocolParser.ReadString(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.userid = ProtocolParser.ReadUInt64(stream);
							continue;
						}
					}
					else if (num == 26)
					{
						if (instance.inventory != null)
						{
							PlayerInventory.DeserializeLengthDelimited(stream, instance.inventory, isDelta);
							continue;
						}
						else
						{
							instance.inventory = PlayerInventory.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					else if (num != 34)
					{
						if (num == 50)
						{
							if (instance.modelState != null)
							{
								ModelState.DeserializeLengthDelimited(stream, instance.modelState, isDelta);
								continue;
							}
							else
							{
								instance.modelState = ModelState.DeserializeLengthDelimited(stream);
								continue;
							}
						}
					}
					else if (instance.metabolism != null)
					{
						PlayerMetabolism.DeserializeLengthDelimited(stream, instance.metabolism, isDelta);
						continue;
					}
					else
					{
						instance.metabolism = PlayerMetabolism.DeserializeLengthDelimited(stream);
						continue;
					}
				}
				else if (num <= 64)
				{
					if (num == 56)
					{
						instance.playerFlags = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 64)
					{
						instance.heldEntity = ProtocolParser.ReadUInt32(stream);
						continue;
					}
				}
				else if (num == 77)
				{
					instance.health = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num != 82)
				{
					if (num == 125)
					{
						instance.skinCol = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (instance.persistantData != null)
				{
					PersistantPlayer.DeserializeLengthDelimited(stream, instance.persistantData, isDelta);
					continue;
				}
				else
				{
					instance.persistantData = PersistantPlayer.DeserializeLengthDelimited(stream);
					continue;
				}
				Key key = ProtocolParser.ReadKey((byte)num, stream);
				uint field = key.Field;
				if (field == 0)
				{
					throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
				}
				switch (field)
				{
					case 16:
					{
						if (key.WireType != Wire.Fixed32)
						{
							continue;
						}
						instance.skinTex = ProtocolParser.ReadSingle(stream);
						continue;
					}
					case 17:
					{
						if (key.WireType != Wire.Fixed32)
						{
							continue;
						}
						instance.skinMesh = ProtocolParser.ReadSingle(stream);
						continue;
					}
					case 18:
					case 19:
					{
						ProtocolParser.SkipKey(stream, key);
						continue;
					}
					case 20:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.currentLife != null)
						{
							PlayerLifeStory.DeserializeLengthDelimited(stream, instance.currentLife, isDelta);
							continue;
						}
						else
						{
							instance.currentLife = PlayerLifeStory.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 21:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.previousLife != null)
						{
							PlayerLifeStory.DeserializeLengthDelimited(stream, instance.previousLife, isDelta);
							continue;
						}
						else
						{
							instance.previousLife = PlayerLifeStory.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 22:
					{
						if (key.WireType != Wire.Varint)
						{
							continue;
						}
						instance.mounted = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					case 23:
					{
						if (key.WireType != Wire.Varint)
						{
							continue;
						}
						instance.currentTeam = ProtocolParser.ReadUInt64(stream);
						continue;
					}
					default:
					{
						goto case 19;
					}
				}
			}
			return instance;
		}

		public static BasePlayer DeserializeLength(Stream stream, int length)
		{
			BasePlayer basePlayer = Pool.Get<BasePlayer>();
			BasePlayer.DeserializeLength(stream, length, basePlayer, false);
			return basePlayer;
		}

		public static BasePlayer DeserializeLength(Stream stream, int length, BasePlayer instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num <= 50)
				{
					if (num <= 16)
					{
						if (num == 10)
						{
							instance.name = ProtocolParser.ReadString(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.userid = ProtocolParser.ReadUInt64(stream);
							continue;
						}
					}
					else if (num == 26)
					{
						if (instance.inventory != null)
						{
							PlayerInventory.DeserializeLengthDelimited(stream, instance.inventory, isDelta);
							continue;
						}
						else
						{
							instance.inventory = PlayerInventory.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					else if (num != 34)
					{
						if (num == 50)
						{
							if (instance.modelState != null)
							{
								ModelState.DeserializeLengthDelimited(stream, instance.modelState, isDelta);
								continue;
							}
							else
							{
								instance.modelState = ModelState.DeserializeLengthDelimited(stream);
								continue;
							}
						}
					}
					else if (instance.metabolism != null)
					{
						PlayerMetabolism.DeserializeLengthDelimited(stream, instance.metabolism, isDelta);
						continue;
					}
					else
					{
						instance.metabolism = PlayerMetabolism.DeserializeLengthDelimited(stream);
						continue;
					}
				}
				else if (num <= 64)
				{
					if (num == 56)
					{
						instance.playerFlags = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 64)
					{
						instance.heldEntity = ProtocolParser.ReadUInt32(stream);
						continue;
					}
				}
				else if (num == 77)
				{
					instance.health = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num != 82)
				{
					if (num == 125)
					{
						instance.skinCol = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (instance.persistantData != null)
				{
					PersistantPlayer.DeserializeLengthDelimited(stream, instance.persistantData, isDelta);
					continue;
				}
				else
				{
					instance.persistantData = PersistantPlayer.DeserializeLengthDelimited(stream);
					continue;
				}
				Key key = ProtocolParser.ReadKey((byte)num, stream);
				uint field = key.Field;
				if (field == 0)
				{
					throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
				}
				switch (field)
				{
					case 16:
					{
						if (key.WireType != Wire.Fixed32)
						{
							continue;
						}
						instance.skinTex = ProtocolParser.ReadSingle(stream);
						continue;
					}
					case 17:
					{
						if (key.WireType != Wire.Fixed32)
						{
							continue;
						}
						instance.skinMesh = ProtocolParser.ReadSingle(stream);
						continue;
					}
					case 18:
					case 19:
					{
						ProtocolParser.SkipKey(stream, key);
						continue;
					}
					case 20:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.currentLife != null)
						{
							PlayerLifeStory.DeserializeLengthDelimited(stream, instance.currentLife, isDelta);
							continue;
						}
						else
						{
							instance.currentLife = PlayerLifeStory.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 21:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.previousLife != null)
						{
							PlayerLifeStory.DeserializeLengthDelimited(stream, instance.previousLife, isDelta);
							continue;
						}
						else
						{
							instance.previousLife = PlayerLifeStory.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 22:
					{
						if (key.WireType != Wire.Varint)
						{
							continue;
						}
						instance.mounted = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					case 23:
					{
						if (key.WireType != Wire.Varint)
						{
							continue;
						}
						instance.currentTeam = ProtocolParser.ReadUInt64(stream);
						continue;
					}
					default:
					{
						goto case 19;
					}
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static BasePlayer DeserializeLengthDelimited(Stream stream)
		{
			BasePlayer basePlayer = Pool.Get<BasePlayer>();
			BasePlayer.DeserializeLengthDelimited(stream, basePlayer, false);
			return basePlayer;
		}

		public static BasePlayer DeserializeLengthDelimited(Stream stream, BasePlayer instance, bool isDelta)
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
				if (num <= 50)
				{
					if (num <= 16)
					{
						if (num == 10)
						{
							instance.name = ProtocolParser.ReadString(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.userid = ProtocolParser.ReadUInt64(stream);
							continue;
						}
					}
					else if (num == 26)
					{
						if (instance.inventory != null)
						{
							PlayerInventory.DeserializeLengthDelimited(stream, instance.inventory, isDelta);
							continue;
						}
						else
						{
							instance.inventory = PlayerInventory.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					else if (num != 34)
					{
						if (num == 50)
						{
							if (instance.modelState != null)
							{
								ModelState.DeserializeLengthDelimited(stream, instance.modelState, isDelta);
								continue;
							}
							else
							{
								instance.modelState = ModelState.DeserializeLengthDelimited(stream);
								continue;
							}
						}
					}
					else if (instance.metabolism != null)
					{
						PlayerMetabolism.DeserializeLengthDelimited(stream, instance.metabolism, isDelta);
						continue;
					}
					else
					{
						instance.metabolism = PlayerMetabolism.DeserializeLengthDelimited(stream);
						continue;
					}
				}
				else if (num <= 64)
				{
					if (num == 56)
					{
						instance.playerFlags = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 64)
					{
						instance.heldEntity = ProtocolParser.ReadUInt32(stream);
						continue;
					}
				}
				else if (num == 77)
				{
					instance.health = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num != 82)
				{
					if (num == 125)
					{
						instance.skinCol = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (instance.persistantData != null)
				{
					PersistantPlayer.DeserializeLengthDelimited(stream, instance.persistantData, isDelta);
					continue;
				}
				else
				{
					instance.persistantData = PersistantPlayer.DeserializeLengthDelimited(stream);
					continue;
				}
				Key key = ProtocolParser.ReadKey((byte)num, stream);
				uint field = key.Field;
				if (field == 0)
				{
					throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
				}
				switch (field)
				{
					case 16:
					{
						if (key.WireType != Wire.Fixed32)
						{
							continue;
						}
						instance.skinTex = ProtocolParser.ReadSingle(stream);
						continue;
					}
					case 17:
					{
						if (key.WireType != Wire.Fixed32)
						{
							continue;
						}
						instance.skinMesh = ProtocolParser.ReadSingle(stream);
						continue;
					}
					case 18:
					case 19:
					{
						ProtocolParser.SkipKey(stream, key);
						continue;
					}
					case 20:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.currentLife != null)
						{
							PlayerLifeStory.DeserializeLengthDelimited(stream, instance.currentLife, isDelta);
							continue;
						}
						else
						{
							instance.currentLife = PlayerLifeStory.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 21:
					{
						if (key.WireType != Wire.LengthDelimited)
						{
							continue;
						}
						if (instance.previousLife != null)
						{
							PlayerLifeStory.DeserializeLengthDelimited(stream, instance.previousLife, isDelta);
							continue;
						}
						else
						{
							instance.previousLife = PlayerLifeStory.DeserializeLengthDelimited(stream);
							continue;
						}
					}
					case 22:
					{
						if (key.WireType != Wire.Varint)
						{
							continue;
						}
						instance.mounted = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					case 23:
					{
						if (key.WireType != Wire.Varint)
						{
							continue;
						}
						instance.currentTeam = ProtocolParser.ReadUInt64(stream);
						continue;
					}
					default:
					{
						goto case 19;
					}
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
			BasePlayer.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			BasePlayer.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(BasePlayer instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.name = string.Empty;
			instance.userid = (ulong)0;
			if (instance.inventory != null)
			{
				instance.inventory.ResetToPool();
				instance.inventory = null;
			}
			if (instance.metabolism != null)
			{
				instance.metabolism.ResetToPool();
				instance.metabolism = null;
			}
			if (instance.modelState != null)
			{
				instance.modelState.ResetToPool();
				instance.modelState = null;
			}
			instance.playerFlags = 0;
			instance.heldEntity = 0;
			instance.health = 0f;
			if (instance.persistantData != null)
			{
				instance.persistantData.ResetToPool();
				instance.persistantData = null;
			}
			instance.skinCol = 0f;
			instance.skinTex = 0f;
			instance.skinMesh = 0f;
			if (instance.currentLife != null)
			{
				instance.currentLife.ResetToPool();
				instance.currentLife = null;
			}
			if (instance.previousLife != null)
			{
				instance.previousLife.ResetToPool();
				instance.previousLife = null;
			}
			instance.mounted = 0;
			instance.currentTeam = (ulong)0;
			Pool.Free<BasePlayer>(ref instance);
		}

		public void ResetToPool()
		{
			BasePlayer.ResetToPool(this);
		}

		public static void Serialize(Stream stream, BasePlayer instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.name != null)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteString(stream, instance.name);
			}
			stream.WriteByte(16);
			ProtocolParser.WriteUInt64(stream, instance.userid);
			if (instance.inventory != null)
			{
				stream.WriteByte(26);
				memoryStream.SetLength((long)0);
				PlayerInventory.Serialize(memoryStream, instance.inventory);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			if (instance.metabolism != null)
			{
				stream.WriteByte(34);
				memoryStream.SetLength((long)0);
				PlayerMetabolism.Serialize(memoryStream, instance.metabolism);
				uint num = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			}
			if (instance.modelState != null)
			{
				stream.WriteByte(50);
				memoryStream.SetLength((long)0);
				ModelState.Serialize(memoryStream, instance.modelState);
				uint length1 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length1);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length1);
			}
			stream.WriteByte(56);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.playerFlags);
			stream.WriteByte(64);
			ProtocolParser.WriteUInt32(stream, instance.heldEntity);
			stream.WriteByte(77);
			ProtocolParser.WriteSingle(stream, instance.health);
			if (instance.persistantData != null)
			{
				stream.WriteByte(82);
				memoryStream.SetLength((long)0);
				PersistantPlayer.Serialize(memoryStream, instance.persistantData);
				uint num1 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num1);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num1);
			}
			stream.WriteByte(125);
			ProtocolParser.WriteSingle(stream, instance.skinCol);
			stream.WriteByte(133);
			stream.WriteByte(1);
			ProtocolParser.WriteSingle(stream, instance.skinTex);
			stream.WriteByte(141);
			stream.WriteByte(1);
			ProtocolParser.WriteSingle(stream, instance.skinMesh);
			if (instance.currentLife != null)
			{
				stream.WriteByte(162);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				PlayerLifeStory.Serialize(memoryStream, instance.currentLife);
				uint length2 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length2);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length2);
			}
			if (instance.previousLife != null)
			{
				stream.WriteByte(170);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				PlayerLifeStory.Serialize(memoryStream, instance.previousLife);
				uint num2 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num2);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num2);
			}
			stream.WriteByte(176);
			stream.WriteByte(1);
			ProtocolParser.WriteUInt32(stream, instance.mounted);
			stream.WriteByte(184);
			stream.WriteByte(1);
			ProtocolParser.WriteUInt64(stream, instance.currentTeam);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, BasePlayer instance, BasePlayer previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.name != null && instance.name != previous.name)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteString(stream, instance.name);
			}
			if (instance.userid != previous.userid)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt64(stream, instance.userid);
			}
			if (instance.inventory != null)
			{
				stream.WriteByte(26);
				memoryStream.SetLength((long)0);
				PlayerInventory.SerializeDelta(memoryStream, instance.inventory, previous.inventory);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			if (instance.metabolism != null)
			{
				stream.WriteByte(34);
				memoryStream.SetLength((long)0);
				PlayerMetabolism.SerializeDelta(memoryStream, instance.metabolism, previous.metabolism);
				uint num = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			}
			if (instance.modelState != null)
			{
				stream.WriteByte(50);
				memoryStream.SetLength((long)0);
				ModelState.SerializeDelta(memoryStream, instance.modelState, previous.modelState);
				uint length1 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length1);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length1);
			}
			if (instance.playerFlags != previous.playerFlags)
			{
				stream.WriteByte(56);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.playerFlags);
			}
			if (instance.heldEntity != previous.heldEntity)
			{
				stream.WriteByte(64);
				ProtocolParser.WriteUInt32(stream, instance.heldEntity);
			}
			if (instance.health != previous.health)
			{
				stream.WriteByte(77);
				ProtocolParser.WriteSingle(stream, instance.health);
			}
			if (instance.persistantData != null)
			{
				stream.WriteByte(82);
				memoryStream.SetLength((long)0);
				PersistantPlayer.SerializeDelta(memoryStream, instance.persistantData, previous.persistantData);
				uint num1 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num1);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num1);
			}
			if (instance.skinCol != previous.skinCol)
			{
				stream.WriteByte(125);
				ProtocolParser.WriteSingle(stream, instance.skinCol);
			}
			if (instance.skinTex != previous.skinTex)
			{
				stream.WriteByte(133);
				stream.WriteByte(1);
				ProtocolParser.WriteSingle(stream, instance.skinTex);
			}
			if (instance.skinMesh != previous.skinMesh)
			{
				stream.WriteByte(141);
				stream.WriteByte(1);
				ProtocolParser.WriteSingle(stream, instance.skinMesh);
			}
			if (instance.currentLife != null)
			{
				stream.WriteByte(162);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				PlayerLifeStory.SerializeDelta(memoryStream, instance.currentLife, previous.currentLife);
				uint length2 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length2);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length2);
			}
			if (instance.previousLife != null)
			{
				stream.WriteByte(170);
				stream.WriteByte(1);
				memoryStream.SetLength((long)0);
				PlayerLifeStory.SerializeDelta(memoryStream, instance.previousLife, previous.previousLife);
				uint num2 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num2);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num2);
			}
			if (instance.mounted != previous.mounted)
			{
				stream.WriteByte(176);
				stream.WriteByte(1);
				ProtocolParser.WriteUInt32(stream, instance.mounted);
			}
			if (instance.currentTeam != previous.currentTeam)
			{
				stream.WriteByte(184);
				stream.WriteByte(1);
				ProtocolParser.WriteUInt64(stream, instance.currentTeam);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, BasePlayer instance)
		{
			byte[] bytes = BasePlayer.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(BasePlayer instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BasePlayer.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			BasePlayer.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return BasePlayer.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			BasePlayer.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, BasePlayer previous)
		{
			if (previous == null)
			{
				BasePlayer.Serialize(stream, this);
				return;
			}
			BasePlayer.SerializeDelta(stream, this, previous);
		}
	}
}