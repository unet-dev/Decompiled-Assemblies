using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ProtoBuf
{
	public class ArcadeGame : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public List<ArcadeGame.arcadeEnt> arcadeEnts;

		public bool ShouldPool = true;

		private bool _disposed;

		public ArcadeGame()
		{
		}

		public ArcadeGame Copy()
		{
			ArcadeGame arcadeGame = Pool.Get<ArcadeGame>();
			this.CopyTo(arcadeGame);
			return arcadeGame;
		}

		public void CopyTo(ArcadeGame instance)
		{
			throw new NotImplementedException();
		}

		public static ArcadeGame Deserialize(Stream stream)
		{
			ArcadeGame arcadeGame = Pool.Get<ArcadeGame>();
			ArcadeGame.Deserialize(stream, arcadeGame, false);
			return arcadeGame;
		}

		public static ArcadeGame Deserialize(byte[] buffer)
		{
			ArcadeGame arcadeGame = Pool.Get<ArcadeGame>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				ArcadeGame.Deserialize(memoryStream, arcadeGame, false);
			}
			return arcadeGame;
		}

		public static ArcadeGame Deserialize(byte[] buffer, ArcadeGame instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				ArcadeGame.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static ArcadeGame Deserialize(Stream stream, ArcadeGame instance, bool isDelta)
		{
			if (!isDelta && instance.arcadeEnts == null)
			{
				instance.arcadeEnts = Pool.Get<List<ArcadeGame.arcadeEnt>>();
			}
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num != 10)
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
					instance.arcadeEnts.Add(ArcadeGame.arcadeEnt.DeserializeLengthDelimited(stream));
				}
			}
			return instance;
		}

		public static ArcadeGame DeserializeLength(Stream stream, int length)
		{
			ArcadeGame arcadeGame = Pool.Get<ArcadeGame>();
			ArcadeGame.DeserializeLength(stream, length, arcadeGame, false);
			return arcadeGame;
		}

		public static ArcadeGame DeserializeLength(Stream stream, int length, ArcadeGame instance, bool isDelta)
		{
			if (!isDelta && instance.arcadeEnts == null)
			{
				instance.arcadeEnts = Pool.Get<List<ArcadeGame.arcadeEnt>>();
			}
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
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
				else
				{
					instance.arcadeEnts.Add(ArcadeGame.arcadeEnt.DeserializeLengthDelimited(stream));
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static ArcadeGame DeserializeLengthDelimited(Stream stream)
		{
			ArcadeGame arcadeGame = Pool.Get<ArcadeGame>();
			ArcadeGame.DeserializeLengthDelimited(stream, arcadeGame, false);
			return arcadeGame;
		}

		public static ArcadeGame DeserializeLengthDelimited(Stream stream, ArcadeGame instance, bool isDelta)
		{
			if (!isDelta && instance.arcadeEnts == null)
			{
				instance.arcadeEnts = Pool.Get<List<ArcadeGame.arcadeEnt>>();
			}
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
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					if (key.Field == 0)
					{
						throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
					}
					ProtocolParser.SkipKey(stream, key);
				}
				else
				{
					instance.arcadeEnts.Add(ArcadeGame.arcadeEnt.DeserializeLengthDelimited(stream));
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
			ArcadeGame.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			ArcadeGame.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(ArcadeGame instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			if (instance.arcadeEnts != null)
			{
				for (int i = 0; i < instance.arcadeEnts.Count; i++)
				{
					if (instance.arcadeEnts[i] != null)
					{
						instance.arcadeEnts[i].ResetToPool();
						instance.arcadeEnts[i] = null;
					}
				}
				List<ArcadeGame.arcadeEnt> arcadeEnts = instance.arcadeEnts;
				Pool.FreeList<ArcadeGame.arcadeEnt>(ref arcadeEnts);
				instance.arcadeEnts = arcadeEnts;
			}
			Pool.Free<ArcadeGame>(ref instance);
		}

		public void ResetToPool()
		{
			ArcadeGame.ResetToPool(this);
		}

		public static void Serialize(Stream stream, ArcadeGame instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.arcadeEnts != null)
			{
				for (int i = 0; i < instance.arcadeEnts.Count; i++)
				{
					ArcadeGame.arcadeEnt item = instance.arcadeEnts[i];
					stream.WriteByte(10);
					memoryStream.SetLength((long)0);
					ArcadeGame.arcadeEnt.Serialize(memoryStream, item);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, ArcadeGame instance, ArcadeGame previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.arcadeEnts != null)
			{
				for (int i = 0; i < instance.arcadeEnts.Count; i++)
				{
					ArcadeGame.arcadeEnt item = instance.arcadeEnts[i];
					stream.WriteByte(10);
					memoryStream.SetLength((long)0);
					ArcadeGame.arcadeEnt.SerializeDelta(memoryStream, item, item);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, ArcadeGame instance)
		{
			byte[] bytes = ArcadeGame.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(ArcadeGame instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				ArcadeGame.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			ArcadeGame.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return ArcadeGame.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			ArcadeGame.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, ArcadeGame previous)
		{
			if (previous == null)
			{
				ArcadeGame.Serialize(stream, this);
				return;
			}
			ArcadeGame.SerializeDelta(stream, this, previous);
		}

		public class arcadeEnt : IDisposable, Pool.IPooled, IProto
		{
			[NonSerialized]
			public uint id;

			[NonSerialized]
			public string name;

			[NonSerialized]
			public uint spriteID;

			[NonSerialized]
			public uint soundID;

			[NonSerialized]
			public bool visible;

			[NonSerialized]
			public Vector3 position;

			[NonSerialized]
			public Vector3 heading;

			[NonSerialized]
			public bool enabled;

			[NonSerialized]
			public Vector3 scale;

			[NonSerialized]
			public Vector3 colliderScale;

			[NonSerialized]
			public float alpha;

			[NonSerialized]
			public uint prefabID;

			[NonSerialized]
			public uint parentID;

			public bool ShouldPool;

			private bool _disposed;

			public arcadeEnt()
			{
			}

			public ArcadeGame.arcadeEnt Copy()
			{
				ArcadeGame.arcadeEnt _arcadeEnt = Pool.Get<ArcadeGame.arcadeEnt>();
				this.CopyTo(_arcadeEnt);
				return _arcadeEnt;
			}

			public void CopyTo(ArcadeGame.arcadeEnt instance)
			{
				instance.id = this.id;
				instance.name = this.name;
				instance.spriteID = this.spriteID;
				instance.soundID = this.soundID;
				instance.visible = this.visible;
				instance.position = this.position;
				instance.heading = this.heading;
				instance.enabled = this.enabled;
				instance.scale = this.scale;
				instance.colliderScale = this.colliderScale;
				instance.alpha = this.alpha;
				instance.prefabID = this.prefabID;
				instance.parentID = this.parentID;
			}

			public static ArcadeGame.arcadeEnt Deserialize(Stream stream)
			{
				ArcadeGame.arcadeEnt _arcadeEnt = Pool.Get<ArcadeGame.arcadeEnt>();
				ArcadeGame.arcadeEnt.Deserialize(stream, _arcadeEnt, false);
				return _arcadeEnt;
			}

			public static ArcadeGame.arcadeEnt Deserialize(byte[] buffer)
			{
				ArcadeGame.arcadeEnt _arcadeEnt = Pool.Get<ArcadeGame.arcadeEnt>();
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					ArcadeGame.arcadeEnt.Deserialize(memoryStream, _arcadeEnt, false);
				}
				return _arcadeEnt;
			}

			public static ArcadeGame.arcadeEnt Deserialize(byte[] buffer, ArcadeGame.arcadeEnt instance, bool isDelta = false)
			{
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					ArcadeGame.arcadeEnt.Deserialize(memoryStream, instance, isDelta);
				}
				return instance;
			}

			public static ArcadeGame.arcadeEnt Deserialize(Stream stream, ArcadeGame.arcadeEnt instance, bool isDelta)
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
						if (num <= 24)
						{
							if (num == 8)
							{
								instance.id = ProtocolParser.ReadUInt32(stream);
								continue;
							}
							else if (num == 18)
							{
								instance.name = ProtocolParser.ReadString(stream);
								continue;
							}
							else if (num == 24)
							{
								instance.spriteID = ProtocolParser.ReadUInt32(stream);
								continue;
							}
						}
						else if (num == 32)
						{
							instance.soundID = ProtocolParser.ReadUInt32(stream);
							continue;
						}
						else if (num == 40)
						{
							instance.visible = ProtocolParser.ReadBool(stream);
							continue;
						}
						else if (num == 50)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.position, isDelta);
							continue;
						}
					}
					else if (num <= 74)
					{
						if (num == 58)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.heading, isDelta);
							continue;
						}
						else if (num == 64)
						{
							instance.enabled = ProtocolParser.ReadBool(stream);
							continue;
						}
						else if (num == 74)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.scale, isDelta);
							continue;
						}
					}
					else if (num <= 93)
					{
						if (num == 82)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.colliderScale, isDelta);
							continue;
						}
						else if (num == 93)
						{
							instance.alpha = ProtocolParser.ReadSingle(stream);
							continue;
						}
					}
					else if (num == 96)
					{
						instance.prefabID = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 104)
					{
						instance.parentID = ProtocolParser.ReadUInt32(stream);
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

			public static ArcadeGame.arcadeEnt DeserializeLength(Stream stream, int length)
			{
				ArcadeGame.arcadeEnt _arcadeEnt = Pool.Get<ArcadeGame.arcadeEnt>();
				ArcadeGame.arcadeEnt.DeserializeLength(stream, length, _arcadeEnt, false);
				return _arcadeEnt;
			}

			public static ArcadeGame.arcadeEnt DeserializeLength(Stream stream, int length, ArcadeGame.arcadeEnt instance, bool isDelta)
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
						if (num <= 24)
						{
							if (num == 8)
							{
								instance.id = ProtocolParser.ReadUInt32(stream);
								continue;
							}
							else if (num == 18)
							{
								instance.name = ProtocolParser.ReadString(stream);
								continue;
							}
							else if (num == 24)
							{
								instance.spriteID = ProtocolParser.ReadUInt32(stream);
								continue;
							}
						}
						else if (num == 32)
						{
							instance.soundID = ProtocolParser.ReadUInt32(stream);
							continue;
						}
						else if (num == 40)
						{
							instance.visible = ProtocolParser.ReadBool(stream);
							continue;
						}
						else if (num == 50)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.position, isDelta);
							continue;
						}
					}
					else if (num <= 74)
					{
						if (num == 58)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.heading, isDelta);
							continue;
						}
						else if (num == 64)
						{
							instance.enabled = ProtocolParser.ReadBool(stream);
							continue;
						}
						else if (num == 74)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.scale, isDelta);
							continue;
						}
					}
					else if (num <= 93)
					{
						if (num == 82)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.colliderScale, isDelta);
							continue;
						}
						else if (num == 93)
						{
							instance.alpha = ProtocolParser.ReadSingle(stream);
							continue;
						}
					}
					else if (num == 96)
					{
						instance.prefabID = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 104)
					{
						instance.parentID = ProtocolParser.ReadUInt32(stream);
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

			public static ArcadeGame.arcadeEnt DeserializeLengthDelimited(Stream stream)
			{
				ArcadeGame.arcadeEnt _arcadeEnt = Pool.Get<ArcadeGame.arcadeEnt>();
				ArcadeGame.arcadeEnt.DeserializeLengthDelimited(stream, _arcadeEnt, false);
				return _arcadeEnt;
			}

			public static ArcadeGame.arcadeEnt DeserializeLengthDelimited(Stream stream, ArcadeGame.arcadeEnt instance, bool isDelta)
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
						if (num <= 24)
						{
							if (num == 8)
							{
								instance.id = ProtocolParser.ReadUInt32(stream);
								continue;
							}
							else if (num == 18)
							{
								instance.name = ProtocolParser.ReadString(stream);
								continue;
							}
							else if (num == 24)
							{
								instance.spriteID = ProtocolParser.ReadUInt32(stream);
								continue;
							}
						}
						else if (num == 32)
						{
							instance.soundID = ProtocolParser.ReadUInt32(stream);
							continue;
						}
						else if (num == 40)
						{
							instance.visible = ProtocolParser.ReadBool(stream);
							continue;
						}
						else if (num == 50)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.position, isDelta);
							continue;
						}
					}
					else if (num <= 74)
					{
						if (num == 58)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.heading, isDelta);
							continue;
						}
						else if (num == 64)
						{
							instance.enabled = ProtocolParser.ReadBool(stream);
							continue;
						}
						else if (num == 74)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.scale, isDelta);
							continue;
						}
					}
					else if (num <= 93)
					{
						if (num == 82)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.colliderScale, isDelta);
							continue;
						}
						else if (num == 93)
						{
							instance.alpha = ProtocolParser.ReadSingle(stream);
							continue;
						}
					}
					else if (num == 96)
					{
						instance.prefabID = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 104)
					{
						instance.parentID = ProtocolParser.ReadUInt32(stream);
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
				ArcadeGame.arcadeEnt.Deserialize(stream, this, isDelta);
			}

			public virtual void LeavePool()
			{
				this._disposed = false;
			}

			public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
			{
				ArcadeGame.arcadeEnt.DeserializeLength(stream, size, this, isDelta);
			}

			public static void ResetToPool(ArcadeGame.arcadeEnt instance)
			{
				if (!instance.ShouldPool)
				{
					return;
				}
				instance.id = 0;
				instance.name = string.Empty;
				instance.spriteID = 0;
				instance.soundID = 0;
				instance.visible = false;
				instance.position = new Vector3();
				instance.heading = new Vector3();
				instance.enabled = false;
				instance.scale = new Vector3();
				instance.colliderScale = new Vector3();
				instance.alpha = 0f;
				instance.prefabID = 0;
				instance.parentID = 0;
				Pool.Free<ArcadeGame.arcadeEnt>(ref instance);
			}

			public void ResetToPool()
			{
				ArcadeGame.arcadeEnt.ResetToPool(this);
			}

			public static void Serialize(Stream stream, ArcadeGame.arcadeEnt instance)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				stream.WriteByte(8);
				ProtocolParser.WriteUInt32(stream, instance.id);
				if (instance.name == null)
				{
					throw new ArgumentNullException("name", "Required by proto specification.");
				}
				stream.WriteByte(18);
				ProtocolParser.WriteString(stream, instance.name);
				stream.WriteByte(24);
				ProtocolParser.WriteUInt32(stream, instance.spriteID);
				stream.WriteByte(32);
				ProtocolParser.WriteUInt32(stream, instance.soundID);
				stream.WriteByte(40);
				ProtocolParser.WriteBool(stream, instance.visible);
				stream.WriteByte(50);
				memoryStream.SetLength((long)0);
				Vector3Serialized.Serialize(memoryStream, instance.position);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				stream.WriteByte(58);
				memoryStream.SetLength((long)0);
				Vector3Serialized.Serialize(memoryStream, instance.heading);
				uint num = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num);
				stream.WriteByte(64);
				ProtocolParser.WriteBool(stream, instance.enabled);
				stream.WriteByte(74);
				memoryStream.SetLength((long)0);
				Vector3Serialized.Serialize(memoryStream, instance.scale);
				uint length1 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length1);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length1);
				stream.WriteByte(82);
				memoryStream.SetLength((long)0);
				Vector3Serialized.Serialize(memoryStream, instance.colliderScale);
				uint num1 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num1);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num1);
				stream.WriteByte(93);
				ProtocolParser.WriteSingle(stream, instance.alpha);
				stream.WriteByte(96);
				ProtocolParser.WriteUInt32(stream, instance.prefabID);
				stream.WriteByte(104);
				ProtocolParser.WriteUInt32(stream, instance.parentID);
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeDelta(Stream stream, ArcadeGame.arcadeEnt instance, ArcadeGame.arcadeEnt previous)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				if (instance.id != previous.id)
				{
					stream.WriteByte(8);
					ProtocolParser.WriteUInt32(stream, instance.id);
				}
				if (instance.name != previous.name)
				{
					if (instance.name == null)
					{
						throw new ArgumentNullException("name", "Required by proto specification.");
					}
					stream.WriteByte(18);
					ProtocolParser.WriteString(stream, instance.name);
				}
				if (instance.spriteID != previous.spriteID)
				{
					stream.WriteByte(24);
					ProtocolParser.WriteUInt32(stream, instance.spriteID);
				}
				if (instance.soundID != previous.soundID)
				{
					stream.WriteByte(32);
					ProtocolParser.WriteUInt32(stream, instance.soundID);
				}
				stream.WriteByte(40);
				ProtocolParser.WriteBool(stream, instance.visible);
				if (instance.position != previous.position)
				{
					stream.WriteByte(50);
					memoryStream.SetLength((long)0);
					Vector3Serialized.SerializeDelta(memoryStream, instance.position, previous.position);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
				if (instance.heading != previous.heading)
				{
					stream.WriteByte(58);
					memoryStream.SetLength((long)0);
					Vector3Serialized.SerializeDelta(memoryStream, instance.heading, previous.heading);
					uint num = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, num);
					stream.Write(memoryStream.GetBuffer(), 0, (int)num);
				}
				stream.WriteByte(64);
				ProtocolParser.WriteBool(stream, instance.enabled);
				if (instance.scale != previous.scale)
				{
					stream.WriteByte(74);
					memoryStream.SetLength((long)0);
					Vector3Serialized.SerializeDelta(memoryStream, instance.scale, previous.scale);
					uint length1 = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length1);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length1);
				}
				if (instance.colliderScale != previous.colliderScale)
				{
					stream.WriteByte(82);
					memoryStream.SetLength((long)0);
					Vector3Serialized.SerializeDelta(memoryStream, instance.colliderScale, previous.colliderScale);
					uint num1 = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, num1);
					stream.Write(memoryStream.GetBuffer(), 0, (int)num1);
				}
				if (instance.alpha != previous.alpha)
				{
					stream.WriteByte(93);
					ProtocolParser.WriteSingle(stream, instance.alpha);
				}
				if (instance.prefabID != previous.prefabID)
				{
					stream.WriteByte(96);
					ProtocolParser.WriteUInt32(stream, instance.prefabID);
				}
				if (instance.parentID != previous.parentID)
				{
					stream.WriteByte(104);
					ProtocolParser.WriteUInt32(stream, instance.parentID);
				}
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeLengthDelimited(Stream stream, ArcadeGame.arcadeEnt instance)
			{
				byte[] bytes = ArcadeGame.arcadeEnt.SerializeToBytes(instance);
				ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
				stream.Write(bytes, 0, (int)bytes.Length);
			}

			public static byte[] SerializeToBytes(ArcadeGame.arcadeEnt instance)
			{
				byte[] array;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					ArcadeGame.arcadeEnt.Serialize(memoryStream, instance);
					array = memoryStream.ToArray();
				}
				return array;
			}

			public void ToProto(Stream stream)
			{
				ArcadeGame.arcadeEnt.Serialize(stream, this);
			}

			public byte[] ToProtoBytes()
			{
				return ArcadeGame.arcadeEnt.SerializeToBytes(this);
			}

			public virtual void WriteToStream(Stream stream)
			{
				ArcadeGame.arcadeEnt.Serialize(stream, this);
			}

			public virtual void WriteToStreamDelta(Stream stream, ArcadeGame.arcadeEnt previous)
			{
				if (previous == null)
				{
					ArcadeGame.arcadeEnt.Serialize(stream, this);
					return;
				}
				ArcadeGame.arcadeEnt.SerializeDelta(stream, this, previous);
			}
		}
	}
}