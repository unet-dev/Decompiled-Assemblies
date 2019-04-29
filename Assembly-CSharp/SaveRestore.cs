using ConVar;
using Facepunch;
using Facepunch.Math;
using Network;
using Oxide.Core;
using ProtoBuf;
using Rust;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

public class SaveRestore : SingletonComponent<SaveRestore>
{
	public static bool IsSaving;

	public bool timedSave = true;

	public int timedSavePause;

	public static DateTime SaveCreatedTime;

	private static MemoryStream SaveBuffer;

	static SaveRestore()
	{
		SaveRestore.IsSaving = false;
		SaveRestore.SaveBuffer = new MemoryStream(33554432);
	}

	public SaveRestore()
	{
	}

	internal static void ClearMapEntities()
	{
		BaseEntity[] baseEntityArray = UnityEngine.Object.FindObjectsOfType<BaseEntity>();
		if (baseEntityArray.Length != 0)
		{
			DebugEx.Log(string.Concat("Destroying ", (int)baseEntityArray.Length, " old entities"), StackTraceLogType.None);
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < (int)baseEntityArray.Length; i++)
			{
				baseEntityArray[i].Kill(BaseNetworkable.DestroyMode.None);
				if (stopwatch.Elapsed.TotalMilliseconds > 2000)
				{
					stopwatch.Reset();
					stopwatch.Start();
					DebugEx.Log(string.Concat(new object[] { "\t", i + 1, " / ", (int)baseEntityArray.Length }), StackTraceLogType.None);
				}
			}
			ItemManager.Heartbeat();
			DebugEx.Log("\tdone.", StackTraceLogType.None);
		}
	}

	private IEnumerator DoAutomatedSave(bool AndWait = false)
	{
		Interface.CallHook("OnServerSave");
		return new SaveRestore.<DoAutomatedSave>d__8(0)
		{
			<>4__this = this,
			AndWait = AndWait
		};
	}

	public static void GetSaveCache()
	{
		BaseEntity[] array = BaseEntity.saveList.ToArray<BaseEntity>();
		if (array.Length != 0)
		{
			DebugEx.Log(string.Concat("Initializing ", (int)array.Length, " entity save caches"), StackTraceLogType.None);
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < (int)array.Length; i++)
			{
				BaseEntity baseEntity = array[i];
				if (baseEntity.IsValid())
				{
					baseEntity.GetSaveCache();
					if (stopwatch.Elapsed.TotalMilliseconds > 2000)
					{
						stopwatch.Reset();
						stopwatch.Start();
						DebugEx.Log(string.Concat(new object[] { "\t", i + 1, " / ", (int)array.Length }), StackTraceLogType.None);
					}
				}
			}
			DebugEx.Log("\tdone.", StackTraceLogType.None);
		}
	}

	public static void InitializeEntityConditionals()
	{
		BuildingBlock[] array = (
			from x in BaseNetworkable.serverEntities
			where x is BuildingBlock
			select x as BuildingBlock).ToArray<BuildingBlock>();
		if (array.Length != 0)
		{
			DebugEx.Log(string.Concat("Initializing ", (int)array.Length, " conditional models"), StackTraceLogType.None);
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < (int)array.Length; i++)
			{
				RCon.Update();
				array[i].UpdateSkin(true);
				if (stopwatch.Elapsed.TotalMilliseconds > 2000)
				{
					stopwatch.Reset();
					stopwatch.Start();
					DebugEx.Log(string.Concat(new object[] { "\t", i + 1, " / ", (int)array.Length }), StackTraceLogType.None);
				}
			}
			DebugEx.Log("\tdone.", StackTraceLogType.None);
		}
	}

	public static void InitializeEntityLinks()
	{
		BaseEntity[] array = (
			from x in BaseNetworkable.serverEntities
			where x is BaseEntity
			select x as BaseEntity).ToArray<BaseEntity>();
		if (array.Length != 0)
		{
			DebugEx.Log(string.Concat("Initializing ", (int)array.Length, " entity links"), StackTraceLogType.None);
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < (int)array.Length; i++)
			{
				RCon.Update();
				array[i].RefreshEntityLinks();
				if (stopwatch.Elapsed.TotalMilliseconds > 2000)
				{
					stopwatch.Reset();
					stopwatch.Start();
					DebugEx.Log(string.Concat(new object[] { "\t", i + 1, " / ", (int)array.Length }), StackTraceLogType.None);
				}
			}
			DebugEx.Log("\tdone.", StackTraceLogType.None);
		}
	}

	public static void InitializeEntitySupports()
	{
		if (!ConVar.Server.stability)
		{
			return;
		}
		StabilityEntity[] array = (
			from x in BaseNetworkable.serverEntities
			where x is StabilityEntity
			select x as StabilityEntity).ToArray<StabilityEntity>();
		if (array.Length != 0)
		{
			DebugEx.Log(string.Concat("Initializing ", (int)array.Length, " stability supports"), StackTraceLogType.None);
			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < (int)array.Length; i++)
			{
				RCon.Update();
				array[i].InitializeSupports();
				if (stopwatch.Elapsed.TotalMilliseconds > 2000)
				{
					stopwatch.Reset();
					stopwatch.Start();
					DebugEx.Log(string.Concat(new object[] { "\t", i + 1, " / ", (int)array.Length }), StackTraceLogType.None);
				}
			}
			DebugEx.Log("\tdone.", StackTraceLogType.None);
		}
	}

	public static bool Load(string strFilename = "", bool allowOutOfDateSaves = false)
	{
		bool flag;
		SaveRestore.SaveCreatedTime = DateTime.UtcNow;
		try
		{
			if (strFilename == "")
			{
				strFilename = string.Concat(World.SaveFolderName, "/", World.SaveFileName);
			}
			if (!File.Exists(strFilename))
			{
				if (!File.Exists(string.Concat("TestSaves/", strFilename)))
				{
					UnityEngine.Debug.LogWarning(string.Concat("Couldn't load ", strFilename, " - file doesn't exist"));
					Interface.CallHook("OnNewSave", strFilename);
					flag = false;
					return flag;
				}
				else
				{
					strFilename = string.Concat("TestSaves/", strFilename);
				}
			}
			Dictionary<BaseEntity, ProtoBuf.Entity> baseEntities = new Dictionary<BaseEntity, ProtoBuf.Entity>();
			using (FileStream fileStream = File.OpenRead(strFilename))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					SaveRestore.SaveCreatedTime = File.GetCreationTime(strFilename);
					if (binaryReader.ReadSByte() != 83 || binaryReader.ReadSByte() != 65 || binaryReader.ReadSByte() != 86 || binaryReader.ReadSByte() != 82)
					{
						UnityEngine.Debug.LogWarning("Invalid save (missing header)");
						flag = false;
						return flag;
					}
					else
					{
						if (binaryReader.PeekChar() == 68)
						{
							binaryReader.ReadChar();
							SaveRestore.SaveCreatedTime = Epoch.ToDateTime(binaryReader.ReadInt32());
						}
						if (binaryReader.ReadUInt32() != 177)
						{
							if (!allowOutOfDateSaves)
							{
								UnityEngine.Debug.LogWarning("This save is from an older version. It might not load properly.");
							}
							else
							{
								UnityEngine.Debug.LogWarning("This save is from an older (possibly incompatible) version!");
							}
						}
						SaveRestore.ClearMapEntities();
						UnityEngine.Assertions.Assert.IsTrue(BaseEntity.saveList.Count == 0, "BaseEntity.saveList isn't empty!");
						Network.Net.sv.Reset();
						Rust.Application.isLoadingSave = true;
						HashSet<uint> nums = new HashSet<uint>();
						while (fileStream.Position < fileStream.Length)
						{
							RCon.Update();
							ProtoBuf.Entity entity = ProtoBuf.Entity.DeserializeLength(fileStream, (int)binaryReader.ReadUInt32());
							if (entity.basePlayer != null && baseEntities.Any<KeyValuePair<BaseEntity, ProtoBuf.Entity>>((KeyValuePair<BaseEntity, ProtoBuf.Entity> x) => {
								if (x.Value.basePlayer == null)
								{
									return false;
								}
								return x.Value.basePlayer.userid == entity.basePlayer.userid;
							}))
							{
								UnityEngine.Debug.LogWarning(string.Concat(new object[] { "Skipping entity ", entity.baseNetworkable.uid, " - it's a player ", entity.basePlayer.userid, " who is in the save multiple times" }));
							}
							else if (entity.baseNetworkable.uid <= 0 || !nums.Contains(entity.baseNetworkable.uid))
							{
								if (entity.baseNetworkable.uid > 0)
								{
									nums.Add(entity.baseNetworkable.uid);
								}
								BaseEntity baseEntity = GameManager.server.CreateEntity(StringPool.Get(entity.baseNetworkable.prefabID), entity.baseEntity.pos, Quaternion.Euler(entity.baseEntity.rot), true);
								if (!baseEntity)
								{
									continue;
								}
								baseEntity.InitLoad(entity.baseNetworkable.uid);
								baseEntities.Add(baseEntity, entity);
							}
							else
							{
								UnityEngine.Debug.LogWarning(string.Concat(new object[] { "Skipping entity ", entity.baseNetworkable.uid, " ", StringPool.Get(entity.baseNetworkable.prefabID), " - uid is used multiple times" }));
							}
						}
					}
				}
			}
			DebugEx.Log(string.Concat("Spawning ", baseEntities.Count, " entities"), StackTraceLogType.None);
			object obj = Interface.CallHook("OnSaveLoad", baseEntities);
			if (obj as bool)
			{
				return (bool)obj;
			}
			BaseNetworkable.LoadInfo value = new BaseNetworkable.LoadInfo()
			{
				fromDisk = true
			};
			Stopwatch stopwatch = Stopwatch.StartNew();
			int num = 0;
			foreach (KeyValuePair<BaseEntity, ProtoBuf.Entity> keyValuePair in baseEntities)
			{
				BaseEntity key = keyValuePair.Key;
				if (key == null)
				{
					continue;
				}
				RCon.Update();
				value.msg = keyValuePair.Value;
				key.Spawn();
				key.Load(value);
				if (!key.IsValid())
				{
					continue;
				}
				num++;
				if (stopwatch.Elapsed.TotalMilliseconds <= 2000)
				{
					continue;
				}
				stopwatch.Reset();
				stopwatch.Start();
				DebugEx.Log(string.Concat(new object[] { "\t", num, " / ", baseEntities.Count }), StackTraceLogType.None);
			}
			foreach (KeyValuePair<BaseEntity, ProtoBuf.Entity> baseEntity1 in baseEntities)
			{
				BaseEntity key1 = baseEntity1.Key;
				if (key1 == null)
				{
					continue;
				}
				RCon.Update();
				if (!key1.IsValid())
				{
					continue;
				}
				key1.PostServerLoad();
			}
			DebugEx.Log("\tdone.", StackTraceLogType.None);
			if (SingletonComponent<SpawnHandler>.Instance)
			{
				DebugEx.Log("Enforcing SpawnPopulation Limits", StackTraceLogType.None);
				SingletonComponent<SpawnHandler>.Instance.EnforceLimits(false);
				DebugEx.Log("\tdone.", StackTraceLogType.None);
			}
			Rust.Application.isLoadingSave = false;
			flag = true;
		}
		catch (Exception exception)
		{
			UnityEngine.Debug.LogWarning(string.Concat("Error loading save (", strFilename, ")"));
			UnityEngine.Debug.LogException(exception);
			flag = false;
		}
		return flag;
	}

	public static IEnumerator Save(string strFilename, bool AndWait = false)
	{
		object obj;
		if (!Rust.Application.isQuitting)
		{
			Stopwatch stopwatch = new Stopwatch();
			Stopwatch stopwatch1 = new Stopwatch();
			Stopwatch stopwatch2 = new Stopwatch();
			int num = 0;
			stopwatch.Start();
			using (TimeWarning timeWarning = TimeWarning.New("SaveCache", (long)100))
			{
				Stopwatch stopwatch3 = Stopwatch.StartNew();
				BaseEntity[] array = BaseEntity.saveList.ToArray<BaseEntity>();
				for (int i = 0; i < (int)array.Length; i++)
				{
					BaseEntity baseEntity = array[i];
					if (!(baseEntity == null) && baseEntity.IsValid())
					{
						try
						{
							baseEntity.GetSaveCache();
						}
						catch (Exception exception)
						{
							UnityEngine.Debug.LogException(exception);
						}
						if (stopwatch3.Elapsed.TotalMilliseconds > 5)
						{
							if (!AndWait)
							{
								yield return CoroutineEx.waitForEndOfFrame;
							}
							stopwatch3.Reset();
							stopwatch3.Start();
						}
					}
				}
				array = null;
				stopwatch3 = null;
			}
			timeWarning = null;
			stopwatch.Stop();
			SaveRestore.SaveBuffer.Position = (long)0;
			SaveRestore.SaveBuffer.SetLength((long)0);
			stopwatch1.Start();
			using (timeWarning = TimeWarning.New("SaveWrite", (long)100))
			{
				BinaryWriter binaryWriter = new BinaryWriter(SaveRestore.SaveBuffer);
				binaryWriter.Write((sbyte)83);
				binaryWriter.Write((sbyte)65);
				binaryWriter.Write((sbyte)86);
				binaryWriter.Write((sbyte)82);
				binaryWriter.Write((sbyte)68);
				binaryWriter.Write(Epoch.FromDateTime(SaveRestore.SaveCreatedTime));
				binaryWriter.Write((uint)177);
				BaseNetworkable.SaveInfo saveInfo = new BaseNetworkable.SaveInfo()
				{
					forDisk = true
				};
				if (!AndWait)
				{
					yield return CoroutineEx.waitForEndOfFrame;
				}
				foreach (BaseEntity baseEntity1 in BaseEntity.saveList)
				{
					if (baseEntity1 == null || baseEntity1.IsDestroyed)
					{
						UnityEngine.Debug.LogWarning(string.Concat("Entity is NULL but is still in saveList - not destroyed properly? ", baseEntity1), baseEntity1);
					}
					else
					{
						MemoryStream saveCache = null;
						try
						{
							saveCache = baseEntity1.GetSaveCache();
						}
						catch (Exception exception1)
						{
							UnityEngine.Debug.LogException(exception1);
						}
						if (saveCache == null || saveCache.Length <= (long)0)
						{
							object[] objArray = new object[] { baseEntity1, null };
							obj = (saveCache == null ? "savecache is null" : "savecache is 0");
							objArray[1] = obj;
							UnityEngine.Debug.LogWarningFormat("Skipping saving entity {0} - because {1}", objArray);
						}
						else
						{
							binaryWriter.Write((uint)saveCache.Length);
							binaryWriter.Write(saveCache.GetBuffer(), 0, (int)saveCache.Length);
							num++;
						}
					}
				}
				binaryWriter = null;
			}
			timeWarning = null;
			stopwatch1.Stop();
			if (!AndWait)
			{
				yield return CoroutineEx.waitForEndOfFrame;
			}
			stopwatch2.Start();
			using (TimeWarning timeWarning1 = TimeWarning.New("SaveDisk", (long)100))
			{
				try
				{
					if (File.Exists(string.Concat(strFilename, ".new")))
					{
						File.Delete(string.Concat(strFilename, ".new"));
					}
					try
					{
						File.WriteAllBytes(string.Concat(strFilename, ".new"), SaveRestore.SaveBuffer.ToArray());
					}
					catch (Exception exception2)
					{
						UnityEngine.Debug.LogWarning(string.Concat("Couldn't write save file! We got an exception: ", exception2));
						if (File.Exists(string.Concat(strFilename, ".new")))
						{
							File.Delete(string.Concat(strFilename, ".new"));
						}
					}
					if (File.Exists(strFilename))
					{
						File.Delete(strFilename);
					}
					File.Move(string.Concat(strFilename, ".new"), strFilename);
				}
				catch (Exception exception3)
				{
					UnityEngine.Debug.LogWarning(string.Concat("Error when saving to disk: ", exception3));
				}
			}
			stopwatch2.Stop();
			object[] str = new object[] { num.ToString("N0"), null, null, null };
			double totalSeconds = stopwatch.Elapsed.TotalSeconds;
			str[1] = totalSeconds.ToString("0.00");
			totalSeconds = stopwatch1.Elapsed.TotalSeconds;
			str[2] = totalSeconds.ToString("0.00");
			totalSeconds = stopwatch2.Elapsed.TotalSeconds;
			str[3] = totalSeconds.ToString("0.00");
			UnityEngine.Debug.LogFormat("Saved {0} ents, cache({1}), write({2}), disk({3}).", str);
		}
		else
		{
		}
	}

	public static bool Save(bool AndWait)
	{
		if (SingletonComponent<SaveRestore>.Instance == null)
		{
			return false;
		}
		if (SaveRestore.IsSaving)
		{
			return false;
		}
		IEnumerator enumerator = SingletonComponent<SaveRestore>.Instance.DoAutomatedSave(true);
		while (enumerator.MoveNext())
		{
		}
		return true;
	}

	private IEnumerator SaveRegularly()
	{
		SaveRestore saveRestore = null;
		while (true)
		{
			yield return CoroutineEx.waitForSeconds((float)ConVar.Server.saveinterval);
			if (saveRestore.timedSave && saveRestore.timedSavePause <= 0)
			{
				yield return saveRestore.StartCoroutine(saveRestore.DoAutomatedSave(false));
			}
		}
	}

	private void Start()
	{
		base.StartCoroutine(this.SaveRegularly());
	}
}