using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;
using UnityEngine;

namespace ProtoBuf
{
	public class BradleyAPC : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public float engineThrottle;

		[NonSerialized]
		public float throttleLeft;

		[NonSerialized]
		public float throttleRight;

		[NonSerialized]
		public Vector3 mainGunVec;

		[NonSerialized]
		public Vector3 topTurretVec;

		[NonSerialized]
		public Vector3 rearGunVec;

		[NonSerialized]
		public Vector3 leftSideGun1;

		[NonSerialized]
		public Vector3 leftSideGun2;

		[NonSerialized]
		public Vector3 rightSideGun1;

		[NonSerialized]
		public Vector3 rightSideGun2;

		public bool ShouldPool = true;

		private bool _disposed;

		public BradleyAPC()
		{
		}

		public BradleyAPC Copy()
		{
			BradleyAPC bradleyAPC = Pool.Get<BradleyAPC>();
			this.CopyTo(bradleyAPC);
			return bradleyAPC;
		}

		public void CopyTo(BradleyAPC instance)
		{
			instance.engineThrottle = this.engineThrottle;
			instance.throttleLeft = this.throttleLeft;
			instance.throttleRight = this.throttleRight;
			instance.mainGunVec = this.mainGunVec;
			instance.topTurretVec = this.topTurretVec;
			instance.rearGunVec = this.rearGunVec;
			instance.leftSideGun1 = this.leftSideGun1;
			instance.leftSideGun2 = this.leftSideGun2;
			instance.rightSideGun1 = this.rightSideGun1;
			instance.rightSideGun2 = this.rightSideGun2;
		}

		public static BradleyAPC Deserialize(Stream stream)
		{
			BradleyAPC bradleyAPC = Pool.Get<BradleyAPC>();
			BradleyAPC.Deserialize(stream, bradleyAPC, false);
			return bradleyAPC;
		}

