using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ProtoBuf
{
	public class Helicopter : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public Vector3 tiltRot;

		[NonSerialized]
		public Vector3 leftGun;

		[NonSerialized]
		public Vector3 rightGun;

		[NonSerialized]
		public Vector3 spotlightVec;

		[NonSerialized]
		public List<float> weakspothealths;

		public bool ShouldPool = true;

		private bool _disposed;

		public Helicopter()
		{
		}

		public Helicopter Copy()
		{
			Helicopter helicopter = Pool.Get<Helicopter>();
			this.CopyTo(helicopter);
			return helicopter;
		}

		public void CopyTo(Helicopter instance)
		{
			instance.tiltRot = this.tiltRot;
			instance.leftGun = this.leftGun;
			instance.rightGun = this.rightGun;
			instance.spotlightVec = this.spotlightVec;
			throw new NotImplementedException();
		}

		public static Helicopter Deserialize(Stream stream)
		{
			Helicopter helicopter = Pool.Get<Helicopter>();
			Helicopter.Deserialize(stream, helicopter, false);
			return helicopter;
		}

		public static Helicopter Deserialize(byte[] buffer)
		{
			Helicopter helicopter = Pool.Get<Helicopter>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				Helicopter.Deserialize(memoryStream, helicopter, false);
			}
			return helicopter;
		}

		public static Helicopter Deserialize(byte[] buffer, Helicopter instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				Helicopter.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static Helicopter Deserialize(Stream stream, Helicopter instance, bool isDelta)
		{
			if (!isDelta && instance.weakspothealths == null)
			{
				instance.weakspothealths = Pool.Get<List<float>>();
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
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.tiltRot, isDelta);
						continue;
					}
					else if (num == 18)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.leftGun, isDelta);
						continue;
					}
				}
				else if (num == 26)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.rightGun, isDelta);
					continue;
				}
				else if (num == 34)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.spotlightVec, isDelta);
					continue;
				}
				else if (num == 45)
				{
					instance.weakspothealths.Add(ProtocolParser.ReadSingle(stream));
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

		public static Helicopter DeserializeLength(Stream stream, int length)
		{
			Helicopter helicopter = Pool.Get<Helicopter>();
			Helicopter.DeserializeLength(stream, length, helicopter, false);
			return helicopter;
		}

		public static Helicopter DeserializeLength(Stream stream, int length, Helicopter instance, bool isDelta)
		{
			if (!isDelta && instance.weakspothealths == null)
			{
				instance.weakspothealths = Pool.Get<List<float>>();
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
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.tiltRot, isDelta);
						continue;
					}
					else if (num == 18)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.leftGun, isDelta);
						continue;
					}
				}
				else if (num == 26)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.rightGun, isDelta);
					continue;
				}
				else if (num == 34)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.spotlightVec, isDelta);
					continue;
				}
				else if (num == 45)
				{
					instance.weakspothealths.Add(ProtocolParser.ReadSingle(stream));
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

		public static Helicopter DeserializeLengthDelimited(Stream stream)
		{
			Helicopter helicopter = Pool.Get<Helicopter>();
			Helicopter.DeserializeLengthDelimited(stream, helicopter, false);
			return helicopter;
		}

		public static Helicopter DeserializeLengthDelimited(Stream stream, Helicopter instance, bool isDelta)
		{
			if (!isDelta && instance.weakspothealths == null)
			{
				instance.weakspothealths = Pool.Get<List<float>>();
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
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.tiltRot, isDelta);
						continue;
					}
					else if (num == 18)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.leftGun, isDelta);
						continue;
					}
				}
				else if (num == 26)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.rightGun, isDelta);
					continue;
				}
				else if (num == 34)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.spotlightVec, isDelta);
					continue;
				}
				else if (num == 45)
				{
					instance.weakspothealths.Add(ProtocolParser.ReadSingle(stream));
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
			Helicopter.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			Helicopter.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(Helicopter instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.tiltRot = new Vector3();
			instance.leftGun = new Vector3();
			instance.rightGun = new Vector3();
			instance.spotlightVec = new Vector3();
			if (instance.weakspothealths != null)
			{
				List<float> singles = instance.weakspothealths;
				Pool.FreeList<float>(ref singles);
				instance.weakspothealths = singles;
			}
			Pool.Free<Helicopter>(ref instance);
		}

		public void ResetToPool()
		{
			Helicopter.ResetToPool(this);
		}

		public static void Serialize(Stream stream, Helicopter instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(10);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.tiltRot);
			uint length = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			stream.WriteByte(18);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.leftGun);
			uint num = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, num);
			stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			stream.WriteByte(26);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.rightGun);
			uint length1 = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length1);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length1);
			stream.WriteByte(34);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.spotlightVec);
			uint num1 = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, num1);
			stream.Write(memoryStream.GetBuffer(), 0, (int)num1);
			if (instance.weakspothealths != null)
			{
				for (int i = 0; i < instance.weakspothealths.Count; i++)
				{
					float item = instance.weakspothealths[i];
					stream.WriteByte(45);
					ProtocolParser.WriteSingle(stream, item);
				}
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, Helicopter instance, Helicopter previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.tiltRot != previous.tiltRot)
			{
				stream.WriteByte(10);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.tiltRot, previous.tiltRot);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			if (instance.leftGun != previous.leftGun)
			{
				stream.WriteByte(18);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.leftGun, previous.leftGun);
				uint num = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num);
			}
			if (instance.rightGun != previous.rightGun)
			{
				stream.WriteByte(26);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.rightGun, previous.rightGun);
				uint length1 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length1);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length1);
			}
			if (instance.spotlightVec != previous.spotlightVec)
			{
				stream.WriteByte(34);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.spotlightVec, previous.spotlightVec);
				uint num1 = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num1);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num1);
			}
			if (instance.weakspothealths != null)
			{
				for (int i = 0; i < instance.weakspothealths.Count; i++)
				{
					float item = instance.weakspothealths[i];
					stream.WriteByte(45);
					ProtocolParser.WriteSingle(stream, item);
				}
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, Helicopter instance)
		{
			byte[] bytes = Helicopter.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(Helicopter instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Helicopter.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			Helicopter.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return Helicopter.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			Helicopter.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, Helicopter previous)
		{
			if (previous == null)
			{
				Helicopter.Serialize(stream, this);
				return;
			}
			Helicopter.SerializeDelta(stream, this, previous);
		}
	}
}