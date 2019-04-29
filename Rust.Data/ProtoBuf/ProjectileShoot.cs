using Facepunch;
using SilentOrbit.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ProtoBuf
{
	public class ProjectileShoot : IDisposable, Pool.IPooled, IProto
	{
		[NonSerialized]
		public int ammoType;

		[NonSerialized]
		public List<ProjectileShoot.Projectile> projectiles;

		public bool ShouldPool = true;

		private bool _disposed;

		public ProjectileShoot()
		{
		}

		public ProjectileShoot Copy()
		{
			ProjectileShoot projectileShoot = Pool.Get<ProjectileShoot>();
			this.CopyTo(projectileShoot);
			return projectileShoot;
		}

		public void CopyTo(ProjectileShoot instance)
		{
			instance.ammoType = this.ammoType;
			throw new NotImplementedException();
		}

		public static ProjectileShoot Deserialize(Stream stream)
		{
			ProjectileShoot projectileShoot = Pool.Get<ProjectileShoot>();
			ProjectileShoot.Deserialize(stream, projectileShoot, false);
			return projectileShoot;
		}

		public static ProjectileShoot Deserialize(byte[] buffer)
		{
			ProjectileShoot projectileShoot = Pool.Get<ProjectileShoot>();
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				ProjectileShoot.Deserialize(memoryStream, projectileShoot, false);
			}
			return projectileShoot;
		}

		public static ProjectileShoot Deserialize(byte[] buffer, ProjectileShoot instance, bool isDelta = false)
		{
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				ProjectileShoot.Deserialize(memoryStream, instance, isDelta);
			}
			return instance;
		}

		public static ProjectileShoot Deserialize(Stream stream, ProjectileShoot instance, bool isDelta)
		{
			if (!isDelta && instance.projectiles == null)
			{
				instance.projectiles = Pool.Get<List<ProjectileShoot.Projectile>>();
			}
			while (true)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if (num == 8)
				{
					instance.ammoType = (int)ProtocolParser.ReadUInt64(stream);
				}
				else if (num == 18)
				{
					instance.projectiles.Add(ProjectileShoot.Projectile.DeserializeLengthDelimited(stream));
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

		public static ProjectileShoot DeserializeLength(Stream stream, int length)
		{
			ProjectileShoot projectileShoot = Pool.Get<ProjectileShoot>();
			ProjectileShoot.DeserializeLength(stream, length, projectileShoot, false);
			return projectileShoot;
		}

		public static ProjectileShoot DeserializeLength(Stream stream, int length, ProjectileShoot instance, bool isDelta)
		{
			if (!isDelta && instance.projectiles == null)
			{
				instance.projectiles = Pool.Get<List<ProjectileShoot.Projectile>>();
			}
			long position = stream.Position + (long)length;
			while (stream.Position < position)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				if (num == 8)
				{
					instance.ammoType = (int)ProtocolParser.ReadUInt64(stream);
				}
				else if (num == 18)
				{
					instance.projectiles.Add(ProjectileShoot.Projectile.DeserializeLengthDelimited(stream));
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

		public static ProjectileShoot DeserializeLengthDelimited(Stream stream)
		{
			ProjectileShoot projectileShoot = Pool.Get<ProjectileShoot>();
			ProjectileShoot.DeserializeLengthDelimited(stream, projectileShoot, false);
			return projectileShoot;
		}

		public static ProjectileShoot DeserializeLengthDelimited(Stream stream, ProjectileShoot instance, bool isDelta)
		{
			if (!isDelta && instance.projectiles == null)
			{
				instance.projectiles = Pool.Get<List<ProjectileShoot.Projectile>>();
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
				if (num == 8)
				{
					instance.ammoType = (int)ProtocolParser.ReadUInt64(stream);
				}
				else if (num == 18)
				{
					instance.projectiles.Add(ProjectileShoot.Projectile.DeserializeLengthDelimited(stream));
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
			ProjectileShoot.Deserialize(stream, this, isDelta);
		}

		public virtual void LeavePool()
		{
			this._disposed = false;
		}

		public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
		{
			ProjectileShoot.DeserializeLength(stream, size, this, isDelta);
		}

		public static void ResetToPool(ProjectileShoot instance)
		{
			if (!instance.ShouldPool)
			{
				return;
			}
			instance.ammoType = 0;
			if (instance.projectiles != null)
			{
				for (int i = 0; i < instance.projectiles.Count; i++)
				{
					if (instance.projectiles[i] != null)
					{
						instance.projectiles[i].ResetToPool();
						instance.projectiles[i] = null;
					}
				}
				List<ProjectileShoot.Projectile> projectiles = instance.projectiles;
				Pool.FreeList<ProjectileShoot.Projectile>(ref projectiles);
				instance.projectiles = projectiles;
			}
			Pool.Free<ProjectileShoot>(ref instance);
		}

		public void ResetToPool()
		{
			ProjectileShoot.ResetToPool(this);
		}

		public static void Serialize(Stream stream, ProjectileShoot instance)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			stream.WriteByte(8);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.ammoType);
			if (instance.projectiles != null)
			{
				for (int i = 0; i < instance.projectiles.Count; i++)
				{
					ProjectileShoot.Projectile item = instance.projectiles[i];
					stream.WriteByte(18);
					memoryStream.SetLength((long)0);
					ProjectileShoot.Projectile.Serialize(memoryStream, item);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeDelta(Stream stream, ProjectileShoot instance, ProjectileShoot previous)
		{
			MemoryStream memoryStream = Pool.Get<MemoryStream>();
			if (instance.ammoType != previous.ammoType)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.ammoType);
			}
			if (instance.projectiles != null)
			{
				for (int i = 0; i < instance.projectiles.Count; i++)
				{
					ProjectileShoot.Projectile item = instance.projectiles[i];
					stream.WriteByte(18);
					memoryStream.SetLength((long)0);
					ProjectileShoot.Projectile.SerializeDelta(memoryStream, item, item);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
			}
			Pool.FreeMemoryStream(ref memoryStream);
		}

		public static void SerializeLengthDelimited(Stream stream, ProjectileShoot instance)
		{
			byte[] bytes = ProjectileShoot.SerializeToBytes(instance);
			ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
			stream.Write(bytes, 0, (int)bytes.Length);
		}

		public static byte[] SerializeToBytes(ProjectileShoot instance)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				ProjectileShoot.Serialize(memoryStream, instance);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public void ToProto(Stream stream)
		{
			ProjectileShoot.Serialize(stream, this);
		}

		public byte[] ToProtoBytes()
		{
			return ProjectileShoot.SerializeToBytes(this);
		}

		public virtual void WriteToStream(Stream stream)
		{
			ProjectileShoot.Serialize(stream, this);
		}

		public virtual void WriteToStreamDelta(Stream stream, ProjectileShoot previous)
		{
			if (previous == null)
			{
				ProjectileShoot.Serialize(stream, this);
				return;
			}
			ProjectileShoot.SerializeDelta(stream, this, previous);
		}

		public class Projectile : IDisposable, Pool.IPooled, IProto
		{
			[NonSerialized]
			public int projectileID;

			[NonSerialized]
			public Vector3 startPos;

			[NonSerialized]
			public Vector3 startVel;

			[NonSerialized]
			public int seed;

			public bool ShouldPool;

			private bool _disposed;

			public Projectile()
			{
			}

			public ProjectileShoot.Projectile Copy()
			{
				ProjectileShoot.Projectile projectile = Pool.Get<ProjectileShoot.Projectile>();
				this.CopyTo(projectile);
				return projectile;
			}

			public void CopyTo(ProjectileShoot.Projectile instance)
			{
				instance.projectileID = this.projectileID;
				instance.startPos = this.startPos;
				instance.startVel = this.startVel;
				instance.seed = this.seed;
			}

			public static ProjectileShoot.Projectile Deserialize(Stream stream)
			{
				ProjectileShoot.Projectile projectile = Pool.Get<ProjectileShoot.Projectile>();
				ProjectileShoot.Projectile.Deserialize(stream, projectile, false);
				return projectile;
			}

			public static ProjectileShoot.Projectile Deserialize(byte[] buffer)
			{
				ProjectileShoot.Projectile projectile = Pool.Get<ProjectileShoot.Projectile>();
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					ProjectileShoot.Projectile.Deserialize(memoryStream, projectile, false);
				}
				return projectile;
			}

			public static ProjectileShoot.Projectile Deserialize(byte[] buffer, ProjectileShoot.Projectile instance, bool isDelta = false)
			{
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					ProjectileShoot.Projectile.Deserialize(memoryStream, instance, isDelta);
				}
				return instance;
			}

			public static ProjectileShoot.Projectile Deserialize(Stream stream, ProjectileShoot.Projectile instance, bool isDelta)
			{
				while (true)
				{
					int num = stream.ReadByte();
					if (num == -1)
					{
						break;
					}
					if (num <= 18)
					{
						if (num == 8)
						{
							instance.projectileID = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
						else if (num == 18)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.startPos, isDelta);
							continue;
						}
					}
					else if (num == 26)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.startVel, isDelta);
						continue;
					}
					else if (num == 32)
					{
						instance.seed = (int)ProtocolParser.ReadUInt64(stream);
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

			public static ProjectileShoot.Projectile DeserializeLength(Stream stream, int length)
			{
				ProjectileShoot.Projectile projectile = Pool.Get<ProjectileShoot.Projectile>();
				ProjectileShoot.Projectile.DeserializeLength(stream, length, projectile, false);
				return projectile;
			}

			public static ProjectileShoot.Projectile DeserializeLength(Stream stream, int length, ProjectileShoot.Projectile instance, bool isDelta)
			{
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
						if (num == 8)
						{
							instance.projectileID = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
						else if (num == 18)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.startPos, isDelta);
							continue;
						}
					}
					else if (num == 26)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.startVel, isDelta);
						continue;
					}
					else if (num == 32)
					{
						instance.seed = (int)ProtocolParser.ReadUInt64(stream);
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

			public static ProjectileShoot.Projectile DeserializeLengthDelimited(Stream stream)
			{
				ProjectileShoot.Projectile projectile = Pool.Get<ProjectileShoot.Projectile>();
				ProjectileShoot.Projectile.DeserializeLengthDelimited(stream, projectile, false);
				return projectile;
			}

			public static ProjectileShoot.Projectile DeserializeLengthDelimited(Stream stream, ProjectileShoot.Projectile instance, bool isDelta)
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
					if (num <= 18)
					{
						if (num == 8)
						{
							instance.projectileID = (int)ProtocolParser.ReadUInt64(stream);
							continue;
						}
						else if (num == 18)
						{
							Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.startPos, isDelta);
							continue;
						}
					}
					else if (num == 26)
					{
						Vector3Serialized.DeserializeLengthDelimited(stream, ref instance.startVel, isDelta);
						continue;
					}
					else if (num == 32)
					{
						instance.seed = (int)ProtocolParser.ReadUInt64(stream);
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
				ProjectileShoot.Projectile.Deserialize(stream, this, isDelta);
			}

			public virtual void LeavePool()
			{
				this._disposed = false;
			}

			public virtual void ReadFromStream(Stream stream, int size, bool isDelta = false)
			{
				ProjectileShoot.Projectile.DeserializeLength(stream, size, this, isDelta);
			}

			public static void ResetToPool(ProjectileShoot.Projectile instance)
			{
				if (!instance.ShouldPool)
				{
					return;
				}
				instance.projectileID = 0;
				instance.startPos = new Vector3();
				instance.startVel = new Vector3();
				instance.seed = 0;
				Pool.Free<ProjectileShoot.Projectile>(ref instance);
			}

			public void ResetToPool()
			{
				ProjectileShoot.Projectile.ResetToPool(this);
			}

			public static void Serialize(Stream stream, ProjectileShoot.Projectile instance)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				stream.WriteByte(8);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.projectileID);
				stream.WriteByte(18);
				memoryStream.SetLength((long)0);
				Vector3Serialized.Serialize(memoryStream, instance.startPos);
				uint length = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, length);
				stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				stream.WriteByte(26);
				memoryStream.SetLength((long)0);
				Vector3Serialized.Serialize(memoryStream, instance.startVel);
				uint num = (uint)memoryStream.Length;
				ProtocolParser.WriteUInt32(stream, num);
				stream.Write(memoryStream.GetBuffer(), 0, (int)num);
				stream.WriteByte(32);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.seed);
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeDelta(Stream stream, ProjectileShoot.Projectile instance, ProjectileShoot.Projectile previous)
			{
				MemoryStream memoryStream = Pool.Get<MemoryStream>();
				if (instance.projectileID != previous.projectileID)
				{
					stream.WriteByte(8);
					ProtocolParser.WriteUInt64(stream, (ulong)instance.projectileID);
				}
				if (instance.startPos != previous.startPos)
				{
					stream.WriteByte(18);
					memoryStream.SetLength((long)0);
					Vector3Serialized.SerializeDelta(memoryStream, instance.startPos, previous.startPos);
					uint length = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, length);
					stream.Write(memoryStream.GetBuffer(), 0, (int)length);
				}
				if (instance.startVel != previous.startVel)
				{
					stream.WriteByte(26);
					memoryStream.SetLength((long)0);
					Vector3Serialized.SerializeDelta(memoryStream, instance.startVel, previous.startVel);
					uint num = (uint)memoryStream.Length;
					ProtocolParser.WriteUInt32(stream, num);
					stream.Write(memoryStream.GetBuffer(), 0, (int)num);
				}
				if (instance.seed != previous.seed)
				{
					stream.WriteByte(32);
					ProtocolParser.WriteUInt64(stream, (ulong)instance.seed);
				}
				Pool.FreeMemoryStream(ref memoryStream);
			}

			public static void SerializeLengthDelimited(Stream stream, ProjectileShoot.Projectile instance)
			{
				byte[] bytes = ProjectileShoot.Projectile.SerializeToBytes(instance);
				ProtocolParser.WriteUInt32(stream, (uint)bytes.Length);
				stream.Write(bytes, 0, (int)bytes.Length);
			}

			public static byte[] SerializeToBytes(ProjectileShoot.Projectile instance)
			{
				byte[] array;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					ProjectileShoot.Projectile.Serialize(memoryStream, instance);
					array = memoryStream.ToArray();
				}
				return array;
			}

			public void ToProto(Stream stream)
			{
				ProjectileShoot.Projectile.Serialize(stream, this);
			}

			public byte[] ToProtoBytes()
			{
				return ProjectileShoot.Projectile.SerializeToBytes(this);
			}

			public virtual void WriteToStream(Stream stream)
			{
				ProjectileShoot.Projectile.Serialize(stream, this);
			}

			public virtual void WriteToStreamDelta(Stream stream, ProjectileShoot.Projectile previous)
			{
				if (previous == null)
				{
					ProjectileShoot.Projectile.Serialize(stream, this);
					return;
				}
				ProjectileShoot.Projectile.SerializeDelta(stream, this, previous);
			}
		}
	}
}