using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;
using UnityEngine;

namespace ProtoBuf
{
	public class Attack : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public Vector3 pointStart;

		[NonSerialized]
		public Vector3 pointEnd;

		[NonSerialized]
		public uint hitID;

		[NonSerialized]
		public uint hitBone;

		[NonSerialized]
		public Vector3 hitNormalLocal;

		[NonSerialized]
		public Vector3 hitPositionLocal;

		[NonSerialized]
		public Vector3 hitNormalWorld;

		[NonSerialized]
		public Vector3 hitPositionWorld;

		[NonSerialized]
		public uint hitPartID;

		[NonSerialized]
		public uint hitMaterialID;

		[NonSerialized]
		public uint hitItem;

		public bool ShouldPool = true;

		private bool _disposed;

		public Attack()
		{
		}

		public Attack Copy()
		{
			Attack attack = Pool.Get<Attack>();
			this.CopyTo(attack);
			return attack;
		}

		public void CopyTo(Attack instance)
		{
			instance.pointStart = this.pointStart;
			instance.pointEnd = this.pointEnd;
			instance.hitID = this.hitID;
			instance.hitBone = this.hitBone;
			instance.hitNormalLocal = this.hitNormalLocal;
			instance.hitPositionLocal = this.hitPositionLocal;
			instance.hitNormalWorld = this.hitNormalWorld;
			instance.hitPositionWorld = this.hitPositionWorld;
			instance.hitPartID = this.hitPartID;
			instance.hitMaterialID = this.hitMaterialID;
			instance.hitItem = this.hitItem;
		}

		public static Attack Deserialize(Stream stream)
		{
			Attack attack = Pool.Get<Attack>();
			Attack.Deserialize(stream, attack, false);
			return attack;
		}

