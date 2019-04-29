using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProtoBuf
{
	public class EggHunt : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public List<EggHunt.EggHunter> hunters;

		public bool ShouldPool = true;

		private bool _disposed;

		public EggHunt()
		{
		}

		public EggHunt Copy()
		{
			EggHunt eggHunt = Pool.Get<EggHunt>();
			this.CopyTo(eggHunt);
			return eggHunt;
		}

		public void CopyTo(EggHunt instance)
		{
			throw new NotImplementedException();
		}

		public static EggHunt Deserialize(Stream stream)
		{
			EggHunt eggHunt = Pool.Get<EggHunt>();
			EggHunt.Deserialize(stream, eggHunt, false);
			return eggHunt;
		}

		public static EggHunt Deserialize(byte[] buffer)
		{
			EggHunt eggHunt = Pool.Get<EggHunt>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				EggHunt.Deserialize(memoryStream, eggHunt, false);
			}
			return eggHunt;
		}

		public static EggHunt Deserialize(byte[] buffer, EggHunt instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				EggHunt.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static EggHunt Deserialize(Stream stream, EggHunt instance, bool isDelta)
		{
			if (!isDelta && instance.hunters == null)
			{
				instance.hunters = Pool.Get<List<EggHunt.EggHunter>>();
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
					instance.hunters.Add(EggHunt.EggHunter.DeserializeLengthDelimited(stream));
				}
			}
			return instance;
		}

		public static EggHunt DeserializeLength(Stream stream, int length)
		{
			EggHunt eggHunt = Pool.Get<EggHunt>();
			EggHunt.DeserializeLength(stream, length, eggHunt, false);
			return eggHunt;
		}

		public static EggHunt DeserializeLength(Stream stream, int length, EggHunt instance, bool isDelta)
		{
			if (!isDelta && instance.hunters == null)
			{
				instance.hunters = Pool.Get<List<EggHunt.EggHunter>>();
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
					instance.hunters.Add(EggHunt.EggHunter.DeserializeLengthDelimited(stream));
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static EggHunt DeserializeLengthDelimited(Stream stream)
		{
			EggHunt eggHunt = Pool.Get<EggHunt>();
			EggHunt.DeserializeLengthDelimited(stream, eggHunt, false);
			return eggHunt;
		}

		public static EggHunt DeserializeLengthDelimited(Stream stream, EggHunt instance, bool isDelta)
		{
			if (!isDelta && instance.hunters == null)
			{
				instance.hunters = Pool.Get<List<EggHunt.EggHunter>>();
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
					instance.hunters.Add(EggHunt.EggHunter.DeserializeLengthDelimited(stream));
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
			EggHunt.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			EggHunt.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(EggHunt instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			if (instance.hunters != null)
			{
				for (int i = 0; i < instance.hunters.Count; i++)
				{
					if (instance.hunters[i] != null)
					{
						instance.hunters[i].ResetToPool();
						instance.hunters[i] = null;
					}
				}
				List<EggHunt.EggHunter> eggHunters = instance.hunters;
				Pool.FreeList<EggHunt.EggHunter>(ref eggHunters);
				instance.hunters = eggHunters;
			}
			Pool.Free<EggHunt>(ref instance);
		}

		public void ResetToPool()
		{
			EggHunt.ResetToPool(this);
		}

		public static void Serialize(Stream stream, EggHunt instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.hunters != null)
			{
				for (int i = 0; i < instance.hunters.Count; i++)
				{
					EggHunt.EggHunter item = instance.hunters[i];
					stream.WriteByte(10);
					memoryStream.SetLength((long)0);
					EggHunt.EggHunter.Serialize(memoryStream, item);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, EggHunt instance, EggHunt previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.hunters != null)
			{
				for (int i = 0; i < instance.hunters.Count; i++)
				{
					EggHunt.EggHunter item = instance.hunters[i];
					stream.WriteByte(10);
					memoryStream.SetLength((long)0);
					EggHunt.EggHunter.SerializeDelta(memoryStream, item, item);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, EggHunt instance)
		{
			byte[] bytes = EggHunt.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(EggHunt instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				EggHunt.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			EggHunt.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return EggHunt.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			EggHunt.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, EggHunt previous)
		{
			if (previous == null)
			{
				EggHunt.Serialize(stream, this);
				return;
			}
			EggHunt.SerializeDelta(stream, this, previous);
		}

		public class EggHunter : IDisposable, Pool.IPooled, IProto
		{
			[NonSerialized]
			public string displayName;

			[NonSerialized]
			public int numEggs;

			public bool ShouldPool;

			private bool _disposed;

			public EggHunter()
			{
			}

			public EggHunt.EggHunter Copy()
			{
				EggHunt.EggHunter eggHunter = Pool.Get<EggHunt.EggHunter>();
				this.CopyTo(eggHunter);
				return eggHunter;
			}

			public void CopyTo(EggHunt.EggHunter instance)
			{
				instance.displayName = this.displayName;
				instance.numEggs = this.numEggs;
			}

			public static EggHunt.EggHunter Deserialize(Stream stream)
			{
				EggHunt.EggHunter eggHunter = Pool.Get<EggHunt.EggHunter>();
				EggHunt.EggHunter.Deserialize(stream, eggHunter, false);
				return eggHunter;
			}

			public static EggHunt.EggHunter Deserialize(byte[] buffer)
			{
				EggHunt.EggHunter eggHunter = Pool.Get<EggHunt.EggHunter>();
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					EggHunt.EggHunter.Deserialize(memoryStream, eggHunter, false);
				}
				return eggHunter;
			}

			public static EggHunt.EggHunter Deserialize(byte[] buffer, EggHunt.EggHunter instance, bool isDelta = false)
			{
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					EggHunt.EggHunter.Deserialize(memoryStream, instance, isDelta);
				}
				return instance;
			}

			public static EggHunt.EggHunter Deserialize(Stream stream, EggHunt.EggHunter instance, bool isDelta)
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
						instance.displayName = ProtocolParser.ReadString(stream);
					}
					else if (num == 16)
					{
						instance.numEggs = (int)ProtocolParser.ReadUInt64(stream);
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

			public static EggHunt.EggHunter DeserializeLength(Stream stream, int length)
			{
				EggHunt.EggHunter eggHunter = Pool.Get<EggHunt.EggHunter>();
				EggHunt.EggHunter.DeserializeLength(stream, length, eggHunter, false);
				return eggHunter;
			}

			public static EggHunt.EggHunter DeserializeLength(Stream stream, int length, EggHunt.EggHunter instance, bool isDelta)
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
						instance.displayName = ProtocolParser.ReadString(stream);
					}
					else if (num == 16)
					{
						instance.numEggs = (int)ProtocolParser.ReadUInt64(stream);
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

			public static EggHunt.EggHunter DeserializeLengthDelimited(Stream stream)
			{
				EggHunt.EggHunter eggHunter = Pool.Get<EggHunt.EggHunter>();
				EggHunt.EggHunter.DeserializeLengthDelimited(stream, eggHunter, false);
				return eggHunter;
			}

			public static EggHunt.EggHunter DeserializeLengthDelimited(Stream stream, EggHunt.EggHunter instance, bool isDelta)
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
						instance.displayName = ProtocolParser.ReadString(stream);
					}
					else if (num == 16)
					{
						instance.numEggs = (int)ProtocolParser.ReadUInt64(stream);
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
				EggHunt.EggHunter.Deserialize(stream, this, isDelta);
			}

			public virtual void LeavePool()
			{
				this._disposed = false;
			}

			public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
			{
				EggHunt.EggHunter.DeserializeLength(stream, size, this, isDelta);
			}

			public static void ResetToPool(EggHunt.EggHunter instance)
			{
				if (!instance.ShouldPool)
				{
					return;
				}
				instance.displayName = string.Empty;
				instance.numEggs = 0;
				Pool.Free<EggHunt.EggHunter>(ref instance);
			}

			public void ResetToPool()
			{
				EggHunt.EggHunter.ResetToPool(this);
			}

			public static void Serialize(Stream stream, EggHunt.EggHunter instance)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				if (instance.displayName != null)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteString(stream, instance.displayName);
				}
				stream.WriteByte(16);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.numEggs);
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeDelta(Stream stream, EggHunt.EggHunter instance, EggHunt.EggHunter previous)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				if (instance.displayName != null && instance.displayName != previous.displayName)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteString(stream, instance.displayName);
				}
				if (instance.numEggs != previous.numEggs)
				{
					stream.WriteByte(16);
					ProtocolParser.WriteUInt64(stream, (ulong)instance.numEggs);
				}
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeLengthDelimited(Stream stream, EggHunt.EggHunter instance)
			{
				byte[] bytes = EggHunt.EggHunter.SerializeToBytes(instance);
				ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
				stream.Write(bytes, 0, (int)bytes.Length);
			}

			public static byte[] SerializeToBytes(EggHunt.EggHunter instance)
			{
				byte[] array;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					EggHunt.EggHunter.Serialize(memoryStream, instance);
					array = memoryStream.ToArray();
				}
				return array;
			}

			public void ToProto(Stream stream)
			{
				EggHunt.EggHunter.Serialize(stream, this);
			}

			public byte[] ToProtoBytes()
			{
				return EggHunt.EggHunter.SerializeToBytes(this);
			}

			public virtual void WriteToStream(Stream stream)
			{
				EggHunt.EggHunter.Serialize(stream, this);
			}

			public virtual void WriteToStreamDelta(Stream stream, EggHunt.EggHunter previous)
			{
				if (previous == null)
				{
					EggHunt.EggHunter.Serialize(stream, this);
					return;
				}
				EggHunt.EggHunter.SerializeDelta(stream, this, previous);
			}
		}
	}
}