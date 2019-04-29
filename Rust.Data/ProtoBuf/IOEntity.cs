using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ProtoBuf
{
	public class IOEntity : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public List<IOEntity.IOConnection> inputs;

		[NonSerialized]
		public List<IOEntity.IOConnection> outputs;

		[NonSerialized]
		public uint genericEntRef1;

		[NonSerialized]
		public uint genericEntRef2;

		[NonSerialized]
		public uint genericEntRef3;

		[NonSerialized]
		public int genericInt1;

		[NonSerialized]
		public int genericInt2;

		[NonSerialized]
		public float genericFloat1;

		[NonSerialized]
		public float genericFloat2;

		[NonSerialized]
		public int genericInt3;

		public bool ShouldPool = true;

		private bool _disposed;

		public IOEntity()
		{
		}

		public IOEntity Copy()
		{
			IOEntity oEntity = Pool.Get<IOEntity>();
			this.CopyTo(oEntity);
			return oEntity;
		}

		public void CopyTo(IOEntity instance)
		{
			throw new NotImplementedException();
		}

		public static IOEntity Deserialize(Stream stream)
		{
			IOEntity oEntity = Pool.Get<IOEntity>();
			IOEntity.Deserialize(stream, oEntity, false);
			return oEntity;
		}

		public static IOEntity Deserialize(byte[] buffer)
		{
			IOEntity oEntity = Pool.Get<IOEntity>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				IOEntity.Deserialize(memoryStream, oEntity, false);
			}
			return oEntity;
		}

		public static IOEntity Deserialize(byte[] buffer, IOEntity instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				IOEntity.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static IOEntity Deserialize(Stream stream, IOEntity instance, bool isDelta)
		{
			if (!isDelta)
			{
				if (instance.inputs == null)
				{
					instance.inputs = Pool.Get<List<IOEntity.IOConnection>>();
				}
				if (instance.outputs == null)
				{
					instance.outputs = Pool.Get<List<IOEntity.IOConnection>>();
				}
			}
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num <= 40)
				{
					if (num <= 18)
					{
						if (num == 10)
						{
							instance.inputs.Add(IOEntity.IOConnection.DeserializeLengthDelimited(stream));
							continue;
						}
						else if (num == 18)
						{
							instance.outputs.Add(IOEntity.IOConnection.DeserializeLengthDelimited(stream));
							continue;
						}
					}
					else if (num == 24)
					{
						instance.genericEntRef1 = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 32)
					{
						instance.genericEntRef2 = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 40)
					{
						instance.genericEntRef3 = ProtocolParser.ReadUInt32(stream);
						continue;
					}
				}
				else if (num <= 56)
				{
					if (num == 48)
					{
						instance.genericInt1 = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 56)
					{
						instance.genericInt2 = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
				}
				else if (num == 69)
				{
					instance.genericFloat1 = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 77)
				{
					instance.genericFloat2 = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 80)
				{
					instance.genericInt3 = (int)ProtocolParser.ReadUInt64(stream);
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

		public static IOEntity DeserializeLength(Stream stream, int length)
		{
			IOEntity oEntity = Pool.Get<IOEntity>();
			IOEntity.DeserializeLength(stream, length, oEntity, false);
			return oEntity;
		}

		public static IOEntity DeserializeLength(Stream stream, int length, IOEntity instance, bool isDelta)
		{
			if (!isDelta)
			{
				if (instance.inputs == null)
				{
					instance.inputs = Pool.Get<List<IOEntity.IOConnection>>();
				}
				if (instance.outputs == null)
				{
					instance.outputs = Pool.Get<List<IOEntity.IOConnection>>();
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
				if (num <= 40)
				{
					if (num <= 18)
					{
						if (num == 10)
						{
							instance.inputs.Add(IOEntity.IOConnection.DeserializeLengthDelimited(stream));
							continue;
						}
						else if (num == 18)
						{
							instance.outputs.Add(IOEntity.IOConnection.DeserializeLengthDelimited(stream));
							continue;
						}
					}
					else if (num == 24)
					{
						instance.genericEntRef1 = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 32)
					{
						instance.genericEntRef2 = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 40)
					{
						instance.genericEntRef3 = ProtocolParser.ReadUInt32(stream);
						continue;
					}
				}
				else if (num <= 56)
				{
					if (num == 48)
					{
						instance.genericInt1 = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 56)
					{
						instance.genericInt2 = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
				}
				else if (num == 69)
				{
					instance.genericFloat1 = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 77)
				{
					instance.genericFloat2 = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 80)
				{
					instance.genericInt3 = (int)ProtocolParser.ReadUInt64(stream);
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

		public static IOEntity DeserializeLengthDelimited(Stream stream)
		{
			IOEntity oEntity = Pool.Get<IOEntity>();
			IOEntity.DeserializeLengthDelimited(stream, oEntity, false);
			return oEntity;
		}

		public static IOEntity DeserializeLengthDelimited(Stream stream, IOEntity instance, bool isDelta)
		{
			if (!isDelta)
			{
				if (instance.inputs == null)
				{
					instance.inputs = Pool.Get<List<IOEntity.IOConnection>>();
				}
				if (instance.outputs == null)
				{
					instance.outputs = Pool.Get<List<IOEntity.IOConnection>>();
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
				if (num <= 40)
				{
					if (num <= 18)
					{
						if (num == 10)
						{
							instance.inputs.Add(IOEntity.IOConnection.DeserializeLengthDelimited(stream));
							continue;
						}
						else if (num == 18)
						{
							instance.outputs.Add(IOEntity.IOConnection.DeserializeLengthDelimited(stream));
							continue;
						}
					}
					else if (num == 24)
					{
						instance.genericEntRef1 = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 32)
					{
						instance.genericEntRef2 = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 40)
					{
						instance.genericEntRef3 = ProtocolParser.ReadUInt32(stream);
						continue;
					}
				}
				else if (num <= 56)
				{
					if (num == 48)
					{
						instance.genericInt1 = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 56)
					{
						instance.genericInt2 = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
				}
				else if (num == 69)
				{
					instance.genericFloat1 = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 77)
				{
					instance.genericFloat2 = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 80)
				{
					instance.genericInt3 = (int)ProtocolParser.ReadUInt64(stream);
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
			IOEntity.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			IOEntity.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(IOEntity instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			if (instance.inputs != null)
			{
				for (int i = 0; i < instance.inputs.Count; i++)
				{
					if (instance.inputs[i] != null)
					{
						instance.inputs[i].ResetToPool();
						instance.inputs[i] = null;
					}
				}
				List<IOEntity.IOConnection> oConnections = instance.inputs;
				Pool.FreeList<IOEntity.IOConnection>(ref oConnections);
				instance.inputs = oConnections;
			}
			if (instance.outputs != null)
			{
				for (int j = 0; j < instance.outputs.Count; j++)
				{
					if (instance.outputs[j] != null)
					{
						instance.outputs[j].ResetToPool();
						instance.outputs[j] = null;
					}
				}
				List<IOEntity.IOConnection> oConnections1 = instance.outputs;
				Pool.FreeList<IOEntity.IOConnection>(ref oConnections1);
				instance.outputs = oConnections1;
			}
			instance.genericEntRef1 = 0;
			instance.genericEntRef2 = 0;
			instance.genericEntRef3 = 0;
			instance.genericInt1 = 0;
			instance.genericInt2 = 0;
			instance.genericFloat1 = 0f;
			instance.genericFloat2 = 0f;
			instance.genericInt3 = 0;
			Pool.Free<IOEntity>(ref instance);
		}

		public void ResetToPool()
		{
			IOEntity.ResetToPool(this);
		}

		public static void Serialize(Stream stream, IOEntity instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.inputs != null)
			{
				for (int i = 0; i < instance.inputs.Count; i++)
				{
					IOEntity.IOConnection item = instance.inputs[i];
					stream.WriteByte(10);
					memoryStream.SetLength((long)0);
					IOEntity.IOConnection.Serialize(memoryStream, item);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			if (instance.outputs != null)
			{
				for (int j = 0; j < instance.outputs.Count; j++)
				{
					IOEntity.IOConnection oConnection = instance.outputs[j];
					stream.WriteByte(18);
					memoryStream.SetLength((long)0);
					IOEntity.IOConnection.Serialize(memoryStream, oConnection);
					uint num = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, num);
					stream.Write(memoryStream.GetBuffer(), 0, (int)num);
				}
			}
			stream.WriteByte(24);
			ProtocolParser.WriteUInt32(stream, instance.genericEntRef1);
			stream.WriteByte(32);
			ProtocolParser.WriteUInt32(stream, instance.genericEntRef2);
			stream.WriteByte(40);
			ProtocolParser.WriteUInt32(stream, instance.genericEntRef3);
			stream.WriteByte(48);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.genericInt1);
			stream.WriteByte(56);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.genericInt2);
			stream.WriteByte(69);
			ProtocolParser.WriteSingle(stream, instance.genericFloat1);
			stream.WriteByte(77);
			ProtocolParser.WriteSingle(stream, instance.genericFloat2);
			stream.WriteByte(80);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.genericInt3);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, IOEntity instance, IOEntity previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.inputs != null)
			{
				for (int i = 0; i < instance.inputs.Count; i++)
				{
					IOEntity.IOConnection item = instance.inputs[i];
					stream.WriteByte(10);
					memoryStream.SetLength((long)0);
					IOEntity.IOConnection.SerializeDelta(memoryStream, item, item);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			if (instance.outputs != null)
			{
				for (int j = 0; j < instance.outputs.Count; j++)
				{
					IOEntity.IOConnection oConnection = instance.outputs[j];
					stream.WriteByte(18);
					memoryStream.SetLength((long)0);
					IOEntity.IOConnection.SerializeDelta(memoryStream, oConnection, oConnection);
					uint num = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, num);
					stream.Write(memoryStream.GetBuffer(), 0, (int)num);
				}
			}
			if (instance.genericEntRef1 != previous.genericEntRef1)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt32(stream, instance.genericEntRef1);
			}
			if (instance.genericEntRef2 != previous.genericEntRef2)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteUInt32(stream, instance.genericEntRef2);
			}
			if (instance.genericEntRef3 != previous.genericEntRef3)
			{
				stream.WriteByte(40);
				ProtocolParser.WriteUInt32(stream, instance.genericEntRef3);
			}
			if (instance.genericInt1 != previous.genericInt1)
			{
				stream.WriteByte(48);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.genericInt1);
			}
			if (instance.genericInt2 != previous.genericInt2)
			{
				stream.WriteByte(56);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.genericInt2);
			}
			if (instance.genericFloat1 != previous.genericFloat1)
			{
				stream.WriteByte(69);
				ProtocolParser.WriteSingle(stream, instance.genericFloat1);
			}
			if (instance.genericFloat2 != previous.genericFloat2)
			{
				stream.WriteByte(77);
				ProtocolParser.WriteSingle(stream, instance.genericFloat2);
			}
			if (instance.genericInt3 != previous.genericInt3)
			{
				stream.WriteByte(80);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.genericInt3);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, IOEntity instance)
		{
			byte[] bytes = IOEntity.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(IOEntity instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				IOEntity.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			IOEntity.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return IOEntity.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			IOEntity.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, IOEntity previous)
		{
			if (previous == null)
			{
				IOEntity.Serialize(stream, this);
				return;
			}
			IOEntity.SerializeDelta(stream, this, previous);
		}

		public class IOConnection : IDisposable, Pool.IPooled, IProto
		{
			[NonSerialized]
			public string niceName;

			[NonSerialized]
			public int type;

			[NonSerialized]
			public uint connectedID;

			[NonSerialized]
			public int connectedToSlot;

			[NonSerialized]
			public bool inUse;

			[NonSerialized]
			public List<IOEntity.IOConnection.LineVec> linePointList;

			public bool ShouldPool;

			private bool _disposed;

			public IOConnection()
			{
			}

			public IOEntity.IOConnection Copy()
			{
				IOEntity.IOConnection oConnection = Pool.Get<IOEntity.IOConnection>();
				this.CopyTo(oConnection);
				return oConnection;
			}

			public void CopyTo(IOEntity.IOConnection instance)
			{
				instance.niceName = this.niceName;
				instance.type = this.type;
				instance.connectedID = this.connectedID;
				instance.connectedToSlot = this.connectedToSlot;
				instance.inUse = this.inUse;
				throw new NotImplementedException();
			}

			public static IOEntity.IOConnection Deserialize(Stream stream)
			{
				IOEntity.IOConnection oConnection = Pool.Get<IOEntity.IOConnection>();
				IOEntity.IOConnection.Deserialize(stream, oConnection, false);
				return oConnection;
			}

			public static IOEntity.IOConnection Deserialize(byte[] buffer)
			{
				IOEntity.IOConnection oConnection = Pool.Get<IOEntity.IOConnection>();
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					IOEntity.IOConnection.Deserialize(memoryStream, oConnection, false);
				}
				return oConnection;
			}

			public static IOEntity.IOConnection Deserialize(byte[] buffer, IOEntity.IOConnection instance, bool isDelta = false)
			{
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					IOEntity.IOConnection.Deserialize(memoryStream, instance, isDelta);
				}
				return instance;
			}

			public static IOEntity.IOConnection Deserialize(Stream stream, IOEntity.IOConnection instance, bool isDelta)
			{
				if (!isDelta && instance.linePointList == null)
				{
					instance.linePointList = Pool.Get<List<IOEntity.IOConnection.LineVec>>();
				}
				while (true)
				{
					int num = stream.ReadByte();
					if (num == -1)
					{
						break;
					}
					if (num <= 24)
					{
						if (num == 10)
						{
							instance.niceName = ProtocolParser.ReadString(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.type = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
						else if (num == 24)
						{
							instance.connectedID = ProtocolParser.ReadUInt32(stream);
							continue;
						}
					}
					else if (num == 32)
					{
						instance.connectedToSlot = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 40)
					{
						instance.inUse = ProtocolParser.ReadBool(stream);
						continue;
					}
					else if (num == 50)
					{
						instance.linePointList.Add(IOEntity.IOConnection.LineVec.DeserializeLengthDelimited(stream));
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

			public static IOEntity.IOConnection DeserializeLength(Stream stream, int length)
			{
				IOEntity.IOConnection oConnection = Pool.Get<IOEntity.IOConnection>();
				IOEntity.IOConnection.DeserializeLength(stream, length, oConnection, false);
				return oConnection;
			}

			public static IOEntity.IOConnection DeserializeLength(Stream stream, int length, IOEntity.IOConnection instance, bool isDelta)
			{
				if (!isDelta && instance.linePointList == null)
				{
					instance.linePointList = Pool.Get<List<IOEntity.IOConnection.LineVec>>();
				}
				long position = stream.Position + (long)length;
				while (stream.Position < position)
				{
					int num = stream.ReadByte();
					if (num == -1)
					{
						throw new EndOfStreamException();
					}
					if (num <= 24)
					{
						if (num == 10)
						{
							instance.niceName = ProtocolParser.ReadString(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.type = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
						else if (num == 24)
						{
							instance.connectedID = ProtocolParser.ReadUInt32(stream);
							continue;
						}
					}
					else if (num == 32)
					{
						instance.connectedToSlot = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 40)
					{
						instance.inUse = ProtocolParser.ReadBool(stream);
						continue;
					}
					else if (num == 50)
					{
						instance.linePointList.Add(IOEntity.IOConnection.LineVec.DeserializeLengthDelimited(stream));
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

			public static IOEntity.IOConnection DeserializeLengthDelimited(Stream stream)
			{
				IOEntity.IOConnection oConnection = Pool.Get<IOEntity.IOConnection>();
				IOEntity.IOConnection.DeserializeLengthDelimited(stream, oConnection, false);
				return oConnection;
			}

			public static IOEntity.IOConnection DeserializeLengthDelimited(Stream stream, IOEntity.IOConnection instance, bool isDelta)
			{
				if (!isDelta && instance.linePointList == null)
				{
					instance.linePointList = Pool.Get<List<IOEntity.IOConnection.LineVec>>();
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
					if (num <= 24)
					{
						if (num == 10)
						{
							instance.niceName = ProtocolParser.ReadString(stream);
							continue;
						}
						else if (num == 16)
						{
							instance.type = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
						else if (num == 24)
						{
							instance.connectedID = ProtocolParser.ReadUInt32(stream);
							continue;
						}
					}
					else if (num == 32)
					{
						instance.connectedToSlot = (int)ProtocolParser.ReadUInt64(stream);
						continue;
					}
					else if (num == 40)
					{
						instance.inUse = ProtocolParser.ReadBool(stream);
						continue;
					}
					else if (num == 50)
					{
						instance.linePointList.Add(IOEntity.IOConnection.LineVec.DeserializeLengthDelimited(stream));
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
				IOEntity.IOConnection.Deserialize(stream, this, isDelta);
			}

			public virtual void LeavePool()
			{
				this._disposed = false;
			}

			public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
			{
				IOEntity.IOConnection.DeserializeLength(stream, size, this, isDelta);
			}

			public static void ResetToPool(IOEntity.IOConnection instance)
			{
				if (!instance.ShouldPool)
				{
					return;
				}
				instance.niceName = string.Empty;
				instance.type = 0;
				instance.connectedID = 0;
				instance.connectedToSlot = 0;
				instance.inUse = false;
				if (instance.linePointList != null)
				{
					for (int i = 0; i < instance.linePointList.Count; i++)
					{
						if (instance.linePointList[i] != null)
						{
							instance.linePointList[i].ResetToPool();
							instance.linePointList[i] = null;
						}
					}
					List<IOEntity.IOConnection.LineVec> lineVecs = instance.linePointList;
					Pool.FreeList<IOEntity.IOConnection.LineVec>(ref lineVecs);
					instance.linePointList = lineVecs;
				}
				Pool.Free<IOEntity.IOConnection>(ref instance);
			}

			public void ResetToPool()
			{
				IOEntity.IOConnection.ResetToPool(this);
			}

			public static void Serialize(Stream stream, IOEntity.IOConnection instance)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				if (instance.niceName != null)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteString(stream, instance.niceName);
				}
				stream.WriteByte(16);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.type);
				stream.WriteByte(24);
				ProtocolParser.WriteUInt32(stream, instance.connectedID);
				stream.WriteByte(32);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.connectedToSlot);
				stream.WriteByte(40);
				ProtocolParser.WriteBool(stream, instance.inUse);
				if (instance.linePointList != null)
				{
					for (int i = 0; i < instance.linePointList.Count; i++)
					{
						IOEntity.IOConnection.LineVec item = instance.linePointList[i];
						stream.WriteByte(50);
						memoryStream.SetLength((long)0);
						IOEntity.IOConnection.LineVec.Serialize(memoryStream, item);
						uint length = (uint)memoryStream.Length;
						ProtocolParser.WriteUInt32(stream, length);
						stream.Write(memoryStream.GetBuffer(), 0, (int)length);
					}
				}
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeDelta(Stream stream, IOEntity.IOConnection instance, IOEntity.IOConnection previous)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				if (instance.niceName != null && instance.niceName != previous.niceName)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteString(stream, instance.niceName);
				}
				if (instance.type != previous.type)
				{
					stream.WriteByte(16);
					ProtocolParser.WriteUInt64(stream, (ulong)instance.type);
				}
				if (instance.connectedID != previous.connectedID)
				{
					stream.WriteByte(24);
					ProtocolParser.WriteUInt32(stream, instance.connectedID);
				}
				if (instance.connectedToSlot != previous.connectedToSlot)
				{
					stream.WriteByte(32);
					ProtocolParser.WriteUInt64(stream, (ulong)instance.connectedToSlot);
				}
				stream.WriteByte(40);
				ProtocolParser.WriteBool(stream, instance.inUse);
				if (instance.linePointList != null)
				{
					for (int i = 0; i < instance.linePointList.Count; i++)
					{
						IOEntity.IOConnection.LineVec item = instance.linePointList[i];
						stream.WriteByte(50);
						memoryStream.SetLength((long)0);
						IOEntity.IOConnection.LineVec.SerializeDelta(memoryStream, item, item);
						uint length = (uint)memoryStream.Length;
						ProtocolParser.WriteUInt32(stream, length);
						stream.Write(memoryStream.GetBuffer(), 0, (int)length);
					}
				}
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeLengthDelimited(Stream stream, IOEntity.IOConnection instance)
			{
				byte[] bytes = IOEntity.IOConnection.SerializeToBytes(instance);
				ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
				stream.Write(bytes, 0, (int)bytes.Length);
			}

			public static byte[] SerializeToBytes(IOEntity.IOConnection instance)
			{
				byte[] array;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					IOEntity.IOConnection.Serialize(memoryStream, instance);
					array = memoryStream.ToArray();
				}
				return array;
			}

			public void ToProto(Stream stream)
			{
				IOEntity.IOConnection.Serialize(stream, this);
			}

			public byte[] ToProtoBytes()
			{
				return IOEntity.IOConnection.SerializeToBytes(this);
			}

			public virtual void WriteToStream(Stream stream)
			{
				IOEntity.IOConnection.Serialize(stream, this);
			}

			public virtual void WriteToStreamDelta(Stream stream, IOEntity.IOConnection previous)
			{
				if (previous == null)
				{
					IOEntity.IOConnection.Serialize(stream, this);
					return;
				}
				IOEntity.IOConnection.SerializeDelta(stream, this, previous);
			}

			public class LinePointList : IDisposable, Pool.IPooled, IProto
			{
				[NonSerialized]
				public Vector3 a;

				[NonSerialized]
				public Vector3 b;

				[NonSerialized]
				public Vector3 c;

				[NonSerialized]
				public Vector3 d;

				[NonSerialized]
				public Vector3 e;

				[NonSerialized]
				public Vector3 f;

				[NonSerialized]
				public Vector3 g;

				[NonSerialized]
				public Vector3 h;

				public bool ShouldPool;

				private bool _disposed;

				public LinePointList()
				{
				}

				public IOEntity.IOConnection.LinePointList Copy()
				{
					IOEntity.IOConnection.LinePointList linePointList = Pool.Get<IOEntity.IOConnection.LinePointList>();
					this.CopyTo(linePointList);
					return linePointList;
				}

				public void CopyTo(IOEntity.IOConnection.LinePointList instance)
				{
					instance.a = this.a;
					instance.b = this.b;
					instance.c = this.c;
					instance.d = this.d;
					instance.e = this.e;
					instance.f = this.f;
					instance.g = this.g;
					instance.h = this.h;
				}

				public static IOEntity.IOConnection.LinePointList Deserialize(Stream stream)
				{
					IOEntity.IOConnection.LinePointList linePointList = Pool.Get<IOEntity.IOConnection.LinePointList>();
					IOEntity.IOConnection.LinePointList.Deserialize(stream, linePointList, false);
					return linePointList;
				}

				public static IOEntity.IOConnection.LinePointList Deserialize(byte[] buffer)
				{
					IOEntity.IOConnection.LinePointList linePointList = Pool.Get<IOEntity.IOConnection.LinePointList>();
					using (MemoryStream memoryStream = new MemoryStream(buffer))
					{
						IOEntity.IOConnection.LinePointList.Deserialize(memoryStream, linePointList, false);
					}
					return linePointList;
				}

				public static IOEntity.IOConnection.LinePointList Deserialize(byte[] buffer, IOEntity.IOConnection.LinePointList instance, bool isDelta = false)
				{
					using (MemoryStream memoryStream = new MemoryStream(buffer))
					{
						IOEntity.IOConnection.LinePointList.Deserialize(memoryStream, instance, isDelta);
					}
					return instance;
				}

				public static IOEntity.IOConnection.LinePointList Deserialize(Stream stream, IOEntity.IOConnection.LinePointList instance, bool isDelta)
				{
					while (true)
					{
						int num = stream.ReadByte();
						if (num == -1)
						{
							break;
						}
						if (num <= 34)
						{
							if (num <= 18)
							{
								if (num == 10)
								{
									Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.a, isDelta);
									continue;
								}
								else if (num == 18)
								{
									Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.b, isDelta);
									continue;
								}
							}
							else if (num == 26)
							{
								Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.c, isDelta);
								continue;
							}
							else if (num == 34)
							{
								Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.d, isDelta);
								continue;
							}
						}
						else if (num <= 50)
						{
							if (num == 42)
							{
								Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.e, isDelta);
								continue;
							}
							else if (num == 50)
							{
								Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.f, isDelta);
								continue;
							}
						}
						else if (num == 58)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.g, isDelta);
							continue;
						}
						else if (num == 66)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.h, isDelta);
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

				public static IOEntity.IOConnection.LinePointList DeserializeLength(Stream stream, int length)
				{
					IOEntity.IOConnection.LinePointList linePointList = Pool.Get<IOEntity.IOConnection.LinePointList>();
					IOEntity.IOConnection.LinePointList.DeserializeLength(stream, length, linePointList, false);
					return linePointList;
				}

				public static IOEntity.IOConnection.LinePointList DeserializeLength(Stream stream, int length, IOEntity.IOConnection.LinePointList instance, bool isDelta)
				{
					long position = stream.Position + (long)length;
					while (stream.Position < position)
					{
						int num = stream.ReadByte();
						if (num == -1)
						{
							throw new EndOfStreamException();
						}
						if (num <= 34)
						{
							if (num <= 18)
							{
								if (num == 10)
								{
									Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.a, isDelta);
									continue;
								}
								else if (num == 18)
								{
									Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.b, isDelta);
									continue;
								}
							}
							else if (num == 26)
							{
								Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.c, isDelta);
								continue;
							}
							else if (num == 34)
							{
								Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.d, isDelta);
								continue;
							}
						}
						else if (num <= 50)
						{
							if (num == 42)
							{
								Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.e, isDelta);
								continue;
							}
							else if (num == 50)
							{
								Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.f, isDelta);
								continue;
							}
						}
						else if (num == 58)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.g, isDelta);
							continue;
						}
						else if (num == 66)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.h, isDelta);
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

				public static IOEntity.IOConnection.LinePointList DeserializeLengthDelimited(Stream stream)
				{
					IOEntity.IOConnection.LinePointList linePointList = Pool.Get<IOEntity.IOConnection.LinePointList>();
					IOEntity.IOConnection.LinePointList.DeserializeLengthDelimited(stream, linePointList, false);
					return linePointList;
				}

				public static IOEntity.IOConnection.LinePointList DeserializeLengthDelimited(Stream stream, IOEntity.IOConnection.LinePointList instance, bool isDelta)
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
						if (num <= 34)
						{
							if (num <= 18)
							{
								if (num == 10)
								{
									Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.a, isDelta);
									continue;
								}
								else if (num == 18)
								{
									Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.b, isDelta);
									continue;
								}
							}
							else if (num == 26)
							{
								Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.c, isDelta);
								continue;
							}
							else if (num == 34)
							{
								Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.d, isDelta);
								continue;
							}
						}
						else if (num <= 50)
						{
							if (num == 42)
							{
								Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.e, isDelta);
								continue;
							}
							else if (num == 50)
							{
								Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.f, isDelta);
								continue;
							}
						}
						else if (num == 58)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.g, isDelta);
							continue;
						}
						else if (num == 66)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.h, isDelta);
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
					IOEntity.IOConnection.LinePointList.Deserialize(stream, this, isDelta);
				}

				public virtual void LeavePool()
				{
					this._disposed = false;
				}

				public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
				{
					IOEntity.IOConnection.LinePointList.DeserializeLength(stream, size, this, isDelta);
				}

				public static void ResetToPool(IOEntity.IOConnection.LinePointList instance)
				{
					if (!instance.ShouldPool)
					{
						return;
					}
					instance.a = new Vector3();
					instance.b = new Vector3();
					instance.c = new Vector3();
					instance.d = new Vector3();
					instance.e = new Vector3();
					instance.f = new Vector3();
					instance.g = new Vector3();
					instance.h = new Vector3();
					Pool.Free<IOEntity.IOConnection.LinePointList>(ref instance);
				}

				public void ResetToPool()
				{
					IOEntity.IOConnection.LinePointList.ResetToPool(this);
				}

				public static void Serialize(Stream stream, IOEntity.IOConnection.LinePointList instance)
				{
					MemoryStream memoryStream = Pool.Get<MemoryStream>();
					stream.WriteByte(10);
					memoryStream.SetLength((long)0);
					Vector3Serialized.Serialize(memoryStream, instance.a);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
					stream.WriteByte(18);
					memoryStream.SetLength((long)0);
					Vector3Serialized.Serialize(memoryStream, instance.b);
					uint num = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, num);
					stream.Write(memoryStream.GetBuffer(), 0, (int)num);
					stream.WriteByte(26);
					memoryStream.SetLength((long)0);
					Vector3Serialized.Serialize(memoryStream, instance.c);
					uint length1 = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length1);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length1);
					stream.WriteByte(34);
					memoryStream.SetLength((long)0);
					Vector3Serialized.Serialize(memoryStream, instance.d);
					uint num1 = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, num1);
					stream.Write(memoryStream.GetBuffer(), 0, (int)num1);
					stream.WriteByte(42);
					memoryStream.SetLength((long)0);
					Vector3Serialized.Serialize(memoryStream, instance.e);
					uint length2 = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length2);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length2);
					stream.WriteByte(50);
					memoryStream.SetLength((long)0);
					Vector3Serialized.Serialize(memoryStream, instance.f);
					uint num2 = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, num2);
					stream.Write(memoryStream.GetBuffer(), 0, (int)num2);
					stream.WriteByte(58);
					memoryStream.SetLength((long)0);
					Vector3Serialized.Serialize(memoryStream, instance.g);
					uint length3 = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length3);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length3);
					stream.WriteByte(66);
					memoryStream.SetLength((long)0);
					Vector3Serialized.Serialize(memoryStream, instance.h);
					uint num3 = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, num3);
					stream.Write(memoryStream.GetBuffer(), 0, (int)num3);
					Pool.FreeMemoryStream(ref memoryStream);
				}

				public static void SerializeDelta(Stream stream, IOEntity.IOConnection.LinePointList instance, IOEntity.IOConnection.LinePointList previous)
				{
					MemoryStream memoryStream = Pool.Get<MemoryStream>();
					if (instance.a != previous.a)
					{
						stream.WriteByte(10);
						memoryStream.SetLength((long)0);
						Vector3Serialized.SerializeDelta(memoryStream, instance.a, previous.a);
						uint length = (uint)memoryStream.Length;
						ProtocolParser.WriteUInt32(stream, length);
						stream.Write(memoryStream.GetBuffer(), 0, (int)length);
					}
					if (instance.b != previous.b)
					{
						stream.WriteByte(18);
						memoryStream.SetLength((long)0);
						Vector3Serialized.SerializeDelta(memoryStream, instance.b, previous.b);
						uint num = (uint)memoryStream.Length;
						ProtocolParser.WriteUInt32(stream, num);
						stream.Write(memoryStream.GetBuffer(), 0, (int)num);
					}
					if (instance.c != previous.c)
					{
						stream.WriteByte(26);
						memoryStream.SetLength((long)0);
						Vector3Serialized.SerializeDelta(memoryStream, instance.c, previous.c);
						uint length1 = (uint)memoryStream.Length;
						ProtocolParser.WriteUInt32(stream, length1);
						stream.Write(memoryStream.GetBuffer(), 0, (int)length1);
					}
					if (instance.d != previous.d)
					{
						stream.WriteByte(34);
						memoryStream.SetLength((long)0);
						Vector3Serialized.SerializeDelta(memoryStream, instance.d, previous.d);
						uint num1 = (uint)memoryStream.Length;
						ProtocolParser.WriteUInt32(stream, num1);
						stream.Write(memoryStream.GetBuffer(), 0, (int)num1);
					}
					if (instance.e != previous.e)
					{
						stream.WriteByte(42);
						memoryStream.SetLength((long)0);
						Vector3Serialized.SerializeDelta(memoryStream, instance.e, previous.e);
						uint length2 = (uint)memoryStream.Length;
						ProtocolParser.WriteUInt32(stream, length2);
						stream.Write(memoryStream.GetBuffer(), 0, (int)length2);
					}
					if (instance.f != previous.f)
					{
						stream.WriteByte(50);
						memoryStream.SetLength((long)0);
						Vector3Serialized.SerializeDelta(memoryStream, instance.f, previous.f);
						uint num2 = (uint)memoryStream.Length;
						ProtocolParser.WriteUInt32(stream, num2);
						stream.Write(memoryStream.GetBuffer(), 0, (int)num2);
					}
					if (instance.g != previous.g)
					{
						stream.WriteByte(58);
						memoryStream.SetLength((long)0);
						Vector3Serialized.SerializeDelta(memoryStream, instance.g, previous.g);
						uint length3 = (uint)memoryStream.Length;
						ProtocolParser.WriteUInt32(stream, length3);
						stream.Write(memoryStream.GetBuffer(), 0, (int)length3);
					}
					if (instance.h != previous.h)
					{
						stream.WriteByte(66);
						memoryStream.SetLength((long)0);
						Vector3Serialized.SerializeDelta(memoryStream, instance.h, previous.h);
						uint num3 = (uint)memoryStream.Length;
						ProtocolParser.WriteUInt32(stream, num3);
						stream.Write(memoryStream.GetBuffer(), 0, (int)num3);
					}
					Pool.FreeMemoryStream(ref memoryStream);
				}

				public static void SerializeLengthDelimited(Stream stream, IOEntity.IOConnection.LinePointList instance)
				{
					byte[] bytes = IOEntity.IOConnection.LinePointList.SerializeToBytes(instance);
					ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
					stream.Write(bytes, 0, (int)bytes.Length);
				}

				public static byte[] SerializeToBytes(IOEntity.IOConnection.LinePointList instance)
				{
					byte[] array;
					using (MemoryStream memoryStream = new MemoryStream())
					{
						IOEntity.IOConnection.LinePointList.Serialize(memoryStream, instance);
						array = memoryStream.ToArray();
					}
					return array;
				}

				public void ToProto(Stream stream)
				{
					IOEntity.IOConnection.LinePointList.Serialize(stream, this);
				}

				public byte[] ToProtoBytes()
				{
					return IOEntity.IOConnection.LinePointList.SerializeToBytes(this);
				}

				public virtual void WriteToStream(Stream stream)
				{
					IOEntity.IOConnection.LinePointList.Serialize(stream, this);
				}

				public virtual void WriteToStreamDelta(Stream stream, IOEntity.IOConnection.LinePointList previous)
				{
					if (previous == null)
					{
						IOEntity.IOConnection.LinePointList.Serialize(stream, this);
						return;
					}
					IOEntity.IOConnection.LinePointList.SerializeDelta(stream, this, previous);
				}
			}

			public class LineVec : IDisposable, Pool.IPooled, IProto
			{
				[NonSerialized]
				public Vector3 vec;

				public bool ShouldPool;

				private bool _disposed;

				public LineVec()
				{
				}

				public IOEntity.IOConnection.LineVec Copy()
				{
					IOEntity.IOConnection.LineVec lineVec = Pool.Get<IOEntity.IOConnection.LineVec>();
					this.CopyTo(lineVec);
					return lineVec;
				}

				public void CopyTo(IOEntity.IOConnection.LineVec instance)
				{
					instance.vec = this.vec;
				}

				public static IOEntity.IOConnection.LineVec Deserialize(Stream stream)
				{
					IOEntity.IOConnection.LineVec lineVec = Pool.Get<IOEntity.IOConnection.LineVec>();
					IOEntity.IOConnection.LineVec.Deserialize(stream, lineVec, false);
					return lineVec;
				}

				public static IOEntity.IOConnection.LineVec Deserialize(byte[] buffer)
				{
					IOEntity.IOConnection.LineVec lineVec = Pool.Get<IOEntity.IOConnection.LineVec>();
					using (MemoryStream memoryStream = new MemoryStream(buffer))
					{
						IOEntity.IOConnection.LineVec.Deserialize(memoryStream, lineVec, false);
					}
					return lineVec;
				}

				public static IOEntity.IOConnection.LineVec Deserialize(byte[] buffer, IOEntity.IOConnection.LineVec instance, bool isDelta = false)
				{
					using (MemoryStream memoryStream = new MemoryStream(buffer))
					{
						IOEntity.IOConnection.LineVec.Deserialize(memoryStream, instance, isDelta);
					}
					return instance;
				}

				public static IOEntity.IOConnection.LineVec Deserialize(Stream stream, IOEntity.IOConnection.LineVec instance, bool isDelta)
				{
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
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.vec, isDelta);
						}
					}
					return instance;
				}

				public static IOEntity.IOConnection.LineVec DeserializeLength(Stream stream, int length)
				{
					IOEntity.IOConnection.LineVec lineVec = Pool.Get<IOEntity.IOConnection.LineVec>();
					IOEntity.IOConnection.LineVec.DeserializeLength(stream, length, lineVec, false);
					return lineVec;
				}

				public static IOEntity.IOConnection.LineVec DeserializeLength(Stream stream, int length, IOEntity.IOConnection.LineVec instance, bool isDelta)
				{
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
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.vec, isDelta);
						}
					}
					if (stream.Position != position)
					{
						throw new ProtocolBufferException("Read past max limit");
					}
					return instance;
				}

				public static IOEntity.IOConnection.LineVec DeserializeLengthDelimited(Stream stream)
				{
					IOEntity.IOConnection.LineVec lineVec = Pool.Get<IOEntity.IOConnection.LineVec>();
					IOEntity.IOConnection.LineVec.DeserializeLengthDelimited(stream, lineVec, false);
					return lineVec;
				}

				public static IOEntity.IOConnection.LineVec DeserializeLengthDelimited(Stream stream, IOEntity.IOConnection.LineVec instance, bool isDelta)
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
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.vec, isDelta);
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
					IOEntity.IOConnection.LineVec.Deserialize(stream, this, isDelta);
				}

				public virtual void LeavePool()
				{
					this._disposed = false;
				}

				public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
				{
					IOEntity.IOConnection.LineVec.DeserializeLength(stream, size, this, isDelta);
				}

				public static void ResetToPool(IOEntity.IOConnection.LineVec instance)
				{
					if (!instance.ShouldPool)
					{
						return;
					}
					instance.vec = new Vector3();
					Pool.Free<IOEntity.IOConnection.LineVec>(ref instance);
				}

				public void ResetToPool()
				{
					IOEntity.IOConnection.LineVec.ResetToPool(this);
				}

				public static void Serialize(Stream stream, IOEntity.IOConnection.LineVec instance)
				{
					MemoryStream memoryStream = Pool.Get<MemoryStream>();
					stream.WriteByte(10);
					memoryStream.SetLength((long)0);
					Vector3Serialized.Serialize(memoryStream, instance.vec);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
					Pool.FreeMemoryStream(ref memoryStream);
				}

				public static void SerializeDelta(Stream stream, IOEntity.IOConnection.LineVec instance, IOEntity.IOConnection.LineVec previous)
				{
					MemoryStream memoryStream = Pool.Get<MemoryStream>();
					if (instance.vec != previous.vec)
					{
						stream.WriteByte(10);
						memoryStream.SetLength((long)0);
						Vector3Serialized.SerializeDelta(memoryStream, instance.vec, previous.vec);
						uint length = (uint)memoryStream.Length;
						ProtocolParser.WriteUInt32(stream, length);
						stream.Write(memoryStream.GetBuffer(), 0, (int)length);
					}
					Pool.FreeMemoryStream(ref memoryStream);
				}

				public static void SerializeLengthDelimited(Stream stream, IOEntity.IOConnection.LineVec instance)
				{
					byte[] bytes = IOEntity.IOConnection.LineVec.SerializeToBytes(instance);
					ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
					stream.Write(bytes, 0, (int)bytes.Length);
				}

				public static byte[] SerializeToBytes(IOEntity.IOConnection.LineVec instance)
				{
					byte[] array;
					using (MemoryStream memoryStream = new MemoryStream())
					{
						IOEntity.IOConnection.LineVec.Serialize(memoryStream, instance);
						array = memoryStream.ToArray();
					}
					return array;
				}

				public void ToProto(Stream stream)
				{
					IOEntity.IOConnection.LineVec.Serialize(stream, this);
				}

				public byte[] ToProtoBytes()
				{
					return IOEntity.IOConnection.LineVec.SerializeToBytes(this);
				}

				public virtual void WriteToStream(Stream stream)
				{
					IOEntity.IOConnection.LineVec.Serialize(stream, this);
				}

				public virtual void WriteToStreamDelta(Stream stream, IOEntity.IOConnection.LineVec previous)
				{
					if (previous == null)
					{
						IOEntity.IOConnection.LineVec.Serialize(stream, this);
						return;
					}
					IOEntity.IOConnection.LineVec.SerializeDelta(stream, this, previous);
				}
			}
		}
	}
}