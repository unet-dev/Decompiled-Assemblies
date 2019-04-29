using ConVar;
using Facepunch.Sqlite;
using Ionic.Crc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine.Assertions;

public class FileStorage : IDisposable
{
	private Database db;

	private CRC32 crc = new CRC32();

	private Dictionary<uint, FileStorage.CacheData> _cache = new Dictionary<uint, FileStorage.CacheData>();

	public static FileStorage server;

	static FileStorage()
	{
		FileStorage.server = new FileStorage(string.Concat("sv.files.", 0), true);
	}

	protected FileStorage(string name, bool server)
	{
		if (server)
		{
			string str = string.Concat(Server.rootFolder, "/", name, ".db");
			this.db = new Database();
			this.db.Open(str);
			if (!this.db.TableExists("data"))
			{
				this.db.Execute("CREATE TABLE data ( crc INTEGER PRIMARY KEY, data BLOB, updated INTEGER, entid INTEGER, filetype INTEGER, part INTEGER )", Array.Empty<object>());
			}
		}
	}

	public void Dispose()
	{
		if (this.db != null)
		{
			this.db.Close();
			this.db = null;
		}
	}

	~FileStorage()
	{
		this.Dispose();
	}

	public byte[] Get(uint crc, FileStorage.Type type, uint entityID)
	{
		FileStorage.CacheData cacheDatum;
		byte[] numArray;
		using (TimeWarning timeWarning = TimeWarning.New("FileStorage.Get", 0.1f))
		{
			if (this._cache.TryGetValue(crc, out cacheDatum))
			{
				Assert.IsTrue(cacheDatum.data != null, "FileStorage cache contains a null texture");
				numArray = cacheDatum.data;
			}
			else if (this.db != null)
			{
				byte[] numArray1 = this.db.QueryBlob("SELECT data FROM data WHERE crc = ? AND filetype = ? AND entid = ? LIMIT 1", new object[] { (int)crc, (int)type, (int)entityID });
				if (numArray1 != null)
				{
					this._cache.Remove(crc);
					this._cache.Add(crc, new FileStorage.CacheData()
					{
						data = numArray1,
						entityID = entityID,
						numID = 0
					});
					numArray = numArray1;
				}
				else
				{
					numArray = null;
				}
			}
			else
			{
				numArray = null;
			}
		}
		return numArray;
	}

	private uint GetCRC(byte[] data, FileStorage.Type type)
	{
		this.crc.Reset();
		this.crc.SlurpBlock(data, 0, (int)data.Length);
		this.crc.UpdateCRC((byte)type);
		return (uint)this.crc.Crc32Result;
	}

	public void Remove(uint crc, FileStorage.Type type, uint entityID)
	{
		using (TimeWarning timeWarning = TimeWarning.New("FileStorage.Remove", 0.1f))
		{
			if (this.db != null)
			{
				this.db.Execute("DELETE FROM data WHERE crc = ? AND filetype = ? AND entid = ?", new object[] { (int)crc, (int)type, (int)entityID });
			}
			if (this._cache.ContainsKey(crc))
			{
				this._cache.Remove(crc);
			}
		}
	}

	internal void RemoveAllByEntity(uint entityid)
	{
		using (TimeWarning timeWarning = TimeWarning.New("FileStorage.RemoveAllByEntity", 0.1f))
		{
			if (this.db != null)
			{
				this.db.Execute("DELETE FROM data WHERE entid = ?", new object[] { (int)entityid });
			}
		}
	}

	public void RemoveEntityNum(uint entityid, uint numid)
	{
		Func<KeyValuePair<uint, FileStorage.CacheData>, bool> func = null;
		using (TimeWarning timeWarning = TimeWarning.New("FileStorage.RemoveEntityNum", 0.1f))
		{
			if (this.db != null)
			{
				this.db.Execute("DELETE FROM data WHERE entid = ? AND part = ?", new object[] { (int)entityid, (int)numid });
			}
			Dictionary<uint, FileStorage.CacheData> nums = this._cache;
			Func<KeyValuePair<uint, FileStorage.CacheData>, bool> func1 = func;
			if (func1 == null)
			{
				Func<KeyValuePair<uint, FileStorage.CacheData>, bool> value = (KeyValuePair<uint, FileStorage.CacheData> x) => {
					if (x.Value.entityID != entityid)
					{
						return false;
					}
					return x.Value.numID == numid;
				};
				Func<KeyValuePair<uint, FileStorage.CacheData>, bool> func2 = value;
				func = value;
				func1 = func2;
			}
			uint[] array = nums.Where<KeyValuePair<uint, FileStorage.CacheData>>(func1).Select<KeyValuePair<uint, FileStorage.CacheData>, uint>((KeyValuePair<uint, FileStorage.CacheData> x) => x.Key).ToArray<uint>();
			for (int i = 0; i < (int)array.Length; i++)
			{
				uint num = array[i];
				this._cache.Remove(num);
			}
		}
	}

	public uint Store(byte[] data, FileStorage.Type type, uint entityID, uint numID = 0)
	{
		uint num;
		using (TimeWarning timeWarning = TimeWarning.New("FileStorage.Store", 0.1f))
		{
			uint cRC = this.GetCRC(data, type);
			if (this.db != null)
			{
				this.db.Execute("INSERT OR REPLACE INTO data ( crc, data, entid, filetype, part ) VALUES ( ?, ?, ?, ?, ? )", new object[] { (int)cRC, data, (int)entityID, (int)type, (int)numID });
			}
			this._cache.Remove(cRC);
			this._cache.Add(cRC, new FileStorage.CacheData()
			{
				data = data,
				entityID = entityID,
				numID = numID
			});
			num = cRC;
		}
		return num;
	}

	private class CacheData
	{
		public byte[] data;

		public uint entityID;

		public uint numID;

		public CacheData()
		{
		}
	}

	public enum Type
	{
		png,
		jpg
	}
}