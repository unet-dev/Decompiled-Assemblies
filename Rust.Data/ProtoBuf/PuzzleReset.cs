using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.IO;
using UnityEngine;

namespace ProtoBuf
{
	public class PuzzleReset : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public bool playerBlocksReset;

		[NonSerialized]
		public float playerDetectionRadius;

		[NonSerialized]
		public Vector3 playerDetectionOrigin;

		[NonSerialized]
		public float timeBetweenResets;

		[NonSerialized]
		public bool scaleWithServerPopulation;

		public bool ShouldPool = true;

		private bool _disposed;

		public PuzzleReset()
		{
		}

		public PuzzleReset Copy()
		{
			PuzzleReset puzzleReset = Pool.Get<PuzzleReset>();
			this.CopyTo(puzzleReset);
			return puzzleReset;
		}

		public void CopyTo(PuzzleReset instance)
		{
			instance.playerBlocksReset = this.playerBlocksReset;
			instance.playerDetectionRadius = this.playerDetectionRadius;
			instance.playerDetectionOrigin = this.playerDetectionOrigin;
			instance.timeBetweenResets = this.timeBetweenResets;
			instance.scaleWithServerPopulation = this.scaleWithServerPopulation;
		}

		public static PuzzleReset Deserialize(Stream stream)
		{
			PuzzleReset puzzleReset = Pool.Get<PuzzleReset>();
			PuzzleReset.Deserialize(stream, puzzleReset, false);
			return puzzleReset;
		}

		public static PuzzleReset Deserialize(byte[] buffer)
		{
			PuzzleReset puzzleReset = Pool.Get<PuzzleReset>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				PuzzleReset.Deserialize(memoryStream, puzzleReset, false);
			}
			return puzzleReset;
		}

		public static PuzzleReset Deserialize(byte[] buffer, PuzzleReset instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				PuzzleReset.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static PuzzleReset Deserialize(Stream stream, PuzzleReset instance, bool isDelta)
		{
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num <= 21)
				{
					if (num == 8)
					{
						instance.playerBlocksReset = ProtocolParser.ReadBool(stream);
						continue;
					}
					else if (num == 21)
					{
						instance.playerDetectionRadius = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (num == 26)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.playerDetectionOrigin, isDelta);
					continue;
				}
				else if (num == 37)
				{
					instance.timeBetweenResets = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 40)
				{
					instance.scaleWithServerPopulation = ProtocolParser.ReadBool(stream);
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

		public static PuzzleReset DeserializeLength(Stream stream, int length)
		{
			PuzzleReset puzzleReset = Pool.Get<PuzzleReset>();
			PuzzleReset.DeserializeLength(stream, length, puzzleReset, false);
			return puzzleReset;
		}

		public static PuzzleReset DeserializeLength(Stream stream, int length, PuzzleReset instance, bool isDelta)
		{
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num <= 21)
				{
					if (num == 8)
					{
						instance.playerBlocksReset = ProtocolParser.ReadBool(stream);
						continue;
					}
					else if (num == 21)
					{
						instance.playerDetectionRadius = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (num == 26)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.playerDetectionOrigin, isDelta);
					continue;
				}
				else if (num == 37)
				{
					instance.timeBetweenResets = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 40)
				{
					instance.scaleWithServerPopulation = ProtocolParser.ReadBool(stream);
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

		public static PuzzleReset DeserializeLengthDelimited(Stream stream)
		{
			PuzzleReset puzzleReset = Pool.Get<PuzzleReset>();
			PuzzleReset.DeserializeLengthDelimited(stream, puzzleReset, false);
			return puzzleReset;
		}

		public static PuzzleReset DeserializeLengthDelimited(Stream stream, PuzzleReset instance, bool isDelta)
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
				if (num <= 21)
				{
					if (num == 8)
					{
						instance.playerBlocksReset = ProtocolParser.ReadBool(stream);
						continue;
					}
					else if (num == 21)
					{
						instance.playerDetectionRadius = ProtocolParser.ReadSingle(stream);
						continue;
					}
				}
				else if (num == 26)
				{
					Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.playerDetectionOrigin, isDelta);
					continue;
				}
				else if (num == 37)
				{
					instance.timeBetweenResets = ProtocolParser.ReadSingle(stream);
					continue;
				}
				else if (num == 40)
				{
					instance.scaleWithServerPopulation = ProtocolParser.ReadBool(stream);
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
			PuzzleReset.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			PuzzleReset.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(PuzzleReset instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.playerBlocksReset = false;
			instance.playerDetectionRadius = 0f;
			instance.playerDetectionOrigin = new Vector3();
			instance.timeBetweenResets = 0f;
			instance.scaleWithServerPopulation = false;
			Pool.Free<PuzzleReset>(ref instance);
		}

		public void ResetToPool()
		{
			PuzzleReset.ResetToPool(this);
		}

		public static void Serialize(Stream stream, PuzzleReset instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteBool(stream, instance.playerBlocksReset);
			stream.WriteByte(21);
			ProtocolParser.WriteSingle(stream, instance.playerDetectionRadius);
			stream.WriteByte(26);
			memoryStream.SetLength((long)0);
			Vector3Serialized.Serialize(memoryStream, instance.playerDetectionOrigin);
			uint length = (uint)memoryStream.Length;
			ProtocolParser.WriteUInt32(stream, length);
			stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			stream.WriteByte(37);
			ProtocolParser.WriteSingle(stream, instance.timeBetweenResets);
			stream.WriteByte(40);
			ProtocolParser.WriteBool(stream, instance.scaleWithServerPopulation);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, PuzzleReset instance, PuzzleReset previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteBool(stream, instance.playerBlocksReset);
			if (instance.playerDetectionRadius != previous.playerDetectionRadius)
			{
				stream.WriteByte(21);
				ProtocolParser.WriteSingle(stream, instance.playerDetectionRadius);
			}
			if (instance.playerDetectionOrigin != previous.playerDetectionOrigin)
			{
				stream.WriteByte(26);
				memoryStream.SetLength((long)0);
				Vector3Serialized.SerializeDelta(memoryStream, instance.playerDetectionOrigin, previous.playerDetectionOrigin);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
			}
			if (instance.timeBetweenResets != previous.timeBetweenResets)
			{
				stream.WriteByte(37);
				ProtocolParser.WriteSingle(stream, instance.timeBetweenResets);
			}
			stream.WriteByte(40);
			ProtocolParser.WriteBool(stream, instance.scaleWithServerPopulation);
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, PuzzleReset instance)
		{
			byte[] bytes = PuzzleReset.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(PuzzleReset instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				PuzzleReset.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			PuzzleReset.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return PuzzleReset.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			PuzzleReset.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, PuzzleReset previous)
		{
			if (previous == null)
			{
				PuzzleReset.Serialize(stream, this);
				return;
			}
			PuzzleReset.SerializeDelta(stream, this, previous);
		}
	}
}