		public static BradleyAPC Deserialize(byte[] buffer)
		{
			BradleyAPC bradleyAPC = Pool.Get<BradleyAPC>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				BradleyAPC.Deserialize(memoryStream, bradleyAPC, false);
			}
			return bradleyAPC;
		}

		public static BradleyAPC Deserialize(byte[] buffer, BradleyAPC instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				BradleyAPC.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static BradleyAPC Deserialize(Stream stream, BradleyAPC instance, bool isDelta)
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
					if (num <= 21)
					{
						if (num == 13)
						{
							instance.engineThrottle = ProtocolParser.ReadSingle(stream);
							continue;
						}
						else if (num == 21)
						{
							instance.throttleLeft = ProtocolParser.ReadSingle(stream);
							continue;
						}
					}
					else if (num == 29)
					{
						instance.throttleRight = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 34)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.mainGunVec, isDelta);
						continue;
					}
					else if (num == 42)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.topTurretVec, isDelta);
						continue;
					}
				}
				else if (num <= 58)
				{
					if (num == 50)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.rearGunVec, isDelta);
						continue;
					}
					else if (num == 58)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.leftSideGun1, isDelta);
						continue;
					}
				}
				else if (num == 66)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.leftSideGun2, isDelta);
					continue;
				}
				else if (num == 74)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.rightSideGun1, isDelta);
					continue;
				}
				else if (num == 82)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.rightSideGun2, isDelta);
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

		public static BradleyAPC DeserializeLength(Stream stream, int length)
		{
			BradleyAPC bradleyAPC = Pool.Get<BradleyAPC>();
			BradleyAPC.DeserializeLength(stream, length, bradleyAPC, false);
			return bradleyAPC;
		}

		public static BradleyAPC DeserializeLength(Stream stream, int length, BradleyAPC instance, bool isDelta)
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
					if (num <= 21)
					{
						if (num == 13)
						{
							instance.engineThrottle = ProtocolParser.ReadSingle(stream);
							continue;
						}
						else if (num == 21)
						{
							instance.throttleLeft = ProtocolParser.ReadSingle(stream);
							continue;
						}
					}
					else if (num == 29)
					{
						instance.throttleRight = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 34)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.mainGunVec, isDelta);
						continue;
					}
					else if (num == 42)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.topTurretVec, isDelta);
						continue;
					}
				}
				else if (num <= 58)
				{
					if (num == 50)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.rearGunVec, isDelta);
						continue;
					}
					else if (num == 58)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.leftSideGun1, isDelta);
						continue;
					}
				}
				else if (num == 66)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.leftSideGun2, isDelta);
					continue;
				}
				else if (num == 74)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.rightSideGun1, isDelta);
					continue;
				}
				else if (num == 82)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.rightSideGun2, isDelta);
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

		public static BradleyAPC DeserializeLengthDelimited(Stream stream)
		{
			BradleyAPC bradleyAPC = Pool.Get<BradleyAPC>();
			BradleyAPC.DeserializeLengthDelimited(stream, bradleyAPC, false);
			return bradleyAPC;
		}

		public static BradleyAPC DeserializeLengthDelimited(Stream stream, BradleyAPC instance, bool isDelta)
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
					if (num <= 21)
					{
						if (num == 13)
						{
							instance.engineThrottle = ProtocolParser.ReadSingle(stream);
							continue;
						}
						else if (num == 21)
						{
							instance.throttleLeft = ProtocolParser.ReadSingle(stream);
							continue;
						}
					}
					else if (num == 29)
					{
						instance.throttleRight = ProtocolParser.ReadSingle(stream);
						continue;
					}
					else if (num == 34)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.mainGunVec, isDelta);
						continue;
					}
					else if (num == 42)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.topTurretVec, isDelta);
						continue;
					}
				}
				else if (num <= 58)
				{
					if (num == 50)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.rearGunVec, isDelta);
						continue;
					}
					else if (num == 58)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.leftSideGun1, isDelta);
						continue;
					}
				}
				else if (num == 66)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.leftSideGun2, isDelta);
					continue;
				}
				else if (num == 74)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.rightSideGun1, isDelta);
					continue;
				}
				else if (num == 82)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.rightSideGun2, isDelta);
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
			BradleyAPC.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			BradleyAPC.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(BradleyAPC instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.engineThrottle = 0f;
			instance.throttleLeft = 0f;
			instance.throttleRight = 0f;
			instance.mainGunVec = new Vector3();
			instance.topTurretVec = new Vector3();
			instance.rearGunVec = new Vector3();
			instance.leftSideGun1 = new Vector3();
			instance.leftSideGun2 = new Vector3();
			instance.rightSideGun1 = new Vector3();
			instance.rightSideGun2 = new Vector3();
			Pool.Free<BradleyAPC>(ref instance);
		}

		public void ResetToPool()
		{
			BradleyAPC.ResetToPool(this);
		}

		public static void Serialize(Stream stream, BradleyAPC instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(13);
			ProtocolParser.WriteSingle(stream, instance.engineThrottle);
			stream.WriteByte(21);
			ProtocolParser.WriteSingle(stream, instance.throttleLeft);
			stream.WriteByte(29);
			ProtocolParser.WriteSingle(stream, instance.throttleRight);
			stream.WriteByte(34);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.mainGunVec);
			uint length = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			stream.WriteByte(42);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.topTurretVec);
			uint num = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, num);
			stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			stream.WriteByte(50);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.rearGunVec);
			uint length1 = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length1);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length1);
			stream.WriteByte(58);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.leftSideGun1);
			uint num1 = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, num1);
			stream.Write(memoryStream.GetBuffer(), 0, (int)num1);
			stream.WriteByte(66);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.leftSideGun2);
			uint length2 = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length2);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length2);
			stream.WriteByte(74);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.rightSideGun1);
			uint num2 = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, num2);
			stream.Write(memoryStream.GetBuffer(), 0, (int)num2);
			stream.WriteByte(82);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.rightSideGun2);
			uint length3 = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length3);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length3);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, BradleyAPC instance, BradleyAPC previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.engineThrottle != previous.engineThrottle)
			{
				stream.WriteByte(13);
				ProtocolParser.WriteSingle(stream, instance.engineThrottle);
			}
			if (instance.throttleLeft != previous.throttleLeft)
			{
				stream.WriteByte(21);
				ProtocolParser.WriteSingle(stream, instance.throttleLeft);
			}
			if (instance.throttleRight != previous.throttleRight)
			{
				stream.WriteByte(29);
				ProtocolParser.WriteSingle(stream, instance.throttleRight);
			}
			if (instance.mainGunVec != previous.mainGunVec)
			{
				stream.WriteByte(34);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.mainGunVec, previous.mainGunVec);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			if (instance.topTurretVec != previous.topTurretVec)
			{
				stream.WriteByte(42);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.topTurretVec, previous.topTurretVec);
				uint num = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			}
			if (instance.rearGunVec != previous.rearGunVec)
			{
				stream.WriteByte(50);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.rearGunVec, previous.rearGunVec);
				uint length1 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length1);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length1);
			}
			if (instance.leftSideGun1 != previous.leftSideGun1)
			{
				stream.WriteByte(58);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.leftSideGun1, previous.leftSideGun1);
				uint num1 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num1);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num1);
			}
			if (instance.leftSideGun2 != previous.leftSideGun2)
			{
				stream.WriteByte(66);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.leftSideGun2, previous.leftSideGun2);
				uint length2 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length2);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length2);
			}
			if (instance.rightSideGun1 != previous.rightSideGun1)
			{
				stream.WriteByte(74);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.rightSideGun1, previous.rightSideGun1);
				uint num2 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num2);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num2);
			}
			if (instance.rightSideGun2 != previous.rightSideGun2)
			{
				stream.WriteByte(82);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.rightSideGun2, previous.rightSideGun2);
				uint length3 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length3);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length3);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, BradleyAPC instance)
		{
			byte[] bytes = BradleyAPC.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(BradleyAPC instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BradleyAPC.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			BradleyAPC.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return BradleyAPC.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			BradleyAPC.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, BradleyAPC previous)
		{
			if (previous == null)
			{
				BradleyAPC.Serialize(stream, this);
				return;
			}
			BradleyAPC.SerializeDelta(stream, this, previous);
		}
	}
}