		public static Attack Deserialize(byte[] buffer)
		{
			Attack attack = Pool.Get<Attack>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				Attack.Deserialize(memoryStream, attack, false);
			}
			return attack;
		}

		public static Attack Deserialize(byte[] buffer, Attack instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				Attack.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static Attack Deserialize(Stream stream, Attack instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num <= 42)
				{
					if (num <= 18)
					{
						if (num == 10)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.pointStart, isDelta);
							continue;
						}
						else if (num == 18)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.pointEnd, isDelta);
							continue;
						}
					}
					else if (num == 24)
					{
						instance.hitID = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 32)
					{
						instance.hitBone = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 42)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.hitNormalLocal, isDelta);
						continue;
					}
				}
				else if (num <= 66)
				{
					if (num == 50)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.hitPositionLocal, isDelta);
						continue;
					}
					else if (num == 58)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.hitNormalWorld, isDelta);
						continue;
					}
					else if (num == 66)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.hitPositionWorld, isDelta);
						continue;
					}
				}
				else if (num == 72)
				{
					instance.hitPartID = ProtocolParser.ReadUInt32(stream);
					continue;
				}
				else if (num == 80)
				{
					instance.hitMaterialID = ProtocolParser.ReadUInt32(stream);
					continue;
				}
				else if (num == 88)
				{
					instance.hitItem = ProtocolParser.ReadUInt32(stream);
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

		public static Attack DeserializeLength(Stream stream, int length)
		{
			Attack attack = Pool.Get<Attack>();
			Attack.DeserializeLength(stream, length, attack, false);
			return attack;
		}

		public static Attack DeserializeLength(Stream stream, int length, Attack instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num <= 42)
				{
					if (num <= 18)
					{
						if (num == 10)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.pointStart, isDelta);
							continue;
						}
						else if (num == 18)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.pointEnd, isDelta);
							continue;
						}
					}
					else if (num == 24)
					{
						instance.hitID = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 32)
					{
						instance.hitBone = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 42)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.hitNormalLocal, isDelta);
						continue;
					}
				}
				else if (num <= 66)
				{
					if (num == 50)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.hitPositionLocal, isDelta);
						continue;
					}
					else if (num == 58)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.hitNormalWorld, isDelta);
						continue;
					}
					else if (num == 66)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.hitPositionWorld, isDelta);
						continue;
					}
				}
				else if (num == 72)
				{
					instance.hitPartID = ProtocolParser.ReadUInt32(stream);
					continue;
				}
				else if (num == 80)
				{
					instance.hitMaterialID = ProtocolParser.ReadUInt32(stream);
					continue;
				}
				else if (num == 88)
				{
					instance.hitItem = ProtocolParser.ReadUInt32(stream);
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

		public static Attack DeserializeLengthDelimited(Stream stream)
		{
			Attack attack = Pool.Get<Attack>();
			Attack.DeserializeLengthDelimited(stream, attack, false);
			return attack;
		}

		public static Attack DeserializeLengthDelimited(Stream stream, Attack instance, bool isDelta)
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
				if (num <= 42)
				{
					if (num <= 18)
					{
						if (num == 10)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.pointStart, isDelta);
							continue;
						}
						else if (num == 18)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.pointEnd, isDelta);
							continue;
						}
					}
					else if (num == 24)
					{
						instance.hitID = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 32)
					{
						instance.hitBone = ProtocolParser.ReadUInt32(stream);
						continue;
					}
					else if (num == 42)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.hitNormalLocal, isDelta);
						continue;
					}
				}
				else if (num <= 66)
				{
					if (num == 50)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.hitPositionLocal, isDelta);
						continue;
					}
					else if (num == 58)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.hitNormalWorld, isDelta);
						continue;
					}
					else if (num == 66)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.hitPositionWorld, isDelta);
						continue;
					}
				}
				else if (num == 72)
				{
					instance.hitPartID = ProtocolParser.ReadUInt32(stream);
					continue;
				}
				else if (num == 80)
				{
					instance.hitMaterialID = ProtocolParser.ReadUInt32(stream);
					continue;
				}
				else if (num == 88)
				{
					instance.hitItem = ProtocolParser.ReadUInt32(stream);
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
			Attack.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			Attack.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(Attack instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.pointStart = new Vector3();
			instance.pointEnd = new Vector3();
			instance.hitID = 0;
			instance.hitBone = 0;
			instance.hitNormalLocal = new Vector3();
			instance.hitPositionLocal = new Vector3();
			instance.hitNormalWorld = new Vector3();
			instance.hitPositionWorld = new Vector3();
			instance.hitPartID = 0;
			instance.hitMaterialID = 0;
			instance.hitItem = 0;
			Pool.Free<Attack>(ref instance);
		}

		public void ResetToPool()
		{
			Attack.ResetToPool(this);
		}

		public static void Serialize(Stream stream, Attack instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(10);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.pointStart);
			uint length = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			stream.WriteByte(18);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.pointEnd);
			uint num = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, num);
			stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			stream.WriteByte(24);
			ProtocolParser.WriteUInt32(stream, instance.hitID);
			stream.WriteByte(32);
			ProtocolParser.WriteUInt32(stream, instance.hitBone);
			stream.WriteByte(42);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.hitNormalLocal);
			uint length1 = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length1);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length1);
			stream.WriteByte(50);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.hitPositionLocal);
			uint num1 = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, num1);
			stream.Write(memoryStream.GetBuffer(), 0, (int)num1);
			stream.WriteByte(58);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.hitNormalWorld);
			uint length2 = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length2);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length2);
			stream.WriteByte(66);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.hitPositionWorld);
			uint num2 = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, num2);
			stream.Write(memoryStream.GetBuffer(), 0, (int)num2);
			stream.WriteByte(72);
			ProtocolParser.WriteUInt32(stream, instance.hitPartID);
			stream.WriteByte(80);
			ProtocolParser.WriteUInt32(stream, instance.hitMaterialID);
			stream.WriteByte(88);
			ProtocolParser.WriteUInt32(stream, instance.hitItem);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, Attack instance, Attack previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.pointStart != previous.pointStart)
			{
				stream.WriteByte(10);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.pointStart, previous.pointStart);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			if (instance.pointEnd != previous.pointEnd)
			{
				stream.WriteByte(18);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.pointEnd, previous.pointEnd);
				uint num = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			}
			if (instance.hitID != previous.hitID)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt32(stream, instance.hitID);
			}
			if (instance.hitBone != previous.hitBone)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteUInt32(stream, instance.hitBone);
			}
			if (instance.hitNormalLocal != previous.hitNormalLocal)
			{
				stream.WriteByte(42);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.hitNormalLocal, previous.hitNormalLocal);
				uint length1 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length1);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length1);
			}
			if (instance.hitPositionLocal != previous.hitPositionLocal)
			{
				stream.WriteByte(50);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.hitPositionLocal, previous.hitPositionLocal);
				uint num1 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num1);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num1);
			}
			if (instance.hitNormalWorld != previous.hitNormalWorld)
			{
				stream.WriteByte(58);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.hitNormalWorld, previous.hitNormalWorld);
				uint length2 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length2);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length2);
			}
			if (instance.hitPositionWorld != previous.hitPositionWorld)
			{
				stream.WriteByte(66);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.hitPositionWorld, previous.hitPositionWorld);
				uint num2 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num2);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num2);
			}
			if (instance.hitPartID != previous.hitPartID)
			{
				stream.WriteByte(72);
				ProtocolParser.WriteUInt32(stream, instance.hitPartID);
			}
			if (instance.hitMaterialID != previous.hitMaterialID)
			{
				stream.WriteByte(80);
				ProtocolParser.WriteUInt32(stream, instance.hitMaterialID);
			}
			if (instance.hitItem != previous.hitItem)
			{
				stream.WriteByte(88);
				ProtocolParser.WriteUInt32(stream, instance.hitItem);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, Attack instance)
		{
			byte[] bytes = Attack.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(Attack instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Attack.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			Attack.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return Attack.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			Attack.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, Attack previous)
		{
			if (previous == null)
			{
				Attack.Serialize(stream, this);
				return;
			}
			Attack.SerializeDelta(stream, this, previous);
		}
	}
}