using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProtoBuf
{
	public class ArcadeMachine : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public List<ArcadeMachine.ScoreEntry> scores;

		[NonSerialized]
		public int genericInt1;

		[NonSerialized]
		public int genericInt2;

		[NonSerialized]
		public int genericInt3;

		[NonSerialized]
		public int genericInt4;

		[NonSerialized]
		public float genericFloat1;

		[NonSerialized]
		public float genericFloat2;

		[NonSerialized]
		public float genericFloat3;

		[NonSerialized]
		public float genericFloat4;

		public bool ShouldPool = true;

		private bool _disposed;

		public ArcadeMachine()
		{
		}

		public ArcadeMachine Copy()
		{
			ArcadeMachine arcadeMachine = Pool.Get<ArcadeMachine>();
			this.CopyTo(arcadeMachine);
			return arcadeMachine;
		}

		public void CopyTo(ArcadeMachine instance)
		{
			throw new NotImplementedException();
		}

		public static ArcadeMachine Deserialize(Stream stream)
		{
			ArcadeMachine arcadeMachine = Pool.Get<ArcadeMachine>();
			ArcadeMachine.Deserialize(stream, arcadeMachine, false);
			return arcadeMachine;
		}

		public static ArcadeMachine Deserialize(byte[] buffer)
		{
			ArcadeMachine arcadeMachine = Pool.Get<ArcadeMachine>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				ArcadeMachine.Deserialize(memoryStream, arcadeMachine, false);
			}
			return arcadeMachine;
		}

		public static ArcadeMachine Deserialize(byte[] buffer, ArcadeMachine instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				ArcadeMachine.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static ArcadeMachine Deserialize(Stream stream, ArcadeMachine instance, bool isDelta)
		{
			if (!isDelta && instance.scores == null)
			{
				instance.scores = Pool.Get<List<ArcadeMachine.ScoreEntry>>();
			}
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num <= 32)
				{
					if (num <= 16)
					{
						if (num == 10)
						{
							instance.scores.Add(ArcadeMachine.ScoreEntry.DeserializeLengthDelimited(stream));
							continue;
						}
						else if (num == 16)
						{
							instance.genericInt1 = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
					}
					else if (num == 24)
					{
						instance.genericInt2 = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 32)
					{
						instance.genericInt3 = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
				}
				else if (num <= 53)
				{
					if (num == 40)
					{
						instance.genericInt4 = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 53)
					{
						instance.genericFloat1 = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (num == 61)
				{
					instance.genericFloat2 = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 69)
				{
					instance.genericFloat3 = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 77)
				{
					instance.genericFloat4 = ProtocolParser.ReadSingle(stream);
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

		public static ArcadeMachine DeserializeLength(Stream stream, int length)
		{
			ArcadeMachine arcadeMachine = Pool.Get<ArcadeMachine>();
			ArcadeMachine.DeserializeLength(stream, length, arcadeMachine, false);
			return arcadeMachine;
		}

		public static ArcadeMachine DeserializeLength(Stream stream, int length, ArcadeMachine instance, bool isDelta)
		{
			if (!isDelta && instance.scores == null)
			{
				instance.scores = Pool.Get<List<ArcadeMachine.ScoreEntry>>();
			}
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
					if (num <= 16)
					{
						if (num == 10)
						{
							instance.scores.Add(ArcadeMachine.ScoreEntry.DeserializeLengthDelimited(stream));
							continue;
						}
						else if (num == 16)
						{
							instance.genericInt1 = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
					}
					else if (num == 24)
					{
						instance.genericInt2 = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 32)
					{
						instance.genericInt3 = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
				}
				else if (num <= 53)
				{
					if (num == 40)
					{
						instance.genericInt4 = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 53)
					{
						instance.genericFloat1 = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (num == 61)
				{
					instance.genericFloat2 = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 69)
				{
					instance.genericFloat3 = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 77)
				{
					instance.genericFloat4 = ProtocolParser.ReadSingle(stream);
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

		public static ArcadeMachine DeserializeLengthDelimited(Stream stream)
		{
			ArcadeMachine arcadeMachine = Pool.Get<ArcadeMachine>();
			ArcadeMachine.DeserializeLengthDelimited(stream, arcadeMachine, false);
			return arcadeMachine;
		}

		public static ArcadeMachine DeserializeLengthDelimited(Stream stream, ArcadeMachine instance, bool isDelta)
		{
			if (!isDelta && instance.scores == null)
			{
				instance.scores = Pool.Get<List<ArcadeMachine.ScoreEntry>>();
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
				if (num <= 32)
				{
					if (num <= 16)
					{
						if (num == 10)
						{
							instance.scores.Add(ArcadeMachine.ScoreEntry.DeserializeLengthDelimited(stream));
							continue;
						}
						else if (num == 16)
						{
							instance.genericInt1 = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
					}
					else if (num == 24)
					{
						instance.genericInt2 = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 32)
					{
						instance.genericInt3 = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
				}
				else if (num <= 53)
				{
					if (num == 40)
					{
						instance.genericInt4 = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 53)
					{
						instance.genericFloat1 = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (num == 61)
				{
					instance.genericFloat2 = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 69)
				{
					instance.genericFloat3 = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 77)
				{
					instance.genericFloat4 = ProtocolParser.ReadSingle(stream);
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
			ArcadeMachine.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			ArcadeMachine.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(ArcadeMachine instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			if (instance.scores != null)
			{
				for (int i = 0; i < instance.scores.Count; i++)
				{
					if (instance.scores[i] != null)
					{
						instance.scores[i].ResetToPool();
						instance.scores[i] = null;
					}
				}
				List<ArcadeMachine.ScoreEntry> scoreEntries = instance.scores;
				Pool.FreeList<ArcadeMachine.ScoreEntry>(ref scoreEntries);
				instance.scores = scoreEntries;
			}
			instance.genericInt1 = 0;
			instance.genericInt2 = 0;
			instance.genericInt3 = 0;
			instance.genericInt4 = 0;
			instance.genericFloat1 = 0f;
			instance.genericFloat2 = 0f;
			instance.genericFloat3 = 0f;
			instance.genericFloat4 = 0f;
			Pool.Free<ArcadeMachine>(ref instance);
		}

		public void ResetToPool()
		{
			ArcadeMachine.ResetToPool(this);
		}

		public static void Serialize(Stream stream, ArcadeMachine instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.scores != null)
			{
				for (int i = 0; i < instance.scores.Count; i++)
				{
					ArcadeMachine.ScoreEntry item = instance.scores[i];
					stream.WriteByte(10);
					memoryStream.SetLength((long)0);
					ArcadeMachine.ScoreEntry.Serialize(memoryStream, item);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			stream.WriteByte(16);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.genericInt1);
			stream.WriteByte(24);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.genericInt2);
			stream.WriteByte(32);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.genericInt3);
			stream.WriteByte(40);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.genericInt4);
			stream.WriteByte(53);
			ProtocolParser.WriteSingle(stream, instance.genericFloat1);
			stream.WriteByte(61);
			ProtocolParser.WriteSingle(stream, instance.genericFloat2);
			stream.WriteByte(69);
			ProtocolParser.WriteSingle(stream, instance.genericFloat3);
			stream.WriteByte(77);
			ProtocolParser.WriteSingle(stream, instance.genericFloat4);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, ArcadeMachine instance, ArcadeMachine previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.scores != null)
			{
				for (int i = 0; i < instance.scores.Count; i++)
				{
					ArcadeMachine.ScoreEntry item = instance.scores[i];
					stream.WriteByte(10);
					memoryStream.SetLength((long)0);
					ArcadeMachine.ScoreEntry.SerializeDelta(memoryStream, item, item);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			if (instance.genericInt1 != previous.genericInt1)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.genericInt1);
			}
			if (instance.genericInt2 != previous.genericInt2)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.genericInt2);
			}
			if (instance.genericInt3 != previous.genericInt3)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.genericInt3);
			}
			if (instance.genericInt4 != previous.genericInt4)
			{
				stream.WriteByte(40);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.genericInt4);
			}
			if (instance.genericFloat1 != previous.genericFloat1)
			{
				stream.WriteByte(53);
				ProtocolParser.WriteSingle(stream, instance.genericFloat1);
			}
			if (instance.genericFloat2 != previous.genericFloat2)
			{
				stream.WriteByte(61);
				ProtocolParser.WriteSingle(stream, instance.genericFloat2);
			}
			if (instance.genericFloat3 != previous.genericFloat3)
			{
				stream.WriteByte(69);
				ProtocolParser.WriteSingle(stream, instance.genericFloat3);
			}
			if (instance.genericFloat4 != previous.genericFloat4)
			{
				stream.WriteByte(77);
				ProtocolParser.WriteSingle(stream, instance.genericFloat4);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, ArcadeMachine instance)
		{
			byte[] bytes = ArcadeMachine.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(ArcadeMachine instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				ArcadeMachine.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			ArcadeMachine.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return ArcadeMachine.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			ArcadeMachine.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, ArcadeMachine previous)
		{
			if (previous == null)
			{
				ArcadeMachine.Serialize(stream, this);
				return;
			}
			ArcadeMachine.SerializeDelta(stream, this, previous);
		}

		public class ScoreEntry : IDisposable, Pool.IPooled, IProto
		{
			[NonSerialized]
			public ulong playerID;

			[NonSerialized]
			public string displayName;

			[NonSerialized]
			public int score;

			public bool ShouldPool;

			private bool _disposed;

			public ScoreEntry()
			{
			}

			public ArcadeMachine.ScoreEntry Copy()
			{
				ArcadeMachine.ScoreEntry scoreEntry = Pool.Get<ArcadeMachine.ScoreEntry>();
				this.CopyTo(scoreEntry);
				return scoreEntry;
			}

			public void CopyTo(ArcadeMachine.ScoreEntry instance)
			{
				instance.playerID = this.playerID;
				instance.displayName = this.displayName;
				instance.score = this.score;
			}

			public static ArcadeMachine.ScoreEntry Deserialize(Stream stream)
			{
				ArcadeMachine.ScoreEntry scoreEntry = Pool.Get<ArcadeMachine.ScoreEntry>();
				ArcadeMachine.ScoreEntry.Deserialize(stream, scoreEntry, false);
				return scoreEntry;
			}

			public static ArcadeMachine.ScoreEntry Deserialize(byte[] buffer)
			{
				ArcadeMachine.ScoreEntry scoreEntry = Pool.Get<ArcadeMachine.ScoreEntry>();
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					ArcadeMachine.ScoreEntry.Deserialize(memoryStream, scoreEntry, false);
				}
				return scoreEntry;
			}

			public static ArcadeMachine.ScoreEntry Deserialize(byte[] buffer, ArcadeMachine.ScoreEntry instance, bool isDelta = false)
			{
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					ArcadeMachine.ScoreEntry.Deserialize(memoryStream, instance, isDelta);
				}
				return instance;
			}

			public static ArcadeMachine.ScoreEntry Deserialize(Stream stream, ArcadeMachine.ScoreEntry instance, bool isDelta)
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
						instance.playerID = ProtocolParser.ReadUInt64(stream);
					}
					else if (num == 18)
					{
						instance.displayName = ProtocolParser.ReadString(stream);
					}
					else if (num == 24)
					{
						instance.score = (int)ProtocolParser.ReadUInt64(stream);
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

			public static ArcadeMachine.ScoreEntry DeserializeLength(Stream stream, int length)
			{
				ArcadeMachine.ScoreEntry scoreEntry = Pool.Get<ArcadeMachine.ScoreEntry>();
				ArcadeMachine.ScoreEntry.DeserializeLength(stream, length, scoreEntry, false);
				return scoreEntry;
			}

			public static ArcadeMachine.ScoreEntry DeserializeLength(Stream stream, int length, ArcadeMachine.ScoreEntry instance, bool isDelta)
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
						instance.playerID = ProtocolParser.ReadUInt64(stream);
					}
					else if (num == 18)
					{
						instance.displayName = ProtocolParser.ReadString(stream);
					}
					else if (num == 24)
					{
						instance.score = (int)ProtocolParser.ReadUInt64(stream);
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

			public static ArcadeMachine.ScoreEntry DeserializeLengthDelimited(Stream stream)
			{
				ArcadeMachine.ScoreEntry scoreEntry = Pool.Get<ArcadeMachine.ScoreEntry>();
				ArcadeMachine.ScoreEntry.DeserializeLengthDelimited(stream, scoreEntry, false);
				return scoreEntry;
			}

			public static ArcadeMachine.ScoreEntry DeserializeLengthDelimited(Stream stream, ArcadeMachine.ScoreEntry instance, bool isDelta)
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
						instance.playerID = ProtocolParser.ReadUInt64(stream);
					}
					else if (num == 18)
					{
						instance.displayName = ProtocolParser.ReadString(stream);
					}
					else if (num == 24)
					{
						instance.score = (int)ProtocolParser.ReadUInt64(stream);
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
				ArcadeMachine.ScoreEntry.Deserialize(stream, this, isDelta);
			}

			public virtual void LeavePool()
			{
				this._disposed = false;
			}

			public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
			{
				ArcadeMachine.ScoreEntry.DeserializeLength(stream, size, this, isDelta);
			}

			public static void ResetToPool(ArcadeMachine.ScoreEntry instance)
			{
				if (!instance.ShouldPool)
				{
					return;
				}
				instance.playerID = (ulong)0;
				instance.displayName = string.Empty;
				instance.score = 0;
				Pool.Free<ArcadeMachine.ScoreEntry>(ref instance);
			}

			public void ResetToPool()
			{
				ArcadeMachine.ScoreEntry.ResetToPool(this);
			}

			public static void Serialize(Stream stream, ArcadeMachine.ScoreEntry instance)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				stream.WriteByte(8);
				ProtocolParser.WriteUInt64(stream, instance.playerID);
				if (instance.displayName != null)
				{
					stream.WriteByte(18);
					ProtocolParser.WriteString(stream, instance.displayName);
				}
				stream.WriteByte(24);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.score);
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeDelta(Stream stream, ArcadeMachine.ScoreEntry instance, ArcadeMachine.ScoreEntry previous)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				if (instance.playerID != previous.playerID)
				{
					stream.WriteByte(8);
					ProtocolParser.WriteUInt64(stream, instance.playerID);
				}
				if (instance.displayName != null && instance.displayName != previous.displayName)
				{
					stream.WriteByte(18);
					ProtocolParser.WriteString(stream, instance.displayName);
				}
				if (instance.score != previous.score)
				{
					stream.WriteByte(24);
					ProtocolParser.WriteUInt64(stream, (ulong)instance.score);
				}
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeLengthDelimited(Stream stream, ArcadeMachine.ScoreEntry instance)
			{
				byte[] bytes = ArcadeMachine.ScoreEntry.SerializeToBytes(instance);
				ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
				stream.Write(bytes, 0, (int)bytes.Length);
			}

			public static byte[] SerializeToBytes(ArcadeMachine.ScoreEntry instance)
			{
				byte[] array;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					ArcadeMachine.ScoreEntry.Serialize(memoryStream, instance);
					array = memoryStream.ToArray();
				}
				return array;
			}

			public void ToProto(Stream stream)
			{
				ArcadeMachine.ScoreEntry.Serialize(stream, this);
			}

			public byte[] ToProtoBytes()
			{
				return ArcadeMachine.ScoreEntry.SerializeToBytes(this);
			}

			public virtual void WriteToStream(Stream stream)
			{
				ArcadeMachine.ScoreEntry.Serialize(stream, this);
			}

			public virtual void WriteToStreamDelta(Stream stream, ArcadeMachine.ScoreEntry previous)
			{
				if (previous == null)
				{
					ArcadeMachine.ScoreEntry.Serialize(stream, this);
					return;
				}
				ArcadeMachine.ScoreEntry.SerializeDelta(stream, this, previous);
			}
		}
	}
}