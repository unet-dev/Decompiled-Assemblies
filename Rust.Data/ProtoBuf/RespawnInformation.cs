using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProtoBuf
{
	public class RespawnInformation : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public List<RespawnInformation.SpawnOptions> spawnOptions;

		[NonSerialized]
		public PlayerLifeStory previousLife;

		[NonSerialized]
		public bool fadeIn;

		public bool ShouldPool = true;

		private bool _disposed;

		public RespawnInformation()
		{
		}

		public RespawnInformation Copy()
		{
			RespawnInformation respawnInformation = Pool.Get<RespawnInformation>();
			this.CopyTo(respawnInformation);
			return respawnInformation;
		}

		public void CopyTo(RespawnInformation instance)
		{
			throw new NotImplementedException();
		}

		public static RespawnInformation Deserialize(Stream stream)
		{
			RespawnInformation respawnInformation = Pool.Get<RespawnInformation>();
			RespawnInformation.Deserialize(stream, respawnInformation, false);
			return respawnInformation;
		}

		public static RespawnInformation Deserialize(byte[] buffer)
		{
			RespawnInformation respawnInformation = Pool.Get<RespawnInformation>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				RespawnInformation.Deserialize(memoryStream, respawnInformation, false);
			}
			return respawnInformation;
		}

		public static RespawnInformation Deserialize(byte[] buffer, RespawnInformation instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				RespawnInformation.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static RespawnInformation Deserialize(Stream stream, RespawnInformation instance, bool isDelta)
		{
			if (!isDelta && instance.spawnOptions == null)
			{
				instance.spawnOptions = Pool.Get<List<RespawnInformation.SpawnOptions>>();
			}
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num == 10)
				{
					instance.spawnOptions.Add(RespawnInformation.SpawnOptions.DeserializeLengthDelimited(stream));
				}
				else if (num != 18)
				{
					if (num == 24)
					{
						instance.fadeIn = ProtocolParser.ReadBool(stream);
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
				else if (instance.previousLife != null)
				{
					PlayerLifeStory.DeserializeLengthDelimited(stream, instance.previousLife, isDelta);
				}
				else
				{
					instance.previousLife = PlayerLifeStory.DeserializeLengthDelimited(stream);
				}
			}
			return instance;
		}

		public static RespawnInformation DeserializeLength(Stream stream, int length)
		{
			RespawnInformation respawnInformation = Pool.Get<RespawnInformation>();
			RespawnInformation.DeserializeLength(stream, length, respawnInformation, false);
			return respawnInformation;
		}

		public static RespawnInformation DeserializeLength(Stream stream, int length, RespawnInformation instance, bool isDelta)
		{
			if (!isDelta && instance.spawnOptions == null)
			{
				instance.spawnOptions = Pool.Get<List<RespawnInformation.SpawnOptions>>();
			}
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
					instance.spawnOptions.Add(RespawnInformation.SpawnOptions.DeserializeLengthDelimited(stream));
				}
				else if (num != 18)
				{
					if (num == 24)
					{
						instance.fadeIn = ProtocolParser.ReadBool(stream);
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
				else if (instance.previousLife != null)
				{
					PlayerLifeStory.DeserializeLengthDelimited(stream, instance.previousLife, isDelta);
				}
				else
				{
					instance.previousLife = PlayerLifeStory.DeserializeLengthDelimited(stream);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static RespawnInformation DeserializeLengthDelimited(Stream stream)
		{
			RespawnInformation respawnInformation = Pool.Get<RespawnInformation>();
			RespawnInformation.DeserializeLengthDelimited(stream, respawnInformation, false);
			return respawnInformation;
		}

		public static RespawnInformation DeserializeLengthDelimited(Stream stream, RespawnInformation instance, bool isDelta)
		{
			if (!isDelta && instance.spawnOptions == null)
			{
				instance.spawnOptions = Pool.Get<List<RespawnInformation.SpawnOptions>>();
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
				if (num == 10)
				{
					instance.spawnOptions.Add(RespawnInformation.SpawnOptions.DeserializeLengthDelimited(stream));
				}
				else if (num != 18)
				{
					if (num == 24)
					{
						instance.fadeIn = ProtocolParser.ReadBool(stream);
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
				else if (instance.previousLife != null)
				{
					PlayerLifeStory.DeserializeLengthDelimited(stream, instance.previousLife, isDelta);
				}
				else
				{
					instance.previousLife = PlayerLifeStory.DeserializeLengthDelimited(stream);
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
			RespawnInformation.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			RespawnInformation.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(RespawnInformation instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			if (instance.spawnOptions != null)
			{
				for (int i = 0; i < instance.spawnOptions.Count; i++)
				{
					if (instance.spawnOptions[i] != null)
					{
						instance.spawnOptions[i].ResetToPool();
						instance.spawnOptions[i] = null;
					}
				}
				List<RespawnInformation.SpawnOptions> spawnOptions = instance.spawnOptions;
				Pool.FreeList<RespawnInformation.SpawnOptions>(ref spawnOptions);
				instance.spawnOptions = spawnOptions;
			}
			if (instance.previousLife != null)
			{
				instance.previousLife.ResetToPool();
				instance.previousLife = null;
			}
			instance.fadeIn = false;
			Pool.Free<RespawnInformation>(ref instance);
		}

		public void ResetToPool()
		{
			RespawnInformation.ResetToPool(this);
		}

		public static void Serialize(Stream stream, RespawnInformation instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.spawnOptions != null)
			{
				for (int i = 0; i < instance.spawnOptions.Count; i++)
				{
					RespawnInformation.SpawnOptions item = instance.spawnOptions[i];
					stream.WriteByte(10);
					memoryStream.SetLength((long)0);
					RespawnInformation.SpawnOptions.Serialize(memoryStream, item);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			if (instance.previousLife != null)
			{
				stream.WriteByte(18);
				memoryStream.SetLength((long)0);
				PlayerLifeStory.Serialize(memoryStream, instance.previousLife);
				uint num = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			}
			stream.WriteByte(24);
			ProtocolParser.WriteBool(stream, instance.fadeIn);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, RespawnInformation instance, RespawnInformation previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.spawnOptions != null)
			{
				for (int i = 0; i < instance.spawnOptions.Count; i++)
				{
					RespawnInformation.SpawnOptions item = instance.spawnOptions[i];
					stream.WriteByte(10);
					memoryStream.SetLength((long)0);
					RespawnInformation.SpawnOptions.SerializeDelta(memoryStream, item, item);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			if (instance.previousLife != null)
			{
				stream.WriteByte(18);
				memoryStream.SetLength((long)0);
				PlayerLifeStory.SerializeDelta(memoryStream, instance.previousLife, previous.previousLife);
				uint num = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			}
			stream.WriteByte(24);
			ProtocolParser.WriteBool(stream, instance.fadeIn);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, RespawnInformation instance)
		{
			byte[] bytes = RespawnInformation.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(RespawnInformation instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				RespawnInformation.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			RespawnInformation.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return RespawnInformation.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			RespawnInformation.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, RespawnInformation previous)
		{
			if (previous == null)
			{
				RespawnInformation.Serialize(stream, this);
				return;
			}
			RespawnInformation.SerializeDelta(stream, this, previous);
		}

		public class SpawnOptions : IDisposable, Pool.IPooled, IProto
		{
			[NonSerialized]
			public RespawnInformation.SpawnOptions.RespawnType type;

			[NonSerialized]
			public uint id;

			[NonSerialized]
			public string name;

			[NonSerialized]
			public float unlockSeconds;

			public bool ShouldPool;

			private bool _disposed;

			public SpawnOptions()
			{
			}

			public RespawnInformation.SpawnOptions Copy()
			{
				RespawnInformation.SpawnOptions spawnOption = Pool.Get<RespawnInformation.SpawnOptions>();
				this.CopyTo(spawnOption);
				return spawnOption;
			}

			public void CopyTo(RespawnInformation.SpawnOptions instance)
			{
				instance.type = this.type;
				instance.id = this.id;
				instance.name = this.name;
				instance.unlockSeconds = this.unlockSeconds;
			}

			public static RespawnInformation.SpawnOptions Deserialize(Stream stream)
			{
				RespawnInformation.SpawnOptions spawnOption = Pool.Get<RespawnInformation.SpawnOptions>();
				RespawnInformation.SpawnOptions.Deserialize(stream, spawnOption, false);
				return spawnOption;
			}

			public static RespawnInformation.SpawnOptions Deserialize(byte[] buffer)
			{
				RespawnInformation.SpawnOptions spawnOption = Pool.Get<RespawnInformation.SpawnOptions>();
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					RespawnInformation.SpawnOptions.Deserialize(memoryStream, spawnOption, false);
				}
				return spawnOption;
			}

			public static RespawnInformation.SpawnOptions Deserialize(byte[] buffer, RespawnInformation.SpawnOptions instance, bool isDelta = false)
			{
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					RespawnInformation.SpawnOptions.Deserialize(memoryStream, instance, isDelta);
				}
				return instance;
			}

			public static RespawnInformation.SpawnOptions Deserialize(Stream stream, RespawnInformation.SpawnOptions instance, bool isDelta)
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
							instance.type = (RespawnInformation.SpawnOptions.RespawnType)((int)ProtocolParser.ReadUInt64(stream));
							continue;
						}
						else if (num == 16)
						{
							instance.id = ProtocolParser.ReadUInt32(stream);
							continue;
						}
					}
					else if (num == 26)
					{
						instance.name = ProtocolParser.ReadString(stream);
						continue;
					}
					else if (num == 37)
					{
						instance.unlockSeconds = ProtocolParser.ReadSingle(stream);
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

			public static RespawnInformation.SpawnOptions DeserializeLength(Stream stream, int length)
			{
				RespawnInformation.SpawnOptions spawnOption = Pool.Get<RespawnInformation.SpawnOptions>();
				RespawnInformation.SpawnOptions.DeserializeLength(stream, length, spawnOption, false);
				return spawnOption;
			}

			public static RespawnInformation.SpawnOptions DeserializeLength(Stream stream, int length, RespawnInformation.SpawnOptions instance, bool isDelta)
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
							instance.type = (RespawnInformation.SpawnOptions.RespawnType)((int)ProtocolParser.ReadUInt64(stream));
							continue;
						}
						else if (num == 16)
						{
							instance.id = ProtocolParser.ReadUInt32(stream);
							continue;
						}
					}
					else if (num == 26)
					{
						instance.name = ProtocolParser.ReadString(stream);
						continue;
					}
					else if (num == 37)
					{
						instance.unlockSeconds = ProtocolParser.ReadSingle(stream);
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

			public static RespawnInformation.SpawnOptions DeserializeLengthDelimited(Stream stream)
			{
				RespawnInformation.SpawnOptions spawnOption = Pool.Get<RespawnInformation.SpawnOptions>();
				RespawnInformation.SpawnOptions.DeserializeLengthDelimited(stream, spawnOption, false);
				return spawnOption;
			}

			public static RespawnInformation.SpawnOptions DeserializeLengthDelimited(Stream stream, RespawnInformation.SpawnOptions instance, bool isDelta)
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
							instance.type = (RespawnInformation.SpawnOptions.RespawnType)((int)ProtocolParser.ReadUInt64(stream));
							continue;
						}
						else if (num == 16)
						{
							instance.id = ProtocolParser.ReadUInt32(stream);
							continue;
						}
					}
					else if (num == 26)
					{
						instance.name = ProtocolParser.ReadString(stream);
						continue;
					}
					else if (num == 37)
					{
						instance.unlockSeconds = ProtocolParser.ReadSingle(stream);
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
				RespawnInformation.SpawnOptions.Deserialize(stream, this, isDelta);
			}

			public virtual void LeavePool()
			{
				this._disposed = false;
			}

			public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
			{
				RespawnInformation.SpawnOptions.DeserializeLength(stream, size, this, isDelta);
			}

			public static void ResetToPool(RespawnInformation.SpawnOptions instance)
			{
				if (!instance.ShouldPool)
				{
					return;
				}
				instance.type = (RespawnInformation.SpawnOptions.RespawnType)0;
				instance.id = 0;
				instance.name = string.Empty;
				instance.unlockSeconds = 0f;
				Pool.Free<RespawnInformation.SpawnOptions>(ref instance);
			}

			public void ResetToPool()
			{
				RespawnInformation.SpawnOptions.ResetToPool(this);
			}

			public static void Serialize(Stream stream, RespawnInformation.SpawnOptions instance)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				stream.WriteByte(8);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.type);
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.id);
				if (instance.name == null)
				{
					throw new ArgumentNullException("name", "Required by proto specification.");
				}
				stream.WriteByte(26);
				ProtocolParser.WriteString(stream, instance.name);
				stream.WriteByte(37);
				ProtocolParser.WriteSingle(stream, instance.unlockSeconds);
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeDelta(Stream stream, RespawnInformation.SpawnOptions instance, RespawnInformation.SpawnOptions previous)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				stream.WriteByte(8);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.type);
				if (instance.id != previous.id)
				{
					stream.WriteByte(16);
					ProtocolParser.WriteUInt32(stream, instance.id);
				}
				if (instance.name != previous.name)
				{
					if (instance.name == null)
					{
						throw new ArgumentNullException("name", "Required by proto specification.");
					}
					stream.WriteByte(26);
					ProtocolParser.WriteString(stream, instance.name);
				}
				if (instance.unlockSeconds != previous.unlockSeconds)
				{
					stream.WriteByte(37);
					ProtocolParser.WriteSingle(stream, instance.unlockSeconds);
				}
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeLengthDelimited(Stream stream, RespawnInformation.SpawnOptions instance)
			{
				byte[] bytes = RespawnInformation.SpawnOptions.SerializeToBytes(instance);
				ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
				stream.Write(bytes, 0, (int)bytes.Length);
			}

			public static byte[] SerializeToBytes(RespawnInformation.SpawnOptions instance)
			{
				byte[] array;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					RespawnInformation.SpawnOptions.Serialize(memoryStream, instance);
					array = memoryStream.ToArray();
				}
				return array;
			}

			public void ToProto(Stream stream)
			{
				RespawnInformation.SpawnOptions.Serialize(stream, this);
			}

			public byte[] ToProtoBytes()
			{
				return RespawnInformation.SpawnOptions.SerializeToBytes(this);
			}

			public virtual void WriteToStream(Stream stream)
			{
				RespawnInformation.SpawnOptions.Serialize(stream, this);
			}

			public virtual void WriteToStreamDelta(Stream stream, RespawnInformation.SpawnOptions previous)
			{
				if (previous == null)
				{
					RespawnInformation.SpawnOptions.Serialize(stream, this);
					return;
				}
				RespawnInformation.SpawnOptions.SerializeDelta(stream, this, previous);
			}

			public enum RespawnType
			{
				SleepingBag = 1,
				Bed = 2
			}
		}
	}
}