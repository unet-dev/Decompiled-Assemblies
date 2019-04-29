using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;
using UnityEngine;

namespace ProtoBuf
{
	public class SAMSite : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public Vector3 aimDir;

		public bool ShouldPool = true;

		private bool _disposed;

		public SAMSite()
		{
		}

		public SAMSite Copy()
		{
			SAMSite sAMSite = Pool.Get<SAMSite>();
			this.CopyTo(sAMSite);
			return sAMSite;
		}

		public void CopyTo(SAMSite instance)
		{
			instance.aimDir = this.aimDir;
		}

		public static SAMSite Deserialize(Stream stream)
		{
			SAMSite sAMSite = Pool.Get<SAMSite>();
			SAMSite.Deserialize(stream, sAMSite, false);
			return sAMSite;
		}

		public static SAMSite Deserialize(byte[] buffer)
		{
			SAMSite sAMSite = Pool.Get<SAMSite>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				SAMSite.Deserialize(memoryStream, sAMSite, false);
			}
			return sAMSite;
		}

		public static SAMSite Deserialize(byte[] buffer, SAMSite instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				SAMSite.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static SAMSite Deserialize(Stream stream, SAMSite instance, bool isDelta)
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
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.aimDir, isDelta);
				}
			}
			return instance;
		}

		public static SAMSite DeserializeLength(Stream stream, int length)
		{
			SAMSite sAMSite = Pool.Get<SAMSite>();
			SAMSite.DeserializeLength(stream, length, sAMSite, false);
			return sAMSite;
		}

		public static SAMSite DeserializeLength(Stream stream, int length, SAMSite instance, bool isDelta)
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
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.aimDir, isDelta);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static SAMSite DeserializeLengthDelimited(Stream stream)
		{
			SAMSite sAMSite = Pool.Get<SAMSite>();
			SAMSite.DeserializeLengthDelimited(stream, sAMSite, false);
			return sAMSite;
		}

		public static SAMSite DeserializeLengthDelimited(Stream stream, SAMSite instance, bool isDelta)
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
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.aimDir, isDelta);
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
			SAMSite.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			SAMSite.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(SAMSite instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.aimDir = new Vector3();
			Pool.Free<SAMSite>(ref instance);
		}

		public void ResetToPool()
		{
			SAMSite.ResetToPool(this);
		}

		public static void Serialize(Stream stream, SAMSite instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(10);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.aimDir);
			uint length = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, SAMSite instance, SAMSite previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.aimDir != previous.aimDir)
			{
				stream.WriteByte(10);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.aimDir, previous.aimDir);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, SAMSite instance)
		{
			byte[] bytes = SAMSite.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(SAMSite instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				SAMSite.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			SAMSite.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return SAMSite.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			SAMSite.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, SAMSite previous)
		{
			if (previous == null)
			{
				SAMSite.Serialize(stream, this);
				return;
			}
			SAMSite.SerializeDelta(stream, this, previous);
		}
	}
}