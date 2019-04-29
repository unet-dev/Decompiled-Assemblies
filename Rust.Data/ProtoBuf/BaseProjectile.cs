using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;

namespace ProtoBuf
{
	public class BaseProjectile : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public Magazine primaryMagazine;

		public bool ShouldPool = true;

		private bool _disposed;

		public BaseProjectile()
		{
		}

		public BaseProjectile Copy()
		{
			BaseProjectile baseProjectile = Pool.Get<BaseProjectile>();
			this.CopyTo(baseProjectile);
			return baseProjectile;
		}

		public void CopyTo(BaseProjectile instance)
		{
			if (this.primaryMagazine == null)
			{
				instance.primaryMagazine = null;
				return;
			}
			if (instance.primaryMagazine == null)
			{
				instance.primaryMagazine = this.primaryMagazine.Copy();
				return;
			}
			this.primaryMagazine.CopyTo(instance.primaryMagazine);
		}

		public static BaseProjectile Deserialize(Stream stream)
		{
			BaseProjectile baseProjectile = Pool.Get<BaseProjectile>();
			BaseProjectile.Deserialize(stream, baseProjectile, false);
			return baseProjectile;
		}

		public static BaseProjectile Deserialize(byte[] buffer)
		{
			BaseProjectile baseProjectile = Pool.Get<BaseProjectile>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				BaseProjectile.Deserialize(memoryStream, baseProjectile, false);
			}
			return baseProjectile;
		}

		public static BaseProjectile Deserialize(byte[] buffer, BaseProjectile instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				BaseProjectile.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static BaseProjectile Deserialize(Stream stream, BaseProjectile instance, bool isDelta)
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
				else if (instance.primaryMagazine != null)
				{
					Magazine.DeserializeLengthDelimited(stream, instance.primaryMagazine, isDelta);
				}
				else
				{
					instance.primaryMagazine = Magazine.DeserializeLengthDelimited(stream);
				}
			}
			return instance;
		}

		public static BaseProjectile DeserializeLength(Stream stream, int length)
		{
			BaseProjectile baseProjectile = Pool.Get<BaseProjectile>();
			BaseProjectile.DeserializeLength(stream, length, baseProjectile, false);
			return baseProjectile;
		}

		public static BaseProjectile DeserializeLength(Stream stream, int length, BaseProjectile instance, bool isDelta)
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
				else if (instance.primaryMagazine != null)
				{
					Magazine.DeserializeLengthDelimited(stream, instance.primaryMagazine, isDelta);
				}
				else
				{
					instance.primaryMagazine = Magazine.DeserializeLengthDelimited(stream);
				}
			}
			if (stream.Position != position)
			{
				throw new ProtocolBufferException("Read past max limit");
			}
			return instance;
		}

		public static BaseProjectile DeserializeLengthDelimited(Stream stream)
		{
			BaseProjectile baseProjectile = Pool.Get<BaseProjectile>();
			BaseProjectile.DeserializeLengthDelimited(stream, baseProjectile, false);
			return baseProjectile;
		}

		public static BaseProjectile DeserializeLengthDelimited(Stream stream, BaseProjectile instance, bool isDelta)
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
				else if (instance.primaryMagazine != null)
				{
					Magazine.DeserializeLengthDelimited(stream, instance.primaryMagazine, isDelta);
				}
				else
				{
					instance.primaryMagazine = Magazine.DeserializeLengthDelimited(stream);
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
			BaseProjectile.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			BaseProjectile.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(BaseProjectile instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			if (instance.primaryMagazine != null)
			{
				instance.primaryMagazine.ResetToPool();
				instance.primaryMagazine = null;
			}
			Pool.Free<BaseProjectile>(ref instance);
		}

		public void ResetToPool()
		{
			BaseProjectile.ResetToPool(this);
		}

		public static void Serialize(Stream stream, BaseProjectile instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.primaryMagazine != null)
			{
				stream.WriteByte(10);
				memoryStream.SetLength((long)0);
				Magazine.Serialize(memoryStream, instance.primaryMagazine);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, BaseProjectile instance, BaseProjectile previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.primaryMagazine != null)
			{
				stream.WriteByte(10);
				memoryStream.SetLength((long)0);
				Magazine.SerializeDelta(memoryStream, instance.primaryMagazine, previous.primaryMagazine);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, BaseProjectile instance)
		{
			byte[] bytes = BaseProjectile.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(BaseProjectile instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BaseProjectile.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			BaseProjectile.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return BaseProjectile.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			BaseProjectile.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, BaseProjectile previous)
		{
			if (previous == null)
			{
				BaseProjectile.Serialize(stream, this);
				return;
			}
			BaseProjectile.SerializeDelta(stream, this, previous);
		}
	}
}