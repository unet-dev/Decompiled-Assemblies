using ConVar;
using Facepunch;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EggHuntEvent : BaseEntity
{
	public GameObjectRef[] EggPrefabs;

	public AudioSource eggMusic;

	public AudioSource endingMusic;

	public float warmupTime = 10f;

	public float cooldownTime = 10f;

	public float warnTime = 20f;

	public float timeAlive;

	public static EggHuntEvent serverEvent;

	public static EggHuntEvent clientEvent;

	[NonSerialized]
	public static float durationSeconds;

	private Dictionary<ulong, EggHuntEvent.EggHunter> _eggHunters = new Dictionary<ulong, EggHuntEvent.EggHunter>();

	public List<CollectableEasterEgg> _spawnedEggs = new List<CollectableEasterEgg>();

	public ItemAmount[] placementAwards;

	static EggHuntEvent()
	{
		EggHuntEvent.serverEvent = null;
		EggHuntEvent.clientEvent = null;
		EggHuntEvent.durationSeconds = 180f;
	}

	public EggHuntEvent()
	{
	}

	public void CleanupEggs()
	{
		foreach (CollectableEasterEgg _spawnedEgg in this._spawnedEggs)
		{
			if (_spawnedEgg == null)
			{
				continue;
			}
			_spawnedEgg.Kill(BaseNetworkable.DestroyMode.None);
		}
	}

	public void Cooldown()
	{
		base.CancelInvoke(new Action(this.Cooldown));
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	public override void DestroyShared()
	{
		base.DestroyShared();
		if (base.isServer)
		{
			EggHuntEvent.serverEvent = null;
			return;
		}
		EggHuntEvent.clientEvent = null;
	}

	public void DoNetworkUpdate()
	{
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public void EggCollected(BasePlayer player)
	{
		EggHuntEvent.EggHunter eggHunter = null;
		if (!this._eggHunters.ContainsKey(player.userID))
		{
			eggHunter = new EggHuntEvent.EggHunter()
			{
				displayName = player.displayName,
				userid = player.userID
			};
			this._eggHunters.Add(player.userID, eggHunter);
		}
		else
		{
			eggHunter = this._eggHunters[player.userID];
		}
		if (eggHunter == null)
		{
			Debug.LogWarning("Easter error");
			return;
		}
		eggHunter.numEggs++;
		this.QueueUpdate();
		int num = ((float)Mathf.RoundToInt(player.eggVision) * 0.5f < 1f ? UnityEngine.Random.Range(0, 2) : 1);
		this.SpawnEggsAtPoint(UnityEngine.Random.Range(1 + num, 3 + num), player.transform.position, player.eyes.BodyForward(), 15f, 25f);
	}

	public float GetTimeRemaining()
	{
		float single = EggHuntEvent.durationSeconds - this.timeAlive;
		if (single < 0f)
		{
			single = 0f;
		}
		return single;
	}

	public List<EggHuntEvent.EggHunter> GetTopHunters()
	{
		List<EggHuntEvent.EggHunter> list = Facepunch.Pool.GetList<EggHuntEvent.EggHunter>();
		foreach (KeyValuePair<ulong, EggHuntEvent.EggHunter> _eggHunter in this._eggHunters)
		{
			list.Add(_eggHunter.Value);
		}
		EggHuntEvent.Sort(list);
		return list;
	}

	public bool IsEventActive()
	{
		if (this.timeAlive <= this.warmupTime)
		{
			return false;
		}
		return this.timeAlive - this.warmupTime < EggHuntEvent.durationSeconds;
	}

	public void PrintWinnersAndAward()
	{
		List<EggHuntEvent.EggHunter> topHunters = this.GetTopHunters();
		if (topHunters.Count <= 0)
		{
			Chat.Broadcast("Wow, no one played so no one won.", "", "#eee", (ulong)0);
			return;
		}
		EggHuntEvent.EggHunter item = topHunters[0];
		Chat.Broadcast(string.Concat(new object[] { item.displayName, " is the top bunny with ", item.numEggs, " eggs collected." }), "", "#eee", (ulong)0);
		for (int i = 0; i < topHunters.Count; i++)
		{
			EggHuntEvent.EggHunter eggHunter = topHunters[i];
			BasePlayer basePlayer = BasePlayer.FindByID(eggHunter.userid);
			if (!basePlayer)
			{
				Debug.LogWarning(string.Concat("EggHuntEvent Printwinners could not find player with id :", eggHunter.userid));
			}
			else
			{
				basePlayer.ChatMessage(string.Concat(new object[] { "You placed ", i + 1, " of ", topHunters.Count, " with ", topHunters[i].numEggs, " eggs collected." }));
			}
		}
		for (int j = 0; j < (int)this.placementAwards.Length && j < topHunters.Count; j++)
		{
			BasePlayer basePlayer1 = BasePlayer.FindByID(topHunters[j].userid);
			if (basePlayer1)
			{
				basePlayer1.inventory.GiveItem(ItemManager.Create(this.placementAwards[j].itemDef, (int)this.placementAwards[j].amount, (ulong)0), basePlayer1.inventory.containerMain);
				basePlayer1.ChatMessage(string.Concat(new object[] { "You received ", (int)this.placementAwards[j].amount, "x ", this.placementAwards[j].itemDef.displayName.english, " as an award!" }));
			}
		}
	}

	public void QueueUpdate()
	{
		if (base.IsInvoking(new Action(this.DoNetworkUpdate)))
		{
			return;
		}
		base.Invoke(new Action(this.DoNetworkUpdate), 2f);
	}

	public void RandPickup()
	{
		foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
		{
		}
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.eggHunt = Facepunch.Pool.Get<EggHunt>();
		List<EggHuntEvent.EggHunter> topHunters = this.GetTopHunters();
		info.msg.eggHunt.hunters = Facepunch.Pool.GetList<EggHunt.EggHunter>();
		for (int i = 0; i < Mathf.Min(10, topHunters.Count); i++)
		{
			EggHunt.EggHunter item = Facepunch.Pool.Get<EggHunt.EggHunter>();
			item.displayName = topHunters[i].displayName;
			item.numEggs = topHunters[i].numEggs;
			info.msg.eggHunt.hunters.Add(item);
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (EggHuntEvent.serverEvent && base.isServer)
		{
			EggHuntEvent.serverEvent.Kill(BaseNetworkable.DestroyMode.None);
			EggHuntEvent.serverEvent = null;
		}
		EggHuntEvent.serverEvent = this;
		base.Invoke(new Action(this.StartEvent), this.warmupTime);
	}

	public static void Sort(List<EggHuntEvent.EggHunter> hunterList)
	{
		hunterList.Sort((EggHuntEvent.EggHunter a, EggHuntEvent.EggHunter b) => b.numEggs.CompareTo(a.numEggs));
	}

	[ContextMenu("SpawnDebug")]
	public void SpawnEggs()
	{
		foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
		{
			this.SpawnEggsAtPoint(UnityEngine.Random.Range(6, 8) + Mathf.RoundToInt(basePlayer.eggVision), basePlayer.transform.position, basePlayer.eyes.BodyForward(), 15f, 25f);
		}
	}

	public void SpawnEggsAtPoint(int numEggs, Vector3 pos, Vector3 aimDir, float minDist = 1f, float maxDist = 2f)
	{
		for (int i = 0; i < numEggs; i++)
		{
			Vector3 height = pos;
			if (aimDir != Vector3.zero)
			{
				aimDir = AimConeUtil.GetModifiedAimConeDirection(90f, aimDir, true);
			}
			else
			{
				aimDir = UnityEngine.Random.onUnitSphere;
			}
			height = pos + (Vector3Ex.Direction2D(pos + (aimDir * 10f), pos) * UnityEngine.Random.Range(minDist, maxDist));
			height.y = TerrainMeta.HeightMap.GetHeight(height);
			GameManager gameManager = GameManager.server;
			string eggPrefabs = this.EggPrefabs[UnityEngine.Random.Range(0, (int)this.EggPrefabs.Length)].resourcePath;
			Quaternion quaternion = new Quaternion();
			CollectableEasterEgg collectableEasterEgg = gameManager.CreateEntity(eggPrefabs, height, quaternion, true) as CollectableEasterEgg;
			collectableEasterEgg.Spawn();
			this._spawnedEggs.Add(collectableEasterEgg);
		}
	}

	public void StartEvent()
	{
		this.SpawnEggs();
	}

	public void Update()
	{
		this.timeAlive += UnityEngine.Time.deltaTime;
		if (base.isServer && !base.IsDestroyed)
		{
			if (this.timeAlive - this.warmupTime > EggHuntEvent.durationSeconds - this.warnTime)
			{
				base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
			}
			if (this.timeAlive - this.warmupTime > EggHuntEvent.durationSeconds && !base.IsInvoking(new Action(this.Cooldown)))
			{
				base.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
				this.CleanupEggs();
				this.PrintWinnersAndAward();
				base.Invoke(new Action(this.Cooldown), 10f);
			}
		}
	}

	public class EggHunter
	{
		public ulong userid;

		public string displayName;

		public int numEggs;

		public EggHunter()
		{
		}
	}
}