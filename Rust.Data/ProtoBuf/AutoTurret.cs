using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ProtoBuf
{
	public class AutoTurret : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public Vector3 aimPos;

		[NonSerialized]
		public Vector3 aimDir;

		[NonSerialized]
		public uint targetID;

		[NonSerialized]
		public List<PlayerNameID> users;

		public bool ShouldPool = true;

		private bool _disposed;

		public AutoTurret()
		{
		}

		public AutoTurret Copy()
		{
			AutoTurret autoTurret = Pool.Get<AutoTurret>();
			this.CopyTo(autoTurret);
			return autoTurret;
		}

		public void CopyTo(AutoTurret instance)
		{
			instance.aimPos = this.aimPos;
			instance.aimDir = this.aimDir;
			instance.targetID = this.targetID;
			throw new NotImplementedException();
		}

		public static AutoTurret Deserialize(Stream stream)
		{
			AutoTurret autoTurret = Pool.Get<AutoTurret>();
			AutoTurret.Deserialize(stream, autoTurret, false);
			return autoTurret;
		}

		public static AutoTurret Deserialize(byte[] buffer)
		{
			AutoTurret autoTurret = Pool.Get<AutoTurret>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				AutoTurret.Deserialize(memoryStream, autoTurret, false);
			}
			return autoTurret;
		}

		public static AutoTurret Deserialize(byte[] buffer, AutoTurret instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				AutoTurret.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static AutoTurret Deserialize(Stream stream, AutoTurret instance, bool isDelta)
		{
			if (!isDelta && instance.users == null)
			{
				instance.users = Pool.Get<List<PlayerNameID>>();
			}
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num <= 18)
				{
					if (num == 10)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.aimPos, isDelta);
						continue;
					}
					else if (num == 18)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.aimDir, isDelta);
						continue;
					}
				}
				else if (num == 24)
				{
					instance.targetID = ProtocolParser.ReadUInt32(stream);
					continue;
				}
				else if (num == 34)
				{
					instance.users.Add(PlayerNameID.DeserializeLengthDelimited(stream));
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

		public static AutoTurret DeserializeLength(Stream stream, int length)
		{
			AutoTurret autoTurret = Pool.Get<AutoTurret>();
			AutoTurret.DeserializeLength(stream, length, autoTurret, false);
			return autoTurret;
		}

		public static AutoTurret DeserializeLength(Stream stream, int length, AutoTurret instance, bool isDelta)
		{
			if (!isDelta && instance.users == null)
			{
				instance.users = Pool.Get<List<PlayerNameID>>();
			}
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num <= 18)
				{
					if (num == 10)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.aimPos, isDelta);
						continue;
					}
					else if (num == 18)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.aimDir, isDelta);
						continue;
					}
				}
				else if (num == 24)
				{
					instance.targetID = ProtocolParser.ReadUInt32(stream);
					continue;
				}
				else if (num == 34)
				{
					instance.users.Add(PlayerNameID.DeserializeLengthDelimited(stream));
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

		public static AutoTurret DeserializeLengthDelimited(Stream stream)
		{
			AutoTurret autoTurret = Pool.Get<AutoTurret>();
			AutoTurret.DeserializeLengthDelimited(stream, autoTurret, false);
			return autoTurret;
		}

		public static AutoTurret DeserializeLengthDelimited(Stream stream, AutoTurret instance, bool isDelta)
		{
			if (!isDelta && instance.users == null)
			{
				instance.users = Pool.Get<List<PlayerNameID>>();
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
				if (num <= 18)
				{
					if (num == 10)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.aimPos, isDelta);
						continue;
					}
					else if (num == 18)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.aimDir, isDelta);
						continue;
					}
				}
				else if (num == 24)
				{
					instance.targetID = ProtocolParser.ReadUInt32(stream);
					continue;
				}
				else if (num == 34)
				{
					instance.users.Add(PlayerNameID.DeserializeLengthDelimited(stream));
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
			AutoTurret.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			AutoTurret.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(AutoTurret instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.aimPos = new Vector3();
			instance.aimDir = new Vector3();
			instance.targetID = 0;
			if (instance.users != null)
			{
				for (int i = 0; i < instance.users.Count; i++)
				{
					if (instance.users[i] != null)
					{
						instance.users[i].ResetToPool();
						instance.users[i] = null;
					}
				}
				List<PlayerNameID> playerNameIDs = instance.users;
				Pool.FreeList<PlayerNameID>(ref playerNameIDs);
				instance.users = playerNameIDs;
			}
			Pool.Free<AutoTurret>(ref instance);
		}

		public void ResetToPool()
		{
			AutoTurret.ResetToPool(this);
		}

		public static void Serialize(Stream stream, AutoTurret instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(10);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.aimPos);
			uint length = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			stream.WriteByte(18);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.aimDir);
			uint num = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, num);
			stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			stream.WriteByte(24);
			ProtocolParser.WriteUInt32(stream, instance.targetID);
			if (instance.users != null)
			{
				for (int i = 0; i < instance.users.Count; i++)
				{
					PlayerNameID item = instance.users[i];
					stream.WriteByte(34);
					memoryStream.SetLength((long)0);
					PlayerNameID.Serialize(memoryStream, item);
					uint length1 = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length1);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length1);
				}
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, AutoTurret instance, AutoTurret previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.aimPos != previous.aimPos)
			{
				stream.WriteByte(10);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.aimPos, previous.aimPos);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			if (instance.aimDir != previous.aimDir)
			{
				stream.WriteByte(18);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.aimDir, previous.aimDir);
				uint num = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			}
			if (instance.targetID != previous.targetID)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt32(stream, instance.targetID);
			}
			if (instance.users != null)
			{
				for (int i = 0; i < instance.users.Count; i++)
				{
					PlayerNameID item = instance.users[i];
					stream.WriteByte(34);
					memoryStream.SetLength((long)0);
					PlayerNameID.SerializeDelta(memoryStream, item, item);
					uint length1 = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length1);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length1);
				}
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, AutoTurret instance)
		{
			byte[] bytes = AutoTurret.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(AutoTurret instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				AutoTurret.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			AutoTurret.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return AutoTurret.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			AutoTurret.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, AutoTurret previous)
		{
			if (previous == null)
			{
				AutoTurret.Serialize(stream, this);
				return;
			}
			AutoTurret.SerializeDelta(stream, this, previous);
		}
	}
}