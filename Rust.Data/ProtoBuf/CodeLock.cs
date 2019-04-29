using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProtoBuf
{
	public class CodeLock : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public CodeLock.Private pv;

		[NonSerialized]
		public bool hasCode;

		[NonSerialized]
		public bool hasGuestCode;

		public bool ShouldPool = true;

		private bool _disposed;

		public CodeLock()
		{
		}

		public CodeLock Copy()
		{
			CodeLock codeLock = Pool.Get<CodeLock>();
			this.CopyTo(codeLock);
			return codeLock;
		}

		public void CopyTo(CodeLock instance)
		{
			if (this.pv == null)
			{
				instance.pv = null;
			}
			else if (instance.pv != null)
			{
				this.pv.CopyTo(instance.pv);
			}
			else
			{
				instance.pv = this.pv.Copy();
			}
			instance.hasCode = this.hasCode;
			instance.hasGuestCode = this.hasGuestCode;
		}

		public static CodeLock Deserialize(Stream stream)
		{
			CodeLock codeLock = Pool.Get<CodeLock>();
			CodeLock.Deserialize(stream, codeLock, false);
			return codeLock;
		}

		public static CodeLock Deserialize(byte[] buffer)
		{
			CodeLock codeLock = Pool.Get<CodeLock>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				CodeLock.Deserialize(memoryStream, codeLock, false);
			}
			return codeLock;
		}

		public static CodeLock Deserialize(byte[] buffer, CodeLock instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				CodeLock.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static CodeLock Deserialize(Stream stream, CodeLock instance, bool isDelta)
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
					if (instance.pv != null)
					{
						CodeLock.Private.DeserializeLengthDelimited(stream, instance.pv, isDelta);
					}
					else
					{
						instance.pv = CodeLock.Private.DeserializeLengthDelimited(stream);
					}
				}
				else if (num == 16)
				{
					instance.hasCode = ProtocolParser.ReadBool(stream);
				}
				else if (num == 24)
				{
					instance.hasGuestCode = ProtocolParser.ReadBool(stream);
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

		public static CodeLock DeserializeLength(Stream stream, int length)
		{
			CodeLock codeLock = Pool.Get<CodeLock>();
			CodeLock.DeserializeLength(stream, length, codeLock, false);
			return codeLock;
		}

		public static CodeLock DeserializeLength(Stream stream, int length, CodeLock instance, bool isDelta)
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
					if (instance.pv != null)
					{
						CodeLock.Private.DeserializeLengthDelimited(stream, instance.pv, isDelta);
					}
					else
					{
						instance.pv = CodeLock.Private.DeserializeLengthDelimited(stream);
					}
				}
				else if (num == 16)
				{
					instance.hasCode = ProtocolParser.ReadBool(stream);
				}
				else if (num == 24)
				{
					instance.hasGuestCode = ProtocolParser.ReadBool(stream);
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

		public static CodeLock DeserializeLengthDelimited(Stream stream)
		{
			CodeLock codeLock = Pool.Get<CodeLock>();
			CodeLock.DeserializeLengthDelimited(stream, codeLock, false);
			return codeLock;
		}

		public static CodeLock DeserializeLengthDelimited(Stream stream, CodeLock instance, bool isDelta)
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
					if (instance.pv != null)
					{
						CodeLock.Private.DeserializeLengthDelimited(stream, instance.pv, isDelta);
					}
					else
					{
						instance.pv = CodeLock.Private.DeserializeLengthDelimited(stream);
					}
				}
				else if (num == 16)
				{
					instance.hasCode = ProtocolParser.ReadBool(stream);
				}
				else if (num == 24)
				{
					instance.hasGuestCode = ProtocolParser.ReadBool(stream);
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
			CodeLock.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			CodeLock.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(CodeLock instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			if (instance.pv != null)
			{
				instance.pv.ResetToPool();
				instance.pv = null;
			}
			instance.hasCode = false;
			instance.hasGuestCode = false;
			Pool.Free<CodeLock>(ref instance);
		}

		public void ResetToPool()
		{
			CodeLock.ResetToPool(this);
		}

		public static void Serialize(Stream stream, CodeLock instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.pv != null)
			{
				stream.WriteByte(10);
				memoryStream.SetLength((long)0);
				CodeLock.Private.Serialize(memoryStream, instance.pv);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			stream.WriteByte(16);
			ProtocolParser.WriteBool(stream, instance.hasCode);
			stream.WriteByte(24);
			ProtocolParser.WriteBool(stream, instance.hasGuestCode);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, CodeLock instance, CodeLock previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.pv != null)
			{
				stream.WriteByte(10);
				memoryStream.SetLength((long)0);
				CodeLock.Private.SerializeDelta(memoryStream, instance.pv, previous.pv);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			stream.WriteByte(16);
			ProtocolParser.WriteBool(stream, instance.hasCode);
			stream.WriteByte(24);
			ProtocolParser.WriteBool(stream, instance.hasGuestCode);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, CodeLock instance)
		{
			byte[] bytes = CodeLock.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(CodeLock instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				CodeLock.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			CodeLock.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return CodeLock.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			CodeLock.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, CodeLock previous)
		{
			if (previous == null)
			{
				CodeLock.Serialize(stream, this);
				return;
			}
			CodeLock.SerializeDelta(stream, this, previous);
		}

		public class Private : IDisposable, Pool.IPooled, IProto
		{
			[NonSerialized]
			public string code;

			[NonSerialized]
			public List<ulong> users;

			[NonSerialized]
			public string guestCode;

			[NonSerialized]
			public List<ulong> guestUsers;

			public bool ShouldPool;

			private bool _disposed;

			public Private()
			{
			}

			public CodeLock.Private Copy()
			{
				CodeLock.Private @private = Pool.Get<CodeLock.Private>();
				this.CopyTo(@private);
				return @private;
			}

			public void CopyTo(CodeLock.Private instance)
			{
				instance.code = this.code;
				throw new NotImplementedException();
			}

			public static CodeLock.Private Deserialize(Stream stream)
			{
				CodeLock.Private @private = Pool.Get<CodeLock.Private>();
				CodeLock.Private.Deserialize(stream, @private, false);
				return @private;
			}

			public static CodeLock.Private Deserialize(byte[] buffer)
			{
				CodeLock.Private @private = Pool.Get<CodeLock.Private>();
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					CodeLock.Private.Deserialize(memoryStream, @private, false);
				}
				return @private;
			}

			public static CodeLock.Private Deserialize(byte[] buffer, CodeLock.Private instance, bool isDelta = false)
			{
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					CodeLock.Private.Deserialize(memoryStream, instance, isDelta);
				}
				return instance;
			}

			public static CodeLock.Private Deserialize(Stream stream, CodeLock.Private instance, bool isDelta)
			{
				if (!isDelta)
				{
					if (instance.users == null)
					{
						instance.users = Pool.Get<List<ulong>>();
					}
					if (instance.guestUsers == null)
					{
						instance.guestUsers = Pool.Get<List<ulong>>();
					}
				}
				while (true)
				{
					int num = stream.ReadByte();
					if (num == -1)
					{
						break;
					}
					if (num <= 16)
					{
						if (num == 10)
						{
							instance.code = ProtocolParser.ReadString(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.users.Add(ProtocolParser.ReadUInt64(stream));
							continue;
						}
					}
					else if (num == 34)
					{
						instance.guestCode = ProtocolParser.ReadString(stream);
						continue;
					}
					else if (num == 40)
					{
						instance.guestUsers.Add(ProtocolParser.ReadUInt64(stream));
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

			public static CodeLock.Private DeserializeLength(Stream stream, int length)
			{
				CodeLock.Private @private = Pool.Get<CodeLock.Private>();
				CodeLock.Private.DeserializeLength(stream, length, @private, false);
				return @private;
			}

			public static CodeLock.Private DeserializeLength(Stream stream, int length, CodeLock.Private instance, bool isDelta)
			{
				if (!isDelta)
				{
					if (instance.users == null)
					{
						instance.users = Pool.Get<List<ulong>>();
					}
					if (instance.guestUsers == null)
					{
						instance.guestUsers = Pool.Get<List<ulong>>();
					}
				}
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
						if (num == 10)
						{
							instance.code = ProtocolParser.ReadString(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.users.Add(ProtocolParser.ReadUInt64(stream));
							continue;
						}
					}
					else if (num == 34)
					{
						instance.guestCode = ProtocolParser.ReadString(stream);
						continue;
					}
					else if (num == 40)
					{
						instance.guestUsers.Add(ProtocolParser.ReadUInt64(stream));
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

			public static CodeLock.Private DeserializeLengthDelimited(Stream stream)
			{
				CodeLock.Private @private = Pool.Get<CodeLock.Private>();
				CodeLock.Private.DeserializeLengthDelimited(stream, @private, false);
				return @private;
			}

			public static CodeLock.Private DeserializeLengthDelimited(Stream stream, CodeLock.Private instance, bool isDelta)
			{
				if (!isDelta)
				{
					if (instance.users == null)
					{
						instance.users = Pool.Get<List<ulong>>();
					}
					if (instance.guestUsers == null)
					{
						instance.guestUsers = Pool.Get<List<ulong>>();
					}
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
					if (num <= 16)
					{
						if (num == 10)
						{
							instance.code = ProtocolParser.ReadString(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.users.Add(ProtocolParser.ReadUInt64(stream));
							continue;
						}
					}
					else if (num == 34)
					{
						instance.guestCode = ProtocolParser.ReadString(stream);
						continue;
					}
					else if (num == 40)
					{
						instance.guestUsers.Add(ProtocolParser.ReadUInt64(stream));
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
				CodeLock.Private.Deserialize(stream, this, isDelta);
			}

			public virtual void LeavePool()
			{
				this._disposed = false;
			}

			public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
			{
				CodeLock.Private.DeserializeLength(stream, size, this, isDelta);
			}

			public static void ResetToPool(CodeLock.Private instance)
			{
				if (!instance.ShouldPool)
				{
					return;
				}
				instance.code = string.Empty;
				if (instance.users != null)
				{
					List<ulong> nums = instance.users;
					Pool.FreeList<ulong>(ref nums);
					instance.users = nums;
				}
				instance.guestCode = string.Empty;
				if (instance.guestUsers != null)
				{
					List<ulong> nums1 = instance.guestUsers;
					Pool.FreeList<ulong>(ref nums1);
					instance.guestUsers = nums1;
				}
				Pool.Free<CodeLock.Private>(ref instance);
			}

			public void ResetToPool()
			{
				CodeLock.Private.ResetToPool(this);
			}

			public static void Serialize(Stream stream, CodeLock.Private instance)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				if (instance.code != null)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteString(stream, instance.code);
				}
				if (instance.users != null)
				{
					for (int i = 0; i < instance.users.Count; i++)
					{
						ulong item = instance.users[i];
						stream.WriteByte(16);
						ProtocolParser.WriteUInt64(stream, item);
					}
				}
				if (instance.guestCode != null)
				{
					stream.WriteByte(34);
					ProtocolParser.WriteString(stream, instance.guestCode);
				}
				if (instance.guestUsers != null)
				{
					for (int j = 0; j < instance.guestUsers.Count; j++)
					{
						ulong num = instance.guestUsers[j];
						stream.WriteByte(40);
						ProtocolParser.WriteUInt64(stream, num);
					}
				}
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeDelta(Stream stream, CodeLock.Private instance, CodeLock.Private previous)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				if (instance.code != null && instance.code != previous.code)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteString(stream, instance.code);
				}
				if (instance.users != null)
				{
					for (int i = 0; i < instance.users.Count; i++)
					{
						ulong item = instance.users[i];
						stream.WriteByte(16);
						ProtocolParser.WriteUInt64(stream, item);
					}
				}
				if (instance.guestCode != null && instance.guestCode != previous.guestCode)
				{
					stream.WriteByte(34);
					ProtocolParser.WriteString(stream, instance.guestCode);
				}
				if (instance.guestUsers != null)
				{
					for (int j = 0; j < instance.guestUsers.Count; j++)
					{
						ulong num = instance.guestUsers[j];
						stream.WriteByte(40);
						ProtocolParser.WriteUInt64(stream, num);
					}
				}
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeLengthDelimited(Stream stream, CodeLock.Private instance)
			{
				byte[] bytes = CodeLock.Private.SerializeToBytes(instance);
				ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
				stream.Write(bytes, 0, (int)bytes.Length);
			}

			public static byte[] SerializeToBytes(CodeLock.Private instance)
			{
				byte[] array;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					CodeLock.Private.Serialize(memoryStream, instance);
					array = memoryStream.ToArray();
				}
				return array;
			}

			public void ToProto(Stream stream)
			{
				CodeLock.Private.Serialize(stream, this);
			}

			public byte[] ToProtoBytes()
			{
				return CodeLock.Private.SerializeToBytes(this);
			}

			public virtual void WriteToStream(Stream stream)
			{
				CodeLock.Private.Serialize(stream, this);
			}

			public virtual void WriteToStreamDelta(Stream stream, CodeLock.Private previous)
			{
				if (previous == null)
				{
					CodeLock.Private.Serialize(stream, this);
					return;
				}
				CodeLock.Private.SerializeDelta(stream, this, previous);
			}
		}
	}
}