using Facepunch;
using Network;
using Rust.Ai.HTN.ScientistJunkpile;
using System;
using System.Collections.Generic;
using UnityEngine;

public class JunkPile : BaseEntity
{
	public GameObjectRef sinkEffect;

	public SpawnGroup[] spawngroups;

	public ScientistJunkpileSpawner npcSpawnGroup;

	private const float lifetimeMinutes = 30f;

	private List<NPCPlayerApex> _npcs;

	private List<HTNPlayer> _htnPlayers;

	protected bool isSinking;

	public JunkPile()
	{
	}

	public void AddNpc(NPCPlayerApex npc)
	{
		if (this._npcs == null)
		{
			this._npcs = new List<NPCPlayerApex>(1);
		}
		this._npcs.Add(npc);
	}

	public void AddNpc(HTNPlayer npc)
	{
		if (this._htnPlayers == null)
		{
			this._htnPlayers = new List<HTNPlayer>();
		}
		this._htnPlayers.Add(npc);
	}

	public void CheckEmpty()
	{
		if (this.SpawnGroupsEmpty())
		{
			base.CancelInvoke(new Action(this.CheckEmpty));
			this.SinkAndDestroy();
		}
	}

	public void KillMe()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning timeWarning = TimeWarning.New("JunkPile.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.TimeOut), 1800f);
		base.InvokeRepeating(new Action(this.CheckEmpty), 10f, 30f);
		base.Invoke(new Action(this.SpawnInitial), 1f);
		this.isSinking = false;
	}

	public void SinkAndDestroy()
	{
		base.CancelInvoke(new Action(this.SinkAndDestroy));
		SpawnGroup[] spawnGroupArray = this.spawngroups;
		for (int i = 0; i < (int)spawnGroupArray.Length; i++)
		{
			spawnGroupArray[i].Clear();
		}
		ScientistJunkpileSpawner scientistJunkpileSpawner = this.npcSpawnGroup;
		if (scientistJunkpileSpawner != null)
		{
			scientistJunkpileSpawner.Clear();
		}
		else
		{
		}
		base.ClientRPC(null, "CLIENT_StartSink");
		Transform vector3 = base.transform;
		vector3.position = vector3.position - new Vector3(0f, 5f, 0f);
		this.isSinking = true;
		base.Invoke(new Action(this.KillMe), 22f);
	}

	public bool SpawnGroupsEmpty()
	{
		bool flag;
		SpawnGroup[] spawnGroupArray = this.spawngroups;
		for (int i = 0; i < (int)spawnGroupArray.Length; i++)
		{
			if (spawnGroupArray[i].currentPopulation > 0)
			{
				return false;
			}
		}
		ScientistJunkpileSpawner scientistJunkpileSpawner = this.npcSpawnGroup;
		if (scientistJunkpileSpawner != null)
		{
			flag = scientistJunkpileSpawner.currentPopulation > 0;
		}
		else
		{
			flag = false;
		}
		if (flag)
		{
			return false;
		}
		return true;
	}

	private void SpawnInitial()
	{
		SpawnGroup[] spawnGroupArray = this.spawngroups;
		for (int i = 0; i < (int)spawnGroupArray.Length; i++)
		{
			spawnGroupArray[i].SpawnInitial();
		}
		ScientistJunkpileSpawner scientistJunkpileSpawner = this.npcSpawnGroup;
		if (scientistJunkpileSpawner == null)
		{
			return;
		}
		scientistJunkpileSpawner.SpawnInitial();
	}

	public void TimeOut()
	{
		if (this.SpawnGroupsEmpty())
		{
			this.SinkAndDestroy();
			return;
		}
		List<BasePlayer> list = Pool.GetList<BasePlayer>();
		Vis.Entities<BasePlayer>(base.transform.position, this.TimeoutPlayerCheckRadius(), list, 131072, QueryTriggerInteraction.Collide);
		bool flag = false;
		foreach (BasePlayer basePlayer in list)
		{
			if (basePlayer.IsSleeping() || !basePlayer.IsAlive())
			{
				continue;
			}
			flag = true;
			goto Label0;
		}
	Label0:
		if (!flag)
		{
			if (this._npcs != null)
			{
				foreach (NPCPlayerApex _npc in this._npcs)
				{
					if (_npc == null || _npc.transform == null || _npc.IsDestroyed || _npc.IsDead())
					{
						continue;
					}
					_npc.Kill(BaseNetworkable.DestroyMode.None);
				}
				this._npcs.Clear();
			}
			if (this._htnPlayers != null)
			{
				foreach (HTNPlayer _htnPlayer in this._htnPlayers)
				{
					if (_htnPlayer == null || _htnPlayer.transform == null || _htnPlayer.IsDestroyed || _htnPlayer.IsDead())
					{
						continue;
					}
					_htnPlayer.Kill(BaseNetworkable.DestroyMode.None);
				}
				this._htnPlayers.Clear();
			}
			this.SinkAndDestroy();
		}
		else
		{
			base.Invoke(new Action(this.TimeOut), 300f);
		}
		Pool.FreeList<BasePlayer>(ref list);
	}

	public virtual float TimeoutPlayerCheckRadius()
	{
		return 15f;
	}
}