using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProtoBuf
{
	public class GenericSpawner : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public List<GenericSpawner.SpawnedEnt> ents;

		public bool ShouldPool = true;

		private bool _disposed;

		public GenericSpawner()
		{
		}

		public GenericSpawner Copy()
		{
			GenericSpawner genericSpawner = Pool.Get<GenericSpawner>();
			this.CopyTo(genericSpawner);
			return genericSpawner;
		}

		public void CopyTo(GenericSpawner instance)
		{
			throw new NotImplementedException();
		}

		public static GenericSpawner Deserialize(Stream stream)
		{
			GenericSpawner genericSpawner = Pool.Get<GenericSpawner>();
			GenericSpawner.Deserialize(stream, genericSpawner, false);
			return genericSpawner;
		}

		public static GenericSpawner Deserialize(byte[] buffer)
		{
			GenericSpawner genericSpawner = Pool.Get<GenericSpawner>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				GenericSpawner.Deserialize(memoryStream, genericSpawner, false);
			}
			return genericSpawner;
		}

		public static GenericSpawner Deserialize(byte[] buffer, GenericSpawner instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				GenericSpawner.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static GenericSpawner Deserialize(Stream stream, GenericSpawner instance, bool isDelta)
		{
			if (!isDelta && instance.ents == null)
			{
				instance.ents = Pool.Get<List<GenericSpawner.SpawnedEnt>>();
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
					instance.ents.Add(GenericSpawner.SpawnedEnt.DeserializeLengthDelimited(stream));
				}
			}
			return instance;
		}

		public static GenericSpawner DeserializeLength(Stream stream, int length)
		{
			GenericSpawner genericSpawner = Pool.Get<GenericSpawner>();
			GenericSpawner.DeserializeLength(stream, length, genericSpawner, false);
			return genericSpawner;
		}

		public static GenericSpawner DeserializeLength(Stream stream, int length, GenericSpawner instance, bool isDelta)
		{
			if (!isDelta && instance.ents == null)
			{
				instance.ents = Pool.Get<List<GenericSpawner.SpawnedEnt>>();
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
					instance.ents.Add(GenericSpawner.SpawnedEnt.DeserializeLengthDelimited(stream));
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static GenericSpawner DeserializeLengthDelimited(Stream stream)
		{
			GenericSpawner genericSpawner = Pool.Get<GenericSpawner>();
			GenericSpawner.DeserializeLengthDelimited(stream, genericSpawner, false);
			return genericSpawner;
		}

		public static GenericSpawner DeserializeLengthDelimited(Stream stream, GenericSpawner instance, bool isDelta)
		{
			if (!isDelta && instance.ents == null)
			{
				instance.ents = Pool.Get<List<GenericSpawner.SpawnedEnt>>();
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
					instance.ents.Add(GenericSpawner.SpawnedEnt.DeserializeLengthDelimited(stream));
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
			GenericSpawner.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			GenericSpawner.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(GenericSpawner instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			if (instance.ents != null)
			{
				for (int i = 0; i < instance.ents.Count; i++)
				{
					if (instance.ents[i] != null)
					{
						instance.ents[i].ResetToPool();
						instance.ents[i] = null;
					}
				}
				List<GenericSpawner.SpawnedEnt> spawnedEnts = instance.ents;
				Pool.FreeList<GenericSpawner.SpawnedEnt>(ref spawnedEnts);
				instance.ents = spawnedEnts;
			}
			Pool.Free<GenericSpawner>(ref instance);
		}

		public void ResetToPool()
		{
			GenericSpawner.ResetToPool(this);
		}

		public static void Serialize(Stream stream, GenericSpawner instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.ents != null)
			{
				for (int i = 0; i < instance.ents.Count; i++)
				{
					GenericSpawner.SpawnedEnt item = instance.ents[i];
					stream.WriteByte(10);
					memoryStream.SetLength((long)0);
					GenericSpawner.SpawnedEnt.Serialize(memoryStream, item);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, GenericSpawner instance, GenericSpawner previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.ents != null)
			{
				for (int i = 0; i < instance.ents.Count; i++)
				{
					GenericSpawner.SpawnedEnt item = instance.ents[i];
					stream.WriteByte(10);
					memoryStream.SetLength((long)0);
					GenericSpawner.SpawnedEnt.SerializeDelta(memoryStream, item, item);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, GenericSpawner instance)
		{
			byte[] bytes = GenericSpawner.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(GenericSpawner instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				GenericSpawner.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			GenericSpawner.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return GenericSpawner.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			GenericSpawner.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, GenericSpawner previous)
		{
			if (previous == null)
			{
				GenericSpawner.Serialize(stream, this);
				return;
			}
			GenericSpawner.SerializeDelta(stream, this, previous);
		}

		public class SpawnedEnt : IDisposable, Pool.IPooled, IProto
		{
			[NonSerialized]
			public uint uid;

			[NonSerialized]
			public uint spawnPointIndex;

			[NonSerialized]
			public bool mobile;

			public bool ShouldPool;

			private bool _disposed;

			public SpawnedEnt()
			{
			}

			public GenericSpawner.SpawnedEnt Copy()
			{
				GenericSpawner.SpawnedEnt spawnedEnt = Pool.Get<GenericSpawner.SpawnedEnt>();
				this.CopyTo(spawnedEnt);
				return spawnedEnt;
			}

			public void CopyTo(GenericSpawner.SpawnedEnt instance)
			{
				instance.uid = this.uid;
				instance.spawnPointIndex = this.spawnPointIndex;
				instance.mobile = this.mobile;
			}

			public static GenericSpawner.SpawnedEnt Deserialize(Stream stream)
			{
				GenericSpawner.SpawnedEnt spawnedEnt = Pool.Get<GenericSpawner.SpawnedEnt>();
				GenericSpawner.SpawnedEnt.Deserialize(stream, spawnedEnt, false);
				return spawnedEnt;
			}

			public static GenericSpawner.SpawnedEnt Deserialize(byte[] buffer)
			{
				GenericSpawner.SpawnedEnt spawnedEnt = Pool.Get<GenericSpawner.SpawnedEnt>();
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					GenericSpawner.SpawnedEnt.Deserialize(memoryStream, spawnedEnt, false);
				}
				return spawnedEnt;
			}

			public static GenericSpawner.SpawnedEnt Deserialize(byte[] buffer, GenericSpawner.SpawnedEnt instance, bool isDelta = false)
			{
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					GenericSpawner.SpawnedEnt.Deserialize(memoryStream, instance, isDelta);
				}
				return instance;
			}

			public static GenericSpawner.SpawnedEnt Deserialize(Stream stream, GenericSpawner.SpawnedEnt instance, bool isDelta)
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
						instance.uid = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 16)
					{
						instance.spawnPointIndex = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 24)
					{
						instance.mobile = ProtocolParser.ReadBool(stream);
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

			public static GenericSpawner.SpawnedEnt DeserializeLength(Stream stream, int length)
			{
				GenericSpawner.SpawnedEnt spawnedEnt = Pool.Get<GenericSpawner.SpawnedEnt>();
				GenericSpawner.SpawnedEnt.DeserializeLength(stream, length, spawnedEnt, false);
				return spawnedEnt;
			}

			public static GenericSpawner.SpawnedEnt DeserializeLength(Stream stream, int length, GenericSpawner.SpawnedEnt instance, bool isDelta)
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
						instance.uid = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 16)
					{
						instance.spawnPointIndex = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 24)
					{
						instance.mobile = ProtocolParser.ReadBool(stream);
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

			public static GenericSpawner.SpawnedEnt DeserializeLengthDelimited(Stream stream)
			{
				GenericSpawner.SpawnedEnt spawnedEnt = Pool.Get<GenericSpawner.SpawnedEnt>();
				GenericSpawner.SpawnedEnt.DeserializeLengthDelimited(stream, spawnedEnt, false);
				return spawnedEnt;
			}

			public static GenericSpawner.SpawnedEnt DeserializeLengthDelimited(Stream stream, GenericSpawner.SpawnedEnt instance, bool isDelta)
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
						instance.uid = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 16)
					{
						instance.spawnPointIndex = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 24)
					{
						instance.mobile = ProtocolParser.ReadBool(stream);
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
				GenericSpawner.SpawnedEnt.Deserialize(stream, this, isDelta);
			}

			public virtual void LeavePool()
			{
				this._disposed = false;
			}

			public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
			{
				GenericSpawner.SpawnedEnt.DeserializeLength(stream, size, this, isDelta);
			}

			public static void ResetToPool(GenericSpawner.SpawnedEnt instance)
			{
				if (!instance.ShouldPool)
				{
					return;
				}
				instance.uid = 0;
				instance.spawnPointIndex = 0;
				instance.mobile = false;
				Pool.Free<GenericSpawner.SpawnedEnt>(ref instance);
			}

			public void ResetToPool()
			{
				GenericSpawner.SpawnedEnt.ResetToPool(this);
			}

			public static void Serialize(Stream stream, GenericSpawner.SpawnedEnt instance)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				stream.WriteByte(8);
				ProtocolParser.WriteUInt32(stream, instance.uid);
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.spawnPointIndex);
				stream.WriteByte(24);
				ProtocolParser.WriteBool(stream, instance.mobile);
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeDelta(Stream stream, GenericSpawner.SpawnedEnt instance, GenericSpawner.SpawnedEnt previous)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				if (instance.uid != previous.uid)
				{
					stream.WriteByte(8);
					ProtocolParser.WriteUInt32(stream, instance.uid);
				}
				if (instance.spawnPointIndex != previous.spawnPointIndex)
				{
					stream.WriteByte(16);
					ProtocolParser.WriteUInt32(stream, instance.spawnPointIndex);
				}
				stream.WriteByte(24);
				ProtocolParser.WriteBool(stream, instance.mobile);
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeLengthDelimited(Stream stream, GenericSpawner.SpawnedEnt instance)
			{
				byte[] bytes = GenericSpawner.SpawnedEnt.SerializeToBytes(instance);
				ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
				stream.Write(bytes, 0, (int)bytes.Length);
			}

			public static byte[] SerializeToBytes(GenericSpawner.SpawnedEnt instance)
			{
				byte[] array;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					GenericSpawner.SpawnedEnt.Serialize(memoryStream, instance);
					array = memoryStream.ToArray();
				}
				return array;
			}

			public void ToProto(Stream stream)
			{
				GenericSpawner.SpawnedEnt.Serialize(stream, this);
			}

			public byte[] ToProtoBytes()
			{
				return GenericSpawner.SpawnedEnt.SerializeToBytes(this);
			}

			public virtual void WriteToStream(Stream stream)
			{
				GenericSpawner.SpawnedEnt.Serialize(stream, this);
			}

			public virtual void WriteToStreamDelta(Stream stream, GenericSpawner.SpawnedEnt previous)
			{
				if (previous == null)
				{
					GenericSpawner.SpawnedEnt.Serialize(stream, this);
					return;
				}
				GenericSpawner.SpawnedEnt.SerializeDelta(stream, this, previous);
			}
		}
	}
}