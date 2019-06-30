using LZ4;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public class WorldSerialization
{
	public const uint CurrentVersion = 9;

	public WorldSerialization.WorldData world = new WorldSerialization.WorldData();

	public string Checksum
	{
		get;
		private set;
	}

	public uint Version
	{
		get;
		private set;
	}

	public WorldSerialization()
	{
		this.Version = 9;
		this.Checksum = null;
	}

	public void AddMap(string name, byte[] data)
	{
		WorldSerialization.MapData mapDatum = new WorldSerialization.MapData()
		{
			name = name,
			data = data
		};
		this.world.maps.Add(mapDatum);
	}

	public void AddPath(WorldSerialization.PathData path)
	{
		this.world.paths.Add(path);
	}

	public void AddPrefab(string category, uint id, Vector3 position, Quaternion rotation, Vector3 scale)
	{
		WorldSerialization.PrefabData prefabDatum = new WorldSerialization.PrefabData()
		{
			category = category,
			id = id,
			position = position,
			rotation = rotation,
			scale = scale
		};
		this.world.prefabs.Add(prefabDatum);
	}

	public void CalculateChecksum()
	{
		this.Checksum = this.Hash();
	}

	public void Clear()
	{
		this.world.maps.Clear();
		this.world.prefabs.Clear();
		this.world.paths.Clear();
		this.Version = 9;
		this.Checksum = null;
	}

	public WorldSerialization.MapData GetMap(string name)
	{
		for (int i = 0; i < this.world.maps.Count; i++)
		{
			if (this.world.maps[i].name == name)
			{
				return this.world.maps[i];
			}
		}
		return null;
	}

	public WorldSerialization.PathData GetPath(string name)
	{
		for (int i = 0; i < this.world.paths.Count; i++)
		{
			if (this.world.paths[i].name == name)
			{
				return this.world.paths[i];
			}
		}
		return null;
	}

	public IEnumerable<WorldSerialization.PathData> GetPaths(string name)
	{
		return 
			from p in this.world.paths
			where p.name.Contains(name)
			select p;
	}

	public IEnumerable<WorldSerialization.PrefabData> GetPrefabs(string category)
	{
		return 
			from p in this.world.prefabs
			where p.category == category
			select p;
	}

	private string Hash()
	{
		Checksum checksum = new Checksum();
		WorldSerialization.MapData map = this.GetMap("terrain");
		if (map != null)
		{
			for (int i = 0; i < (int)map.data.Length; i++)
			{
				checksum.Add(map.data[i]);
			}
		}
		List<WorldSerialization.PrefabData> prefabDatas = this.world.prefabs;
		if (prefabDatas != null)
		{
			for (int j = 0; j < prefabDatas.Count; j++)
			{
				WorldSerialization.PrefabData item = prefabDatas[j];
				checksum.Add(item.id);
				checksum.Add(item.position.x, 3);
				checksum.Add(item.position.y, 3);
				checksum.Add(item.position.z, 3);
				checksum.Add(item.scale.x, 3);
				checksum.Add(item.scale.y, 3);
				checksum.Add(item.scale.z, 3);
			}
		}
		return checksum.MD5();
	}

	public void Load(string fileName)
	{
		try
		{
			using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					this.Version = binaryReader.ReadUInt32();
					if (this.Version == 9)
					{
						using (LZ4Stream lZ4Stream = new LZ4Stream(fileStream, LZ4StreamMode.Decompress, LZ4StreamFlags.None, 1048576))
						{
							this.world = Serializer.Deserialize<WorldSerialization.WorldData>(lZ4Stream);
						}
					}
				}
			}
			this.Checksum = this.Hash();
		}
		catch (Exception exception)
		{
			Debug.LogError(exception.Message);
		}
	}

	public void Save(string fileName)
	{
		try
		{
			using (FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
				{
					binaryWriter.Write(this.Version);
					using (LZ4Stream lZ4Stream = new LZ4Stream(fileStream, LZ4StreamMode.Compress, LZ4StreamFlags.None, 1048576))
					{
						Serializer.Serialize<WorldSerialization.WorldData>(lZ4Stream, this.world);
					}
				}
			}
			this.Checksum = this.Hash();
		}
		catch (Exception exception)
		{
			Debug.LogError(exception.Message);
		}
	}

	[ProtoContract]
	public class MapData
	{
		[ProtoMember(1)]
		public string name;

		[ProtoMember(2)]
		public byte[] data;

		public MapData()
		{
		}
	}

	[ProtoContract]
	public class PathData
	{
		[ProtoMember(1)]
		public string name;

		[ProtoMember(2)]
		public bool spline;

		[ProtoMember(3)]
		public bool start;

		[ProtoMember(4)]
		public bool end;

		[ProtoMember(5)]
		public float width;

		[ProtoMember(6)]
		public float innerPadding;

		[ProtoMember(7)]
		public float outerPadding;

		[ProtoMember(8)]
		public float innerFade;

		[ProtoMember(9)]
		public float outerFade;

		[ProtoMember(10)]
		public float randomScale;

		[ProtoMember(11)]
		public float meshOffset;

		[ProtoMember(12)]
		public float terrainOffset;

		[ProtoMember(13)]
		public int splat;

		[ProtoMember(14)]
		public int topology;

		[ProtoMember(15)]
		public WorldSerialization.VectorData[] nodes;

		public PathData()
		{
		}
	}

	[ProtoContract]
	public class PrefabData
	{
		[ProtoMember(1)]
		public string category;

		[ProtoMember(2)]
		public uint id;

		[ProtoMember(3)]
		public WorldSerialization.VectorData position;

		[ProtoMember(4)]
		public WorldSerialization.VectorData rotation;

		[ProtoMember(5)]
		public WorldSerialization.VectorData scale;

		public PrefabData()
		{
		}
	}

	[ProtoContract]
	public class VectorData
	{
		[ProtoMember(1)]
		public float x;

		[ProtoMember(2)]
		public float y;

		[ProtoMember(3)]
		public float z;

		public VectorData()
		{
		}

		public VectorData(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public static implicit operator VectorData(Vector3 v)
		{
			return new WorldSerialization.VectorData(v.x, v.y, v.z);
		}

		public static implicit operator VectorData(Quaternion q)
		{
			return q.eulerAngles;
		}

		public static implicit operator Vector3(WorldSerialization.VectorData v)
		{
			return new Vector3(v.x, v.y, v.z);
		}

		public static implicit operator Quaternion(WorldSerialization.VectorData v)
		{
			return Quaternion.Euler(v);
		}
	}

	[ProtoContract]
	public class WorldData
	{
		[ProtoMember(1)]
		public uint size;

		[ProtoMember(2)]
		public List<WorldSerialization.MapData> maps;

		[ProtoMember(3)]
		public List<WorldSerialization.PrefabData> prefabs;

		[ProtoMember(4)]
		public List<WorldSerialization.PathData> paths;

		public WorldData()
		{
		}
	}
}