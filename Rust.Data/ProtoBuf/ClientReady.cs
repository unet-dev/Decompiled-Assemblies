using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProtoBuf
{
	public class ClientReady : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public List<ClientReady.ClientInfo> clientInfo;

		public bool ShouldPool = true;

		private bool _disposed;

		public ClientReady()
		{
		}

		public ClientReady Copy()
		{
			ClientReady clientReady = Pool.Get<ClientReady>();
			this.CopyTo(clientReady);
			return clientReady;
		}

		public void CopyTo(ClientReady instance)
		{
			throw new NotImplementedException();
		}

		public static ClientReady Deserialize(Stream stream)
		{
			ClientReady clientReady = Pool.Get<ClientReady>();
			ClientReady.Deserialize(stream, clientReady, false);
			return clientReady;
		}

		public static ClientReady Deserialize(byte[] buffer)
		{
			ClientReady clientReady = Pool.Get<ClientReady>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				ClientReady.Deserialize(memoryStream, clientReady, false);
			}
			return clientReady;
		}

		public static ClientReady Deserialize(byte[] buffer, ClientReady instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				ClientReady.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static ClientReady Deserialize(Stream stream, ClientReady instance, bool isDelta)
		{
			if (!isDelta && instance.clientInfo == null)
			{
				instance.clientInfo = Pool.Get<List<ClientReady.ClientInfo>>();
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
					instance.clientInfo.Add(ClientReady.ClientInfo.DeserializeLengthDelimited(stream));
				}
			}
			return instance;
		}

		public static ClientReady DeserializeLength(Stream stream, int length)
		{
			ClientReady clientReady = Pool.Get<ClientReady>();
			ClientReady.DeserializeLength(stream, length, clientReady, false);
			return clientReady;
		}

		public static ClientReady DeserializeLength(Stream stream, int length, ClientReady instance, bool isDelta)
		{
			if (!isDelta && instance.clientInfo == null)
			{
				instance.clientInfo = Pool.Get<List<ClientReady.ClientInfo>>();
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
					instance.clientInfo.Add(ClientReady.ClientInfo.DeserializeLengthDelimited(stream));
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static ClientReady DeserializeLengthDelimited(Stream stream)
		{
			ClientReady clientReady = Pool.Get<ClientReady>();
			ClientReady.DeserializeLengthDelimited(stream, clientReady, false);
			return clientReady;
		}

		public static ClientReady DeserializeLengthDelimited(Stream stream, ClientReady instance, bool isDelta)
		{
			if (!isDelta && instance.clientInfo == null)
			{
				instance.clientInfo = Pool.Get<List<ClientReady.ClientInfo>>();
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
					instance.clientInfo.Add(ClientReady.ClientInfo.DeserializeLengthDelimited(stream));
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
			ClientReady.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			ClientReady.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(ClientReady instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			if (instance.clientInfo != null)
			{
				for (int i = 0; i < instance.clientInfo.Count; i++)
				{
					if (instance.clientInfo[i] != null)
					{
						instance.clientInfo[i].ResetToPool();
						instance.clientInfo[i] = null;
					}
				}
				List<ClientReady.ClientInfo> clientInfos = instance.clientInfo;
				Pool.FreeList<ClientReady.ClientInfo>(ref clientInfos);
				instance.clientInfo = clientInfos;
			}
			Pool.Free<ClientReady>(ref instance);
		}

		public void ResetToPool()
		{
			ClientReady.ResetToPool(this);
		}

		public static void Serialize(Stream stream, ClientReady instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.clientInfo != null)
			{
				for (int i = 0; i < instance.clientInfo.Count; i++)
				{
					ClientReady.ClientInfo item = instance.clientInfo[i];
					stream.WriteByte(10);
					memoryStream.SetLength((long)0);
					ClientReady.ClientInfo.Serialize(memoryStream, item);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, ClientReady instance, ClientReady previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.clientInfo != null)
			{
				for (int i = 0; i < instance.clientInfo.Count; i++)
				{
					ClientReady.ClientInfo item = instance.clientInfo[i];
					stream.WriteByte(10);
					memoryStream.SetLength((long)0);
					ClientReady.ClientInfo.SerializeDelta(memoryStream, item, item);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, ClientReady instance)
		{
			byte[] bytes = ClientReady.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(ClientReady instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				ClientReady.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			ClientReady.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return ClientReady.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			ClientReady.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, ClientReady previous)
		{
			if (previous == null)
			{
				ClientReady.Serialize(stream, this);
				return;
			}
			ClientReady.SerializeDelta(stream, this, previous);
		}

		public class ClientInfo : IDisposable, Pool.IPooled, IProto
		{
			[NonSerialized]
			public string name;

			[NonSerialized]
			public string @value;

			public bool ShouldPool;

			private bool _disposed;

			public ClientInfo()
			{
			}

			public ClientReady.ClientInfo Copy()
			{
				ClientReady.ClientInfo clientInfo = Pool.Get<ClientReady.ClientInfo>();
				this.CopyTo(clientInfo);
				return clientInfo;
			}

			public void CopyTo(ClientReady.ClientInfo instance)
			{
				instance.name = this.name;
				instance.@value = this.@value;
			}

			public static ClientReady.ClientInfo Deserialize(Stream stream)
			{
				ClientReady.ClientInfo clientInfo = Pool.Get<ClientReady.ClientInfo>();
				ClientReady.ClientInfo.Deserialize(stream, clientInfo, false);
				return clientInfo;
			}

			public static ClientReady.ClientInfo Deserialize(byte[] buffer)
			{
				ClientReady.ClientInfo clientInfo = Pool.Get<ClientReady.ClientInfo>();
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					ClientReady.ClientInfo.Deserialize(memoryStream, clientInfo, false);
				}
				return clientInfo;
			}

			public static ClientReady.ClientInfo Deserialize(byte[] buffer, ClientReady.ClientInfo instance, bool isDelta = false)
			{
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					ClientReady.ClientInfo.Deserialize(memoryStream, instance, isDelta);
				}
				return instance;
			}

			public static ClientReady.ClientInfo Deserialize(Stream stream, ClientReady.ClientInfo instance, bool isDelta)
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
						instance.name = ProtocolParser.ReadString(stream);
					}
					else if (num == 18)
					{
						instance.@value = ProtocolParser.ReadString(stream);
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

			public static ClientReady.ClientInfo DeserializeLength(Stream stream, int length)
			{
				ClientReady.ClientInfo clientInfo = Pool.Get<ClientReady.ClientInfo>();
				ClientReady.ClientInfo.DeserializeLength(stream, length, clientInfo, false);
				return clientInfo;
			}

			public static ClientReady.ClientInfo DeserializeLength(Stream stream, int length, ClientReady.ClientInfo instance, bool isDelta)
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
						instance.name = ProtocolParser.ReadString(stream);
					}
					else if (num == 18)
					{
						instance.@value = ProtocolParser.ReadString(stream);
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

			public static ClientReady.ClientInfo DeserializeLengthDelimited(Stream stream)
			{
				ClientReady.ClientInfo clientInfo = Pool.Get<ClientReady.ClientInfo>();
				ClientReady.ClientInfo.DeserializeLengthDelimited(stream, clientInfo, false);
				return clientInfo;
			}

			public static ClientReady.ClientInfo DeserializeLengthDelimited(Stream stream, ClientReady.ClientInfo instance, bool isDelta)
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
						instance.name = ProtocolParser.ReadString(stream);
					}
					else if (num == 18)
					{
						instance.@value = ProtocolParser.ReadString(stream);
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
				ClientReady.ClientInfo.Deserialize(stream, this, isDelta);
			}

			public virtual void LeavePool()
			{
				this._disposed = false;
			}

			public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
			{
				ClientReady.ClientInfo.DeserializeLength(stream, size, this, isDelta);
			}

			public static void ResetToPool(ClientReady.ClientInfo instance)
			{
				if (!instance.ShouldPool)
				{
					return;
				}
				instance.name = string.Empty;
				instance.@value = string.Empty;
				Pool.Free<ClientReady.ClientInfo>(ref instance);
			}

			public void ResetToPool()
			{
				ClientReady.ClientInfo.ResetToPool(this);
			}

			public static void Serialize(Stream stream, ClientReady.ClientInfo instance)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				if (instance.name == null)
				{
					throw new ArgumentNullException("name", "Required by proto specification.");
				}
				stream.WriteByte(10);
				ProtocolParser.WriteString(stream, instance.name);
				if (instance.@value == null)
				{
					throw new ArgumentNullException("value", "Required by proto specification.");
				}
				stream.WriteByte(18);
				ProtocolParser.WriteString(stream, instance.@value);
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeDelta(Stream stream, ClientReady.ClientInfo instance, ClientReady.ClientInfo previous)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				if (instance.name != previous.name)
				{
					if (instance.name == null)
					{
						throw new ArgumentNullException("name", "Required by proto specification.");
					}
					stream.WriteByte(10);
					ProtocolParser.WriteString(stream, instance.name);
				}
				if (instance.@value != previous.@value)
				{
					if (instance.@value == null)
					{
						throw new ArgumentNullException("value", "Required by proto specification.");
					}
					stream.WriteByte(18);
					ProtocolParser.WriteString(stream, instance.@value);
				}
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeLengthDelimited(Stream stream, ClientReady.ClientInfo instance)
			{
				byte[] bytes = ClientReady.ClientInfo.SerializeToBytes(instance);
				ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
				stream.Write(bytes, 0, (int)bytes.Length);
			}

			public static byte[] SerializeToBytes(ClientReady.ClientInfo instance)
			{
				byte[] array;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					ClientReady.ClientInfo.Serialize(memoryStream, instance);
					array = memoryStream.ToArray();
				}
				return array;
			}

			public void ToProto(Stream stream)
			{
				ClientReady.ClientInfo.Serialize(stream, this);
			}

			public byte[] ToProtoBytes()
			{
				return ClientReady.ClientInfo.SerializeToBytes(this);
			}

			public virtual void WriteToStream(Stream stream)
			{
				ClientReady.ClientInfo.Serialize(stream, this);
			}

			public virtual void WriteToStreamDelta(Stream stream, ClientReady.ClientInfo previous)
			{
				if (previous == null)
				{
					ClientReady.ClientInfo.Serialize(stream, this);
					return;
				}
				ClientReady.ClientInfo.SerializeDelta(stream, this, previous);
			}
		}
	}
}