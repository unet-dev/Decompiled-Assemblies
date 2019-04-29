using Facepunch.Math;
using Facepunch.Sqlite;
using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UserPersistance : IDisposable
{
	public static Database blueprints;

	public static Database deaths;

	public UserPersistance(string strFolder)
	{
		UserPersistance.blueprints = new Database();
		UserPersistance.blueprints.Open(string.Concat(new object[] { strFolder, "/player.blueprints.", 3, ".db" }));
		if (!UserPersistance.blueprints.TableExists("data"))
		{
			UserPersistance.blueprints.Execute("CREATE TABLE data ( userid TEXT PRIMARY KEY, info BLOB, updated INTEGER )", Array.Empty<object>());
		}
		UserPersistance.deaths = new Database();
		UserPersistance.deaths.Open(string.Concat(new object[] { strFolder, "/player.deaths.", 3, ".db" }));
		if (!UserPersistance.deaths.TableExists("data"))
		{
			UserPersistance.deaths.Execute("CREATE TABLE data ( userid TEXT, born INTEGER, died INTEGER, info BLOB )", Array.Empty<object>());
			UserPersistance.deaths.Execute("CREATE INDEX IF NOT EXISTS userindex ON data ( userid )", Array.Empty<object>());
			UserPersistance.deaths.Execute("CREATE INDEX IF NOT EXISTS diedindex ON data ( died )", Array.Empty<object>());
		}
	}

	public void AddLifeStory(ulong playerID, PlayerLifeStory lifeStory)
	{
		byte[] protoBytes;
		if (UserPersistance.deaths == null)
		{
			return;
		}
		if (lifeStory == null)
		{
			return;
		}
		using (TimeWarning timeWarning = TimeWarning.New("AddLifeStory", 0.1f))
		{
			using (TimeWarning timeWarning1 = TimeWarning.New("ToProtoBytes", 0.1f))
			{
				protoBytes = lifeStory.ToProtoBytes();
			}
			UserPersistance.deaths.Execute("INSERT INTO data ( userid, born, died, info ) VALUES ( ?, ?, ?, ? )", new object[] { playerID.ToString(), (int)lifeStory.timeBorn, (int)lifeStory.timeDied, protoBytes });
		}
	}

	public virtual void Dispose()
	{
		if (UserPersistance.blueprints != null)
		{
			UserPersistance.blueprints.Close();
			UserPersistance.blueprints = null;
		}
		if (UserPersistance.deaths != null)
		{
			UserPersistance.deaths.Close();
			UserPersistance.deaths = null;
		}
	}

	private PersistantPlayer FetchFromDatabase(ulong playerID)
	{
		try
		{
			Row row = UserPersistance.blueprints.QueryRow("SELECT info FROM data WHERE userid = ?", new object[] { playerID.ToString() });
			if (row != null)
			{
				return PersistantPlayer.Deserialize(row.GetBlob("info"));
			}
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			Debug.LogError(string.Concat("Error loading player blueprints: (", exception.Message, ")"));
		}
		return null;
	}

	public PlayerLifeStory GetLastLifeStory(ulong playerID)
	{
		PlayerLifeStory playerLifeStory;
		if (UserPersistance.deaths == null)
		{
			return null;
		}
		using (TimeWarning timeWarning = TimeWarning.New("GetLastLifeStory", 0.1f))
		{
			try
			{
				byte[] numArray = UserPersistance.deaths.QueryBlob("SELECT info FROM data WHERE userid = ? ORDER BY died DESC LIMIT 1", new object[] { playerID.ToString() });
				if (numArray != null)
				{
					PlayerLifeStory playerLifeStory1 = PlayerLifeStory.Deserialize(numArray);
					playerLifeStory1.ShouldPool = false;
					playerLifeStory = playerLifeStory1;
					return playerLifeStory;
				}
				else
				{
					playerLifeStory = null;
					return playerLifeStory;
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Debug.LogError(string.Concat("Error loading lifestory from database: (", exception.Message, ")"));
			}
			playerLifeStory = null;
		}
		return playerLifeStory;
	}

	public PersistantPlayer GetPlayerInfo(ulong playerID)
	{
		PersistantPlayer nums = this.FetchFromDatabase(playerID) ?? new PersistantPlayer();
		nums.ShouldPool = false;
		if (nums.unlockedItems == null)
		{
			nums.unlockedItems = new List<int>();
		}
		return nums;
	}

	public void SetPlayerInfo(ulong playerID, PersistantPlayer info)
	{
		byte[] protoBytes;
		using (TimeWarning timeWarning = TimeWarning.New("SetPlayerInfo", 0.1f))
		{
			using (TimeWarning timeWarning1 = TimeWarning.New("ToProtoBytes", 0.1f))
			{
				protoBytes = info.ToProtoBytes();
			}
			UserPersistance.blueprints.Execute("INSERT OR REPLACE INTO data ( userid, info, updated ) VALUES ( ?, ?, ? )", new object[] { playerID.ToString(), protoBytes, Epoch.Current });
		}
	}
}